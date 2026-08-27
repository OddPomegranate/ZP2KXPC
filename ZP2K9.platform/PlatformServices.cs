using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ZP2K9.platform;

// PC replacements for the handful of Microsoft.Xna.Framework.GamerServices /
// Microsoft.Xna.Framework.Storage types this game used for things that have
// nothing to do with multiplayer transport (that's INetworkSession, in
// ZP2K9.net) - the Xbox "Guide" system UI (sign-in blade, on-screen
// keyboard, storage device picker, achievement notifications) and the
// Gamer/SignedInGamer identity model. MonoGame doesn't ship either
// namespace, so - same trick as ZP2K9.net's PacketWriter/PacketReader -
// these are same-named drop-ins in our own namespace: every calling file
// just swaps its `using Microsoft.Xna.Framework.GamerServices;` (and
// `...Storage;`) for `using ZP2K9.platform;`.
//
// These are intentionally thin placeholders, not a finished PC experience:
//   - Guide.ShowFriends (2026-08-25): superseded, not removed - the pause
//     menu's "Invite Friends" item now calls NetSession.InviteFriends()
//     instead (see GameMain.cs), which delegates to whatever the active
//     INetworkSession backend's OpenInviteOverlay() does (a real Steam
//     overlay invite for SteamNetworkSession, a harmless no-op for
//     Lan/Local - see INetworkSession.cs). Left here as a no-op in case
//     anything still calls it directly. The invite-ACCEPTING half of this
//     - NetworkBackend.InviteAccepted - is real now too, raised by
//     SteamNetworkSession.cs's SteamFriends.OnGameLobbyJoinRequested
//     handler; InviteAcceptedEventArgs.LobbyToken (below) is what carries
//     the target lobby from there through to NetSession.JoinInvite.
//   - Guide.BeginShowKeyboardInput/EndShowKeyboardInput: now a REAL PC
//     text-entry widget (KeyboardOverlay.cs, this same folder) instead of a
//     placeholder - physical-keyboard typing via GameWindow.TextInput, an
//     on-screen box drawn on top of everything, Enter to confirm/Escape to
//     cancel. Used for clan tag / map rename / class name entry.
//   - Guide.BeginShowStorageDeviceSelector: always "succeeds" with a single
//     always-connected device backed by a local folder - there's no
//     device-picker concept on PC.
// None of that changes how NetSession/NetPlay/the menus are structured, so
// filling these in later never means touching the calling files again.

public enum NotificationPosition
{
    TopLeft = 0,
    TopCenter = 1,
    TopRight = 2,
    MiddleLeft = 3,
    Center = 4,
    MiddleRight = 5,
    BottomLeft = 6,
    BottomCenter = 7,
    BottomRight = 8
}

public class Gamer
{
    public string Gamertag { get; init; }

    public static SignedInGamerCollection SignedInGamers { get; } = new SignedInGamerCollection();
}

public class Privileges
{
    public bool AllowOnlineSessions => true;
}

public class SignedInGamer : Gamer
{
    public PlayerIndex PlayerIndex { get; init; }
    public bool IsSignedInToLive { get; init; }
    public Privileges Privileges { get; } = new Privileges();
}

// PC stand-in for XNA's SignedInGamerCollection. There is exactly one local
// player on PC (PlayerIndex.One) - no per-controller Xbox Live sign-in
// ceremony - so slot 0 is always occupied and slots 1-3 are always empty.
// Both indexers from the real type are kept because the game calls both:
// `this[int]` for iterating "currently signed in gamers", `this[PlayerIndex]`
// for "who's signed in on this specific controller" (nullable).
public class SignedInGamerCollection
{
    private readonly SignedInGamer[] _slots = new SignedInGamer[4];

    public SignedInGamerCollection()
    {
        _slots[0] = new SignedInGamer
        {
            PlayerIndex = PlayerIndex.One,
            Gamertag = "Player",
            IsSignedInToLive = true
        };
    }

    public int Count
    {
        get
        {
            int count = 0;
            foreach (SignedInGamer slot in _slots)
            {
                if (slot != null)
                {
                    count++;
                }
            }
            return count;
        }
    }

    public SignedInGamer this[int position]
    {
        get
        {
            int seen = 0;
            foreach (SignedInGamer slot in _slots)
            {
                if (slot == null)
                {
                    continue;
                }
                if (seen == position)
                {
                    return slot;
                }
                seen++;
            }
            throw new ArgumentOutOfRangeException(nameof(position));
        }
    }

    public SignedInGamer this[PlayerIndex slot] => _slots[(int)slot];
}

public class StorageContainer : IDisposable
{
    public string Path { get; }

    public StorageContainer(string name)
    {
        Path = System.IO.Path.Combine(AppContext.BaseDirectory, "Saves", name);
        System.IO.Directory.CreateDirectory(Path);
    }

    public void Dispose()
    {
    }
}

public class StorageDevice
{
    public bool IsConnected => true;

    public StorageContainer OpenContainer(string name)
    {
        return new StorageContainer(name);
    }
}

public class InviteAcceptedEventArgs : EventArgs
{
    public SignedInGamer Gamer { get; init; }

    // Opaque, backend-specific "what to join" payload - a boxed
    // Steamworks.Data.Lobby when this came from SteamNetworkSession.cs's
    // invite listener. Typed as object (not Steamworks.Data.Lobby) so this
    // platform-agnostic file never needs a Steamworks reference; whichever
    // INetworkSessionFactory.BeginJoinInvite implementation receives it
    // (see NetSession.JoinInvite) is the only place that casts it back.
    public object LobbyToken { get; init; }
}

public static class Guide
{
    // True for real now while the PC keyboard-input overlay (KeyboardOverlay.cs)
    // is up - matches the real Xbox Guide's "blocking" semantics, and the
    // decompiled CharKeys.Update/InterfaceKeys.Update already gate themselves
    // on this exact flag (written for the real Guide, dormant the whole time
    // this was hardcoded false).
    public static bool IsVisible => KeyboardOverlay.IsActive;

    // No demo/trial restriction on a PC build.
    public static bool IsTrialMode => false;

    public static NotificationPosition NotificationPosition { get; set; }

    public static void ShowSignIn(int panes, bool onlineOnly)
    {
        // No-op: PC has exactly one always-signed-in local player (see
        // Gamer.SignedInGamers), so there's nothing to sign into.
    }

    public static void ShowFriends(PlayerIndex player)
    {
        // TODO(steamworks): open the Steam overlay friends list.
    }

    public static IAsyncResult BeginShowKeyboardInput(PlayerIndex player, string title, string description, string defaultText, AsyncCallback callback, object state)
    {
        return KeyboardOverlay.Begin(title, description, defaultText, callback, state);
    }

    public static string EndShowKeyboardInput(IAsyncResult result)
    {
        return ((KeyboardInputResult)result).Value;
    }

    public static IAsyncResult BeginShowStorageDeviceSelector(PlayerIndex player, AsyncCallback callback, object state)
    {
        var result = new ZP2K9.net.CompletedAsyncResult<StorageDevice>(new StorageDevice(), state);
        callback?.Invoke(result);
        return result;
    }

    public static StorageDevice EndShowStorageDeviceSelector(IAsyncResult result)
    {
        return ((ZP2K9.net.CompletedAsyncResult<StorageDevice>)result).Value;
    }
}
