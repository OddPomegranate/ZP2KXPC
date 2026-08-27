using System.Text;
using Microsoft.Xna.Framework;
using ZP2K9.ai;
using ZP2K9.hud;

namespace ZP2K9.menu.levels;

public class GameMain : MenuLevel
{
	private const int ITEM_RESUME = 0;

	private const int ITEM_PLAYER_SETUP = 1;

	public const int ITEM_TEAM = 2;

	private const int ITEM_GAMESETTINGS = 3;

	private const int ITEM_CONTROLS = 4;

	public const int ITEM_TAKE_A_BREAK = 5;

	private const int ITEM_SERVER_SETTINGS = 6;

	private const int ITEM_INVITE = 7;

	private const int ITEM_END_GAME = 8;

	public GameMain()
	{
		item = new MenuItem[9]
		{
			new MenuItem("Resume Game", 0),
			new MenuItem("Character Roster", 1),
			new MenuItem(new string[3] { "Team: Character Default", "Team: Humans", "Team: Zombies" }, 2),
			new MenuItem("Settings", 3),
			new MenuItem("Controls", 4),
			new MenuItem(new string[2] { "Break Time: Off", "Break Time: On" }, 5),
			new MenuItem("Server Settings", 6),
			new MenuItem("Invite Friends", 7),
			new MenuItem("End Game", 8)
		};
		name = new StringBuilder("Not Paused");
		item[1].newBump = 10f;
		width = 260;
		height = 300;
	}

	public override void CheckNewUnlocks()
	{
		item[1].newunlock = false;
		Game1.menu.menuLevel[19].CheckNewUnlocks();
		for (int i = 0; i < 9; i++)
		{
			if (Game1.menu.menuLevel[19].item[i].newunlock)
			{
				item[1].newunlock = true;
			}
		}
		Game1.menu.menuLevel[17].CheckNewUnlocks();
		for (int j = 0; j < 3; j++)
		{
			if (Game1.menu.menuLevel[17].item[j].newunlock)
			{
				item[1].newunlock = true;
			}
		}
		base.CheckNewUnlocks();
	}

	public override void Update(InterfaceKeys iKeys, Menu menu)
	{
		width = 290;
		if (active)
		{
			if (!menu.infoBox.active)
			{
				menu.InitInfoBox();
			}
			item[6].disabled = false;
			if (Game1.netSession.netType == 3 || Game1.netSession.netType == 2)
			{
				item[6].disabled = true;
				if (Game1.netSession.netSession != null && Game1.netSession.netSession.IsHost)
				{
					item[6].disabled = false;
				}
			}
			if (Game1.netSession.netType == 3)
			{
				item[7].disabled = false;
			}
			else
			{
				item[7].disabled = true;
			}
			Game1.zProfile.defaultTeam = item[2].selX;
			CheckNewUnlocks();
		}
		if (!active || selected != 5)
		{
			item[5].selX = 0;
		}
		int playerOne = Game1.netSession.GetPlayerOne();
		if (item[5].selX == 0)
		{
			if (Game1.character[playerOne] != null && Game1.character[playerOne].ai != null)
			{
				Game1.character[playerOne].ai = null;
				Game1.character[playerOne].keySrc = 0;
			}
			selOnly = false;
		}
		else
		{
			if (Game1.character[playerOne] != null && Game1.character[playerOne].ai == null)
			{
				Game1.character[playerOne].ai = new AI(playerOne);
				Game1.character[playerOne].keySrc = -1;
			}
			selOnly = true;
		}
		base.Update(iKeys, menu);
	}

	public override void SelectItem(Menu menu)
	{
		switch (selected)
		{
		case 0:
			active = false;
			break;
		case 6:
			active = false;
			menu.menuLevel[15].active = true;
			menu.menuLevel[15].item[0].selX = Game1.netSession.botCount;
			menu.menuLevel[15].item[1].selX = Game1.netSession.botDifficulty;
			menu.menuLevel[15].item[2].selX = GameState.gameType;
			menu.menuLevel[15].item[3].selX = Game1.netSession.mutator;
			break;
		case 7:
			// Opens Steam's native invite-a-friend overlay for the lobby
			// this session is hosting/joined - see NetSession.InviteFriends
			// and SteamNetworkSession.OpenInviteOverlay. Only enabled in
			// the first place when netType == 3 (Steam) - see Update() above.
			Game1.netSession.InviteFriends();
			break;
		case 1:
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
		case 3:
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
		case 4:
			active = false;
			menu.menuLevel[14].active = true;
			break;
		case 8:
			active = false;
			if (Game1.netSession.netType == 1)
			{
				GameState.mode = 0;
				break;
			}
			menu.menuLevel[0].active = true;
			GameState.mode = 2;
			if (Game1.netSession.netType == 3 || Game1.netSession.netType == 2)
			{
				Game1.netSession.Kill();
			}
			break;
		case 2:
		case 5:
			break;
		}
	}

	public override void Cancel(Menu menu)
	{
		active = false;
	}
}
