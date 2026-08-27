namespace ZP2K9.characters.weapons;

public class PlasmaRifle : Weapon
{
	public PlasmaRifle()
	{
		damage = 10;
		maxClip = 24;
		idx = 1;
		projType = 3;
		ammoType = 3;
		reloadTime = 0.8f;
		spread = 50f;
		fireRate = 0.1f;
		type = 1;
		burst = 1;
		imgIdx = 4;
		snd = "plasma";
	}
}
