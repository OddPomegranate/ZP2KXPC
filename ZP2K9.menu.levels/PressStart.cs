using System.Text;
using ZP2K9.hud;

namespace ZP2K9.menu.levels;

public class PressStart : MenuLevel
{
	private const int ITEM_XBOXLIVE = 0;

	private const int ITEM_SYSTEMLINK = 1;

	private const int ITEM_PLAYER_SETUP = 2;

	private const int ITEM_EDITOR = 3;

	private const int ITEM_QUIT = 4;

	public PressStart()
	{
		item = new MenuItem[0];
		name = new StringBuilder("Press Start!");
		width = 200;
		height = 90;
		isStartGate = true;
	}

	public override void Update(InterfaceKeys iKeys, Menu menu)
	{
		base.Update(iKeys, menu);
		height = 50;
	}

	public override void SelectItem(Menu menu)
	{
	}

	public override void Cancel(Menu menu)
	{
	}
}
