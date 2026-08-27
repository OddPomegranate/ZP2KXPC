using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZP2K9.particles.shot;

public class BeeBit
{
	public static void Init(Particle p, Vector2 loc, Vector2 traj, float size)
	{
		p.loc = loc;
		p.traj = traj;
		p.size = size;
		p.angle = Rand.GetRandomFloat(0f, 6f);
		p.alpha = false;
		p.frame = 0.05f;
	}

	public static void Draw(Particle p, SpriteBatch sprite)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc), (Rectangle?)new Rectangle(832 + (int)(p.frame * 30f) % 2 * 16, 224, 16, 16), new Color(1f, 1f, 1f, 1f), 0f, new Vector2(8f, 8f), Scroll.zoom * 0.8f, (!(p.traj.X < 0f) ? SpriteEffects.FlipHorizontally : SpriteEffects.None), 1f);
	}
}
