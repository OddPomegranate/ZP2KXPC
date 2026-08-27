namespace ZP2K9.characters.weapons;

public class Weapon
{
	public const int AMMO_PISTOLBULLETS = 0;

	public const int AMMO_SMGBULLETS = 1;

	public const int AMMO_SHELLS = 2;

	public const int AMMO_ENERGY = 3;

	public const int AMMO_ROCKETS = 4;

	public const int AMMO_GRENADES = 5;

	public const int AMMO_FLARES = 6;

	public const int AMMO_FREEZE = 7;

	public const int AMMO_SHRINK = 8;

	public const int AMMO_FLAME = 9;

	public const int AMMO_LASER = 10;

	public const int AMMO_BEES = 11;

	public const int AMMO_SYRINGES = 12;

	public const int AMMO_RAINBOWS = 13;

	public const int AMMO_CATS = 14;

	public const int PROJ_BULLET = 0;

	public const int PROJ_SHELL = 1;

	public const int PROJ_MINIBULLET = 2;

	public const int PROJ_PLASMA = 3;

	public const int PROJ_ROCKET = 4;

	public const int PROJ_GRENADE = 5;

	public const int PROJ_FLARE = 6;

	public const int PROJ_FREEZE = 7;

	public const int PROJ_SHRINK = 8;

	public const int PROJ_FLAME = 9;

	public const int PROJ_SWORD = 10;

	public const int PROJ_ELECTRICITY = 11;

	public const int PROJ_LASER = 12;

	public const int PROJ_BEES = 13;

	public const int PROJ_SYRINGE = 14;

	public const int PROJ_RAINBOW = 15;

	public const int PROJ_FLAMESWORD = 16;

	public const int PROJ_FIREBALL = 17;

	public const int PROJ_GOLDENBULLET = 18;

	public const int PROJ_CATS = 19;

	public int type;

	public int idx;

	public int imgIdx;

	public int damage;

	public string snd = "";

	public int ammoType;

	public bool isAkimbo;

	public bool canAkimbo;

	public int projType;

	public float splash;

	public int maxClip;

	public float fireAnimSpeed;

	public float fireRate;

	public float spread;

	public int burst;

	public float reloadTime;

	public bool shells;

	public bool charge;

	public void Reload(Character c, int idx)
	{
		int num = maxClip;
		if (maxClip > 1 && c.perk[2] == 7)
		{
			num *= 3;
		}
		int num2 = num - c.magazine[idx];
		if (num2 > 0)
		{
			if (num2 > c.ammo[ammoType])
			{
				num2 = c.ammo[ammoType];
			}
			c.ammo[ammoType] -= num2;
			c.magazine[idx] += num2;
		}
	}
}
