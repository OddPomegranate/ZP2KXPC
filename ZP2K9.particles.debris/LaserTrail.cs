using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Yuki_Win;
using ZP2K9.characters;
using ZP2K9.map;

namespace ZP2K9.particles.debris;

public class LaserTrail
{
	public static void Init(Particle p, Vector2 loc, Vector2 traj, float size)
	{
		p.size = size;
		p.frame = 1f;
		p.loc = loc;
		p.traj = traj;
		p.angle = Trig.GetAngle(default(Vector2), traj);
		p.dir = Rand.GetRandomFloat(-1f, 1f);
		p.alpha = true;
	}

	public static void Update(Particle p, GameMap map, Character[] c, float fTime)
	{
		p.angle += p.dir * fTime;
		p.traj.X += fTime * Rand.GetRandomFloat(10f, 40f);
		p.traj.Y -= fTime * Rand.GetRandomFloat(10f, 40f);
		p.BaseUpdate(map, c, fTime);
	}

	public static void Draw(Particle p, SpriteBatch sprite)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < 3; i++)
		{
			sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc), (Rectangle?)new Rectangle(256 + i * 64, 0, 64, 64), new Color(new Vector4(0.1f, 0.5f, 1f, 1f) * p.frame), p.angle, new Vector2(32f, 32f), p.size * Scroll.zoom * new Vector2(2f, 1.01f - p.frame) * 5f, (SpriteEffects)0, 1f);
		}
		sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc), (Rectangle?)new Rectangle(64, 0, 64, 64), new Color(new Vector4(0.1f, 0.5f, 1f, 1f) * p.frame * 0.5f), p.angle, new Vector2(32f, 32f), p.size * Scroll.zoom * new Vector2(2f, 2f - p.frame) * 5f, (SpriteEffects)0, 1f);
	}
}
