using System;
using ZP2K9.store.leveling;

namespace ZP2K9.store;

public class Leveling
{
	public static LevelUnlock[] level;

	public static LevelUnlock[] baseUnlock;

	private static int cur;

	public static bool IsHappyHour(int h)
	{
		switch (h)
		{
		case 21:
		case 22:
		case 23:
			return true;
		default:
			return false;
		}
	}

	public static void Init()
	{
		level = new LevelUnlock[100];
		int num = 50;
		int num2 = 0;
		for (int i = 0; i < level.Length; i++)
		{
			level[i] = new LevelUnlock();
			num = ((i >= 25) ? ((i >= 50) ? ((i >= 60) ? ((i >= 70) ? ((i >= 80) ? ((i >= 90) ? (num + 80) : (num + 70)) : (num + 60)) : (num + 50)) : (num + 40)) : (num + 30)) : (num + 20));
			level[i].score = num + num2;
			num2 = level[i].score;
		}
		baseUnlock = new LevelUnlock[36];
		for (int j = 0; j < baseUnlock.Length; j++)
		{
			baseUnlock[j] = new LevelUnlock();
		}
		cur = 0;
		AddBaseUnlock(0, 0);
		AddBaseUnlock(0, 1);
		AddBaseUnlock(0, 2);
		AddBaseUnlock(1, 0);
		AddBaseUnlock(1, 1);
		AddBaseUnlock(1, 2);
		AddBaseUnlock(1, 3);
		AddBaseUnlock(1, 4);
		AddBaseUnlock(1, 6);
		AddBaseUnlock(1, 10);
		AddBaseUnlock(2, 0);
		AddBaseUnlock(2, 1);
		AddBaseUnlock(2, 2);
		AddBaseUnlock(2, 3);
		AddBaseUnlock(2, 4);
		AddBaseUnlock(2, 15);
		AddBaseUnlock(6, 0);
		AddBaseUnlock(5, 5);
		AddBaseUnlock(7, 2);
		AddBaseUnlock(6, 2);
		AddBaseUnlock(5, 0);
		AddBaseUnlock(7, 6);
		AddBaseUnlock(6, 6);
		AddBaseUnlock(5, 2);
		AddBaseUnlock(7, 5);
		AddBaseUnlock(6, 5);
		AddBaseUnlock(5, 7);
		AddBaseUnlock(7, 1);
		AddBaseUnlock(3, 0);
		AddBaseUnlock(3, 1);
		AddBaseUnlock(3, 2);
		AddBaseUnlock(3, 3);
		AddBaseUnlock(12, 0);
		AddBaseUnlock(1, 22);
		AddBaseUnlock(2, 40);
		cur = 0;
		AddLevel(0, 3);
		AddLevel(9, 0);
		AddLevel(11, 0);
		AddLevel(6, 3);
		AddLevel(8, 0);
		AddLevel(2, 13);
		AddLevel(10, 0);
		AddLevel(5, 6);
		AddLevel(7, 4);
		AddLevel(0, 4);
		AddLevel(6, 4);
		AddLevel(2, 8);
		AddLevel(5, 3);
		AddLevel(1, 9);
		AddLevel(2, 11);
		AddLevel(7, 3);
		AddLevel(0, 5);
		AddLevel(3, 4);
		AddLevel(6, 1);
		AddLevel(2, 6);
		AddLevel(5, 4);
		AddLevel(2, 7);
		AddLevel(3, 8);
		AddLevel(7, 7);
		AddLevel(1, 11);
		AddLevel(2, 14);
		AddLevel(3, 10);
		AddLevel(0, 6);
		AddLevel(2, 9);
		AddLevel(6, 7);
		AddLevel(2, 10);
		AddLevel(3, 5);
		AddLevel(5, 1);
		AddLevel(2, 12);
		AddLevel(7, 0);
		AddLevel(1, 5);
		AddLevel(2, 5);
		AddLevel(3, 9);
		AddLevel(2, 23);
		AddLevel(0, 7);
		AddLevel(2, 24);
		AddLevel(12, 1);
		AddLevel(6, 8);
		AddLevel(1, 7);
		AddLevel(3, 6);
		AddLevel(2, 16);
		AddLevel(5, 8);
		AddLevel(12, 2);
		AddLevel(1, 8);
		AddLevel(2, 25);
		AddLevel(3, 7);
		AddLevel(7, 9);
		AddLevel(2, 21);
		AddLevel(3, 11);
		AddLevel(7, 8);
		AddLevel(1, 12);
		AddLevel(2, 17);
		AddLevel(6, 9);
		AddLevel(12, 3);
		AddLevel(3, 12);
		AddLevel(2, 26);
		AddLevel(1, 13);
		AddLevel(2, 18);
		AddLevel(5, 9);
		AddLevel(12, 4);
		AddLevel(1, 14);
		AddLevel(2, 19);
		AddLevel(3, 13);
		AddLevel(2, 27);
		AddLevel(2, 20);
		AddLevel(12, 5);
		AddLevel(2, 22);
		AddLevel(3, 14);
		AddLevel(2, 28);
		AddLevel(1, 15);
		AddLevel(2, 29);
		AddLevel(1, 16);
		AddLevel(3, 15);
		AddLevel(2, 30);
		AddLevel(1, 17);
		AddLevel(2, 32);
		AddLevel(12, 6);
		AddLevel(3, 16);
		AddLevel(1, 18);
		AddLevel(2, 33);
		AddLevel(3, 17);
		AddLevel(1, 19);
		AddLevel(2, 34);
		AddLevel(3, 18);
		AddLevel(12, 7);
		AddLevel(2, 35);
		AddLevel(1, 20);
		AddLevel(2, 36);
		AddLevel(1, 21);
		AddLevel(2, 37);
		AddLevel(2, 38);
		AddLevel(2, 31);
		AddLevel(12, 8);
		AddLevel(2, 39);
		Console.WriteLine("Total level unlocks: " + cur);
	}

	private static void AddLevel(int type, int idx)
	{
		level[cur].SetData(type, idx);
		cur++;
	}

	private static void AddBaseUnlock(int type, int idx)
	{
		baseUnlock[cur].SetData(type, idx);
		cur++;
	}
}
