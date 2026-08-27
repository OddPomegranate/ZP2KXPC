namespace ZP2K9.store.leveling;

public class LevelUnlock
{
	public const int UNLOCK_NONE = -1;

	public const int UNLOCK_CHARACTER_CLASS = 0;

	public const int UNLOCK_BOY_CLOTHES = 1;

	public const int UNLOCK_HAT = 2;

	public const int UNLOCK_GIRL_CLOTHES = 3;

	public const int UNLOCK_PERK_MOD = 5;

	public const int UNLOCK_PERK_OFFENSE = 6;

	public const int UNLOCK_PERK_DEFENSE = 7;

	public const int UNLOCK_APPEARANCE_EDITOR = 8;

	public const int UNLOCK_PERK_EDITOR = 9;

	public const int UNLOCK_RENAME = 10;

	public const int UNLOCK_CLANTAG = 11;

	public const int UNLOCK_JETPACK = 12;

	public int score;

	public int type = -1;

	public int idx;

	public void SetData(int type, int idx)
	{
		this.type = type;
		this.idx = idx;
	}
}
