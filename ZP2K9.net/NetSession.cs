using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework;
using ZP2K9.characters;
using ZP2K9.hud.messageHud;
using ZP2K9.menu;
using ZP2K9.menu.levels;
using ZP2K9.platform;

namespace ZP2K9.net;

public class NetSession
{
	public const int VERSION = 206;

	public const int NET_LOCAL = 0;

	public const int NET_EDITOR_TEST = 1;

	public const int NET_SYSTEMLINK = 2;

	public const int NET_LIVE = 3;

	public const int BOT_OFF = 0;

	public const int BOT_REPLACEMENT = 1;

	public const int BOT_MAX = 2;

	public const int DIFF_EASY = 0;

	public const int DIFF_NORMAL = 1;

	public const int DIFF_HARD = 2;

	public const int DIFF_EXPERT = 3;

	public const int FLAG_HOME = 200;

	public const int HILL_OPEN = 0;

	public const int HILL_BLUE = 1;

	public const int HILL_RED = 2;

	public StringBuilder version = new StringBuilder("Port Version: 1.1.0");

	public bool newVersAvailable;

	public StringBuilder[] newAvail = new StringBuilder[4]
	{
		new StringBuilder("New Version Available!"),
		new StringBuilder("-"),
		new StringBuilder("Download from"),
		new StringBuilder("Games Marketplace!")
	};

	public INetworkSession netSession;

	private IAsyncResult createResult;

	private IAsyncResult findResult;

	private IAsyncResult joinResult;

	public bool pendingCreate;

	public bool pendingFind;

	public bool pendingJoin;

	public bool createFailed;

	public bool findFailed;

	public bool joinFailed;

	public bool joinInviteFailed;

	public string failMessage;

	public IReadOnlyList<IAvailableNetworkSession> sessions;

	public NetPlay netPlay;

	private int freeSlot;

	// TEMP DIAGNOSTIC (2026-08-24, "joiner shares host's data over WAN"
	// investigation): GetPlayerOne() below decides which Character[] slot
	// the LOCAL machine treats as "mine" for input/HUD/camera (see its
	// pervasive use as character[netSession.GetPlayerOne()] in Game1.cs).
	// If a client's netPlay.ID never gets set (MSG_INIT lost/discarded -
	// see the matching NetPlay.cs case 2 diagnostics), this silently falls
	// back to slot 0 - the HOST's own character - which would explain
	// "joining player has the same data as host and can't control
	// themselves" exactly. These two bools make sure each state only logs
	// once (GetPlayerOne() runs multiple times per frame) instead of
	// flooding the output. Safe to remove once this is understood.
	private bool _loggedPlayerOneFallback;

	private bool _loggedPlayerOneAssigned;

	public float redTime;

	public float blueTime;

	public int redScore;

	public int blueScore;

	public int netType;

	public Dictionary<byte, int> playerList;

	public int botCount;

	public int botDifficulty = 1;

	public bool rebootBot;

	public int mutator;

	public int scoreLimit = 100;

	private float postLobbyFrame;

	public bool postLobby;

	public float gameLength;

	public bool privateMatch;

	public int pRedFlagState = 200;

	public int pBlueFlagState = 200;

	public int redFlagState = 200;

	public int blueFlagState = 200;

	public int hillState;

	public int[] DMScores = new int[4] { 500, 1000, 1500, 2500 };

	public int[] TDMScores = new int[4] { 1000, 2500, 5000, 10000 };

	public int[] ZHScores = new int[4] { 800, 2000, 4500, 9000 };

	public int[] CTFScores = new int[4] { 3, 5, 7, 10 };

	public float[] KOTHScores = new float[4] { 180f, 300f, 420f, 600f };

	public int DMScoreIdx;

	public int TDMScoreIdx;

	public int ZHScoreIdx;

	public int CTFScoreIdx;

	public int KOTHScoreIdx;

	public NetSession()
	{
		playerList = new Dictionary<byte, int>();
		netPlay = new NetPlay();
	}

	public int BotCount()
	{
		int num = 0;
		switch (botCount)
		{
		case 1:
			if (netSession != null)
			{
				num = 7 - netSession.AllGamers.Count;
			}
			break;
		case 2:
			num = 6;
			break;
		}
		if (num > 6)
		{
			num = 6;
		}
		return num;
	}

	public void ResetGameStats()
	{
		gameLength = 0f;
		redScore = 0;
		blueScore = 0;
		redTime = 0f;
		blueTime = 0f;
		redFlagState = 200;
		blueFlagState = 200;
		pRedFlagState = 200;
		pBlueFlagState = 200;
		hillState = 0;
	}

	// Real now (2026-08-25) - Game1.cs's HandleInvite/FinishHandleInvite call
	// this once NetworkBackend.InviteAccepted actually fires (see
	// SteamNetworkSession.cs's SteamFriends.OnGameLobbyJoinRequested
	// handler). ie.LobbyToken carries whatever opaque token that backend
	// captured (a boxed Steamworks.Data.Lobby for Steam - see
	// PlatformServices.cs's InviteAcceptedEventArgs); BeginJoinInvite is the
	// matching entry point every INetworkSessionFactory implements (Lan/
	// Local just fail cleanly, having no invite concept of their own - see
	// INetworkSession.cs). This mirrors JoinSession(IAvailableNetworkSession)
	// below almost exactly - same joinResult/pendingJoin fields, so the
	// shared EndJoin handling in Update() completes either kind of join
	// identically, including the "no response"/timeout error handling
	// already proven there.
	public void JoinInvite(InviteAcceptedEventArgs ie)
	{
		Kill();
		netType = 3;
		netPlay.needsInit = true;
		netPlay.ID = -1;
		Game1.hud.scoreBoard.Reset();
		Game1.character = new Character[32];
		playerList = new Dictionary<byte, int>();
		try
		{
			joinResult = NetworkBackend.Current.BeginJoinInvite(ie.LobbyToken, null, null);
			pendingJoin = true;
		}
		catch (Exception ex)
		{
			joinInviteFailed = true;
			failMessage = ex.Message;
		}
	}

	// Called from the pause menu's "Invite Friends" item (GameMain.cs). Only
	// meaningful for a backend that's actually got a real lobby to invite
	// people into (currently just Steam - see SteamNetworkSession.
	// OpenInviteOverlay); Lan/Local's implementations are harmless no-ops,
	// so this never needs a netType check to stay safe either way.
	public void InviteFriends()
	{
		try
		{
			netSession?.OpenInviteOverlay();
		}
		catch (Exception ex)
		{
			Console.WriteLine("[NetSession] InviteFriends: OpenInviteOverlay threw " + ex.GetType().Name + ": " + ex.Message);
		}
	}

	public int GetPlayerOne()
	{
		if (netPlay != null)
		{
			if (netType == 1 || netType == 0)
			{
				return 0;
			}
			if (netPlay.ID > -1)
			{
				// TEMP DIAGNOSTIC, see the comment on the fields above.
				if (!_loggedPlayerOneAssigned)
				{
					_loggedPlayerOneAssigned = true;
					Console.WriteLine("[GetPlayerOne] returning assigned netPlay.ID=" + netPlay.ID + " (first time this became non-fallback).");
				}
				return netPlay.ID;
			}
			// TEMP DIAGNOSTIC, see the comment on the fields above. netType
			// 2/3 = Steam/system-link client or host; falling back to slot 0
			// here on a CLIENT (not the host) means netPlay.ID was never
			// assigned - it treats the host's own character as its own.
			if (!_loggedPlayerOneFallback)
			{
				_loggedPlayerOneFallback = true;
				Console.WriteLine("[GetPlayerOne] FALLBACK to slot 0 - netPlay.ID=" + netPlay.ID + ", netType=" + netType + ", IsHost()=" + IsHost() + ". If IsHost() is false here, this client never got a valid slot assigned.");
			}
		}
		return 0;
	}

	public bool IsHost()
	{
		if (netType == 0 || netType == 1)
		{
			return true;
		}
		if (netSession != null && netSession.IsHost)
		{
			return true;
		}
		return false;
	}

	public bool GetNetworkOwner(int i)
	{
		int playerOne = GetPlayerOne();
		if (netSession == null)
		{
			return true;
		}
		if (netType == 0 || netType == 1)
		{
			return true;
		}
		if (netPlay != null)
		{
			if (playerOne == i)
			{
				return true;
			}
			if (Game1.character[i] == null)
			{
				return false;
			}
			if (netSession.IsHost)
			{
				for (int j = 0; j < netSession.RemoteGamers.Count; j++)
				{
					if (playerList.ContainsKey(netSession.RemoteGamers[j].Id) && playerList[netSession.RemoteGamers[j].Id] == i)
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}
		return true;
	}

	public void Kill()
	{
		Game1.store.Write(0);
		postLobbyFrame = 0f;
		postLobby = false;
		if (netSession == null || netSession.IsDisposed)
		{
			return;
		}
		try
		{
			netSession.Dispose();
			while (!netSession.IsDisposed)
			{
			}
			netSession = null;
		}
		catch (Exception ex)
		{
			failMessage = ex.Message;
		}
	}

	public bool GetHasGold()
	{
		if (netType == 0 || netType == 2 || netType == 1)
		{
			return true;
		}
		if (Game1.mainPlayerIndex < 0)
		{
			return false;
		}
		for (int i = 0; i < Gamer.SignedInGamers.Count; i++)
		{
			SignedInGamer val = Gamer.SignedInGamers[i];
			if (val.PlayerIndex == (PlayerIndex)Game1.mainPlayerIndex && val.Privileges.AllowOnlineSessions)
			{
				return true;
			}
		}
		return false;
	}

	public void CreateSession(int type)
	{
		Kill();
		netType = type;
		NetworkSessionProperties properties = new NetworkSessionProperties();
		properties[0] = 206;
		properties[1] = GameState.gameType;
		NetworkSessionType sessionType = (netType == 2) ? NetworkSessionType.SystemLink : NetworkSessionType.PlayerMatch;
		if (netType == 0)
		{
			sessionType = NetworkSessionType.SystemLink;
		}
		createResult = NetworkBackend.Current.BeginCreate(sessionType, 1, 10, privateMatch ? 9 : 0, properties, null, null);
		pendingCreate = true;
	}

	public void GetSessions(int type)
	{
		Kill();
		sessions = null;
		netType = type;
		NetworkSessionProperties properties = new NetworkSessionProperties();
		NetworkSessionType sessionType = (netType == 2) ? NetworkSessionType.SystemLink : NetworkSessionType.PlayerMatch;
		findResult = NetworkBackend.Current.BeginFind(sessionType, 1, properties, null, null);
		pendingFind = true;
	}

	public void JoinSession(IAvailableNetworkSession s)
	{
		Kill();
		joinResult = NetworkBackend.Current.BeginJoin(s, null, null);
		pendingJoin = true;
	}

	private void ManageLobby(Character[] c)
	{
		_ = c[GetPlayerOne()];
		if (postLobbyFrame > 0f)
		{
			Music.Reset();
			postLobbyFrame -= Game1.frameTime;
			if (postLobbyFrame <= 0f)
			{
				if (netSession.SessionState != NetworkSessionState.Playing)
				{
					netSession.StartGame();
				}
				netPlay.currentMapListIdx = (netPlay.currentMapListIdx + 1) % MapList.total;
				netPlay.currentMap = MapList.maplist[netPlay.currentMapListIdx];
				Game1.store.Write(0);
				Game1.gameMap.Read(new BinaryReader(File.Open("map/data/" + MapList.mapCatalog[netPlay.currentMap].path + ".zkx", FileMode.Open, FileAccess.Read)));
				Game1.nodeMgr.Refresh(Game1.gameMap);
				c[0] = new Character(netPlay.ID, 0, default(Vector2));
				c[0].SetNewClass();
				c[0].Reset();
				Game1.gameMap.GetSpawn(0, Game1.character[0]);
				for (int i = 0; i < Game1.netSession.BotCount(); i++)
				{
					Game1.character[i + 20] = new Character(i + 20, -1, default(Vector2));
					Game1.character[i + 20].headTex = (Game1.character[i + 20].hatTex = (Game1.character[i + 20].torsoTex = (Game1.character[i + 20].legsTex = 7)));
					Game1.character[i + 20].team = i % 2;
					Game1.character[i + 20].jetpack = 0;
					Game1.gameMap.GetSpawn(0, Game1.character[i + 20]);
				}
				// BUG FIX (2026-08-25, "bot starts new match already over the
				// score limit" playtest report): only c[0] (the host's own
				// character) and bot slots WITHIN the current BotCount() range
				// get a brand-new Character object above, which is the only
				// thing that ever zeroes .score - Character.Reset() (called on
				// c[0] just above) deliberately only touches respawn/loadout
				// state, never match stats, see its own comment. Any other
				// slot - a human player already in the lobby, or a bot slot
				// left over from a previous match if BotCount() has since
				// shrunk (fewer bots needed because someone joined) - keeps
				// whatever Character object and .score it had from the match
				// that just ended. In Deathmatch, CheckWinner() below compares
				// exactly that leftover .score against the new match's score
				// limit, so a bot that finished the last match already past
				// the new (possibly lower) limit ends the new match before it
				// visibly starts. Reset every still-present character's score
				// explicitly rather than relying on which slots happen to get
				// reconstructed - this is host-only code, but score is part of
				// the regular character sync (see NetPlay.cs), so the zeroed
				// value reaches every client on the next sync tick too.
				for (int i = 0; i < c.Length; i++)
				{
					if (c[i] != null)
					{
						c[i].score = 0;
					}
				}
				ResetGameStats();
			}
		}
		postLobby = postLobbyFrame > 0f;
		if (postLobby || !(gameLength > 10f))
		{
			return;
		}
		if (GameState.gameType == 2)
		{
			if (redFlagState != 200 && c[redFlagState] == null)
			{
				redFlagState = 200;
			}
			if (blueFlagState != 200 && c[blueFlagState] == null)
			{
				blueFlagState = 200;
			}
		}
		CheckWinner(c);
	}

	private void CheckWinner(Character[] c)
	{
		bool flag = false;
		switch (GameState.gameType)
		{
		case 0:
		{
			for (int i = 0; i < c.Length; i++)
			{
				if (c[i] != null && c[i].score >= DMScores[DMScoreIdx])
				{
					flag = true;
				}
			}
			break;
		}
		case 1:
			if (blueScore >= TDMScores[TDMScoreIdx] || redScore >= TDMScores[TDMScoreIdx])
			{
				flag = true;
			}
			break;
		case 4:
			if (blueScore >= ZHScores[ZHScoreIdx] || redScore >= ZHScores[ZHScoreIdx])
			{
				flag = true;
			}
			break;
		case 2:
			if (blueScore >= CTFScores[CTFScoreIdx] || redScore >= CTFScores[CTFScoreIdx])
			{
				flag = true;
			}
			break;
		case 3:
			if (blueTime >= KOTHScores[KOTHScoreIdx] || redTime >= KOTHScores[KOTHScoreIdx])
			{
				flag = true;
			}
			break;
		}
		if (!flag)
		{
			return;
		}
		if (netSession.SessionState == NetworkSessionState.Playing)
		{
			try
			{
				netSession.EndGame();
			}
			catch
			{
			}
		}
		postLobbyFrame = 10f;
		redFlagState = 200;
		blueFlagState = 200;
	}

	public void Update(Character[] c)
	{
		gameLength += Game1.frameTime;
		if (netSession != null)
		{
			if (!netSession.IsDisposed)
			{
				for (int i = 0; i < c.Length; i++)
				{
					if (c[i] == null)
					{
						freeSlot = i;
						break;
					}
				}
				try
				{
					netSession.Update();
				}
				catch (Exception e)
				{
					ServerCrashRehost(e);
					return;
				}
				if (netSession.IsHost)
				{
					ManageLobby(c);
				}
				if (netSession.AllGamers.Count >= 1)
				{
					netPlay.Update(netSession, c);
				}
				else
				{
					for (int j = 0; j < c.Length; j++)
					{
						if (c[j] != null)
						{
							c[j].deltaSinceUpdate = 0f;
						}
					}
					Game1.pMan.NetWriteCleanup();
				}
				if (netSession.AllGamers.Count == 0 && GameState.mode == 1 && (netType == 2 || netType == 3))
				{
					GameState.mode = 2;
					Kill();
					Game1.menu.Close();
					Game1.menu.DoError("Game ended!", (netType == 2) ? 5 : 6);
				}
			}
		}
		else if (GameState.mode == 1 && (netType == 2 || netType == 3))
		{
			GameState.mode = 2;
			Game1.menu.Close();
			Game1.menu.DoError("Game ended!", (netType == 2) ? 5 : 6);
		}
		if (pendingCreate && createResult.IsCompleted)
		{
			try
			{
				playerList = new Dictionary<byte, int>();
				// Slot 0 is always the host's own character (see StartServer()/
				// ManageLobby(): "c[0] = new Character(netPlay.ID, 0, ...)" with
				// netPlay.ID hard-set to 0 for the host) - but nothing ever
				// recorded that reservation in playerList itself, so
				// netSession_GamerJoined()'s freeSlot search below (which only
				// treats a slot as taken if some AllGamers entry maps to it in
				// playerList) would happily hand slot 0 to the very first
				// joining remote gamer, colliding with the host's own character
				// in Game1.character[0] (both machines fighting over the same
				// array index - reported 2026-08-23 as "host camera switches to
				// the other character and neither player can move" the first
				// time a second real gamer ever actually joined a hosted game).
				// Reserve it explicitly: gamer id 0 is always the host's own
				// local gamer on every INetworkSession backend in this project
				// (LocalNetworkSession, LanNetworkSession both construct their
				// host's local gamer with id 0).
				playerList[0] = 0;
				Game1.character = new Character[32];
				c = Game1.character;
				netSession = NetworkBackend.Current.EndCreate(createResult);
				netSession.AllowJoinInProgress = true;
				netSession.StartGame();
			}
			catch (Exception ex)
			{
				createFailed = true;
				failMessage = ex.Message;
			}
			if (netSession != null)
			{
				try
				{
					netSession.GamerJoined += netSession_GamerJoined;
					netSession.GamerLeft += netSession_GamerLeft;
				}
				catch (Exception)
				{
				}
			}
			pendingCreate = false;
		}
		if (pendingFind && findResult.IsCompleted)
		{
			try
			{
				sessions = NetworkBackend.Current.EndFind(findResult);
			}
			catch (Exception ex3)
			{
				findFailed = true;
				failMessage = ex3.Message;
			}
			pendingFind = false;
		}
		if (pendingJoin && joinResult.IsCompleted)
		{
			try
			{
				netSession = NetworkBackend.Current.EndJoin(joinResult);
				playerList = new Dictionary<byte, int>();
			}
			catch (Exception ex4)
			{
				joinFailed = true;
				failMessage = ex4.Message;
			}
			// Guard added 2026-08-23: mirrors the host-side "if (netSession !=
			// null)" check a few dozen lines up in the pendingCreate block.
			// When EndJoin() throws (e.g. the "no response" join-timeout),
			// netSession is left null by the catch above - unconditionally
			// touching netSession.GamerJoined here threw a NullReferenceException
			// that was never actually reachable from a failed join before now
			// (this is the first backend that can make EndJoin fail this way).
			if (netSession != null)
			{
				try
				{
					netSession.GamerJoined += netSession_ClientGamerJoined;
					netSession.GamerLeft += netSession_ClientGamerLeft;
				}
				catch (Exception)
				{
				}
			}
			pendingJoin = false;
		}
	}

	internal void ServerCrashRehost(Exception e)
	{
		rebootBot = Game1.menu.menuLevel[9].item[5].selX == 1;
		GameState.mode = 2;
		Kill();
		Game1.menu.Close();
		if (e == null)
		{
			Game1.menu.DoError("Game Ended! Unexpected error.", (netType == 2) ? 5 : 6, 1);
		}
		else
		{
			Game1.menu.DoError("Game Ended! Error: " + e.Message, (netType == 2) ? 5 : 6, 1);
		}
	}

	private void netSession_ClientGamerJoined(object sender, GamerJoinedEventArgs e)
	{
		Game1.hud.AddMessage(new StringBuilder(e.Gamer.Gamertag), Message.msgJoined, 0, 0, -1);
	}

	private void netSession_ClientGamerLeft(object sender, GamerLeftEventArgs e)
	{
		Game1.hud.AddMessage(new StringBuilder(e.Gamer.Gamertag), Message.msgQuit, 0, 0, -1);
	}

	private void netSession_GamerJoined(object sender, GamerJoinedEventArgs e)
	{
		// TEMP DIAGNOSTIC (2026-08-23, "joiner shares host's data" investigation):
		// safe to remove once the reported "no second player appears" bug is
		// understood. Builds the playerList dump manually (no System.Linq
		// using in this file) rather than risking a new build error over a
		// diagnostic line.
		string playerListDump = "";
		foreach (KeyValuePair<byte, int> kv in playerList)
		{
			playerListDump = playerListDump + kv.Key + "->" + kv.Value + " ";
		}
		Console.WriteLine("[Host] netSession_GamerJoined fired: gamer.Id=" + e.Gamer.Id + ", tag=" + e.Gamer.Gamertag + ", AllGamers.Count=" + netSession.AllGamers.Count + ", playerList before search: " + playerListDump);
		freeSlot = -1;
		for (int i = 0; i < 20; i++)
		{
			bool flag = true;
			for (int j = 0; j < netSession.AllGamers.Count; j++)
			{
				INetworkGamer val = netSession.AllGamers[j];
				if (val.Id != e.Gamer.Id && playerList.ContainsKey(val.Id) && playerList[val.Id] == i)
				{
					flag = false;
				}
			}
			if (flag)
			{
				freeSlot = i;
				break;
			}
		}
		// TEMP DIAGNOSTIC, see the comment above.
		Console.WriteLine("[Host] netSession_GamerJoined: computed freeSlot=" + freeSlot + " for gamer.Id=" + e.Gamer.Id + ".");
		INetworkGamer gamer = e.Gamer;
		// TEMP DIAGNOSTIC (2026-08-24, "joiner never gets MSG_INIT over WAN"
		// investigation): this whole block - building the MSG_INIT packet and
		// handing it to SendData - used to run with nothing catching an
		// exception here directly, and this method is invoked synchronously
		// from GamerJoined?.Invoke(...) inside SteamNetworkSession.CompleteJoin,
		// itself called from deep inside the Steam callback dispatch chain
		// (OnMessage -> HostMessageReceived -> CompleteJoin). Until just now,
		// SteamNetworkSession.Pump() wrapped that entire chain in a bare
		// `catch { }`, so any exception thrown anywhere in here (this method,
		// SendData, the low-level Steamworks call) vanished with zero trace -
		// which matches the last WAN test's full host log stopping dead right
		// after the "computed freeSlot=1" line above, with no further output
		// on either machine for the rest of the session. Pump()'s catch now
		// logs too, but wrapping it here as well gives a log line with the
		// specific gamer/slot context instead of just a generic dispatch-level
		// exception dump. Safe to remove once understood.
		try
		{
			PacketWriter val2 = new PacketWriter();
			ILocalNetworkGamer val3 = netSession.LocalGamers[0];
			if (freeSlot == -1)
			{
				NetPacker.WriteMsg(val2, 9);
				NetPacker.WriteByte(val2, 0);
				NetPacker.WriteMsg(val2, 1);
				val3.SendData(val2, SendDataOptions.Reliable, gamer);
				return;
			}
			NetPacker.WriteMsg(val2, 2);
			NetPacker.WriteByte(val2, freeSlot);
			NetPacker.WriteByte(val2, MapList.maplist[netPlay.currentMapListIdx]);
			NetPacker.WriteByte(val2, GameState.gameType);
			NetPacker.WriteMsg(val2, 1);
			val3.SendData(val2, SendDataOptions.Reliable, gamer);
			if (playerList.ContainsKey(gamer.Id))
			{
				if (playerList[gamer.Id] != freeSlot)
				{
					playerList[gamer.Id] = freeSlot;
				}
			}
			else
			{
				playerList.Add(gamer.Id, freeSlot);
			}
			Game1.hud.AddMessage(new StringBuilder(e.Gamer.Gamertag), Message.msgJoined, 0, 0, -1);
			// TEMP DIAGNOSTIC, see the comment above - confirms this method
			// ran to completion (i.e. MSG_INIT really was handed to SendData)
			// without needing to guess from silence.
			Console.WriteLine("[Host] netSession_GamerJoined: MSG_INIT build/send completed for gamer.Id=" + gamer.Id + ", slot=" + freeSlot + ".");
		}
		catch (Exception ex)
		{
			// TEMP DIAGNOSTIC, see the comment above.
			Console.WriteLine("[Host] netSession_GamerJoined: EXCEPTION building/sending MSG_INIT for gamer.Id=" + gamer.Id + ", freeSlot=" + freeSlot + ": " + ex.GetType().Name + ": " + ex);
		}
	}

	private void netSession_GamerLeft(object sender, GamerLeftEventArgs e)
	{
		INetworkGamer gamer = e.Gamer;
		Game1.DestroyChar(playerList[gamer.Id]);
		playerList.Remove(gamer.Id);
		Game1.hud.AddMessage(new StringBuilder(e.Gamer.Gamertag), Message.msgQuit, 0, 0, -1);
	}

	internal void StartServer(Menu menu)
	{
		Game1.netSession.netPlay.needsInit = true;
		Game1.netSession.netPlay.ID = 0;
		Game1.hud.scoreBoard.Reset();
		MapList.Scramble();
		Game1.netSession.netPlay.currentMapListIdx = 0;
		Game1.netSession.netPlay.currentMap = MapList.maplist[Game1.netSession.netPlay.currentMapListIdx];
		Game1.store.Write(0);
		Game1.gameMap.Read(new BinaryReader(File.Open("map/data/" + MapList.mapCatalog[Game1.netSession.netPlay.currentMap].path + ".zkx", FileMode.Open, FileAccess.Read)));
		Game1.nodeMgr.Refresh(Game1.gameMap);
		Game1.netSession.playerList = new Dictionary<byte, int>();
		Game1.netSession.playerList[0] = 0; // reserve the host's own slot - see the matching comment in Update() above
		Game1.netSession.CreateSession(Game1.netSession.netType);
		Game1.character = new Character[32];
		menu.menuLevel[4] = new Lobby(host: true);
		menu.menuLevel[4].active = true;
	}

	internal void ChangeMutator()
	{
		for (int i = 0; i < Game1.character.Length; i++)
		{
			if (Game1.character[i] != null && GetNetworkOwner(i))
			{
				Game1.character[i].Reset();
			}
		}
		if (Mutators.GetCrates(mutator))
		{
			return;
		}
		for (int j = 0; j < Game1.pMan.particle.Length; j++)
		{
			if (Game1.pMan.particle[j].exists && Game1.pMan.particle[j].type == 43)
			{
				Game1.pMan.particle[j].exists = false;
			}
		}
	}

	internal void NullCrash()
	{
		if (IsHost())
		{
			ServerCrashRehost(new Exception("Null'd!!1"));
			return;
		}
		Kill();
		GameState.mode = 2;
		Game1.menu.Close();
		Game1.menu.DoError("Error: disconnected from game.", 6);
	}
}
