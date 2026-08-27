using Microsoft.Xna.Framework;
using ZP2K9.platform;
using Microsoft.Xna.Framework.Input;

namespace ZP2K9.hud;

public class InterfaceKeys
{
	public bool keyLeft;

	public bool keyRight;

	public bool keyUp;

	public bool keyDown;

	public bool keyAccept;

	public bool keyCancel;

	public bool keySelect;

	public bool keyStart;

	public bool keyDrawA;

	public bool keyDrawB;

	public bool keyDrawX;

	public bool keyDrawY;

	public Vector2 leftAnalog;

	public Vector2 rightAnalog;

	public bool keyDLeft;

	public bool keyDRight;

	public bool keyDUp;

	public bool keyDDown;

	public bool keyY;

	private GamePadState pgs;

	public float keyLeftTrig;

	public float keyRightTrig;

	public bool keyRightShoulder;

	public bool keyLeftShoulder;

	public void Reset()
	{
		keyLeft = false;
		keyRight = false;
		keyUp = false;
		keyDown = false;
		keyDLeft = false;
		keyDRight = false;
		keyDUp = false;
		keyDDown = false;
		keyAccept = false;
		keyCancel = false;
		keySelect = false;
		keyStart = false;
		keyY = false;
		keyDrawA = false;
		keyDrawB = false;
		keyDrawX = false;
		keyDrawY = false;
		leftAnalog = default(Vector2);
		keyLeftTrig = 0f;
		keyRightTrig = 0f;
		keyLeftShoulder = false;
		keyRightShoulder = false;
	}

	public void Update(GamePadState gs)
	{
		Reset();
		if (!Guide.IsVisible)
		{
			if (gs.ThumbSticks.Left.X < -0.3f && pgs.ThumbSticks.Left.X >= -0.3f)
			{
				keyLeft = true;
			}
			if (gs.ThumbSticks.Left.X > 0.3f && pgs.ThumbSticks.Left.X <= 0.3f)
			{
				keyRight = true;
			}
			if (gs.ThumbSticks.Left.Y < -0.3f && pgs.ThumbSticks.Left.Y >= -0.3f)
			{
				keyDown = true;
			}
			if (gs.ThumbSticks.Left.Y > 0.3f && pgs.ThumbSticks.Left.Y <= 0.3f)
			{
				keyUp = true;
			}
			leftAnalog = gs.ThumbSticks.Left;
			rightAnalog = gs.ThumbSticks.Right;
			if (gs.DPad.Left == ButtonState.Pressed && pgs.DPad.Left == ButtonState.Released)
			{
				keyLeft = true;
				keyDLeft = true;
			}
			if (gs.DPad.Right == ButtonState.Pressed && pgs.DPad.Right == ButtonState.Released)
			{
				keyRight = true;
				keyDRight = true;
			}
			if (gs.DPad.Up == ButtonState.Pressed && pgs.DPad.Up == ButtonState.Released)
			{
				keyUp = true;
				keyDUp = true;
			}
			if (gs.DPad.Down == ButtonState.Pressed && pgs.DPad.Down == ButtonState.Released)
			{
				keyDown = true;
				keyDDown = true;
			}
			if (gs.Buttons.A == ButtonState.Pressed && pgs.Buttons.A == ButtonState.Released)
			{
				keyAccept = true;
			}
			if (gs.Buttons.Y == ButtonState.Pressed && pgs.Buttons.Y == ButtonState.Released)
			{
				keyY = true;
			}
			if (gs.Buttons.Start == ButtonState.Pressed && pgs.Buttons.Start == ButtonState.Released)
			{
				keyStart = true;
			}
			if (gs.Buttons.Back == ButtonState.Pressed && pgs.Buttons.Back == ButtonState.Released)
			{
				keySelect = true;
			}
			if (gs.Buttons.B == ButtonState.Pressed && pgs.Buttons.B == ButtonState.Released)
			{
				keyCancel = true;
			}
			if (gs.Buttons.A == ButtonState.Pressed)
			{
				keyDrawA = true;
			}
			if (gs.Buttons.B == ButtonState.Pressed)
			{
				keyDrawB = true;
			}
			if (gs.Buttons.X == ButtonState.Pressed)
			{
				keyDrawX = true;
			}
			if (gs.Buttons.Y == ButtonState.Pressed)
			{
				keyDrawY = true;
			}
			keyLeftTrig = gs.Triggers.Left;
			keyRightTrig = gs.Triggers.Right;
			if (gs.Buttons.LeftShoulder == ButtonState.Pressed && pgs.Buttons.LeftShoulder == ButtonState.Released)
			{
				keyLeftShoulder = true;
			}
			if (gs.Buttons.RightShoulder == ButtonState.Pressed && pgs.Buttons.RightShoulder == ButtonState.Released)
			{
				keyRightShoulder = true;
			}
			pgs = gs;
		}
	}
}
