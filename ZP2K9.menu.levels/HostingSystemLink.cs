using System.Collections.ObjectModel;
using System.Text;
using ZP2K9.hud;

namespace ZP2K9.menu.levels;

public class HostingSystemLink : MenuLevel
{
	private const int ITEM_CREATING = 0;

	private const int ITEM_BACK = 2;

	public HostingSystemLink()
	{
		name = new StringBuilder("Hosting System Link");
		item = new MenuItem[3]
		{
			new MenuItem("Creating...", 0),
			null,
			new MenuItem("Back", 2)
		};
		width = 200;
		height = 300;
	}

	public override void Update(InterfaceKeys iKeys, Menu menu)
	{
		selected = 2;
		if (Game1.netSession.netSession != null)
		{
			_ = (Game1.netSession.netSession.AllGamers).Count;
		}
		base.Update(iKeys, menu);
	}

	public override void SelectItem(Menu menu)
	{
		switch (selected)
		{
		case 2:
			active = false;
			menu.menuLevel[0].active = true;
			break;
		case 0:
		case 1:
			break;
		}
	}

	public override void Cancel(Menu menu)
	{
		active = false;
		menu.menuLevel[0].active = true;
	}
}
