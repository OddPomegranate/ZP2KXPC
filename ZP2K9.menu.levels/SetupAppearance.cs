using System.Text;
using ZP2K9.hud;

namespace ZP2K9.menu.levels;

public class SetupAppearance : MenuLevel
{
	public const int ITEM_HAT = 0;

	public const int ITEM_HEAD = 1;

	public const int ITEM_TORSO = 2;

	public const int ITEM_LEGS = 3;

	public const int ITEM_JETPACK = 4;

	public const int ITEM_SKIN = 5;

	public const int ITEM_BODY = 6;

	public const int ITEM_TEAM = 7;

	public const int ITEM_DONE = 8;

	public SetupAppearance()
	{
		name = new StringBuilder("Player Setup");
		item = new MenuItem[9]
		{
			new MenuItem(new string[1] { "Hat: " }, 0),
			new MenuItem(new string[1] { "Head: " }, 1),
			new MenuItem(new string[1] { "Torso: " }, 2),
			new MenuItem(new string[1] { "Legs: " }, 3),
			new MenuItem(new string[1] { "Jet: " }, 4),
			new MenuItem(new string[1] { "Skin: " }, 5),
			new MenuItem(new string[2] { "Body: Guy", "Body: Girl" }, 6),
			new MenuItem(new string[2] { "Team: Humans", "Team: Zombies" }, 7),
			new MenuItem("Done", 8)
		};
		item[0].roster = true;
		item[6].bump = 10f;
		width = 200;
		height = 330;
	}

	public override void CycleOff(int i, int x)
	{
		int selX = item[6].selX;
		if (i == 4 && Game1.zProfile.unlocks.jetpackUnlocked[x] == 1)
		{
			Game1.zProfile.unlocks.jetpackUnlocked[x] = 2;
		}
		if (x > 0)
		{
			x--;
			if (selX == 0 || selX == 1)
			{
				switch (i)
				{
				case 0:
					if (selX == 0)
					{
						if (Game1.zProfile.unlocks.boyHatUnlocked[x] == 1)
						{
							Game1.zProfile.unlocks.boyHatUnlocked[x] = 2;
						}
					}
					else if (Game1.zProfile.unlocks.girlHatUnlocked[x] == 1)
					{
						Game1.zProfile.unlocks.girlHatUnlocked[x] = 2;
					}
					break;
				case 1:
					if (selX == 0)
					{
						if (Game1.zProfile.unlocks.boyHeadUnlocked[x] == 1)
						{
							Game1.zProfile.unlocks.boyHeadUnlocked[x] = 2;
						}
					}
					else if (Game1.zProfile.unlocks.girlHeadUnlocked[x] == 1)
					{
						Game1.zProfile.unlocks.girlHeadUnlocked[x] = 2;
					}
					break;
				case 2:
					if (selX == 0)
					{
						if (Game1.zProfile.unlocks.boyTorsoUnlocked[x] == 1)
						{
							Game1.zProfile.unlocks.boyTorsoUnlocked[x] = 2;
						}
					}
					else if (Game1.zProfile.unlocks.girlTorsoUnlocked[x] == 1)
					{
						Game1.zProfile.unlocks.girlTorsoUnlocked[x] = 2;
					}
					break;
				case 3:
					if (selX == 0)
					{
						if (Game1.zProfile.unlocks.boyLegsUnlocked[x] == 1)
						{
							Game1.zProfile.unlocks.boyLegsUnlocked[x] = 2;
						}
					}
					else if (Game1.zProfile.unlocks.girlLegsUnlocked[x] == 1)
					{
						Game1.zProfile.unlocks.girlLegsUnlocked[x] = 2;
					}
					break;
				}
			}
		}
		base.CycleOff(i, x);
	}

	public override void CheckNewUnlocks()
	{
		item[0].newunlock = false;
		item[1].newunlock = false;
		item[2].newunlock = false;
		item[3].newunlock = false;
		item[4].newunlock = false;
		if (item[6].selX == 0)
		{
			for (int i = 0; i < Game1.zProfile.unlocks.boyHatUnlocked.Length; i++)
			{
				if (Game1.zProfile.unlocks.boyHatUnlocked[i] == 1)
				{
					item[0].newunlock = true;
				}
			}
			for (int j = 0; j < Game1.zProfile.unlocks.boyHeadUnlocked.Length; j++)
			{
				if (Game1.zProfile.unlocks.boyHeadUnlocked[j] == 1)
				{
					item[1].newunlock = true;
				}
			}
			for (int k = 0; k < Game1.zProfile.unlocks.boyTorsoUnlocked.Length; k++)
			{
				if (Game1.zProfile.unlocks.boyTorsoUnlocked[k] == 1)
				{
					item[2].newunlock = true;
				}
			}
			for (int l = 0; l < Game1.zProfile.unlocks.boyLegsUnlocked.Length; l++)
			{
				if (Game1.zProfile.unlocks.boyLegsUnlocked[l] == 1)
				{
					item[3].newunlock = true;
				}
			}
		}
		if (item[6].selX == 1)
		{
			for (int m = 0; m < Game1.zProfile.unlocks.girlHatUnlocked.Length; m++)
			{
				if (Game1.zProfile.unlocks.girlHatUnlocked[m] == 1)
				{
					item[0].newunlock = true;
				}
			}
			for (int n = 0; n < Game1.zProfile.unlocks.girlHeadUnlocked.Length; n++)
			{
				if (Game1.zProfile.unlocks.girlHeadUnlocked[n] == 1)
				{
					item[1].newunlock = true;
				}
			}
			for (int num = 0; num < Game1.zProfile.unlocks.girlTorsoUnlocked.Length; num++)
			{
				if (Game1.zProfile.unlocks.girlTorsoUnlocked[num] == 1)
				{
					item[2].newunlock = true;
				}
			}
			for (int num2 = 0; num2 < Game1.zProfile.unlocks.girlLegsUnlocked.Length; num2++)
			{
				if (Game1.zProfile.unlocks.girlLegsUnlocked[num2] == 1)
				{
					item[3].newunlock = true;
				}
			}
		}
		for (int num3 = 0; num3 < Game1.zProfile.unlocks.jetpackUnlocked.Length; num3++)
		{
			if (Game1.zProfile.unlocks.jetpackUnlocked[num3] == 1)
			{
				item[4].newunlock = true;
			}
		}
		base.CheckNewUnlocks();
	}

	public override void Update(InterfaceKeys iKeys, Menu menu)
	{
		_ = item[selected].selX;
		base.Update(iKeys, menu);
		width = 300;
		height = 390;
		if (!active)
		{
			return;
		}
		Game1.zProfile.EditingSet().defaultTeam = item[7].selX;
		if (item[6].selX != Game1.zProfile.EditingSet().bodyType)
		{
			Game1.zProfile.EditingSet().bodyType = item[6].selX;
			item[1].selX = Game1.zProfile.EditingClass().headTex;
			item[0].selX = Game1.zProfile.EditingClass().hatTex;
			item[2].selX = Game1.zProfile.EditingClass().torsoTex;
			item[3].selX = Game1.zProfile.EditingClass().legsTex;
			item[4].selX = Game1.zProfile.EditingClass().jetpack;
		}
		if (item[6].selX == 0)
		{
			if (Game1.zProfile.unlocks.BoyHatUnlocked(item[0].selX) > 0)
			{
				Game1.zProfile.EditingClass().hatTex = item[0].selX;
			}
			if (Game1.zProfile.unlocks.BoyHeadUnlocked(item[1].selX) > 0)
			{
				Game1.zProfile.EditingClass().headTex = item[1].selX;
			}
			if (Game1.zProfile.unlocks.BoyTorsoUnlocked(item[2].selX) > 0)
			{
				Game1.zProfile.EditingClass().torsoTex = item[2].selX;
			}
			if (Game1.zProfile.unlocks.BoyLegsUnlocked(item[3].selX) > 0)
			{
				Game1.zProfile.EditingClass().legsTex = item[3].selX;
			}
		}
		else
		{
			if (Game1.zProfile.unlocks.GirlHatUnlocked(item[0].selX) > 0)
			{
				Game1.zProfile.EditingClass().hatTex = item[0].selX;
			}
			if (Game1.zProfile.unlocks.GirlHeadUnlocked(item[1].selX) > 0)
			{
				Game1.zProfile.EditingClass().headTex = item[1].selX;
			}
			if (Game1.zProfile.unlocks.GirlTorsoUnlocked(item[2].selX) > 0)
			{
				Game1.zProfile.EditingClass().torsoTex = item[2].selX;
			}
			if (Game1.zProfile.unlocks.GirlLegsUnlocked(item[3].selX) > 0)
			{
				Game1.zProfile.EditingClass().legsTex = item[3].selX;
			}
		}
		Game1.zProfile.EditingClass().skinTex = item[5].selX;
		if (Game1.zProfile.unlocks.jetpackUnlocked[item[4].selX] > 0)
		{
			Game1.zProfile.EditingClass().jetpack = item[4].selX;
		}
		_ = item[selected].selX;
		CheckNewUnlocks();
		item[2].maxX = Game1.bodyCatalog.bodyType[Game1.zProfile.EditingSet().bodyType].clothesList.Length;
		item[3].maxX = Game1.bodyCatalog.bodyType[Game1.zProfile.EditingSet().bodyType].clothesList.Length;
		item[1].maxX = Game1.bodyCatalog.bodyType[Game1.zProfile.EditingSet().bodyType].clothesList.Length;
		item[0].maxX = Game1.bodyCatalog.bodyType[Game1.zProfile.EditingSet().bodyType].hatList.Length;
		item[5].maxX = Game1.bodyCatalog.bodyType[Game1.zProfile.EditingSet().bodyType].skinList.Length - 1;
		item[4].maxX = 8;
		for (int i = 0; i < 6; i++)
		{
			if (item[i].selX > item[i].maxX)
			{
				item[i].selX = item[i].maxX;
			}
		}
		if (Game1.zProfile.EditingClass().skinTex > Game1.bodyCatalog.bodyType[Game1.zProfile.EditingSet().bodyType].skinList.Length)
		{
			Game1.zProfile.EditingClass().skinTex = Game1.bodyCatalog.bodyType[Game1.zProfile.EditingSet().bodyType].skinList.Length;
		}
		if (Game1.zProfile.EditingClass().hatTex > Game1.bodyCatalog.bodyType[Game1.zProfile.EditingSet().bodyType].hatList.Length)
		{
			Game1.zProfile.EditingClass().hatTex = Game1.bodyCatalog.bodyType[Game1.zProfile.EditingSet().bodyType].hatList.Length;
		}
		if (Game1.zProfile.EditingClass().headTex > Game1.bodyCatalog.bodyType[Game1.zProfile.EditingSet().bodyType].clothesList.Length)
		{
			Game1.zProfile.EditingClass().headTex = Game1.bodyCatalog.bodyType[Game1.zProfile.EditingSet().bodyType].clothesList.Length;
		}
		if (Game1.zProfile.EditingClass().torsoTex > Game1.bodyCatalog.bodyType[Game1.zProfile.EditingSet().bodyType].clothesList.Length)
		{
			Game1.zProfile.EditingClass().torsoTex = Game1.bodyCatalog.bodyType[Game1.zProfile.EditingSet().bodyType].clothesList.Length;
		}
		if (Game1.zProfile.EditingClass().legsTex > Game1.bodyCatalog.bodyType[Game1.zProfile.EditingSet().bodyType].clothesList.Length)
		{
			Game1.zProfile.EditingClass().legsTex = Game1.bodyCatalog.bodyType[Game1.zProfile.EditingSet().bodyType].clothesList.Length;
		}
	}

	public override void SelectItem(Menu menu)
	{
		int num = selected;
		if (num == 8)
		{
			active = false;
			Game1.store.Write(0);
			menu.menuLevel[17].active = true;
		}
	}

	public override void Cancel(Menu menu)
	{
		active = false;
		Game1.store.Write(0);
		menu.menuLevel[17].active = true;
	}
}
