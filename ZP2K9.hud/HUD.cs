using System;
using System.Collections.ObjectModel;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Yuki_Win;
using ZP2K9.characters;
using ZP2K9.characters.weapons;
using ZP2K9.debug;
using ZP2K9.hud.messageHud;
using ZP2K9.store;

namespace ZP2K9.hud;

public class HUD
{
	private const float POPUP_SCORE_TIME = 1f;

	private MessageMgr messageMgr;

	public Scoreboard scoreBoard;

	private StringBuilder nilStr = new StringBuilder("-");

	private StringBuilder serverChangingSettingsStr = new StringBuilder("* Head's up! Host is changing settings *");

	private Popup popup;

	private Pickup pickup;

	private float ammoA;

	public float suitDescFrame;

	public int suitDescIdx = -1;

	private int pSuit;

	public int popScoreAdd;

	public float popScoreFrame;

	private StringBuilder popupScoreAddStr;

	public float red;

	private int pickupShowType;

	private int pickupShowCue;

	public StringBuilder deadString;

	private float frame;

	public float serverChangingSettingsFrame;

	private float nullCharCrashFrame;

	public void SetServerChangingSettings()
	{
		serverChangingSettingsFrame = 1f;
	}

	public HUD()
	{
		messageMgr = new MessageMgr();
		scoreBoard = new Scoreboard();
		popup = new Popup();
		pickup = new Pickup();
	}

	public bool IsPopupActive()
	{
		return popup.IsActive();
	}

	public void AddMessage(StringBuilder txt1, StringBuilder txt2, int team1, int team2, int kill)
	{
		messageMgr.AddMessage(txt1, txt2, team1, team2, kill);
	}

	public void AddPopup(string msg, int points, float duration)
	{
		popup.Add(msg, points, this, duration);
	}

	public void AddPopup(string msg, int unlockType, int unlockIdx, int level, float duration)
	{
		popup.Add(msg, unlockType, unlockIdx, level, this, duration);
	}

	public void Update(InterfaceKeys ikeys, Character c)
	{
		if (serverChangingSettingsFrame > 0f)
		{
			serverChangingSettingsFrame -= Game1.frameTime;
		}
		messageMgr.Update();
		scoreBoard.Update(ikeys);
		popup.Update(this);
		pickup.Update();
		if (popScoreFrame > 0f)
		{
			popScoreFrame -= Game1.frameTime * 0.9f;
		}
		pickupShowType = -1;
		frame += Game1.frameTime;
		if (frame > 1f)
		{
			frame--;
		}
		if (c != null)
		{
			if (c.weapon[c.curWeap] > -1)
			{
				try
				{
					int num = WeaponCatalog.weapons[c.weapon[c.curWeap]].maxClip;
					if (num > 1 && c.perk[2] == 7)
					{
						num *= 3;
					}
					float num2 = (float)c.magazine[c.curWeap] / (float)num;
					ammoA += (num2 - ammoA) * Game1.frameTime * 3f;
					if (c.suit != pSuit && c.suit > 0)
					{
						suitDescFrame = 5f;
						suitDescIdx = c.suit;
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.StackTrace);
				}
			}
			pSuit = c.suit;
			nullCharCrashFrame = 0f;
		}
		else
		{
			nullCharCrashFrame += Game1.frameTime;
			if (nullCharCrashFrame > 1f)
			{
				nullCharCrashFrame = 0f;
				Game1.netSession.NullCrash();
			}
		}
		if (suitDescFrame > 0f)
		{
			suitDescFrame -= Game1.frameTime;
		}
	}

	public void Draw(Character c, SpriteBatch sprite)
	{
		//IL_043c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Unknown result type (might be due to invalid IL or missing references)
		//IL_0647: Unknown result type (might be due to invalid IL or missing references)
		//IL_050d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0599: Unknown result type (might be due to invalid IL or missing references)
		//IL_0863: Unknown result type (might be due to invalid IL or missing references)
		//IL_0865: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0954: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b49: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b95: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b9a: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_071e: Unknown result type (might be due to invalid IL or missing references)
		//IL_077e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0807: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d0f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0db3: Unknown result type (might be due to invalid IL or missing references)
		//IL_112e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1197: Unknown result type (might be due to invalid IL or missing references)
		//IL_12d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e49: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ea9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ef5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e1c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e1e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1942: Unknown result type (might be due to invalid IL or missing references)
		//IL_19d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_1204: Unknown result type (might be due to invalid IL or missing references)
		//IL_137e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1aa3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a37: Unknown result type (might be due to invalid IL or missing references)
		//IL_170c: Unknown result type (might be due to invalid IL or missing references)
		//IL_151d: Unknown result type (might be due to invalid IL or missing references)
		//IL_157a: Unknown result type (might be due to invalid IL or missing references)
		//IL_13f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_1466: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b6b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1613: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f82: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fe6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		if (c == null || DebugManager.hideHud)
		{
			return;
		}
		bool flag = false;
		if (GameState.gameType == 4 && c.team == 1)
		{
			flag = true;
		}
		if (!Game1.netSession.postLobby && !Game1.menu.IsActive())
		{
			try
			{
				if (Game1.settings.showNames)
				{
					Color color = default(Color);
					for (int i = 0; i < Game1.character.Length; i++)
					{
						if (Game1.character[i] == null || i == c.ID || !(Game1.character[i].nameAlpha > 0f) || i >= scoreBoard.charName.Length || scoreBoard.charName[i] == null)
						{
							continue;
						}
						Vector2 loc = Scroll.GetLoc(Game1.character[i].drawVec);
						if (loc.X > 0f && loc.Y > 0f && loc.X < 1280f && loc.Y < 720f && Game1.character[i].hp >= 0)
						{
							color = new Color(new Vector4(1f, 1f, 1f, Game1.character[i].nameAlpha));
							switch (Game1.character[i].GetTeam())
							{
							case 1:
								color = new Color(new Vector4(0.5f, 0.5f, 1f, Game1.character[i].nameAlpha));
								break;
							case 2:
								color = new Color(new Vector4(1f, 0.5f, 0.5f, Game1.character[i].nameAlpha));
								break;
							}
							if (GameState.gameType == 4 && c.team == 0 && Game1.character[i].team == 1 && c.dyingFrame <= 0f)
							{
								color = new Color(0f, 0f, 0f, 0f);
							}
							Game1.text.color = color;
							Game1.text.size = 1f;
							Game1.text.DrawString(loc, scoreBoard.charName[i], 1, -1f, Game1.impact, sprite);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.StackTrace);
			}
			try
			{
				int num = 0;
				Color val = default(Color);
				for (int j = 0; j < (Game1.netSession.netSession.AllGamers).Count; j++)
				{
					if (!(Game1.netSession.netSession.AllGamers)[j].IsTalking)
					{
						continue;
					}
					byte id = (Game1.netSession.netSession.AllGamers)[j].Id;
					if (!Game1.netSession.playerList.ContainsKey(id))
					{
						continue;
					}
					int num2 = Game1.netSession.playerList[id];
					if (scoreBoard.charName[num2] != null && Game1.character[num2] != null)
					{
						val = new Color(new Vector4(1f, 1f, 1f, 1f));
						switch (Game1.character[num2].GetTeam())
						{
						case 1:
							val = new Color(new Vector4(0.5f, 0.5f, 1f, 1f));
							break;
						case 2:
							val = new Color(new Vector4(1f, 0.5f, 0.5f, 1f));
							break;
						}
						float num3 = 800f;
						Game1.text.color = val;
						Game1.text.size = 1f;
						Game1.text.DrawString(new Vector2(num3, 610f - (float)num * 27f), scoreBoard.charName[num2], 0, -1f, Game1.impact, sprite);
						sprite.Draw(Game1.spritesTex, new Vector2(num3 - 30f, 604f - (float)num * 27f), (Rectangle?)new Rectangle(864, 96, 32, 32), val);
						num++;
					}
				}
			}
			catch (Exception ex2)
			{
				Console.WriteLine(ex2.StackTrace);
			}
			Color white = Color.White;
			Vector2 loc2 = Scroll.GetLoc(c.loc - new Vector2(0f, 40f));
			float num4 = c.charKeys.shootVec.Length();
			if (num4 > 0.1f)
			{
				Vector2 shootVec = c.charKeys.shootVec;
				if (num4 > 1f)
				{
					num4 = 1f;
				}
				num4 = num4 / 2f + 0.5f;
				shootVec.Normalize();
				sprite.Draw(Game1.spritesTex, loc2 + shootVec * 220f * num4, (Rectangle?)new Rectangle(224, 32, 32, 32), new Color(new Vector4(0f, 0f, 0f, (num4 - 0.1f) / 2f)), Trig.GetAngle(default(Vector2), shootVec), new Vector2(16f, 16f), 0.3f, (SpriteEffects)1, 1f);
				sprite.Draw(Game1.spritesTex, loc2 + shootVec * 250f * num4, (Rectangle?)new Rectangle(224, 32, 32, 32), new Color(new Vector4(1f, 1f, 1f, (num4 - 0.1f) / 2f)), Trig.GetAngle(default(Vector2), shootVec), new Vector2(16f, 16f), 0.5f, (SpriteEffects)1, 1f);
			}
			switch (c.GetTeam())
			{
			case 1:
				white = new Color(new Vector4(0.5f, 0.5f, 1f, 1f));
				break;
			case 2:
				white = new Color(new Vector4(1f, 0.5f, 0.5f, 1f));
				break;
			}
			DrawPopScore(sprite);
			popup.Draw(sprite);
			pickup.Draw(sprite, white);
			if (c.hp < 0)
			{
				DrawYouDied(sprite);
			}
			if (pickupShowType > -1)
			{
				int num5 = pickupShowType - 1;
				Vector2 vector = new Vector2(640f, 440f);
				float num6 = ((pickupShowCue == 0) ? 32f : 0f);
				sprite.Draw(Game1.spritesTex, vector + new Vector2(0f - num6, 0f), (Rectangle?)new Rectangle(672, 192, 32, 32), Color.White, 0f, new Vector2(32f, 16f), 1f, (SpriteEffects)0, 1f);
				if (pickupShowCue == 0)
				{
					sprite.Draw(Game1.spritesTex, vector, (Rectangle?)new Rectangle(672, 224, 96, 64), Color.White, 0f, new Vector2(48f, 32f), 0.6f, (SpriteEffects)0, 1f);
				}
				if (pickupShowCue == 4)
				{
					sprite.Draw(Game1.spritesTex, vector + new Vector2(num6, 0f), (Rectangle?)new Rectangle(768, 32, 64, 64), Color.White, 0f, new Vector2(0f, 32f), 1f, (SpriteEffects)0, 1f);
				}
				else
				{
					sprite.Draw(Game1.spritesTex, vector + new Vector2(num6, 0f), (Rectangle?)((pickupShowCue == 1) ? new Rectangle(768, 225, 64, 64) : new Rectangle(num5 % 16 * 64, 320 + num5 / 16 * 64, 64, 64)), Color.White, 0f, new Vector2(0f, 32f), 1f, (SpriteEffects)0, 1f);
				}
			}
			Vector2 vector2 = new Vector2(160f, 570f);
			Vector2 vector3 = new Vector2(450f, 610f);
			Vector2 vector4 = new Vector2(1100f, 610f);
			Color val2 = white;
			val2.A = 80;
			Vector2 vector5 = new Vector2(1120f, 140f);
			sprite.Draw(Game1.spritesTex, vector5, (Rectangle?)new Rectangle(256, 672, 128, 128), val2, 0f, new Vector2(64f, 64f), 1f, (SpriteEffects)0, 1f);
			sprite.Draw(Game1.nullTex, vector5, (Rectangle?)new Rectangle(0, 0, 1, 1), new Color(1f, 1f, 1f, 0.2f), 0f, new Vector2(0.5f, 0.5f), new Vector2(113f, 1f), (SpriteEffects)0, 1f);
			sprite.Draw(Game1.nullTex, vector5, (Rectangle?)new Rectangle(0, 0, 1, 1), new Color(1f, 1f, 1f, 0.2f), 0f, new Vector2(0.5f, 0.5f), new Vector2(1f, 113f), (SpriteEffects)0, 1f);
			Color val3 = default(Color);
			for (int k = 0; k < Game1.character.Length; k++)
			{
				if (Game1.character[k] == null || Game1.character[k].hp < 0)
				{
					continue;
				}
				float num7 = Game1.character[k].radarTraj.LengthSquared() / 80000f;
				if (Game1.character[k].perk[2] == 6)
				{
					num7 = 0f;
				}
				if (GameState.gameType == 4 && c.team == 0 && Game1.character[k].team == 1)
				{
					num7 = 0f;
				}
				if (Game1.character[Game1.netSession.GetPlayerOne()].perk[1] == 2 && num7 < 0.5f)
				{
					num7 = 0.5f;
				}
				if (!(num7 > 0.1f))
				{
					continue;
				}
				num7 -= 0.1f;
				val3 = new Color(1f, 1f, 1f, num7);
				if (k != Game1.netSession.GetPlayerOne())
				{
					if (GameState.gameType == 0)
					{
						val3 = new Color(1f, 0f, 0f, num7);
					}
					else if (Game1.character[k].team == 0)
					{
						val3 = new Color(0f, 0.2f, 1f, num7);
					}
					else
					{
						val3 = new Color(1f, 0f, 0f, num7);
					}
				}
				Vector2 vector6 = Game1.character[k].loc - Game1.character[Game1.netSession.GetPlayerOne()].loc;
				float num8 = 2000f;
				if (vector6.X > 0f - num8 && vector6.X < num8 && vector6.Y > 0f - num8 && vector6.Y < num8 && vector6.Length() < num8)
				{
					sprite.Draw(Game1.nullTex, vector5 + vector6 / num8 * 57f, (Rectangle?)new Rectangle(0, 0, 1, 1), val3, 0f, new Vector2(0.5f, 0.5f), 4f, (SpriteEffects)0, 1f);
				}
			}
			if (!flag)
			{
				float num9 = 0.25f;
				int num10 = (int)(c.jetGas / num9);
				Color val4 = default(Color);
				for (int l = 0; l < num10 + 1; l++)
				{
					float num11 = 25f;
					if (l == num10)
					{
						num11 *= c.jetGas / num9 - (float)l;
					}
					float num12 = 0f;
					val4 = new Color(1f, 1f, 1f, 0.35f + num12);
					if (l == num10)
					{
						switch (l)
						{
						case 0:
							val4 = new Color(1f, 0f, 0f, 0.5f);
							if ((int)(frame * 60f) % 4 == 0)
							{
								val4 = new Color(1f, 1f, 1f, 1f);
							}
							break;
						case 1:
							val4 = new Color(1f, 1f, 0f, 0.5f);
							if ((int)(frame * 30f) % 4 == 0)
							{
								val4 = new Color(1f, 1f, 0.5f, 1f);
							}
							break;
						default:
							val4 = new Color(1f, 1f, 1f, 0.9f);
							break;
						}
					}
					sprite.Draw(Game1.nullTex, vector3 + new Vector2(-75f + (float)l * 28f, 32f), (Rectangle?)new Rectangle(0, 0, 1, 1), val4, 0f, default(Vector2), new Vector2(num11, 14f), (SpriteEffects)0, 1f);
				}
				for (int m = 0; m < 3; m++)
				{
					int num13 = Game1.character[Game1.netSession.GetPlayerOne()].perk[m];
					sprite.Draw(Game1.perksTex, new Vector2(600f + (float)m * 60f, 585f), (Rectangle?)new Rectangle(768 + m * 128, num13 * 128, 128, 128), val2, 0f, default(Vector2), 0.4f, (SpriteEffects)0, 1f);
				}
				Color val5 = default(Color);
				for (int n = 0; n < 2; n++)
				{
					val5 = new Color(new Vector4(0f, 0f, 0f, 0.5f));
					if (n == 1)
					{
						sprite.End();
						sprite.Begin(blendState: BlendState.Additive);
						val5 = white;
					}
					sprite.Draw(Game1.spritesTex, vector2, (Rectangle?)new Rectangle(128 * n, 576, 128, 128), val5, 0f, new Vector2(64f, 64f), 1.5f, (SpriteEffects)0, 1f);
					sprite.Draw(Game1.spritesTex, vector3 + new Vector2(96f, -3f), (Rectangle?)new Rectangle(128 * n, 702, 128, 64), val5, 0f, new Vector2(64f, 32f), 1.5f, (SpriteEffects)0, 1f);
					sprite.Draw(Game1.spritesTex, vector3, (Rectangle?)new Rectangle(128 * n, 768, 128, 64), val5, 0f, new Vector2(64f, 32f), 1.5f, (SpriteEffects)0, 1f);
					int num14 = 2;
					if (!Game1.settings.twinStickShooter)
					{
						num14 = 1;
					}
					for (int num15 = 0; num15 < num14; num15++)
					{
						if (c.grenAmmo[num15] > 0)
						{
							sprite.Draw(Game1.spritesTex, vector4 + new Vector2(48f, -64f * (float)num15), (Rectangle?)new Rectangle(32 + 128 * n, 702, 64, 64), val5, 0f, new Vector2(32f, 32f), 1.5f, (SpriteEffects)0, 1f);
							sprite.Draw(Game1.spritesTex, vector4 + new Vector2(0f, -64f * (float)num15), (Rectangle?)new Rectangle(32 + 128 * n, 702, 64, 64), val5, 0f, new Vector2(32f, 32f), 1.5f, (SpriteEffects)0, 1f);
						}
					}
					if (n == 1)
					{
						sprite.End();
						sprite.Begin(blendState: BlendState.AlphaBlend);
					}
				}
			}
			if (!flag)
			{
				for (int num16 = 0; num16 < c.weapon.Length; num16++)
				{
					Vector2 vector7 = vector2;
					switch (num16)
					{
					case 0:
						vector7.X -= 48f;
						break;
					case 1:
						vector7.X += 48f;
						break;
					case 2:
						vector7.Y -= 48f;
						break;
					case 3:
						vector7.Y += 48f;
						break;
					}
					if (c.weapon[num16] > -1)
					{
						int num17 = WeaponCatalog.weapons[c.weapon[num16]].imgIdx;
						bool flag2 = false;
						if (num17 >= 64)
						{
							num17 -= 64;
							flag2 = true;
						}
						sprite.Draw(Game1.spritesTex, vector7, (Rectangle?)new Rectangle(0, 832, 192, 192), new Color(1f, 1f, 1f, 0.15f), 0f, new Vector2(96f, 96f), 0.4f, (SpriteEffects)0, 1f);
						sprite.Draw(Game1.spritesTex, vector7, (Rectangle?)new Rectangle(num17 % 16 * 64, 384 + num17 / 16 * 64, 64, 64), new Color(1f, 1f, 1f, 1f), 0f, new Vector2(32f, 32f), 1f, (SpriteEffects)0, 1f);
						if (flag2)
						{
							sprite.Draw(Game1.spritesTex, vector7, (Rectangle?)new Rectangle(num17 % 16 * 64, 384 + num17 / 16 * 64, 64, 64), new Color(1f, 1f, 1f, 1f), 0f, new Vector2(32f, 32f), 1f, (SpriteEffects)1, 1f);
						}
					}
				}
				if (c.weapon[c.curWeap] > -1)
				{
					int num18 = c.ammo[WeaponCatalog.weapons[c.weapon[c.curWeap]].ammoType] + c.magazine[c.curWeap];
					if (WeaponCatalog.weapons[c.weapon[c.curWeap]].ammoType == 0)
					{
						num18 = -1;
					}
					sprite.DrawString(Game1.impact, (num18 == -1) ? nilStr : Numbers.GetNumber(num18), vector3 + new Vector2(80f, -10f), white);
					int num19 = WeaponCatalog.weapons[c.weapon[c.curWeap]].maxClip;
					if (num19 > 1 && c.perk[2] == 7)
					{
						num19 *= 3;
					}
					_ = (float)c.magazine[c.curWeap] / (float)num19;
					sprite.Draw(Game1.nullTex, new Rectangle((int)vector3.X - 69, (int)vector3.Y - 19, (int)(137f * ammoA), 41), new Color(new Vector4(0.85f, 0.85f * ammoA, 1f * ammoA, 0.5f)));
					int num20 = WeaponCatalog.weapons[c.weapon[c.curWeap]].imgIdx;
					bool flag3 = false;
					if (num20 >= 64)
					{
						num20 -= 64;
						flag3 = true;
					}
					sprite.Draw(Game1.spritesTex, vector3 - new Vector2(80f, 0f), (Rectangle?)new Rectangle(num20 % 16 * 64, 384 + num20 / 16 * 64, 64, 64), Color.White, 0f, new Vector2(32f, 32f), 1.5f, (SpriteEffects)0, 1f);
					if (flag3)
					{
						sprite.Draw(Game1.spritesTex, vector3 - new Vector2(80f, 0f), (Rectangle?)new Rectangle(num20 % 16 * 64, 384 + num20 / 16 * 64, 64, 64), Color.White, 0f, new Vector2(32f, 32f), 1.5f, (SpriteEffects)1, 1f);
					}
				}
				int num21 = 2;
				if (!Game1.settings.twinStickShooter)
				{
					num21 = 1;
				}
				for (int num22 = 0; num22 < num21; num22++)
				{
					if (c.grenType[num22] > -1 && c.grenAmmo[num22] > 0)
					{
						sprite.Draw(Game1.spritesTex, vector4 + new Vector2(0f, -64f * (float)num22), (Rectangle?)new Rectangle((c.grenType[num22] - 1) % 16 * 64, 320 + (c.grenType[num22] - 1) / 16 * 64, 64, 64), Color.White, 0f, new Vector2(32f, 32f), 1f, (SpriteEffects)1, 1f);
						sprite.DrawString(Game1.impact, Numbers.GetNumber(c.grenAmmo[num22]), vector4 + new Vector2(42f, -64f * (float)num22 - 8f), white);
					}
				}
				for (int num23 = num21; num23 < c.grenType.Length; num23++)
				{
					if (c.grenType[num23] > -1 && c.grenAmmo[num23] > 0)
					{
						sprite.Draw(Game1.spritesTex, vector4 + new Vector2(64f, -36f * (float)num23 - 60f), (Rectangle?)new Rectangle((c.grenType[num23] - 1) % 16 * 64, 320 + (c.grenType[num23] - 1) / 16 * 64, 64, 64), Color.White, 0f, new Vector2(32f, 32f), 1f, (SpriteEffects)1, 1f);
					}
				}
			}
			if (c.GetTeam() != 0)
			{
				for (int num24 = 0; num24 < Game1.character.Length; num24++)
				{
					if (num24 != c.ID && Game1.character[num24] != null && Game1.character[num24].GetTeam() == c.GetTeam() && Game1.character[num24].hp >= 0 && Game1.character[num24].spawnFrame <= 0f)
					{
						sprite.Draw(Game1.spritesTex, Scroll.GetLoc(Game1.character[num24].loc - new Vector2(0f, 70f)), (Rectangle?)new Rectangle(672, 32 * Game1.character[num24].GetTeam(), 64, 32), Color.White, 0f, new Vector2(32f, 32f), 0.7f, (SpriteEffects)0, 1f);
					}
				}
			}
			if (GameState.gameType == 2)
			{
				Vector2 gVec = default(Vector2);
				if (Game1.netSession.redFlagState == 200)
				{
					gVec = Game1.gameMap.redFlagHome;
				}
				else if (Game1.character[Game1.netSession.redFlagState] != null)
				{
					gVec = Game1.character[Game1.netSession.redFlagState].loc - new Vector2(0f, 48f);
				}
				DrawPointer(gVec, c, 1, sprite);
				gVec = default(Vector2);
				if (Game1.netSession.blueFlagState == 200)
				{
					gVec = Game1.gameMap.blueFlagHome;
				}
				else if (Game1.character[Game1.netSession.blueFlagState] != null)
				{
					gVec = Game1.character[Game1.netSession.blueFlagState].loc - new Vector2(0f, 48f);
				}
				DrawPointer(gVec, c, 2, sprite);
			}
			else if (GameState.gameType == 3)
			{
				if (Game1.netSession.hillState == 0)
				{
					DrawPointer(Game1.gameMap.hill, c, 0, sprite);
				}
				else if (Game1.netSession.hillState == 1)
				{
					DrawPointer(Game1.gameMap.hill, c, 2, sprite);
				}
				else if (Game1.netSession.hillState == 2)
				{
					DrawPointer(Game1.gameMap.hill, c, 1, sprite);
				}
			}
			if (Game1.netSession.netPlay != null)
			{
				Game1.netSession.netPlay.DrawHud(sprite);
			}
			if (suitDescFrame > 0f)
			{
				int num25 = suitDescIdx - 1;
				if (suitDescIdx == 100)
				{
					num25 = 6;
				}
				float num26 = suitDescFrame / 2f;
				if (num26 > 0.5f)
				{
					num26 = 0.5f;
				}
				sprite.Draw(Game1.spritesTex, new Vector2(640f, 510f), (Rectangle?)new Rectangle(0, 768, 128, 64), new Color(new Vector4(0f, 0f, 0f, num26)), 0f, new Vector2(64f, 32f), new Vector2(5f, 2.2f), (SpriteEffects)0, 1f);
				sprite.DrawString(Game1.impact, SuitManager.suitText[num25 * 2], new Vector2(640f, 500f) - Game1.impact.MeasureString(SuitManager.suitText[num25 * 2]) / 2f, new Color(new Vector4(1f, 1f, 1f, suitDescFrame)));
				if (suitDescIdx == 100)
				{
					sprite.DrawString(Game1.impact, SuitManager.phoenixFix, new Vector2(640f, 520f) - Game1.impact.MeasureString(SuitManager.phoenixFix) / 2f, new Color(new Vector4(1f, 1f, 0f, suitDescFrame)));
				}
				else
				{
					sprite.DrawString(Game1.impact, SuitManager.suitText[num25 * 2 + 1], new Vector2(640f, 520f) - Game1.impact.MeasureString(SuitManager.suitText[num25 * 2 + 1]) / 2f, new Color(new Vector4(1f, 1f, 0f, suitDescFrame)));
				}
			}
		}
		if (scoreBoard.alpha <= 0f)
		{
			messageMgr.Draw(sprite);
		}
		scoreBoard.Draw(sprite);
		if (serverChangingSettingsFrame > 0f)
		{
			Vector2 vector8 = new Vector2(1280f, 720f) / 2f + new Vector2(0f, -250f);
			sprite.Draw(Game1.spritesTex, vector8, (Rectangle?)new Rectangle(0, 768, 128, 64), new Color(0f, 0f, 0f, 0.75f), 0f, new Vector2(64f, 24f), new Vector2(4.25f, 2f), (SpriteEffects)0, 1f);
			Game1.text.color = new Color(1f, 1f, 1f, 1f);
			Game1.text.size = 1f;
			Game1.text.DrawString(vector8, serverChangingSettingsStr, 1, -1f, Game1.impact, sprite);
		}
	}

	private void DrawPopScore(SpriteBatch sprite)
	{
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		if (!(popScoreFrame <= 0f))
		{
			float num = 1f;
			Game1.text.size = 2f;
			if (popScoreFrame < 0.25f)
			{
				Game1.text.size -= (0.25f - popScoreFrame) * 2f;
				num = popScoreFrame * 4f;
			}
			if (popScoreFrame > 0.75f)
			{
				Game1.text.size += (popScoreFrame - 0.75f) * 4f;
				num = (1f - popScoreFrame) * 4f;
			}
			Game1.text.size *= 0.9f;
			Game1.text.color = new Color(1f, 1f, 1f, num);
			Game1.text.DrawString(new Vector2(640f, 150f), popupScoreAddStr, 1, -1f, Game1.impact, sprite);
		}
	}

	private void DrawYouDied(SpriteBatch sprite)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		Game1.text.size = 2f;
		Game1.text.size *= 0.9f;
		Game1.text.color = new Color(1f, 1f, 1f, 1f);
		if (deadString == null)
		{
			deadString = new StringBuilder("You died");
		}
		Game1.text.DrawString(new Vector2(640f, 250f), deadString, 1, -1f, Game1.impact, sprite);
	}

	private void DrawPointer(Vector2 gVec, Character c, int idx, SpriteBatch sprite)
	{
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		gVec -= c.loc;
		if (gVec.Length() < 300f)
		{
			gVec /= 300f;
		}
		else
		{
			gVec.Normalize();
		}
		sprite.Draw(Game1.spritesTex, Scroll.GetLoc(c.loc - new Vector2(0f, 48f)) + gVec * 300f, (Rectangle?)new Rectangle(672 + idx * 64, 96, 64, 64), new Color(new Vector4(1f, 1f, 1f, 0.8f)), Trig.GetAngle(default(Vector2), gVec), new Vector2(32f, 32f), 0.75f, (SpriteEffects)1, 1f);
	}

	public void AddPopScore(int p)
	{
		if (p % 10 != 0)
		{
			p = p / 10 * 10;
			if (p < 10)
			{
				p = 10;
			}
			Console.WriteLine("Score not a multiple of 10.");
		}
		if (p > 0)
		{
			if (popScoreFrame <= 0f)
			{
				popScoreAdd = p;
			}
			else
			{
				popScoreAdd += p;
			}
			popScoreFrame = 1f;
			int num = popScoreAdd;
			if (Leveling.IsHappyHour(DateTime.Now.TimeOfDay.Hours))
			{
				num *= 2;
			}
			popupScoreAddStr = new StringBuilder("+" + num);
			Sound.PlayCue("chime");
		}
	}

	internal void AddPickup(byte type, int cue)
	{
		pickupShowType = type;
		pickupShowCue = cue;
	}

	internal void SetDead(string p)
	{
		deadString = new StringBuilder(p);
	}

	internal void DoPickup(int weapImgIdx)
	{
		pickup.DoPickup(weapImgIdx);
	}

	internal void DoName(int iidx)
	{
		pickup.DoName(iidx);
	}
}
