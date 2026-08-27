using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZP2K9.menu;

public class Ticker
{
	private const float BUFFER = 100f;

	private StringBuilder[] news;

	private float[] xLoc;

	public Ticker()
	{
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		news = new StringBuilder[33]
		{
			new StringBuilder("Remember: double XP every night at 9:00 PM local time!"),
			new StringBuilder("Someone set you on fire? Stop, drop and roll! (LT)"),
			new StringBuilder("ZP2KX Happy Hour is 9 PM local time!  Double XP!"),
			new StringBuilder("Running along a ceiling is a good way to rid your body of poison!"),
			new StringBuilder("Have you tried out Zombie Hunt yet?"),
			new StringBuilder("Pterodactyls just love a good battle!"),
			new StringBuilder("Modify your controls to go more twin-stick or more classic in Settings."),
			new StringBuilder("Want double XP? Jump in 9:00 PM local time!"),
			new StringBuilder("Unlock class slots to be able to modify your classes."),
			new StringBuilder("I MAED A GAM3 W1TH Z0MB1ES 1N IT!!!1 (it's called The Dishwasher: Dead Samurai)"),
			new StringBuilder("Like ZP2KX? Rate it a 5! Don't like it? Keep your opinion to yourself!"),
			new StringBuilder("Fun Fact: ZP2KX is a defacto sequel to an old, old Windows game, ZP2K5."),
			new StringBuilder("ZP2KX: Now with actual pterodactyls!"),
			new StringBuilder("Check out www.ska-studios.com for updates on current projects!"),
			new StringBuilder("We didn't sell out. We bought in."),
			new StringBuilder("Online ghost town trouble? Come back at 9 PM local time for double XP!"),
			new StringBuilder("Now with 0% Avatars!"),
			new StringBuilder("Mix and match skills for maximum awesome!"),
			new StringBuilder("If a robot can bleed, can a robot love?"),
			new StringBuilder("All likenesses in this game are coincidental and/or meant for satirical purposes."),
			new StringBuilder("This game will not mine all your data, partly because XNA won't let us."),
			new StringBuilder("Hop on every day at 9 PM for double XP!"),
			new StringBuilder("Follow us on twitter @skastudios!"),
			new StringBuilder("If you don't have the latest version, you can delete the game and redownload from the Games Marketplace without losing any data."),
			new StringBuilder("Tired of life? Hold LT + Down + X to snuff it out."),
			new StringBuilder("Click LS to humiliate enemies (and fight off poisoning, coincidentally)"),
			new StringBuilder("You can self-terminate! (not sure why) Hold LT + Down + X."),
			new StringBuilder("<3 <3 <3"),
			new StringBuilder("Dirty word filter is in full effect on Clan Tags!"),
			new StringBuilder("Try out the Explosionade mutation!  Better yet, try out Explosionade!"),
			new StringBuilder("Version 2.0.6: Fixed some bugs, balanced Zombie Hunt. Check www.zp2kx.com for the full list."),
			new StringBuilder("Have a suggestion? Tweet it to us at @skastudios!"),
			new StringBuilder("Remember: when you assume, you make a masamune.")
		};
		xLoc = new float[news.Length];
		for (int i = 0; i < news.Length; i++)
		{
			int randomInt = Rand.GetRandomInt(0, news.Length);
			string value = news[i].ToString();
			news[i] = new StringBuilder(news[randomInt].ToString());
			news[randomInt] = new StringBuilder(value);
		}
		Game1.text.size = 0.9f;
		Game1.text.color = new Color(1f, 1f, 1f, 1f);
		for (int j = 1; j < xLoc.Length; j++)
		{
			xLoc[j] = xLoc[j - 1] + Game1.text.GetStringLength(news[j - 1], Game1.impact) + 100f;
		}
	}

	public void Update()
	{
		Game1.text.size = 0.9f;
		for (int i = 0; i < xLoc.Length; i++)
		{
			xLoc[i] -= Game1.frameTime * 80f;
			if (!(xLoc[i] + Game1.text.GetStringLength(news[i], Game1.impact) < 0f))
			{
				continue;
			}
			float num = 0f;
			for (int j = 0; j < xLoc.Length; j++)
			{
				float num2 = xLoc[j] + Game1.text.GetStringLength(news[j], Game1.impact);
				if (num2 > num)
				{
					num = num2;
				}
			}
			xLoc[i] = num + 100f;
		}
	}

	public void Draw(SpriteBatch sprite, float alpha)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		Game1.text.size = 0.9f;
		Game1.text.color = new Color(1f, 1f, 1f, alpha);
		float num = 630f;
		sprite.Draw(Game1.nullTex, new Vector2(0f, num), (Rectangle?)new Rectangle(0, 0, 1, 1), new Color(0f, 0f, 0f, 0.65f * alpha), 0f, default(Vector2), new Vector2(1280f, 24f), (SpriteEffects)0, 1f);
		for (int i = 0; i < news.Length; i++)
		{
			if (xLoc[i] - 50f < 1280f)
			{
				Game1.text.DrawString(new Vector2(xLoc[i], num), news[i], 0, -1f, Game1.impact, sprite);
				sprite.Draw(Game1.nullTex, new Vector2(xLoc[i] - 50f, num + 10f), (Rectangle?)new Rectangle(0, 0, 1, 1), new Color(1f, 1f, 1f, alpha), 0f, default(Vector2), 5f, (SpriteEffects)0, 1f);
			}
		}
	}
}
