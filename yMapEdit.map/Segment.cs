using System;
using Microsoft.Xna.Framework;
using yMapEdit.segdef;

namespace yMapEdit.map;

public class Segment
{
	public Vector2 loc;

	public float rotation;

	public int idx;

	public Rectangle rect;

	public Segment()
	{
		idx = -1;
	}

	public void CalculateRect(SegDef segDef)
	{
		Vector2 vector = loc;
		Vector2 vector2 = new Vector2((float)Math.Cos(rotation) * ((float)segDef.sRect.Right - segDef.origLoc.X), (float)Math.Sin(rotation) * ((float)segDef.sRect.Right - segDef.origLoc.X));
		Vector2 vector3 = new Vector2((float)Math.Cos(rotation + 1.57f) * ((float)segDef.sRect.Bottom - segDef.origLoc.Y), (float)Math.Sin(rotation + 1.57f) * ((float)segDef.sRect.Bottom - segDef.origLoc.Y));
		Vector2 vector4 = new Vector2((float)Math.Cos(rotation) * (segDef.origLoc.X - (float)segDef.sRect.X), (float)Math.Sin(rotation) * (segDef.origLoc.X - (float)segDef.sRect.X));
		Vector2 vector5 = new Vector2((float)Math.Cos(rotation + 1.57f) * (segDef.origLoc.Y - (float)segDef.sRect.Y), (float)Math.Sin(rotation + 1.57f) * (segDef.origLoc.Y - (float)segDef.sRect.Y));
		Vector2 vector6 = vector - vector4 - vector5;
		Vector2 vector7 = vector + vector2 - vector5;
		Vector2 vector8 = vector - vector4 + vector3;
		Vector2 vector9 = vector + vector2 + vector3;
		Vector2 vector10 = vector;
		Vector2 vector11 = vector;
		if (vector7.X < vector10.X)
		{
			vector10.X = vector7.X;
		}
		if (vector7.Y < vector10.Y)
		{
			vector10.Y = vector7.Y;
		}
		if (vector8.X < vector10.X)
		{
			vector10.X = vector8.X;
		}
		if (vector8.Y < vector10.Y)
		{
			vector10.Y = vector8.Y;
		}
		if (vector9.X < vector10.X)
		{
			vector10.X = vector9.X;
		}
		if (vector9.Y < vector10.Y)
		{
			vector10.Y = vector9.Y;
		}
		if (vector6.X < vector10.X)
		{
			vector10.X = vector6.X;
		}
		if (vector6.Y < vector10.Y)
		{
			vector10.Y = vector6.Y;
		}
		if (vector7.X > vector11.X)
		{
			vector11.X = vector7.X;
		}
		if (vector7.Y > vector11.Y)
		{
			vector11.Y = vector7.Y;
		}
		if (vector8.X > vector11.X)
		{
			vector11.X = vector8.X;
		}
		if (vector8.Y > vector11.Y)
		{
			vector11.Y = vector8.Y;
		}
		if (vector9.X > vector11.X)
		{
			vector11.X = vector9.X;
		}
		if (vector9.Y > vector11.Y)
		{
			vector11.Y = vector9.Y;
		}
		if (vector6.X > vector11.X)
		{
			vector11.X = vector6.X;
		}
		if (vector6.Y > vector11.Y)
		{
			vector11.Y = vector6.Y;
		}
		rect = new Rectangle((int)vector10.X, (int)vector10.Y, (int)(vector11.X - vector10.X), (int)(vector11.Y - vector10.Y));
	}
}
