using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Yuki_Win;

namespace ZP2K9.particles.debris;

public class RainbowDust
{
	public static void Init(Particle p, Vector2 loc, Vector2 traj, float size)
	{
		p.size = size;
		p.frame = Rand.GetRandomFloat(0.5f, 1f);
		p.loc = loc;
		p.traj = traj;
		p.angle = Rand.GetRandomRadian();
		p.dir = Rand.GetRandomFloat(-10f, 10f);
		p.flags = Rand.GetRandomInt(0, 3);
		p.alpha = true;
	}

	public static void Draw(Particle p, SpriteBatch sprite)
	{
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		if (p.flags == 2)
		{
			sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc), (Rectangle?)new Rectangle(832, 256, 80, 32), new Color(1f, p.frame, p.frame, p.frame * 2f), Trig.GetAngle(default(Vector2), p.traj), new Vector2(40f, 16f), p.size * Scroll.zoom + (1f - p.frame), (SpriteEffects)0, 1f);
		}
		else
		{
			sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc), (Rectangle?)new Rectangle(912 + p.flags * 32, 256, 32, 32), new Color(1f, p.frame, p.frame, p.frame * 2f), p.angle, new Vector2(16f, 16f), p.size * Scroll.zoom + (1f - p.frame) * 3f, (SpriteEffects)0, 1f);
		}
	}
}
