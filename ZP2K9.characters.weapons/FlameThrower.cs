namespace ZP2K9.characters.weapons;

public class FlameThrower : Weapon
{
	public FlameThrower()
	{
		damage = 5;
		maxClip = 50;
		idx = 2;
		projType = 9;
		ammoType = 9;
		reloadTime = 0.8f;
		spread = 1f;
		fireRate = 0.04f;
		type = 2;
		burst = 1;
		imgIdx = 10;
		snd = "flame";
	}
}
