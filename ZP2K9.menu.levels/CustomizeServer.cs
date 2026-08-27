using System.Text;
using ZP2K9.hud;

namespace ZP2K9.menu.levels;

public class CustomizeServer : MenuLevel
{
	public const int ITEM_FRAGS = 0;

	public const int ITEM_EDIT_MAPLIST = 1;

	public const int ITEM_MUTATOR = 2;

	private const int ITEM_DONE = 3;

	public CustomizeServer()
	{
		item = new MenuItem[4]
		{
			new MenuItem(new string[1] { "" }, 0),
			new MenuItem("Edit Map List", 1),
			new MenuItem(Mutators.GetAllStrings(), 2),
			new MenuItem("Done", 3)
		};
		name = new StringBuilder("Customize Server");
		width = 300;
		height = 300;
	}

	public override void Update(InterfaceKeys iKeys, Menu menu)
	{
		if (active)
		{
			switch (GameState.gameType)
			{
			case 0:
				Game1.netSession.DMScoreIdx = item[0].selX;
				break;
			case 1:
				Game1.netSession.TDMScoreIdx = item[0].selX;
				break;
			case 2:
				Game1.netSession.CTFScoreIdx = item[0].selX;
				break;
			case 3:
				Game1.netSession.KOTHScoreIdx = item[0].selX;
				break;
			case 4:
				Game1.netSession.ZHScoreIdx = item[0].selX;
				break;
			}
			Game1.netSession.mutator = item[2].selX;
		}
		base.Update(iKeys, menu);
	}

	public override void SelectItem(Menu menu)
	{
		switch (selected)
		{
		case 1:
		{
			active = false;
			EditMapList editMapList = (EditMapList)menu.menuLevel[20];
			editMapList.Init();
			menu.menuLevel[20].active = true;
			break;
		}
		case 3:
			active = false;
			menu.menuLevel[11].active = true;
			break;
		case 2:
			break;
		}
	}

	public override void Cancel(Menu menu)
	{
		active = false;
		menu.menuLevel[11].active = true;
	}
}
