using System;
using System.IO;
using Microsoft.Xna.Framework;
using ZP2K9.net;
using Yuki_Win;

namespace ZP2K9.net;

public static class NetPacker
{
	public static void WriteVec2(PacketWriter writer, Vector2 v)
	{
		((BinaryWriter)(object)writer).Write(FloatToInt16(v.X));
		((BinaryWriter)(object)writer).Write(FloatToInt16(v.Y));
	}

	public static void WriteMsg(PacketWriter writer, byte msg)
	{
		((BinaryWriter)(object)writer).Write(msg);
		((BinaryWriter)(object)writer).Write((short)1337);
	}

	public static Vector2 ReadVec2(PacketReader reader)
	{
		return new Vector2(((BinaryReader)(object)reader).ReadInt16(), ((BinaryReader)(object)reader).ReadInt16());
	}

	public static void WriteSByte(PacketWriter writer, int i)
	{
		((BinaryWriter)(object)writer).Write(IntToSByte(i));
	}

	public static int ReadSByte(PacketReader reader)
	{
		return ((BinaryReader)(object)reader).ReadSByte();
	}

	public static void WriteByte(PacketWriter writer, int i)
	{
		((BinaryWriter)(object)writer).Write(IntToByte(i));
	}

	public static int ReadByte(PacketReader reader)
	{
		return ((BinaryReader)(object)reader).ReadByte();
	}

	public static void WriteNormalizedVec2(PacketWriter writer, Vector2 v)
	{
		float angle = Trig.GetAngle(default(Vector2), v);
		angle /= 6.28f;
		angle *= 256f;
		((BinaryWriter)(object)writer).Write(IntToByte((int)angle));
	}

	public static Vector2 ReadNormalizedVec2(PacketReader reader)
	{
		float num = ReadByte(reader);
		num /= 256f;
		num *= 6.28f;
		return new Vector2((float)Math.Cos(num) * -1f, (float)Math.Sin(num) * -1f);
	}

	public static void WriteRadian(PacketWriter writer, float a)
	{
		while (a > 6.28f)
		{
			a -= 6.28f;
		}
		while (a < 0f)
		{
			a += 6.28f;
		}
		a /= 6.28f;
		a *= 256f;
		WriteByte(writer, (int)a);
	}

	public static float ReadRadian(PacketReader reader)
	{
		float num = ReadByte(reader);
		num /= 256f;
		return num * 6.28f;
	}

	public static byte SmallFloatToByte(float f)
	{
		f *= 10f;
		if (f > 255f)
		{
			f = 255f;
		}
		if (f < 0f)
		{
			f = 0f;
		}
		return (byte)f;
	}

	public static float ByteToSmallFloat(byte b)
	{
		float num = (int)b;
		return num / 10f;
	}

	public static byte TinyFloatToByte(float f)
	{
		f *= 50f;
		if (f > 255f)
		{
			f = 255f;
		}
		if (f < 0f)
		{
			f = 0f;
		}
		return (byte)f;
	}

	public static float ByteToTinyFloat(byte b)
	{
		float num = (int)b;
		return num / 50f;
	}

	public static byte SmallFloatToSByte(float f)
	{
		f *= 10f;
		if (f > 127f)
		{
			f = 127f;
		}
		if (f < -128f)
		{
			f = -128f;
		}
		return (byte)f;
	}

	public static float SByteToSmallFloat(sbyte b)
	{
		float num = b;
		return num / 10f;
	}

	public static byte IntToByte(int i)
	{
		if (i > 255)
		{
			i = 255;
		}
		if (i < 0)
		{
			i = 0;
		}
		return (byte)i;
	}

	public static sbyte IntToSByte(int i)
	{
		if (i > 127)
		{
			i = 127;
		}
		if (i < -128)
		{
			i = -128;
		}
		return (sbyte)i;
	}

	public static short FloatToInt16(float f)
	{
		if (f > 32767f)
		{
			f = 32767f;
		}
		if (f < -32768f)
		{
			f = -32768f;
		}
		return (short)f;
	}

	public static short IntToInt16(int i)
	{
		if (i > 32767)
		{
			i = 32767;
		}
		if (i < -32768)
		{
			i = -32768;
		}
		return (short)i;
	}
}
