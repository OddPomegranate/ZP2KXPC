using System.Collections.Generic;

namespace yMapEdit.map;

public class Special
{
	public const byte SPECIAL_SPAWN = 1;

	public const byte SPECIAL_SPAWN_BLUE = 2;

	public const byte SPECIAL_SPAWN_RED = 3;

	public const byte SPECIAL_FLAG_BLUE = 4;

	public const byte SPECIAL_FLAG_RED = 5;

	public const byte SPECIAL_HILL = 6;

	public const byte WEAPON_PISTOL = 17;

	public const byte WEAPON_SMG = 18;

	public const byte WEAPON_SHOTTY = 19;

	public const byte WEAPON_MP5K = 20;

	public const byte WEAPON_PLASMARIFLE = 21;

	public const byte WEAPON_ROCKETLAUNCHER = 22;

	public const byte WEAPON_GRENADELAUNCHER = 23;

	public const byte WEAPON_SHRINKRAY = 24;

	public const byte WEAPON_FLAREGUN = 25;

	public const byte WEAPON_FREEZEGUN = 26;

	public const byte WEAPON_FLAMETHROWER = 27;

	public const byte WEAPON_SWORD = 28;

	public const byte WEAPON_KATANA = 29;

	public const byte WEAPON_LASERCANNON = 30;

	public const byte WEAPON_BEEGUN = 31;

	public const byte WEAPON_MASS_INFECTOR = 32;

	public const byte GRENADE_NORMAL = 33;

	public const byte GRENADE_FIRE = 34;

	public const byte GRENADE_GAS = 35;

	public const byte GRENADE_MIRV = 36;

	public const byte GRENADE_MINE = 37;

	public const byte GRENADE_FREEZE = 38;

	public const byte WEAPON_RAINBOW2X = 39;

	public const byte WEAPON_LIGHTSWORD = 40;

	public const byte GRENADE_TIME = 41;

	public const byte GRENADE_ZAPPER = 42;

	public const byte GRENADE_NUKE = 43;

	public const byte GRENADE_VAMPIRE = 44;

	public const byte WEAPON_UZI = 45;

	public const byte WEAPON_FLAMESWORD = 46;

	public const byte WEAPON_GOLDENGUN = 47;

	public const byte WEAPON_AK47 = 48;

	public const byte WEAPON_FIREBALL = 49;

	public byte type;

	public int x;

	public int y;

	public bool exists;

	public static Dictionary<int, string> names;

	public static void InitNames()
	{
		names = new Dictionary<int, string>();
		names.Add(17, "P256 Pistol");
		names.Add(18, "M-Zero Assault Rifle");
		names.Add(19, "Model 187 Pump Action Shotgun");
		names.Add(20, "MPK Submachinegun");
		names.Add(21, "ZX Plasma Rifle");
		names.Add(22, "DRGN-2 Rocket Launcher");
		names.Add(23, "MGL-R Multishot Grenade Launcher");
		names.Add(24, "DN3-D Shrink Ray");
		names.Add(25, "Flare Gun");
		names.Add(26, "ICE-X Integrated Coolant Effect Weapon");
		names.Add(27, "TZTR Flamethrower");
		names.Add(28, "Cloud Sword");
		names.Add(29, "Masamune");
		names.Add(30, "LARR-3 Light Accelerated Radiated Railgun");
		names.Add(31, "BZBZ Hive Weapon");
		names.Add(32, "Mass Infector");
		names.Add(39, "ATW-ATS Rainbow 2X");
		names.Add(40, "Light Sword");
		names.Add(33, "Frag Grenade");
		names.Add(34, "Napalm Grenade");
		names.Add(35, "Gas Grenade");
		names.Add(36, "MIRV Grenade");
		names.Add(37, "Proxy Mine");
		names.Add(38, "Freeze Grenade");
		names.Add(41, "Time Grenade");
		names.Add(42, "Lightning Grenade");
		names.Add(43, "Nuke Grenade");
		names.Add(44, "Blood Grenade");
		names.Add(45, "Uzi 9mm");
		names.Add(46, "Blade of Tyr' Az-Gul'r");
		names.Add(47, "Golden .45");
		names.Add(48, "AK-47 That Fires Cats");
		names.Add(49, "BLZ-R \"The Fireball\"");
	}
}
