using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ZP2K9.hud;

namespace ZP2K9.menu;

public class MenuItem
{
	public const int MAPLIST_NONE = 0;

	public const int MAPLIST_INCLUDED = 1;

	public const int MAPLIST_EXCLUDED = 2;

	public StringBuilder text;

	public StringBuilder[] selText;

	public int idx;

	public bool noSelect;

	public int minX;

	public int maxX;

	public int selX;

	public bool roster;

	public bool disabled;

	public float bump;

	public bool locked;

	public bool newunlock;

	public int mapList;

	public float rosterAnim;

	public float rosterFrame;

	public bool classAtAGlance;

	public bool perksAtAGlance;

	public bool appearanceAtAGlance;

	public int perk = -1;

	public bool server;

	private StringBuilder gamers;

	private StringBuilder ping;

	private StringBuilder version;

	private StringBuilder gameType;

	public float newBump;

	public MenuItem(string s, int idx)
	{
		text = new StringBuilder(s);
		this.idx = idx;
	}

	public void UpdatePing(int ping)
	{
		this.ping = new StringBuilder(ping + "ms");
	}

	public MenuItem(string s, int idx, int gamers, int maxGamers, int ping, int? version, int? gameType)
	{
		this.text = new StringBuilder(s);
		this.idx = idx;
		this.gamers = new StringBuilder(gamers + "/" + maxGamers);
		if (ping < 0)
		{
			this.ping = new StringBuilder("--");
		}
		else
		{
			this.ping = new StringBuilder(ping + "ms");
		}
		int num = ((!version.HasValue) ? 100 : version.Value);
		if (num > 206)
		{
			Game1.netSession.newVersAvailable = true;
		}
		string text = num.ToString();
		text = text.Substring(0, 1) + "." + text.Substring(1, 1) + "." + text.Substring(2, 1);
		this.version = new StringBuilder(text);
		switch (gameType.HasValue ? gameType.Value : 0)
		{
		case 0:
			this.gameType = new StringBuilder("DM");
			break;
		case 1:
			this.gameType = new StringBuilder("TDM");
			break;
		case 2:
			this.gameType = new StringBuilder("CTF");
			break;
		case 3:
			this.gameType = new StringBuilder("KotH");
			break;
		case 4:
			this.gameType = new StringBuilder("ZH");
			break;
		}
		server = true;
	}

	public MenuItem(string s, int idx, bool noSelect)
	{
		text = new StringBuilder(s);
		this.idx = idx;
		this.noSelect = noSelect;
	}

	public MenuItem(string[] s, int idx)
	{
		selText = new StringBuilder[s.Length];
		for (int i = 0; i < selText.Length; i++)
		{
			selText[i] = new StringBuilder(s[i]);
		}
		minX = 0;
		selX = 0;
		maxX = s.Length - 1;
		this.idx = idx;
	}

	public void Update(InterfaceKeys ikeys)
	{
		if (maxX > minX)
		{
			if (selX >= maxX)
			{
				selX = maxX;
			}
			if (ikeys.keyRight && selX < maxX)
			{
				selX++;
				Sound.PlayCue("swing");
			}
			if (ikeys.keyLeft && selX > minX)
			{
				selX--;
				Sound.PlayCue("throw");
			}
		}
	}

	public virtual void Draw(SpriteBatch sprite, Vector2 orig, int selItem, float a, float width)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0986: Unknown result type (might be due to invalid IL or missing references)
		//IL_086f: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0613: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0654: Unknown result type (might be due to invalid IL or missing references)
		//IL_0659: Unknown result type (might be due to invalid IL or missing references)
		//IL_068d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0692: Unknown result type (might be due to invalid IL or missing references)
		//IL_06df: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0adc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ae1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b15: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b1a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bb3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bb5: Unknown result type (might be due to invalid IL or missing references)
		if (disabled)
		{
			a /= 2f;
		}
		if (noSelect)
		{
			Game1.text.color = new Color(new Vector4(0.6f, 0.6f, 1f, 0.5f * a));
		}
		else if (selItem != idx)
		{
			Game1.text.color = new Color(new Vector4(0.6f, 0.6f, 1f, 0.8f * a));
		}
		else
		{
			Game1.text.color = new Color(new Vector4(0.7f, 0.7f, 1f, 1f * a));
		}
		Game1.text.size = 1f;
		if (selItem == idx)
		{
			for (int i = 0; i < 3; i++)
			{
				sprite.Draw(Game1.spritesTex, orig + new Vector2(width / 2f, 50f + (float)idx * 32f + 16f), (Rectangle?)new Rectangle(0, 832, 192, 192), new Color(1f, 1f, 1f, a * 0.075f), 0f, new Vector2(96f, 96f), new Vector2(width / (48f * (float)(i + 1)), 0.1f), (SpriteEffects)0, 1f);
			}
		}
		if (selText != null)
		{
			if (selX > maxX)
			{
				selX = 0;
			}
			try
			{
				int num = selX;
				if (selX >= selText.Length)
				{
					num = selText.Length - 1;
				}
				Game1.text.DrawString(orig + new Vector2(30f, 50f + (float)idx * 32f), selText[num], 0, -1f, Game1.impact, sprite);
				if (!locked && newunlock)
				{
					float size = Game1.text.size;
					Color color = Game1.text.color;
					Game1.text.size = 0.8f;
					Game1.text.color = new Color(1f, 1f, 1f, Rand.GetRandomFloat(0.5f, 1f));
					Game1.text.DrawString(orig + new Vector2(Game1.text.GetStringLength(selText[num], Game1.impact) + 48f + newBump, 50f + (float)idx * 32f) + new Vector2(-8f, -4f), Game1.menu.newString, 0, -1f, Game1.impact, sprite);
					Game1.text.color = color;
					Game1.text.size = size;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.StackTrace);
			}
			if (roster)
			{
				DrawRoster(sprite, rosterFrame, orig);
			}
			if (perk > -1)
			{
				sprite.End();
				sprite.Begin(blendState: BlendState.AlphaBlend);
				bool flag = true;
				if (Game1.zProfile.unlocks.perkUnlocked[idx, selX] > 0)
				{
					flag = false;
				}
				bool flag2 = Game1.zProfile.unlocks.perkUnlocked[idx, selX] == 1;
				float num2 = (flag ? 0.1f : 1f);
				float num3 = 165f;
				sprite.Draw(Game1.perksTex, orig + new Vector2(num3, 50f + (float)idx * 32f), (Rectangle?)new Rectangle(768 + perk * 128, selX * 128, 128, 128), new Color(num2, num2, num2, a * 0.9f), 0f, new Vector2(64f, 64f), 0.5f, (SpriteEffects)0, 1f);
				if (selX > 0)
				{
					num2 = ((Game1.zProfile.unlocks.perkUnlocked[idx, selX - 1] == 0) ? 0.1f : 1f);
					sprite.Draw(Game1.perksTex, orig + new Vector2(num3 - 80f, 50f + (float)idx * 32f), (Rectangle?)new Rectangle(768 + perk * 128, (selX - 1) * 128, 128, 128), new Color(num2, num2, num2, a * 0.2f), 0f, new Vector2(64f, 64f), 0.5f, (SpriteEffects)0, 1f);
				}
				if (selX < 9)
				{
					num2 = ((Game1.zProfile.unlocks.perkUnlocked[idx, selX + 1] == 0) ? 0.1f : 1f);
					sprite.Draw(Game1.perksTex, orig + new Vector2(num3 + 80f, 50f + (float)idx * 32f), (Rectangle?)new Rectangle(768 + perk * 128, (selX + 1) * 128, 128, 128), new Color(num2, num2, num2, a * 0.2f), 0f, new Vector2(64f, 64f), 0.5f, (SpriteEffects)0, 1f);
				}
				if (flag)
				{
					sprite.Draw(Game1.spritesTex, orig + new Vector2(num3, 60f + (float)idx * 32f), (Rectangle?)new Rectangle(864, 128, 32, 32), Game1.text.color, 0f, new Vector2(16f, 16f), 1f, (SpriteEffects)0, 1f);
				}
				if (flag2)
				{
					float size2 = Game1.text.size;
					Color color2 = Game1.text.color;
					Game1.text.size = 0.8f;
					Game1.text.color = new Color(1f, 1f, 1f, Rand.GetRandomFloat(0.5f, 1f));
					Game1.text.DrawString(orig + new Vector2(185f, 7f + (float)idx * 32f), Game1.menu.newString, 0, -1f, Game1.impact, sprite);
					Game1.text.color = color2;
					Game1.text.size = size2;
				}
				sprite.End();
				sprite.Begin(blendState: BlendState.Additive);
				Game1.text.DrawString(orig + new Vector2(num3, 80f + (float)idx * 32f), Game1.perkDescriptions.descriptions[idx][selX].name, 1, -1f, Game1.impact, sprite);
				float size3 = Game1.text.size;
				Game1.text.size *= 0.9f;
				Game1.text.DrawString(orig + new Vector2(num3, 100f + (float)idx * 32f), Game1.perkDescriptions.descriptions[idx][selX].description, 1, -1f, Game1.impact, sprite);
				Game1.text.size = size3;
			}
			if (selItem == idx)
			{
				DrawScrollers(sprite, orig, selItem, a, width);
			}
		}
		else
		{
			if (locked)
			{
				sprite.Draw(Game1.spritesTex, orig + new Vector2(10f + (classAtAGlance ? 60f : 0f), 50f + (float)idx * 32f) + new Vector2(-8f, -4f), (Rectangle?)new Rectangle(864, 128, 32, 32), Game1.text.color, 0f, default(Vector2), 1f, (SpriteEffects)0, 1f);
			}
			if (mapList != 0)
			{
				sprite.Draw(Game1.spritesTex, orig + new Vector2(16f, 65f + (float)idx * 32f), (Rectangle?)new Rectangle(256, 672, 128, 128), new Color(1f, 1f, 1f, 0.15f), 0f, new Vector2(64f, 64f), 0.1f, (SpriteEffects)0, 1f);
				sprite.Draw(Game1.spritesTex, orig + new Vector2(16f, 65f + (float)idx * 32f), (Rectangle?)new Rectangle(256, 672, 128, 128), (mapList == 1) ? new Color(0f, 1f, 0f, 1f) : new Color(1f, 0f, 0f, 1f), 0f, new Vector2(64f, 64f), 0.08f, (SpriteEffects)0, 1f);
				orig.X += 16f;
			}
			Game1.text.DrawString(orig + new Vector2(10f + (classAtAGlance ? 60f : 0f) + (locked ? 30f : 0f), 50f + (float)idx * 32f), text, 0, -1f, Game1.impact, sprite);
			if (classAtAGlance && a > 0.9f)
			{
				DrawClassAtAGlance(sprite, rosterFrame, orig, selItem == idx);
			}
			if (appearanceAtAGlance && a > 0.45f)
			{
				DrawCharacterAtAGlance(sprite, rosterFrame, orig);
			}
			if (perksAtAGlance && a > 0.9f)
			{
				DrawPerksAtAGlance(sprite, rosterFrame, orig, selItem == idx);
			}
			if (!locked && newunlock)
			{
				float size4 = Game1.text.size;
				Color color3 = Game1.text.color;
				Game1.text.size = 0.8f;
				Game1.text.color = new Color(1f, 1f, 1f, Rand.GetRandomFloat(0.5f, 1f));
				Game1.text.DrawString(orig + new Vector2(10f + (classAtAGlance ? 60f : 0f) + Game1.text.GetStringLength(text, Game1.impact) + 32f + newBump, 50f + (float)idx * 32f) + new Vector2(-8f, -4f), Game1.menu.newString, 0, -1f, Game1.impact, sprite);
				Game1.text.color = color3;
				Game1.text.size = size4;
			}
			if (server)
			{
				float num4 = 65f;
				Game1.text.DrawString(orig + new Vector2(144f + num4, 50f + (float)idx * 32f), gamers, 0, -1f, Game1.impact, sprite);
				Game1.text.DrawString(orig + new Vector2(225f + num4, 50f + (float)idx * 32f), gameType, 0, -1f, Game1.impact, sprite);
				Game1.text.DrawString(orig + new Vector2(332f + num4, 50f + (float)idx * 32f), version, 0, -1f, Game1.impact, sprite);
				Game1.text.DrawString(orig + new Vector2(395f + num4, 50f + (float)idx * 32f), ping, 0, -1f, Game1.impact, sprite);
			}
		}
	}

	private void DrawClassAtAGlance(SpriteBatch sprite, float rosterFrame, Vector2 orig, bool selected)
	{
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		sprite.End();
		sprite.Begin(blendState: BlendState.AlphaBlend);
		Game1.rosterChar.bodyType = Game1.zProfile.ClassSet(idx).bodyType;
		Game1.rosterChar.skinTex = Game1.zProfile.Class(idx).skinTex;
		Game1.rosterChar.headTex = Game1.zProfile.Class(idx).headTex;
		Game1.rosterChar.hatTex = Game1.zProfile.Class(idx).hatTex;
		Game1.rosterChar.torsoTex = Game1.zProfile.Class(idx).torsoTex;
		Game1.rosterChar.legsTex = Game1.zProfile.Class(idx).legsTex;
		Game1.rosterChar.team = Game1.zProfile.ClassSet(idx).defaultTeam;
		Game1.rosterChar.jetpack = Game1.zProfile.Class(idx).jetpack;
		Game1.rosterChar.face = 1;
		Game1.rosterChar.bodySec[0].SetAnim(Game1.rosterChar.GetAnimName(0), Game1.rosterChar);
		try
		{
			Game1.rosterChar.isRosterChar = false;
			Game1.rosterChar.Draw(sprite, 0, 0, all: true, orig + new Vector2(35f, 78f + (float)idx * 32f), 0.4f);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.StackTrace);
		}
		for (int i = 0; i < 3; i++)
		{
			sprite.Draw(Game1.perksTex, orig + new Vector2((float)i * 40f + 230f, 42f + (float)idx * 32f), (Rectangle?)new Rectangle(768 + i * 128, Game1.zProfile.ClassSet(idx).perk[i] * 128, 128, 128), selected ? new Color(0.8f, 0.8f, 1f, 1f) : new Color(0.6f, 0.6f, 1f, 1f), 0f, default(Vector2), 0.3f, (SpriteEffects)0, 1f);
		}
		if (idx == Game1.zProfile.defaultClass)
		{
			sprite.Draw(Game1.spritesTex, orig + new Vector2(-12f, 42f + (float)idx * 32f), (Rectangle?)new Rectangle(224, 32, 32, 32), new Color(0.6f, 0.6f, 1f, 1f), 0f, default(Vector2), 0.9f, (SpriteEffects)0, 1f);
		}
		sprite.End();
		sprite.Begin(blendState: BlendState.Additive);
	}

	private void DrawRoster(SpriteBatch sprite, float rosterFrame, Vector2 orig)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		sprite.End();
		sprite.Begin(blendState: BlendState.AlphaBlend);
		sprite.Draw(Game1.spritesTex, orig + new Vector2(170f, 90f), (Rectangle?)new Rectangle(0, 832, 192, 192), new Color(1f, 1f, 1f, 0.2f), 0f, new Vector2(96f, 96f), 2f, (SpriteEffects)0, 1f);
		Game1.rosterChar.bodyType = Game1.zProfile.EditingSet().bodyType;
		Game1.rosterChar.skinTex = Game1.zProfile.EditingClass().skinTex;
		Game1.rosterChar.headTex = Game1.zProfile.EditingClass().headTex;
		Game1.rosterChar.hatTex = Game1.zProfile.EditingClass().hatTex;
		Game1.rosterChar.torsoTex = Game1.zProfile.EditingClass().torsoTex;
		Game1.rosterChar.legsTex = Game1.zProfile.EditingClass().legsTex;
		Game1.rosterChar.team = Game1.zProfile.EditingSet().defaultTeam;
		Game1.rosterChar.jetpack = Game1.zProfile.EditingClass().jetpack;
		Game1.rosterChar.face = 0;
		if (rosterFrame > 0f)
		{
			Game1.rosterChar.bodySec[0].SetAnim(Game1.rosterChar.GetAnimName(10), Game1.rosterChar);
		}
		else
		{
			Game1.rosterChar.bodySec[0].SetAnim(Game1.rosterChar.GetAnimName(0), Game1.rosterChar);
		}
		Game1.rosterChar.isRosterChar = true;
		Game1.rosterChar.Draw(sprite, 0, 0, all: true, orig + new Vector2(170f, 190f + (float)idx * 32f) + new Vector2(rosterAnim * 40f, 0f), 1.5f);
		sprite.End();
		sprite.Begin(blendState: BlendState.Additive);
	}

	private void DrawCharacterAtAGlance(SpriteBatch sprite, float rosterFrame, Vector2 orig)
	{
		sprite.End();
		sprite.Begin(blendState: BlendState.AlphaBlend);
		Game1.rosterChar.bodyType = Game1.zProfile.EditingSet().bodyType;
		Game1.rosterChar.skinTex = Game1.zProfile.EditingClass().skinTex;
		Game1.rosterChar.headTex = Game1.zProfile.EditingClass().headTex;
		Game1.rosterChar.hatTex = Game1.zProfile.EditingClass().hatTex;
		Game1.rosterChar.torsoTex = Game1.zProfile.EditingClass().torsoTex;
		Game1.rosterChar.legsTex = Game1.zProfile.EditingClass().legsTex;
		Game1.rosterChar.team = Game1.zProfile.EditingSet().defaultTeam;
		Game1.rosterChar.jetpack = Game1.zProfile.EditingClass().jetpack;
		Game1.rosterChar.face = 0;
		if (rosterFrame > 0f)
		{
			Game1.rosterChar.bodySec[0].SetAnim(Game1.rosterChar.GetAnimName(10), Game1.rosterChar);
		}
		else
		{
			Game1.rosterChar.bodySec[0].SetAnim(Game1.rosterChar.GetAnimName(0), Game1.rosterChar);
		}
		Game1.rosterChar.isRosterChar = false;
		Game1.rosterChar.Draw(sprite, 0, 0, all: true, orig + new Vector2(100f, 45f + (float)idx * 32f) + new Vector2(rosterAnim * 40f, 0f), 1f);
		sprite.End();
		sprite.Begin(blendState: BlendState.Additive);
	}

	private void DrawPerksAtAGlance(SpriteBatch sprite, float rosterFrame, Vector2 orig, bool selected)
	{
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		sprite.End();
		sprite.Begin(blendState: BlendState.AlphaBlend);
		for (int i = 0; i < 3; i++)
		{
			sprite.Draw(Game1.perksTex, orig + new Vector2((float)i * 40f + 110f, 42f + (float)idx * 32f), (Rectangle?)new Rectangle(768 + i * 128, Game1.zProfile.EditingSet().perk[i] * 128, 128, 128), selected ? new Color(0.8f, 0.8f, 1f, 1f) : new Color(0.6f, 0.6f, 1f, 1f), 0f, default(Vector2), 0.3f, (SpriteEffects)0, 1f);
		}
		sprite.End();
		sprite.Begin(blendState: BlendState.Additive);
	}

	public virtual void DrawScrollers(SpriteBatch sprite, Vector2 orig, int selItem, float a, float width)
	{
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		sprite.Draw(Game1.spritesTex, orig + new Vector2(12f, 52f + (float)idx * 32f) + new Vector2(-5f, 0f), (Rectangle?)new Rectangle(224, 32, 32, 32), (selX > minX) ? new Color(new Vector4(0.7f, 0.7f, 1f, 1f * a)) : new Color(new Vector4(0.6f, 0.6f, 1f, 0.7f * a)), 0f, new Vector2(0f, 0f), 0.5f, (SpriteEffects)1, 1f);
		sprite.Draw(Game1.spritesTex, orig + new Vector2(10f, 52f + (float)idx * 32f) + new Vector2(width - 40f, 0f), (Rectangle?)new Rectangle(224, 32, 32, 32), (selX < maxX) ? new Color(new Vector4(0.7f, 0.7f, 1f, 1f * a)) : new Color(new Vector4(0.6f, 0.6f, 1f, 0.7f * a)), 0f, new Vector2(0f, 0f), 0.5f, (SpriteEffects)0, 1f);
	}
}
