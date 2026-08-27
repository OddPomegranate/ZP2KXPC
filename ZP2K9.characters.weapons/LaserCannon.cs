namespace ZP2K9.characters.weapons;

public class LaserCannon : Weapon
{
	public LaserCannon()
	{
		damage = 200;
		splash = 160f;
		maxClip = 3;
		idx = 2;
		projType = 12;
		ammoType = 10;
		reloadTime = 0.8f;
		spread = 1f;
		fireRate = 0.5f;
		type = 4;
		burst = 1;
		imgIdx = 13;
		charge = true;
		snd = "zexplode";
	}
}
