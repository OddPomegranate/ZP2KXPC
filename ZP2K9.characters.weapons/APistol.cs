namespace ZP2K9.characters.weapons;

public class APistol : Weapon
{
	public APistol()
	{
		isAkimbo = true;
		projType = 0;
		damage = 13;
		maxClip = 34;
		idx = 0;
		ammoType = 0;
		reloadTime = 0.9f;
		fireRate = 0.12f;
		spread = 10f;
		type = 3;
		burst = 1;
		imgIdx = 64;
		snd = "pistol";
	}
}
