using System;
using System.Collections.Generic;

namespace ZP2K9.net;

// PC replacement for the multiplayer transport surface the game used to get
// from Microsoft.Xna.Framework.Net.NetworkSession (Xbox Live / System Link).
// MonoGame doesn't ship that namespace, so instead of hard-coding a new
// transport directly into NetSession.cs/NetPlay.cs, that code is written
// against this interface. It's sized to exactly the members NetSession.cs,
// NetPlay.cs, and the multiplayer menu screens actually used - nothing more.
//
// Backends implement INetworkSessionFactory + INetworkSession:
//   1. (done) LocalNetworkSessionFactory (see LocalNetworkSession.cs) - an
//      in-process, host-only session with no real socket transport. Turns
//      out Practice/solo hosting is NOT actually offline from this
//      interface's point of view - on the original Xbox, starting a
//      bots-only match still went through NetworkSession.Create(SystemLink,
//      ...) like real multiplayer hosting (NetSession.CreateSession() has no
//      separate "local-only" path), it just always succeeded instantly
//      since XNA didn't need a second machine to create a session, only to
//      join one. The old NotConfiguredNetworkSessionFactory stub threw for
//      every Create call, which broke Practice along with real multiplayer.
//      Kept around, unused, as a template/fallback.
//   2. (done, 2026-08-23) LanNetworkSessionFactory (see
//      LanNetworkSession.cs) - real LAN multiplayer: a second machine
//      actually discovering and joining a hosted session over the local
//      network, host+clients talking over plain TCP/UDP sockets (no
//      third-party transport library). This is NetworkBackend.Current's
//      default now. See that file's header comment for the wire protocol
//      and threading model.
//   3. (done, 2026-08-25) a Steamworks backend (lobbies + P2P), swapped in
//      behind the same interface without touching gameplay/menu code again
//      - see SteamNetworkSession.cs.
//
// NotConfiguredNetworkSessionFactory (below) is kept around as a harmless,
// unused "hard off" stub, in case it's ever useful to force every
// Create/Find/Join to fail cleanly again for testing.
//
// OpenInviteOverlay()/BeginJoinInvite() (2026-08-25): the "Invite Friends"
// pause-menu item and Steam's own "Join Game" friend-notification flow.
// OpenInviteOverlay() opens whatever this backend's native invite UI is for
// the session it's currently in - only Steam has one (SteamNetworkSession's
// SteamFriends.OpenGameInviteOverlay call); Lan/Local are harmless no-ops
// since there's nothing to invite anyone into. BeginJoinInvite() is the
// receiving half: it joins using an opaque, backend-specific token (a boxed
// Steamworks.Data.Lobby for Steam) captured by whatever raised
// NetworkBackend.InviteAccepted, so this file - and NetSession.cs, which
// calls both - never needs a Steamworks reference. Completion reuses each
// backend's existing EndJoin(), since BeginJoinInvite returns the exact same
// kind of IAsyncResult BeginJoin does.

public enum NetworkSessionType
{
    Local = 0,
    SystemLink = 1,
    PlayerMatch = 2,
    Ranked = 3
}

public enum NetworkSessionState
{
    Lobby = 0,
    Playing = 1,
    Ended = 2
}

// Simple slot-indexed bag of nullable ints, matching how the original XNA
// NetworkSessionProperties was used here (index 0 = protocol version, index
// 1 = game type - see NetSession.VERSION / GameState.gameType).
public class NetworkSessionProperties
{
    private readonly int?[] _values = new int?[8];

    public int? this[int index]
    {
        get => _values[index];
        set => _values[index] = value;
    }
}

public class NetworkQualityOfService
{
    public bool IsAvailable { get; init; }
    public TimeSpan AverageRoundtripTime { get; init; }
}

public interface INetworkGamer
{
    byte Id { get; }
    string Gamertag { get; }
    bool IsHost { get; }

    // No PC voice chat yet - backends can just always report false here.
    bool IsTalking { get; }
    TimeSpan RoundtripTime { get; }
}

public interface ILocalNetworkGamer : INetworkGamer
{
    bool IsDataAvailable { get; }
    void SendData(PacketWriter writer, SendDataOptions options, INetworkGamer recipient);
    void ReceiveData(PacketReader reader, out INetworkGamer sender);
}

public class GamerJoinedEventArgs : EventArgs
{
    public INetworkGamer Gamer { get; }

    public GamerJoinedEventArgs(INetworkGamer gamer)
    {
        Gamer = gamer;
    }
}

public class GamerLeftEventArgs : EventArgs
{
    public INetworkGamer Gamer { get; }

    public GamerLeftEventArgs(INetworkGamer gamer)
    {
        Gamer = gamer;
    }
}

public interface IAvailableNetworkSession
{
    string HostGamertag { get; }
    int CurrentGamerCount { get; }
    int OpenPublicGamerSlots { get; }
    NetworkSessionProperties SessionProperties { get; }
    NetworkQualityOfService QualityOfService { get; }
}

public interface INetworkSession : IDisposable
{
    bool IsHost { get; }
    bool IsDisposed { get; }
    bool AllowJoinInProgress { get; set; }
    NetworkSessionState SessionState { get; }

    IReadOnlyList<ILocalNetworkGamer> LocalGamers { get; }
    IReadOnlyList<INetworkGamer> RemoteGamers { get; }
    IReadOnlyList<INetworkGamer> AllGamers { get; }

    NetworkSessionProperties SessionProperties { get; }

    float BytesPerSecondSent { get; }
    float BytesPerSecondReceived { get; }

    event EventHandler<GamerJoinedEventArgs> GamerJoined;
    event EventHandler<GamerLeftEventArgs> GamerLeft;

    void Update();
    void StartGame();
    void EndGame();

    // Opens this backend's native "invite a friend" UI for whatever session
    // this is, if it has one. See the header comment above for the full
    // picture; a no-op is always a safe implementation.
    void OpenInviteOverlay();
}

// The Begin/End async pattern mirrors the original XNA calls in
// NetSession.cs one-for-one, so that file's control flow barely changes -
// only the receiver (a factory instance instead of static NetworkSession
// calls) does.
public interface INetworkSessionFactory
{
    IAsyncResult BeginCreate(NetworkSessionType sessionType, int maxLocalGamers, int maxGamers, int privateGamerSlots, NetworkSessionProperties properties, AsyncCallback callback, object asyncState);
    INetworkSession EndCreate(IAsyncResult result);

    IAsyncResult BeginFind(NetworkSessionType sessionType, int maxLocalGamers, NetworkSessionProperties searchProperties, AsyncCallback callback, object asyncState);
    IReadOnlyList<IAvailableNetworkSession> EndFind(IAsyncResult result);

    IAsyncResult BeginJoin(IAvailableNetworkSession session, AsyncCallback callback, object asyncState);
    INetworkSession EndJoin(IAsyncResult result);

    // Joins using an opaque token captured off a backend-specific "the local
    // player accepted an invite" event instead of a BeginFind() result - see
    // the header comment above. Completes through the SAME EndJoin() above
    // (not a separate EndJoinInvite) - the returned IAsyncResult is always
    // the kind that backend's own EndJoin already knows how to unwrap.
    IAsyncResult BeginJoinInvite(object inviteToken, AsyncCallback callback, object asyncState);
}

// A no-op IAsyncResult for factories that can answer synchronously (the
// NotConfigured stub below, and the platform-services stubs in
// PlatformServices.cs) without a real async operation behind them.
public sealed class CompletedAsyncResult<T> : IAsyncResult
{
    public T Value { get; }
    public object AsyncState { get; }
    public System.Threading.WaitHandle AsyncWaitHandle => null;
    public bool CompletedSynchronously => true;
    public bool IsCompleted => true;

    public CompletedAsyncResult(T value, object asyncState)
    {
        Value = value;
        AsyncState = asyncState;
    }
}

// Not used by default anymore (see the header comment above) - kept as an
// opt-in stub that fails every multiplayer entry point cleanly with a clear
// message, for whenever that's useful to force during testing.
public sealed class NotConfiguredNetworkSessionFactory : INetworkSessionFactory
{
    public const string Message = "Multiplayer isn't available in this build yet.";

    public IAsyncResult BeginCreate(NetworkSessionType sessionType, int maxLocalGamers, int maxGamers, int privateGamerSlots, NetworkSessionProperties properties, AsyncCallback callback, object asyncState)
    {
        var result = new CompletedAsyncResult<object>(null, asyncState);
        callback?.Invoke(result);
        return result;
    }

    public INetworkSession EndCreate(IAsyncResult result) => throw new InvalidOperationException(Message);

    public IAsyncResult BeginFind(NetworkSessionType sessionType, int maxLocalGamers, NetworkSessionProperties searchProperties, AsyncCallback callback, object asyncState)
    {
        var result = new CompletedAsyncResult<object>(null, asyncState);
        callback?.Invoke(result);
        return result;
    }

    public IReadOnlyList<IAvailableNetworkSession> EndFind(IAsyncResult result) => throw new InvalidOperationException(Message);

    public IAsyncResult BeginJoin(IAvailableNetworkSession session, AsyncCallback callback, object asyncState)
    {
        var result = new CompletedAsyncResult<object>(null, asyncState);
        callback?.Invoke(result);
        return result;
    }

    public INetworkSession EndJoin(IAsyncResult result) => throw new InvalidOperationException(Message);

    public IAsyncResult BeginJoinInvite(object inviteToken, AsyncCallback callback, object asyncState)
    {
        var result = new CompletedAsyncResult<object>(null, asyncState);
        callback?.Invoke(result);
        return result;
    }
}

public static class NetworkBackend
{
    public static INetworkSessionFactory Current { get; set; } = new LanNetworkSessionFactory();

    // Stand-in for the old static NetworkSession.InviteAccepted event.
    // Raised for real now by SteamNetworkSession.cs's
    // SteamFriends.OnGameLobbyJoinRequested handler (see SteamInit there),
    // via RaiseInviteAccepted below - Game1.cs's HandleInvite/
    // FinishHandleInvite and NetSession.JoinInvite were already written
    // against this event as inert scaffolding waiting for exactly this.
    public static event EventHandler InviteAccepted;

    // Event fields can only be raised from their declaring class, so
    // whoever actually detects an accepted invite (currently just
    // SteamNetworkSession.cs, a different class in this same namespace)
    // goes through this instead of invoking InviteAccepted directly.
    internal static void RaiseInviteAccepted(EventArgs e)
    {
        InviteAccepted?.Invoke(null, e);
    }
}
