namespace ZP2K9.characters.weapons;

public class RocketLauncher : Weapon
{
	public RocketLauncher()
	{
		damage = 90;
		splash = 160f;
		maxClip = 4;
		idx = 0;
		projType = 4;
		ammoType = 4;
		reloadTime = 0.8f;
		spread = 1f;
		fireRate = 0.5f;
		type = 4;
		burst = 1;
		imgIdx = 5;
		snd = "launch";
	}
}
