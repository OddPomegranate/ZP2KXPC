using System.Collections.ObjectModel;
using System.Text;
using Microsoft.Xna.Framework;
using ZP2K9.net;

namespace ZP2K9.particles;

public class KillManager
{
	public static Kill[] kill = new Kill[4]
	{
		new Kill(),
		new Kill(),
		new Kill(),
		new Kill()
	};

	public static void DoKill(int killer, int killee, int type)
	{
		DoKill(killer, killee, type, netInduced: false);
	}

	public static bool WriteKills(PacketWriter writer)
	{
		bool result = false;
		for (int i = 0; i < kill.Length; i++)
		{
			if (kill[i].killer > -1 && kill[i].killee > -1)
			{
				result = true;
				NetPacker.WriteMsg(writer, 6);
				NetPacker.WriteByte(writer, kill[i].killer);
				NetPacker.WriteByte(writer, kill[i].killee);
				NetPacker.WriteByte(writer, kill[i].type);
			}
		}
		return result;
	}

	public static void CleanKills()
	{
		for (int i = 0; i < kill.Length; i++)
		{
			if (kill[i].killer > -1 && kill[i].killee > -1)
			{
				kill[i].killer = -1;
				kill[i].killee = -1;
			}
		}
	}

	public static void ReadKill(PacketReader reader)
	{
		int killer = NetPacker.ReadByte(reader);
		int killee = NetPacker.ReadByte(reader);
		int type = NetPacker.ReadByte(reader);
		DoKill(killer, killee, type, netInduced: true);
	}

	public static StringBuilder GetPlayerName(int i)
	{
		if (Game1.character[i] == null)
		{
			return new StringBuilder("Player " + i);
		}
		if (Game1.netSession.netType == 1 || Game1.netSession.netType == 0)
		{
			if (Game1.character[i].ai == null)
			{
				return new StringBuilder("Player " + i);
			}
			return Game1.botBag.Style(i).name;
		}
		if (Game1.netSession.netSession != null && (Game1.netSession.netSession.AllGamers).Count > 0)
		{
			for (int j = 0; j < (Game1.netSession.netSession.AllGamers).Count; j++)
			{
				INetworkGamer val = (Game1.netSession.netSession.AllGamers)[j];
				if (Game1.netSession.playerList.ContainsKey(val.Id) && Game1.netSession.playerList[val.Id] == i)
				{
					return new StringBuilder(val.Gamertag);
				}
			}
			return Game1.botBag.Style(i).name;
		}
		return new StringBuilder("Player " + i);
	}

	public static void DoDedz(int killee)
	{
		if (Game1.character[killee].gibbed)
		{
			Game1.pMan.AddParticle(51, Game1.character[killee].loc + new Vector2(0f, -40f), default(Vector2), 0f, 0, -1);
		}
	}

	public static void DoKill(int killer, int killee, int type, bool netInduced)
	{
		if (Game1.character[killer] == null || Game1.character[killee] == null)
		{
			return;
		}
		if (Game1.netSession.GetNetworkOwner(killee))
		{
			DoDedz(killee);
		}
		else if (netInduced)
		{
			DoDedz(killee);
		}
		if (Game1.netSession.netType == 1 || Game1.netSession.netType == 0)
		{
			StringBuilder stringBuilder = ((Game1.character[killer].ai != null) ? Game1.botBag.Style(killer).name : new StringBuilder("Player " + killer));
			StringBuilder stringBuilder2 = ((Game1.character[killee].ai != null) ? Game1.botBag.Style(killee).name : new StringBuilder("Player " + killee));
			if (killer == killee)
			{
				stringBuilder = new StringBuilder(" ");
			}
			Game1.hud.AddMessage(stringBuilder, stringBuilder2, Game1.character[killer].GetTeam(), Game1.character[killee].GetTeam(), type);
			if (killer != killee)
			{
				if (Game1.netSession.GetPlayerOne() == killer)
				{
					Game1.zProfile.AddKill();
					Game1.hud.SetDead("Killed " + stringBuilder2.ToString());
				}
				if (Game1.netSession.GetPlayerOne() == killee)
				{
					Game1.hud.SetDead("Killed by " + stringBuilder.ToString());
				}
				Game1.character[killer].AddKill();
			}
			else if (Game1.netSession.GetPlayerOne() == killee)
			{
				Game1.hud.SetDead("You killed yourself");
			}
			Game1.character[killee].deaths++;
			if (Game1.netSession.GetPlayerOne() == killee)
			{
				Game1.zProfile.deaths++;
			}
			Game1.character[killee].killStreak = 0;
			Game1.character[killee].multikill = 0;
		}
		else
		{
			if (Game1.netSession.netSession == null)
			{
				return;
			}
			if (Game1.netSession.IsHost())
			{
				int gameType = GameState.gameType;
				if ((gameType == 1 || gameType == 4) && killer != killee)
				{
					if (Game1.character[killer].team == 0)
					{
						Game1.netSession.blueScore += 10;
					}
					else
					{
						Game1.netSession.redScore += 10;
					}
				}
			}
			if ((Game1.netSession.netSession.AllGamers).Count <= 0)
			{
				return;
			}
			StringBuilder stringBuilder3 = null;
			StringBuilder stringBuilder4 = null;
			for (int i = 0; i < (Game1.netSession.netSession.AllGamers).Count; i++)
			{
				INetworkGamer val = (Game1.netSession.netSession.AllGamers)[i];
				if (Game1.netSession.playerList.ContainsKey(val.Id))
				{
					if (Game1.netSession.playerList[val.Id] == killer)
					{
						stringBuilder3 = new StringBuilder(val.Gamertag);
						stringBuilder3 = Game1.character[killer].GetClanName(stringBuilder3);
					}
					if (Game1.netSession.playerList[val.Id] == killee)
					{
						stringBuilder4 = new StringBuilder(val.Gamertag);
						stringBuilder4 = Game1.character[killee].GetClanName(stringBuilder4);
					}
				}
			}
			if (stringBuilder3 == null)
			{
				stringBuilder3 = Game1.botBag.Style(killer).name;
			}
			if (stringBuilder4 == null)
			{
				stringBuilder4 = Game1.botBag.Style(killee).name;
			}
			bool flag = false;
			if (Game1.netSession.GetNetworkOwner(killee))
			{
				for (int j = 0; j < kill.Length; j++)
				{
					if (kill[j].killer < 0 && kill[j].killee < 0)
					{
						kill[j].killer = killer;
						kill[j].killee = killee;
						kill[j].type = type;
						break;
					}
				}
				flag = true;
			}
			else if (netInduced)
			{
				flag = true;
			}
			if (flag)
			{
				if (Game1.character[killee].killRefreshFrame > 0f)
				{
					return;
				}
				Game1.character[killee].killRefreshFrame = 3f;
			}
			if (!flag)
			{
				return;
			}
			if (killer == killee)
			{
				stringBuilder3 = new StringBuilder(" ");
			}
			Game1.hud.AddMessage(stringBuilder3, stringBuilder4, Game1.character[killer].GetTeam(), Game1.character[killee].GetTeam(), type);
			if (Game1.netSession.GetNetworkOwner(killee))
			{
				Game1.character[killee].deaths++;
				if (Game1.netSession.GetPlayerOne() == killee)
				{
					Game1.zProfile.deaths++;
				}
				Game1.character[killee].killStreak = 0;
				Game1.character[killee].multikill = 0;
			}
			if (Game1.netSession.GetNetworkOwner(killer))
			{
				if (killer != killee)
				{
					if (Game1.netSession.GetPlayerOne() == killer)
					{
						Game1.zProfile.AddKill();
						Game1.hud.AddPopup("Killed " + stringBuilder4.ToString(), 10, 1f);
					}
					Game1.character[killer].AddKill();
				}
				else if (Game1.netSession.GetPlayerOne() == killee)
				{
					Game1.hud.SetDead("You killed yourself");
				}
			}
			if (killer != killee && Game1.netSession.GetPlayerOne() == killee)
			{
				Game1.hud.SetDead("Killed by " + stringBuilder3.ToString());
			}
		}
	}
}
