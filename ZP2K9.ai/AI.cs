using System;
using Microsoft.Xna.Framework;
using Yuki_Win;
using ZP2K9.characters;
using ZP2K9.characters.weapons;
using ZP2K9.debug;
using ZP2K9.map;
using ZP2K9.particles;

namespace ZP2K9.ai;

public class AI
{
	public const int BASE_INDEX = 20;

	public const int BOT_STRING_COUNT = 7;

	public Trail trail;

	private bool hasTrail;

	public int ID;

	public int trailNode;

	private int maxNode;

	private float maxNodeTime;

	private int targ;

	private float targFrame;

	private float grenadeHappy;

	private float grenadeSad;

	private int trailCount;

	private float visTime;

	private float badAim;

	public float shotTime = 2f;

	private float stuckFrame;

	private float angle;

	private bool hitWall;

	private int climbAttempts;

	public AI(int ID)
	{
		this.ID = ID;
		trail = new Trail();
	}

	public void GetTrail(int start, int end)
	{
		if (DebugManager.aiFollow && Game1.character[0].lastNode > -1)
		{
			end = Game1.character[0].lastNode;
		}
		trailCount++;
		if (trail.FindTrail(Game1.nodeMgr, start, end))
		{
			hasTrail = true;
			trailNode = 0;
			maxNode = 0;
			maxNodeTime = 0f;
			climbAttempts = 0;
		}
		else
		{
			hasTrail = false;
		}
	}

	public static bool GetVis(Vector2 s, Vector2 e, GameMap map)
	{
		float num = (e - s).Length();
		int num2 = (int)(num / 20f);
		if (num2 < 2)
		{
			num2 = 2;
		}
		for (int i = 1; i < num2; i++)
		{
			float num3 = (float)i / (float)num2;
			if (map.GetIsCol((e - s) * num3 + s))
			{
				return false;
			}
		}
		return true;
	}

	public void Update(Character[] c, GameMap map)
	{
		bool flag = false;
		float num = 1f;
		float num2 = 1f;
		switch (Game1.netSession.botDifficulty)
		{
		case 0:
			grenadeHappy = 0f;
			grenadeSad = 10f;
			shotTime = 2f;
			num = 0.5f;
			num2 = 0.6f;
			break;
		case 1:
			num = 0.25f;
			shotTime = 1f;
			if (grenadeHappy > 1f)
			{
				grenadeHappy = 1f;
			}
			num2 = 1f;
			break;
		case 2:
			num = 0.1f;
			shotTime = 0f;
			num2 = 2.8f;
			break;
		case 3:
			num = 0.05f;
			shotTime = 0f;
			num2 = 10f;
			break;
		}
		try
		{
			_ = ID;
			c[ID].level = 0;
			c[ID].charKeys.ClearKeys();
			c[ID].charKeys.SetKeyPickup();
			if (Game1.netSession.postLobby)
			{
				return;
			}
			_ = trail.trail[trailNode];
			c[ID].bodyType = 2;
			c[ID].headTex = (c[ID].hatTex = (c[ID].legsTex = (c[ID].torsoTex = (c[ID].skinTex = 0))));
			c[ID].hatTex = ID % (Game1.bodyCatalog.bodyType[2].hatList.Length + 1);
			if (DebugManager.fakeRealPlayers)
			{
				c[ID].bodyType = Game1.zProfile.ClassSet(ID % 8).bodyType;
				c[ID].skinTex = Game1.zProfile.Class(ID % 8).skinTex;
				c[ID].headTex = Game1.zProfile.Class(ID % 8).headTex;
				c[ID].hatTex = Game1.zProfile.Class(ID % 8).hatTex;
				c[ID].torsoTex = Game1.zProfile.Class(ID % 8).torsoTex;
				c[ID].legsTex = Game1.zProfile.Class(ID % 8).legsTex;
				c[ID].jetpack = Game1.zProfile.Class(ID % 8).jetpack;
			}
			if (ID >= 20)
			{
				c[ID].bodyType = Game1.botBag.Style(ID).body;
				c[ID].skinTex = Game1.botBag.Style(ID).skin;
				c[ID].headTex = Game1.botBag.Style(ID).head;
				c[ID].hatTex = Game1.botBag.Style(ID).hat;
				c[ID].torsoTex = Game1.botBag.Style(ID).torso;
				c[ID].legsTex = Game1.botBag.Style(ID).legs;
				c[ID].jetpack = 0;
			}
			if (grenadeHappy >= 0f)
			{
				grenadeHappy -= Game1.frameTime;
				if (grenadeHappy <= 0f)
				{
					grenadeSad = Rand.GetRandomFloat(1f, 11f);
				}
			}
			if (grenadeSad >= 0f)
			{
				grenadeSad -= Game1.frameTime;
				if (grenadeSad <= 0f)
				{
					grenadeHappy = Rand.GetRandomFloat(1f, 11f);
				}
			}
			targFrame -= Game1.frameTime;
			if (GameState.gameType == 4 && c[ID].team == 0)
			{
				targFrame -= Game1.frameTime * 10f;
			}
			if (WeaponCatalog.weapons[c[ID].weapon[c[ID].curWeap]].projType == 10)
			{
				targFrame -= Game1.frameTime * 10f;
			}
			if (targFrame < 0f)
			{
				targFrame += Rand.GetRandomFloat(0.1f, 0.5f);
				targ = -1;
				int num3 = -1;
				float num4 = 0f;
				for (int i = 0; i < c.Length; i++)
				{
					if (i == ID || c[i] == null || !(c[i].dyingFrame <= 0f))
					{
						continue;
					}
					if (c[i].suit == 5)
					{
						c[i].traj.LengthSquared();
					}
					if ((c[i].suit != 10 || c[i].state != 4 || !(c[i].traj.LengthSquared() <= 100f)) && HitManager.GetHostile(i, ID))
					{
						float num5 = (c[i].loc - c[ID].loc).Length();
						float num6 = 550f;
						if (WeaponCatalog.weapons[c[ID].weapon[c[ID].curWeap]].projType == 10)
						{
							num6 = 300f;
						}
						if (num5 < num6 && GetVis(c[ID].loc + new Vector2(0f, -40f), c[i].loc + new Vector2(0f, -40f), map) && (num3 == -1 || num5 < num4))
						{
							num4 = num5;
							num3 = i;
						}
					}
				}
				if (num3 > -1)
				{
					targ = num3;
				}
			}
			if (grenadeHappy > 0f)
			{
				if (c[ID].grenAmmo[0] > 0)
				{
					c[ID].charKeys.keyGrenade = true;
				}
				if (c[ID].grenAmmo[1] > 0)
				{
					c[ID].charKeys.keyGren2 = true;
				}
			}
			badAim += Game1.frameTime;
			if (badAim > 6.28f)
			{
				badAim -= 6.28f;
			}
			if (targ > -1)
			{
				if (c[targ] != null)
				{
					if ((c[targ].loc - c[ID].loc).Length() < 20f && Rand.CointToss(0.5f))
					{
						c[ID].charKeys.keyKick = true;
					}
					visTime += Game1.frameTime;
					if (visTime > shotTime)
					{
						float num7 = 4f - (visTime - shotTime);
						num7 /= 10f;
						if (num7 > 1f)
						{
							num7 = 1f;
						}
						if (num7 < 0.5f)
						{
							num7 = 0.5f;
						}
						Vector2 loc = c[targ].loc;
						float num8 = Trig.GetAngle(loc - c[ID].loc, default(Vector2));
						num8 += (float)Math.Cos(badAim) * num * num7;
						float num9;
						for (num9 = num8 - angle; num9 > 3.14f; num9 -= 6.28f)
						{
						}
						for (; num9 < -3.14f; num9 += 6.28f)
						{
						}
						float num10 = num2 * Game1.frameTime;
						if (num10 > 1f)
						{
							num10 = 1f;
						}
						angle += num9 * num10;
						if (num9 > -1f && num9 < 1f)
						{
							c[ID].charKeys.shootVec = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
							if (flag)
							{
								Game1.pMan.AddParticle(1, c[ID].loc + c[ID].charKeys.shootVec * 100f, default(Vector2), 0.1f, 0, 0);
							}
						}
					}
					if (c[ID].ammo[WeaponCatalog.weapons[c[ID].weapon[c[ID].curWeap]].ammoType] <= 0)
					{
						c[ID].curWeap = 0;
					}
				}
				else
				{
					targ = -1;
				}
			}
			else
			{
				visTime = Rand.GetRandomFloat(0f, 0.5f);
			}
			if (hasTrail)
			{
				if (c[ID].lastNode > -1)
				{
					for (int j = 0; j < trail.trailLen; j++)
					{
						if (trail.trail[j] == c[ID].lastNode)
						{
						}
					}
				}
				if (maxNodeTime > 30f)
				{
					hasTrail = false;
				}
			}
			else
			{
				switch (c[ID].state)
				{
				case 2:
				case 3:
				case 4:
					c[ID].charKeys.keyJump = true;
					break;
				}
			}
			if (c[ID].hp < 0 || c[ID].bodySec[0].animName == "jhit" || c[ID].bodySec[0].animName == "hitland")
			{
				hasTrail = false;
			}
			else if (!hasTrail)
			{
				if (c[ID].state != 0 && !(c[ID].spawnFrame > 0f))
				{
					GetTrail(c, map);
				}
			}
			else
			{
				if (trailNode < 1)
				{
					trailNode = 1;
				}
				Node node = Game1.nodeMgr.node[trail.trail[trailNode - 1]];
				Node node2 = Game1.nodeMgr.node[trail.trail[trailNode]];
				int type = node.GetNeighborFromIdx(node2.ID).type;
				Vector2 vector = new Vector2((float)node2.x * 64f + 32f, (float)node2.y * 32f + 32f);
				Vector2 vector2 = new Vector2((float)node2.x * 64f + 32f, (float)node2.y * 32f + 32f);
				Vector2 dif = vector - c[ID].loc;
				_ = vector2 - c[ID].loc;
				_ = c[ID].loc.X / 64f;
				_ = (c[ID].loc.Y - 16f) / 32f;
				bool flag2 = false;
				switch (type)
				{
				case 0:
					if (dif.X > 10f)
					{
						c[ID].charKeys.keyRight = true;
					}
					else if (dif.X < -10f)
					{
						c[ID].charKeys.keyLeft = true;
					}
					else
					{
						c[ID].traj.X = 0f;
						if (c[ID].state != 0 || !(dif.Y > 0f))
						{
							flag2 = true;
						}
					}
					if (hitWall)
					{
						flag2 = true;
					}
					if (GameState.gameType == 4 && c[ID].team == 1 && ((c[ID].charKeys.keyLeft && dif.X < -60f) || (c[ID].charKeys.keyRight && dif.X > 60f)))
					{
						c[ID].charKeys.keyFloat = true;
					}
					break;
				case 1:
					if (dif.X > 12f)
					{
						c[ID].charKeys.keyRight = true;
					}
					else if (dif.X < -12f)
					{
						c[ID].charKeys.keyLeft = true;
					}
					else
					{
						c[ID].traj.X = 0f;
					}
					if (c[ID].state == 1)
					{
						if (dif.X > 60f || dif.X < -60f)
						{
							c[ID].charKeys.keyJump = true;
							c[ID].charKeys.jumpPower = 0.5f;
							if (dif.X > 70f || dif.X < -70f)
							{
								c[ID].charKeys.jumpPower = 0.9f;
							}
							if (dif.Y > 0f)
							{
								c[ID].charKeys.jumpPower = 0.3f;
								if (dif.X > 70f || dif.X < -70f)
								{
									c[ID].charKeys.jumpPower = 0.75f;
								}
							}
							if (dif.X < 0f)
							{
								c[ID].traj.X = -250f;
							}
							if (dif.X > 0f)
							{
								c[ID].traj.X = 250f;
							}
						}
						else
						{
							flag2 = true;
						}
					}
					else if ((!(dif.X < 30f) || !(dif.X > -30f)) && dif.Y < 0f)
					{
						c[ID].charKeys.keyUp = true;
					}
					break;
				case 2:
					if (dif.Y < 0f)
					{
						if (c[ID].state == 4)
						{
							hasTrail = false;
						}
						if (c[ID].state == 1)
						{
							if (dif.X < 32f && dif.X > -32f && dif.Y < -128f)
							{
								hasTrail = false;
							}
							float num11 = 10f;
							if (dif.X <= num11 && dif.X > -60f)
							{
								c[ID].charKeys.keyRight = true;
							}
							else if (dif.X >= 0f - num11 && dif.X < 60f)
							{
								c[ID].charKeys.keyLeft = true;
							}
							else if (dif.X > 0f - num11 && dif.X < num11)
							{
								flag2 = true;
							}
							else
							{
								c[ID].charKeys.jumpPower = 1f;
								c[ID].charKeys.keyJump = true;
								switch (c[ID].bodySec[0].animName)
								{
								case "idlew":
								case "idlem":
								case "idles":
								case "idlea":
								case "idler":
								case "idlex":
								case "runw":
								case "runm":
								case "runs":
								case "runa":
								case "runr":
								case "runx":
									climbAttempts++;
									break;
								}
								c[ID].traj.X = 0f;
							}
						}
						else if (c[ID].state == 0)
						{
							if (c[ID].traj.Y > -100f)
							{
								if (dif.X > 12f)
								{
									c[ID].charKeys.keyUp = true;
									c[ID].charKeys.keyRight = true;
								}
								else if (dif.X < -12f)
								{
									c[ID].charKeys.keyUp = true;
									c[ID].charKeys.keyLeft = true;
								}
								else
								{
									c[ID].traj.X = 0f;
								}
							}
						}
						else if (c[ID].state == 2 || c[ID].state == 3)
						{
							c[ID].charKeys.keyUp = true;
							if (GameState.gameType == 4 && c[ID].team == 1)
							{
								c[ID].charKeys.keyFloat = true;
							}
						}
					}
					else if (c[ID].state != 4)
					{
						if (dif.X > -10f)
						{
							c[ID].charKeys.keyRight = true;
						}
						else if (dif.X < 10f)
						{
							c[ID].charKeys.keyLeft = true;
						}
						else
						{
							c[ID].traj.X = 0f;
						}
					}
					if (climbAttempts >= 3)
					{
						hasTrail = false;
					}
					break;
				case 3:
					if (dif.X > 12f)
					{
						c[ID].charKeys.keyRight = true;
					}
					else if (dif.X < -12f)
					{
						c[ID].charKeys.keyLeft = true;
					}
					else
					{
						c[ID].traj.X = 0f;
					}
					break;
				}
				if (flag2)
				{
					stuckFrame += Game1.frameTime;
				}
				else
				{
					stuckFrame = 0f;
				}
				if (stuckFrame > 1f)
				{
					hasTrail = false;
				}
				else
				{
					maxNodeTime += Game1.frameTime;
					CheckMovement(c, map, dif, node2, vector);
				}
				CounterMeasures(c);
				c[ID].charKeys.runVec = new Vector2(1f, 1f);
				c[ID].charKeys.runSpeed = 1f;
			}
			if (c[ID].charKeys.keyUp)
			{
				c[ID].charKeys.keyJetpack = true;
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.ToString());
		}
	}

	private void CheckMovement(Character[] c, GameMap map, Vector2 dif, Node node, Vector2 goal)
	{
		int num = (int)(c[ID].loc.X / 64f);
		int num2 = (int)(c[ID].loc.Y / 32f);
		if (trailNode < trail.trailLen && num < 256 && num2 < 256 && num2 >= 0 && num >= 0 && map.node[num, num2] > -1 && trail.trail[trailNode + 1] == map.node[num, num2])
		{
			trailNode++;
			node = Game1.nodeMgr.node[trail.trail[trailNode]];
			goal = new Vector2((float)node.x * 64f + 32f, (float)node.y * 32f + 32f);
			dif = goal - c[ID].loc;
		}
		float num3 = 24f;
		if (!(dif.X >= 0f - num3) || !(dif.X <= num3) || !(dif.Y <= 24f) || !(dif.Y > -24f) || c[ID].state != 1)
		{
			return;
		}
		if (GameState.gameType == 3 && trailNode > -1)
		{
			for (int i = 0; i < Game1.gameMap.entityCount; i++)
			{
				if (trailNode > -1 && map.entity[i].node == trail.trail[trailNode] && map.entity[i].type == 6)
				{
					trailNode--;
				}
			}
		}
		trailNode++;
		maxNode = trailNode;
		maxNodeTime = 0f;
		climbAttempts = 0;
		if (trailNode >= trail.trailLen)
		{
			hasTrail = false;
		}
	}

	private void CounterMeasures(Character[] c)
	{
		if (c[ID].charKeys.shootVec.X == 0f && c[ID].charKeys.shootVec.Y == 0f && c[ID].state == 1)
		{
			if (c[ID].poison > 0f)
			{
				c[ID].charKeys.keySquat = true;
			}
			if (c[ID].fire > 0f)
			{
				c[ID].charKeys.keyRoll = true;
			}
		}
	}

	private void GetTrail(Character[] c, GameMap map)
	{
		if (c[ID].lastNode <= -1)
		{
			return;
		}
		int num = map.GetWeapEntity();
		if (num > -1)
		{
			num = map.entity[num].node;
		}
		if (num == -1)
		{
			num = Rand.GetRandomInt(0, Game1.nodeMgr.maxNodes);
		}
		byte b = 0;
		if (GameState.gameType == 2 && (Game1.netSession.redFlagState == ID || Game1.netSession.blueFlagState == ID))
		{
			b = (byte)((c[ID].GetTeam() != 1) ? 5 : 4);
			if (b > 0)
			{
				for (int i = 0; i < map.entityCount; i++)
				{
					if (map.entity[i].type == b && map.entity[i].node > -1)
					{
						num = map.entity[i].node;
					}
				}
			}
			GetTrail(c[ID].lastNode, num);
			return;
		}
		if (trailCount % 2 == 0)
		{
			int randomInt = Rand.GetRandomInt(0, Game1.gameMap.entityCount);
			if (Game1.gameMap.entity[randomInt] != null && Game1.gameMap.entity[randomInt].type > 16)
			{
				num = Game1.gameMap.entity[randomInt].node;
			}
			GetTrail(c[ID].lastNode, num);
			return;
		}
		if (GameState.gameType == 3)
		{
			for (int j = 0; j < map.entityCount; j++)
			{
				if (map.entity[j].type == 6 && map.entity[j].node > -1)
				{
					num = map.entity[j].node;
				}
			}
		}
		else if (GameState.gameType == 2 && Rand.CointToss(0.8f))
		{
			if (c[ID].GetTeam() == 1)
			{
				if (Game1.netSession.redFlagState == 200)
				{
					b = 5;
				}
			}
			else if (Game1.netSession.blueFlagState == 200)
			{
				b = 4;
			}
			if (b > 0)
			{
				for (int k = 0; k < map.entityCount; k++)
				{
					if (map.entity[k].type == b && map.entity[k].node > -1)
					{
						num = map.entity[k].node;
					}
				}
			}
		}
		GetTrail(c[ID].lastNode, num);
	}

	internal void RedFlag()
	{
	}

	internal void KillTrail()
	{
		hasTrail = false;
	}

	internal void HitWall()
	{
		hitWall = true;
	}

	internal void ResetHitWall()
	{
		hitWall = false;
	}
}
