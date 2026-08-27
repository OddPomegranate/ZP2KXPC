using System.Text;

namespace ZP2K9.ai;

public class BotStyle
{
	public StringBuilder name;

	public int body;

	public int hat;

	public int head;

	public int torso;

	public int legs;

	public int skin;

	public BotStyle(string name, int body, int hat, int head, int torso, int legs, int skin)
	{
		this.name = new StringBuilder(name);
		this.body = body;
		this.hat = hat;
		this.head = head;
		this.torso = torso;
		this.legs = legs;
		this.skin = skin;
	}
}
