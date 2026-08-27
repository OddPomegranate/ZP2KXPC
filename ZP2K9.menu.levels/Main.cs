using System.Collections.ObjectModel;
using System.Text;
using ZP2K9.platform;
using ZP2K9.hud;
using ZP2K9.net;

namespace ZP2K9.menu.levels;

public class Main : MenuLevel
{
	private const int ITEM_XBOXLIVE = 0;

	private const int ITEM_LAN = 1;

	private const int ITEM_PLAYER_SETUP = 2;

	private const int ITEM_PRACTICE = 3;

	private const int ITEM_SETTINGS = 4;

	private const int ITEM_CONTROLS = 5;

	private const int ITEM_QUIT = 6;

	public Main()
	{
		item = new MenuItem[7]
		{
			new MenuItem("Online Multiplayer", 0),
			new MenuItem("LAN Multiplayer", 1),
			new MenuItem("Character Roster", 2),
			new MenuItem("Practice", 3),
			new MenuItem("Settings", 4),
			new MenuItem("Controls", 5),
			new MenuItem("Quit", 6)
		};
		name = new StringBuilder("Main Menu");
		item[2].newBump = 10f;
		width = 200;
		height = 300;
	}

	public override void CheckNewUnlocks()
	{
		item[2].newunlock = false;
		Game1.menu.menuLevel[19].CheckNewUnlocks();
		for (int i = 0; i < 9; i++)
		{
			if (Game1.menu.menuLevel[19].item[i].newunlock)
			{
				item[2].newunlock = true;
			}
		}
		base.CheckNewUnlocks();
	}

	public override void Update(InterfaceKeys iKeys, Menu menu)
	{
		if (active)
		{
			bool flag = false;
			bool flag2 = false;
			if (!menu.infoBox.active)
			{
				menu.InitInfoBox();
			}
			for (int i = 0; i < (Gamer.SignedInGamers).Count; i++)
			{
				flag2 = true;
				if ((Gamer.SignedInGamers)[i].IsSignedInToLive)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				item[0].disabled = true;
				item[0].locked = true;
			}
			else
			{
				item[0].disabled = false;
				item[0].locked = false;
			}
			if (!flag2)
			{
				item[3].disabled = true;
				item[1].disabled = true;
				item[2].disabled = true;
				item[4].disabled = true;
				item[3].locked = true;
				item[1].locked = true;
				item[2].locked = true;
				item[4].locked = true;
			}
			else
			{
				item[3].disabled = false;
				item[1].disabled = false;
				item[2].disabled = false;
				item[4].disabled = false;
				item[3].locked = false;
				item[1].locked = false;
				item[2].locked = false;
				item[4].locked = false;
			}
			if (Guide.IsTrialMode)
			{
				item[0].disabled = true;
				item[0].locked = true;
			}
			CheckNewUnlocks();
		}
		base.Update(iKeys, menu);
	}

	public override void SelectItem(Menu menu)
	{
		switch (selected)
		{
		case 3:
			Game1.netSession.netType = 2;
			active = false;
			menu.menuLevel[11].active = true;
			break;
		case 0:
			// "Online Multiplayer" - routes through the Steam relay backend as of
			// 2026-08-23 (see ZP2K9.net/SteamNetworkSession.cs). Same pattern as the
			// "LAN Multiplayer" item below: NetworkBackend.Current is a single mutable
			// static factory (ZP2K9.net/INetworkSession.cs), so forcing it here rather
			// than trusting whatever it was last left as guarantees XboxLive.cs's
			// Create/Join Game (menuLevel[6]) always goes over Steam, regardless of
			// whether the LAN item was used earlier in the same session. Requires
			// Steam to be running - see SteamNetworkSessionFactory.BeginCreate/BeginFind
			// for the error path when it isn't.
			NetworkBackend.Current = new SteamNetworkSessionFactory();
			active = false;
			menu.menuLevel[6].active = true;
			break;
		case 1:
			// "LAN Multiplayer" - added 2026-08-23 to give a direct way back into the
			// existing (pre-Steamworks) System Link flow (SystemLink.cs, menuLevel[5])
			// once "Online Multiplayer" (case 0, XboxLive.cs) starts routing through a
			// real internet backend. NetworkBackend.Current is a single mutable static
			// factory (see ZP2K9.net/INetworkSession.cs) - explicitly forcing it to LAN
			// here, rather than trusting whatever it was last left as, guarantees this
			// menu item always plays over the local network regardless of what "Online
			// Multiplayer" does elsewhere.
			NetworkBackend.Current = new LanNetworkSessionFactory();
			active = false;
			menu.menuLevel[5].active = true;
			break;
		case 2:
		{
			active = false;
			menu.menuLevel[19].active = true;
			for (int i = 0; i < 8; i++)
			{
				menu.menuLevel[19].item[i].text = new StringBuilder(Game1.zProfile.ClassSet(i).name);
			}
			if (Game1.zProfile.clanTag != null)
			{
				menu.menuLevel[19].item[8].text = new StringBuilder("Clan Tag: [" + Game1.zProfile.clanTag.ToString() + "]");
			}
			else
			{
				menu.menuLevel[19].item[8].text = new StringBuilder("Clan Tag");
			}
			break;
		}
		case 4:
			active = false;
			menu.menuLevel[16].active = true;
			menu.menuLevel[16].item[1].selX = (Game1.settings.vibration ? 1 : 0);
			menu.menuLevel[16].item[0].selX = (Game1.settings.showNames ? 1 : 0);
			menu.menuLevel[16].item[2].selX = (Game1.settings.autoSwitch ? 1 : 0);
			menu.menuLevel[16].item[3].selX = (Game1.settings.upToJetpack ? 1 : 0);
			menu.menuLevel[16].item[4].selX = (Game1.settings.twinStickShooter ? 1 : 0);
			menu.menuLevel[16].item[5].selX = Game1.settings.sfx;
			menu.menuLevel[16].item[6].selX = Game1.settings.bgm;
			break;
		case 5:
			active = false;
			menu.menuLevel[14].active = true;
			break;
		case 6:
			active = false;
			menu.menuLevel[1].active = true;
			break;
		}
	}

	public override void Cancel(Menu menu)
	{
		active = false;
		menu.menuLevel[13].active = true;
		Game1.mainPlayerIndex = -1;
	}
}
