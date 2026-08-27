namespace ZP2K9.characters.weapons;

public class SwordWeap : Weapon
{
	public SwordWeap()
	{
		projType = 10;
		damage = 80;
		maxClip = 1;
		idx = 0;
		ammoType = 0;
		reloadTime = 0.6f;
		fireRate = 1f;
		spread = 1f;
		type = 5;
		burst = 1;
		imgIdx = 11;
		snd = "sword";
	}
}
