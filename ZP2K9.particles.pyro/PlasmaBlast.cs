using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZP2K9.particles.pyro;

public class PlasmaBlast
{
	public static void Init(Particle p, Vector2 loc, Vector2 traj)
	{
		p.loc = loc;
		p.traj = traj;
		p.frame = Rand.GetRandomFloat(0.3f, 0.8f);
		p.alpha = true;
		p.size = Rand.GetRandomFloat(0.5f, 1f);
		p.dir = Rand.GetRandomFloat(-10f, 10f);
	}

	public static void Draw(Particle p, SpriteBatch sprite)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc), (Rectangle?)new Rectangle(0, 0, 64, 64), new Color(new Vector4(0.3f, 1f, 0.3f, p.frame)), p.angle + p.frame * p.dir, new Vector2(32f, 32f), Scroll.zoom * p.size * 0.8f, (SpriteEffects)0, 1f);
		sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc), (Rectangle?)new Rectangle(0, 0, 64, 64), new Color(new Vector4(0.3f, 0.3f, 1f, p.frame)), p.angle + p.frame * (0f - p.dir), new Vector2(32f, 32f), Scroll.zoom * p.size * 0.8f, (SpriteEffects)0, 1f);
	}
}
