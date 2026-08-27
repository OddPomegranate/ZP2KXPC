using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace yMapEdit.map.postglow;

public class PostGlow
{
	public Vector2 loc;

	public float r;

	public float g;

	public float b;

	public float size;

	public Vector2 glareVec;

	public float glareAlpha;

	public void Init(Vector2 loc, float r, float g, float b, float mag, float size)
	{
		this.loc = loc;
		this.r = r * mag;
		this.g = g * mag;
		this.b = b * mag;
		this.size = size;
		glareVec = loc;
		glareAlpha = 0.3f;
	}

	public void Init(Vector2 loc, float r, float g, float b, float mag, float size, float glareAlpha)
	{
		this.loc = loc;
		this.r = r * mag;
		this.g = g * mag;
		this.b = b * mag;
		this.size = size;
		glareVec = loc;
		this.glareAlpha = glareAlpha;
	}

	public void Init(Vector2 loc, float r, float g, float b, float mag, float size, Vector2 glareVec, float glareAlpha)
	{
		this.loc = loc;
		this.r = r * mag;
		this.g = g * mag;
		this.b = b * mag;
		this.size = size;
		this.glareVec = glareVec;
		this.glareAlpha = glareAlpha;
	}

	public void Draw(SpriteBatch sprite, Texture2D spritesTex, float fac)
	{
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		float num = 1f;
		if (loc.X < 0f)
		{
			num *= (100f + loc.X) / 100f;
		}
		if (loc.Y < 0f)
		{
			num *= (100f + loc.Y) / 100f;
		}
		if (loc.X > 1280f)
		{
			num *= (1380f - loc.X) / 100f;
		}
		if (loc.Y > 720f)
		{
			num *= (820f - loc.Y) / 100f;
		}
		if (num > 0f)
		{
			sprite.Draw(spritesTex, loc, (Rectangle?)new Rectangle(0, 832, 192, 192), new Color(r * fac, g * fac, b * fac, num), 0f, new Vector2(96f, 96f), size * ScrollManager.zoom, (SpriteEffects)0, 1f);
			if (glareAlpha > 0f)
			{
				sprite.Draw(spritesTex, glareVec, (Rectangle?)new Rectangle(0, 832, 192, 192), new Color(r * fac, g * fac, b * fac, glareAlpha * num), 0f, new Vector2(96f, 96f), new Vector2(5f, 0.04f), (SpriteEffects)0, 1f);
			}
		}
	}
}
