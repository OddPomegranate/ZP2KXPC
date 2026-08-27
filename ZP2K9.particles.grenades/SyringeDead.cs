using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZP2K9.particles.grenades;

public class SyringeDead
{
	public static void Init(Particle p, Vector2 loc, Vector2 traj)
	{
		p.loc = loc;
		p.traj = traj + Rand.GetRandomVec2(-50f, 50f, -50f, 50f);
		p.frame = 1f;
		p.dir = Rand.GetRandomFloat(-70f, 70f);
		p.bounce = true;
	}

	public static void Draw(Particle p, SpriteBatch sprite)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc), (Rectangle?)new Rectangle(832, 192, 48, 32), new Color(1f, 1f, 1f, p.frame * 2f), p.angle, new Vector2(24f, 16f), Scroll.zoom * 0.65f, (SpriteEffects)0, 1f);
	}
}
