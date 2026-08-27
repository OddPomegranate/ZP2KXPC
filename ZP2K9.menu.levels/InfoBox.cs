using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZP2K9.menu.levels;

public class InfoBox
{
	public StringBuilder name;

	public StringBuilder score;

	public StringBuilder level;

	public StringBuilder time;

	public int iLevel;

	public bool active;

	public float alpha;

	public void Init(string name, long score, long next, long level, long time)
	{
		this.name = new StringBuilder(name);
		if (level < 99)
		{
			this.score = new StringBuilder("Score: " + score + "/" + next);
		}
		else
		{
			this.score = new StringBuilder("Score: " + score);
		}
		this.level = new StringBuilder("Level: " + (level + 1));
		iLevel = (int)level;
		long num = (int)time;
		long num2 = num % 60;
		string text = num2.ToString();
		if (num2 < 10)
		{
			text = "0" + text;
		}
		num /= 60;
		long num3 = num % 60;
		text = num3 + ":" + text;
		if (num3 < 10 && num >= 60)
		{
			text = "0" + text;
		}
		if (num >= 60)
		{
			num /= 60;
			text = num % 24 + ":" + text;
			if (num >= 24)
			{
				num /= 24;
				long num4 = num;
				text = num4 + "d " + text;
			}
		}
		this.time = new StringBuilder("Time played: " + text);
		active = true;
	}

	public void Update()
	{
		if (active)
		{
			if (alpha < 1f)
			{
				alpha += Game1.frameTime * 2f;
				if (alpha > 0.5f)
				{
					alpha += Game1.frameTime * 5f;
				}
				if (alpha > 1f)
				{
					alpha = 1f;
				}
			}
			if (GameState.mode == 1 && !Game1.menu.menuLevel[9].active)
			{
				active = false;
			}
		}
		else if (alpha > 0f)
		{
			alpha -= Game1.frameTime * 5f;
			if (alpha < 0f)
			{
				alpha = 0f;
			}
		}
	}

	public void Draw(SpriteBatch sprite)
	{
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		if (alpha > 0.5f)
		{
			float num = (alpha - 0.5f) * 2f;
			float num2 = 1f - alpha;
			num2 *= 10f;
			Vector2 vector = new Vector2(100f, 520f);
			if (GameState.mode == 2)
			{
				vector.Y -= 50f;
			}
			MenuLevel.DrawBox(sprite, new Rectangle((int)vector.X + (int)num2, (int)vector.Y + 2 + (int)num2, 250 - (int)(num2 * 2f), 116 - (int)(num2 * 2f)), new Color(0f, 0f, 0f, 1f * num), new Color(0.6f, 0.7f, 1f, 1f * num));
			Game1.text.size = 1f;
			Game1.text.color = new Color(1f, 1f, 1f, 1f * num);
			if (alpha >= 1f)
			{
				sprite.Begin(blendState: BlendState.Additive);
				sprite.Draw(Game1.badgesTex, vector + new Vector2(220f, 30f), (Rectangle?)new Rectangle(iLevel % 10 * 128, iLevel / 10 * 128, 128, 128), new Color(1f, 1f, 1f, alpha), 0f, new Vector2(64f, 64f), 0.4f, (SpriteEffects)0, 1f);
				Game1.text.DrawString(vector + new Vector2(8f, 8f), name, 0, -1f, Game1.impact, sprite);
				Game1.text.color = new Color(0.6f, 0.7f, 1f, 1f * num);
				float num3 = 40f;
				float num4 = 24f;
				Game1.text.DrawString(vector + new Vector2(8f, num3 + 0f * num4), level, 0, -1f, Game1.impact, sprite);
				Game1.text.DrawString(vector + new Vector2(8f, num3 + 1f * num4), score, 0, -1f, Game1.impact, sprite);
				Game1.text.DrawString(vector + new Vector2(8f, num3 + 2f * num4), time, 0, -1f, Game1.impact, sprite);
				sprite.End();
			}
		}
	}
}
