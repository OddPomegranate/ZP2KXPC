using System.Collections.ObjectModel;
using System.Text;
using Microsoft.Xna.Framework;
using ZP2K9.characters;
using ZP2K9.hud;

namespace ZP2K9.menu.levels;

public class Lobby : MenuLevel
{
	private const int ITEM_BACK = 0;

	private const int ITEM_START = 1;

	private const string t = "(Open Slot)";

	public Lobby()
	{
		Init(host: false);
	}

	public Lobby(bool host)
	{
		Init(host);
	}

	private void Init(bool host)
	{
		name = new StringBuilder("Connecting...");
		if (host)
		{
			item = new MenuItem[1]
			{
				new MenuItem("Back", 0)
			};
		}
		else
		{
			item = new MenuItem[1]
			{
				new MenuItem("Back", 0)
			};
		}
		width = 300;
		height = 300;
	}

	public override void Update(InterfaceKeys iKeys, Menu menu)
	{
		if (active)
		{
			if (Game1.netSession.netSession != null)
			{
				if (Game1.netSession.netSession.IsHost && Game1.character[0] == null)
				{
					Game1.character[0] = new Character(0, 0, default(Vector2));
					for (int i = 0; i < Game1.character[0].perk.Length; i++)
					{
						Game1.character[0].perk[i] = Game1.zProfile.ClassSet().perk[i];
					}
					Game1.character[0].SetNewClass();
					Game1.character[0].Reset();
				}
				if ((Game1.netSession.netSession.AllGamers).Count > 0)
				{
					active = false;
					int num = 100;
					if (Game1.netSession.netSession.SessionProperties[0].HasValue)
					{
						num = Game1.netSession.netSession.SessionProperties[0].Value;
					}
					if (num != 206)
					{
						menu.DoError("Server has different version.", 0);
						Game1.netSession.newVersAvailable = true;
						Game1.netSession.netSession.Dispose();
						while (!Game1.netSession.netSession.IsDisposed)
						{
						}
					}
					else
					{
						GameState.mode = 1;
						Game1.gameMap.GetSpawn(0, Game1.character[0]);
						for (int j = 0; j < Game1.netSession.BotCount(); j++)
						{
							Game1.character[j + 20] = new Character(j + 20, -1, default(Vector2));
							Game1.character[j + 20].team = j % 2;
							Game1.gameMap.GetSpawn(0, Game1.character[j + 20]);
						}
						Game1.netSession.netPlay.needsInit = true;
						Game1.netSession.ResetGameStats();
					}
				}
			}
			if (Game1.netSession.joinFailed || Game1.netSession.findFailed || Game1.netSession.createFailed || Game1.netSession.joinInviteFailed)
			{
				Game1.netSession.joinFailed = false;
				Game1.netSession.joinInviteFailed = false;
				Game1.netSession.findFailed = false;
				Game1.netSession.createFailed = false;
				active = false;
				if (Game1.netSession.failMessage == null)
				{
					Game1.netSession.failMessage = "Unknown Error.";
				}
				else
				{
					Game1.netSession.failMessage = "Error: " + Game1.netSession.failMessage;
				}
				menu.DoError(Game1.netSession.failMessage, (Game1.netSession.netType == 2) ? 5 : 6);
			}
		}
		base.Update(iKeys, menu);
	}

	public override void SelectItem(Menu menu)
	{
		switch (selected)
		{
		case 0:
			active = false;
			Game1.netSession.Kill();
			menu.menuLevel[(Game1.netSession.netType == 2) ? 5 : 6].active = true;
			break;
		case 1:
		{
			active = false;
			GameState.mode = 1;
			Game1.gameMap.GetSpawn(0, Game1.character[0]);
			for (int i = 0; i < Game1.netSession.BotCount(); i++)
			{
				Game1.character[i + 20] = new Character(i + 20, -1, default(Vector2));
				Game1.character[i + 20].team = i % 2;
				Game1.gameMap.GetSpawn(0, Game1.character[i + 20]);
			}
			Game1.netSession.netPlay.needsInit = true;
			Game1.netSession.ResetGameStats();
			break;
		}
		}
	}

	public override void Cancel(Menu menu)
	{
		Game1.netSession.Kill();
		active = false;
		menu.menuLevel[(Game1.netSession.netType == 2) ? 5 : 6].active = true;
	}
}
