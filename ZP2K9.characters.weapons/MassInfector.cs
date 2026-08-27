namespace ZP2K9.characters.weapons;

public class MassInfector : Weapon
{
	public MassInfector()
	{
		damage = 8;
		maxClip = 8;
		idx = 3;
		projType = 14;
		ammoType = 12;
		reloadTime = 0.5f;
		shells = true;
		spread = 1f;
		fireRate = 0.1f;
		type = 2;
		burst = 1;
		imgIdx = 15;
		snd = "infector";
	}
}
