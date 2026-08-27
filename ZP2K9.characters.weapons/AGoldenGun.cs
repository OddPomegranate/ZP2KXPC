namespace ZP2K9.characters.weapons;

public class AGoldenGun : Weapon
{
	public AGoldenGun()
	{
		isAkimbo = true;
		projType = 0;
		damage = 30;
		maxClip = 16;
		idx = 4;
		ammoType = 0;
		reloadTime = 0.9f;
		fireRate = 0.15f;
		spread = 10f;
		type = 3;
		burst = 1;
		imgIdx = 94;
		snd = "deagle";
	}
}
