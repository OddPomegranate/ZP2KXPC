namespace ZP2K9.characters.weapons;

public class BeeGun : Weapon
{
	public BeeGun()
	{
		damage = 5;
		maxClip = 24;
		idx = 3;
		projType = 13;
		ammoType = 11;
		reloadTime = 0.8f;
		spread = 250f;
		fireRate = 0.1f;
		type = 1;
		burst = 3;
		imgIdx = 14;
		snd = "bee";
	}
}
