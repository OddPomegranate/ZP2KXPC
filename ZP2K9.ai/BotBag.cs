namespace ZP2K9.ai;

public class BotBag
{
	public BotStyle[] botStyle;

	public BotBag()
	{
		botStyle = new BotStyle[8]
		{
			new BotStyle("DUNKAN", 0, 0, 7, 13, 1, 0),
			new BotStyle("Baron Earl", 0, 26, 10, 10, 5, 0),
			new BotStyle("The Chef", 0, 17, 6, 12, 12, 1),
			new BotStyle("Party Chris", 0, 18, 11, 0, 13, 0),
			new BotStyle("Ty 2000", 0, 21, 14, 6, 18, 0),
			new BotStyle("Izzy", 1, 2, 18, 11, 15, 0),
			new BotStyle("Sadie", 1, 3, 19, 8, 17, 0),
			new BotStyle("Courtney", 1, 5, 11, 2, 8, 1)
		};
	}

	public BotStyle Style(int idx)
	{
		return botStyle[(idx + Game1.gameMap.entityCount) % botStyle.Length];
	}
}
