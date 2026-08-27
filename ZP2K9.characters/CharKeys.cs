using System;
using Microsoft.Xna.Framework;
using ZP2K9.platform;
using Microsoft.Xna.Framework.Input;

namespace ZP2K9.characters;

public class CharKeys
{
	public bool keyLeft;

	public bool keyRight;

	public bool keyJump;

	public bool keyUp;

	public bool keyDown;

	public bool keyBack;

	public float jumpPower;

	public bool keyDUp;

	public bool keyDRight;

	public bool keyDDown;

	public bool keyDLeft;

	public bool keyA;

	public bool keyX;

	public bool keyB;

	public bool keyY;

	public bool keyPickup;

	public bool keyReload;

	private float xFrame;

	public bool keyGrenade;

	public bool keyGren2;

	public bool keyStart;

	public Vector2 shootVec;

	public Vector2 runVec;

	public bool keyShoot;

	public bool keyLeftShoulder;

	public bool keyRightShoulder;

	private GamePadState pgs;

	public bool keyKick;

	public bool keyRoll;

	public bool keyFloat;

	public bool keyJetpack;

	public bool keySquat;

	public bool keySuicide;

	private float suicideFrame;

	public float runSpeed;

	public bool KeyPickup()
	{
		if (keyX)
		{
			return xFrame > 0.15f;
		}
		return false;
	}

	public void ClearKeys()
	{
		keyLeft = false;
		keyRight = false;
		keyJump = false;
		keyUp = false;
		keyDown = false;
		keyBack = false;
		keyStart = false;
		keyDUp = false;
		keyDRight = false;
		keyDDown = false;
		keyDLeft = false;
		keyJetpack = false;
		keyKick = false;
		keyRoll = false;
		keyShoot = false;
		keyGren2 = false;
		keyGrenade = false;
		keyFloat = false;
		keyA = false;
		keyB = false;
		keyY = false;
		keyPickup = false;
		keyReload = false;
		keyLeftShoulder = false;
		keyRightShoulder = false;
		shootVec.X = 0f;
		shootVec.Y = 0f;
		runVec.X = 0f;
		runVec.Y = 0f;
		keySquat = false;
		jumpPower = 0f;
		keySuicide = false;
	}

	public void Update(GamePadState gs, Character c)
	{
		ClearKeys();
		if (Game1.menu.IsActive() || Game1.netSession.postLobby || Guide.IsVisible)
		{
			return;
		}
		if (gs.Buttons.LeftShoulder == ButtonState.Pressed && pgs.Buttons.LeftShoulder == ButtonState.Released)
		{
			keyJump = true;
			keyLeftShoulder = true;
		}
		if (gs.Buttons.LeftShoulder == ButtonState.Pressed)
		{
			keyJetpack = true;
		}
		if (gs.Buttons.Start == ButtonState.Pressed && pgs.Buttons.Start == ButtonState.Released)
		{
			keyStart = true;
		}
		if (gs.Buttons.RightStick == ButtonState.Pressed && pgs.Buttons.RightStick == ButtonState.Released)
		{
			keyKick = true;
		}
		if (gs.Buttons.LeftStick == ButtonState.Pressed && pgs.Buttons.LeftStick == ButtonState.Released)
		{
			keySquat = true;
		}
		if (gs.Triggers.Left > 0.3f && pgs.Triggers.Left <= 0.3f)
		{
			keyRoll = true;
		}
		if (gs.Triggers.Left > 0.3f)
		{
			keyFloat = true;
		}
		if (gs.Buttons.A == ButtonState.Pressed && pgs.Buttons.A == ButtonState.Released)
		{
			keyA = true;
		}
		if (gs.Triggers.Left > 0.5f && gs.ThumbSticks.Left.Y < -0.5f && gs.Buttons.X == ButtonState.Pressed)
		{
			suicideFrame += Game1.frameTime;
			if (suicideFrame > 0.5f)
			{
				keySuicide = true;
			}
		}
		else
		{
			suicideFrame = 0f;
		}
		if (gs.Buttons.B == ButtonState.Pressed && pgs.Buttons.B == ButtonState.Released)
		{
			keyB = true;
		}
		if (gs.Buttons.X == ButtonState.Pressed)
		{
			keyX = true;
			xFrame += Game1.frameTime;
		}
		else
		{
			if (keyX)
			{
				keyX = false;
				if (xFrame < 0.35f)
				{
					keyReload = true;
				}
				else
				{
					keyPickup = true;
				}
			}
			xFrame = 0f;
		}
		if (gs.Buttons.Y == ButtonState.Pressed && pgs.Buttons.Y == ButtonState.Released)
		{
			keyY = true;
		}
		if (gs.Buttons.RightShoulder == ButtonState.Pressed && pgs.Buttons.RightShoulder == ButtonState.Released)
		{
			keyRightShoulder = true;
		}
		if (gs.Buttons.Back == ButtonState.Pressed && pgs.Buttons.Back == ButtonState.Released)
		{
			keyBack = true;
		}
		if (gs.DPad.Right == ButtonState.Pressed && pgs.DPad.Right == ButtonState.Released)
		{
			keyDRight = true;
		}
		if (gs.DPad.Right == ButtonState.Pressed && pgs.DPad.Right == ButtonState.Released)
		{
			keyDRight = true;
		}
		if (gs.DPad.Left == ButtonState.Pressed && pgs.DPad.Left == ButtonState.Released)
		{
			keyDLeft = true;
		}
		if (gs.DPad.Up == ButtonState.Pressed && pgs.DPad.Up == ButtonState.Released)
		{
			keyDUp = true;
		}
		if (gs.DPad.Down == ButtonState.Pressed && pgs.DPad.Down == ButtonState.Released)
		{
			keyDDown = true;
		}
		if (gs.Triggers.Right > 0.3f)
		{
			keyGrenade = true;
		}
		if (gs.Buttons.RightShoulder == ButtonState.Pressed)
		{
			keyGren2 = true;
		}
		if (gs.ThumbSticks.Left.X < -0.2f)
		{
			keyLeft = true;
		}
		if (gs.ThumbSticks.Left.X > 0.2f)
		{
			keyRight = true;
		}
		if (gs.ThumbSticks.Left.Y < -0.2f)
		{
			keyDown = true;
		}
		if (gs.ThumbSticks.Left.Y > 0.3f)
		{
			keyUp = true;
		}
		jumpPower = gs.ThumbSticks.Left.Y;
		if (keyJump || keyA)
		{
			jumpPower = 1f;
		}
		if (GameState.gameType == 4 && c.team == 1)
		{
			jumpPower = 1f;
		}
		runVec = gs.ThumbSticks.Left;
		runSpeed = runVec.Length();
		runSpeed *= 1.2f;
		if (runSpeed > 1f)
		{
			runSpeed = 1f;
		}
		shootVec = gs.ThumbSticks.Right;
		shootVec.Y = 0f - shootVec.Y;
		if (!Game1.settings.twinStickShooter)
		{
			bool flag = keyGrenade;
			bool flag2 = keyGren2;
			if (flag || flag2)
			{
				Vector2 vector;
				if (shootVec.X == 0f && shootVec.Y == 0f)
				{
					if (runVec.X == 0f && runVec.Y == 0f)
					{
						vector = new Vector2((float)Math.Cos(c.angle), (float)Math.Sin(c.angle));
						vector.Y = 0f - vector.Y;
						if (c.face == 0)
						{
							vector = -vector;
						}
					}
					else
					{
						vector = runVec;
						vector.Y = 0f - vector.Y;
					}
				}
				else
				{
					vector = shootVec;
				}
				if (!flag2 || vector.Length() < 0.61f)
				{
					vector.Normalize();
					vector *= 0.61f;
				}
				if (c.grenAmmo[0] <= 0 && !flag)
				{
					vector.Normalize();
					vector *= 0.59f;
				}
				shootVec = vector;
			}
			else if ((shootVec.X != 0f || shootVec.Y != 0f) && shootVec.Length() > 0.59f)
			{
				shootVec.Normalize();
				shootVec *= 0.59f;
			}
			keyGren2 = false;
			keyGrenade = flag2;
		}
		if (c.spawnFrame > 0f || c.dyingFrame > 0f)
		{
			shootVec = default(Vector2);
		}
		if (Game1.menu.menuLevel[9].alpha > 0f)
		{
			ClearKeys();
		}
		pgs = gs;
	}

	internal void SetKeyPickup()
	{
		keyX = true;
		xFrame = 0.2f;
	}
}
