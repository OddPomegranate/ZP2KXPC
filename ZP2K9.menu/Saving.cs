using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using yMapEdit.map;

namespace ZP2K9.menu;

public class Saving
{
	public float frame;

	public bool Active()
	{
		return frame > 0f;
	}

	public void Update()
	{
		frame -= Game1.frameTime;
	}

	public void Set()
	{
		frame = 2f;
	}

	public void Draw(SpriteBatch sprite)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		sprite.Begin(blendState: BlendState.AlphaBlend);
		float num = 100f;
		Vector2 vector = new Vector2(ScrollManager.screenSize.X - num, num);
		sprite.Draw(Game1.spritesTex, vector, (Rectangle?)new Rectangle(928, 0, 64, 64), new Color(1f, 1f, 1f, frame), (float)Math.Cos(frame * 5f) * 0.1f, new Vector2(32f, 32f), 1f + (float)Math.Cos(frame * 3f) * 0.1f, (SpriteEffects)0, 1f);
		sprite.End();
	}
}
