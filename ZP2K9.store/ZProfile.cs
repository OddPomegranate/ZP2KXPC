using System;
using System.IO;
using System.Text;
using ZP2K9.store.leveling;

namespace ZP2K9.store;

public class ZProfile
{
	public const int TEAM_DEFAULT = 0;

	public const int TEAM_HUMANS = 1;

	public const int TEAM_ZOMBIES = 2;

	public const int BODY_ROBOT = 0;

	public const int BODY_BOY = 1;

	public const int BODY_GIRL = 2;

	public const int PERK_OFFENSE_DIESEL_POWER = 0;

	public const int PERK_OFFENSE_LEADSTORM = 1;

	public const int PERK_OFFENSE_KUNG_FU_HERO = 2;

	public const int PERK_OFFENSE_SAMURAI = 3;

	public const int PERK_OFFENSE_DEADLY_AIM = 4;

	public const int PERK_OFFENSE_CHEMIST = 5;

	public const int PERK_OFFENSE_ROBOT_HANDS = 6;

	public const int PERK_OFFENSE_SCAVENGER = 7;

	public const int PERK_OFFENSE_LEECH = 8;

	public const int PERK_OFFENSE_SHIFTER = 9;

	public const int PERK_MOD_QUICK = 0;

	public const int PERK_MOD_NINJA = 1;

	public const int PERK_MOD_MR_RADAR = 2;

	public const int PERK_MOD_AMMO_JUNKIE = 3;

	public const int PERK_MOD_GUNSLINGER = 4;

	public const int PERK_MOD_ROCKET_PANTS = 5;

	public const int PERK_MOD_MAD_BOMBER = 6;

	public const int PERK_MOD_MORTAR = 7;

	public const int PERK_MOD_GRAB_BAG = 8;

	public const int PERK_MOD_GRIEFER = 9;

	public const int PERK_DEFENSE_HAZMAT_SUIT = 0;

	public const int PERK_DEFENSE_BLAST_ARMOR = 1;

	public const int PERK_DEFENSE_CLUMSY = 2;

	public const int PERK_DEFENSE_BULLETPROOF = 3;

	public const int PERK_DEFENSE_MEDIC = 4;

	public const int PERK_DEFENSE_TANK = 5;

	public const int PERK_DEFENSE_STEALTH = 6;

	public const int PERK_DEFENSE_CLIPS = 7;

	public const int PERK_DEFENSE_PREPARED = 8;

	public const int PERK_DEFENSE_TURBOCHARGE = 9;

	public const int PERK_OFFENSE = 0;

	public const int PERK_MOD = 1;

	public const int PERK_DEFENSE = 2;

	public int kills;

	public int deaths;

	public int fragKills;

	public int flameKills;

	public int bulletKills;

	public int poisonKills;

	public int frozenKills;

	public int swordKills;

	public int squashKills;

	public int zapKills;

	public int kickKills;

	public int shotKills;

	public int highestKillStreak;

	public int mostKillsInMatch;

	public int gamesPlayed;

	public int gamesWon;

	public int defaultTeam;

	private CharacterSet[] charClass;

	public int curClass;

	public int defaultClass;

	public long careerScore;

	public int level;

	public int editingClass;

	public Unlocks unlocks;

	public long time;

	public float second;

	public StringBuilder clanTag;

	public ZProfile()
	{
		charClass = new CharacterSet[8];
		for (int i = 0; i < charClass.Length; i++)
		{
			charClass[i] = new CharacterSet();
			charClass[i].SetDefaultLoadout(i);
		}
		unlocks = new Unlocks();
	}

	// ---- TEMP DEBUG HOOK (added 2026-08-23) ----
	// Flip to false (or delete this block + its 2 call sites in Game1.cs Initialize()
	// and the end of Read() below) to go back to normal earned progression.
	public const bool DEBUG_MAX_LEVEL = true;

	public void ApplyDebugMaxLevel()
	{
		if (!DEBUG_MAX_LEVEL)
		{
			return;
		}
		level = 100;
		careerScore = 999999999L;
		unlocks.UpdateUnlocks();
	}

	public void AddCareerScore(long score)
	{
		careerScore += score;
		if (Leveling.IsHappyHour(DateTime.Now.TimeOfDay.Hours))
		{
			careerScore += score;
		}
		if (level < 99 && careerScore >= Leveling.level[level].score)
		{
			string msg = "You Are Level " + (level + 2) + "!";
			Game1.hud.AddPopup(msg, Leveling.level[level].type, Leveling.level[level].idx, level + 1, 2f);
			level++;
			unlocks.UpdateUnlocks();
			Game1.store.Write(0);
		}
	}

	public void AddKill()
	{
		kills++;
	}

	public CharacterClass Class()
	{
		return charClass[curClass].CharClass();
	}

	public CharacterSet ClassSet()
	{
		return charClass[curClass];
	}

	public CharacterSet ClassSet(int i)
	{
		return charClass[i];
	}

	public CharacterClass DefaultClass()
	{
		return charClass[defaultClass].CharClass();
	}

	public CharacterClass EditingClass()
	{
		return charClass[editingClass].CharClass();
	}

	public CharacterSet EditingSet()
	{
		return charClass[editingClass];
	}

	public CharacterClass Class(int i)
	{
		return charClass[i].CharClass();
	}

	public void Read(BinaryReader reader)
	{
		kills = reader.ReadInt32();
		deaths = reader.ReadInt32();
		fragKills = reader.ReadInt32();
		flameKills = reader.ReadInt32();
		bulletKills = reader.ReadInt32();
		poisonKills = reader.ReadInt32();
		frozenKills = reader.ReadInt32();
		swordKills = reader.ReadInt32();
		squashKills = reader.ReadInt32();
		zapKills = reader.ReadInt32();
		kickKills = reader.ReadInt32();
		shotKills = reader.ReadInt32();
		defaultClass = reader.ReadInt32();
		defaultTeam = reader.ReadInt32();
		careerScore = reader.ReadInt64();
		level = reader.ReadInt32();
		highestKillStreak = reader.ReadInt32();
		mostKillsInMatch = reader.ReadInt32();
		gamesPlayed = reader.ReadInt32();
		gamesWon = reader.ReadInt32();
		time = reader.ReadInt64();
		if (reader.ReadBoolean())
		{
			clanTag = new StringBuilder(reader.ReadString());
		}
		else
		{
			clanTag = null;
		}
		for (int i = 0; i < charClass.Length; i++)
		{
			charClass[i].Read(reader);
		}
		unlocks.Read(reader);
	
		ApplyDebugMaxLevel();
	}

	public void Write(BinaryWriter writer)
	{
		writer.Write(kills);
		writer.Write(deaths);
		writer.Write(fragKills);
		writer.Write(flameKills);
		writer.Write(bulletKills);
		writer.Write(poisonKills);
		writer.Write(frozenKills);
		writer.Write(swordKills);
		writer.Write(squashKills);
		writer.Write(zapKills);
		writer.Write(kickKills);
		writer.Write(shotKills);
		writer.Write(defaultClass);
		writer.Write(defaultTeam);
		writer.Write(careerScore);
		writer.Write(level);
		writer.Write(highestKillStreak);
		writer.Write(mostKillsInMatch);
		writer.Write(gamesPlayed);
		writer.Write(gamesWon);
		writer.Write(time);
		if (clanTag == null)
		{
			writer.Write(value: false);
		}
		else
		{
			writer.Write(value: true);
			writer.Write(clanTag.ToString());
		}
		for (int i = 0; i < charClass.Length; i++)
		{
			charClass[i].Write(writer);
		}
		unlocks.Write(writer);
	}

	internal void UpdateClass()
	{
		curClass = defaultClass;
	}
}
