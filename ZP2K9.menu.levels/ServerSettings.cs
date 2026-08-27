using System.Text;
using Microsoft.Xna.Framework;
using ZP2K9.characters;
using ZP2K9.hud;

namespace ZP2K9.menu.levels;

public class ServerSettings : MenuLevel
{
	public const int ITEM_BOTS = 0;

	public const int ITEM_BOT_DIFFICULTY = 1;

	public const int ITEM_TYPE = 2;

	public const int ITEM_MUTATOR = 3;

	private const int ITEM_BACK = 4;

	public ServerSettings()
	{
		item = new MenuItem[5]
		{
			new MenuItem(new string[3] { "Bots: None", "Bots: Replacement", "Bots: Max" }, 0),
			new MenuItem(new string[4] { "AI: Easy", "AI: Normal", "AI: Hard", "AI: Tough" }, 1),
			new MenuItem(new string[5] { "Type: Deathmatch", "Type: Team Deathmatch", "Type: CTF", "Type: King of the Hill", "Type: Zombie Hunt" }, 2),
			new MenuItem(Mutators.GetAllStrings(), 3),
			new MenuItem("Back", 4)
		};
		name = new StringBuilder("Server Settings");
		width = 300;
		height = 300;
	}

	public override void Update(InterfaceKeys iKeys, Menu menu)
	{
		if (active)
		{
			Game1.netSession.botDifficulty = item[1].selX;
			Game1.netSession.botCount = item[0].selX;
			if (item[3].selX != Game1.netSession.mutator)
			{
				Game1.netSession.mutator = item[3].selX;
				Game1.netSession.ChangeMutator();
			}
			for (int i = 0; i < 6; i++)
			{
				if (i < Game1.netSession.BotCount())
				{
					if (Game1.character[i + 20] == null)
					{
						Game1.character[i + 20] = new Character(i + 20, -1, default(Vector2));
						if (GameState.gameType == 0)
						{
							Game1.character[i + 20].team = 0;
						}
						else
						{
							Game1.character[i + 20].team = i % 2;
						}
						Game1.gameMap.GetSpawn(Game1.character[i + 20].team, Game1.character[i + 20]);
					}
				}
				else if (Game1.character[i + 20] != null)
				{
					Game1.DestroyChar(i + 20);
				}
			}
		}
		int selX = item[2].selX;
		base.Update(iKeys, menu);
		if (selX != item[2].selX)
		{
			GameState.gameType = item[2].selX;
			switch (GameState.gameType)
			{
			case 2:
				Game1.netSession.redFlagState = 200;
				Game1.netSession.blueFlagState = 200;
				break;
			}
			Game1.netSession.netSession.SessionProperties[1] = GameState.gameType;
			Game1.netSession.ChangeMutator();
		}
	}

	public override void SelectItem(Menu menu)
	{
		int num = selected;
		if (num == 4)
		{
			active = false;
			menu.menuLevel[9].active = true;
		}
	}

	public override void Cancel(Menu menu)
	{
		active = false;
		menu.menuLevel[9].active = true;
	}
}
