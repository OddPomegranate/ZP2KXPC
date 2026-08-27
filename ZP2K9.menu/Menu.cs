using System;
using System.Text;
using Microsoft.Xna.Framework;
using ZP2K9.platform;
using Microsoft.Xna.Framework.Graphics;
using ZP2K9.hud;
using ZP2K9.menu.levels;
using ZP2K9.store;

namespace ZP2K9.menu;

public class Menu
{
	public const int LEVEL_MAIN = 0;

	public const int LEVEL_QUITYOUSURE = 1;

	public const int LEVEL_EDITOR = 2;

	public const int LEVEL_HOSTINGSYSTEMLINK = 3;

	public const int LEVEL_LOBBY = 4;

	public const int LEVEL_SYSTEMLINK = 5;

	public const int LEVEL_XBOXLIVE = 6;

	public const int LEVEL_SEARCHING = 7;

	public const int LEVEL_LISTGAMES = 8;

	public const int LEVEL_GAMEMAIN = 9;

	public const int LEVEL_SETUP_APPEARANCE = 10;

	public const int LEVEL_SERVER_SETUP = 11;

	public const int LEVEL_ERROR = 12;

	public const int LEVEL_PRESS_START = 13;

	public const int LEVEL_CONTROLS = 14;

	public const int LEVEL_SERVERSETTINGS = 15;

	public const int LEVEL_GAMESETTINGS = 16;

	public const int LEVEL_PLAYER_SETUP = 17;

	public const int LEVEL_PERKS = 18;

	public const int LEVEL_CLASSLIST = 19;

	public const int LEVEL_EDITMAPLIST = 20;

	public const int LEVEL_CUSTOMIZESERVER = 21;

	public const int LEVEL_DEBUG = 22;

	public const int ERR_FLAG_NONE = 0;

	public const int ERR_FLAG_REHOST = 1;

	public IAsyncResult keyResult;

	public bool waitingKeyResult;

	public StringBuilder newString;

	public StringBuilder yString;

	public Saving saving;

	public MenuLevel[] menuLevel = new MenuLevel[23]
	{
		new Main(),
		new QuitYouSure(),
		new Editor(),
		new HostingSystemLink(),
		new Lobby(),
		new SystemLink(),
		new XboxLive(),
		new Searching(),
		new ListGames(),
		new GameMain(),
		new SetupAppearance(),
		new StartServer(),
		new Error(),
		new PressStart(),
		new Controls(),
		new ServerSettings(),
		new GameSettings(),
		new PlayerSetup(),
		new Perks(),
		new ClassList(),
		new EditMapList(),
		new CustomizeServer(),
		new DebugLev()
	};

	public int errorOutLev;

	public InfoBox infoBox;

	public float rehost;

	public Menu()
	{
		saving = new Saving();
		infoBox = new InfoBox();
		newString = new StringBuilder("NEW!");
		yString = new StringBuilder("[ Y ] Edit");
	}

	public void DoError(string str, int outLev)
	{
		DoError(str, outLev, 0);
	}

	public void DoError(string str, int outLev, int flags)
	{
		for (int i = 0; i < menuLevel.Length; i++)
		{
			menuLevel[i].active = false;
		}
		menuLevel[12].active = true;
		menuLevel[12].name = new StringBuilder("Error");
		menuLevel[12].error = new StringBuilder[16];
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		while (num2 < str.Length - 1)
		{
			num2++;
			if (str[num2] == ' ' && num2 - num > 80)
			{
				menuLevel[12].error[num3] = new StringBuilder(str.Substring(num, num2 - num));
				num3++;
				num = num2 + 1;
			}
		}
		menuLevel[12].error[num3] = new StringBuilder(str.Substring(num));
		errorOutLev = outLev;
		rehost = 0f;
		if (flags == 1)
		{
			rehost = 3f;
		}
	}

	public bool IsActive()
	{
		for (int i = 0; i < menuLevel.Length; i++)
		{
			if (menuLevel[i].active)
			{
				return true;
			}
		}
		return false;
	}

	public void Close()
	{
		for (int i = 0; i < menuLevel.Length; i++)
		{
			menuLevel[i].active = false;
		}
	}

	public void Update(InterfaceKeys iKeys)
	{
		for (int i = 0; i < menuLevel.Length; i++)
		{
			menuLevel[i].Update(iKeys, this);
		}
		if (saving.Active())
		{
			saving.Update();
		}
		infoBox.Update();
		if (!waitingKeyResult || !keyResult.IsCompleted)
		{
			return;
		}
		try
		{
			string text = Guide.EndShowKeyboardInput(keyResult);
			if (text != null)
			{
				try
				{
					Game1.impact.MeasureString(text);
					if (text.Length > 12)
					{
						text = text.Substring(0, 12);
					}
					Game1.zProfile.EditingSet().SetName(text);
					menuLevel[17].name = new StringBuilder(Game1.zProfile.EditingSet().name);
					menuLevel[19].item[Game1.zProfile.editingClass].text = new StringBuilder(Game1.zProfile.EditingSet().name);
				}
				catch (Exception)
				{
				}
			}
		}
		catch (Exception ex2)
		{
			Console.WriteLine(ex2.StackTrace);
		}
		waitingKeyResult = false;
	}

	public void InitInfoBox()
	{
		try
		{
			infoBox.Init(((Gamer)Gamer.SignedInGamers[(PlayerIndex)Game1.mainPlayerIndex]).Gamertag, Game1.zProfile.careerScore, Leveling.level[Game1.zProfile.level].score, Game1.zProfile.level, Game1.zProfile.time);
		}
		catch
		{
		}
	}

	public void Draw(SpriteBatch sprite)
	{
		for (int i = 0; i < menuLevel.Length; i++)
		{
			menuLevel[i].Draw(sprite, this);
		}
		if (Game1.mainPlayerIndex > -1)
		{
			infoBox.Draw(sprite);
		}
		else
		{
			infoBox.active = false;
		}
		if (saving.Active())
		{
			saving.Draw(sprite);
		}
	}
}
