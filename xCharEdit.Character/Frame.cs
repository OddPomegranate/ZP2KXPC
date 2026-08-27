using Microsoft.Xna.Framework;

namespace xCharEdit.Character;

public class Frame
{
	private Part[] part;

	public string name;

	public Frame()
	{
		part = new Part[16];
		for (int i = 0; i < part.Length; i++)
		{
			part[i] = new Part();
		}
		name = "";
	}

	public Part GetPart(int idx)
	{
		return part[idx];
	}

	public void SetPart(int idx, Part _part)
	{
		part[idx] = _part;
	}

	public Part[] GetPartArray()
	{
		return part;
	}

	public static bool CanLerp(Frame orig, Frame next, int part)
	{
		if (orig.GetPart(part).idx < 0)
		{
			return false;
		}
		if (next.GetPart(part).idx < 0)
		{
			return false;
		}
		if (orig.GetPart(part).idx != next.GetPart(part).idx)
		{
			return false;
		}
		if (orig.GetPart(part).flip != next.GetPart(part).flip)
		{
			return false;
		}
		return true;
	}

	public static float LerpRotation(float orig, float next, float progress)
	{
		float num;
		for (num = next - orig; num > 3.14f; num -= 6.28f)
		{
		}
		for (; num < -3.14f; num += 6.28f)
		{
		}
		return orig + num * progress;
	}

	public static Vector2 LerpScale(Vector2 orig, Vector2 next, float progress)
	{
		return orig + (next - orig) * progress;
	}

	public static Vector2 LerpLoc(Vector2 orig, Vector2 next, float progress)
	{
		return orig + (next - orig) * progress;
	}
}
