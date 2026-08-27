namespace ZP2K9.characters.weapons;

public class Rainbow2X : Weapon
{
	public Rainbow2X()
	{
		damage = 150;
		splash = 150f;
		maxClip = 3;
		idx = 3;
		projType = 15;
		ammoType = 13;
		reloadTime = 0.8f;
		spread = 1f;
		fireRate = 0.5f;
		type = 4;
		burst = 1;
		imgIdx = 22;
		snd = "rainbow";
	}
}
