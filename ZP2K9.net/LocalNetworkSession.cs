using System;
using System.Collections.Generic;
using ZP2K9.platform;

namespace ZP2K9.net;

// First real INetworkSessionFactory backend: a fully in-process, host-only
// session with exactly one local gamer and no actual socket transport. This
// is what makes "Practice" (and any other Start-Server flow) work again -
// on the original Xbox, hosting a bots-only local match still went through
// NetworkSession.Create(SystemLink, ...), which XNA could satisfy instantly
// without needing a second machine. Our NotConfiguredNetworkSessionFactory
// stub deliberately threw for *every* Create call (see its own comment),
// which was correct while nothing existed yet, but it meant Practice threw
// too, since Practice never had its own "local-only" code path - it always
// went through NetSession.CreateSession() like real multiplayer hosting.
//
// This backend fixes that by actually succeeding for Create (so Practice/
// solo hosting works end-to-end, bots and all), while Find/Join still
// honestly report "nothing out there yet" instead of pretending to work -
// real LAN discovery/join (a second machine actually connecting in) is the
// next increment on top of this, behind the same INetworkSessionFactory
// interface, without gameplay/menu code needing to change again.
public sealed class LocalNetworkGamer : ILocalNetworkGamer
{
    public byte Id { get; }
    public string Gamertag { get; }
    public bool IsHost => true;
    public bool IsTalking => false;
    public TimeSpan RoundtripTime => TimeSpan.Zero;
    public bool IsDataAvailable => false;

    internal LocalNetworkGamer(byte id, string gamertag)
    {
        Id = id;
        Gamertag = gamertag;
    }

    public void SendData(PacketWriter writer, SendDataOptions options, INetworkGamer recipient)
    {
        // No remote peers on a local-only session - nothing to send anywhere.
    }

    public void ReceiveData(PacketReader reader, out INetworkGamer sender)
    {
        sender = null;
    }
}

public sealed class LocalNetworkSession : INetworkSession
{
    private readonly List<ILocalNetworkGamer> _localGamers;
    private readonly List<INetworkGamer> _allGamers;

    public bool IsHost => true;
    public bool IsDisposed { get; private set; }
    public bool AllowJoinInProgress { get; set; }
    public NetworkSessionState SessionState { get; private set; } = NetworkSessionState.Lobby;

    public IReadOnlyList<ILocalNetworkGamer> LocalGamers => _localGamers;
    public IReadOnlyList<INetworkGamer> RemoteGamers { get; } = Array.Empty<INetworkGamer>();
    public IReadOnlyList<INetworkGamer> AllGamers => _allGamers;

    public NetworkSessionProperties SessionProperties { get; }

    public float BytesPerSecondSent => 0f;
    public float BytesPerSecondReceived => 0f;

    public event EventHandler<GamerJoinedEventArgs> GamerJoined;
    public event EventHandler<GamerLeftEventArgs> GamerLeft;

    internal LocalNetworkSession(NetworkSessionProperties properties)
    {
        SessionProperties = properties;
        string gamertag = Gamer.SignedInGamers.Count > 0 ? Gamer.SignedInGamers[0].Gamertag : "Player";
        LocalNetworkGamer localGamer = new LocalNetworkGamer(0, gamertag);
        _localGamers = new List<ILocalNetworkGamer> { localGamer };
        _allGamers = new List<INetworkGamer> { localGamer };
    }

    public void Update()
    {
        // Nothing to pump yet - no real transport behind this session.
    }

    public void StartGame()
    {
        SessionState = NetworkSessionState.Playing;
    }

    public void EndGame()
    {
        SessionState = NetworkSessionState.Ended;
    }

    public void Dispose()
    {
        IsDisposed = true;
    }

    // Practice/solo hosting has no one else to invite - see INetworkSession.cs.
    public void OpenInviteOverlay()
    {
    }
}

public sealed class LocalNetworkSessionFactory : INetworkSessionFactory
{
    public IAsyncResult BeginCreate(NetworkSessionType sessionType, int maxLocalGamers, int maxGamers, int privateGamerSlots, NetworkSessionProperties properties, AsyncCallback callback, object asyncState)
    {
        LocalNetworkSession session = new LocalNetworkSession(properties);
        CompletedAsyncResult<LocalNetworkSession> result = new CompletedAsyncResult<LocalNetworkSession>(session, asyncState);
        callback?.Invoke(result);
        return result;
    }

    public INetworkSession EndCreate(IAsyncResult result) => ((CompletedAsyncResult<LocalNetworkSession>)result).Value;

    public IAsyncResult BeginFind(NetworkSessionType sessionType, int maxLocalGamers, NetworkSessionProperties searchProperties, AsyncCallback callback, object asyncState)
    {
        // No real LAN discovery yet - report "found nothing" rather than
        // failing outright, so the Searching/ListGames screens can show a
        // normal empty results list instead of an error.
        CompletedAsyncResult<IReadOnlyList<IAvailableNetworkSession>> result = new CompletedAsyncResult<IReadOnlyList<IAvailableNetworkSession>>(Array.Empty<IAvailableNetworkSession>(), asyncState);
        callback?.Invoke(result);
        return result;
    }

    public IReadOnlyList<IAvailableNetworkSession> EndFind(IAsyncResult result) => ((CompletedAsyncResult<IReadOnlyList<IAvailableNetworkSession>>)result).Value;

    public IAsyncResult BeginJoin(IAvailableNetworkSession session, AsyncCallback callback, object asyncState)
    {
        CompletedAsyncResult<object> result = new CompletedAsyncResult<object>(null, asyncState);
        callback?.Invoke(result);
        return result;
    }

    public INetworkSession EndJoin(IAsyncResult result) => throw new InvalidOperationException("Joining another player's game over LAN isn't available yet.");

    // No invite concept for a local-only/Practice session - see INetworkSession.cs.
    public IAsyncResult BeginJoinInvite(object inviteToken, AsyncCallback callback, object asyncState)
    {
        CompletedAsyncResult<object> result = new CompletedAsyncResult<object>(null, asyncState);
        callback?.Invoke(result);
        return result;
    }
}
