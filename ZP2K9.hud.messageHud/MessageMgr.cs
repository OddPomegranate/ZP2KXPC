using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace ZP2K9.hud.messageHud;

public class MessageMgr
{
	private const int MAX_MESSAGE = 5;

	private Message[] message = new Message[64];

	private int min;

	private int max;

	private float frame;

	public MessageMgr()
	{
		for (int i = 0; i < message.Length; i++)
		{
			message[i] = new Message();
		}
	}

	public void AddMessage(StringBuilder txt1, StringBuilder txt2, int team1, int team2, int kill)
	{
		message[max].txt1 = txt1;
		message[max].txt2 = txt2;
		message[max].team1 = team1;
		message[max].team2 = team2;
		message[max].kill = kill;
		max = (max + 1) % message.Length;
		frame = 2f;
	}

	public void Update()
	{
		if (!(frame > 0f))
		{
			return;
		}
		frame -= Game1.frameTime;
		if (frame <= 0f)
		{
			min = (min + 1) % message.Length;
			if (min != max)
			{
				frame = 2f;
			}
		}
	}

	public void Draw(SpriteBatch sprite)
	{
		int num = min;
		float num2 = 0f;
		if (frame < 0.25f)
		{
			num2 = (0.25f - frame) * 4f;
		}
		int num3 = 0;
		while (num != max)
		{
			float a = 1f;
			if (num == min && frame < 0.25f)
			{
				a = frame * 4f;
			}
			message[num].Draw(sprite, (float)num3 * 24f - num2 * 24f, a);
			num = (num + 1) % message.Length;
			num3++;
		}
		if (num3 > 5 && frame > 0.25f)
		{
			frame = 0.25f;
		}
	}
}
