using System.Text;
using ZP2K9.debug;
using ZP2K9.hud;
using ZP2K9.net;

namespace ZP2K9.menu.levels;

public class StartServer : MenuLevel
{
	public const int ITEM_BOTS = 0;

	public const int ITEM_BOT_DIFFICULTY = 1;

	public const int ITEM_TYPE = 2;

	public const int ITEM_STATUS = 3;

	public const int ITEM_CUSTOMIZE = 4;

	public const int ITEM_START = 5;

	public const int ITEM_CANCEL = 6;

	public StartServer()
	{
		name = new StringBuilder("Server Setup");
		item = new MenuItem[7]
		{
			new MenuItem(new string[3] { "Bots: Off", "Bots: Replacement", "Bots: Max" }, 0),
			new MenuItem(new string[4] { "AI: Easy", "AI: Normal", "AI: Hard", "AI: Tough" }, 1),
			new MenuItem(new string[5] { "Type: Deathmatch", "Type: Team Deathmatch", "Type: CTF", "Type: King of the Hill", "Type: Zombie Hunt" }, 2),
			new MenuItem(new string[2] { "Status: Public", "Status: Private" }, 3),
			new MenuItem("Customize", 4),
			new MenuItem("Start", 5),
			new MenuItem("Cancel", 6)
		};
		width = 300;
		height = 300;
	}

	public override void Update(InterfaceKeys iKeys, Menu menu)
	{
		if (active)
		{
			Game1.netSession.botCount = item[0].selX;
			Game1.netSession.botDifficulty = item[1].selX;
			if (item[2].selX == 5)
			{
				GameState.gameType = 0;
				DebugManager.mapTestMode = true;
			}
			else
			{
				int gameType = GameState.gameType;
				GameState.gameType = item[2].selX;
				DebugManager.mapTestMode = false;
				if (gameType != GameState.gameType)
				{
					SetMapList();
				}
			}
			Game1.netSession.privateMatch = item[3].selX == 1;
		}
		base.Update(iKeys, menu);
	}

	public override void SelectItem(Menu menu)
	{
		switch (selected)
		{
		case 5:
			active = false;
			if (Game1.netSession.GetHasGold())
			{
				Game1.netSession.StartServer(menu);
			}
			else
			{
				Game1.menu.DoError("You need an Xbox LIVE Gold account to play on Xbox LIVE.", 0);
			}
			break;
		case 4:
		{
			active = false;
			menu.menuLevel[21].active = true;
			CustomizeServer customizeServer = (CustomizeServer)menu.menuLevel[21];
			switch (GameState.gameType)
			{
			case 0:
				customizeServer.item[0] = new MenuItem(new string[4] { "Score Limit: 500", "Score Limit: 1000", "Score Limit: 1500", "Score Limit: 2500" }, 0);
				customizeServer.item[0].selX = Game1.netSession.DMScoreIdx;
				break;
			case 1:
				customizeServer.item[0] = new MenuItem(new string[4] { "Score Limit: 1000", "Score Limit: 2500", "Score Limit: 5000", "Score Limit: 10000" }, 0);
				customizeServer.item[0].selX = Game1.netSession.TDMScoreIdx;
				break;
			case 4:
				customizeServer.item[0] = new MenuItem(new string[4] { "Score Limit: 800", "Score Limit: 2000", "Score Limit: 5000", "Score Limit: 10000" }, 0);
				customizeServer.item[0].selX = Game1.netSession.ZHScoreIdx;
				break;
			case 2:
				customizeServer.item[0] = new MenuItem(new string[4] { "Score Limit: 3", "Score Limit: 5", "Score Limit: 7", "Score Limit: 10" }, 0);
				customizeServer.item[0].selX = Game1.netSession.CTFScoreIdx;
				break;
			case 3:
				customizeServer.item[0] = new MenuItem(new string[4] { "Score Limit: 3:00", "Score Limit: 5:00", "Score Limit: 7:00", "Score Limit: 10:00" }, 0);
				customizeServer.item[0].selX = Game1.netSession.KOTHScoreIdx;
				break;
			}
			customizeServer.item[2].selX = Game1.netSession.mutator;
			break;
		}
		case 6:
			active = false;
			menu.menuLevel[(Game1.netSession.netType != 2) ? 6 : 0].active = true;
			break;
		}
	}

	public void SetMapList()
	{
		switch (GameState.gameType)
		{
		case 0:
			MapList.mapCatalog[0].included = true;
			MapList.mapCatalog[1].included = true;
			MapList.mapCatalog[9].included = true;
			MapList.mapCatalog[3].included = true;
			MapList.mapCatalog[6].included = true;
			MapList.mapCatalog[8].included = true;
			Game1.netSession.DMScoreIdx = 0;
			break;
		case 1:
			MapList.mapCatalog[0].included = true;
			MapList.mapCatalog[6].included = true;
			MapList.mapCatalog[5].included = true;
			MapList.mapCatalog[4].included = true;
			MapList.mapCatalog[8].included = true;
			MapList.mapCatalog[9].included = true;
			Game1.netSession.TDMScoreIdx = 0;
			break;
		case 2:
			MapList.mapCatalog[2].included = true;
			MapList.mapCatalog[7].included = true;
			MapList.mapCatalog[5].included = true;
			MapList.mapCatalog[4].included = true;
			MapList.mapCatalog[6].included = true;
			MapList.mapCatalog[9].included = true;
			Game1.netSession.CTFScoreIdx = 0;
			break;
		case 3:
			MapList.mapCatalog[0].included = true;
			MapList.mapCatalog[1].included = true;
			MapList.mapCatalog[5].included = true;
			MapList.mapCatalog[4].included = true;
			MapList.mapCatalog[8].included = true;
			MapList.mapCatalog[9].included = true;
			Game1.netSession.KOTHScoreIdx = 0;
			break;
		case 4:
			MapList.mapCatalog[0].included = true;
			MapList.mapCatalog[1].included = true;
			MapList.mapCatalog[5].included = true;
			MapList.mapCatalog[3].included = true;
			MapList.mapCatalog[8].included = true;
			MapList.mapCatalog[9].included = true;
			Game1.netSession.ZHScoreIdx = 0;
			break;
		}
		for (int i = 0; i < MapList.mapCatalog.Count; i++)
		{
			MapList.mapCatalog[i].included = true;
		}
	}

	public override void Cancel(Menu menu)
	{
		active = false;
		menu.menuLevel[(Game1.netSession.netType != 2) ? 6 : 0].active = true;
	}
}
