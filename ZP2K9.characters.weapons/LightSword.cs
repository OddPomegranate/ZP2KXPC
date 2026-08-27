namespace ZP2K9.characters.weapons;

public class LightSword : Weapon
{
	public LightSword()
	{
		projType = 10;
		damage = 70;
		maxClip = 1;
		idx = 2;
		ammoType = 0;
		reloadTime = 0.5f;
		fireRate = 1f;
		spread = 1f;
		type = 5;
		burst = 1;
		imgIdx = 23;
		snd = "saber";
	}
}
