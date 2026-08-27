using ZP2K9.map;

namespace ZP2K9.ai;

public class NodeMgr
{
	public Node[] node = new Node[512];

	public int maxNodes;

	public int xMin = -1;

	public int yMin = -1;

	public int xMax = -1;

	public int yMax = -1;

	public void Refresh(GameMap map)
	{
		xMin = -1;
		yMin = -1;
		xMax = -1;
		yMax = -1;
		maxNodes = 0;
		for (int i = 1; i < 255; i++)
		{
			for (int j = 1; j < 255; j++)
			{
				map.node[i, j] = -1;
				if (map.map.collision[i, j - 1] != 0)
				{
					continue;
				}
				if (map.map.collision[i, j] == 1)
				{
					if ((map.map.collision[i - 1, j - 1] == 0 && map.map.collision[i - 1, j] == 0) || (map.map.collision[i + 1, j - 1] == 0 && map.map.collision[i + 1, j] == 0) || map.map.collision[i + 1, j - 1] == 1 || map.map.collision[i + 1, j - 1] == 3 || map.map.collision[i - 1, j - 1] == 1 || map.map.collision[i - 1, j - 1] == 2)
					{
						node[maxNodes] = new Node(maxNodes, i, j - 1);
						map.node[i, j - 1] = maxNodes;
						maxNodes++;
					}
				}
				else if (map.map.collision[i, j] == 2)
				{
					if ((map.map.collision[i - 1, j] == 0 && map.map.collision[i - 1, j + 1] == 0) || (map.map.collision[i + 1, j - 1] == 0 && map.map.collision[i + 1, j] == 0) || map.map.collision[i + 1, j - 1] == 1 || map.map.collision[i + 1, j - 1] == 3 || map.map.collision[i - 1, j] == 1 || map.map.collision[i - 1, j] == 2)
					{
						node[maxNodes] = new Node(maxNodes, i, j - 1);
						map.node[i, j - 1] = maxNodes;
						maxNodes++;
					}
				}
				else if (map.map.collision[i, j] == 3 && ((map.map.collision[i - 1, j - 1] == 0 && map.map.collision[i - 1, j] == 0) || (map.map.collision[i + 1, j] == 0 && map.map.collision[i + 1, j + 1] == 0) || map.map.collision[i + 1, j] == 1 || map.map.collision[i + 1, j] == 3 || map.map.collision[i - 1, j - 1] == 1 || map.map.collision[i - 1, j - 1] == 2))
				{
					node[maxNodes] = new Node(maxNodes, i, j - 1);
					map.node[i, j - 1] = maxNodes;
					maxNodes++;
				}
			}
		}
		for (int k = 0; k < map.entityCount; k++)
		{
			if (!map.entity[k].exists)
			{
				continue;
			}
			bool flag = false;
			int x = map.entity[k].x;
			int num = map.entity[k].y;
			while (!flag)
			{
				num++;
				if (num >= 256)
				{
					break;
				}
				if (map.map.collision[x, num] > 0)
				{
					num--;
					if (map.node[x, num] < 0)
					{
						map.entity[k].node = maxNodes;
						node[maxNodes] = new Node(maxNodes, x, num);
						map.node[x, num] = maxNodes;
						maxNodes++;
					}
					else
					{
						map.entity[k].node = map.node[x, num];
					}
					break;
				}
			}
		}
		for (int l = 0; l < maxNodes; l++)
		{
			int x2 = node[l].x;
			int y = node[l].y;
			int num2 = node[l].x;
			int num3 = node[l].y;
			int num4 = 0;
			bool flag2 = false;
			bool flag3 = false;
			while (!flag3)
			{
				if (num2 > 0 && num3 > 0 && num2 < 255 && num3 < 254)
				{
					if (map.map.collision[num2, num3] == 1 || map.map.collision[num2, num3] == 3)
					{
						num3--;
						if (map.map.collision[num2 - 1, num3] != 0)
						{
						}
						flag3 = true;
					}
					else
					{
						switch (map.map.collision[num2, num3 + 1])
						{
						case 1:
							num2++;
							if (map.map.collision[num2, num3] == 2)
							{
								num3--;
							}
							break;
						case 2:
							num2++;
							num3--;
							break;
						case 3:
							num2++;
							num3++;
							break;
						case 0:
							num3++;
							num4 = ((map.map.collision[num2 - 1, num3] == 0 || map.map.collision[num2 - 1, num3] == 3) ? (num4 + 1) : 0);
							if (map.map.collision[num2, num3 + 1] != 0 && num3 > y)
							{
								AddMapNode(num2, num3, map);
								if (map.node[num2, num3] > -1 && !flag2 && num4 < ((map.map.collision[num2, num3 + 1] == 0) ? 6 : 5))
								{
									node[map.node[num2, num3]].AddNeighbor(l, 2);
								}
							}
							if (map.node[num2 - 1, num3] > -1)
							{
								flag2 = true;
							}
							break;
						}
					}
					if (map.node[num2, num3] > -1 && map.node[num2, num3] != l && !flag3)
					{
						node[map.node[x2, y]].AddNeighbor(map.node[num2, num3], 0);
						flag3 = true;
					}
				}
				else
				{
					flag3 = true;
				}
			}
			num2 = node[l].x;
			num3 = node[l].y;
			num4 = 0;
			flag2 = false;
			flag3 = false;
			while (!flag3)
			{
				if (num2 > 0 && num3 > 0 && num2 < 255 && num3 < 254)
				{
					if (map.map.collision[num2, num3] == 1 || map.map.collision[num2, num3] == 2)
					{
						num3--;
						if (map.map.collision[num2 + 1, num3] != 0)
						{
						}
						flag3 = true;
					}
					else
					{
						switch (map.map.collision[num2, num3 + 1])
						{
						case 1:
							num2--;
							if (map.map.collision[num2, num3] == 3)
							{
								num3--;
							}
							break;
						case 2:
							num2--;
							num3++;
							break;
						case 3:
							num2--;
							num3--;
							break;
						case 0:
							num3++;
							num4 = ((map.map.collision[num2 + 1, num3] == 0 || map.map.collision[num2 + 1, num3] == 2) ? (num4 + 1) : 0);
							if (map.map.collision[num2, num3 + 1] != 0 && num3 > y)
							{
								AddMapNode(num2, num3, map);
								if (map.node[num2, num3] > -1 && !flag2 && num4 < ((map.map.collision[num2, num3 + 1] == 0) ? 6 : 5))
								{
									node[map.node[num2, num3]].AddNeighbor(l, 2);
								}
							}
							if (map.node[num2 + 1, num3] > -1)
							{
								flag2 = true;
							}
							break;
						}
					}
					if (map.node[num2, num3] <= -1 || map.node[num2, num3] == l || flag3)
					{
						continue;
					}
					for (int m = 0; m < node[l].neighbors; m++)
					{
						if (node[l].neighbor[m].idx == map.node[num2, num3])
						{
						}
					}
					node[map.node[x2, y]].AddNeighbor(map.node[num2, num3], 0);
					flag3 = true;
				}
				else
				{
					flag3 = true;
				}
			}
			num2 = node[l].x;
			num3 = node[l].y;
			flag3 = false;
			int num5 = 5;
			while (!flag3)
			{
				num2++;
				if (num2 > 0 && num3 > 1 && num2 < 255 && num3 < 250)
				{
					if (num2 == node[l].x + 1)
					{
						for (int num6 = num3 + 3; num6 > num3 - 3; num6--)
						{
							if (map.map.collision[num2, num6] != 0)
							{
								flag3 = true;
							}
						}
					}
					else if (num2 == node[l].x + 3 || num2 == node[l].x + 2)
					{
						bool flag4 = false;
						for (int num7 = num3 + 3; num7 > num3 - num5; num7--)
						{
							if (map.map.collision[num2, num7] != 0)
							{
								flag4 = true;
							}
						}
						if (flag4)
						{
							if (num2 > node[l].x + 1)
							{
								num3 += 3;
								bool flag5 = false;
								while (!flag5)
								{
									num3--;
									if (map.map.collision[num2 - 1, num3] != 0)
									{
										flag5 = true;
									}
									else if (num2 > 0 && num3 > 1 && num2 < 255 && num3 < 254)
									{
										if (num3 < node[l].y - (num5 + 1))
										{
											flag5 = true;
										}
										else if (map.map.collision[num2, num3] == 0 && map.map.collision[num2, num3 + 1] != 0)
										{
											flag5 = true;
											if (map.node[num2, num3] > -1)
											{
												node[map.node[num2, num3]].AddNeighbor(map.node[x2, y], 1);
											}
										}
									}
									else
									{
										flag5 = true;
									}
								}
							}
							flag3 = true;
						}
					}
					else if (map.map.collision[num2, num3 + 1] != 0)
					{
						flag3 = true;
					}
				}
				else
				{
					flag3 = true;
				}
				if (num2 > node[l].x + 2)
				{
					flag3 = true;
				}
			}
			num2 = node[l].x;
			num3 = node[l].y;
			flag3 = false;
			while (!flag3)
			{
				num2--;
				if (num2 > 0 && num3 > 1 && num2 < 255 && num3 < 250)
				{
					if (num2 == node[l].x - 1)
					{
						for (int num8 = num3 + 3; num8 > num3 - 3; num8--)
						{
							if (map.map.collision[num2, num8] != 0)
							{
								flag3 = true;
							}
						}
					}
					else if (num2 == node[l].x - 3 || num2 == node[l].x - 2)
					{
						bool flag6 = false;
						for (int num9 = num3 + 3; num9 > num3 - num5; num9--)
						{
							if (map.map.collision[num2, num9] != 0)
							{
								flag6 = true;
							}
						}
						if (flag6)
						{
							if (num2 < node[l].x - 1)
							{
								num3 += 3;
								bool flag7 = false;
								while (!flag7)
								{
									num3--;
									if (map.map.collision[num2 + 1, num3] != 0)
									{
										flag7 = true;
									}
									else if (num2 > 0 && num3 > 1 && num2 < 255 && num3 < 254)
									{
										if (num3 < node[l].y - (num5 + 1))
										{
											flag7 = true;
										}
										else if (map.map.collision[num2, num3] == 0 && map.map.collision[num2, num3 + 1] != 0)
										{
											flag7 = true;
											if (map.node[num2, num3] > -1)
											{
												node[map.node[num2, num3]].AddNeighbor(map.node[x2, y], 1);
											}
										}
									}
									else
									{
										flag7 = true;
									}
								}
							}
							flag3 = true;
						}
					}
					else if (map.map.collision[num2, num3 + 1] != 0)
					{
						flag3 = true;
					}
				}
				else
				{
					flag3 = true;
				}
				if (num2 < node[l].x - 2)
				{
					flag3 = true;
				}
			}
		}
	}

	private void AddMapNode(int tX, int tY, GameMap map)
	{
		if (xMin == -1 || tX < xMin)
		{
			xMin = tX;
		}
		if (yMin == -1 || tY < yMin)
		{
			yMin = tY;
		}
		if (xMax == -1 || tX > xMax)
		{
			xMax = tX;
		}
		if (yMax == -1 || tY > yMax)
		{
			yMax = tY;
		}
		if (map.node[tX, tY] == -1 && maxNodes < node.Length - 1)
		{
			node[maxNodes] = new Node(maxNodes, tX, tY);
			map.node[tX, tY] = maxNodes;
			maxNodes++;
		}
	}
}
