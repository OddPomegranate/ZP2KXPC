namespace ZP2K9.characters.weapons;

public class GoldenGun : Weapon
{
	public GoldenGun()
	{
		canAkimbo = true;
		projType = 0;
		damage = 30;
		maxClip = 8;
		idx = 4;
		ammoType = 0;
		reloadTime = 0.6f;
		fireRate = 0.3f;
		spread = 1f;
		type = 0;
		burst = 1;
		imgIdx = 30;
		snd = "deagle";
	}
}
