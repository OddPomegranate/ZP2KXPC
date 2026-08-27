namespace ZP2K9.characters.weapons;

public class AK47 : Weapon
{
	public AK47()
	{
		damage = 9;
		maxClip = 50;
		idx = 4;
		projType = 19;
		ammoType = 14;
		reloadTime = 0.8f;
		spread = 50f;
		fireRate = 0.08f;
		type = 1;
		burst = 1;
		imgIdx = 31;
		snd = "mp5";
	}
}
