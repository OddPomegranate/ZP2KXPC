namespace ZP2K9.ai;

public class Node
{
	public struct NodeNeighbor
	{
		public const int TYPE_RUN = 0;

		public const int TYPE_JUMP = 1;

		public const int TYPE_CLIMB = 2;

		public const int TYPE_DROP = 3;

		public int idx;

		public int type;

		public void Set(int idx, int type)
		{
			this.idx = idx;
			this.type = type;
		}
	}

	public int ID;

	public NodeNeighbor[] neighbor;

	public int neighbors;

	public int x;

	public int y;

	public Node(int ID, int x, int y)
	{
		this.ID = ID;
		this.x = x;
		this.y = y;
		neighbor = new NodeNeighbor[4];
		neighbors = 0;
	}

	public NodeNeighbor GetNeighborFromIdx(int idx)
	{
		for (int i = 0; i < neighbors; i++)
		{
			if (neighbor[i].idx == idx)
			{
				return neighbor[i];
			}
		}
		return default(NodeNeighbor);
	}

	public void AddNeighbor(int idx, int type)
	{
		if (neighbors >= neighbor.Length)
		{
			return;
		}
		for (int i = 0; i < neighbors; i++)
		{
			if (neighbor[i].idx == idx)
			{
				if (neighbor[i].type != 2)
				{
					neighbor[i].idx = idx;
					neighbor[i].type = type;
				}
				return;
			}
		}
		neighbor[neighbors].idx = idx;
		neighbor[neighbors].type = type;
		neighbors++;
	}
}
