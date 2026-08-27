using System;

namespace Yuki_Win.chars.script;

public class ScriptCommandParser
{
	public enum Command
	{
		JoyJump = 0,
		SetAnim = 1,
		GotoFrame = 2,
		SetJump = 3,
		Slide = 4,
		Backup = 6,
		SetAnyGoto = 7,
		SetAtkGoto = 8,
		SetUpperGoto = 9,
		SetLowerGoto = 10,
		SetSawAnyGoto = 11,
		SetSawUpperGoto = 12,
		SetSawLowerGoto = 13,
		SetSecAnyGoto = 14,
		SetSecUpperGoto = 15,
		SetSecLowerGoto = 16,
		ClearGotos = 17,
		Float = 18,
		ShowGrabber = 19,
		SetGrabber = 20,
		KillMe = 21,
		Ethereal = 22,
		Solid = 23,
		SetStrongAnyGoto = 24,
		SetStrongUpperGoto = 25,
		SetStrongLowerGoto = 26,
		CheckSec = 27,
		OverSec = 28,
		SetSawSpecial = 29,
		SetGunSpecial = 30,
		Unfloat = 31,
		WallJump = 32,
		SetLandAnim = 33,
		SetFall = 34,
		DeathCheck = 35,
		IfUpGoto = 36,
		IfDownGoto = 37,
		SpeedFac = 38
	}

	public static ScriptCommandDef[] command;

	public static void Init()
	{
		command = new ScriptCommandDef[64];
		command[0] = new ScriptCommandDef("joyjump", ScriptCommandDef.ParamType.None);
		command[1] = new ScriptCommandDef("setanim", ScriptCommandDef.ParamType.String);
		command[2] = new ScriptCommandDef("gotoframe", ScriptCommandDef.ParamType.Int);
		command[3] = new ScriptCommandDef("setjump", ScriptCommandDef.ParamType.Int);
		command[4] = new ScriptCommandDef("slide", ScriptCommandDef.ParamType.Int);
		command[6] = new ScriptCommandDef("backup", ScriptCommandDef.ParamType.Int);
		command[8] = new ScriptCommandDef("setatkgoto", ScriptCommandDef.ParamType.Int);
		command[7] = new ScriptCommandDef("setanygoto", ScriptCommandDef.ParamType.Int);
		command[10] = new ScriptCommandDef("setlowergoto", ScriptCommandDef.ParamType.Int);
		command[9] = new ScriptCommandDef("setuppergoto", ScriptCommandDef.ParamType.Int);
		command[11] = new ScriptCommandDef("setsawanygoto", ScriptCommandDef.ParamType.Int);
		command[12] = new ScriptCommandDef("setsawuppergoto", ScriptCommandDef.ParamType.Int);
		command[13] = new ScriptCommandDef("setsawlowergoto", ScriptCommandDef.ParamType.Int);
		command[14] = new ScriptCommandDef("setsecanygoto", ScriptCommandDef.ParamType.Int);
		command[15] = new ScriptCommandDef("setsecuppergoto", ScriptCommandDef.ParamType.Int);
		command[16] = new ScriptCommandDef("setseclowergoto", ScriptCommandDef.ParamType.Int);
		command[17] = new ScriptCommandDef("cleargotos", ScriptCommandDef.ParamType.None);
		command[18] = new ScriptCommandDef("float", ScriptCommandDef.ParamType.None);
		command[31] = new ScriptCommandDef("unfloat", ScriptCommandDef.ParamType.None);
		command[19] = new ScriptCommandDef("showgrabber", ScriptCommandDef.ParamType.None);
		command[20] = new ScriptCommandDef("setgrabber", ScriptCommandDef.ParamType.String);
		command[21] = new ScriptCommandDef("killme", ScriptCommandDef.ParamType.None);
		command[22] = new ScriptCommandDef("ethereal", ScriptCommandDef.ParamType.None);
		command[23] = new ScriptCommandDef("solid", ScriptCommandDef.ParamType.None);
		command[24] = new ScriptCommandDef("setstronganygoto", ScriptCommandDef.ParamType.Int);
		command[25] = new ScriptCommandDef("setstronguppergoto", ScriptCommandDef.ParamType.Int);
		command[26] = new ScriptCommandDef("setstronglowergoto", ScriptCommandDef.ParamType.Int);
		command[27] = new ScriptCommandDef("checksec", ScriptCommandDef.ParamType.None);
		command[28] = new ScriptCommandDef("oversec", ScriptCommandDef.ParamType.None);
		command[29] = new ScriptCommandDef("setsawspecial", ScriptCommandDef.ParamType.None);
		command[30] = new ScriptCommandDef("setgunspecial", ScriptCommandDef.ParamType.None);
		command[32] = new ScriptCommandDef("walljump", ScriptCommandDef.ParamType.None);
		command[33] = new ScriptCommandDef("setlandanim", ScriptCommandDef.ParamType.String);
		command[34] = new ScriptCommandDef("setfall", ScriptCommandDef.ParamType.Int);
		command[35] = new ScriptCommandDef("deathcheck", ScriptCommandDef.ParamType.None);
		command[36] = new ScriptCommandDef("ifupgoto", ScriptCommandDef.ParamType.Int);
		command[37] = new ScriptCommandDef("ifdowngoto", ScriptCommandDef.ParamType.Int);
		command[38] = new ScriptCommandDef("speedfac", ScriptCommandDef.ParamType.Int);
	}

	public static ScriptCommand ParseString(string line)
	{
		for (int i = 0; i < command.Length; i++)
		{
			if (command[i] != null && line.Length >= command[i].text.Length && line.Substring(0, command[i].text.Length) == command[i].text)
			{
				return command[i].paramType switch
				{
					ScriptCommandDef.ParamType.String => new ScriptCommand((Command)i, -1, line.Substring(command[i].text.Length + 1)), 
					ScriptCommandDef.ParamType.Int => new ScriptCommand((Command)i, Convert.ToInt32(line.Substring(command[i].text.Length + 1)), ""), 
					_ => new ScriptCommand((Command)i, -1, ""), 
				};
			}
		}
		Console.WriteLine("Unrecognized command: " + line);
		return null;
	}
}
