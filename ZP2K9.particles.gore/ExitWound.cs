using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Yuki_Win;
using ZP2K9.characters;
using ZP2K9.map;

namespace ZP2K9.particles.gore;

public class ExitWound
{
	public static void Init(Particle p, Vector2 loc, Vector2 traj, float size)
	{
		p.loc = loc;
		p.traj = traj;
		p.frame = 0.2f;
		p.size = size;
	}

	public static void Update(Particle p, GameMap map, Character[] c, float fTime)
	{
		p.traj.Y += fTime * Game1.gravity * 0.3f;
		p.BaseUpdate(map, c, fTime);
	}

	public static void Draw(Particle p, SpriteBatch sprite)
	{
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		p.angle = Trig.GetAngle(default(Vector2), p.traj);
		int num = (int)((1f - p.frame / 0.2f) * 9f);
		sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc), (Rectangle?)new Rectangle(num * 64, 96, 64, 64), new Color(new Vector4(0.5f, 0f, 0f, 0.5f)), p.angle, new Vector2(32f, 32f), Scroll.zoom * new Vector2(p.size * 2f, p.size * 0.4f), (SpriteEffects)0, 1f);
	}
}
