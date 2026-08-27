using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZP2K9;

public class Loader
{
	public bool loadComplete;

	public float splashFrame;

	public float loadFrame;

	public float loadOutFrame;

	public bool loadBegin;

	private StringBuilder zp2kx = new StringBuilder("ZP2KX");

	private StringBuilder[] zp2kdesc = new StringBuilder[4]
	{
		new StringBuilder("Zombies"),
		new StringBuilder("and"),
		new StringBuilder("Pterodactyls"),
		new StringBuilder("20XX")
	};

	private StringBuilder[] loadedText = new StringBuilder[192];

	public Loader()
	{
		int randomInt = Rand.GetRandomInt(0, 3);
		for (int i = 0; i < loadedText.Length; i++)
		{
			string text = "";
			if (i < 128)
			{
				switch (Rand.GetRandomInt(0, 10))
				{
				case 0:
					text = "ZSX";
					break;
				case 1:
					text = ".";
					break;
				case 2:
					text = "###";
					break;
				case 3:
					text = "|";
					break;
				case 4:
					text = "#DES";
					break;
				case 5:
					text = "#ENC";
					break;
				case 6:
					text = "##FIX##";
					break;
				case 7:
					text = "UNPACK#";
					break;
				case 8:
					text = "DAT:";
					break;
				case 9:
					text = "MOV:";
					break;
				}
				for (int j = 0; j < 3; j++)
				{
					if (Rand.CointToss(0.2f))
					{
						text += Rand.GetRandomInt(0, 100);
					}
					if (Rand.CointToss(0.2f))
					{
						text += Rand.GetRandomInt(0, 1000);
					}
					switch (Rand.GetRandomInt(0, 10))
					{
					case 0:
						text += ">>";
						break;
					case 1:
						text += "--";
						break;
					case 2:
						text += " ";
						break;
					case 3:
						text += "<<";
						break;
					case 4:
						text += "###";
						break;
					case 5:
						text += "#";
						break;
					case 6:
						text += ":-";
						break;
					case 7:
						text += "=";
						break;
					case 8:
						text += "/";
						break;
					case 9:
						text += ".";
						break;
					}
					if (Rand.CointToss(0.2f))
					{
						text += Rand.GetRandomInt(0, 1000);
					}
					if (Rand.CointToss(0.2f))
					{
						text += Rand.GetRandomInt(0, 100);
					}
					switch (Rand.GetRandomInt(0, 10))
					{
					case 0:
						text += ">>";
						break;
					case 1:
						text += "--";
						break;
					case 2:
						text += " ";
						break;
					case 3:
						text += "<<";
						break;
					case 4:
						text += "###";
						break;
					case 5:
						text += "#";
						break;
					case 6:
						text += "********";
						break;
					case 7:
						text += "=";
						break;
					case 8:
						text += "/";
						break;
					case 9:
						text += ".";
						break;
					}
					if (Rand.CointToss(0.2f))
					{
						text += Rand.GetRandomInt(0, 1000);
					}
					switch (Rand.GetRandomInt(0, 10))
					{
					case 0:
						text += ">>";
						break;
					case 1:
						text += "--";
						break;
					case 2:
						text += " ";
						break;
					case 3:
						text += "<<";
						break;
					case 4:
						text += "###";
						break;
					case 5:
						text += "#";
						break;
					case 6:
						text += ":-";
						break;
					case 7:
						text += "=";
						break;
					case 8:
						text += "/";
						break;
					case 9:
						text += "......";
						break;
					}
				}
				if (text.Length > 15)
				{
					text = text.Substring(0, 15);
				}
			}
			else
			{
				int num = i - 128;
				switch (randomInt)
				{
				case 0:
					switch (num % 4)
					{
					case 0:
						text = ((!Rand.CointToss(0.5f)) ? ((!Rand.CointToss(0.33f)) ? "allllllllworkand" : "ALL WORK AND") : "all W0rk and");
						break;
					case 1:
						text = ((!Rand.CointToss(0.5f)) ? ((!Rand.CointToss(0.33f)) ? "noooo playyyyy" : "NO PLAYY") : "n0 play");
						break;
					case 2:
						text = ((!Rand.CointToss(0.5f)) ? ((!Rand.CointToss(0.33f)) ? "makesmeeeeegoooo" : "MAKEZ JAMEZ G0") : "makes james g0");
						break;
					case 3:
						text = ((!Rand.CointToss(0.5f)) ? ((!Rand.CointToss(0.33f)) ? "crazycrazycrazy" : "CRAZAYYYY!!!111") : "crazy");
						break;
					}
					break;
				case 1:
					switch (num % 4)
					{
					case 0:
						text = ((!Rand.CointToss(0.5f)) ? ((!Rand.CointToss(0.33f)) ? "noootv and" : "NO TEEVEE AND") : "n0 tV and");
						break;
					case 1:
						text = ((!Rand.CointToss(0.5f)) ? ((!Rand.CointToss(0.33f)) ? "noooo beeeer" : "NO BEERZ") : "no b33r");
						break;
					case 2:
						text = ((!Rand.CointToss(0.5f)) ? ((!Rand.CointToss(0.33f)) ? "makesmeeeeegoooo" : "MAKEZ JAMEZ G0") : "makes james g0");
						break;
					case 3:
						text = ((!Rand.CointToss(0.5f)) ? ((!Rand.CointToss(0.33f)) ? "crazycrazycrazy" : "CRAZAYYYY!!!111") : "crazy");
						break;
					}
					break;
				case 2:
					switch (num % 4)
					{
					case 0:
						text = ((!Rand.CointToss(0.5f)) ? ((!Rand.CointToss(0.33f)) ? "crawwwwwling crawwwling" : "crawling CRAWLING") : "the bugz are");
						break;
					case 1:
						text = ((!Rand.CointToss(0.5f)) ? ((!Rand.CointToss(0.33f)) ? "crawlingcrawling" : "BUGZ BUGZ BUGZ") : "crawlzing the");
						break;
					case 2:
						text = ((!Rand.CointToss(0.5f)) ? ((!Rand.CointToss(0.33f)) ? "olololzolzolzolz" : "AND TERRORZ IZ") : "is ok is ok");
						break;
					case 3:
						text = ((!Rand.CointToss(0.5f)) ? ((!Rand.CointToss(0.33f)) ? "crazycrazycrazy" : "CRAZAYYYY!!!111") : "fix f1X f1x fx");
						break;
					}
					break;
				case 3:
					switch (num % 4)
					{
					case 0:
						text = ((!Rand.CointToss(0.5f)) ? ((!Rand.CointToss(0.33f)) ? "allllllllworkand" : "ALL WORK AND") : "all W0rk and");
						break;
					case 1:
						text = ((!Rand.CointToss(0.5f)) ? ((!Rand.CointToss(0.33f)) ? "noooo playyyyy" : "NO PLAYY") : "n0 play");
						break;
					case 2:
						text = ((!Rand.CointToss(0.5f)) ? ((!Rand.CointToss(0.33f)) ? "makesmeeeeegoooo" : "MAKEZ JAMEZ G0") : "makes james g0");
						break;
					case 3:
						text = ((!Rand.CointToss(0.5f)) ? ((!Rand.CointToss(0.33f)) ? "crazycrazycrazy" : "CRAZAYYYY!!!111") : "crazy");
						break;
					}
					break;
				}
			}
			loadedText[i] = new StringBuilder(text);
		}
	}

	public void Update()
	{
		if (splashFrame < 3f)
		{
			splashFrame += Game1.frameTime;
			return;
		}
		loadFrame += Game1.frameTime;
		if (loadFrame >= 1f && loadComplete)
		{
			loadOutFrame += Game1.frameTime;
		}
	}

	public bool IsDone()
	{
		if (loadComplete)
		{
			return loadOutFrame >= 1f;
		}
		return false;
	}

	public void Draw(SpriteBatch sprite)
	{
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		if (splashFrame < 3f)
		{
			float num = 1f;
			if (splashFrame < 0.5f)
			{
				num = splashFrame * 2f;
			}
			else if (splashFrame > 2.5f)
			{
				num = (3f - splashFrame) * 2f;
			}
			sprite.Begin(blendState: BlendState.AlphaBlend);
			sprite.Draw(Game1.skaLogoTex, new Vector2(640f, 360f), (Rectangle?)new Rectangle(0, 0, 400, 268), new Color(new Vector4(1f, num, num, num)), 0f, new Vector2(200f, 134f), 1f, (SpriteEffects)0, 1f);
			sprite.End();
			return;
		}
		sprite.Begin(blendState: BlendState.AlphaBlend);
		Game1.text.size = 4f;
		Game1.text.color = new Color(1f, 1f, 1f, 1f);
		Vector2 vector = new Vector2(50f, 50f);
		sprite.Draw(Game1.zp2kxTex, vector, Color.White);
		Game1.text.size = 2f;
		for (int i = 0; i < 4; i++)
		{
			Game1.text.DrawString(vector + new Vector2(6f, 132f + (float)i * 30f), zp2kdesc[i], 0, -1f, Game1.impact, sprite);
		}
		int num2 = (int)(loadFrame * 8f);
		if (num2 > loadedText.Length - 1)
		{
			num2 = loadedText.Length - 1;
		}
		Game1.text.size = 1f;
		for (int j = 0; j < num2; j++)
		{
			int num3 = j;
			if (num2 > 10)
			{
				num3 -= num2 - 10;
			}
			if (num3 >= 0)
			{
				float num4 = 1f;
				if (j >= 128)
				{
					num4 = 0f;
				}
				if (num3 == 0 || num3 == 9)
				{
					Game1.text.color = new Color(0.5f, num4 * 0.5f, num4 * 0.5f, 1f);
				}
				else
				{
					Game1.text.color = new Color(1f, num4, num4, 1f);
				}
				Game1.text.DrawString(vector + new Vector2(1140f, 332f + (float)num3 * 20f), loadedText[j], 2, -1f, Game1.impact, sprite);
			}
		}
		//sprite.Draw(Game1.spritesTex, new Vector2(640f, 360f), (Rectangle?)new Rectangle(0, 832, 192, 192), new Color(1f, 1f, 1f, 0.1f), 0f, new Vector2(96f, 96f), 8f, (SpriteEffects)0, 1f);
		sprite.Draw(Game1.spritesTex, new Vector2(640f, 360f) + new Vector2(100f, -100f), (Rectangle?)new Rectangle(0, 832, 192, 192), new Color(1f, 1f, 1f, 0.1f), 0f, new Vector2(96f, 96f), 5f, (SpriteEffects)0, 1f);
		float num5 = 0f;
		if (loadFrame < 1f)
		{
			num5 = 1f - loadFrame;
		}
		if (loadOutFrame > 0f)
		{
			num5 = loadOutFrame;
		}
		if (num5 > 0f)
		{
			sprite.Draw(Game1.nullTex, new Rectangle(0, 0, 1280, 720), new Color(0f, 0f, 0f, num5));
		}
		sprite.End();
	}
}
