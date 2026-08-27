using System.Text;

namespace ZP2K9.net;

public class MapDescription
{
	public string path;

	public StringBuilder name;

	public StringBuilder description;

	public bool included;

	public MapDescription(string path, string name, string description)
	{
		this.path = path;
		this.name = new StringBuilder(name);
		this.description = new StringBuilder(description);
		included = true;
	}
}
