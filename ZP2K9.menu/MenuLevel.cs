using System.Text;
using Microsoft.Xna.Framework;
using ZP2K9.platform;
using Microsoft.Xna.Framework.Graphics;
using ZP2K9.hud;
using ZP2K9.menu.levels;

namespace ZP2K9.menu;

public class MenuLevel
{
	public MenuItem[] item;

	public int selected;

	public bool active;

	public int width;

	public int height;

	public float alpha;

	public StringBuilder name;

	public StringBuilder[] error;

	public bool isStartGate;

	public bool isControls;

	public bool selOnly;

	public virtual void SelectItem(Menu menu)
	{
	}

	public virtual void Cancel(Menu menu)
	{
	}

	public virtual void CycleOff(int i, int x)
	{
	}

	public virtual void Update(InterfaceKeys iKeys, Menu menu)
	{
		if (Guide.IsVisible)
		{
			return;
		}
		if (active)
		{
			height = item.Length * 30 + 80;
			if (error != null)
			{
				height = 200;
			}
			for (int i = 0; i < item.Length; i++)
			{
				if (item[i].perk > -1)
				{
					height += 90;
				}
				if (item[i].classAtAGlance)
				{
					height += 20;
				}
				if (item[i].appearanceAtAGlance)
				{
					height += 120;
				}
				if (item[i].perksAtAGlance)
				{
					height += 20;
				}
			}
			if (alpha < 1f)
			{
				alpha += Game1.frameTime * 5f;
			}
			if (!(alpha >= 1f))
			{
				return;
			}
			alpha = 1f;
			if (iKeys.keyDown)
			{
				CycleOff(selected, item[selected].selX);
				selected = (selected + 1) % item.Length;
				Sound.PlayCue("throw");
				if (selected >= item.Length)
				{
					selected = 0;
				}
				else if (item[selected] == null || item[selected].noSelect)
				{
					selected = (selected + 1) % item.Length;
				}
			}
			if (iKeys.keyUp)
			{
				CycleOff(selected, item[selected].selX);
				selected = (selected + (item.Length - 1)) % item.Length;
				Sound.PlayCue("swing");
				if (selected < 0)
				{
					selected = item.Length - 1;
				}
				else if (item[selected] == null || item[selected].noSelect)
				{
					selected = (selected + (item.Length - 1)) % item.Length;
				}
			}
			if (iKeys.keyAccept && !item[selected].disabled)
			{
				Sound.PlayCue("pop");
				SelectItem(menu);
			}
			if (iKeys.keyY)
			{
				ItemHitY(menu);
			}
			if (iKeys.keyCancel)
			{
				Cancel(menu);
			}
			if (selected > -1 && selected < item.Length)
			{
				int selX = item[selected].selX;
				item[selected].Update(iKeys);
				if (selX != item[selected].selX)
				{
					CycleOff(selected, selX);
				}
			}
		}
		else if (alpha > 0f)
		{
			alpha -= Game1.frameTime * 5f;
		}
	}

	public virtual void ItemHitY(Menu menu)
	{
	}

	public virtual void CheckNewUnlocks()
	{
	}

	public virtual void Draw(SpriteBatch sprite, Menu menu)
	{
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_040f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0414: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0675: Unknown result type (might be due to invalid IL or missing references)
		//IL_068e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0772: Unknown result type (might be due to invalid IL or missing references)
		//IL_0777: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0805: Unknown result type (might be due to invalid IL or missing references)
		//IL_0807: Unknown result type (might be due to invalid IL or missing references)
		if (!(alpha > 0.5f))
		{
			return;
		}
		int num = (int)((float)width * (alpha * 0.5f + 0.5f));
		int num2 = (int)((float)height * (alpha * 0.5f + 0.5f));
		Rectangle rect = new Rectangle(640 - num / 2, 360 - num2 / 2, num, num2);
		float num3 = (alpha - 0.5f) * 2f;
		if (!selOnly)
		{
			if (error != null)
			{
				DrawBox(sprite, rect, new Color(new Vector4(0f, 0f, 0f, 0.85f * num3)), new Color(new Vector4(1f * num3, 0.5f * num3, 0.5f * num3, alpha)));
			}
			else if (GameState.mode == 1)
			{
				DrawBox(sprite, rect, new Color(new Vector4(0f, 0f, 0f, 0.85f * num3)), new Color(0.3f, 0.3f, 0.3f, alpha));
			}
			else
			{
				DrawBox(sprite, rect, new Color(new Vector4(0f, 0f, 0f, 0.5f * num3)), new Color(0.1f, 0.1f, 0.1f, alpha));
			}
		}
		float num4 = (alpha - 0.9f) * 10f;
		if (!(num4 > 0f))
		{
			return;
		}
		sprite.Begin(blendState: BlendState.Additive);
		Game1.text.color = new Color(new Vector4(0.5f, 0.5f, 0.6f, 0.7f * num4));
		if (isStartGate)
		{
			Game1.text.color = new Color(new Vector4(1f, 1f, 1f, 1f * num4));
		}
		Game1.text.size = 1f;
		if (!selOnly)
		{
			Game1.text.DrawString(new Vector2(rect.Center.X, rect.Y) + new Vector2(0f, 12f), name, 1, -1f, Game1.impact, sprite);
		}
		if (isControls)
		{
			sprite.Draw(Game1.controlsTex, new Vector2(640f, 360f), (Rectangle?)new Rectangle(0, 0, 836, 444), new Color(new Vector4(1f, 1f, 1f, num4)), 0f, new Vector2(418f, 222f), 1f, (SpriteEffects)0, 1f);
			Controls controls = (Controls)this;
			controls.DrawControls(num4, sprite);
		}
		float num5 = 0f;
		if (isControls)
		{
			num5 = 410f;
		}
		if (error != null)
		{
			for (int i = 0; i < error.Length; i++)
			{
				if (error[i] != null)
				{
					Game1.text.color = new Color(new Vector4(0.6f, 0.5f, 0.5f, 1f * num4));
					Game1.text.size = 1f;
					Game1.text.DrawString(new Vector2(640f, rect.Y) + new Vector2(10f, 50f + (float)i * 28f), error[i], 1, -1f, Game1.impact, sprite);
					num5 += 28f;
				}
			}
			for (int j = 0; j < item.Length; j++)
			{
				if (selected == j)
				{
					Game1.text.color = new Color(new Vector4(1f, 1f, 1f, 1f * num4));
				}
				else
				{
					Game1.text.color = new Color(new Vector4(1f, 0.8f, 0.8f, 1f * num4));
				}
				Game1.text.DrawString(new Vector2(640f, rect.Y) + new Vector2(10f, 70f + (float)j * 32f + num5), item[j].text, 1, -1f, Game1.impact, sprite);
			}
		}
		else
		{
			for (int k = 0; k < item.Length; k++)
			{
				if (item[k].roster)
				{
					num5 += 40f;
				}
				if (item[k].perk > -1)
				{
					num5 += 40f;
				}
				if (item[k].classAtAGlance)
				{
					num5 += 20f;
				}
				if (item[k].appearanceAtAGlance)
				{
					num5 += 140f;
				}
				if (item[k].perksAtAGlance)
				{
					num5 += 10f;
				}
				num5 += item[k].bump;
				if (!selOnly || k == selected)
				{
					item[k].Draw(sprite, new Vector2(rect.X, (float)rect.Y + num5), selected, num4, width);
				}
				if (item[k].classAtAGlance && Game1.zProfile.unlocks.perkEditorUnlocked > 0 && !item[k].disabled && k == selected)
				{
					sprite.End();
					float num6 = -16f;
					float num7 = 16f;
					bool flag = Game1.zProfile.unlocks.perkEditorUnlocked == 1 || menu.menuLevel[17].item[2].newunlock || menu.menuLevel[17].item[1].newunlock || menu.menuLevel[17].item[0].newunlock;
					if (GameState.mode == 1)
					{
						DrawBox(sprite, new Rectangle(rect.Right + (int)num7, (int)(num5 + (float)k * 32f + num6) + 158, flag ? 120 : 90, 18), new Color(0f, 0f, 0f, 0.85f), new Color(0.3f, 0.3f, 0.3f, 0.85f));
					}
					else
					{
						DrawBox(sprite, new Rectangle(rect.Right + (int)num7, (int)(num5 + (float)k * 32f + num6) + 158, flag ? 120 : 90, 18), new Color(0.1f, 0.1f, 0.1f, 0.5f), new Color(0f, 0f, 0f, 0f));
					}
					sprite.Begin(blendState: BlendState.Additive);
					Game1.text.DrawString(new Vector2((float)rect.Right + num7, num5 + (float)k * 32f + 154f + num6), menu.yString, 0, -1f, Game1.impact, sprite);
					menu.menuLevel[17].CheckNewUnlocks();
					if (flag)
					{
						float size = Game1.text.size;
						Color color = Game1.text.color;
						Game1.text.size = 0.8f;
						Game1.text.color = new Color(1f, 1f, 1f, Rand.GetRandomFloat(0.5f, 1f));
						Game1.text.DrawString(new Vector2((float)rect.Right + 78f + num7, num5 + (float)k * 32f + 154f + num6), Game1.menu.newString, 0, -1f, Game1.impact, sprite);
						Game1.text.color = color;
						Game1.text.size = size;
					}
				}
				if (item[k].perk > -1)
				{
					num5 += 60f;
				}
				if (item[k].perksAtAGlance)
				{
					num5 += 10f;
				}
			}
		}
		sprite.End();
	}

	public static void DrawBox(SpriteBatch sprite, Rectangle rect, Color inner, Color outer)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		DrawBox(sprite, rect, inner, bright: false);
		DrawBox(sprite, rect, outer, bright: true);
	}

	public static void DrawBox(SpriteBatch sprite, Rectangle rect, Color c, bool bright)
	{
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		rect.X -= 16;
		rect.Y -= 16;
		rect.Width += 32;
		rect.Height += 32;
		int num = 40;
		int num2 = 712;
		if (bright)
		{
			num += 128;
			sprite.Begin(blendState: BlendState.Additive);
		}
		else
		{
			sprite.Begin(blendState: BlendState.AlphaBlend);
		}
		sprite.Draw(Game1.spritesTex, new Rectangle(rect.X, rect.Y, 16, 16), (Rectangle?)new Rectangle(num, num2, 16, 16), c);
		sprite.Draw(Game1.spritesTex, new Rectangle(rect.X + rect.Width - 16, rect.Y, 16, 16), (Rectangle?)new Rectangle(num + 32, num2, 16, 16), c);
		sprite.Draw(Game1.spritesTex, new Rectangle(rect.X, rect.Y + rect.Height - 16, 16, 16), (Rectangle?)new Rectangle(num, num2 + 32, 16, 16), c);
		sprite.Draw(Game1.spritesTex, new Rectangle(rect.X + rect.Width - 16, rect.Y + rect.Height - 16, 16, 16), (Rectangle?)new Rectangle(num + 32, num2 + 32, 16, 16), c);
		sprite.Draw(Game1.spritesTex, new Rectangle(rect.X + 16, rect.Y, rect.Width - 32, 16), (Rectangle?)new Rectangle(num + 16, num2, 16, 16), c);
		sprite.Draw(Game1.spritesTex, new Rectangle(rect.X, rect.Y + 16, 16, rect.Height - 32), (Rectangle?)new Rectangle(num, num2 + 16, 16, 16), c);
		sprite.Draw(Game1.spritesTex, new Rectangle(rect.X + 16, rect.Bottom - 16, rect.Width - 32, 16), (Rectangle?)new Rectangle(num + 16, num2 + 32, 16, 16), c);
		sprite.Draw(Game1.spritesTex, new Rectangle(rect.Right - 16, rect.Y + 16, 16, rect.Height - 32), (Rectangle?)new Rectangle(num + 32, num2 + 16, 16, 16), c);
		if (!bright)
		{
			sprite.Draw(Game1.spritesTex, new Rectangle(rect.X + 16, rect.Y + 16, rect.Width - 32, rect.Height - 32), (Rectangle?)new Rectangle(num + 16, num2 + 16, 16, 16), c);
		}
		sprite.End();
	}
}
