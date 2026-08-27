using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZP2K9.particles.debris;

public class ShrinkZap
{
	public static void Init(Particle p, Vector2 loc, Vector2 traj, float size)
	{
		p.size = size;
		p.frame = Rand.GetRandomFloat(0.25f, 0.5f);
		p.loc = loc;
		p.traj = traj;
		p.flags = Rand.GetRandomInt(0, 3);
		p.angle = Rand.GetRandomRadian();
		p.dir = Rand.GetRandomFloat(-1f, 1f);
		p.alpha = true;
	}

	public static void Draw(Particle p, SpriteBatch sprite)
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc), (Rectangle?)new Rectangle(256 + p.flags * 64, 0, 64, 64), new Color(new Vector4(0.1f, 0.8f, 1f, 1f) * p.frame * 3f), p.angle, new Vector2(32f, 32f), p.size * Scroll.zoom + (1f - p.frame), (SpriteEffects)0, 1f);
	}
}
