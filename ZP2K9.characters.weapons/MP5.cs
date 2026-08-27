namespace ZP2K9.characters.weapons;

public class MP5 : Weapon
{
	public MP5()
	{
		canAkimbo = true;
		projType = 2;
		damage = 5;
		maxClip = 50;
		idx = 1;
		ammoType = 1;
		reloadTime = 0.6f;
		fireRate = 0.03f;
		spread = 130f;
		type = 0;
		burst = 1;
		imgIdx = 3;
		snd = "tec9fire";
	}
}
