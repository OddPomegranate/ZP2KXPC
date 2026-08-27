using System;
using System.Text;
using Microsoft.Xna.Framework;
using ZP2K9.platform;
using ZP2K9.hud;

namespace ZP2K9.menu.levels;

public class ClassList : MenuLevel
{
	public const int ITEM_CLANTAG = 8;

	public const int ITEM_BACK = 9;

	private IAsyncResult clanResult;

	private bool waitingForClanKeys;

	public ClassList()
	{
		name = new StringBuilder("Player Setup");
		item = new MenuItem[10]
		{
			new MenuItem("", 0),
			new MenuItem("", 1),
			new MenuItem("", 2),
			new MenuItem("", 3),
			new MenuItem("", 4),
			new MenuItem("", 5),
			new MenuItem("", 6),
			new MenuItem("", 7),
			new MenuItem("Clan Tag", 8),
			new MenuItem("Done", 9)
		};
		for (int i = 0; i < 8; i++)
		{
			item[i].classAtAGlance = true;
		}
		item[8].bump = 10f;
		width = 360;
		height = 632;
	}

	public override void CheckNewUnlocks()
	{
		for (int i = 0; i < 8; i++)
		{
			if (Game1.zProfile.unlocks.classUnlocked[i] > 0)
			{
				item[i].locked = false;
				item[i].disabled = false;
				if (Game1.zProfile.unlocks.classUnlocked[i] == 1)
				{
					item[i].newunlock = true;
				}
				else
				{
					item[i].newunlock = false;
				}
			}
			else
			{
				item[i].locked = true;
				item[i].disabled = true;
			}
		}
		if (Game1.zProfile.unlocks.clanTagUnlocked > 0)
		{
			item[8].locked = false;
			item[8].disabled = false;
			if (Game1.zProfile.unlocks.clanTagUnlocked == 1)
			{
				item[8].newunlock = true;
			}
			else
			{
				item[8].newunlock = false;
			}
		}
		else
		{
			item[8].locked = true;
			item[8].disabled = true;
		}
		base.CheckNewUnlocks();
	}

	public override void Update(InterfaceKeys iKeys, Menu menu)
	{
		if (active)
		{
			CheckNewUnlocks();
			if (waitingForClanKeys && clanResult != null && clanResult.IsCompleted)
			{
				try
				{
					waitingForClanKeys = false;
					string text = Guide.EndShowKeyboardInput(clanResult);
					if (text.Length > 3)
					{
						text = text.Substring(0, 3);
					}
                    if (text == null)
                    {
						text = "";
                    }
                    switch (text.ToLower())
					{
					default:
						Game1.impact.MeasureString(text);
						if (text.Length > 0)
						{
							Game1.zProfile.clanTag = new StringBuilder(text);
							item[8].text = new StringBuilder("Clan Tag: [" + Game1.zProfile.clanTag.ToString() + "]");
						}
						else
						{
							Game1.zProfile.clanTag = null;
							item[8].text = new StringBuilder("Clan Tag");
						}
						break;
					case "fuk":
					case "fuc":
					case "sht":
					case "nig":
					case "kkk":
					case "cok":
					case "c0k":
					case "nïg":
					case "nîg":
					case "níg":
					case "nìg":
					case "ñig":
					case "ñïg":
					case "ñîg":
					case "ñíg":
					case "ñìg":
					case "dïk":
					case "dîk":
					case "dík":
					case "dìk":
					case "còk":
					case "cók":
					case "côk":
					case "cõk":
					case "cök":
					case "cøk":
					case "jiz":
					case "cum":
					case "ass":
					case "as$":
					case "a$s":
					case "a$$":
					case "@$$":
					case "@sš":
					case "@šš":
					case "@šs":
					case "@$š":
					case "@š$":
					case "àss":
					case "às$":
					case "à$s":
					case "à$$":
					case "àsš":
					case "àšš":
					case "àšs":
					case "à$š":
					case "àš$":
					case "áss":
					case "ás$":
					case "á$s":
					case "á$$":
					case "ásš":
					case "ášš":
					case "ášs":
					case "á$š":
					case "áš$":
					case "âss":
					case "âs$":
					case "â$s":
					case "â$$":
					case "âsš":
					case "âšš":
					case "âšs":
					case "â$š":
					case "âš$":
					case "ãss":
					case "ãs$":
					case "ã$s":
					case "ã$$":
					case "ãsš":
					case "ãšš":
					case "ãšs":
					case "ã$š":
					case "ãš$":
					case "äss":
					case "äs$":
					case "ä$s":
					case "ä$$":
					case "äsš":
					case "äšš":
					case "äšs":
					case "ä$š":
					case "äš$":
					case "åss":
					case "ås$":
					case "å$s":
					case "å$$":
					case "åsš":
					case "åšš":
					case "åšs":
					case "å$š":
					case "åš$":
					case "sex":
					case "kum":
					case "dik":
					case "8=d":
					case "d1k":
					case "n1g":
					case "@ss":
					case "fag":
					case "f@g":
					case "fāg":
					case "făg":
					case "fąg":
					case "fâg":
					case "fàg":
					case "fág":
					case "fãg":
					case "fäg":
					case "fåg":
					case "fæg":
					case "j1z":
					case "cūm":
					case "cùm":
					case "cüm":
					case "cũm":
					case "cûm":
					case "cúm":
					case "cűm":
					case "cųm":
					case "çum":
					case "çūm":
					case "çùm":
					case "çüm":
					case "çũm":
					case "çûm":
					case "çúm":
					case "çűm":
					case "çųm":
					case "çok":
					case "çòk":
					case "çók":
					case "çôk":
					case "çõk":
					case "çök":
					case "çøk":
					case "kūm":
					case "kùm":
					case "küm":
					case "kũm":
					case "kûm":
					case "kúm":
					case "kűm":
					case "kųm":
					case "fūk":
					case "fùk":
					case "fük":
					case "fũk":
					case "fûk":
					case "fúk":
					case "fűk":
					case "fųk":
						break;
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.ToString());
				}
			}
		}
		base.Update(iKeys, menu);
	}

	public override void ItemHitY(Menu menu)
	{
		if (item[selected].disabled || item[selected].locked || Game1.zProfile.unlocks.perkEditorUnlocked == 0)
		{
			return;
		}
		switch (selected)
		{
		case 0:
		case 1:
		case 2:
		case 3:
		case 4:
		case 5:
		case 6:
		case 7:
			if (Game1.zProfile.unlocks.classUnlocked[selected] == 1)
			{
				Game1.zProfile.unlocks.classUnlocked[selected] = 2;
			}
			Sound.PlayCue("pop");
			Game1.zProfile.editingClass = selected;
			active = false;
			menu.menuLevel[17].active = true;
			menu.menuLevel[17].name = new StringBuilder(Game1.zProfile.EditingSet().name);
			break;
		}
	}

	public override void SelectItem(Menu menu)
	{
		if (selected < 8 && Game1.zProfile.unlocks.classUnlocked[selected] == 0)
		{
			return;
		}
		switch (selected)
		{
		case 0:
		case 1:
		case 2:
		case 3:
		case 4:
		case 5:
		case 6:
		case 7:
			if (Game1.zProfile.unlocks.classUnlocked[selected] == 1)
			{
				Game1.zProfile.unlocks.classUnlocked[selected] = 2;
			}
			if (Game1.zProfile.defaultClass != selected)
			{
				Game1.zProfile.defaultClass = selected;
				break;
			}
			active = false;
			Game1.store.Write(0);
			if (GameState.mode == 2)
			{
				menu.menuLevel[0].active = true;
			}
			else
			{
				menu.menuLevel[9].active = true;
			}
			break;
		case 8:
			try
			{
				clanResult = Guide.BeginShowKeyboardInput((PlayerIndex)Game1.mainPlayerIndex, "Enter clan tag:", "Tags will be truncated to three characters.", (Game1.zProfile.clanTag != null) ? Game1.zProfile.clanTag.ToString() : "SKA", (AsyncCallback)null, (object)null);
				waitingForClanKeys = true;
				if (Game1.zProfile.unlocks.clanTagUnlocked == 1)
				{
					Game1.zProfile.unlocks.clanTagUnlocked = 2;
				}
				break;
			}
			catch
			{
				break;
			}
		case 9:
			active = false;
			Game1.store.Write(0);
			if (GameState.mode == 2)
			{
				menu.menuLevel[0].active = true;
			}
			else
			{
				menu.menuLevel[9].active = true;
			}
			break;
		}
	}

	public override void Cancel(Menu menu)
	{
		active = false;
		Game1.store.Write(0);
		if (GameState.mode == 2)
		{
			menu.menuLevel[0].active = true;
		}
		else
		{
			menu.menuLevel[9].active = true;
		}
		base.Cancel(menu);
	}
}
