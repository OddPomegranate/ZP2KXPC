using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ZP2K9.characters;
using ZP2K9.map;

namespace ZP2K9.particles.debris;

public class Nukesplode
{
	public static void Init(Particle p, Vector2 loc, float size)
	{
		p.loc = loc;
		p.size = size;
		p.frame = Rand.GetRandomFloat(4f, 5f);
		p.alpha = false;
		p.angle = Rand.GetRandomRadian();
	}

	public static void Init(Particle p, Vector2 loc, Vector2 traj, float size)
	{
		p.loc = loc;
		p.size = size;
		p.traj = traj;
		p.frame = Rand.GetRandomFloat(2f, 3f);
		p.alpha = false;
		p.angle = Rand.GetRandomRadian();
	}

	public static void Draw(Particle p, SpriteBatch sprite)
	{
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		int num = (int)((3f - p.frame) / 3f * 9f);
		float num2 = (3f - p.frame) / 3f * 9f;
		num2 -= (float)(int)num2;
		float num3 = p.size * 100f;
		float num4 = 1f;
		if (p.frame < 0.1f)
		{
			num4 = p.frame * 10f;
		}
		float num5 = p.size * 10f;
		num5 -= (float)(int)p.size;
		sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc), (Rectangle?)new Rectangle(num * 64, 160, 64, 64), new Color(new Vector4(num5, num5, num5, num4 * (1f - num2))), p.angle + p.frame * (num3 - (float)(int)num3 - 0.5f) * 1f, new Vector2(32f, 32f), p.size * Scroll.zoom, (SpriteEffects)0, 1f);
		sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc), (Rectangle?)new Rectangle((num + 1) * 64, 160, 64, 64), new Color(new Vector4(p.frame, p.frame, p.frame, num4 * num2)), p.angle + p.frame * (num3 - (float)(int)num3 - 0.5f) * 1f, new Vector2(32f, 32f), p.size * Scroll.zoom, (SpriteEffects)0, 1f);
		num = (int)((3f - p.frame) * 4f);
		num2 = (3f - p.frame) * 4f;
		num2 -= (float)(int)num2;
		num %= 9;
		float num6 = p.frame * 10f;
		if (num6 > 0.95f)
		{
			num6 = 0.95f;
		}
		sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc + new Vector2((3f - p.frame) * (0f - p.traj.X) * 1.3f, 0f)), (Rectangle?)new Rectangle(num * 32, 224, 32, 64), new Color(new Vector4(p.frame, p.frame, p.frame, num6 * (1f - num2))), 3.14f, new Vector2(16f, 52f), p.size * Scroll.zoom * new Vector2(0.5f, 0.4f * (3f - p.frame)), (SpriteEffects)0, 1f);
		sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc + new Vector2((3f - p.frame) * (0f - p.traj.X) * 1.3f, 0f)), (Rectangle?)new Rectangle((num + 1) % 9 * 32, 224, 32, 64), new Color(new Vector4(p.frame, p.frame, p.frame, num6 * num2)), 3.14f, new Vector2(16f, 52f), p.size * Scroll.zoom * new Vector2(0.5f, 0.4f * (3f - p.frame)), (SpriteEffects)0, 1f);
		float num7 = p.frame - 2.5f;
		if (num7 < 0f)
		{
			num7 = 0f;
		}
		num7 *= 30f;
		float num8 = (p.frame - 1f) * 2f;
		if (num8 > 0.3f)
		{
			num8 = 0.3f;
		}
		if (num8 > 0f)
		{
			Game1.postGlowMgr.Add(Scroll.GetLoc(p.loc), 1f, 0.5f, 0.2f, num8, 3f + num7);
		}
		if (p.frame > 1f && Quake.quakeVal < 0.6f)
		{
			Quake.quakeVal += 0.01f;
		}
	}

	internal static void Update(Particle p, GameMap map, Character[] c, float fTime)
	{
		if (p.frame < 1f)
		{
			fTime *= 0.5f;
		}
		if (p.frame < 0.5f)
		{
			fTime *= 0.5f;
		}
		if (p.frame < 0.25f)
		{
			fTime *= 0.5f;
		}
		if (map.GetIsCol(p.loc))
		{
			p.traj = default(Vector2);
		}
		else
		{
			p.loc += fTime * p.traj;
		}
		p.frame -= fTime;
	}
}
