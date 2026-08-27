using System.Text;
using ZP2K9.hud;
using ZP2K9.net;

namespace ZP2K9.menu.levels;

public class EditMapList : MenuLevel
{
	public EditMapList()
	{
		name = new StringBuilder("Map List");
		width = 200;
		height = 300;
	}

	public void Init()
	{
		item = new MenuItem[MapList.mapCatalog.Count + 1];
		for (int i = 0; i < MapList.mapCatalog.Count; i++)
		{
			item[i] = new MenuItem(MapList.mapCatalog[i].name.ToString(), i);
		}
		item[item.Length - 1] = new MenuItem("Done", item.Length - 1);
	}

	public override void Update(InterfaceKeys iKeys, Menu menu)
	{
		if (active)
		{
			for (int i = 0; i < item.Length - 1; i++)
			{
				if (MapList.mapCatalog[i].included)
				{
					item[i].mapList = 1;
				}
				else
				{
					item[i].mapList = 2;
				}
			}
		}
		base.Update(iKeys, menu);
	}

	public override void SelectItem(Menu menu)
	{
		if (selected == item.Length - 1)
		{
			active = false;
			menu.menuLevel[21].active = true;
		}
		else
		{
			MapList.mapCatalog[selected].included = !MapList.mapCatalog[selected].included;
		}
	}

	public override void Cancel(Menu menu)
	{
		active = false;
		menu.menuLevel[21].active = true;
	}
}
