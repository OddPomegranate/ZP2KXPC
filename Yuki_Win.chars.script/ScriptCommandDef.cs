namespace Yuki_Win.chars.script;

public class ScriptCommandDef
{
	public enum ParamType
	{
		None,
		String,
		Int
	}

	public string text;

	public ParamType paramType;

	public ScriptCommandDef(string text, ParamType paramType)
	{
		this.text = text;
		this.paramType = paramType;
	}
}
