namespace ZP2K9.characters.weapons;

public class AFlareGun : Weapon
{
	public AFlareGun()
	{
		isAkimbo = true;
		projType = 6;
		damage = 10;
		maxClip = 8;
		idx = 2;
		ammoType = 6;
		reloadTime = 0.6f;
		fireRate = 0.15f;
		spread = 1f;
		type = 3;
		burst = 1;
		imgIdx = 72;
		snd = "rflare";
	}
}
