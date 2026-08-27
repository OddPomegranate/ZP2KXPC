namespace ZP2K9.menu;

public class Mutators
{
	public const int MUTATOR_NONE = 0;

	public const int MUTATOR_CLASSIC = 1;

	public const int MUTATOR_LOCKEDANDLOADED = 2;

	public const int MUTATOR_SWORDS = 3;

	public const int MUTATOR_HARDBOILED = 4;

	public const int MUTATOR_GRENADECRAZY = 5;

	public const int MUTATOR_NUKED = 6;

	public const int MUTATOR_EXPLOSIONADE = 7;

	public const int MUTATOR_NRG = 8;

	public const int MUTATOR_FIRESTORM = 9;

	public const int MUTATOR_INSTAKILL = 10;

	public const int MUTATOR_SCIENCE = 11;

	public const int MUTATOR_TIMESMASHERS = 12;

	public const int MUTATOR_SILLY = 13;

	public const int MUTATOR_JETMAX = 14;

	public const int MUTATOR_NOSWORDS = 15;

	public const int MUTATOR_GOODIEBAG = 16;

	public static string[] mutator = new string[17]
	{
		"None", "No Crates", "Heavy Metal", "Swords", "Hardboiled", "All 'Nades", "Nuked", "Explosionade™", "Hi-NRG", "Firestorm",
		"Instakill", "Science!", "Timesmashers", "WTF...?", "Jet Max", "No Swords", "Goodie Bag"
	};

	public static string[] GetAllStrings()
	{
		string[] array = new string[mutator.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = "Mutator: " + mutator[i];
		}
		return array;
	}

	internal static bool GetCrates(int mutator)
	{
		switch (mutator)
		{
		case 0:
		case 14:
		case 15:
			return true;
		default:
			return false;
		}
	}
}
