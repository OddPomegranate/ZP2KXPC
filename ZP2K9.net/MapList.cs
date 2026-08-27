using System.Collections.Generic;
using ZP2K9.debug;

namespace ZP2K9.net;

public class MapList
{
	public enum MapName
	{
		Core,
		Mound,
		TwoFort,
		Vert,
		Beach,
		Aqua,
		Grassy,
		Freighter,
		Research,
		IceBerg,
		Harvest
	}

	public static int[] maplist = new int[11]
	{
		0, 1, 4, 5, 2, 3, 6, 7, 8, 9,
		10
	};

	public static int total;

	public static Dictionary<int, MapDescription> mapCatalog;

	public static void Init()
	{
		mapCatalog = new Dictionary<int, MapDescription>();
		mapCatalog.Add(0, new MapDescription("core", "Megacore", "Small map. High energy."));
		mapCatalog.Add(1, new MapDescription("mound", "Big Clod", "Maze of honeycombs."));
		mapCatalog.Add(2, new MapDescription("2fort", "Two Fortresses", "It's red versus blue!"));
		mapCatalog.Add(3, new MapDescription("vert", "The Mine", "Vertical action."));
		mapCatalog.Add(4, new MapDescription("beach", "Castle Defense", "The castle is being overrun!"));
		mapCatalog.Add(5, new MapDescription("aqua", "Invaders!", "From pirate ship to underground lab!"));
		mapCatalog.Add(6, new MapDescription("grassy", "The Grassy Knoll", "A big map divided into four structures."));
		mapCatalog.Add(7, new MapDescription("freighter", "Derelict Freighter", "Big map that's sinking into the ocean."));
		mapCatalog.Add(8, new MapDescription("research", "Research Lab", "Mid-sized mountain research lab."));
		mapCatalog.Add(9, new MapDescription("iceberg", "Ice Burg", "Frozen little bloodbath."));
		mapCatalog.Add(10, new MapDescription("harvest", "The Harvest", "Get the green!"));
		Scramble();
	}

	public static void Scramble()
	{
		total = 0;
		foreach (KeyValuePair<int, MapDescription> item in mapCatalog)
		{
			if (item.Value.included)
			{
				maplist[total] = item.Key;
				total++;
			}
		}
		if (total == 0)
		{
			total = mapCatalog.Count;
			for (int i = 0; i < maplist.Length; i++)
			{
				maplist[i] = i;
			}
		}
		for (int j = 0; j < 10; j++)
		{
			int randomInt = Rand.GetRandomInt(0, total);
			int randomInt2 = Rand.GetRandomInt(0, total);
			int num = maplist[randomInt];
			maplist[randomInt] = maplist[randomInt2];
			maplist[randomInt2] = num;
		}
		if (!DebugManager.mapTestMode)
		{
			return;
		}
		for (int k = 0; k < maplist.Length; k++)
		{
			try
			{
				maplist[k] = 7;
			}
			catch
			{
			}
		}
	}
}
