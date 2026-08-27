using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ZP2K9;

internal class Quake
{
	public static float quakeVal;

	public static void UpdateQuake()
	{
		if (quakeVal > 0f)
		{
			quakeVal -= Game1.frameTime;
			if (quakeVal < 0f)
			{
				quakeVal = 0f;
			}
		}
		if (Game1.mainPlayerIndex <= -1)
		{
			return;
		}
		try
		{
			if (Game1.settings.vibration)
			{
				GamePad.SetVibration((PlayerIndex)Game1.mainPlayerIndex, quakeVal, quakeVal);
			}
			else
			{
				GamePad.SetVibration((PlayerIndex)Game1.mainPlayerIndex, 0f, 0f);
			}
		}
		catch
		{
		}
	}

	public static void SetQuake(float v)
	{
		if (v > quakeVal)
		{
			quakeVal = v;
		}
	}

	public static void UpdateScroll()
	{
		float num = 25f;
		Scroll.scroll += Rand.GetRandomVec2((0f - quakeVal) * num, quakeVal * num, (0f - quakeVal) * num, quakeVal * num);
	}
}
