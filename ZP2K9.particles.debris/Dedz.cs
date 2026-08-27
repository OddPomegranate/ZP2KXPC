using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ZP2K9.characters;
using ZP2K9.map;

namespace ZP2K9.particles.debris;

public class Dedz
{
	public static void Init(Particle p, Vector2 loc)
	{
		p.loc = loc;
		p.size = 1f;
		p.frame = 2f;
		p.alpha = true;
	}

	public static void Draw(Particle p, SpriteBatch sprite)
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		float num = (((int)(p.frame * 10f) % 2 == 0) ? 1f : 0.5f);
		sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc), (Rectangle?)new Rectangle(512, 576, 128, 64), new Color(num, num, num, 1f), 0f, new Vector2(64f, 32f), Scroll.zoom * 0.8f, (SpriteEffects)0, 1f);
	}

	internal static void Update(Particle p, GameMap map, Character[] c, float fTime)
	{
		p.frame -= fTime;
	}
}
