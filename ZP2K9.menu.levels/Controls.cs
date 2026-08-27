using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ZP2K9.hud;

namespace ZP2K9.menu.levels;

public class Controls : MenuLevel
{
	private const int ITEM_BACK = 0;

	private StringBuilder LBJetPack;

	private StringBuilder LTRoll;

	private StringBuilder LSMove;

	private StringBuilder LSMoveJetpack;

	private StringBuilder RSShoot;

	private StringBuilder RSAim;

	private StringBuilder RTPrimeGren1;

	private StringBuilder RTShoot;

	private StringBuilder RBPrimeGren2;

	private StringBuilder RBPrimeGrenade;

	private StringBuilder XReloadPickup;

	private StringBuilder YSwitchWeapon;

	private StringBuilder AJumpAccept;

	private StringBuilder BCancelGrenades;

	private StringBuilder DPadSwitchWeapon;

	public Controls()
	{
		item = new MenuItem[1]
		{
			new MenuItem("Back", 0)
		};
		name = new StringBuilder("Controls");
		width = 1000;
		height = 600;
		LBJetPack = new StringBuilder("LB: Jetpack");
		LTRoll = new StringBuilder("LT: Roll");
		LSMove = new StringBuilder("LS: Move");
		LSMoveJetpack = new StringBuilder("LS: Move/Jetpack");
		RSShoot = new StringBuilder("RS: Shoot");
		RSAim = new StringBuilder("RS: Aim");
		RTPrimeGren1 = new StringBuilder("RT: Prime Grenade 1");
		RTShoot = new StringBuilder("RT: Shoot");
		RBPrimeGren2 = new StringBuilder("RB: Prime Grenade 2");
		RBPrimeGrenade = new StringBuilder("RB: Throw Grenade");
		XReloadPickup = new StringBuilder("X: Reload/Pickup");
		YSwitchWeapon = new StringBuilder("Y: Switch Weapon");
		AJumpAccept = new StringBuilder("A: Jump/Accept");
		BCancelGrenades = new StringBuilder("B: Switch Grenades/Cancel");
		DPadSwitchWeapon = new StringBuilder("Select Weapon");
		isControls = true;
	}

	public override void Update(InterfaceKeys iKeys, Menu menu)
	{
		base.Update(iKeys, menu);
		width = 1000;
		height = 492;
	}

	public override void SelectItem(Menu menu)
	{
		if (selected == 0)
		{
			active = false;
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
		if (GameState.mode == 2)
		{
			menu.menuLevel[0].active = true;
		}
		else
		{
			menu.menuLevel[9].active = true;
		}
	}

	internal void DrawControls(float alpha, SpriteBatch sprite)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		Vector2 vector = new Vector2(640f, 360f);
		float num = 22f;
		Game1.text.size = 1f;
		Game1.text.color = new Color(1f, 1f, 1f, alpha);
		Game1.text.DrawString(vector + new Vector2(-332f, -188f), LTRoll, 2, -1f, Game1.impact, sprite);
		Game1.text.DrawString(vector + new Vector2(-332f, -188f - num), LBJetPack, 2, -1f, Game1.impact, sprite);
		Game1.text.DrawString(vector + new Vector2(-322f, -136f), Game1.settings.upToJetpack ? LSMoveJetpack : LSMove, 2, -1f, Game1.impact, sprite);
		Game1.text.DrawString(vector + new Vector2(-143f, 131f), DPadSwitchWeapon, 0, -1f, Game1.impact, sprite);
		Game1.text.DrawString(vector + new Vector2(156f, 137f), Game1.settings.twinStickShooter ? RSShoot : RSAim, 0, -1f, Game1.impact, sprite);
		Game1.text.DrawString(vector + new Vector2(147f, -36f), AJumpAccept, 0, -1f, Game1.impact, sprite);
		Game1.text.DrawString(vector + new Vector2(139f, -77f), BCancelGrenades, 0, -1f, Game1.impact, sprite);
		Game1.text.DrawString(vector + new Vector2(136f, -112f), XReloadPickup, 0, -1f, Game1.impact, sprite);
		Game1.text.DrawString(vector + new Vector2(124f, -139f), YSwitchWeapon, 0, -1f, Game1.impact, sprite);
		Game1.text.DrawString(vector + new Vector2(98f, -185f), Game1.settings.twinStickShooter ? RTPrimeGren1 : RTShoot, 0, -1f, Game1.impact, sprite);
		Game1.text.DrawString(vector + new Vector2(98f, -185f - num), Game1.settings.twinStickShooter ? RBPrimeGren2 : RBPrimeGrenade, 0, -1f, Game1.impact, sprite);
	}
}
