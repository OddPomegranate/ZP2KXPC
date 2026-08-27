using System.Collections.Generic;

namespace ZP2K9.characters.weapons;

public class WeaponCatalog
{
	public const int AKIMBO_ADD = 100;

	public static Dictionary<int, Weapon> weapons;

	public static void Initialize()
	{
		weapons = new Dictionary<int, Weapon>();
		weapons.Add(125, new AFlareGun());
		weapons.Add(147, new AGoldenGun());
		weapons.Add(120, new AMP5());
		weapons.Add(117, new APistol());
		weapons.Add(145, new AUzi());
		weapons.Add(48, new AK47());
		weapons.Add(31, new BeeGun());
		weapons.Add(49, new FireBall());
		weapons.Add(46, new FlameSwordWeapon());
		weapons.Add(27, new FlameThrower());
		weapons.Add(25, new FlareGun());
		weapons.Add(26, new FreezeGun());
		weapons.Add(47, new GoldenGun());
		weapons.Add(23, new GrenadeLauncher());
		weapons.Add(29, new Katana());
		weapons.Add(30, new LaserCannon());
		weapons.Add(40, new LightSword());
		weapons.Add(32, new MassInfector());
		weapons.Add(20, new MP5());
		weapons.Add(17, new Pistol());
		weapons.Add(21, new PlasmaRifle());
		weapons.Add(39, new Rainbow2X());
		weapons.Add(22, new RocketLauncher());
		weapons.Add(19, new Shotty());
		weapons.Add(24, new ShrinkRay());
		weapons.Add(18, new SMG());
		weapons.Add(28, new SwordWeap());
		weapons.Add(45, new Uzi());
	}
}
