using Microsoft.Xna.Framework;

namespace ZP2K9.ai;

public class Trail
{
	public int[] trail;

	private int[] closed;

	private int closedLen;

	public int trailLen;

	public Trail()
	{
		trail = new int[512];
		closed = new int[512];
	}

	public bool FindTrail(NodeMgr nodeMgr, int start, int end)
	{
		int num = 0;
		closedLen = 0;
		bool flag = false;
		trail[num] = start;
		if (start == end)
		{
			return false;
		}
		for (int i = 0; i < closed.Length; i++)
		{
			closed[i] = -1;
		}
		while (!flag)
		{
			int num2 = -1;
			int num3 = -1;
			for (int j = 0; j < nodeMgr.node[trail[num]].neighbors; j++)
			{
				int idx = nodeMgr.node[trail[num]].neighbor[j].idx;
				if (nodeMgr.node[idx] != null && !GetClosed(idx))
				{
					int intDist = GetIntDist(nodeMgr.node[end].x, nodeMgr.node[end].y, nodeMgr.node[idx].x, nodeMgr.node[idx].y);
					if (idx == end)
					{
						num2 = idx;
						num3 = intDist;
					}
					else if (num2 == -1)
					{
						num2 = idx;
						num3 = intDist;
					}
					else if (intDist < num3)
					{
						num2 = idx;
						num3 = intDist;
					}
				}
			}
			if (num2 > -1)
			{
				closed[closedLen] = num2;
				closedLen++;
				num++;
				trail[num] = num2;
				if (num2 == end)
				{
					trailLen = num + 1;
					return true;
				}
			}
			else
			{
				num--;
				if (num < 0)
				{
					return false;
				}
			}
		}
		return false;
	}

	private bool GetClosed(int i)
	{
		for (int j = 0; j < closedLen; j++)
		{
			if (closed[j] == i)
			{
				return true;
			}
		}
		return false;
	}

	private int GetIntDist(int x, int y, int tX, int tY)
	{
		return (int)((new Vector2(x, y) - new Vector2(tX, tY)).Length() * 10f);
	}
}
