using Microsoft.Xna.Framework;
using ZP2K9.characters;
using ZP2K9.map;

namespace ZP2K9.particles.gas;

public class PoisonSplash
{
	public static void Init(Particle p, Vector2 loc, int owner, int damage, float range)
	{
		p.loc = loc;
		p.frame = 1f;
		p.flags = damage;
		p.size = range;
		p.netOwner = owner;
	}

	public static void Update(Particle p, GameMap map, Character[] c, float fTime)
	{
		HitManager.CheckHit(c, p, map, p.netOwner);
		p.frame = -1f;
	}
}
