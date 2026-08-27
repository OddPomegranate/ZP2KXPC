using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZP2K9.net.bandwidth;

public class CharHistory
{
	public float[] read = new float[64];

	public float[] write = new float[64];

	public int curSendID;

	public long[] sendTime = new long[256];

	public void Send()
	{
		curSendID = (curSendID + 1) % sendTime.Length;
	}

	public void AddRead(float d)
	{
		for (int num = read.Length - 2; num >= 0; num--)
		{
			read[num + 1] = read[num];
		}
		read[0] = d;
	}

	public void AddWrite(float d)
	{
		for (int num = write.Length - 2; num >= 0; num--)
		{
			write[num + 1] = write[num];
		}
		write[0] = d;
	}

	public void Draw(SpriteBatch sprite, Vector2 loc)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < read.Length; i++)
		{
			float num = read[i] / 10f;
			sprite.Draw(Game1.nullTex, loc + new Vector2(i, 0f - num), (Rectangle?)new Rectangle(0, 0, 1, 1), Color.Lime, 0f, default(Vector2), new Vector2(1f, num), (SpriteEffects)0, 1f);
			num = write[i] / 10f;
			sprite.Draw(Game1.nullTex, loc + new Vector2(i, num), (Rectangle?)new Rectangle(0, 0, 1, 1), Color.Red, 0f, new Vector2(0f, 1f), new Vector2(1f, num), (SpriteEffects)0, 1f);
		}
	}
}
