namespace ZP2K9.characters.weapons;

public class Pistol : Weapon
{
	public Pistol()
	{
		canAkimbo = true;
		projType = 0;
		damage = 13;
		maxClip = 17;
		idx = 0;
		ammoType = 0;
		reloadTime = 0.6f;
		fireRate = 0.2f;
		spread = 1f;
		type = 0;
		burst = 1;
		imgIdx = 0;
		snd = "pistol";
	}
}
