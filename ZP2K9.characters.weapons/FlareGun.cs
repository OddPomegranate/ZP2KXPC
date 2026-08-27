namespace ZP2K9.characters.weapons;

public class FlareGun : Weapon
{
	public FlareGun()
	{
		canAkimbo = true;
		projType = 6;
		damage = 10;
		maxClip = 4;
		idx = 2;
		ammoType = 6;
		reloadTime = 0.6f;
		fireRate = 0.2f;
		spread = 1f;
		type = 0;
		burst = 1;
		imgIdx = 8;
		snd = "rflare";
	}
}
