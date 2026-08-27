using System;
using System.Text;
using Microsoft.Xna.Framework;
using ZP2K9.platform;
using ZP2K9.hud;

namespace ZP2K9.menu.levels;

public class Editor : MenuLevel
{
	private const int ITEM_RENAME = 0;

	private const int ITEM_LOAD = 1;

	private const int ITEM_SAVE = 2;

	private const int ITEM_NEW = 3;

	private const int ITEM_BACK = 4;

	private const int ITEM_MENU = 5;

	private IAsyncResult renameResult;

	private bool pendingRename;

	public Editor()
	{
		name = new StringBuilder("Editor Menu");
		item = new MenuItem[6]
		{
			new MenuItem("Filename: " + Game1.store.mapPath, 0),
			new MenuItem("Load", 1),
			new MenuItem("Save", 2),
			new MenuItem("New", 3),
			new MenuItem("Back to Editor", 4),
			new MenuItem("Back to Main Menu", 5)
		};
		width = 200;
		height = 300;
	}

	public override void Update(InterfaceKeys iKeys, Menu menu)
	{
		if (pendingRename && renameResult.IsCompleted)
		{
			string text = "";
			try
			{
				text = Guide.EndShowKeyboardInput(renameResult);
			}
			catch (Exception)
			{
			}
			pendingRename = false;
			if (text != "")
			{
				Game1.store.mapPath = text;
			}
			item[0] = new MenuItem("Filename: " + Game1.store.mapPath, 0);
		}
		base.Update(iKeys, menu);
	}

	public override void SelectItem(Menu menu)
	{
		if (!pendingRename)
		{
			switch (selected)
			{
			case 0:
				renameResult = Guide.BeginShowKeyboardInput((PlayerIndex)Game1.mainPlayerIndex, "Rename", "Rename map", Game1.store.mapPath, (AsyncCallback)null, (object)null);
				pendingRename = true;
				break;
			case 1:
				Game1.store.Read(2);
				active = false;
				GameState.mode = 0;
				break;
			case 2:
				Game1.store.Write(2);
				active = false;
				GameState.mode = 0;
				break;
			case 4:
				active = false;
				GameState.mode = 0;
				break;
			case 5:
				active = false;
				menu.menuLevel[0].active = true;
				break;
			case 3:
				break;
			}
		}
	}

	public override void Cancel(Menu menu)
	{
		if (!pendingRename)
		{
			active = false;
			GameState.mode = 0;
		}
	}
}
