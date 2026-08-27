namespace ZP2K9.map;

public class SpawnMgr
{
	private int[] spawnBank = new int[128];

	private GameMap gameMap;

	public SpawnMgr(GameMap map)
	{
		gameMap = map;
	}

	public int GetSpawn(int team)
	{
		if (gameMap.entityCount <= 0)
		{
			return -1;
		}
		int num = 0;
		for (int i = 0; i < gameMap.entityCount; i++)
		{
			if (gameMap.entity[i].type == team + 1 && num < spawnBank.Length)
			{
				spawnBank[num] = i;
				num++;
			}
		}
		if (num > 0)
		{
			int randomInt = Rand.GetRandomInt(0, num);
			return spawnBank[randomInt];
		}
		return -1;
	}

	private bool IsTeamSpawn(int team, int entity)
	{
		switch (GameState.gameType)
		{
		case 0:
			if (gameMap.entity[entity].type == 1 || gameMap.entity[entity].type == 3 || gameMap.entity[entity].type == 2)
			{
				return true;
			}
			break;
		case 1:
		case 2:
		case 3:
			switch (team)
			{
			case 1:
				if (gameMap.entity[entity].type == 2)
				{
					return true;
				}
				break;
			case 2:
				if (gameMap.entity[entity].type == 3)
				{
					return true;
				}
				break;
			case 0:
				if (gameMap.entity[entity].type == 1 || gameMap.entity[entity].type == 3 || gameMap.entity[entity].type == 2)
				{
					return true;
				}
				break;
			}
			break;
		}
		return false;
	}
}
