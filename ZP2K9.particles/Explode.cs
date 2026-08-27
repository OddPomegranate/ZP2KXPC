using Microsoft.Xna.Framework;

namespace ZP2K9.particles;

public class Explode
{
	public Vector2 loc;

	public float splash;

	public int damage;

	public bool exists;

	public void Init(Vector2 loc, float splash, int damage)
	{
		this.loc = loc;
		this.splash = splash;
		this.damage = damage;
		exists = true;
	}
}
