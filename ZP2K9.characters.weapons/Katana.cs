namespace ZP2K9.characters.weapons;

public class Katana : Weapon
{
	public Katana()
	{
		projType = 10;
		damage = 70;
		maxClip = 1;
		idx = 1;
		ammoType = 0;
		reloadTime = 0.5f;
		fireRate = 1f;
		spread = 1f;
		type = 5;
		burst = 1;
		imgIdx = 12;
		snd = "sword";
	}
}
