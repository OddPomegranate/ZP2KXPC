using Microsoft.Xna.Framework;

namespace yMapEdit.map;

public class ScrollManager
{
	public static float zoom = 1f;

	public static Vector2 screenSize;

	public static Vector2 scroll;

	public static Vector2 GetRealLoc(Vector2 loc, float layer)
	{
		return (loc - screenSize / 2f) / (zoom * layer) + scroll;
	}

	public static Vector2 GetScreenLoc(Vector2 loc, float layer)
	{
		return (loc - scroll) * zoom * layer + screenSize / 2f;
	}
}
