using Microsoft.Xna.Framework;
using ZP2K9.characters;
using ZP2K9.hud.messageHud;
using ZP2K9.particles;

namespace ZP2K9.map;

public class Entity
{
	public const int CUE_REPLACE = 0;

	public const int CUE_PICKUP_AMMO = 1;

	public const int CUE_NEW_WEAPON = 2;

	public const int CUE_NOPICKUP = 3;

	public const int CUE_GOODIES = 4;

	public int x;

	public int y;

	public byte type;

	public float frame;

	public float respawn;

	public int node = -1;

	public bool exists;

	public Entity()
	{
		exists = false;
	}

	public Entity(int x, int y, byte type)
	{
		this.x = x;
		this.y = y;
		this.type = type;
		exists = true;
	}

	public void Init(int x, int y, byte type)
	{
		this.x = x;
		this.y = y;
		this.type = type;
		exists = true;
		frame = 0f;
		respawn = 0f;
		node = -1;
	}

	public byte GetAdjustedType()
	{
		if (type < 17)
		{
			return type;
		}
		int num = x + y;
		switch (Game1.netSession.mutator)
		{
		case 15:
			switch (type)
			{
			case 28:
				return 33;
			case 40:
				return 42;
			case 29:
				return 36;
			case 46:
				return 34;
			}
			break;
		case 7:
			switch (num % 9)
			{
			case 0:
			case 8:
				return 23;
			case 1:
			case 7:
				return 22;
			case 2:
			case 6:
				return 33;
			case 3:
			case 5:
				return 36;
			case 4:
				return 43;
			}
			break;
		case 9:
			switch (num % 6)
			{
			case 0:
				return 25;
			case 1:
				return 27;
			case 2:
				return 49;
			case 3:
				return 33;
			case 4:
				return 34;
			case 5:
				return 46;
			}
			break;
		case 5:
			switch (num % 10)
			{
			case 0:
				return 34;
			case 1:
				return 38;
			case 2:
				return 35;
			case 3:
				return 37;
			case 4:
				return 36;
			case 5:
				return 33;
			case 6:
				return 43;
			case 7:
				return 41;
			case 8:
				return 44;
			case 9:
				return 42;
			}
			break;
		case 4:
			switch (num % 6)
			{
			case 0:
				return 47;
			case 1:
				return 45;
			case 2:
				return 19;
			case 3:
				return 33;
			case 4:
				return 18;
			case 5:
				return 20;
			}
			break;
		case 10:
			return 30;
		case 2:
			switch (num % 10)
			{
			case 0:
				return 17;
			case 1:
				return 20;
			case 2:
				return 19;
			case 3:
				return 18;
			case 4:
				return 23;
			case 5:
				return 22;
			case 6:
				return 33;
			case 7:
				return 27;
			case 8:
				return 29;
			case 9:
				return 34;
			}
			break;
		case 8:
			switch (num % 7)
			{
			case 0:
				return 30;
			case 1:
				return 21;
			case 2:
				return 42;
			case 3:
				return 40;
			case 4:
				return 33;
			case 5:
				return 26;
			case 6:
				return 38;
			}
			break;
		case 6:
			switch (type)
			{
			case 33:
			case 34:
			case 35:
			case 36:
			case 37:
			case 38:
			case 41:
			case 42:
			case 43:
			case 44:
				return 43;
			default:
				return type;
			}
		case 3:
			switch (num % 4)
			{
			case 0:
				return 29;
			case 1:
				return 28;
			case 2:
				return 40;
			case 3:
				return 46;
			}
			break;
		case 11:
			switch (num % 11)
			{
			case 0:
				return 24;
			case 1:
				return 30;
			case 2:
				return 31;
			case 3:
				return 32;
			case 4:
				return 49;
			case 5:
				return 41;
			case 6:
				return 44;
			case 7:
				return 42;
			case 8:
				return 43;
			case 9:
				return 40;
			case 10:
				return 35;
			}
			break;
		case 12:
			switch (type)
			{
			case 33:
			case 34:
			case 35:
			case 36:
			case 37:
			case 38:
			case 41:
			case 42:
			case 43:
			case 44:
				return 41;
			default:
				return type;
			}
		case 13:
			switch (num % 10)
			{
			case 0:
				return 48;
			case 1:
				return 39;
			case 2:
				return 24;
			case 3:
				return 32;
			case 4:
				return 49;
			case 5:
				return 41;
			case 6:
				return 44;
			case 7:
				return 42;
			case 8:
				return 43;
			case 9:
				return 31;
			}
			break;
		}
		return type;
	}

	public void Update()
	{
		if (respawn > 0f)
		{
			respawn -= Game1.frameTime;
			if (!(respawn <= 0f))
			{
			}
		}
		else if (type == 6)
		{
			float num = frame;
			frame += Game1.frameTime;
			if (frame > 12f)
			{
				frame -= 12f;
			}
			if (GameState.gameType != 3 || (int)(num * 15f) == (int)(frame * 15f))
			{
				return;
			}
			Game1.pMan.AddParticle(36, new Vector2((float)x * 64f + 32f, (float)y * 32f + 16f) + Rand.GetRandomVec2(-128f, 128f, -64f, 0f), new Vector2(0f, -32f), 0.4f, 0, 0);
			Game1.pMan.AddParticle(41, new Vector2((float)x * 64f + 32f, (float)y * 32f + 16f) + Rand.GetRandomVec2(-128f, 128f, -64f, 0f), new Vector2(0f, -32f), 0.4f, 0, 0);
			bool flag = false;
			bool flag2 = false;
			Game1.netSession.hillState = 0;
			for (int i = 0; i < Game1.character.Length; i++)
			{
				if (Game1.character[i] == null || Game1.character[i].hp < 0)
				{
					continue;
				}
				int num2 = (int)(Game1.character[i].loc.X / 64f);
				int num3 = (int)(Game1.character[i].loc.Y / 32f);
				if (num2 >= x - 5 && num2 <= x + 5 && num3 >= y - 4 && num3 <= y + 3)
				{
					if (Game1.character[i].GetTeam() == 1)
					{
						flag2 = true;
					}
					else
					{
						flag = true;
					}
				}
			}
			if ((flag2 && !flag) || (flag && !flag2))
			{
				if (flag2)
				{
					Game1.netSession.hillState = 1;
				}
				else
				{
					Game1.netSession.hillState = 2;
				}
				if (Game1.netSession.IsHost())
				{
					if (flag2)
					{
						Game1.netSession.blueTime += 1f / 15f;
					}
					else
					{
						Game1.netSession.redTime += 1f / 15f;
					}
				}
				if ((int)(num * 0.25f) == (int)(frame * 0.25f))
				{
					return;
				}
				for (int j = 0; j < Game1.character.Length; j++)
				{
					if (Game1.character[j] == null || Game1.character[j].hp < 0)
					{
						continue;
					}
					int num4 = (int)(Game1.character[j].loc.X / 64f);
					int num5 = (int)(Game1.character[j].loc.Y / 32f);
					if (num4 >= x - 3 && num4 <= x + 3 && num5 >= y - 3 && num5 <= y + 2)
					{
						if (Game1.netSession.GetPlayerOne() == j)
						{
							Game1.hud.AddPopScore(10);
						}
						Game1.character[j].AddScore(10);
					}
				}
			}
			else
			{
				frame = 0f;
			}
		}
		else
		{
			frame += Game1.frameTime;
			if (frame > 6.28f)
			{
				frame -= 6.28f;
			}
		}
	}

	private int GetPickupCue(Character c)
	{
		int result = 0;
		if (GameState.gameType == 4 && c.team == 1)
		{
			return 3;
		}
		int adjustedType = GetAdjustedType();
		switch (adjustedType)
		{
		case 17:
		case 18:
		case 19:
		case 20:
		case 21:
		case 22:
		case 23:
		case 24:
		case 25:
		case 26:
		case 27:
		case 28:
		case 29:
		case 30:
		case 31:
		case 32:
		case 39:
		case 40:
		case 45:
		case 46:
		case 47:
		case 48:
		case 49:
			result = c.GetHasWeapon(adjustedType);
			break;
		case 33:
		case 34:
		case 35:
		case 36:
		case 37:
		case 38:
		case 41:
		case 42:
		case 43:
		case 44:
			result = c.CanPickupGren(type);
			break;
		}
		return result;
	}

	public void Pickup(Character c)
	{
		if (respawn > 0f || c.hp < 0 || !exists)
		{
			return;
		}
		switch (type)
		{
		default:
			if (c.charKeys.KeyPickup())
			{
				break;
			}
			if (Game1.netSession.GetPlayerOne() == c.ID)
			{
				int pickupCue = GetPickupCue(c);
				if (pickupCue != 3)
				{
					Game1.hud.AddPickup(GetAdjustedType(), pickupCue);
				}
			}
			return;
		case 1:
		case 2:
		case 3:
		case 4:
		case 5:
		case 6:
			break;
		}
		bool flag = false;
		int adjustedType = GetAdjustedType();
		switch (adjustedType)
		{
		case 4:
		case 5:
		{
			if (GameState.gameType != 2)
			{
				break;
			}
			bool flag2 = false;
			bool flag3 = false;
			if (type == 4)
			{
				if (Game1.netSession.blueFlagState == 200)
				{
					if (c.GetTeam() == 2)
					{
						if (Game1.netSession.IsHost())
						{
							Game1.netSession.blueFlagState = c.ID;
							flag2 = true;
						}
					}
					else if (Game1.netSession.redFlagState == c.ID && c.capFixFrame <= 0f)
					{
						Game1.netSession.redFlagState = 200;
						if (Game1.netSession.GetPlayerOne() == c.ID)
						{
							Game1.hud.AddPopup("You captured the flag!", 50, 1f);
						}
						c.AddScore(50);
						flag3 = true;
						c.capFixFrame = 5f;
					}
				}
			}
			else if (type == 5 && Game1.netSession.redFlagState == 200)
			{
				if (c.GetTeam() == 1)
				{
					if (Game1.netSession.IsHost())
					{
						Game1.netSession.redFlagState = c.ID;
						flag2 = true;
					}
				}
				else if (Game1.netSession.blueFlagState == c.ID && c.capFixFrame <= 0f)
				{
					Game1.netSession.blueFlagState = 200;
					if (Game1.netSession.GetPlayerOne() == c.ID)
					{
						Game1.hud.AddPopup("You captured the flag!", 50, 1f);
					}
					c.AddScore(50);
					flag3 = true;
					c.capFixFrame = 5f;
				}
			}
			if ((!flag2 && !flag3) || !Game1.netSession.IsHost())
			{
				break;
			}
			Game1.hud.AddMessage(KillManager.GetPlayerName(c.ID), flag2 ? Message.msgGotFlag : Message.msgCappedFlag, c.GetTeam(), 0, -1);
			if (flag3)
			{
				if (type == 5)
				{
					Game1.netSession.redScore++;
				}
				else
				{
					Game1.netSession.blueScore++;
				}
			}
			break;
		}
		case 17:
		case 18:
		case 19:
		case 20:
		case 21:
		case 22:
		case 23:
		case 24:
		case 25:
		case 26:
		case 27:
		case 28:
		case 29:
		case 30:
		case 31:
		case 32:
		case 39:
		case 40:
		case 45:
		case 46:
		case 47:
		case 48:
		case 49:
			flag = c.Pickup(adjustedType);
			break;
		case 33:
			flag = c.PickupGren(adjustedType, 3);
			break;
		case 34:
			flag = c.PickupGren(adjustedType, 3);
			break;
		case 35:
			flag = c.PickupGren(adjustedType, 2);
			break;
		case 36:
			flag = c.PickupGren(adjustedType, 1);
			break;
		case 37:
			flag = c.PickupGren(adjustedType, 2);
			break;
		case 38:
			flag = c.PickupGren(adjustedType, 2);
			break;
		case 41:
			flag = c.PickupGren(adjustedType, 2);
			break;
		case 42:
			flag = c.PickupGren(adjustedType, 2);
			break;
		case 43:
			flag = c.PickupGren(adjustedType, 1);
			break;
		case 44:
			flag = c.PickupGren(adjustedType, 2);
			break;
		}
		if (flag)
		{
			if (Game1.netSession.GetPlayerOne() == c.ID)
			{
				Game1.hud.DoPickup(adjustedType);
			}
			if ((new Vector2((float)x * 64f, (float)y * 32f) - Scroll.scroll).Length() < 500f)
			{
				Sound.PlayCue("click2");
			}
			respawn = 15f;
		}
	}
}
