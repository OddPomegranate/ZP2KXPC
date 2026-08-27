namespace ZP2K9.characters.weapons;

public class FlameSwordWeapon : Weapon
{
	public FlameSwordWeapon()
	{
		projType = 16;
		damage = 60;
		maxClip = 1;
		idx = 3;
		ammoType = 0;
		reloadTime = 0.5f;
		fireRate = 1f;
		spread = 1f;
		type = 5;
		burst = 1;
		imgIdx = 29;
		snd = "fsword";
	}
}
