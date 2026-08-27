using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ZP2K9.characters;
using ZP2K9.map;

namespace ZP2K9.particles.debris;

public class Bubble
{
	public static void Init(Particle p, Vector2 loc, Vector2 traj, float size)
	{
		p.loc = loc;
		p.traj = traj;
		p.size = size;
		p.frame = 1f;
		p.alpha = true;
	}

	public static void Update(Particle p, GameMap map, Character[] c, float fTime)
	{
		Vector2 loc = p.loc;
		int num = (int)(loc.X / 64f);
		int num2 = (int)(loc.Y / 32f);
		if (num > 0 && num > 0 && num2 < 256 && num2 < 256 && !map.water.water[num, num2])
		{
			p.frame -= fTime * 30f;
		}
		p.BaseUpdate(map, c, fTime);
	}

	public static void Draw(Particle p, SpriteBatch sprite)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc), (Rectangle?)new Rectangle(384, 672, 64, 64), new Color(new Vector4(1f, 1f, 1f, p.frame)), p.frame, new Vector2(32f, 32f), p.size * Scroll.zoom, (SpriteEffects)0, 1f);
	}
}
