using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Yuki_Win;
using ZP2K9.characters;
using ZP2K9.map;

namespace ZP2K9.particles.debris;

public class Spark
{
	public static void Init(Particle p, Vector2 loc, Vector2 traj)
	{
		p.frame = Rand.GetRandomFloat(0.1f, 1f);
		p.loc = loc;
		p.traj = traj;
		p.angle = Trig.GetAngle(default(Vector2), traj);
		p.alpha = true;
		p.bounce = true;
	}

	public static void Update(Particle p, GameMap map, Character[] c, float fTime)
	{
		if (!p.ground)
		{
			p.traj.Y += fTime * 1800f;
			p.angle = Trig.GetAngle(default(Vector2), p.traj);
		}
		p.BaseUpdate(map, c, fTime);
	}

	public static void Draw(Particle p, SpriteBatch sprite)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		float num = p.frame * 4f;
		if (num > 1f)
		{
			num = 1f;
		}
		sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc), (Rectangle?)new Rectangle(650, 576, 160, 64), new Color(1f, 1f, 1f, num), p.angle, new Vector2(80f, 32f), new Vector2(Trig.GetDist(default(Vector2), p.traj) * 0.0004f, 0.2f) * Scroll.zoom, (SpriteEffects)0, 1f);
		if (Rand.CointToss(0.1f))
		{
			Game1.postGlowMgr.Add(Scroll.GetLoc(p.loc), 1f, 0.1f, 1f, p.frame * 0.5f, p.frame * 1.5f, 1f);
		}
	}
}
