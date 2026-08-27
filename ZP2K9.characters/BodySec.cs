using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using xCharEdit.Character;
using ZP2K9.characters.weapons;
using ZP2K9.particles;

namespace ZP2K9.characters;

public class BodySec
{
	public const int END_NONE = 0;

	public const int END_IDLE = 1;

	public const int END_JUMP = 2;

	public const int END_GETUP = 3;

	public const int END_FLY = 4;

	public const int END_DIE = 5;

	public float curFrame;

	public int anim;

	public int key;

	public int ID;

	private Vector2 torsoVec;

	public int endAction;

	public string animName;

	public BodySec(int ID)
	{
		this.ID = ID;
	}

	public void SetAnimNameFromInt(Character c)
	{
		animName = Game1.charDef[c.defIdx].GetAnimation(anim).name;
	}

	public void Update(Character c, float fTime)
	{
		Animation animation = Game1.charDef[c.defIdx].GetAnimation(anim);
		KeyFrame keyFrame = animation.GetKeyFrame(key);
		float num = fTime;
		if (c.freeze > 0f || c.rainbowed > 0f)
		{
			num /= 5f;
		}
		bool flag = false;
		if (GameState.gameType == 4 && c.team == 1)
		{
			flag = true;
		}
		if (ID == 0)
		{
			if ((c.suit == 2 || flag) && c.charKeys.keyFloat)
			{
				switch (animName)
				{
				case "idlew":
				case "idlem":
				case "idles":
				case "idlea":
				case "idler":
				case "runw":
				case "runm":
				case "runs":
				case "runa":
				case "runr":
				case "runx":
					num *= 2f;
					break;
				}
			}
			if (c.perk[0] == 9)
			{
				switch (animName)
				{
				case "cart":
				case "roll":
				case "rollx":
					num *= 2f;
					break;
				}
			}
		}
		curFrame += num * 30f;
		int num2 = key;
		if (curFrame > (float)keyFrame.duration)
		{
			if (key != 0)
			{
				CheckTrig(c);
			}
			curFrame -= keyFrame.duration;
			key++;
			keyFrame = animation.GetKeyFrame(key);
			if (key >= animation.getKeyFrameArray().Length)
			{
				key = 0;
			}
		}
		if (keyFrame.frameRef >= 0)
		{
			return;
		}
		key = 0;
		if (ID == 1)
		{
			c.splitAnim = false;
		}
		switch (endAction)
		{
		case 1:
			SetAnim(c.GetAnimName(0), c);
			switch (c.state)
			{
			case 1:
				c.angle = 0f;
				break;
			case 2:
				c.angle = 1.57f;
				break;
			case 3:
				c.angle = 4.71f;
				break;
			case 4:
				c.angle = 3.14f;
				break;
			}
			break;
		case 4:
			SetAnim(c.GetAnimName(2), c);
			break;
		case 2:
			SetAnim(c.GetAnimName(2), c);
			switch (c.state)
			{
			case 2:
				c.traj.X = 300f;
				if (c.suit == 2)
				{
					c.traj *= 1.5f;
				}
				c.angle -= 6.28f;
				break;
			case 3:
				c.traj.X = -300f;
				if (c.suit == 2)
				{
					c.traj *= 1.5f;
				}
				c.angle += 6.28f;
				break;
			case 4:
				c.traj.Y = 400f;
				break;
			default:
				if (c.charKeys.jumpPower < 0.3f)
				{
					c.charKeys.jumpPower = 1f;
				}
				if (c.submerged)
				{
					c.charKeys.jumpPower = 1f;
				}
				c.traj.Y = -590f * c.charKeys.jumpPower;
				if (c.suit == 2)
				{
					c.traj *= 1.2f;
				}
				break;
			}
			c.state = 0;
			break;
		case 3:
			if (c.hp < 0)
			{
				key = num2;
				if (c.dyingFrame <= 0f)
				{
					c.dyingFrame = 1f;
				}
			}
			else
			{
				SetAnim(c.GetAnimName(8), c);
				endAction = 1;
			}
			if (c.ai != null)
			{
				c.ai.KillTrail();
			}
			break;
		case 5:
			c.hp = -1;
			c.lastHitBy = c.ID;
			break;
		}
	}

	public void SetAnim(string anim, Character c)
	{
		SetAnim(anim, c, overRide: false);
	}

	public void SetAnim(string anim, Character c, bool overRide)
	{
		for (int i = 0; i < Game1.charDef[c.defIdx].GetAnimationArray().Length; i++)
		{
			if (Game1.charDef[c.defIdx].GetAnimation(i).name == anim && (this.anim != i || overRide))
			{
				animName = anim;
				endAction = 0;
				this.anim = i;
				key = 0;
				curFrame = 0f;
				break;
			}
		}
	}

	public void CheckTrig(Character c)
	{
		if (ID == 1)
		{
			if (c.splitAnim)
			{
				CheckPartTrig(0, 0, all: false, c);
				CheckPartTrig(1, 1, all: false, c);
			}
		}
		else
		{
			CheckPartTrig(0, 0, all: true, c);
		}
	}

	public void CheckPartTrig(int ps, int sec, bool all, Character c)
	{
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		int frameRef = Game1.charDef[c.defIdx].GetAnimation(c.bodySec[sec].anim).GetKeyFrame(c.bodySec[sec].key).frameRef;
		Frame frame = Game1.charDef[c.defIdx].GetFrame(frameRef);
		Vector2 vector = default(Vector2);
		Vector2 loc = c.loc;
		float scale = c.scale;
		loc += new Vector2((float)Math.Cos(c.angle + 1.57f), (float)Math.Sin(c.angle + 1.57f)) * scale * 60f - new Vector2(0f, 60f) * scale;
		if (ps == 1 && sec == 1)
		{
			for (int i = 0; i < frame.GetPartArray().Length; i++)
			{
				Part part = frame.GetPart(i);
				if (part.idx > -1)
				{
					float num = part.rotation;
					Vector2 vector2 = part.location * scale + loc;
					_ = part.scaling * scale;
					if ((c.face != 0 || part.flip != 0) && c.face == 1)
					{
						_ = part.flip;
					}
					if (c.face == 1)
					{
						num = 0f - num;
						vector2.X -= part.location.X * scale * 2f;
					}
					vector2 -= new Vector2((float)Math.Sin(num), (float)Math.Cos(num)) * scale * 24f;
					if (part.idx == 8 || part.idx == 9)
					{
						vector = torsoVec - vector2;
					}
				}
			}
		}
		for (int j = 0; j < frame.GetPartArray().Length; j++)
		{
			Part part2 = frame.GetPart(j);
			if (part2.idx <= -1)
			{
				continue;
			}
			float num2 = part2.rotation;
			Vector2 loc2 = part2.location * scale + loc;
			_ = part2.scaling * scale;
			bool flag = false;
			if ((c.face == 0 && part2.flip == 0) || (c.face == 1 && part2.flip == 1))
			{
				flag = true;
			}
			if (c.face == 1)
			{
				num2 = 0f - num2;
				loc2.X -= part2.location.X * scale * 2f;
			}
			new Color(new Vector4(1f, 1f, 1f, 1f));
			bool flag2 = false;
			if (ps == 0)
			{
				if (part2.idx >= 24 && part2.idx / 64 == 0)
				{
					flag2 = true;
				}
				if (part2.idx == 8 || part2.idx == 9)
				{
					torsoVec = loc2;
					torsoVec -= new Vector2((float)Math.Sin(num2), (float)Math.Cos(num2)) * scale * 24f;
				}
			}
			else
			{
				if (part2.idx < 24 || part2.idx / 64 != 0)
				{
					flag2 = true;
				}
				loc2 += vector;
			}
			if (all)
			{
				flag2 = true;
			}
			if (!flag2 || part2.idx < 1000)
			{
				continue;
			}
			float num3 = part2.rotation;
			if (!flag)
			{
				num3 = 3.14f - num3;
			}
			loc2 = Character.GetAngleAdjustedVec(loc, loc2, c.angle);
			num3 += c.angle;
			Vector2 vector3 = new Vector2((float)Math.Cos(num3), (float)Math.Sin(num3));
			vector3.Normalize();
			switch (part2.idx)
			{
			case 1000:
			case 1005:
			{
				Vector2 vector5 = new Vector2(c.charKeys.shootVec.X, c.charKeys.shootVec.Y);
				if (part2.idx - 1000 == 5)
				{
					vector5 = new Vector2((c.face == 1) ? 1f : (-1f), 0.2f);
				}
				Vector2 vector6 = (vector3 + vector5) / 2f;
				if (part2.idx - 1000 == 5)
				{
					for (int k = 0; k < 10; k++)
					{
						Game1.pMan.AddParticle(22, loc2 + vector6 * (k * 3), vector6 * 100f, (1f - (float)k * 0.07f) * 0.5f, 0, 0);
					}
				}
				else
				{
					switch (WeaponCatalog.weapons[c.weapon[c.curWeap]].projType)
					{
					case 2:
					{
						for (int m = 0; m < 9; m++)
						{
							Game1.pMan.AddParticle(22, loc2 + vector6 * (m * 3), vector6 * 100f, (1f - (float)m * 0.07f) * 0.35f, 0, 0);
						}
						break;
					}
					case 0:
					case 1:
					case 19:
					{
						for (int num5 = 0; num5 < 10; num5++)
						{
							Game1.pMan.AddParticle(22, loc2 + vector6 * (num5 * 3), vector6 * 100f, (1f - (float)num5 * 0.07f) * 0.5f, 0, 0);
						}
						break;
					}
					case 3:
					{
						for (int n = 0; n < 10; n++)
						{
							Game1.pMan.AddParticle(24, loc2 + vector6 * (n * 3), vector6 * 100f, (1f - (float)n * 0.07f) * 0.5f, 0, 0);
						}
						Game1.pMan.AddParticle(17, loc2, vector6 * 50f, 0f, 0, 0);
						break;
					}
					case 7:
					case 14:
					{
						for (int l = 0; l < 5; l++)
						{
							Game1.pMan.AddParticle(38, loc2, vector6 * (50f + (float)l * 30f), 0f, 0, 0);
						}
						break;
					}
					}
				}
				for (int num6 = 0; num6 < WeaponCatalog.weapons[c.weapon[c.curWeap]].burst; num6++)
				{
					vector5.Normalize();
					Vector2 vector7 = vector5 * 2000f + Rand.GetRandomVec2(0f - WeaponCatalog.weapons[c.weapon[c.curWeap]].spread, WeaponCatalog.weapons[c.weapon[c.curWeap]].spread, 0f - WeaponCatalog.weapons[c.weapon[c.curWeap]].spread, WeaponCatalog.weapons[c.weapon[c.curWeap]].spread);
					if (part2.idx - 1000 == 5)
					{
						if (c.hp > -1)
						{
							Sound.PlayCue("pistol");
							c.lastHitBy = -1;
							c.hp = -1;
							if (c.face == 1)
							{
								c.face = 0;
								c.traj.X = 200f;
							}
							else
							{
								c.face = 1;
								c.traj.X = -200f;
							}
							KillManager.DoKill(c.ID, c.ID, 1);
							for (int num7 = 0; num7 < 6; num7++)
							{
								Game1.pMan.AddParticle(6, loc2, vector7 * Rand.GetRandomFloat(-0.2f, 0f), Rand.GetRandomFloat(0.8f, 1.2f), 0, 0);
								Game1.pMan.AddParticle(7, loc2, vector7 * Rand.GetRandomFloat(0.1f, 1f), Rand.GetRandomFloat(0f, 1.5f), 0, 0);
							}
						}
						continue;
					}
					switch (WeaponCatalog.weapons[c.weapon[c.curWeap]].projType)
					{
					case 12:
						if (c.charge < 1f)
						{
							Game1.pMan.AddParticle(49, loc2 + (c.drawVec - c.loc), Rand.GetRandomVec2(2f), 0.1f * c.charge, 0, c.ID);
							break;
						}
						if ((loc2 - Scroll.scroll).Length() < 300f)
						{
							Quake.SetQuake(0.3f);
						}
						Game1.pMan.AddParticle(47, loc2, vector7, WeaponCatalog.weapons[c.weapon[c.curWeap]].splash, WeaponCatalog.weapons[c.weapon[c.curWeap]].damage, c.ID);
						break;
					case 2:
						Game1.pMan.AddParticle(21, loc2, vector7, 0f, WeaponCatalog.weapons[c.weapon[c.curWeap]].damage, c.ID);
						break;
					case 0:
						Game1.pMan.AddParticle(19, loc2, vector7, 0f, WeaponCatalog.weapons[c.weapon[c.curWeap]].damage, c.ID);
						break;
					case 1:
						Game1.pMan.AddParticle(27, loc2, vector7, 0f, WeaponCatalog.weapons[c.weapon[c.curWeap]].damage, c.ID);
						break;
					case 3:
						Game1.pMan.AddParticle(23, loc2, vector7 * 0.5f, 0f, WeaponCatalog.weapons[c.weapon[c.curWeap]].damage, c.ID);
						break;
					case 19:
						Game1.pMan.AddParticle(67, loc2, vector7 * 0.25f, 0f, WeaponCatalog.weapons[c.weapon[c.curWeap]].damage, c.ID);
						break;
					case 13:
						Game1.pMan.AddParticle(52, loc2, vector7 * 0.25f, 0f, WeaponCatalog.weapons[c.weapon[c.curWeap]].damage, c.ID);
						break;
					case 14:
						Game1.pMan.AddParticle(54, loc2, vector7 * 0.65f, 0f, WeaponCatalog.weapons[c.weapon[c.curWeap]].damage, c.ID);
						break;
					case 4:
						Game1.pMan.AddParticle(25, loc2, vector7 * 0.5f, WeaponCatalog.weapons[c.weapon[c.curWeap]].splash, WeaponCatalog.weapons[c.weapon[c.curWeap]].damage, c.ID);
						break;
					case 17:
						Game1.pMan.AddParticle(64, loc2, vector7 * 0.5f, WeaponCatalog.weapons[c.weapon[c.curWeap]].splash, WeaponCatalog.weapons[c.weapon[c.curWeap]].damage, c.ID);
						break;
					case 5:
						Game1.pMan.AddParticle(10, loc2, vector7 * 0.5f, WeaponCatalog.weapons[c.weapon[c.curWeap]].splash, WeaponCatalog.weapons[c.weapon[c.curWeap]].damage, c.ID);
						break;
					case 9:
						Game1.pMan.AddParticle(34, loc2, vector7 * 0.4f, 0f, WeaponCatalog.weapons[c.weapon[c.curWeap]].damage, c.ID);
						break;
					case 11:
						Game1.pMan.AddParticle(45, loc2, vector7 * 0.4f, 0f, WeaponCatalog.weapons[c.weapon[c.curWeap]].damage, c.ID);
						break;
					case 6:
						Game1.pMan.AddParticle(31, loc2, vector7 * 0.5f, 0f, WeaponCatalog.weapons[c.weapon[c.curWeap]].damage, c.ID);
						break;
					case 7:
						Game1.pMan.AddParticle(33, loc2, vector7 * 0.4f, 0f, WeaponCatalog.weapons[c.weapon[c.curWeap]].damage, c.ID);
						break;
					case 8:
						Game1.pMan.AddParticle(35, loc2, vector7 * 0.5f, WeaponCatalog.weapons[c.weapon[c.curWeap]].splash, WeaponCatalog.weapons[c.weapon[c.curWeap]].damage, c.ID);
						break;
					case 15:
						Game1.pMan.AddParticle(55, loc2, vector7 * 0.5f, WeaponCatalog.weapons[c.weapon[c.curWeap]].splash, WeaponCatalog.weapons[c.weapon[c.curWeap]].damage, c.ID);
						break;
					case 10:
						Game1.pMan.AddParticle(44, loc2, vector7 * 0.3f + c.traj * 1.5f, 0f, WeaponCatalog.weapons[c.weapon[c.curWeap]].damage, c.ID);
						break;
					case 16:
						Game1.pMan.AddParticle(65, loc2, vector7 * 0.3f + c.traj * 1.5f, 0f, WeaponCatalog.weapons[c.weapon[c.curWeap]].damage, c.ID);
						break;
					}
				}
				if (WeaponCatalog.weapons[c.weapon[c.curWeap]].fireRate <= 0.03f)
				{
					key += Rand.GetRandomInt(1, 3);
				}
				break;
			}
			case 1004:
			{
				Vector2 vector4 = vector3;
				vector4.Normalize();
				Game1.pMan.AddParticle(20, loc2, vector4 * 1000f, 0f, 15, c.ID);
				if ((c.loc - Scroll.scroll).Length() < 500f)
				{
					Sound.PlayCue("swing");
				}
				break;
			}
			case 1001:
			case 1006:
				if (WeaponCatalog.weapons[c.weapon[c.curWeap]].ammoType == 0 || WeaponCatalog.weapons[c.weapon[c.curWeap]].ammoType == 1 || part2.idx - 1000 == 6)
				{
					Game1.pMan.AddParticle(18, loc2, vector3 * 200f, 0f, 0, 0);
				}
				break;
			case 1002:
				Game1.pMan.AddParticle(26, loc2, vector3 * 200f, 0f, 0, 0);
				break;
			case 1003:
			{
				vector3 = ((!(c.charKeys.shootVec.Length() > 0.6f)) ? c.grenVec : c.charKeys.shootVec);
				float num4 = vector3.Length();
				vector3.Normalize();
				if ((c.loc - Scroll.scroll).LengthSquared() < 250000f)
				{
					Sound.PlayCue("throw");
				}
				if (c.grenAmmo[c.lastGren] > 0)
				{
					c.grenAmmo[c.lastGren]--;
					byte b = (byte)c.grenType[c.lastGren];
					if (c.perk[1] == 7)
					{
						vector3 *= 1.1f;
					}
					switch (b)
					{
					case 33:
						Game1.pMan.AddParticle(11, loc2, vector3 * 1000f * num4, 0f, 0, c.ID);
						break;
					case 34:
						Game1.pMan.AddParticle(9, loc2, vector3 * 1000f * num4, 0f, 0, c.ID);
						break;
					case 35:
						Game1.pMan.AddParticle(14, loc2, vector3 * 1000f * num4, 0f, 0, c.ID);
						break;
					case 37:
						Game1.pMan.AddParticle(12, loc2, vector3 * 1000f * num4, 0f, 0, c.ID);
						break;
					case 36:
						Game1.pMan.AddParticle(13, loc2, vector3 * 1000f * num4, 0f, 0, c.ID);
						break;
					case 38:
						Game1.pMan.AddParticle(29, loc2, vector3 * 1000f * num4, 0f, 0, c.ID);
						break;
					case 41:
						Game1.pMan.AddParticle(59, loc2, vector3 * 1000f * num4, 0f, 0, c.ID);
						break;
					case 42:
						Game1.pMan.AddParticle(60, loc2, vector3 * 1000f * num4, 0f, 0, c.ID);
						break;
					case 43:
						Game1.pMan.AddParticle(61, loc2, vector3 * 1000f * num4, 0f, 0, c.ID);
						break;
					case 44:
						Game1.pMan.AddParticle(62, loc2, vector3 * 1000f * num4, 0f, 0, c.ID);
						break;
					}
					c.SortGrenades();
				}
				break;
			}
			}
		}
	}
}
