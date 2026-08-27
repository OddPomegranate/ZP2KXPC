using Microsoft.Xna.Framework;

namespace ZP2K9.particles;

public class ChronoMgr
{
	private struct ChronoList
	{
		public Vector2[] chronosVec;

		public int chronos;

		public void AddChrono(Vector2 loc)
		{
			if (chronos < chronosVec.Length)
			{
				chronosVec[chronos] = loc;
				chronos++;
			}
		}

		public void ResetChronos()
		{
			chronos = 0;
		}

		public bool GetChronod(Vector2 loc)
		{
			if (chronos <= 0)
			{
				return false;
			}
			for (int i = 0; i < chronos; i++)
			{
				if ((loc - chronosVec[i]).LengthSquared() < 90000f)
				{
					return true;
				}
			}
			return false;
		}
	}

	private ChronoList[] chronos;

	private int curDic;

	public ChronoMgr()
	{
		chronos = new ChronoList[2];
		for (int i = 0; i < chronos.Length; i++)
		{
			chronos[i].chronosVec = new Vector2[10];
		}
	}

	internal void AddChrono(Vector2 vector2)
	{
		chronos[1 - curDic].AddChrono(vector2);
	}

	internal void ResetChronos()
	{
		chronos[curDic].ResetChronos();
		curDic = 1 - curDic;
	}

	internal bool GetChronod(Vector2 loc)
	{
		return chronos[curDic].GetChronod(loc);
	}
}
