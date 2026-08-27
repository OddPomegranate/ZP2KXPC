using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ZP2K9.characters;
using ZP2K9.map;

namespace ZP2K9.particles.debris;

public class FlareTrail
{
	public static void Init(Particle p, Vector2 loc, Vector2 traj, float size)
	{
		p.size = size;
		p.frame = Rand.GetRandomFloat(0.5f, 1f);
		p.loc = loc;
		p.traj = traj;
		p.angle = Rand.GetRandomRadian();
		p.dir = Rand.GetRandomFloat(-1f, 1f);
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
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc), (Rectangle?)new Rectangle(64, 0, 64, 64), new Color(new Vector4(1f, p.frame, p.frame, p.frame * 0.4f)), p.angle, new Vector2(32f, 32f), p.size * Scroll.zoom + (1f - p.frame), (SpriteEffects)0, 1f);
	}
}
