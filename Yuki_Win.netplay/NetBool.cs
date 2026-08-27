using System.IO;
using ZP2K9.net;

namespace Yuki_Win.netplay;

internal class NetBool
{
	private const byte ONE = 1;

	private const byte ZERO = 0;

	public bool[] val;

	private byte[] place = new byte[8] { 1, 2, 4, 8, 16, 32, 64, 128 };

	public NetBool()
	{
		val = new bool[8];
		place = new byte[8] { 1, 2, 4, 8, 16, 32, 64, 128 };
	}

	public void Write(PacketWriter writer)
	{
		byte b = 0;
		for (int i = 0; i < val.Length; i++)
		{
			b += boolToByte(val[i], i);
		}
		((BinaryWriter)(object)writer).Write(b);
	}

	public void Read(PacketReader reader)
	{
		byte b = ((BinaryReader)(object)reader).ReadByte();
		for (int i = 0; i < val.Length; i++)
		{
			val[i] = byteToBool(b, i);
		}
	}

	private bool byteToBool(byte val, int p)
	{
		int num = val & place[p];
		return num > 0;
	}

	private byte boolToByte(bool val, int p)
	{
		byte b = (byte)(val ? 1 : 0);
		return (byte)(b * place[p]);
	}
}
