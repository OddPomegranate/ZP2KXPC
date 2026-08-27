namespace ZP2K9.characters.weapons;

public class Shotty : Weapon
{
	public Shotty()
	{
		damage = 8;
		maxClip = 5;
		idx = 0;
		projType = 1;
		ammoType = 2;
		reloadTime = 0.8f;
		shells = true;
		spread = 160f;
		fireRate = 0.2f;
		type = 2;
		burst = 10;
		imgIdx = 2;
		snd = "handcan";
	}
}
