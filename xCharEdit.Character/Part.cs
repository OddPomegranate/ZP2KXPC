using Microsoft.Xna.Framework;

namespace xCharEdit.Character;

public class Part
{
	public Vector2 location;

	public float rotation;

	public Vector2 scaling;

	public int idx;

	public int flip;

	public Part()
	{
		idx = -1;
		scaling = new Vector2(1f, 1f);
	}
}
