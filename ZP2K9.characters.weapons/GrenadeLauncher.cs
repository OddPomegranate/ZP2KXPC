namespace ZP2K9.characters.weapons;

public class GrenadeLauncher : Weapon
{
	public GrenadeLauncher()
	{
		damage = 90;
		splash = 120f;
		maxClip = 5;
		idx = 1;
		projType = 5;
		ammoType = 5;
		reloadTime = 0.8f;
		spread = 1f;
		fireRate = 0.3f;
		type = 2;
		burst = 1;
		imgIdx = 6;
		snd = "flare";
	}
}
