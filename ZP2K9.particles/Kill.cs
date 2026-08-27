namespace ZP2K9.particles;

public class Kill
{
	public const int KILLTYPE_KILL = 0;

	public const int KILLTYPE_BULLETS = 1;

	public const int KILLTYPE_FLAME = 2;

	public const int KILLTYPE_ICE = 3;

	public const int KILLTYPE_POISON = 4;

	public const int KILLTYPE_EXPLOSION = 5;

	public const int KILLTYPE_SQUASH = 6;

	public const int KILLTYPE_SWORD = 7;

	public const int KILLTYPE_KICK = 8;

	public const int KILLTYPE_ZAP = 9;

	public const int KILLTYPE_SHOT = 10;

	public const int KILLTYPE_FISH = 11;

	public const int KILLTYPE_BEES = 12;

	public const int KILLTYPE_CATS = 13;

	public const int KILLTYPE_DROWN = 14;

	public const int KILLTYPE_RAIL = 15;

	public const int KILLTYPE_LEECH = 16;

	public int killer;

	public int killee;

	public int type;

	public Kill()
	{
		killer = -1;
		killee = -1;
	}
}
