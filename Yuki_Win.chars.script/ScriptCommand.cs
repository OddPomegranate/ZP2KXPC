namespace Yuki_Win.chars.script;

public class ScriptCommand
{
	public ScriptCommandParser.Command command;

	public int iParam;

	public string sParam;

	public ScriptCommand(ScriptCommandParser.Command command, int iParam, string sParam)
	{
		this.command = command;
		this.iParam = iParam;
		this.sParam = sParam;
	}
}
