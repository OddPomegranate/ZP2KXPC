using System.Collections.ObjectModel;
using System.Text;
using ZP2K9.hud;

namespace ZP2K9.menu.levels;

public class Searching : MenuLevel
{
	private const int ITEM_CANCEL = 0;

	public Searching()
	{
		name = new StringBuilder("Searching...");
		item = new MenuItem[1]
		{
			new MenuItem("Cancel", 0)
		};
		width = 200;
		height = 300;
	}

	public override void Update(InterfaceKeys iKeys, Menu menu)
	{
		if (active)
		{
			if (Game1.netSession.findFailed)
			{
				Game1.netSession.findFailed = false;
				active = false;
				menu.menuLevel[(Game1.netSession.netType == 2) ? 5 : 6].active = true;
			}
			else
			{
				bool flag = false;
				if (!Game1.netSession.pendingFind)
				{
					if (Game1.netSession.sessions != null)
					{
						if ((Game1.netSession.sessions).Count > 0)
						{
							menu.menuLevel[8] = new ListGames();
							active = false;
							menu.menuLevel[8].active = true;
						}
						else
						{
							flag = true;
						}
					}
					else
					{
						flag = true;
					}
				}
				if (flag)
				{
					active = false;
					Game1.netSession.Kill();
					menu.menuLevel[(Game1.netSession.netType == 2) ? 5 : 6].active = true;
				}
			}
		}
		base.Update(iKeys, menu);
	}

	public override void SelectItem(Menu menu)
	{
		if (selected == 0)
		{
			active = false;
			menu.menuLevel[(Game1.netSession.netType == 2) ? 5 : 6].active = true;
		}
	}

	public override void Cancel(Menu menu)
	{
		active = false;
		Game1.netSession.Kill();
		menu.menuLevel[(Game1.netSession.netType == 2) ? 5 : 6].active = true;
	}
}
