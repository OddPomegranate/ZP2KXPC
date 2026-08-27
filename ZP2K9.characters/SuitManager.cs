using System.Text;

namespace ZP2K9.characters;

internal class SuitManager
{
	public const int SUIT_NONE = 0;

	public const int SUIT_BEE = 1;

	public const int SUIT_CAT = 2;

	public const int SUIT_TURTLE = 3;

	public const int SUIT_SKUNK = 4;

	public const int SUIT_CHAMELEON = 5;

	public const int SUIT_HEDGEHOG = 6;

	public const int SUIT_PHOENIX = 7;

	public const int SUIT_BOMBER = 8;

	public const int SUIT_PRISM = 9;

	public const int SUIT_BAT = 10;

	public const int SUIT_TESLA = 11;

	public const int SUIT_CHEETA = 12;

	public const int SUIT_NEWPHOENIX = 100;

	public static StringBuilder[] suitText = new StringBuilder[24]
	{
		new StringBuilder("Bee Suit"),
		new StringBuilder("Use roll button to fly!"),
		new StringBuilder("Cat Suit"),
		new StringBuilder("Use roll button to sprint + higher jumps!"),
		new StringBuilder("Turtle Suit"),
		new StringBuilder("Reduced explosion damage!"),
		new StringBuilder("Skunk Suit"),
		new StringBuilder("Poison emitting + poison immune!"),
		new StringBuilder("Chameleon Suit"),
		new StringBuilder("Invisible while standing still!"),
		new StringBuilder("Hedgehog Suit"),
		new StringBuilder("Inflict double damage!"),
		new StringBuilder("Phoenix Suit"),
		new StringBuilder("Immune to fire + phoenix revival!"),
		new StringBuilder("Derka Suit"),
		new StringBuilder("Giant death explosion!"),
		new StringBuilder("Prism Suit"),
		new StringBuilder("Copy confusion + plasma immune!"),
		new StringBuilder("Bat Suit"),
		new StringBuilder("Health leeching + stealth ceiling hang!"),
		new StringBuilder("Tesla Suit"),
		new StringBuilder("Super-fun zappy time!"),
		new StringBuilder("Cyborg Suit"),
		new StringBuilder("Fast firing and reloads!")
	};

	public static StringBuilder phoenixFix = new StringBuilder("Immune to fire!");
}
