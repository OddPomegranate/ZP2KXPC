using System.Text;
using ZP2K9.debug;
using ZP2K9.hud;

namespace ZP2K9.menu.levels;

public class DebugLev : MenuLevel
{
	private const int ITEM_SHOW_AI_ARCS = 0;

	private const int ITEM_SHOW_AI_PATHS = 1;

	private const int ITEM_SHOW_NODE_INDICES = 2;

	private const int ITEM_GODMODE = 3;

	private const int ITEM_BOTSIGNORE = 4;

	private const int ITEM_BOTSFOLLOW = 5;

	private const int ITEM_JUMP_TO_LEV_UP = 6;

	private const int ITEM_LEVELUP = 7;

	private const int ITEM_REFRESH_NODES = 8;

	private const int ITEM_RUN_AUTOJOIN = 9;

	private const int ITEM_FAKEREALPLAYERS = 10;

	private const int ITEM_HIDEHUD = 11;

	private const int ITEM_JUMP_TO_NULL_ME = 12;

	private const int ITEM_BACK = 13;

	public DebugLev()
	{
		name = new StringBuilder("Debug");
		item = new MenuItem[14]
		{
			new MenuItem(new string[2] { "Show AI Arcs: Off", "Show AI Arcs: On" }, 0),
			new MenuItem(new string[2] { "Show AI Paths: Off", "Show AI Paths: On" }, 1),
			new MenuItem(new string[2] { "Show Node Indices: Off", "Show Node Indices: On" }, 2),
			new MenuItem(new string[2] { "God Mode: Off", "God Mode: On" }, 3),
			new MenuItem(new string[2] { "Bots Ignore: Off", "Bots Ignore: On" }, 4),
			new MenuItem(new string[2] { "Bots Follow: Off", "Bots Follow: On" }, 5),
			new MenuItem(new string[2] { "Jump To Level: Off", "Jump To Level: On" }, 6),
			new MenuItem("Level Up to 90", 7),
			new MenuItem("Refresh Nodes", 8),
			new MenuItem("Run Autojoin", 9),
			new MenuItem(new string[2] { "Fake Real Players: Off", "Fake Real Players: On" }, 10),
			new MenuItem(new string[2] { "Hide HUD: Off", "Hide HUD: On" }, 11),
			new MenuItem(new string[2] { "Jump to null me: Off", "Jump to null me: On" }, 12),
			new MenuItem("Back", 13)
		};
		width = 500;
		height = 300;
	}

	public override void Update(InterfaceKeys iKeys, Menu menu)
	{
		if (active)
		{
			width = 400;
			DebugManager.showAIDest = item[0].selX == 1;
			DebugManager.showNodeIndices = item[2].selX == 1;
			DebugManager.showAIPaths = item[1].selX == 1;
			DebugManager.jumpToLevUp = item[6].selX == 1;
			DebugManager.godMode = item[3].selX == 1;
			DebugManager.botsIgnore = item[4].selX == 1;
			DebugManager.aiFollow = item[5].selX == 1;
			DebugManager.fakeRealPlayers = item[10].selX == 1;
			DebugManager.hideHud = item[11].selX == 1;
			DebugManager.jumpToNullMe = item[12].selX == 1;
		}
		base.Update(iKeys, menu);
	}

	public override void SelectItem(Menu menu)
	{
		switch (selected)
		{
		case 7:
		{
			int playerOne = Game1.netSession.GetPlayerOne();
			if (Game1.character[playerOne] != null)
			{
				Game1.character[playerOne].LevelMax();
			}
			break;
		}
		case 8:
			Game1.nodeMgr.Refresh(Game1.gameMap);
			break;
		case 9:
			active = false;
			menu.menuLevel[0].active = true;
			DebugManager.StartAutoJoin();
			break;
		case 13:
			active = false;
			if (GameState.mode == 1)
			{
				menu.menuLevel[9].active = true;
			}
			else
			{
				menu.menuLevel[0].active = true;
			}
			break;
		case 10:
		case 11:
		case 12:
			break;
		}
	}

	public override void Cancel(Menu menu)
	{
		active = false;
		if (GameState.mode == 1)
		{
			menu.menuLevel[9].active = true;
		}
		else
		{
			menu.menuLevel[0].active = true;
		}
	}
}
