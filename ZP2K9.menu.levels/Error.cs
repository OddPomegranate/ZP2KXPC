using System.Text;
using ZP2K9.hud;

namespace ZP2K9.menu.levels;

public class Error : MenuLevel
{
	private const int ITEM_OK = 0;

	public Error()
	{
		name = new StringBuilder("Quit?");
		item = new MenuItem[1]
		{
			new MenuItem("Ok", 0)
		};
		width = 800;
		height = 200;
	}

	public override void Update(InterfaceKeys iKeys, Menu menu)
	{
		if (active && menu.rehost > 0f)
		{
			item[0].text = new StringBuilder("Retrying..." + ((float)(int)menu.rehost + 1f));
			menu.rehost -= Game1.frameTime;
			if (menu.rehost <= 0f)
			{
				active = false;
				Game1.netSession.StartServer(menu);
			}
		}
		base.Update(iKeys, menu);
	}

	public override void SelectItem(Menu menu)
	{
		if (selected == 0 && !(menu.rehost > 0f))
		{
			active = false;
			menu.menuLevel[menu.errorOutLev].active = true;
		}
	}

	public override void Cancel(Menu menu)
	{
		active = false;
		menu.menuLevel[menu.errorOutLev].active = true;
	}
}
