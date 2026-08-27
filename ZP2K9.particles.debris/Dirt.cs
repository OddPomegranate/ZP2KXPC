using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZP2K9.particles.debris;

public class Dirt
{
	public static void Init(Particle p, Vector2 loc, Vector2 traj, float size)
	{
		p.loc = loc;
		p.traj = traj;
		p.size = size;
		p.frame = 1f;
	}

	public static void Draw(Particle p, SpriteBatch sprite)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc), (Rectangle?)new Rectangle(64, 0, 64, 64), new Color(new Vector4(0.5f, 0.4f, 0.3f, p.frame)), p.frame, new Vector2(32f, 32f), p.size * Scroll.zoom, (SpriteEffects)0, 1f);
	}
}
