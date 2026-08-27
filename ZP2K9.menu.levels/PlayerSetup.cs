using System;
using System.Text;
using Microsoft.Xna.Framework;
using ZP2K9.platform;
using ZP2K9.hud;

namespace ZP2K9.menu.levels;

public class PlayerSetup : MenuLevel
{
	public const int ITEM_RENAME = 0;

	public const int ITEM_APPEARANCE = 1;

	public const int ITEM_PERKS = 2;

	public const int ITEM_DONE = 3;

	public PlayerSetup()
	{
		name = new StringBuilder("Player Setup");
		item = new MenuItem[4]
		{
			new MenuItem("Rename", 0),
			new MenuItem("Appearance", 1),
			new MenuItem("Skills", 2),
			new MenuItem("Done", 3)
		};
		item[1].appearanceAtAGlance = true;
		item[2].perksAtAGlance = true;
		item[2].newBump = -5f;
		width = 250;
		height = 300;
	}

	public override void CheckNewUnlocks()
	{
		if (Game1.zProfile.unlocks.renameUnlocked > 0)
		{
			item[0].locked = false;
			item[0].disabled = false;
		}
		else
		{
			item[0].locked = true;
			item[0].disabled = true;
		}
		if (Game1.zProfile.unlocks.appearanceEditorUnlocked > 0)
		{
			item[1].locked = false;
			item[1].disabled = false;
		}
		else
		{
			item[1].locked = true;
			item[1].disabled = true;
		}
		if (Game1.zProfile.unlocks.perkEditorUnlocked > 0)
		{
			item[2].locked = false;
			item[2].disabled = false;
		}
		else
		{
			item[2].locked = true;
			item[2].disabled = true;
		}
		if (Game1.zProfile.unlocks.renameUnlocked == 1)
		{
			item[0].newunlock = true;
		}
		else
		{
			item[0].newunlock = false;
		}
		if (Game1.zProfile.unlocks.appearanceEditorUnlocked == 1)
		{
			item[1].newunlock = true;
		}
		else
		{
			item[1].newunlock = false;
		}
		if (Game1.zProfile.unlocks.appearanceEditorUnlocked != 0)
		{
			for (int i = 0; i < Game1.zProfile.unlocks.boyHeadUnlocked.Length; i++)
			{
				if (Game1.zProfile.unlocks.boyHeadUnlocked[i] == 1)
				{
					item[1].newunlock = true;
				}
			}
			for (int j = 0; j < Game1.zProfile.unlocks.boyTorsoUnlocked.Length; j++)
			{
				if (Game1.zProfile.unlocks.boyTorsoUnlocked[j] == 1)
				{
					item[1].newunlock = true;
				}
			}
			for (int k = 0; k < Game1.zProfile.unlocks.boyLegsUnlocked.Length; k++)
			{
				if (Game1.zProfile.unlocks.boyLegsUnlocked[k] == 1)
				{
					item[1].newunlock = true;
				}
			}
			for (int l = 0; l < Game1.zProfile.unlocks.girlHeadUnlocked.Length; l++)
			{
				if (Game1.zProfile.unlocks.girlHeadUnlocked[l] == 1)
				{
					item[1].newunlock = true;
				}
			}
			for (int m = 0; m < Game1.zProfile.unlocks.girlTorsoUnlocked.Length; m++)
			{
				if (Game1.zProfile.unlocks.girlTorsoUnlocked[m] == 1)
				{
					item[1].newunlock = true;
				}
			}
			for (int n = 0; n < Game1.zProfile.unlocks.girlLegsUnlocked.Length; n++)
			{
				if (Game1.zProfile.unlocks.girlLegsUnlocked[n] == 1)
				{
					item[1].newunlock = true;
				}
			}
			for (int num = 0; num < Game1.zProfile.unlocks.boyHatUnlocked.Length; num++)
			{
				if (Game1.zProfile.unlocks.boyHatUnlocked[num] == 1)
				{
					item[1].newunlock = true;
				}
			}
			for (int num2 = 0; num2 < Game1.zProfile.unlocks.girlHatUnlocked.Length; num2++)
			{
				if (Game1.zProfile.unlocks.girlHatUnlocked[num2] == 1)
				{
					item[1].newunlock = true;
				}
			}
		}
		if (Game1.zProfile.unlocks.perkEditorUnlocked == 1)
		{
			item[2].newunlock = true;
		}
		else
		{
			item[2].newunlock = false;
		}
		if (Game1.zProfile.unlocks.perkEditorUnlocked != 0)
		{
			for (int num3 = 0; num3 < 3; num3++)
			{
				for (int num4 = 0; num4 < 10; num4++)
				{
					if (Game1.zProfile.unlocks.perkUnlocked[num3, num4] == 1)
					{
						item[2].newunlock = true;
					}
				}
			}
		}
		base.CheckNewUnlocks();
	}

	public override void Update(InterfaceKeys iKeys, Menu menu)
	{
		if (active)
		{
			CheckNewUnlocks();
		}
		base.Update(iKeys, menu);
	}

	public override void SelectItem(Menu menu)
	{
		switch (selected)
		{
		case 0:
			try
			{
				Game1.menu.keyResult = Guide.BeginShowKeyboardInput((PlayerIndex)Game1.mainPlayerIndex, "Enter class name:", "Class Name", Game1.zProfile.EditingSet().name, (AsyncCallback)null, (object)null);
				Game1.menu.waitingKeyResult = true;
				Game1.zProfile.unlocks.renameUnlocked = 2;
				break;
			}
			catch
			{
				break;
			}
		case 1:
			active = false;
			menu.menuLevel[10].active = true;
			menu.menuLevel[10].item[0].selX = Game1.zProfile.EditingClass().hatTex;
			menu.menuLevel[10].item[1].selX = Game1.zProfile.EditingClass().headTex;
			menu.menuLevel[10].item[2].selX = Game1.zProfile.EditingClass().torsoTex;
			menu.menuLevel[10].item[3].selX = Game1.zProfile.EditingClass().legsTex;
			menu.menuLevel[10].item[4].selX = Game1.zProfile.EditingClass().jetpack;
			menu.menuLevel[10].item[7].selX = Game1.zProfile.EditingSet().defaultTeam;
			menu.menuLevel[10].item[6].selX = Game1.zProfile.EditingSet().bodyType;
			menu.menuLevel[10].item[5].selX = Game1.zProfile.EditingClass().skinTex;
			Game1.zProfile.unlocks.appearanceEditorUnlocked = 2;
			break;
		case 2:
			active = false;
			menu.menuLevel[18].active = true;
			menu.menuLevel[18].item[0].selX = Game1.zProfile.EditingSet().perk[0];
			menu.menuLevel[18].item[1].selX = Game1.zProfile.EditingSet().perk[1];
			menu.menuLevel[18].item[2].selX = Game1.zProfile.EditingSet().perk[2];
			Game1.zProfile.unlocks.perkEditorUnlocked = 2;
			break;
		case 3:
			active = false;
			Game1.store.Write(0);
			menu.menuLevel[19].active = true;
			break;
		}
	}

	public override void Cancel(Menu menu)
	{
		active = false;
		Game1.store.Write(0);
		menu.menuLevel[19].active = true;
	}
}
