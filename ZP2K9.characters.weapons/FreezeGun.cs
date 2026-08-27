namespace ZP2K9.characters.weapons;

public class FreezeGun : Weapon
{
	public FreezeGun()
	{
		damage = 6;
		maxClip = 24;
		idx = 2;
		projType = 7;
		ammoType = 7;
		reloadTime = 0.8f;
		spread = 50f;
		fireRate = 0.1f;
		type = 1;
		burst = 1;
		imgIdx = 9;
		snd = "ice";
	}
}
