using System.Text;
using ZP2K9.platform;
using ZP2K9.hud;

namespace ZP2K9.menu.levels;

public class XboxLive : MenuLevel
{
	private const int ITEM_CREATEGAME = 0;

	private const int ITEM_JOINGAME = 1;

	private const int ITEM_BACK = 2;

	public XboxLive()
	{
		name = new StringBuilder("Steam Multiplayer");
		item = new MenuItem[3]
		{
			new MenuItem("Create Game", 0),
			new MenuItem("Join Game", 1),
			new MenuItem("Back", 2)
		};
		width = 200;
		height = 300;
	}

	public override void Update(InterfaceKeys iKeys, Menu menu)
	{
		if (Guide.IsTrialMode)
		{
			item[0].disabled = true;
		}
		else
		{
			item[0].disabled = false;
		}
		base.Update(iKeys, menu);
	}

	public override void SelectItem(Menu menu)
	{
		switch (selected)
		{
		case 0:
			Game1.netSession.netType = 3;
			active = false;
			menu.menuLevel[11].active = true;
			menu.menuLevel[11].item[1].selX = Game1.netSession.botDifficulty;
			menu.menuLevel[11].item[0].selX = Game1.netSession.botCount;
			break;
		case 1:
			if (Game1.netSession.GetHasGold())
			{
				Game1.netSession.GetSessions(3);
				active = false;
				menu.menuLevel[7].active = true;
			}
			else
			{
				Game1.menu.DoError("Erm, you shouldn't be seeing this. Xbox Live Gold Much? :/", 0);
			}
			break;
		case 2:
			active = false;
			menu.menuLevel[0].active = true;
			break;
		}
	}

	public override void Cancel(Menu menu)
	{
		active = false;
		menu.menuLevel[0].active = true;
	}
}
