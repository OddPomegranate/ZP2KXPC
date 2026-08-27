namespace ZP2K9.characters.weapons;

public class Uzi : Weapon
{
	public Uzi()
	{
		canAkimbo = true;
		projType = 2;
		damage = 7;
		maxClip = 50;
		idx = 3;
		ammoType = 1;
		reloadTime = 0.6f;
		fireRate = 0.03f;
		spread = 170f;
		type = 0;
		burst = 1;
		imgIdx = 28;
		snd = "tec9fire";
	}
}
