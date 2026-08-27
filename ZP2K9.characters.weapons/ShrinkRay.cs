namespace ZP2K9.characters.weapons;

public class ShrinkRay : Weapon
{
	public ShrinkRay()
	{
		damage = 150;
		splash = 150f;
		maxClip = 5;
		idx = 1;
		projType = 8;
		ammoType = 8;
		reloadTime = 0.8f;
		spread = 1f;
		fireRate = 0.5f;
		type = 4;
		burst = 1;
		imgIdx = 7;
		snd = "shrink";
	}
}
