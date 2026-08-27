using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Steamworks;
using Steamworks.Data;
using ZP2K9.platform;

namespace ZP2K9.net;

// Third INetworkSessionFactory backend (see the roadmap comment at the top of
// INetworkSession.cs): real internet multiplayer via Steam's relay network
// (SDR - Steam Datagram Relay), using the Facepunch.Steamworks wrapper around
// the native Steamworks SDK. Chosen over a self-hosted relay or manual port
// forwarding because SDR does automatic NAT traversal for both sides with no
// server of our own to run and no router configuration for players - see the
// 21st-round project-memory entry for the tradeoffs discussion this followed.
//
// Prototyping AppID: this currently initializes against 480 ("Spacewar"),
// Valve's public test AppID, which is explicitly sanctioned for free
// pre-release prototyping of exactly this kind of feature - no $100
// Steamworks fee needed until this is ready to actually ship on Steam (that
// fee is refunded once a real app earns $1,000 there anyway). Both players
// need Steam running and logged in for any of this to work - unlike LAN,
// there's no offline fallback. See SteamInit.AppId below for where a real
// AppID would eventually replace 480.
//
// Topology: same star shape as LanNetworkSession (host<->each client,
// client<->client traffic relayed through the host) and the same wire
// protocol semantics (Hello/Welcome/GamerJoined/GamerLeft/Data/Full - see
// SteamMsg below, deliberately kept parallel to LanMsg). The transport is
// completely different underneath: instead of a raw TCP byte-stream needing
// manual length-prefixed frame reassembly (LanFrame/LanConnection.Pump()),
// Steam's relay `Connection`/`SocketManager`/`ConnectionManager` deliver one
// complete message per SendMessage() call - so a "frame" here is just
// [byte SteamMsg tag][body...], no length prefix, no reassembly (SteamFrame
// below). Discovery is a Steam Lobby instead of a LAN UDP broadcast -
// BeginFind queries public lobbies tagged as this game instead of shouting on
// the local subnet, and BeginJoin connects straight to the host's SteamId
// once a lobby join succeeds.
//
// Threading: kept exactly as single-threaded as LanNetworkSession, which
// matters even more here since Steam's callback system genuinely can run on
// a background thread if asked to (SteamClient.Init(..., asyncCallbacks:
// true) spawns one). This file deliberately uses asyncCallbacks: false (see
// SteamInit.EnsureInitialized) so that EVERY Steam callback - Task
// completions for CreateLobbyAsync/RequestAsync/Join(), and the
// OnConnecting/OnConnected/OnDisconnected/OnMessage callbacks below - only
// ever fires synchronously from inside SteamClient.RunCallbacks(), which is
// only ever called from the main thread (SteamInit.Pump(), called at the top
// of every poll/Update method in this file). Nothing here spawns a thread of
// its own or touches shared state from more than one thread.
//
// IMPORTANT for whoever debugs the first build of this: same warning as
// LanNetworkSession.cs - an IAsyncResult's IsCompleted getter must never
// throw. This reuses LanOpResult (see LanNetworkSession.cs) rather than
// duplicating that lazy-poll wrapper, since its "capture into .Error instead
// of throwing" logic is completely generic and already proven.
//
// Also same standing caveat as every other networking file in this project:
// there is no .NET SDK or NuGet access anywhere in the environment this was
// written in, so nothing below has been compiled, let alone run against a
// real two-machine Steam session. The exact Facepunch.Steamworks API shapes
// used here (Connection as a struct, SocketManager.Receive() living on the
// manager rather than a connection, ConnectionManager.Connection being a
// public field, the Lobby/LobbyQuery fluent API) were gathered from the
// library's real source and confirmed with high confidence, but a first
// build's compiler errors are the fastest way to fix anything still wrong.

internal enum SteamMsg : byte
{
    // Client -> host, immediately once the relay connection to the host is
    // confirmed connected.
    Hello = 1,
    // Host -> the new client only: its assigned gamer id, plus a roster
    // snapshot of every gamer already in the session (host included).
    Welcome = 2,
    // Host -> every OTHER already-connected client, when a genuinely new
    // client finishes the Hello handshake.
    GamerJoined = 3,
    // Host -> every remaining client, when any gamer disconnects.
    GamerLeft = 4,
    // Both directions: an application data payload (the bytes out of a
    // PacketWriter). Client -> host omits the sender id (implicit from which
    // connection it arrived on); host -> client includes it, since it might
    // be relayed from a different client.
    Data = 5,
    // Host -> a still-pending connection that Hello'd into a session that's
    // already at its gamer cap. The host closes the connection right after
    // sending this.
    Full = 6
}

// Lobby data keys and small shared constants used by the factory below.
internal static class SteamNetConfig
{
    // AppID 480 is Valve's shared public test AppID ("Spacewar") - lots of
    // other prototypes use it too, so without a distinguishing lobby-data
    // key every unrelated test lobby using 480 would show up in BeginFind's
    // results alongside real ZP2KX games. GameTag/GameTagValue is that
    // distinguishing marker; VersionKey/GameTypeKey carry the same
    // information LAN's discovery response carries in NetworkSessionProperties
    // slots 0/1 (protocol version, GameState.gameType - see NetSession.cs).
    public const string GameTag = "zp2kx";
    public const string GameTagValue = "1";
    public const string VersionKey = "zp2kx_ver";
    public const string GameTypeKey = "zp2kx_gametype";
    public const string HostNameKey = "zp2kx_host";

    public const double ConnectTimeoutSeconds = 8.0;

    // Sentinel target id for a Data message meant for every other gamer -
    // same convention and same value as LanConfig.BroadcastId.
    public const byte BroadcastId = 255;

    public const int MaxLobbyResults = 50;

    // How many messages to drain per Receive() call, per Update()/poll tick.
    // Small on purpose (matches typical Facepunch sample usage) - this is
    // called every frame, not once, so it doesn't need to drain everything
    // in a single call.
    public const int ReceiveBatchSize = 64;
}

// One-time Steam API bring-up, shared by every BeginCreate/BeginFind/BeginJoin
// call below. Kept separate from SteamNetworkSessionFactory itself so a
// failed or not-yet-attempted init doesn't need to be repeated per call site.
internal static class SteamInit
{
    // Valve's public, permanently-free "Spacewar" test AppID - see the
    // header comment at the top of this file for why this is safe to
    // prototype against and what changes for a real release.
    public const uint AppId = 480;

    private static bool _initialized;
    private static bool _initFailed;
    private static string _initError;
    private static bool _inviteListenerRegistered;

    public static bool EnsureInitialized(out string error)
    {
        error = null;
        if (_initialized)
        {
            return true;
        }
        if (_initFailed)
        {
            error = _initError;
            return false;
        }
        try
        {
            // asyncCallbacks: false is deliberate - see the threading section
            // of the header comment at the top of this file. Every Steam
            // callback (including Task completions) only ever fires from
            // inside RunCallbacks(), which this file only ever calls from
            // the main thread via Pump() below.
            SteamClient.Init(AppId, asyncCallbacks: false);
            try
            {
                // Kicks off SDR relay ping measurement ahead of time, so the
                // first CreateRelaySocket/ConnectRelay call isn't starting
                // that work cold. Not fatal if this particular call doesn't
                // exist/fails on some SDK version - relay sockets still work
                // without it, just possibly slower on the very first attempt.
                SteamNetworkingUtils.InitRelayNetworkAccess();
            }
            catch (Exception ex)
            {
                // TEMP DIAGNOSTIC, see OnConnecting above.
                Console.WriteLine("[Steam] InitRelayNetworkAccess threw (non-fatal): " + ex.Message);
            }
            _initialized = true;
            EnsureInviteListenerRegistered();
            return true;
        }
        catch (Exception ex)
        {
            // TEMP DIAGNOSTIC (2026-08-23): this catch runs BEFORE any
            // LanOpResult/poll loop exists, so the centralized
            // "[LanOpResult] poll() threw ..." logger added elsewhere in
            // this file never sees a failure that happens here - this is
            // the actual root-cause exception SteamClient.Init() threw,
            // logged directly since it would otherwise only ever surface
            // as a generic wrapped InvalidOperationException downstream.
            Console.WriteLine("[Steam] EnsureInitialized: SteamClient.Init threw " + ex.GetType().Name + ": " + ex.Message);
            _initFailed = true;
            _initError = "Couldn't start Steam networking: " + ex.Message + " (make sure Steam is running and you're logged in).";
            error = _initError;
            return false;
        }
    }

    // Pumps every pending Steam callback - connection state changes AND Task
    // completions for the async lobby/matchmaking calls below - on whatever
    // thread calls this. Every poll function and Update() method in this
    // file calls this first, and only from the main thread, so nothing here
    // ever observes Steam callback state changing mid-frame.
    public static void Pump()
    {
        if (!_initialized)
        {
            return;
        }
        try
        {
            SteamClient.RunCallbacks();
        }
        catch (Exception ex)
        {
            // TEMP DIAGNOSTIC (2026-08-24, "joiner never gets MSG_INIT over
            // WAN" investigation): this used to be a bare `catch { }` that
            // silently ate ANY exception thrown from anywhere in the entire
            // synchronous Steam callback dispatch chain - OnMessage ->
            // HostMessageReceived -> CompleteJoin -> GamerJoined.Invoke ->
            // NetSession.netSession_GamerJoined, which is exactly the code
            // that builds and sends MSG_INIT. The last WAN test's full
            // session log stopped dead right after "[Host]
            // netSession_GamerJoined: computed freeSlot=1 ..." with zero
            // further output on either machine - consistent with something
            // in that call chain throwing here and vanishing without a
            // trace. Logging ex.ToString() (not just ex.Message) so the
            // stack trace pinpoints exactly which line threw next time this
            // reproduces. Safe to remove once understood.
            Console.WriteLine("[Steam] Pump: RunCallbacks threw " + ex.GetType().Name + ": " + ex);
        }
    }

    // Registered exactly once, the first time Steam successfully
    // initializes - independent of whether the local player is hosting,
    // browsing, or just sitting at the main menu, since a friend's invite
    // notification (or a cold "Join Game" launch) can arrive at any time
    // Steam is running. This is the missing half of the invite flow that
    // Game1.cs's HandleInvite/FinishHandleInvite and NetSession.JoinInvite
    // already existed as inert scaffolding for (see their own comments) -
    // nothing ever raised NetworkBackend.InviteAccepted before this.
    private static void EnsureInviteListenerRegistered()
    {
        if (_inviteListenerRegistered)
        {
            return;
        }
        _inviteListenerRegistered = true;
        SteamFriends.OnGameLobbyJoinRequested += OnGameLobbyJoinRequested;
    }

    // Fired when the local player accepts a Steam invite/notification or
    // clicks "Join Game" for a lobby - either while this game is already
    // running, or (once Steam launches it with a +connect_lobby command
    // line for a cold start) shortly after SteamClient.Init completes above.
    // Steam does NOT auto-join the lobby just from this callback - the game
    // has to do that itself, which is exactly what
    // NetSession.JoinInvite -> INetworkSessionFactory.BeginJoinInvite ->
    // SteamNetworkSessionFactory.BeginJoinLobby (below) does once this
    // reaches there via NetworkBackend.InviteAccepted.
    private static void OnGameLobbyJoinRequested(Lobby lobby, SteamId friendId)
    {
        Console.WriteLine("[Steam] OnGameLobbyJoinRequested: lobby=" + lobby.Id + ", invited by friend=" + friendId + ".");
        NetworkBackend.RaiseInviteAccepted(new InviteAcceptedEventArgs
        {
            Gamer = Gamer.SignedInGamers.Count > 0 ? Gamer.SignedInGamers[0] : null,
            LobbyToken = lobby
        });
    }
}

// Builds one Steam relay message: [byte SteamMsg tag][body...]. Unlike
// LanFrame (used over a raw TCP byte-stream, so it needs a 4-byte length
// prefix to know where one frame ends and the next begins), Steam's relay
// messages are already whole - SocketManager/ConnectionManager deliver one
// complete message per OnMessage callback - so no length prefix and no
// reassembly is needed here at all.
internal static class SteamFrame
{
    public static byte[] Build(SteamMsg type, Action<BinaryWriter> writeBody)
    {
        using MemoryStream body = new MemoryStream();
        using (BinaryWriter bw = new BinaryWriter(body, System.Text.Encoding.UTF8, leaveOpen: true))
        {
            bw.Write((byte)type);
            writeBody?.Invoke(bw);
        }
        return body.ToArray();
    }
}

// A remote gamer (host's view of a client, or a client's view of anyone
// other than itself). Pure data, same shape as LanNetworkGamer - all the
// actual send/receive routing lives on SteamNetworkSession/SteamLocalGamer.
internal sealed class SteamNetworkGamer : INetworkGamer
{
    public byte Id { get; }
    public string Gamertag { get; }
    public bool IsHost => Id == 0;
    public bool IsTalking => false;
    public TimeSpan RoundtripTime => TimeSpan.Zero;

    public SteamNetworkGamer(byte id, string gamertag)
    {
        Id = id;
        Gamertag = gamertag;
    }
}

// The local player's own gamer object. Same id convention as LAN: the host's
// local gamer is always id 0; clients are assigned 1, 2, 3... by the host as
// they join.
internal sealed class SteamLocalGamer : ILocalNetworkGamer
{
    public byte Id { get; }
    public string Gamertag { get; }
    public bool IsHost => Id == 0;
    public bool IsTalking => false;
    public TimeSpan RoundtripTime => TimeSpan.Zero;

    public bool IsDataAvailable => Inbox.Count > 0;

    internal readonly Queue<(byte SenderId, byte[] Data)> Inbox = new Queue<(byte, byte[])>();

    private readonly SteamNetworkSession _session;

    internal SteamLocalGamer(SteamNetworkSession session, byte id, string gamertag)
    {
        _session = session;
        Id = id;
        Gamertag = gamertag;
    }

    public void SendData(PacketWriter writer, SendDataOptions options, INetworkGamer recipient)
    {
        _session.SendData(this, writer.ToArray(), recipient);
    }

    public void ReceiveData(PacketReader reader, out INetworkGamer sender)
    {
        sender = null;
        if (!Inbox.TryDequeue(out (byte SenderId, byte[] Data) item))
        {
            return;
        }
        reader.LoadFrom(item.Data, 0, item.Data.Length);
        sender = _session.FindGamer(item.SenderId);
    }
}

// One entry in a BeginFind() result list - a public Steam lobby tagged as a
// ZP2KX game. Lobby (not part of IAvailableNetworkSession) is what BeginJoin
// actually joins/connects to.
internal sealed class SteamAvailableNetworkSession : IAvailableNetworkSession
{
    public string HostGamertag { get; }
    public int CurrentGamerCount { get; }
    public int OpenPublicGamerSlots { get; }
    public NetworkSessionProperties SessionProperties { get; }
    public NetworkQualityOfService QualityOfService { get; }

    internal Lobby Lobby { get; }

    internal SteamAvailableNetworkSession(string hostGamertag, int currentGamerCount, int openPublicGamerSlots, NetworkSessionProperties properties, Lobby lobby)
    {
        HostGamertag = hostGamertag;
        CurrentGamerCount = currentGamerCount;
        OpenPublicGamerSlots = openPublicGamerSlots;
        SessionProperties = properties;
        QualityOfService = new NetworkQualityOfService { IsAvailable = true, AverageRoundtripTime = TimeSpan.Zero };
        Lobby = lobby;
    }
}

// Host-side relay listener - created once per hosted session by
// SteamNetworkSessionFactory.BeginCreate via
// SteamNetworkingSockets.CreateRelaySocket<SteamHostSocketManager>(). Steam's
// relay (SDR) handles all the NAT traversal automatically, so unlike
// LanNetworkSession there's no separate discovery listener socket here -
// BeginFind below finds hosts through a Steam Lobby instead of a UDP
// broadcast.
internal sealed class SteamHostSocketManager : SocketManager
{
    // Set by SteamNetworkSession.CreateHostSide right after both are
    // constructed together in BeginCreate - null only for the brief window
    // between CreateRelaySocket<T>() returning and that assignment, during
    // which no callback below can fire yet (nothing has connected to a
    // socket nobody else knows the address of).
    public SteamNetworkSession Session;

    public override void OnConnecting(Connection connection, ConnectionInfo info)
    {
        Console.WriteLine("[Steam host] OnConnecting fired...");
        base.OnConnecting(connection, info); // Let the base accept and track it
    }

    public override void OnConnected(Connection connection, ConnectionInfo info)
    {
        base.OnConnected(connection, info); // Adds to the base Connected hashset
        Session?.HostConnectionOpened(connection);
    }

    public override void OnDisconnected(Connection connection, ConnectionInfo info)
    {
        base.OnDisconnected(connection, info);
        Session?.HostConnectionClosed(connection);
    }

    public override void OnMessage(Connection connection, NetIdentity identity, IntPtr data, int size, long messageNum, long recvTime, int channel)
    {
        byte[] payload = new byte[size];
        if (size > 0)
        {
            Marshal.Copy(data, payload, 0, size);
        }
        Session?.HostMessageReceived(connection, payload);
    }
}

// The client-side counterpart - a single relay connection to the host,
// created by SteamNetworkingSockets.ConnectRelay<SteamClientConnectionManager>()
// in SteamNetworkSessionFactory.BeginJoin. Also doubles as where BeginJoin's
// handshake-polling code looks for the Welcome/Full reply BEFORE a
// SteamNetworkSession even exists to hand messages to (Session stays null for
// that whole window - see the Connected/Disconnected/PendingWelcome/GotFull
// fields below, and the matching comment on LanOpResult in
// LanNetworkSession.cs about why none of this can throw from a callback).
internal sealed class SteamClientConnectionManager : ConnectionManager
{
    // Set by SteamNetworkSession.CreateClientSide once BeginJoin's handshake
    // finishes and a real session object exists. Until then, OnMessage below
    // falls back to populating the handshake-only fields underneath instead.
    public SteamNetworkSession Session;

    public bool Disconnected;
    public byte[] PendingWelcome;
    public bool GotFull;

    // BUG FIX (2026-08-24, "joiner never gets MSG_INIT over WAN"
    // investigation): see the long comment on the `default:` case in
    // OnMessage below - this is where messages that arrive in that window
    // are now kept instead of discarded.
    public List<byte[]> PendingMessages = new List<byte[]>();

    public override void OnConnected(ConnectionInfo info)
    {
        base.OnConnected(info);
    }

    public override void OnDisconnected(ConnectionInfo info)
    {
        base.OnDisconnected(info);
        Disconnected = true;
        Session?.ClientHostDisconnected();
    }

    public override void OnMessage(IntPtr data, int size, long messageNum, long recvTime, int channel)
    {
        byte[] payload = new byte[size];
        if (size > 0)
        {
            Marshal.Copy(data, payload, 0, size);
        }
        if (Session != null)
        {
            Session.ClientMessageReceived(payload);
            return;
        }
        if (payload.Length < 1)
        {
            return;
        }
        switch ((SteamMsg)payload[0])
        {
            case SteamMsg.Welcome:
                PendingWelcome = payload;
                break;
            case SteamMsg.Full:
                GotFull = true;
                break;
            default:
                // BUG FIX (2026-08-24, "joiner never gets MSG_INIT over WAN"
                // investigation): this used to just discard anything besides
                // Welcome/Full here, on the theory that GamerJoined/GamerLeft/
                // Data "can't legitimately arrive before Session is assigned
                // (the host only sends those post-handshake, once this client
                // is already in its roster)". That's false in practice - on
                // the host side, CompleteJoin sends Welcome and then, still
                // within the SAME synchronous call (CompleteJoin ->
                // GamerJoined.Invoke -> NetSession.netSession_GamerJoined ->
                // SendData), sends the MSG_INIT Data frame to this exact
                // client - microseconds apart, no yield in between. Steam's
                // relay can deliver both to this OnMessage callback before
                // BeginJoin's handshake-polling loop (elsewhere in this file)
                // has gotten back around to notice PendingWelcome and finish
                // constructing/assigning Session. Diagnostics confirmed this:
                // the host logged "MSG_INIT build/send completed" with no
                // send failure, but the joiner's log never showed the MSG_INIT
                // arriving - it was hitting this exact branch and vanishing,
                // with nothing to replay it once Session finally existed.
                // Buffer it instead; CreateClientSide below replays anything
                // collected here into the real session the moment it exists.
                Console.WriteLine("[Steam client] OnMessage: " + (SteamMsg)payload[0] + " arrived before Session was assigned - buffering (" + payload.Length + " bytes).");
                PendingMessages.Add(payload);
                break;
        }
    }
}

// Real INetworkSession backend #3: Steam relay (SDR) transport, star
// topology with the host relaying any client-to-client traffic - same shape
// as LanNetworkSession, see that file's header and this file's header for
// what's different underneath. One instance plays either the host role or
// the client role, decided permanently at construction.
public sealed class SteamNetworkSession : INetworkSession
{
    private readonly bool _isHost;
    private readonly NetworkSessionProperties _properties;
    private readonly SteamLocalGamer _localGamer;
    private readonly List<ILocalNetworkGamer> _localGamers;
    private readonly List<INetworkGamer> _allGamers = new List<INetworkGamer>();
    private readonly List<INetworkGamer> _remoteGamers = new List<INetworkGamer>();

    // Host-only state.
    private SteamHostSocketManager _hostSocket;
    private Lobby? _hostLobby;
    private int _maxGamers;
    private byte _nextGamerId = 1; // 0 is always the host's own local gamer.
    private readonly Dictionary<byte, Connection> _hostConnections = new Dictionary<byte, Connection>();
    private readonly Dictionary<Connection, byte> _connectionToId = new Dictionary<Connection, byte>();
    private readonly Dictionary<Connection, Stopwatch> _pendingHellos = new Dictionary<Connection, Stopwatch>();

    // Client-only state.
    private SteamClientConnectionManager _clientManager;
    private Lobby? _joinedLobby;

    public bool IsHost => _isHost;
    public bool IsDisposed { get; private set; }
    public bool AllowJoinInProgress { get; set; }
    public NetworkSessionState SessionState { get; private set; } = NetworkSessionState.Lobby;

    public IReadOnlyList<ILocalNetworkGamer> LocalGamers => _localGamers;
    public IReadOnlyList<INetworkGamer> RemoteGamers => _remoteGamers;
    public IReadOnlyList<INetworkGamer> AllGamers => _allGamers;
    public NetworkSessionProperties SessionProperties => _properties;

    // No bandwidth accounting behind this backend yet, same as LAN.
    public float BytesPerSecondSent => 0f;
    public float BytesPerSecondReceived => 0f;

    public event EventHandler<GamerJoinedEventArgs> GamerJoined;
    public event EventHandler<GamerLeftEventArgs> GamerLeft;

    private SteamNetworkSession(bool isHost, NetworkSessionProperties properties, byte localId, string localGamertag)
    {
        _isHost = isHost;
        _properties = properties;
        _localGamer = new SteamLocalGamer(this, localId, localGamertag);
        _localGamers = new List<ILocalNetworkGamer> { _localGamer };
        _allGamers.Add(_localGamer);
    }

    internal static SteamNetworkSession CreateHostSide(NetworkSessionProperties properties, string localGamertag, SteamHostSocketManager socket, Lobby lobby, int maxGamers)
    {
        SteamNetworkSession session = new SteamNetworkSession(isHost: true, properties, localId: 0, localGamertag);
        session._hostSocket = socket;
        session._hostLobby = lobby;
        session._maxGamers = Math.Max(1, maxGamers);
        socket.Session = session;
        return session;
    }

    internal static SteamNetworkSession CreateClientSide(NetworkSessionProperties properties, SteamClientConnectionManager manager, Lobby lobby, byte localId, string localGamertag)
    {
        SteamNetworkSession session = new SteamNetworkSession(isHost: false, properties, localId, localGamertag);
        session._clientManager = manager;
        session._joinedLobby = lobby;
        manager.Session = session;
        // BUG FIX (2026-08-24, "joiner never gets MSG_INIT over WAN"
        // investigation): replay anything OnMessage had to buffer above
        // (see the `default:` case there) because it arrived before Session
        // existed - most importantly the MSG_INIT Data frame, which the host
        // can send this fast right on the heels of Welcome. Must run in the
        // order received, and must run after manager.Session is assigned
        // (immediately above) since ClientMessageReceived is what actually
        // applies these.
        if (manager.PendingMessages.Count > 0)
        {
            Console.WriteLine("[Steam client] CreateClientSide: replaying " + manager.PendingMessages.Count + " message(s) buffered before Session existed.");
            foreach (byte[] pending in manager.PendingMessages)
            {
                session.ClientMessageReceived(pending);
            }
            manager.PendingMessages.Clear();
        }
        return session;
    }

    // Called only while building a freshly-joined client session, once per
    // gamer in the Welcome roster - see the matching comment on
    // LanNetworkSession.AddInitialRemoteGamer for why this deliberately does
    // NOT raise GamerJoined.
    internal void AddInitialRemoteGamer(byte id, string gamertag)
    {
        SteamNetworkGamer gamer = new SteamNetworkGamer(id, gamertag);
        _allGamers.Add(gamer);
        _remoteGamers.Add(gamer);
    }

    public void Update()
    {
        if (IsDisposed)
        {
            return;
        }
        SteamInit.Pump();
        if (_isHost)
        {
            UpdateHost();
        }
        else
        {
            UpdateClient();
        }
    }

    private void UpdateHost()
    {
        _hostSocket?.Receive(SteamNetConfig.ReceiveBatchSize);

        if (_pendingHellos.Count > 0)
        {
            List<Connection> stale = null;
            foreach (KeyValuePair<Connection, Stopwatch> kvp in _pendingHellos)
            {
                if (kvp.Value.Elapsed.TotalSeconds > SteamNetConfig.ConnectTimeoutSeconds)
                {
                    (stale ??= new List<Connection>()).Add(kvp.Key);
                }
            }
            if (stale != null)
            {
                foreach (Connection c in stale)
                {
                    _pendingHellos.Remove(c);
                    try
                    {
                        c.Close();
                    }
                    catch
                    {
                    }
                }
            }
        }
    }

    // Called from SteamHostSocketManager.OnConnected - a relay connection is
    // up, but not yet a real gamer until its Hello arrives (see
    // HostMessageReceived/CompleteJoin below), same two-step as LAN's
    // TcpListener accept -> pending Hello wait.
    internal void HostConnectionOpened(Connection connection)
    {
        _pendingHellos[connection] = Stopwatch.StartNew();
    }

    internal void HostConnectionClosed(Connection connection)
    {
        _pendingHellos.Remove(connection);
        if (_connectionToId.TryGetValue(connection, out byte id))
        {
            _connectionToId.Remove(connection);
            RemoveRemoteGamer(id, notifyOthers: true);
        }
    }

    internal void HostMessageReceived(Connection connection, byte[] payload)
    {
        if (payload.Length < 1)
        {
            return;
        }
        SteamMsg type = (SteamMsg)payload[0];
        // TEMP DIAGNOSTIC, see OnConnecting above.
        Console.WriteLine("[Steam host] received " + type + " (" + payload.Length + " bytes).");
        if (type == SteamMsg.Hello)
        {
            if (_connectionToId.ContainsKey(connection))
            {
                // Already completed this connection's handshake earlier - a
                // well-behaved client never sends a second Hello, but don't
                // double-register it if one shows up anyway.
                return;
            }
            string tag;
            using (MemoryStream ms = new MemoryStream(payload, 1, payload.Length - 1))
            using (BinaryReader br = new BinaryReader(ms))
            {
                tag = br.ReadString();
            }
            CompleteJoin(connection, tag);
            return;
        }
        if (type == SteamMsg.Data)
        {
            if (_connectionToId.TryGetValue(connection, out byte senderId))
            {
                HandleHostDataFrame(senderId, payload);
            }
            return;
        }
        // GamerJoined/GamerLeft/Welcome/Full are host->client only - a
        // well-behaved client never sends them back; ignore anything else.
    }

    private void CompleteJoin(Connection connection, string tag)
    {
        _pendingHellos.Remove(connection);
        // TEMP DIAGNOSTIC, see OnConnecting above.
        Console.WriteLine("[Steam host] CompleteJoin: tag=" + tag + ", current gamers=" + _allGamers.Count + "/" + _maxGamers + ".");
        if (_allGamers.Count >= _maxGamers)
        {
            Console.WriteLine("[Steam host] session full, sending Full and closing connection.");
            try
            {
                connection.SendMessage(SteamFrame.Build(SteamMsg.Full, null));
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Steam host] SendMessage(Full) threw: " + ex);
            }
            try
            {
                connection.Close();
            }
            catch
            {
            }
            return;
        }

        byte newId = _nextGamerId++;

        // Roster snapshot built BEFORE the new gamer is added below, so it
        // never includes the new client itself - just everyone already here.
        byte[] welcome = SteamFrame.Build(SteamMsg.Welcome, bw =>
        {
            bw.Write(newId);
            bw.Write(_allGamers.Count);
            for (int i = 0; i < _allGamers.Count; i++)
            {
                INetworkGamer g = _allGamers[i];
                bw.Write(g.Id);
                bw.Write(g.Gamertag);
            }
        });
        // TEMP DIAGNOSTIC, see OnConnecting above.
        Console.WriteLine("[Steam host] sending Welcome to new gamer id " + newId + " (" + welcome.Length + " bytes).");
        try
        {
            // TEMP DIAGNOSTIC, see the comment on SendData's unicast branch
            // further down in this file.
            Result welcomeResult = connection.SendMessage(welcome);
            if (welcomeResult != Result.OK)
            {
                Console.WriteLine("[Steam host] SendMessage(Welcome) returned " + welcomeResult + " - message was NOT delivered.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("[Steam host] SendMessage(Welcome) threw: " + ex);
        }

        byte[] joinedFrame = SteamFrame.Build(SteamMsg.GamerJoined, bw =>
        {
            bw.Write(newId);
            bw.Write(tag);
        });
        foreach (KeyValuePair<byte, Connection> kvp in _hostConnections)
        {
            try
            {
                // TEMP DIAGNOSTIC, see the comment on SendData's unicast
                // branch further down in this file.
                Result joinedResult = kvp.Value.SendMessage(joinedFrame);
                if (joinedResult != Result.OK)
                {
                    Console.WriteLine("[Steam host] SendMessage(GamerJoined) to gamer " + kvp.Key + " returned " + joinedResult + " - message was NOT delivered.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Steam host] SendMessage(GamerJoined) to gamer " + kvp.Key + " threw " + ex.GetType().Name + ": " + ex.Message);
            }
        }

        // Only now does the new gamer become "real" (routable, and visible
        // to the GamerJoined handler) - matches LanNetworkSession's own
        // ordering, which matches what NetSession.netSession_GamerJoined
        // needs (it immediately turns around and calls SendData(...,gamer)
        // on the same event).
        _hostConnections[newId] = connection;
        _connectionToId[connection] = newId;
        SteamNetworkGamer newGamer = new SteamNetworkGamer(newId, tag);
        _allGamers.Add(newGamer);
        _remoteGamers.Add(newGamer);
        GamerJoined?.Invoke(this, new GamerJoinedEventArgs(newGamer));
    }

    private void HandleHostDataFrame(byte senderId, byte[] payload)
    {
        if (payload.Length < 2)
        {
            return;
        }
        byte targetId = payload[1];
        int rawLen = payload.Length - 2;
        byte[] raw = new byte[rawLen];
        Array.Copy(payload, 2, raw, 0, rawLen);

        if (targetId == _localGamer.Id)
        {
            _localGamer.Inbox.Enqueue((senderId, raw));
            return;
        }
        if (targetId == SteamNetConfig.BroadcastId)
        {
            foreach (KeyValuePair<byte, Connection> kvp in _hostConnections)
            {
                if (kvp.Key == senderId)
                {
                    continue;
                }
                byte[] frame = SteamFrame.Build(SteamMsg.Data, bw =>
                {
                    bw.Write(senderId);
                    bw.Write(raw);
                });
                try
                {
                    // TEMP DIAGNOSTIC, see the comment on SendData's
                    // unicast branch below in this file.
                    Result result = kvp.Value.SendMessage(frame);
                    if (result != Result.OK)
                    {
                        Console.WriteLine("[Steam host] relay broadcast to gamer " + kvp.Key + " returned " + result + " - message was NOT delivered.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[Steam host] relay broadcast to gamer " + kvp.Key + " threw " + ex.GetType().Name + ": " + ex.Message);
                }
            }
            _localGamer.Inbox.Enqueue((senderId, raw));
            return;
        }
        if (_hostConnections.TryGetValue(targetId, out Connection targetConn))
        {
            byte[] frame = SteamFrame.Build(SteamMsg.Data, bw =>
            {
                bw.Write(senderId);
                bw.Write(raw);
            });
            try
            {
                // TEMP DIAGNOSTIC, see the comment on SendData's unicast
                // branch below in this file.
                Result result = targetConn.SendMessage(frame);
                if (result != Result.OK)
                {
                    Console.WriteLine("[Steam host] relay unicast to gamer " + targetId + " returned " + result + " - message was NOT delivered.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Steam host] relay unicast to gamer " + targetId + " threw " + ex.GetType().Name + ": " + ex.Message);
            }
        }
    }

    private void UpdateClient()
    {
        _clientManager?.Receive(SteamNetConfig.ReceiveBatchSize);
        if (_clientManager != null && _clientManager.Disconnected)
        {
            // INetworkSession has no "host vanished" event to raise, same as
            // LAN - marking this disposed is the safest thing to do; the
            // menu/timeout logic in NetSession.cs is what surfaces this to
            // the player.
            IsDisposed = true;
        }
    }

    internal void ClientMessageReceived(byte[] payload)
    {
        if (payload.Length < 1)
        {
            return;
        }
        SteamMsg type = (SteamMsg)payload[0];
        switch (type)
        {
        case SteamMsg.GamerJoined:
        {
            byte id;
            string tag;
            using (MemoryStream ms = new MemoryStream(payload, 1, payload.Length - 1))
            using (BinaryReader br = new BinaryReader(ms))
            {
                id = br.ReadByte();
                tag = br.ReadString();
            }
            SteamNetworkGamer gamer = new SteamNetworkGamer(id, tag);
            _allGamers.Add(gamer);
            _remoteGamers.Add(gamer);
            GamerJoined?.Invoke(this, new GamerJoinedEventArgs(gamer));
            break;
        }
        case SteamMsg.GamerLeft:
            if (payload.Length >= 2)
            {
                RemoveRemoteGamer(payload[1], notifyOthers: false);
            }
            break;
        case SteamMsg.Data:
            if (payload.Length >= 2)
            {
                byte senderId = payload[1];
                int rawLen = payload.Length - 2;
                byte[] raw = new byte[rawLen];
                Array.Copy(payload, 2, raw, 0, rawLen);
                _localGamer.Inbox.Enqueue((senderId, raw));
            }
            break;
        default:
            // Welcome/Full only ever matter during the join handshake itself
            // (handled by SteamClientConnectionManager/BeginJoin's poll
            // function, before this session object even exists) - anything
            // else arriving here is stale or a protocol mismatch, safe to
            // ignore rather than fail the whole session over.
            break;
        }
    }

    internal void ClientHostDisconnected()
    {
        IsDisposed = true;
    }

    private void RemoveRemoteGamer(byte id, bool notifyOthers)
    {
        if (_isHost)
        {
            if (_hostConnections.TryGetValue(id, out Connection conn))
            {
                _connectionToId.Remove(conn);
            }
            _hostConnections.Remove(id);
        }
        INetworkGamer gamer = null;
        for (int i = 0; i < _allGamers.Count; i++)
        {
            if (_allGamers[i].Id == id)
            {
                gamer = _allGamers[i];
                break;
            }
        }
        if (gamer == null)
        {
            return;
        }
        _allGamers.Remove(gamer);
        _remoteGamers.Remove(gamer);
        if (_isHost && notifyOthers)
        {
            byte[] leftFrame = SteamFrame.Build(SteamMsg.GamerLeft, bw => bw.Write(id));
            foreach (KeyValuePair<byte, Connection> kvp in _hostConnections)
            {
                try
                {
                    // TEMP DIAGNOSTIC, see the comment on SendData's
                    // unicast branch further down in this file.
                    Result leftResult = kvp.Value.SendMessage(leftFrame);
                    if (leftResult != Result.OK)
                    {
                        Console.WriteLine("[Steam host] SendMessage(GamerLeft) to gamer " + kvp.Key + " returned " + leftResult + " - message was NOT delivered.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[Steam host] SendMessage(GamerLeft) to gamer " + kvp.Key + " threw " + ex.GetType().Name + ": " + ex.Message);
                }
            }
        }
        GamerLeft?.Invoke(this, new GamerLeftEventArgs(gamer));
    }

    // Routes one outgoing SendData call. Host: unicast straight to the
    // target's connection (or relay-broadcast). Client: everything goes to
    // the host first, tagged with who it's really for, and the host relays
    // it on if that's not itself - see HandleHostDataFrame above. Same
    // routing rules as LanNetworkSession.SendData.
    internal void SendData(SteamLocalGamer sender, byte[] data, INetworkGamer recipient)
    {
        if (IsDisposed)
        {
            return;
        }
        byte targetId = recipient?.Id ?? SteamNetConfig.BroadcastId;

        if (_isHost)
        {
            if (targetId == _localGamer.Id)
            {
                _localGamer.Inbox.Enqueue((_localGamer.Id, data));
                return;
            }
            if (targetId == SteamNetConfig.BroadcastId)
            {
                foreach (KeyValuePair<byte, Connection> kvp in _hostConnections)
                {
                    byte[] frame = SteamFrame.Build(SteamMsg.Data, bw =>
                    {
                        bw.Write(_localGamer.Id);
                        bw.Write(data);
                    });
                    try
                    {
                        // TEMP DIAGNOSTIC, see the comment on the unicast
                        // branch below.
                        Result result = kvp.Value.SendMessage(frame);
                        if (result != Result.OK)
                        {
                            Console.WriteLine("[Steam host] SendData broadcast to gamer " + kvp.Key + " returned " + result + " - message was NOT delivered.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("[Steam host] SendData broadcast to gamer " + kvp.Key + " threw " + ex.GetType().Name + ": " + ex.Message);
                    }
                }
                return;
            }
            if (_hostConnections.TryGetValue(targetId, out Connection conn))
            {
                byte[] frame = SteamFrame.Build(SteamMsg.Data, bw =>
                {
                    bw.Write(_localGamer.Id);
                    bw.Write(data);
                });
                try
                {
                    // TEMP DIAGNOSTIC (2026-08-24, "joiner never gets
                    // MSG_INIT over WAN" investigation): this exact call is
                    // what carries MSG_INIT (and every other unicast Data
                    // frame, e.g. the freeSlot assignment) from host to one
                    // specific client. The original code never looked at
                    // SendMessage's return value or logged a thrown
                    // exception - either one failing silently here would
                    // explain a client's slot assignment vanishing with no
                    // trace at all, which matches what the last WAN test's
                    // logs showed (host computed freeSlot, client never saw
                    // MSG_INIT). Safe to remove once understood.
                    Result result = conn.SendMessage(frame);
                    if (result != Result.OK)
                    {
                        Console.WriteLine("[Steam host] SendData unicast to gamer " + targetId + " returned " + result + " (" + frame.Length + " bytes) - message was NOT delivered.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[Steam host] SendData unicast to gamer " + targetId + " threw " + ex.GetType().Name + ": " + ex.Message);
                }
            }
            return;
        }

        if (_clientManager != null)
        {
            byte[] frame = SteamFrame.Build(SteamMsg.Data, bw =>
            {
                bw.Write(targetId);
                bw.Write(data);
            });
            try
            {
                // TEMP DIAGNOSTIC, see the comment above.
                Result result = _clientManager.Connection.SendMessage(frame);
                if (result != Result.OK)
                {
                    Console.WriteLine("[Steam client] SendData to host returned " + result + " (" + frame.Length + " bytes) - message was NOT delivered.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Steam client] SendData to host threw " + ex.GetType().Name + ": " + ex.Message);
            }
        }
    }

    internal INetworkGamer FindGamer(byte id)
    {
        if (_localGamer.Id == id)
        {
            return _localGamer;
        }
        for (int i = 0; i < _allGamers.Count; i++)
        {
            if (_allGamers[i].Id == id)
            {
                return _allGamers[i];
            }
        }
        return null;
    }

    public void StartGame()
    {
        SessionState = NetworkSessionState.Playing;
    }

    public void EndGame()
    {
        SessionState = NetworkSessionState.Ended;
    }

    // Opens Steam's native "invite a friend" overlay for whichever lobby
    // this session actually has - the host's _hostLobby, or a client's
    // _joinedLobby (Steam lets any lobby member invite others, not just the
    // owner). Called from NetSession.InviteFriends() (see GameMain.cs's
    // "Invite Friends" pause-menu item). Whoever the player picks gets a
    // normal Steam invite notification; accepting it raises
    // SteamFriends.OnGameLobbyJoinRequested (see SteamInit below), the same
    // event that fires for clicking "Join Game" on a friend already shown
    // as being in this lobby via the Steam friends list - both paths end up
    // at NetSession.JoinInvite the same way.
    public void OpenInviteOverlay()
    {
        Lobby? lobby = _isHost ? _hostLobby : _joinedLobby;
        if (lobby == null)
        {
            return;
        }
        try
        {
            SteamFriends.OpenGameInviteOverlay(lobby.Value.Id);
        }
        catch (Exception ex)
        {
            Console.WriteLine("[Steam] OpenInviteOverlay: OpenGameInviteOverlay threw " + ex.GetType().Name + ": " + ex.Message);
        }
    }

    public void Dispose()
    {
        if (IsDisposed)
        {
            return;
        }
        IsDisposed = true;
        if (_isHost)
        {
            foreach (KeyValuePair<byte, Connection> kvp in _hostConnections)
            {
                try
                {
                    kvp.Value.Close();
                }
                catch
                {
                }
            }
            _hostConnections.Clear();
            _connectionToId.Clear();
            foreach (KeyValuePair<Connection, Stopwatch> kvp in _pendingHellos)
            {
                try
                {
                    kvp.Key.Close();
                }
                catch
                {
                }
            }
            _pendingHellos.Clear();
            try
            {
                _hostSocket?.Close();
            }
            catch
            {
            }
            try
            {
                _hostLobby?.Leave();
            }
            catch
            {
            }
        }
        else
        {
            try
            {
                _clientManager?.Close();
            }
            catch
            {
            }
            try
            {
                _joinedLobby?.Leave();
            }
            catch
            {
            }
        }
    }
}

// The factory NetworkBackend.Current is set to for internet play (see the
// new "case 0" wiring in ZP2K9.menu.levels/Main.cs). BeginCreate/BeginFind/
// BeginJoin are all genuinely async here (a Steam lobby call, then - for
// Join - a relay connection handshake on top), polled lazily via the reused
// LanOpResult per the header comment at the top of this file.
public sealed class SteamNetworkSessionFactory : INetworkSessionFactory
{
    private static string LocalGamertag()
    {
        if (SteamClient.IsValid)
        {
            try
            {
                return SteamClient.Name;
            }
            catch
            {
            }
        }
        return Gamer.SignedInGamers.Count > 0 ? Gamer.SignedInGamers[0].Gamertag : "Player";
    }

    public IAsyncResult BeginCreate(NetworkSessionType sessionType, int maxLocalGamers, int maxGamers, int privateGamerSlots, NetworkSessionProperties properties, AsyncCallback callback, object asyncState)
    {
        if (!SteamInit.EnsureInitialized(out string initError))
        {
            LanOpResult failedInit = new LanOpResult(asyncState, () => true) { Error = new InvalidOperationException(initError) };
            callback?.Invoke(failedInit);
            return failedInit;
        }

        Task<Lobby?> createTask;
        try
        {
            createTask = SteamMatchmaking.CreateLobbyAsync(Math.Max(1, maxGamers));
        }
        catch (Exception ex)
        {
            // TEMP DIAGNOSTIC, see the EnsureInitialized comment above - same
            // reason: this is a synchronous failure that bypasses the poll
            // loop, so it needs its own log line.
            Console.WriteLine("[Steam host] BeginCreate: CreateLobbyAsync threw " + ex.GetType().Name + ": " + ex.Message);
            LanOpResult failedCreate = new LanOpResult(asyncState, () => true) { Error = ex };
            callback?.Invoke(failedCreate);
            return failedCreate;
        }

        bool configured = false;
        LanOpResult result = null;
        result = new LanOpResult(asyncState, () =>
        {
            SteamInit.Pump();
            if (!createTask.IsCompleted)
            {
                return false;
            }
            if (createTask.IsFaulted || createTask.IsCanceled)
            {
                throw createTask.Exception?.GetBaseException() ?? new InvalidOperationException("Couldn't create a Steam lobby.");
            }
            Lobby? lobby = createTask.Result;
            if (lobby == null)
            {
                throw new InvalidOperationException("Steam couldn't create a lobby (try again in a moment).");
            }
            if (!configured)
            {
                Lobby l = lobby.Value;
                // privateGamerSlots > 0 is StartServer.cs's "Status: Private"
                // toggle (see NetSession.CreateSession, which passes
                // privateMatch ? 9 : 0 here) - reused as-is rather than
                // threading a whole new parameter through
                // INetworkSessionFactory.BeginCreate for one bool. A private
                // Steam lobby (k_ELobbyTypePrivate) never appears in
                // BeginFind's RequestLobbyList results below no matter what
                // key/value filter is used - Valve's matchmaking service
                // excludes private lobbies from that query outright - so
                // this is genuinely invite-only with no further BeginFind
                // changes needed. SetJoinable(true) still has to stay
                // unconditional either way, or an invited friend's
                // Lobby.Join() would be rejected too.
                bool isPrivate = privateGamerSlots > 0;
                if (isPrivate)
                {
                    l.SetPrivate();
                }
                else
                {
                    l.SetPublic();
                }
                l.SetJoinable(true);
                l.SetData(SteamNetConfig.GameTag, SteamNetConfig.GameTagValue);
                l.SetData(SteamNetConfig.VersionKey, (properties?[0] ?? 0).ToString());
                l.SetData(SteamNetConfig.GameTypeKey, (properties?[1] ?? 0).ToString());
                l.SetData(SteamNetConfig.HostNameKey, LocalGamertag());
                configured = true;
                // TEMP DIAGNOSTIC (join-timeout investigation, 2026-08-23): confirms
                // a lobby actually got created, made public/joinable, and tagged
                // with the exact key/value BeginFind's WithKeyValue filter below
                // looks for. Safe to remove once discovery/join are confirmed working.
                Console.WriteLine("[Steam host] lobby created: id=" + l.Id + ", " + (isPrivate ? "private (invite-only)" : "public") + ", tag=" + SteamNetConfig.GameTag + "=" + SteamNetConfig.GameTagValue + ", ver=" + (properties?[0] ?? 0) + ", gameType=" + (properties?[1] ?? 0) + ", hostName=" + LocalGamertag() + ".");
            }
            SteamHostSocketManager socket = SteamNetworkingSockets.CreateRelaySocket<SteamHostSocketManager>();
            SteamNetworkSession session = SteamNetworkSession.CreateHostSide(properties, LocalGamertag(), socket, lobby.Value, maxGamers);
            result.Value = session;
            return true;
        });
        callback?.Invoke(result);
        return result;
    }

    public INetworkSession EndCreate(IAsyncResult result)
    {
        LanOpResult r = (LanOpResult)result;
        if (r.Error != null)
        {
            throw r.Error;
        }
        return (INetworkSession)r.Value;
    }

    public IAsyncResult BeginFind(NetworkSessionType sessionType, int maxLocalGamers, NetworkSessionProperties searchProperties, AsyncCallback callback, object asyncState)
    {
        if (!SteamInit.EnsureInitialized(out string initError))
        {
            LanOpResult failedInit = new LanOpResult(asyncState, () => true) { Error = new InvalidOperationException(initError) };
            callback?.Invoke(failedInit);
            return failedInit;
        }

        Task<Lobby[]> queryTask;
        try
        {
            queryTask = SteamMatchmaking.LobbyList
                .WithKeyValue(SteamNetConfig.GameTag, SteamNetConfig.GameTagValue)
                .WithMaxResults(SteamNetConfig.MaxLobbyResults)
                .RequestAsync();
        }
        catch (Exception ex)
        {
            // TEMP DIAGNOSTIC, see the EnsureInitialized comment above.
            Console.WriteLine("[Steam client] BeginFind: LobbyList.RequestAsync threw " + ex.GetType().Name + ": " + ex.Message);
            LanOpResult failedQuery = new LanOpResult(asyncState, () => true) { Error = ex };
            callback?.Invoke(failedQuery);
            return failedQuery;
        }

        List<IAvailableNetworkSession> found = new List<IAvailableNetworkSession>();
        LanOpResult result = new LanOpResult(asyncState, () =>
        {
            SteamInit.Pump();
            if (!queryTask.IsCompleted)
            {
                return false;
            }
            if (queryTask.IsFaulted || queryTask.IsCanceled)
            {
                throw queryTask.Exception?.GetBaseException() ?? new InvalidOperationException("Couldn't search for Steam games.");
            }
            Lobby[] lobbies = queryTask.Result ?? Array.Empty<Lobby>();
            // TEMP DIAGNOSTIC, see the matching comment in BeginCreate above.
            Console.WriteLine("[Steam client] BeginFind: query for " + SteamNetConfig.GameTag + "=" + SteamNetConfig.GameTagValue + " returned " + lobbies.Length + " lobbies.");
            foreach (Lobby lobby in lobbies)
            {
                string hostTag = lobby.GetData(SteamNetConfig.HostNameKey);
                if (string.IsNullOrEmpty(hostTag))
                {
                    hostTag = lobby.Owner.Name;
                }
                int.TryParse(lobby.GetData(SteamNetConfig.VersionKey), out int ver);
                int.TryParse(lobby.GetData(SteamNetConfig.GameTypeKey), out int gameType);
                NetworkSessionProperties props = new NetworkSessionProperties { [0] = ver, [1] = gameType };
                int openSlots = Math.Max(0, lobby.MaxMembers - lobby.MemberCount);
                found.Add(new SteamAvailableNetworkSession(hostTag, lobby.MemberCount, openSlots, props, lobby));
                Console.WriteLine("[Steam client]   -> lobby id=" + lobby.Id + ", host=" + hostTag + ", ver=" + ver + ", gameType=" + gameType + ", members=" + lobby.MemberCount + "/" + lobby.MaxMembers + ".");
            }
            return true;
        })
        { Value = found };

        callback?.Invoke(result);
        return result;
    }

    public IReadOnlyList<IAvailableNetworkSession> EndFind(IAsyncResult result)
    {
        LanOpResult r = (LanOpResult)result;
        if (r.Error != null)
        {
            throw r.Error;
        }
        return (IReadOnlyList<IAvailableNetworkSession>)r.Value;
    }

    public IAsyncResult BeginJoin(IAvailableNetworkSession session, AsyncCallback callback, object asyncState)
    {
        SteamAvailableNetworkSession steamSession = session as SteamAvailableNetworkSession;
        string initError = null;
        if (steamSession == null || !SteamInit.EnsureInitialized(out initError))
        {
            LanOpResult failedJoin = new LanOpResult(asyncState, () => true)
            {
                Error = new InvalidOperationException(steamSession == null ? "That listing can't be joined." : initError)
            };
            callback?.Invoke(failedJoin);
            return failedJoin;
        }

        Task<RoomEnter> joinTask;
        try
        {
            joinTask = steamSession.Lobby.Join();
        }
        catch (Exception ex)
        {
            // TEMP DIAGNOSTIC, see the EnsureInitialized comment above.
            Console.WriteLine("[Steam client] BeginJoin: Lobby.Join() threw " + ex.GetType().Name + ": " + ex.Message);
            LanOpResult failedJoinTask = new LanOpResult(asyncState, () => true) { Error = ex };
            callback?.Invoke(failedJoinTask);
            return failedJoinTask;
        }

        int phase = 0; // 0 = joining the lobby, 1 = connecting the relay socket, 2 = handshaking (Hello/Welcome).
        SteamClientConnectionManager manager = null;
        bool helloSent = false;
        string myTag = LocalGamertag();
        Stopwatch sw = Stopwatch.StartNew();

        LanOpResult result = null;
        result = new LanOpResult(asyncState, () =>
        {
            SteamInit.Pump();
            if (phase == 0)
            {
                if (!joinTask.IsCompleted)
                {
                    return false;
                }
                if (joinTask.IsFaulted || joinTask.IsCanceled)
                {
                    throw new InvalidOperationException("Couldn't join that game (connection error).");
                }
                if (joinTask.Result != RoomEnter.Success)
                {
                    throw new InvalidOperationException("Couldn't join that game (" + joinTask.Result + ").");
                }
                // TEMP DIAGNOSTIC (join-timeout investigation, 2026-08-23):
                // safe to remove once the "no response" join-timeout bug is
                // understood.
                Console.WriteLine("[Steam client] joined lobby, connecting relay socket to host " + steamSession.Lobby.Owner.Id + "...");
                manager = SteamNetworkingSockets.ConnectRelay<SteamClientConnectionManager>(steamSession.Lobby.Owner.Id);
                phase = 1;
                return false;
            }
            if (phase == 1)
            {
                manager.Receive(SteamNetConfig.ReceiveBatchSize);
                if (manager.Disconnected)
                {
                    throw new InvalidOperationException("Lost connection while joining that game.");
                }
                if (manager.Connected)
                {
                    // TEMP DIAGNOSTIC, see the phase-0 comment above.
                    Console.WriteLine("[Steam client] relay connection to host is open, phase -> handshake.");
                    phase = 2;
                }
                else if (sw.Elapsed.TotalSeconds > SteamNetConfig.ConnectTimeoutSeconds)
                {
                    throw new InvalidOperationException("Couldn't reach that game (timed out connecting).");
                }
                return false;
            }

            if (!helloSent)
            {
                // TEMP DIAGNOSTIC, see the phase-0 comment above.
                Console.WriteLine("[Steam client] sending Hello as \"" + myTag + "\".");
                try
                {
                    manager.Connection.SendMessage(SteamFrame.Build(SteamMsg.Hello, bw => bw.Write(myTag)));
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Couldn't join that game: " + ex.Message);
                }
                helloSent = true;
            }
            manager.Receive(SteamNetConfig.ReceiveBatchSize);
            if (manager.Disconnected)
            {
                throw new InvalidOperationException("Lost connection while joining that game.");
            }
            if (manager.GotFull)
            {
                throw new InvalidOperationException("That game is full.");
            }
            if (manager.PendingWelcome != null)
            {
                byte[] payload = manager.PendingWelcome;
                byte myId;
                int count;
                using (MemoryStream ms = new MemoryStream(payload, 1, payload.Length - 1))
                using (BinaryReader br = new BinaryReader(ms))
                {
                    myId = br.ReadByte();
                    count = br.ReadInt32();
                    SteamNetworkSession newSession = SteamNetworkSession.CreateClientSide(steamSession.SessionProperties, manager, steamSession.Lobby, myId, myTag);
                    for (int i = 0; i < count; i++)
                    {
                        byte id = br.ReadByte();
                        string tag = br.ReadString();
                        newSession.AddInitialRemoteGamer(id, tag);
                    }
                    result.Value = newSession;
                }
                return true;
            }
            if (sw.Elapsed.TotalSeconds > SteamNetConfig.ConnectTimeoutSeconds)
            {
                // TEMP DIAGNOSTIC, see the phase-0 comment above.
                Console.WriteLine("[Steam client] handshake timed out after " + sw.Elapsed.TotalSeconds.ToString("0.0") + "s waiting for Welcome/Full.");
                throw new InvalidOperationException("Couldn't join that game (no response).");
            }
            return false;
        });
        callback?.Invoke(result);
        return result;
    }

    public INetworkSession EndJoin(IAsyncResult result)
    {
        LanOpResult r = (LanOpResult)result;
        if (r.Error != null)
        {
            throw r.Error;
        }
        return (INetworkSession)r.Value;
    }

    // Entry point for NetSession.JoinInvite (see INetworkSession.cs's header
    // comment) - inviteToken is the boxed Steamworks.Data.Lobby captured by
    // SteamInit.OnGameLobbyJoinRequested above. Unlike BeginJoin, there's no
    // BeginFind() result to read host name/version/game type off - just the
    // raw lobby - so this wraps it into the same SteamAvailableNetworkSession
    // shape (built from the lobby's own data, same parsing BeginFind uses)
    // and hands off to BeginJoin, reusing its entire join/handshake state
    // machine rather than duplicating it.
    public IAsyncResult BeginJoinInvite(object inviteToken, AsyncCallback callback, object asyncState)
    {
        if (inviteToken is not Lobby lobby)
        {
            LanOpResult failedToken = new LanOpResult(asyncState, () => true)
            {
                Error = new InvalidOperationException("That invite can't be joined (missing lobby information).")
            };
            callback?.Invoke(failedToken);
            return failedToken;
        }
        return BeginJoinLobby(lobby, callback, asyncState);
    }

    private IAsyncResult BeginJoinLobby(Lobby lobby, AsyncCallback callback, object asyncState)
    {
        string hostTag = lobby.GetData(SteamNetConfig.HostNameKey);
        if (string.IsNullOrEmpty(hostTag))
        {
            hostTag = lobby.Owner.Name;
        }
        int.TryParse(lobby.GetData(SteamNetConfig.VersionKey), out int ver);
        int.TryParse(lobby.GetData(SteamNetConfig.GameTypeKey), out int gameType);
        NetworkSessionProperties props = new NetworkSessionProperties { [0] = ver, [1] = gameType };
        int openSlots = Math.Max(0, lobby.MaxMembers - lobby.MemberCount);
        SteamAvailableNetworkSession wrapped = new SteamAvailableNetworkSession(hostTag, lobby.MemberCount, openSlots, props, lobby);
        return BeginJoin(wrapped, callback, asyncState);
    }
}
