using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ZP2K9;

namespace yMapEdit.map.postglow;

public class PostGlowManager
{
	public PostGlow[] postGlow;

	public int totalGlows;

	private float total;

	public PostGlowManager()
	{
		postGlow = new PostGlow[256];
		for (int i = 0; i < postGlow.Length; i++)
		{
			postGlow[i] = new PostGlow();
		}
	}

	public void Add(Vector2 loc, float r, float g, float b, float mag, float size)
	{
		if (totalGlows < postGlow.Length)
		{
			postGlow[totalGlows].Init(loc, r, g, b, mag, size);
			totalGlows++;
		}
	}

	public void Add(Vector2 loc, float r, float g, float b, float mag, float size, float glareAlpha)
	{
		if (totalGlows < postGlow.Length)
		{
			postGlow[totalGlows].Init(loc, r, g, b, mag, size, glareAlpha);
			totalGlows++;
		}
	}

	public void Add(Vector2 loc, float r, float g, float b, float mag, float size, Vector2 glareVec, float glareAlpha)
	{
		if (totalGlows < postGlow.Length)
		{
			postGlow[totalGlows].Init(loc, r, g, b, mag, size, glareVec, glareAlpha);
			totalGlows++;
		}
	}

	public void Update()
	{
		float num = 0f;
		for (int i = 0; i < totalGlows; i++)
		{
			float num2 = 1f;
			if (postGlow[i].loc.X < 0f)
			{
				num2 *= (100f + postGlow[i].loc.X) / 100f;
			}
			if (postGlow[i].loc.Y < 0f)
			{
				num2 *= (100f + postGlow[i].loc.Y) / 100f;
			}
			if (postGlow[i].loc.X > 1280f)
			{
				num2 *= (1380f - postGlow[i].loc.X) / 100f;
			}
			if (postGlow[i].loc.Y > 720f)
			{
				num2 *= (820f - postGlow[i].loc.Y) / 100f;
			}
			if (num2 > 1f)
			{
				num2 = 1f;
			}
			if (num2 > 0f)
			{
				float num3 = postGlow[i].r + postGlow[i].g + postGlow[i].b;
				num3 *= num2;
				if (num3 > 1f)
				{
					num3 = 1f;
				}
				num += num3;
			}
		}
		float num4 = 1f;
		if (num > 10f)
		{
			num -= 10f;
			num /= 15f;
			if (num > 0.8f)
			{
				num = 0.8f;
			}
			num4 -= num;
		}
		if (total < num4)
		{
			total += Game1.frameTime * 0.2f;
			if (total > num4)
			{
				total = num4;
			}
		}
		if (total > num4)
		{
			total -= Game1.frameTime;
			if (total < num4)
			{
				total = num4;
			}
		}
	}

	public void Draw(SpriteBatch sprite, Texture2D spritesTex)
	{
		for (int i = 0; i < totalGlows; i++)
		{
			postGlow[i].Draw(sprite, spritesTex, total);
		}
	}

	public void Reset()
	{
		totalGlows = 0;
	}
}
