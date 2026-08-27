using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZP2K9.hud.hudBlood;

internal class HUDBloodDrop
{
	private Vector2 loc;

	private float angle;

	private float alpha;

	private float size;

	private int idx;

	public HUDBloodDrop(Vector2 loc, float size, float angle, float alpha, int idx)
	{
		this.loc = loc;
		this.size = size;
		this.alpha = alpha;
		this.angle = angle;
		this.idx = idx;
	}

	public void Draw(SpriteBatch sprite, float frame, float uberAlpha)
	{
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		float num = (float)Math.Cos(frame) - 1f;
		float num2 = uberAlpha * (alpha + num * 0.05f);
		if (num2 > 0f)
		{
			sprite.Draw(Game1.spritesTex, loc, (Rectangle?)new Rectangle(idx * 64, 96, 64, 64), new Color(new Vector4(0.3f, 0f, 0f, num2)), angle, new Vector2(32f, 32f), size, (SpriteEffects)0, 1f);
		}
	}
}
