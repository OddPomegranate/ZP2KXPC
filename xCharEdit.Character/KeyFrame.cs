using Yuki_Win.chars.script;

namespace xCharEdit.Character;

public class KeyFrame
{
	public int frameRef;

	public int duration;

	public bool lerp;

	private string[] script;

	private ScriptCommand[] scriptCommand;

	public KeyFrame()
	{
		frameRef = -1;
		duration = 0;
		script = new string[8];
		for (int i = 0; i < script.Length; i++)
		{
			script[i] = null;
		}
		scriptCommand = new ScriptCommand[8];
		for (int j = 0; j < scriptCommand.Length; j++)
		{
			scriptCommand[j] = null;
		}
	}

	public ScriptCommand GetScriptCommand(int idx)
	{
		return scriptCommand[idx];
	}

	public ScriptCommand[] GetScriptCommandArray()
	{
		return scriptCommand;
	}

	public void SetScript(int idx, string val)
	{
		script[idx] = val;
		if (val != null)
		{
			scriptCommand[idx] = ScriptCommandParser.ParseString(val);
		}
	}

	public string GetScript(int idx)
	{
		return script[idx];
	}

	public string[] getScriptArray()
	{
		return script;
	}

	public int GetScriptLength()
	{
		for (int num = script.Length - 1; num >= 0; num--)
		{
			if (script[num] != null && script[num] != "")
			{
				return num + 1;
			}
		}
		return 0;
	}
}
