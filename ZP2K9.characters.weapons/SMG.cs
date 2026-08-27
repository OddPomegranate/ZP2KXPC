namespace ZP2K9.characters.weapons;

public class SMG : Weapon
{
	public SMG()
	{
		damage = 9;
		maxClip = 50;
		idx = 0;
		projType = 0;
		ammoType = 0;
		reloadTime = 0.8f;
		spread = 50f;
		fireRate = 0.08f;
		type = 1;
		burst = 1;
		imgIdx = 1;
		snd = "mp5";
	}
}
