namespace ZP2K9.characters.weapons;

public class AMP5 : Weapon
{
	public AMP5()
	{
		isAkimbo = true;
		projType = 2;
		damage = 5;
		maxClip = 100;
		idx = 1;
		ammoType = 1;
		reloadTime = 0.9f;
		fireRate = 0.015f;
		spread = 130f;
		type = 3;
		burst = 1;
		imgIdx = 67;
		snd = "tec9fire";
	}
}
