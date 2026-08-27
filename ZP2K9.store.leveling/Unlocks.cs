using System.IO;

namespace ZP2K9.store.leveling;

public class Unlocks
{
	public const int LOCKED = 0;

	public const int UNLOCKED_NEW = 1;

	public const int UNLOCKED = 2;

	public const int PERK_COUNT = 10;

	public int[] classUnlocked = new int[10];

	public int[,] perkUnlocked = new int[3, 10];

	public int[] boyHatUnlocked;

	public int[] girlHatUnlocked;

	public int[] boyHeadUnlocked;

	public int[] girlHeadUnlocked;

	public int[] boyTorsoUnlocked;

	public int[] girlTorsoUnlocked;

	public int[] boyLegsUnlocked;

	public int[] girlLegsUnlocked;

	public int[] jetpackUnlocked;

	public int appearanceEditorUnlocked;

	public int perkEditorUnlocked;

	public int renameUnlocked;

	public int clanTagUnlocked;

	public Unlocks()
	{
		boyHatUnlocked = new int[Game1.bodyCatalog.bodyType[0].hatList.Length];
		boyHeadUnlocked = new int[Game1.bodyCatalog.bodyType[0].clothesList.Length];
		boyTorsoUnlocked = new int[Game1.bodyCatalog.bodyType[0].clothesList.Length];
		boyLegsUnlocked = new int[Game1.bodyCatalog.bodyType[0].clothesList.Length];
		girlHatUnlocked = new int[Game1.bodyCatalog.bodyType[1].hatList.Length];
		girlHeadUnlocked = new int[Game1.bodyCatalog.bodyType[1].clothesList.Length];
		girlTorsoUnlocked = new int[Game1.bodyCatalog.bodyType[1].clothesList.Length];
		girlLegsUnlocked = new int[Game1.bodyCatalog.bodyType[1].clothesList.Length];
		jetpackUnlocked = new int[10];
	}

	public void Write(BinaryWriter writer)
	{
		for (int i = 0; i < classUnlocked.Length; i++)
		{
			writer.Write(classUnlocked[i]);
		}
		for (int j = 0; j < 3; j++)
		{
			for (int k = 0; k < 10; k++)
			{
				writer.Write(perkUnlocked[j, k]);
			}
		}
		for (int l = 0; l < boyHatUnlocked.Length; l++)
		{
			writer.Write(boyHatUnlocked[l]);
		}
		for (int m = 0; m < boyHeadUnlocked.Length; m++)
		{
			writer.Write(boyHeadUnlocked[m]);
		}
		for (int n = 0; n < boyTorsoUnlocked.Length; n++)
		{
			writer.Write(boyTorsoUnlocked[n]);
		}
		for (int num = 0; num < boyLegsUnlocked.Length; num++)
		{
			writer.Write(boyLegsUnlocked[num]);
		}
		for (int num2 = 0; num2 < girlHatUnlocked.Length; num2++)
		{
			writer.Write(girlHatUnlocked[num2]);
		}
		for (int num3 = 0; num3 < girlHeadUnlocked.Length; num3++)
		{
			writer.Write(girlHeadUnlocked[num3]);
		}
		for (int num4 = 0; num4 < girlTorsoUnlocked.Length; num4++)
		{
			writer.Write(girlTorsoUnlocked[num4]);
		}
		for (int num5 = 0; num5 < girlLegsUnlocked.Length; num5++)
		{
			writer.Write(girlLegsUnlocked[num5]);
		}
		for (int num6 = 0; num6 < jetpackUnlocked.Length; num6++)
		{
			writer.Write(jetpackUnlocked[num6]);
		}
		writer.Write(appearanceEditorUnlocked);
		writer.Write(perkEditorUnlocked);
		writer.Write(renameUnlocked);
		writer.Write(clanTagUnlocked);
	}

	public void Read(BinaryReader reader)
	{
		for (int i = 0; i < classUnlocked.Length; i++)
		{
			classUnlocked[i] = reader.ReadInt32();
		}
		for (int j = 0; j < 3; j++)
		{
			for (int k = 0; k < 10; k++)
			{
				perkUnlocked[j, k] = reader.ReadInt32();
			}
		}
		for (int l = 0; l < boyHatUnlocked.Length; l++)
		{
			boyHatUnlocked[l] = reader.ReadInt32();
		}
		for (int m = 0; m < boyHeadUnlocked.Length; m++)
		{
			boyHeadUnlocked[m] = reader.ReadInt32();
		}
		for (int n = 0; n < boyTorsoUnlocked.Length; n++)
		{
			boyTorsoUnlocked[n] = reader.ReadInt32();
		}
		for (int num = 0; num < boyLegsUnlocked.Length; num++)
		{
			boyLegsUnlocked[num] = reader.ReadInt32();
		}
		for (int num2 = 0; num2 < girlHatUnlocked.Length; num2++)
		{
			girlHatUnlocked[num2] = reader.ReadInt32();
		}
		for (int num3 = 0; num3 < girlHeadUnlocked.Length; num3++)
		{
			girlHeadUnlocked[num3] = reader.ReadInt32();
		}
		for (int num4 = 0; num4 < girlTorsoUnlocked.Length; num4++)
		{
			girlTorsoUnlocked[num4] = reader.ReadInt32();
		}
		for (int num5 = 0; num5 < girlLegsUnlocked.Length; num5++)
		{
			girlLegsUnlocked[num5] = reader.ReadInt32();
		}
		for (int num6 = 0; num6 < jetpackUnlocked.Length; num6++)
		{
			jetpackUnlocked[num6] = reader.ReadInt32();
		}
		appearanceEditorUnlocked = reader.ReadInt32();
		perkEditorUnlocked = reader.ReadInt32();
		renameUnlocked = reader.ReadInt32();
		clanTagUnlocked = reader.ReadInt32();
	}

	public int BoyHatUnlocked(int i)
	{
		if (i > 0 && i <= boyHatUnlocked.Length)
		{
			return boyHatUnlocked[i - 1];
		}
		return 2;
	}

	public int BoyHeadUnlocked(int i)
	{
		if (i > 0 && i <= boyHeadUnlocked.Length)
		{
			return boyHeadUnlocked[i - 1];
		}
		return 2;
	}

	public int BoyTorsoUnlocked(int i)
	{
		if (i > 0 && i <= boyTorsoUnlocked.Length)
		{
			return boyTorsoUnlocked[i - 1];
		}
		return 2;
	}

	public int BoyLegsUnlocked(int i)
	{
		if (i > 0 && i <= boyLegsUnlocked.Length)
		{
			return boyLegsUnlocked[i - 1];
		}
		return 2;
	}

	public int GirlHatUnlocked(int i)
	{
		if (i > 0 && i <= girlHatUnlocked.Length)
		{
			return girlHatUnlocked[i - 1];
		}
		return 2;
	}

	public int GirlHeadUnlocked(int i)
	{
		if (i > 0 && i <= girlHeadUnlocked.Length)
		{
			return girlHeadUnlocked[i - 1];
		}
		return 2;
	}

	public int GirlTorsoUnlocked(int i)
	{
		if (i > 0 && i <= girlTorsoUnlocked.Length)
		{
			return girlTorsoUnlocked[i - 1];
		}
		return 2;
	}

	public int GirlLegsUnlocked(int i)
	{
		if (i > 0 && i <= girlLegsUnlocked.Length)
		{
			return girlLegsUnlocked[i - 1];
		}
		return 2;
	}

	public void LockAll()
	{
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < 10; j++)
			{
				perkUnlocked[i, j] = 0;
			}
		}
		perkEditorUnlocked = 0;
		appearanceEditorUnlocked = 0;
		for (int k = 0; k < classUnlocked.Length; k++)
		{
			classUnlocked[k] = 0;
		}
		for (int l = 0; l < boyHatUnlocked.Length; l++)
		{
			boyHatUnlocked[l] = 0;
		}
		for (int m = 0; m < girlHatUnlocked.Length; m++)
		{
			girlHatUnlocked[m] = 0;
		}
		for (int n = 0; n < boyHeadUnlocked.Length; n++)
		{
			boyHeadUnlocked[n] = 0;
		}
		for (int num = 0; num < boyTorsoUnlocked.Length; num++)
		{
			boyTorsoUnlocked[num] = 0;
		}
		for (int num2 = 0; num2 < boyLegsUnlocked.Length; num2++)
		{
			boyLegsUnlocked[num2] = 0;
		}
		for (int num3 = 0; num3 < girlHeadUnlocked.Length; num3++)
		{
			girlHeadUnlocked[num3] = 0;
		}
		for (int num4 = 0; num4 < girlTorsoUnlocked.Length; num4++)
		{
			girlTorsoUnlocked[num4] = 0;
		}
		for (int num5 = 0; num5 < girlLegsUnlocked.Length; num5++)
		{
			girlLegsUnlocked[num5] = 0;
		}
		for (int num6 = 0; num6 < jetpackUnlocked.Length; num6++)
		{
			jetpackUnlocked[num6] = 0;
		}
		for (int num7 = 0; num7 < Leveling.baseUnlock.Length; num7++)
		{
			DoUnlock(Leveling.baseUnlock[num7]);
		}
	}

	public void UpdateUnlocks()
	{
		for (int i = 0; i < Leveling.baseUnlock.Length; i++)
		{
			DoUnlock(Leveling.baseUnlock[i]);
		}
		for (int j = 0; j < Game1.zProfile.level; j++)
		{
			DoUnlock(Leveling.level[j]);
		}
	}

	private void DoUnlock(LevelUnlock unlock)
	{
		switch (unlock.type)
		{
		case 8:
			if (appearanceEditorUnlocked == 0)
			{
				appearanceEditorUnlocked = 1;
			}
			break;
		case 10:
			if (renameUnlocked == 0)
			{
				renameUnlocked = 1;
			}
			break;
		case 11:
			if (clanTagUnlocked == 0)
			{
				clanTagUnlocked = 1;
			}
			break;
		case 1:
			if (boyHeadUnlocked[unlock.idx] == 0)
			{
				boyHeadUnlocked[unlock.idx] = 1;
			}
			if (boyTorsoUnlocked[unlock.idx] == 0)
			{
				boyTorsoUnlocked[unlock.idx] = 1;
			}
			if (boyLegsUnlocked[unlock.idx] == 0)
			{
				boyLegsUnlocked[unlock.idx] = 1;
			}
			break;
		case 2:
			if (boyHatUnlocked[unlock.idx] == 0)
			{
				boyHatUnlocked[unlock.idx] = 1;
			}
			if (girlHatUnlocked[unlock.idx] == 0)
			{
				girlHatUnlocked[unlock.idx] = 1;
			}
			break;
		case 3:
			if (girlHeadUnlocked[unlock.idx] == 0)
			{
				girlHeadUnlocked[unlock.idx] = 1;
			}
			if (girlTorsoUnlocked[unlock.idx] == 0)
			{
				girlTorsoUnlocked[unlock.idx] = 1;
			}
			if (girlLegsUnlocked[unlock.idx] == 0)
			{
				girlLegsUnlocked[unlock.idx] = 1;
			}
			break;
		case 0:
			if (classUnlocked[unlock.idx] == 0)
			{
				classUnlocked[unlock.idx] = 1;
			}
			break;
		case 12:
			if (jetpackUnlocked[unlock.idx] == 0)
			{
				jetpackUnlocked[unlock.idx] = 1;
			}
			break;
		case 7:
			if (perkUnlocked[2, unlock.idx] == 0)
			{
				perkUnlocked[2, unlock.idx] = 1;
			}
			break;
		case 5:
			if (perkUnlocked[1, unlock.idx] == 0)
			{
				perkUnlocked[1, unlock.idx] = 1;
			}
			break;
		case 6:
			if (perkUnlocked[0, unlock.idx] == 0)
			{
				perkUnlocked[0, unlock.idx] = 1;
			}
			break;
		case 9:
			if (perkEditorUnlocked == 0)
			{
				perkEditorUnlocked = 1;
			}
			break;
		case 4:
			break;
		}
	}
}
