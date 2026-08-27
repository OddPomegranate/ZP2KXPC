using Microsoft.Xna.Framework;

namespace yMapEdit.segdef;

public class SegDef
{
	public int texIdx;

	public Rectangle sRect;

	public string name;

	public Vector2 lockLoc;

	public Vector2 origLoc;

	public int flags;

	public int material;

	public void UpdateOrigLoc()
	{
		origLoc = lockLoc + new Vector2(sRect.X, sRect.Y);
	}
}
