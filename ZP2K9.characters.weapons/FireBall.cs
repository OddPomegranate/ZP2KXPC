namespace ZP2K9.characters.weapons;

public class FireBall : Weapon
{
	public FireBall()
	{
		damage = 10;
		splash = 160f;
		maxClip = 3;
		idx = 4;
		projType = 17;
		ammoType = 4;
		reloadTime = 0.8f;
		spread = 1f;
		fireRate = 0.5f;
		type = 4;
		burst = 1;
		imgIdx = 32;
		snd = "flaunch";
	}
}
