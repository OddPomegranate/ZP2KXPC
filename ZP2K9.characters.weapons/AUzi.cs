namespace ZP2K9.characters.weapons;

public class AUzi : Weapon
{
	public AUzi()
	{
		isAkimbo = true;
		projType = 2;
		damage = 7;
		maxClip = 100;
		idx = 3;
		ammoType = 1;
		reloadTime = 0.9f;
		fireRate = 0.015f;
		spread = 170f;
		type = 3;
		burst = 1;
		imgIdx = 92;
		snd = "tec9fire";
	}
}
