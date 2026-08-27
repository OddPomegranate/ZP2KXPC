using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using ZP2K9.characters;
using ZP2K9.hud;

namespace ZP2K9.menu.levels;

public class ListGames : MenuLevel
{
	private float refreshFrame;

	private int refreshCount;

	public ListGames()
	{
		name = new StringBuilder("Available Games");
		if (Game1.netSession.sessions != null)
		{
			item = new MenuItem[(Game1.netSession.sessions).Count + 2];
			item[0] = new MenuItem("Host                               Gamers   Type             Vers     Ping", 0, noSelect: true);
			for (int i = 0; i < item.Length - 2; i++)
			{
				item[i + 1] = new MenuItem((Game1.netSession.sessions)[i].HostGamertag, i + 1, (Game1.netSession.sessions)[i].CurrentGamerCount, (Game1.netSession.sessions)[i].CurrentGamerCount + (Game1.netSession.sessions)[i].OpenPublicGamerSlots, -1, (Game1.netSession.sessions)[i].SessionProperties[0], (Game1.netSession.sessions)[i].SessionProperties[1]);
			}
			item[item.Length - 1] = new MenuItem("Cancel", item.Length - 1);
		}
		else
		{
			item = new MenuItem[1]
			{
				new MenuItem("Cancel", 0)
			};
		}
		refreshCount = 0;
		selected = 1;
		width = 550;
		height = 300;
	}

	public override void Update(InterfaceKeys iKeys, Menu menu)
	{
		if (active && alpha >= 1f)
		{
			try
			{
				refreshFrame += Game1.frameTime;
				if (refreshFrame > 1f)
				{
					for (int i = 0; i < item.Length - 1; i++)
					{
						if ((Game1.netSession.sessions).Count > i && (Game1.netSession.sessions)[i] != null && (Game1.netSession.sessions)[i].QualityOfService.IsAvailable)
						{
							item[i + 1].UpdatePing((Game1.netSession.sessions)[i].QualityOfService.AverageRoundtripTime.Milliseconds);
						}
					}
					refreshFrame = 0f;
					refreshCount++;
					if (refreshCount > 2)
					{
						refreshFrame -= refreshCount;
					}
				}
			}
			catch
			{
				refreshFrame = -20f;
			}
		}
		base.Update(iKeys, menu);
	}

	public override void SelectItem(Menu menu)
	{
		if (selected < item.Length - 1)
		{
			int num = ((!(Game1.netSession.sessions)[selected - 1].SessionProperties[0].HasValue) ? 100 : (Game1.netSession.sessions)[selected - 1].SessionProperties[0].Value);
			if (206 == num)
			{
				Game1.netSession.netPlay.needsInit = true;
				Game1.netSession.netPlay.ID = -1;
				Game1.hud.scoreBoard.Reset();
				Game1.character = new Character[32];
				Game1.netSession.playerList = new Dictionary<byte, int>();
				Game1.netSession.JoinSession((Game1.netSession.sessions)[selected - 1]);
				menu.menuLevel[4] = new Lobby(host: false);
				menu.menuLevel[4].active = true;
				active = false;
			}
			else
			{
				Game1.menu.DoError("Server has different version.", (Game1.netSession.netType == 2) ? 5 : 6);
			}
		}
		else
		{
			active = false;
			menu.menuLevel[(Game1.netSession.netType == 2) ? 5 : 6].active = true;
		}
	}

	public override void Cancel(Menu menu)
	{
		active = false;
		menu.menuLevel[(Game1.netSession.netType == 2) ? 5 : 6].active = true;
	}
}
