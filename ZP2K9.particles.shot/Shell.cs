using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZP2K9.particles.shot;

public class Shell
{
	public static void Init(Particle p, Vector2 loc, Vector2 traj)
	{
		p.loc = loc;
		p.traj = traj + Rand.GetRandomVec2(-50f, 50f, -50f, 50f);
		p.frame = 5f;
		p.dir = Rand.GetRandomFloat(-10f, 10f);
		p.bounce = true;
	}

	public static void Draw(Particle p, SpriteBatch sprite)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc), (Rectangle?)new Rectangle(16, 64, 16, 16), new Color(new Vector4(1f, 1f, 1f, p.frame)), p.angle, new Vector2(8f, 8f), Scroll.zoom * 0.4f, (SpriteEffects)0, 1f);
	}
}
