using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZP2K9.particles.pyro;

public class ElectricitySplash
{
	public static void Init(Particle p, Vector2 loc, Vector2 traj)
	{
		p.loc = loc;
		p.traj = traj;
		p.frame = Rand.GetRandomFloat(0.1f, 0.3f);
		p.alpha = true;
		p.size = Rand.GetRandomFloat(1f, 1f);
		p.dir = Rand.GetRandomFloat(-10f, 10f);
	}

	public static void Draw(Particle p, SpriteBatch sprite)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc), (Rectangle?)new Rectangle(256 + 64 * Rand.GetRandomInt(0, 3), 0, 64, 64), new Color(new Vector4(1f, 1f, 1f, Rand.GetRandomFloat(0.1f, 1f))), Rand.GetRandomRadian(), new Vector2(32f, 32f), Scroll.zoom * p.size * 0.8f, (SpriteEffects)0, 1f);
		sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc), (Rectangle?)new Rectangle(256 + 64 * Rand.GetRandomInt(0, 3), 0, 64, 64), new Color(new Vector4(Rand.GetRandomFloat(0.9f, 1f), Rand.GetRandomFloat(0.9f, 1f), Rand.GetRandomFloat(0.9f, 1f), Rand.GetRandomFloat(0.1f, 1f))), Rand.GetRandomRadian(), new Vector2(32f, 32f), Scroll.zoom * p.size * 0.8f, (SpriteEffects)0, 1f);
	}
}
