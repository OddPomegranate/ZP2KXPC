using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ZP2K9.characters;
using ZP2K9.map;

namespace ZP2K9.particles.debris;

public class Explosion
{
	public static void Init(Particle p, Vector2 loc, float size)
	{
		p.loc = loc;
		p.size = size;
		p.frame = 0.4f;
		p.alpha = true;
		p.angle = Rand.GetRandomRadian();
	}

	public static void Init(Particle p, Vector2 loc, Vector2 traj, float size)
	{
		p.loc = loc;
		p.size = size;
		p.traj = traj;
		p.frame = 0.4f;
		p.alpha = true;
		p.angle = Rand.GetRandomRadian();
	}

	public static void Draw(Particle p, SpriteBatch sprite)
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		int num = (int)((0.4f - p.frame) / 0.4f * 9f);
		sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc), (Rectangle?)new Rectangle(num * 64, 160, 64, 64), new Color(new Vector4(1f, 1f, 1f, 1f)), p.angle, new Vector2(32f, 32f), p.size * Scroll.zoom, (SpriteEffects)0, 1f);
		if (p.size >= 4f)
		{
			float num2 = p.frame * 2f;
			if (num2 > 0.3f)
			{
				num2 = 0.3f;
			}
			Game1.postGlowMgr.Add(Scroll.GetLoc(p.loc), 1f, 0.5f, 0.2f, num2, 3f);
		}
	}

	internal static void Update(Particle p, GameMap map, Character[] c, float fTime)
	{
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
