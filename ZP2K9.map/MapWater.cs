namespace ZP2K9.map;

public class MapWater
{
	public bool[,] water;

	public int waterLevel;

	public MapWater()
	{
		water = new bool[256, 256];
		waterLevel = 140;
	}
}
