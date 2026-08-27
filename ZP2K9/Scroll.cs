using Microsoft.Xna.Framework;

namespace ZP2K9;

public class Scroll
{
	public static Vector2 scroll;

	public static float zoom = 1f;

	public static Vector2 GetLoc(Vector2 loc)
	{
		return (loc - scroll) * zoom + new Vector2(640f, 360f);
	}
}
