using System.Text;
using ZP2K9.hud;

namespace ZP2K9.menu.levels;

public class GameSettings : MenuLevel
{
	public const int ITEM_SHOWNAMES = 0;

	public const int ITEM_VIBRATION = 1;

	public const int ITEM_AUTOSWITCH = 2;

	public const int ITEM_UPTOJETPACK = 3;

	public const int ITEM_TWINSTICKSTYLE = 4;

	public const int ITEM_SFX = 5;

	public const int ITEM_BGM = 6;

	private const int ITEM_BACK = 7;

	public GameSettings()
	{
		item = new MenuItem[8]
		{
			new MenuItem(new string[2] { "Show Names: Off", "Show Names: On" }, 0),
			new MenuItem(new string[2] { "Vibration: Off", "Vibration: On" }, 1),
			new MenuItem(new string[2] { "Autoswitch: Off", "Autoswitch: On" }, 2),
			new MenuItem(new string[2] { "Jetpack Style: LB Only", "Jetpack Style: Twin Stick" }, 3),
			new MenuItem(new string[2] { "Shooting Style: Trigger Shoot", "Shooting Style: Twin Stick" }, 4),
			new MenuItem(new string[11]
			{
				"SFX: Off", "SFX: 10%", "SFX: 20%", "SFX: 30%", "SFX: 40%", "SFX: 50%", "SFX: 60%", "SFX: 70%", "SFX: 80%", "SFX: 90%",
				"SFX: Max"
			}, 5),
			new MenuItem(new string[11]
			{
				"BGM: Off", "BGM: 10%", "BGM: 20%", "BGM: 30%", "BGM: 40%", "BGM: 50%", "BGM: 60%", "BGM: 70%", "BGM: 80%", "BGM: 90%",
				"BGM: Max"
			}, 6),
			new MenuItem("Back", 7)
		};
		name = new StringBuilder("Server Settings");
		width = 350;
		height = 300;
	}

	public override void Update(InterfaceKeys iKeys, Menu menu)
	{
		if (active)
		{
			Game1.settings.vibration = item[1].selX == 1;
			Game1.settings.showNames = item[0].selX == 1;
			Game1.settings.autoSwitch = item[2].selX == 1;
			Game1.settings.upToJetpack = item[3].selX == 1;
			Game1.settings.twinStickShooter = item[4].selX == 1;
			Game1.settings.sfx = item[5].selX;
			Game1.settings.bgm = item[6].selX;
		}
		base.Update(iKeys, menu);
	}

	public override void SelectItem(Menu menu)
	{
		int num = selected;
		if (num == 7)
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
	}
}
