using System;
using Microsoft.Xna.Framework;

namespace Yuki_Win;

public class Trig
{
	public static float GetDist(Vector2 v1, Vector2 v2)
	{
		return (v2 - v1).Length();
	}

	public static float GetAngle(Vector2 v1, Vector2 v2)
	{
		float num = (float)Math.PI;
		Vector2 vector = new Vector2(v2.X - v1.X, v2.Y - v1.Y);
		if (vector.X == 0f)
		{
			if (vector.Y < 0f)
			{
				return num * 0.5f;
			}
			if (vector.Y > 0f)
			{
				return num * 1.5f;
			}
		}
		if (vector.Y == 0f)
		{
			if (vector.X < 0f)
			{
				return 0f;
			}
			if (vector.X > 0f)
			{
				return num;
			}
		}
		float num2 = (float)Math.Atan(Math.Abs(vector.Y) / Math.Abs(vector.X));
		if (vector.X < 0f || vector.Y > 0f)
		{
			num2 = num - num2;
		}
		if (vector.X < 0f || vector.Y < 0f)
		{
			num2 = num + num2;
		}
		if (vector.X > 0f || vector.Y < 0f)
		{
			num2 = num * 2f - num2;
		}
		if (num2 < 0f)
		{
			num2 += num * 2f;
		}
		return num2;
	}
}
