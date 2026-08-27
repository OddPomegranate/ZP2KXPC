using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZP2K9.characters;

public class FlashLight
{
	public Vector2 orig;

	public Vector2 flashVec;

	public Vector2 goalVec;

	public bool active;

	public void Update()
	{
		flashVec += (goalVec - flashVec) * Game1.frameTime * 10f;
		if (float.IsNaN(flashVec.X))
		{
			flashVec.X = 0f;
		}
		if (float.IsNaN(flashVec.Y))
		{
			flashVec.Y = 0f;
		}
		int playerOne = Game1.netSession.GetPlayerOne();
		if (playerOne > -1 && Game1.character[playerOne] != null)
		{
			Vector2 vector = goalVec;
			if (Game1.character[playerOne].charKeys.shootVec.Length() > 0.1f)
			{
				vector = Game1.character[playerOne].charKeys.shootVec;
			}
			else if (Game1.character[playerOne].charKeys.runVec.Length() > 0.1f)
			{
				vector = Game1.character[playerOne].charKeys.runVec;
				vector.Y = 0f - vector.Y;
			}
			vector.Normalize();
			goalVec = vector * 30f;
		}
	}

	public void Draw(SpriteBatch sprite)
	{
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		int playerOne = Game1.netSession.GetPlayerOne();
		if (playerOne > -1 && Game1.character[playerOne] != null)
		{
			orig = Scroll.GetLoc(Game1.character[playerOne].drawVec + new Vector2(0f, -50f));
		}
		sprite.Begin(blendState: BlendState.Additive);
		for (int i = 0; i < 20; i++)
		{
			sprite.Draw(Game1.spritesTex, orig + flashVec * i, (Rectangle?)new Rectangle(0, 832, 192, 192), new Color(1f, 0f, 0f, 0.25f), 0f, new Vector2(96f, 96f), 1f + (float)i / 8f, (SpriteEffects)0, 1f);
		}
		sprite.End();
	}
}
