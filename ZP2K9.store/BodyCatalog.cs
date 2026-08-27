using ZP2K9.store.body;

namespace ZP2K9.store;

public class BodyCatalog
{
	public const int SKIN_WHITE_BOY = 0;

	public const int CLOTHES_BOY_SHIRT = 1;

	public const int SKIN_WHITE_GIRL = 2;

	public const int SKIN_ROBOT = 3;

	public const int CLOTHES_GIRL_SHIRT = 4;

	public const int CLOTHES_BOY_NINJA = 5;

	public const int CLOTHES_BOY_PIRATE = 6;

	public const int CLOTHES_BOY_KNIGHT = 7;

	public const int CLOTHES_BOY_VIKING = 8;

	public const int CLOTHES_BOY_SAMURAI = 9;

	public const int CLOTHES_GIRL_NINJA = 10;

	public const int CLOTHES_GIRL_PIRATE = 11;

	public const int CLOTHES_BOY_SWAT = 12;

	public const int CLOTHES_BOY_CHIEF = 13;

	public const int CLOTHES_BOY_GEAR = 14;

	public const int CLOTHES_GIRL_SWAT = 15;

	public const int CLOTHES_GIRL_HOODIE = 16;

	public const int CLOTHES_GIRL_DRESS = 17;

	public const int CLOTHES_BOY_SUIT = 18;

	public const int SKIN_DARK_BOY = 19;

	public const int SKIN_BLACK_BOY = 20;

	public const int SKIN_DARK_GIRL = 21;

	public const int SKIN_BLACK_GIRL = 22;

	public const int CLOTHES_BOY_SAM_ARMOR = 23;

	public const int CLOTHES_BOY_CHEF = 24;

	public const int CLOTHES_BOY_BEACH = 25;

	public const int CLOTHES_BOY_SUBZERO = 26;

	public const int CLOTHES_BOY_GORILLA = 27;

	public const int CLOTHES_GIRL_CHIEF = 28;

	public const int CLOTHES_GIRL_GEAR = 29;

	public const int CLOTHES_GIRL_RAIDER = 30;

	public const int CLOTHES_GIRL_STARS = 31;

	public const int CLOTHES_GIRL_SCHOOL = 32;

	public const int CLOTHES_GIRL_CORTESAN = 33;

	public const int CLOTHES_GIRL_NURSE = 34;

	public const int CLOTHES_GIRL_MAID = 35;

	public const int CLOTHES_GIRL_GOTH = 36;

	public const int CLOTHES_BOY_MINUTEMAN = 37;

	public const int CLOTHES_BOY_DISHWASHER = 38;

	public const int CLOTHES_GIRL_YUKI = 39;

	public const int CLOTHES_BOY_ARCHIBALD = 40;

	public const int CLOTHES_BOY_SUPER_HERO = 41;

	public const int CLOTHES_BOY_TREKKIE = 42;

	public const int CLOTHES_GIRL_KITTEN = 43;

	public const int CLOTHES_GIRL_KIMONO = 44;

	public const int CLOTHES_GIRL_SNIPER = 45;

	public const int CLOTHES_BOY_PUNK = 46;

	public const int CLOTHES_BOY_MATRIX = 47;

	public const int CLOTHES_BOY_VANDAL = 48;

	public const int BODY_BOY = 0;

	public const int BODY_GIRL = 1;

	public const int BODY_ROBOT = 2;

	public const int JETPACK_PACK = 0;

	public const int JETPACK_BATWINGS = 1;

	public const int JETPACK_BEEWINGS = 2;

	public const int JETPACK_BROWNWINGS = 3;

	public const int JETPACK_CROWWINGS = 4;

	public const int JETPACK_NITRO = 5;

	public const int JETPACK_SKELEWINGS = 6;

	public const int JETPACK_RAINBOW = 7;

	public const int JETPACK_ANGELWINGS = 8;

	public const int JETFIRE_NONE = 0;

	public const int JETFIRE_FIRE = 1;

	public const int JETFIRE_WINGS = 2;

	public const int JETFIRE_BEES = 3;

	public BodyType[] bodyType;

	public static int GetJetFire(int type)
	{
		switch (type)
		{
		case 0:
		case 5:
		case 7:
			return 1;
		case 1:
		case 3:
		case 4:
		case 6:
		case 8:
			return 2;
		case 2:
			return 3;
		default:
			return 0;
		}
	}

	public BodyCatalog()
	{
		bodyType = new BodyType[3];
		bodyType[2] = new BodyType(new int[1] { 3 }, null, new int[5] { 1, 6, 16, 4, 24 });
		bodyType[0] = new BodyType(new int[3] { 0, 19, 20 }, new int[23]
		{
			1, 5, 6, 7, 8, 9, 12, 13, 14, 18,
			23, 24, 25, 26, 27, 37, 38, 40, 41, 42,
			46, 47, 48
		}, new int[41]
		{
			1, 5, 6, 7, 8, 9, 10, 11, 12, 13,
			14, 15, 16, 4, 18, 23, 24, 25, 26, 27,
			28, 29, 30, 31, 32, 33, 34, 35, 36, 37,
			39, 38, 40, 41, 42, 43, 44, 45, 46, 47,
			48
		});
		bodyType[1] = new BodyType(new int[3] { 2, 21, 22 }, new int[19]
		{
			4, 10, 11, 15, 16, 17, 28, 29, 30, 31,
			32, 33, 34, 35, 36, 39, 43, 44, 45
		}, new int[41]
		{
			1, 5, 6, 7, 8, 9, 10, 11, 12, 13,
			14, 15, 16, 4, 18, 23, 24, 25, 26, 27,
			28, 29, 30, 31, 32, 33, 34, 35, 36, 37,
			39, 38, 40, 41, 42, 43, 44, 45, 46, 47,
			48
		});
	}
}
