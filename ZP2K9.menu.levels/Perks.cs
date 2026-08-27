using System.Text;
using ZP2K9.hud;

namespace ZP2K9.menu.levels;

public class Perks : MenuLevel
{
	public const int ITEM_PERK_OFFENSE = 0;

	public const int ITEM_PERK_MOD = 1;

	public const int ITEM_PERK_DEFENSE = 2;

	public const int ITEM_DONE = 3;

	public Perks()
	{
		name = new StringBuilder("Skills");
		item = new MenuItem[4]
		{
			new MenuItem(new string[10] { "", "", "", "", "", "", "", "", "", "" }, 0),
			new MenuItem(new string[10] { "", "", "", "", "", "", "", "", "", "" }, 1),
			new MenuItem(new string[10] { "", "", "", "", "", "", "", "", "", "" }, 2),
			new MenuItem("Done", 3)
		};
		item[0].perk = 0;
		item[1].perk = 1;
		item[2].perk = 2;
		width = 200;
		height = 420;
	}

	public override void Update(InterfaceKeys iKeys, Menu menu)
	{
		width = 330;
		height = 450;
		if (active)
		{
			if (Game1.zProfile.unlocks.perkUnlocked[0, item[0].selX] > 0)
			{
				Game1.zProfile.EditingSet().perk[0] = item[0].selX;
			}
			if (Game1.zProfile.unlocks.perkUnlocked[1, item[1].selX] > 0)
			{
				Game1.zProfile.EditingSet().perk[1] = item[1].selX;
			}
			if (Game1.zProfile.unlocks.perkUnlocked[2, item[2].selX] > 0)
			{
				Game1.zProfile.EditingSet().perk[2] = item[2].selX;
			}
		}
		base.Update(iKeys, menu);
	}

	public override void SelectItem(Menu menu)
	{
		int num = selected;
		if (num == 3)
		{
			UpdateUnlocks();
			active = false;
			Game1.store.Write(0);
			menu.menuLevel[17].active = true;
		}
	}

	public override void Cancel(Menu menu)
	{
		UpdateUnlocks();
		active = false;
		Game1.store.Write(0);
		menu.menuLevel[17].active = true;
	}

	private void UpdateUnlocks()
	{
		for (int i = 0; i < 3; i++)
		{
			if (Game1.zProfile.unlocks.perkUnlocked[i, item[i].selX] == 1)
			{
				Game1.zProfile.unlocks.perkUnlocked[i, item[i].selX] = 2;
			}
		}
	}

	public override void CycleOff(int i, int x)
	{
		if (i < 3 && Game1.zProfile.unlocks.perkUnlocked[i, x] == 1)
		{
			Game1.zProfile.unlocks.perkUnlocked[i, x] = 2;
		}
		base.CycleOff(i, x);
	}
}
