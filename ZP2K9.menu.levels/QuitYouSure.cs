using System.Text;

namespace ZP2K9.menu.levels;

public class QuitYouSure : MenuLevel
{
	private const int ITEM_YES = 0;

	private const int ITEM_NO = 1;

	private const int ITEM_EDITOR = 2;

	private const int ITEM_QUIT = 3;

	public QuitYouSure()
	{
		name = new StringBuilder("Quit?");
		item = new MenuItem[2]
		{
			new MenuItem("Yes", 0),
			new MenuItem("No", 1)
		};
		width = 150;
		height = 200;
	}

	public override void SelectItem(Menu menu)
	{
		switch (selected)
		{
		case 0:
			Game1.needsExit = true;
			break;
		case 1:
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
