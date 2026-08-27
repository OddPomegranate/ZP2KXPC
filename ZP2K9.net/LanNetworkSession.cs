using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using ZP2K9.platform;

namespace ZP2K9.net;

// Second INetworkSessionFactory backend (see the roadmap comment at the top
// of INetworkSession.cs): real multiplayer over the local network, built
// entirely on plain BCL sockets (TcpListener/TcpClient/UdpClient) rather than
// a third-party transport library. That's a deliberate choice, not a
// simplification for its own sake - this file can't be compiled or run
// anywhere in the environment it was written in, so every API used here
// needed to be one I'm completely certain about the shape of. Once this
// compiles and a real two-machine test happens, error messages from an
// actual build are the fastest way to fix anything wrong below.
//
// Topology: a single TCP connection per client, always to the host (a star,
// not a mesh). A client talking to another client's gamer relays through the
// host (LanMsg.Data with a target id that isn't the host's). LAN discovery
// (BeginFind) is a separate, tiny UDP broadcast/response exchange on its own
// fixed port, only used to learn a host's IP before BeginJoin connects to it
// directly over TCP.
//
// Threading: none, on purpose. Every socket is only ever touched from the
// main game thread, inside Update() (host/client message pumping) or inside
// an IAsyncResult.IsCompleted getter (the Create/Find/Join polling below) -
// both of which are already called once per frame by NetSession.cs. Reads
// are non-blocking by only ever consuming bytes TcpClient.Available/
// UdpClient.Available already reports as buffered, so nothing here can stall
// the render thread waiting on the network. The one exception is the initial
// TcpClient.ConnectAsync() call in BeginJoin, which is genuinely async (backed
// by the OS, not a thread I spawn) and polled the same lazy way.
//
// IMPORTANT for whoever debugs the first build of this: an IAsyncResult's
// IsCompleted getter must never throw - NetSession.Update() checks
// `if (pendingJoin && joinResult.IsCompleted)` with no try/catch around that
// condition, only around the EndJoin() call after it. LanOpResult below
// captures any failure from its poll function into .Error instead of letting
// it escape, and EndCreate/EndFind/EndJoin are what actually throw it.

internal enum LanMsg : byte
{
	// Client -> host, immediately after the TCP connection is up.
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
	// Host -> a still-pending connection that tried to Hello into a session
	// that's already at its gamer cap. The host closes the connection right
	// after sending this.
	Full = 6
}

// Ports and small shared constants used by both the factory (Create/Find/
// Join) and LanNetworkSession's own per-frame pumping.
internal static class LanConfig
{
	// Fixed rather than negotiated, to keep BeginJoin simple: a client only
	// needs the host's IP address (learned from a discovery response) to
	// know where to connect. Two hosts on the SAME machine at the same time
	// would collide on these ports - not a scenario this game needs to
	// support (two PCs on one LAN, one hosting, is the target).
	public const int DiscoveryPort = 38610;
	public const int SessionPort = 38611;

	public const byte DiscoveryRequestTag = 0xAA;
	public const byte DiscoveryResponseTag = 0xBB;

	// How long BeginFind collects discovery responses before EndFind can be
	// called; how long BeginJoin waits for a TCP connect + Welcome before
	// giving up.
	public const double DiscoveryWindowSeconds = 2.0;
	public const double ConnectTimeoutSeconds = 8.0;

	// Sentinel target id for a Data message meant for every other gamer
	// (never actually sent anywhere in the game today - every SendData call
	// site addresses a specific recipient - but implemented properly in case
	// that ever changes). Real gamer ids only ever run 0..maxGamers-1, so
	// this is always distinguishable from a real one.
	public const byte BroadcastId = 255;

	public const int MaxFrameBytes = 8 * 1024 * 1024;
}

// Builds one length-prefixed wire frame: [int32 length][byte LanMsg][body...].
// The length prefix covers everything after itself (the tag byte + body), so
// a receiver knows exactly how many bytes to buffer before the frame is
// parseable, without needing any other framing convention.
internal static class LanFrame
{
	public static byte[] Build(LanMsg type, Action<BinaryWriter> writeBody)
	{
		byte[] payload;
		using (MemoryStream body = new MemoryStream())
		{
			using (BinaryWriter bw = new BinaryWriter(body, System.Text.Encoding.UTF8, leaveOpen: true))
			{
				bw.Write((byte)type);
				writeBody?.Invoke(bw);
			}
			payload = body.ToArray();
		}
		byte[] frame = new byte[4 + payload.Length];
		Buffer.BlockCopy(BitConverter.GetBytes(payload.Length), 0, frame, 0, 4);
		Buffer.BlockCopy(payload, 0, frame, 4, payload.Length);
		return frame;
	}

	public static void WriteNullableInt(BinaryWriter bw, int? value)
	{
		bw.Write(value.HasValue);
		bw.Write(value ?? 0);
	}

	public static int? ReadNullableInt(BinaryReader br)
	{
		bool has = br.ReadBoolean();
		int v = br.ReadInt32();
		return has ? v : (int?)null;
	}
}

// One TCP connection (host<->client, in either direction) with its own
// non-blocking, incremental frame reader. Deliberately NOT thread-based:
// Pump() only ever reads bytes TcpClient.Available already reports as
// buffered, so it never blocks, and it's only ever called from Update()/the
// join-polling code on the main thread. That sidesteps every kind of cross-
// thread synchronization bug in code nobody here can compile-test.
internal sealed class LanConnection
{
	public byte GamerId;
	public string Gamertag;
	public bool Disconnected { get; private set; }

	public readonly Queue<byte[]> Inbox = new Queue<byte[]>();

	private readonly TcpClient _client;
	private readonly NetworkStream _stream;

	private readonly byte[] _lenBuf = new byte[4];
	private int _lenBufFilled;
	private byte[] _bodyBuf;
	private int _bodyBufFilled;

	public LanConnection(TcpClient client)
	{
		_client = client;
		_client.NoDelay = true;
		_stream = client.GetStream();
	}

	// Reads whatever is already sitting in the socket buffer (never blocks)
	// and accumulates it into complete frames across as many calls as it
	// takes. Safe to call every frame even when nothing has arrived.
	public void Pump()
	{
		if (Disconnected)
		{
			return;
		}
		try
		{
			int avail = _client.Available;
			while (avail > 0)
			{
				if (_bodyBuf == null)
				{
					int want = 4 - _lenBufFilled;
					int toRead = Math.Min(want, avail);
					int read = _stream.Read(_lenBuf, _lenBufFilled, toRead);
					if (read <= 0)
					{
						Disconnected = true;
						return;
					}
					_lenBufFilled += read;
					avail -= read;
					if (_lenBufFilled < 4)
					{
						break;
					}
					int len = BitConverter.ToInt32(_lenBuf, 0);
					if (len < 0 || len > LanConfig.MaxFrameBytes)
					{
						Disconnected = true;
						return;
					}
					_lenBufFilled = 0;
					_bodyBuf = len == 0 ? Array.Empty<byte>() : new byte[len];
					_bodyBufFilled = 0;
					if (len == 0)
					{
						Inbox.Enqueue(_bodyBuf);
						_bodyBuf = null;
						continue;
					}
				}
				int wantBody = _bodyBuf.Length - _bodyBufFilled;
				if (wantBody <= 0)
				{
					Inbox.Enqueue(_bodyBuf);
					_bodyBuf = null;
					continue;
				}
				int toReadBody = Math.Min(wantBody, avail);
				if (toReadBody <= 0)
				{
					break;
				}
				int readBody = _stream.Read(_bodyBuf, _bodyBufFilled, toReadBody);
				if (readBody <= 0)
				{
					Disconnected = true;
					return;
				}
				_bodyBufFilled += readBody;
				avail -= readBody;
				if (_bodyBufFilled >= _bodyBuf.Length)
				{
					Inbox.Enqueue(_bodyBuf);
					_bodyBuf = null;
				}
			}
		}
		catch
		{
			Disconnected = true;
		}
	}

	public void Send(byte[] frame)
	{
		if (Disconnected)
		{
			return;
		}
		try
		{
			_stream.Write(frame, 0, frame.Length);
		}
		catch
		{
			Disconnected = true;
		}
	}

	public void Close()
	{
		Disconnected = true;
		try
		{
			_client.Close();
		}
		catch
		{
		}
	}
}

// A remote gamer (host's view of a client, or a client's view of anyone
// other than itself). Pure data - all the actual send/receive routing lives
// on LanNetworkSession/LanLocalGamer.
internal sealed class LanNetworkGamer : INetworkGamer
{
	public byte Id { get; }
	public string Gamertag { get; }
	public bool IsHost => Id == 0;
	public bool IsTalking => false;
	public TimeSpan RoundtripTime => TimeSpan.Zero;

	public LanNetworkGamer(byte id, string gamertag)
	{
		Id = id;
		Gamertag = gamertag;
	}
}

// The local player's own gamer object (host's local gamer, or a client's).
// Host convention: the host's local gamer is always id 0; clients are
// assigned 1, 2, 3... by the host as they join.
internal sealed class LanLocalGamer : ILocalNetworkGamer
{
	public byte Id { get; }
	public string Gamertag { get; }
	public bool IsHost => Id == 0;
	public bool IsTalking => false;
	public TimeSpan RoundtripTime => TimeSpan.Zero;

	public bool IsDataAvailable => Inbox.Count > 0;

	// (senderId, raw PacketWriter bytes) for each message not yet drained by
	// NetPlay.cs's per-frame `while (val.IsDataAvailable) { ReceiveData... }`
	// loop.
	internal readonly Queue<(byte SenderId, byte[] Data)> Inbox = new Queue<(byte, byte[])>();

	private readonly LanNetworkSession _session;

	internal LanLocalGamer(LanNetworkSession session, byte id, string gamertag)
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

// One entry in a BeginFind() result list - a host that answered a discovery
// broadcast. HostAddress/HostPort (not part of IAvailableNetworkSession) are
// what BeginJoin actually connects to.
internal sealed class LanAvailableNetworkSession : IAvailableNetworkSession
{
	public string HostGamertag { get; }
	public int CurrentGamerCount { get; }
	public int OpenPublicGamerSlots { get; }
	public NetworkSessionProperties SessionProperties { get; }
	public NetworkQualityOfService QualityOfService { get; }

	internal IPAddress HostAddress { get; }
	internal int HostPort { get; }

	internal LanAvailableNetworkSession(string hostGamertag, int currentGamerCount, int openPublicGamerSlots, NetworkSessionProperties properties, IPAddress hostAddress, int hostPort)
	{
		HostGamertag = hostGamertag;
		CurrentGamerCount = currentGamerCount;
		OpenPublicGamerSlots = openPublicGamerSlots;
		SessionProperties = properties;
		QualityOfService = new NetworkQualityOfService { IsAvailable = true, AverageRoundtripTime = TimeSpan.Zero };
		HostAddress = hostAddress;
		HostPort = hostPort;
	}
}

// A lazily-evaluated IAsyncResult: the actual work (advancing a connect/
// handshake/discovery state machine) happens inside the IsCompleted getter
// itself, driven by NetSession.cs polling it once per frame. See the header
// comment at the top of this file for why the getter must never throw.
internal sealed class LanOpResult : IAsyncResult
{
	public object AsyncState { get; }
	public System.Threading.WaitHandle AsyncWaitHandle => null;
	public bool CompletedSynchronously => false;

	// Set by the poll function once real work is done. EndCreate/EndFind/
	// EndJoin cast this to whatever they expect.
	public object Value;

	// Set instead of Value when the poll function threw. EndCreate/EndFind/
	// EndJoin re-throw this from a safe context (they ARE allowed to throw -
	// NetSession.cs wraps those calls in try/catch).
	public Exception Error;

	private readonly Func<bool> _poll;
	private bool _completed;

	public LanOpResult(object asyncState, Func<bool> poll)
	{
		AsyncState = asyncState;
		_poll = poll;
	}

	public bool IsCompleted
	{
		get
		{
			if (_completed)
			{
				return true;
			}
			try
			{
				_completed = _poll();
			}
			catch (Exception ex)
			{
				// TEMP DIAGNOSTIC (2026-08-23): single choke point for every
				// exception thrown out of a Begin*/poll lambda across BOTH
				// backends (Lan* and Steam*) - logs the real type + message
				// so it shows up in the Output window without needing to
				// touch Visual Studio's Exception Settings. Safe to remove
				// once networking is confirmed solid.
				Console.WriteLine("[LanOpResult] poll() threw " + ex.GetType().Name + ": " + ex.Message);
				Error = ex;
				_completed = true;
			}
			return _completed;
		}
	}
}

// Real INetworkSession backend #2: LAN discovery + join over plain TCP/UDP
// sockets, star-topology with the host relaying any client-to-client traffic.
// One instance of this class plays either the host role or the client role -
// never both - decided permanently at construction (CreateHostSide vs
// CreateClientSide below).
public sealed class LanNetworkSession : INetworkSession
{
	private readonly bool _isHost;
	private readonly NetworkSessionProperties _properties;
	private readonly LanLocalGamer _localGamer;
	private readonly List<ILocalNetworkGamer> _localGamers;
	private readonly List<INetworkGamer> _allGamers = new List<INetworkGamer>();
	private readonly List<INetworkGamer> _remoteGamers = new List<INetworkGamer>();

	// Host-only state.
	private TcpListener _listener;
	private UdpClient _discoveryListener;
	private int _maxGamers;
	private byte _nextGamerId = 1; // 0 is always the host's own local gamer.
	private readonly Dictionary<byte, LanConnection> _hostConnections = new Dictionary<byte, LanConnection>();
	private readonly List<PendingConn> _pendingHellos = new List<PendingConn>();

	// Client-only state.
	private LanConnection _hostConnection;

	public bool IsHost => _isHost;
	public bool IsDisposed { get; private set; }
	public bool AllowJoinInProgress { get; set; }
	public NetworkSessionState SessionState { get; private set; } = NetworkSessionState.Lobby;

	public IReadOnlyList<ILocalNetworkGamer> LocalGamers => _localGamers;
	public IReadOnlyList<INetworkGamer> RemoteGamers => _remoteGamers;
	public IReadOnlyList<INetworkGamer> AllGamers => _allGamers;
	public NetworkSessionProperties SessionProperties => _properties;

	// No bandwidth accounting behind this backend yet - BandwidthManager.cs
	// treats 0 as "unknown/unmeasured", same as LocalNetworkSession reports.
	public float BytesPerSecondSent => 0f;
	public float BytesPerSecondReceived => 0f;

	public event EventHandler<GamerJoinedEventArgs> GamerJoined;
	public event EventHandler<GamerLeftEventArgs> GamerLeft;

	private LanNetworkSession(bool isHost, NetworkSessionProperties properties, byte localId, string localGamertag)
	{
		_isHost = isHost;
		_properties = properties;
		_localGamer = new LanLocalGamer(this, localId, localGamertag);
		_localGamers = new List<ILocalNetworkGamer> { _localGamer };
		// The local gamer is part of AllGamers (matches LocalNetworkSession's
		// own pattern) but never RemoteGamers - it's not remote to itself.
		_allGamers.Add(_localGamer);
	}

	internal static LanNetworkSession CreateHostSide(NetworkSessionProperties properties, string localGamertag, TcpListener listener, UdpClient discoveryListener, int maxGamers)
	{
		LanNetworkSession session = new LanNetworkSession(isHost: true, properties, localId: 0, localGamertag);
		session._listener = listener;
		session._discoveryListener = discoveryListener;
		session._maxGamers = Math.Max(1, maxGamers);
		return session;
	}

	internal static LanNetworkSession CreateClientSide(NetworkSessionProperties properties, LanConnection hostConnection, byte localId, string localGamertag)
	{
		LanNetworkSession session = new LanNetworkSession(isHost: false, properties, localId, localGamertag);
		session._hostConnection = hostConnection;
		return session;
	}

	// Called only while building a freshly-joined client session, once per
	// gamer in the Welcome roster - deliberately does NOT raise GamerJoined,
	// since these gamers were already in the session before this client
	// existed from their point of view. NetSession.cs only subscribes to
	// GamerJoined after EndJoin returns, so nothing would see these events
	// anyway, but staying honest about "already there" vs. "just joined"
	// matters for anything that counts joins later.
	internal void AddInitialRemoteGamer(byte id, string gamertag)
	{
		LanNetworkGamer gamer = new LanNetworkGamer(id, gamertag);
		_allGamers.Add(gamer);
		_remoteGamers.Add(gamer);
	}

	public void Update()
	{
		if (IsDisposed)
		{
			return;
		}
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
		while (_listener != null && _listener.Pending())
		{
			TcpClient tcp;
			try
			{
				tcp = _listener.AcceptTcpClient();
			}
			catch
			{
				break;
			}
			_pendingHellos.Add(new PendingConn(new LanConnection(tcp)));
		}

		while (_discoveryListener != null && _discoveryListener.Available > 0)
		{
			IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
			byte[] data;
			try
			{
				data = _discoveryListener.Receive(ref remoteEP);
			}
			catch
			{
				break;
			}
			if (data.Length > 0 && data[0] == LanConfig.DiscoveryRequestTag)
			{
				try
				{
					byte[] resp = BuildDiscoveryResponse();
					_discoveryListener.Send(resp, resp.Length, remoteEP);
				}
				catch
				{
				}
			}
		}

		for (int i = _pendingHellos.Count - 1; i >= 0; i--)
		{
			PendingConn pending = _pendingHellos[i];
			pending.Conn.Pump();
			if (pending.Conn.Disconnected || pending.Age.Elapsed.TotalSeconds > LanConfig.ConnectTimeoutSeconds)
			{
				_pendingHellos.RemoveAt(i);
				continue;
			}
			bool handled = false;
			while (pending.Conn.Inbox.Count > 0)
			{
				byte[] payload = pending.Conn.Inbox.Dequeue();
				if (handled)
				{
					// Already completed this connection's handshake earlier
					// in this same batch (a well-behaved client never sends
					// a second Hello, but don't double-register it if one
					// shows up anyway) - just drain and drop.
					continue;
				}
				if (payload.Length < 1 || (LanMsg)payload[0] != LanMsg.Hello)
				{
					continue;
				}
				string tag;
				using (MemoryStream ms = new MemoryStream(payload, 1, payload.Length - 1))
				using (BinaryReader br = new BinaryReader(ms))
				{
					tag = br.ReadString();
				}
				CompleteJoin(pending.Conn, tag);
				handled = true;
			}
			if (handled)
			{
				_pendingHellos.RemoveAt(i);
			}
		}

		List<byte> toDrop = null;
		foreach (KeyValuePair<byte, LanConnection> kvp in _hostConnections)
		{
			LanConnection conn = kvp.Value;
			conn.Pump();
			while (conn.Inbox.Count > 0)
			{
				HandleHostDataFrame(kvp.Key, conn.Inbox.Dequeue());
			}
			if (conn.Disconnected)
			{
				(toDrop ??= new List<byte>()).Add(kvp.Key);
			}
		}
		if (toDrop != null)
		{
			foreach (byte id in toDrop)
			{
				RemoveRemoteGamer(id, notifyOthers: true);
			}
		}
	}

	private void CompleteJoin(LanConnection conn, string tag)
	{
		if (_allGamers.Count >= _maxGamers)
		{
			conn.Send(LanFrame.Build(LanMsg.Full, null));
			conn.Close();
			return;
		}

		conn.Gamertag = tag;
		byte newId = _nextGamerId++;
		conn.GamerId = newId;

		// Roster snapshot built BEFORE the new gamer is added below, so it
		// never includes the new client itself - just everyone already here.
		byte[] welcome = LanFrame.Build(LanMsg.Welcome, bw =>
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
		conn.Send(welcome);

		byte[] joinedFrame = LanFrame.Build(LanMsg.GamerJoined, bw =>
		{
			bw.Write(newId);
			bw.Write(tag);
		});
		foreach (KeyValuePair<byte, LanConnection> kvp in _hostConnections)
		{
			kvp.Value.Send(joinedFrame);
		}

		// Only now does the new gamer become "real" (routable, and visible
		// to the GamerJoined handler below) - matches the ordering
		// NetSession.netSession_GamerJoined needs, since it turns around and
		// immediately calls SendData(..., gamer) on the same event.
		_hostConnections[newId] = conn;
		LanNetworkGamer newGamer = new LanNetworkGamer(newId, tag);
		_allGamers.Add(newGamer);
		_remoteGamers.Add(newGamer);
		GamerJoined?.Invoke(this, new GamerJoinedEventArgs(newGamer));
	}

	private void HandleHostDataFrame(byte senderId, byte[] payload)
	{
		if (payload.Length < 2 || (LanMsg)payload[0] != LanMsg.Data)
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
		if (targetId == LanConfig.BroadcastId)
		{
			foreach (KeyValuePair<byte, LanConnection> kvp in _hostConnections)
			{
				if (kvp.Key == senderId)
				{
					continue;
				}
				kvp.Value.Send(LanFrame.Build(LanMsg.Data, bw =>
				{
					bw.Write(senderId);
					bw.Write(raw);
				}));
			}
			_localGamer.Inbox.Enqueue((senderId, raw));
			return;
		}
		if (_hostConnections.TryGetValue(targetId, out LanConnection targetConn))
		{
			targetConn.Send(LanFrame.Build(LanMsg.Data, bw =>
			{
				bw.Write(senderId);
				bw.Write(raw);
			}));
		}
	}

	private void UpdateClient()
	{
		if (_hostConnection == null)
		{
			return;
		}
		_hostConnection.Pump();
		while (_hostConnection.Inbox.Count > 0)
		{
			byte[] payload = _hostConnection.Inbox.Dequeue();
			if (payload.Length < 1)
			{
				continue;
			}
			LanMsg type = (LanMsg)payload[0];
			switch (type)
			{
			case LanMsg.GamerJoined:
			{
				byte id;
				string tag;
				using (MemoryStream ms = new MemoryStream(payload, 1, payload.Length - 1))
				using (BinaryReader br = new BinaryReader(ms))
				{
					id = br.ReadByte();
					tag = br.ReadString();
				}
				LanNetworkGamer gamer = new LanNetworkGamer(id, tag);
				_allGamers.Add(gamer);
				_remoteGamers.Add(gamer);
				GamerJoined?.Invoke(this, new GamerJoinedEventArgs(gamer));
				break;
			}
			case LanMsg.GamerLeft:
				if (payload.Length >= 2)
				{
					RemoveRemoteGamer(payload[1], notifyOthers: false);
				}
				break;
			case LanMsg.Data:
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
				// Welcome/Full only ever matter during the join handshake
				// itself (handled inline by BeginJoin's poll function below,
				// before this session object even exists) - anything else
				// arriving here is stale or a protocol mismatch, safe to
				// ignore rather than fail the whole session over.
				break;
			}
		}
		if (_hostConnection.Disconnected)
		{
			// INetworkSession has no "host vanished" event to raise - the
			// safest thing to do without one is stop pumping and mark this
			// session disposed, same end state as NetSession.Kill() leaves
			// it in. Whatever reacts to a stalled game (menu/timeout logic
			// in NetSession.cs) is what surfaces this to the player; this
			// class doesn't try to.
			IsDisposed = true;
		}
	}

	private void RemoveRemoteGamer(byte id, bool notifyOthers)
	{
		if (_isHost)
		{
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
			byte[] leftFrame = LanFrame.Build(LanMsg.GamerLeft, bw => bw.Write(id));
			foreach (KeyValuePair<byte, LanConnection> kvp in _hostConnections)
			{
				kvp.Value.Send(leftFrame);
			}
		}
		GamerLeft?.Invoke(this, new GamerLeftEventArgs(gamer));
	}

	// Routes one outgoing SendData call. Host: unicast straight to the
	// target's connection (or relay-broadcast). Client: everything goes to
	// the host first, tagged with who it's really for, and the host relays
	// it on if that's not itself - see HandleHostDataFrame above.
	internal void SendData(LanLocalGamer sender, byte[] data, INetworkGamer recipient)
	{
		if (IsDisposed)
		{
			return;
		}
		byte targetId = recipient?.Id ?? LanConfig.BroadcastId;

		if (_isHost)
		{
			if (targetId == _localGamer.Id)
			{
				_localGamer.Inbox.Enqueue((_localGamer.Id, data));
				return;
			}
			if (targetId == LanConfig.BroadcastId)
			{
				foreach (KeyValuePair<byte, LanConnection> kvp in _hostConnections)
				{
					kvp.Value.Send(LanFrame.Build(LanMsg.Data, bw =>
					{
						bw.Write(_localGamer.Id);
						bw.Write(data);
					}));
				}
				return;
			}
			if (_hostConnections.TryGetValue(targetId, out LanConnection conn))
			{
				conn.Send(LanFrame.Build(LanMsg.Data, bw =>
				{
					bw.Write(_localGamer.Id);
					bw.Write(data);
				}));
			}
			return;
		}

		_hostConnection?.Send(LanFrame.Build(LanMsg.Data, bw =>
		{
			bw.Write(targetId);
			bw.Write(data);
		}));
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

	private byte[] BuildDiscoveryResponse()
	{
		using MemoryStream ms = new MemoryStream();
		using (BinaryWriter bw = new BinaryWriter(ms, System.Text.Encoding.UTF8, leaveOpen: true))
		{
			bw.Write(LanConfig.DiscoveryResponseTag);
			bw.Write(_localGamer.Gamertag);
			bw.Write(_allGamers.Count);
			int openSlots = Math.Max(0, _maxGamers - _allGamers.Count);
			bw.Write(openSlots);
			LanFrame.WriteNullableInt(bw, _properties?[0]);
			LanFrame.WriteNullableInt(bw, _properties?[1]);
		}
		return ms.ToArray();
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
		if (IsDisposed)
		{
			return;
		}
		IsDisposed = true;
		if (_isHost)
		{
			try
			{
				_listener?.Stop();
			}
			catch
			{
			}
			try
			{
				_discoveryListener?.Close();
			}
			catch
			{
			}
			foreach (KeyValuePair<byte, LanConnection> kvp in _hostConnections)
			{
				kvp.Value.Close();
			}
			_hostConnections.Clear();
			foreach (PendingConn pending in _pendingHellos)
			{
				pending.Conn.Close();
			}
			_pendingHellos.Clear();
		}
		else
		{
			_hostConnection?.Close();
		}
	}

	// LAN has no Steam-style invite overlay - players find each other via
	// the server browser instead. See INetworkSession.cs.
	public void OpenInviteOverlay()
	{
	}

	private readonly struct PendingConn
	{
		public readonly LanConnection Conn;
		public readonly Stopwatch Age;

		public PendingConn(LanConnection conn)
		{
			Conn = conn;
			Age = Stopwatch.StartNew();
		}
	}
}

// The factory NetworkBackend.Current is set to (see the bottom edit to
// INetworkSession.cs). BeginCreate is synchronous like LocalNetworkSession's
// (host-side setup is just opening a couple of sockets - effectively
// instant), while BeginFind/BeginJoin are genuinely async, polled lazily via
// LanOpResult per the header comment at the top of this file.
public sealed class LanNetworkSessionFactory : INetworkSessionFactory
{
	private static string LocalGamertag()
	{
		return Gamer.SignedInGamers.Count > 0 ? Gamer.SignedInGamers[0].Gamertag : "Player";
	}

	public IAsyncResult BeginCreate(NetworkSessionType sessionType, int maxLocalGamers, int maxGamers, int privateGamerSlots, NetworkSessionProperties properties, AsyncCallback callback, object asyncState)
	{
		LanOpResult result;
		try
		{
			TcpListener listener = new TcpListener(IPAddress.Any, LanConfig.SessionPort);
			listener.Start();

			UdpClient discovery = new UdpClient();
			discovery.ExclusiveAddressUse = false;
			discovery.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
			discovery.Client.Bind(new IPEndPoint(IPAddress.Any, LanConfig.DiscoveryPort));

			LanNetworkSession session = LanNetworkSession.CreateHostSide(properties, LocalGamertag(), listener, discovery, maxGamers);
			result = new LanOpResult(asyncState, () => true) { Value = session };
		}
		catch (Exception ex)
		{
			result = new LanOpResult(asyncState, () => true) { Error = ex };
		}
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
		UdpClient client = null;
		Exception setupError = null;
		try
		{
			client = new UdpClient();
			client.EnableBroadcast = true;
		}
		catch (Exception ex)
		{
			setupError = ex;
		}

		List<IAvailableNetworkSession> found = new List<IAvailableNetworkSession>();
		Stopwatch sw = Stopwatch.StartNew();
		bool requestSent = false;

		LanOpResult result = new LanOpResult(asyncState, () =>
		{
			if (setupError != null)
			{
				throw setupError;
			}
			if (!requestSent)
			{
				try
				{
					byte[] req = { LanConfig.DiscoveryRequestTag };
					client.Send(req, req.Length, new IPEndPoint(IPAddress.Broadcast, LanConfig.DiscoveryPort));
				}
				catch
				{
					// No usable network adapter, or a firewall blocking the
					// broadcast - treat it like "nothing found" rather than
					// failing the whole Searching screen.
				}
				requestSent = true;
			}
			while (client.Available > 0)
			{
				IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
				byte[] data;
				try
				{
					data = client.Receive(ref remoteEP);
				}
				catch
				{
					break;
				}
				if (data.Length > 1 && data[0] == LanConfig.DiscoveryResponseTag)
				{
					using MemoryStream ms = new MemoryStream(data, 1, data.Length - 1);
					using BinaryReader br = new BinaryReader(ms);
					string hostTag = br.ReadString();
					int count = br.ReadInt32();
					int openSlots = br.ReadInt32();
					NetworkSessionProperties props = new NetworkSessionProperties
					{
						[0] = LanFrame.ReadNullableInt(br),
						[1] = LanFrame.ReadNullableInt(br)
					};
					found.Add(new LanAvailableNetworkSession(hostTag, count, openSlots, props, remoteEP.Address, LanConfig.SessionPort));
				}
			}
			if (sw.Elapsed.TotalSeconds < LanConfig.DiscoveryWindowSeconds)
			{
				return false;
			}
			try
			{
				client.Close();
			}
			catch
			{
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
		LanAvailableNetworkSession lanSession = session as LanAvailableNetworkSession;
		TcpClient client = new TcpClient();
		System.Threading.Tasks.Task connectTask = null;
		int phase = 0; // 0 = connecting, 1 = waiting for Welcome.
		LanConnection conn = null;
		string myTag = LocalGamertag();
		Stopwatch sw = Stopwatch.StartNew();

		LanOpResult result = null;
		result = new LanOpResult(asyncState, () =>
		{
			if (lanSession == null)
			{
				throw new InvalidOperationException("That listing can't be joined.");
			}
			if (phase == 0)
			{
				if (connectTask == null)
				{
					connectTask = client.ConnectAsync(lanSession.HostAddress, lanSession.HostPort);
				}
				if (!connectTask.IsCompleted)
				{
					if (sw.Elapsed.TotalSeconds > LanConfig.ConnectTimeoutSeconds)
					{
						throw new InvalidOperationException("Couldn't reach that game (timed out connecting).");
					}
					return false;
				}
				if (connectTask.IsFaulted || !client.Connected)
				{
					throw new InvalidOperationException("Couldn't reach that game.");
				}
				conn = new LanConnection(client);
				conn.Send(LanFrame.Build(LanMsg.Hello, bw => bw.Write(myTag)));
				phase = 1;
				return false;
			}

			conn.Pump();
			if (conn.Disconnected)
			{
				throw new InvalidOperationException("Lost connection while joining that game.");
			}
			while (conn.Inbox.Count > 0)
			{
				byte[] payload = conn.Inbox.Dequeue();
				if (payload.Length < 1)
				{
					continue;
				}
				LanMsg type = (LanMsg)payload[0];
				if (type == LanMsg.Full)
				{
					throw new InvalidOperationException("That game is full.");
				}
				if (type == LanMsg.Welcome)
				{
					byte myId;
					int count;
					using MemoryStream ms = new MemoryStream(payload, 1, payload.Length - 1);
					using BinaryReader br = new BinaryReader(ms);
					myId = br.ReadByte();
					count = br.ReadInt32();
					LanNetworkSession newSession = LanNetworkSession.CreateClientSide(lanSession.SessionProperties, conn, myId, myTag);
					for (int i = 0; i < count; i++)
					{
						byte id = br.ReadByte();
						string tag = br.ReadString();
						newSession.AddInitialRemoteGamer(id, tag);
					}
					result.Value = newSession;
					return true;
				}
			}
			if (sw.Elapsed.TotalSeconds > LanConfig.ConnectTimeoutSeconds)
			{
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

	// No invite concept over LAN - see INetworkSession.cs.
	public IAsyncResult BeginJoinInvite(object inviteToken, AsyncCallback callback, object asyncState)
	{
		LanOpResult failedInvite = new LanOpResult(asyncState, () => true)
		{
			Error = new InvalidOperationException("Invites aren't available over LAN - use the server browser instead.")
		};
		callback?.Invoke(failedInvite);
		return failedInvite;
	}
}
