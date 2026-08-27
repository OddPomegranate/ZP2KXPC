using System.Collections.ObjectModel;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ZP2K9.menu;
using ZP2K9.net;

namespace ZP2K9.hud;

public class Scoreboard
{
	public StringBuilder[] charName;

	public StringBuilder[] clanTag;

	private StringBuilder mapName;

	private StringBuilder victoryConditions;

	private StringBuilder mutator;

	public float alpha;

	public bool vis;

	public StringBuilder name = new StringBuilder("Name");

	public StringBuilder kills = new StringBuilder("Score");

	public StringBuilder deaths = new StringBuilder("Deaths");

	public StringBuilder humans = new StringBuilder("Humans:");

	public StringBuilder zombies = new StringBuilder("Zombies:");

	private float updateFrame;

	private int[] list = new int[32];

	public Scoreboard()
	{
		charName = new StringBuilder[32];
		clanTag = new StringBuilder[32];
		Reset();
	}

	public void Reset()
	{
		for (int i = 0; i < charName.Length; i++)
		{
			charName[i] = null;
		}
	}

	public void Update(InterfaceKeys ikeys)
	{
		if (ikeys.keySelect)
		{
			vis = !vis;
			if (vis)
			{
				SetMapName();
			}
		}
		if (Game1.netSession.netType == 1)
		{
			vis = false;
		}
		if (Game1.netSession.postLobby)
		{
			vis = false;
		}
		if (vis || Game1.netSession.postLobby)
		{
			if (alpha < 1f)
			{
				alpha += Game1.frameTime * 4f;
				if (alpha > 1f)
				{
					alpha = 1f;
				}
			}
		}
		else if (alpha > 0f)
		{
			alpha -= Game1.frameTime * 4f;
			if (alpha < 0f)
			{
				alpha = 0f;
			}
		}
		updateFrame -= Game1.frameTime;
		if (!(updateFrame < 0f))
		{
			return;
		}
		updateFrame = 2f;
		if (Game1.netSession.netType == 3 || Game1.netSession.netType == 2)
		{
			if (Game1.netSession.netSession != null && (Game1.netSession.netSession.AllGamers).Count > 0)
			{
				for (int i = 0; i < (Game1.netSession.netSession.AllGamers).Count; i++)
				{
					INetworkGamer val = (Game1.netSession.netSession.AllGamers)[i];
					if (!Game1.netSession.playerList.ContainsKey(val.Id))
					{
						continue;
					}
					int num = Game1.netSession.playerList[val.Id];
					if (charName[num] == null)
					{
						charName[num] = new StringBuilder(val.Gamertag);
					}
					else if (charName[num].ToString() != val.Gamertag)
					{
						charName[num] = new StringBuilder(val.Gamertag);
						if (Game1.character[num] != null)
						{
							Game1.character[num].clanChar[0] = '\0';
							Game1.character[num].clanChar[1] = '\0';
							Game1.character[num].clanChar[2] = '\0';
						}
					}
					if (Game1.character[num] == null)
					{
						continue;
					}
					bool flag = false;
					if (clanTag[num] == null)
					{
						if (Game1.character[num].clanChar[0] != 0 || Game1.character[num].clanChar[1] != 0 || Game1.character[num].clanChar[2] != 0)
						{
							flag = true;
						}
					}
					else if (clanTag[num][1] != Game1.character[num].clanChar[0] || clanTag[num][2] != Game1.character[num].clanChar[1] || clanTag[num][3] != Game1.character[num].clanChar[2])
					{
						flag = true;
					}
					if (flag)
					{
						if (Game1.character[num].clanChar[0] == '\0' && Game1.character[num].clanChar[1] == '\0' && Game1.character[num].clanChar[2] == '\0')
						{
							clanTag[num] = null;
						}
						else if (Game1.character[num].clanChar[2] != 0)
						{
							clanTag[num] = new StringBuilder("[" + Game1.character[num].clanChar[0] + Game1.character[num].clanChar[1] + Game1.character[num].clanChar[2] + "]");
						}
						else if (Game1.character[num].clanChar[1] != 0)
						{
							clanTag[num] = new StringBuilder("[" + Game1.character[num].clanChar[0] + Game1.character[num].clanChar[1] + "]");
						}
						else
						{
							clanTag[num] = new StringBuilder("[" + Game1.character[num].clanChar[0] + "]");
						}
					}
				}
			}
			for (int j = 20; j < Game1.character.Length; j++)
			{
				if (Game1.character[j] != null && charName[j] == null)
				{
					charName[j] = Game1.botBag.Style(j).name;
					clanTag[j] = null;
				}
			}
			return;
		}
		for (int k = 0; k < Game1.character.Length; k++)
		{
			if (Game1.character[k] != null && charName[k] == null)
			{
				if (Game1.character[k].ai != null)
				{
					charName[k] = Game1.botBag.Style(k).name;
				}
				else
				{
					charName[k] = new StringBuilder("Player" + k);
				}
			}
		}
	}

	public void SetMapName()
	{
		string text = "Deathmatch";
		switch (GameState.gameType)
		{
		case 0:
			victoryConditions = new StringBuilder("To " + Game1.netSession.DMScores[Game1.netSession.DMScoreIdx] + " points");
			break;
		case 2:
			text = "Capture the Flag";
			victoryConditions = new StringBuilder("To " + Game1.netSession.CTFScores[Game1.netSession.CTFScoreIdx] + " captures");
			break;
		case 1:
			text = "Team Deathmatch";
			victoryConditions = new StringBuilder("To " + Game1.netSession.TDMScores[Game1.netSession.TDMScoreIdx] + " points");
			break;
		case 4:
			text = "Zombie Hunt";
			victoryConditions = new StringBuilder("To " + Game1.netSession.ZHScores[Game1.netSession.ZHScoreIdx] + " points");
			break;
		case 3:
		{
			text = "King of the Hill";
			string text2 = Game1.netSession.KOTHScores[Game1.netSession.KOTHScoreIdx] / 60f + ":00";
			victoryConditions = new StringBuilder("To " + text2);
			break;
		}
		}
		mapName = new StringBuilder(text + " on " + MapList.mapCatalog[Game1.netSession.netPlay.currentMap].name.ToString());
		if (Game1.netSession.mutator > 0)
		{
			mutator = new StringBuilder("Mutator: " + Mutators.mutator[Game1.netSession.mutator]);
		}
		else
		{
			mutator = null;
		}
	}

	public void Draw(SpriteBatch sprite)
	{
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_061a: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0842: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0899: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0419: Unknown result type (might be due to invalid IL or missing references)
		//IL_046a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0676: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0561: Unknown result type (might be due to invalid IL or missing references)
		//IL_08fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0704: Unknown result type (might be due to invalid IL or missing references)
		//IL_0771: Unknown result type (might be due to invalid IL or missing references)
		int num = -180;
		int num2 = 0;
		if (!(alpha > 0f))
		{
			return;
		}
		for (int i = 0; i < charName.Length; i++)
		{
			list[i] = -1;
			if (charName[i] != null && Game1.character[i] != null)
			{
				list[i] = i;
			}
		}
		for (int j = 0; j < list.Length; j++)
		{
			for (int k = 0; k < list.Length - 1; k++)
			{
				int num3 = list[k];
				int num4 = list[k + 1];
				bool flag = false;
				if (num3 < 0 || num4 < 0)
				{
					if (num3 < 0 && num4 >= 0)
					{
						flag = true;
					}
				}
				else if (Game1.character[num3].score < Game1.character[num4].score)
				{
					flag = true;
				}
				if (flag)
				{
					int num5 = list[k];
					list[k] = list[k + 1];
					list[k + 1] = num5;
				}
			}
		}
		float num6 = 5f;
		sprite.Draw(Game1.nullTex, new Rectangle(num + 600, num2 + 70, 500, 30), new Color(new Vector4(0f, 0f, 0f, 0.6f * alpha)));
		sprite.DrawString(Game1.impact, name, new Vector2((float)num + 600f + num6, (float)num2 + 70f), new Color(new Vector4(1f, 1f, 1f, alpha)));
		sprite.DrawString(Game1.impact, kills, new Vector2((float)num + 900f + num6, (float)num2 + 70f), new Color(new Vector4(1f, 1f, 1f, alpha)));
		int num7 = 0;
		int num8 = 0;
		int num9 = 0;
		Color val = default(Color);
		for (int l = 0; l < list.Length; l++)
		{
			int num10 = list[l];
			if (num10 > -1 && charName[num10] != null && Game1.character[num10] != null)
			{
				num7++;
				val = new Color(new Vector4(1f, 1f, 1f, alpha));
				switch (Game1.character[num10].GetTeam())
				{
				case 1:
					val = new Color(new Vector4(0.7f, 0.7f, 1f, alpha));
					num8 += Game1.character[num10].score;
					break;
				case 2:
					val = new Color(new Vector4(1f, 0.7f, 0.7f, alpha));
					num9 += Game1.character[num10].score;
					break;
				}
				sprite.Draw(Game1.nullTex, new Rectangle(num + 600, num2 + 110 + l * 32, 500, 30), new Color(new Vector4(0f, 0f, 0f, 0.5f * alpha)));
				if (Game1.character[num10].bodyType != 2)
				{
					sprite.Draw(Game1.badgesTex, new Vector2((float)num + 630f + num6, (float)num2 + 112f + (float)l * 32f), (Rectangle?)new Rectangle(Game1.character[num10].level % 10 * 128, Game1.character[num10].level / 10 * 128, 128, 128), new Color(1f, 1f, 1f, alpha), 0f, default(Vector2), 0.2f, (SpriteEffects)0, 1f);
				}
				if (clanTag[num10] != null)
				{
					sprite.DrawString(Game1.impact, clanTag[num10], new Vector2((float)num + 660f + num6, (float)num2 + 110f + (float)l * 32f), val);
					sprite.DrawString(Game1.impact, charName[num10], new Vector2((float)num + 660f + num6 + Game1.impact.MeasureString(clanTag[num10]).X, (float)num2 + 110f + (float)l * 32f), val);
				}
				else
				{
					sprite.DrawString(Game1.impact, charName[num10], new Vector2((float)num + 660f + num6, (float)num2 + 110f + (float)l * 32f), val);
				}
				sprite.DrawString(Game1.impact, Numbers.GetNumber(Game1.character[num10].score), new Vector2((float)num + 900f + num6, (float)num2 + 110f + (float)l * 32f), val);
				sprite.DrawString(Game1.impact, Numbers.GetNumber(Game1.character[num10].level + 1), new Vector2((float)num + 630f + num6 - Game1.impact.MeasureString(Numbers.GetNumber(Game1.character[num10].level + 1)).X, (float)num2 + 110f + (float)l * 32f), new Color(1f, 1f, 1f, alpha));
			}
		}
		switch (GameState.gameType)
		{
		case 1:
		case 2:
		case 3:
		case 4:
		{
			for (int m = 0; m < 2; m++)
			{
				int num11 = num7 + 1;
				sprite.Draw(Game1.nullTex, new Rectangle(num + 600, num2 + 110 + (num11 + m) * 32, 500, 30), (m == 0) ? new Color(0.1f, 0.1f, 0.6f, alpha * 0.5f) : new Color(0.6f, 0.1f, 0.1f, alpha * 0.5f));
				sprite.DrawString(Game1.impact, (m == 0) ? humans : zombies, new Vector2((float)num + 630f + num6, (float)num2 + 110f + (float)(num11 + m) * 32f), new Color(1f, 1f, 1f, alpha));
				switch (GameState.gameType)
				{
				case 1:
				case 2:
				case 4:
					sprite.DrawString(Game1.impact, Numbers.GetNumber((m == 0) ? Game1.netSession.blueScore : Game1.netSession.redScore), new Vector2((float)num + 850f + num6, (float)num2 + 110f + (float)(num11 + m) * 32f), new Color(1f, 1f, 1f, alpha));
					break;
				case 3:
					sprite.DrawString(Game1.impact, Numbers.GetTime((m == 0) ? ((int)Game1.netSession.blueTime) : ((int)Game1.netSession.redTime)), new Vector2((float)num + 850f + num6, (float)num2 + 110f + (float)(num11 + m) * 32f), new Color(1f, 1f, 1f, alpha));
					break;
				}
			}
			num7 += 2;
			break;
		}
		}
		sprite.Draw(Game1.nullTex, new Rectangle(num + 600, (int)((float)num2 + 110f + (float)(num7 + 1) * 32f), 500, 56 + ((Game1.netSession.mutator > 0) ? 26 : 0)), new Color(0f, 0f, 0f, 0.4f * alpha));
		if (mapName != null)
		{
			sprite.DrawString(Game1.impact, mapName, new Vector2((float)num + 630f + num6, (float)num2 + 110f + (float)(num7 + 1) * 32f), new Color(1f, 1f, 1f, alpha));
		}
		if (victoryConditions != null)
		{
			sprite.DrawString(Game1.impact, victoryConditions, new Vector2((float)num + 630f + num6, (float)num2 + 136f + (float)(num7 + 1) * 32f), new Color(1f, 1f, 1f, alpha));
		}
		if (Game1.netSession.mutator > 0 && mutator != null)
		{
			sprite.DrawString(Game1.impact, mutator, new Vector2((float)num + 630f + num6, (float)num2 + 162f + (float)(num7 + 1) * 32f), new Color(1f, 1f, 1f, alpha));
		}
	}
}
