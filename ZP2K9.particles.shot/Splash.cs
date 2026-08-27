using System.IO;
using Microsoft.Xna.Framework;
using ZP2K9.net;
using ZP2K9.characters;
using ZP2K9.map;

namespace ZP2K9.particles.shot;

public class Splash
{
	public static void Init(Particle p, Vector2 loc, int owner, int damage, float range)
	{
		p.netSprite = 13;
		p.loc = loc;
		p.frame = 10f;
		p.flags = damage;
		p.size = range;
		p.netOwner = owner;
	}

	public static void NetWrite(Particle p, PacketWriter writer)
	{
		NetPacker.WriteByte(writer, p.netSprite);
		NetPacker.WriteByte(writer, p.netOwner);
		NetPacker.WriteVec2(writer, p.loc);
		((BinaryWriter)(object)writer).Write(NetPacker.FloatToInt16(p.flags));
		((BinaryWriter)(object)writer).Write(NetPacker.FloatToInt16(p.size));
	}

	public static void NetInit(Particle p, PacketReader reader)
	{
		p.netInduced = true;
		p.netOwner = NetPacker.ReadByte(reader);
		p.loc = NetPacker.ReadVec2(reader);
		p.flags = ((BinaryReader)(object)reader).ReadInt16();
		p.size = ((BinaryReader)(object)reader).ReadInt16();
		p.frame = 10f;
	}

	public static void Update(Particle p, GameMap map, Character[] c, float fTime)
	{
		if (!p.ground)
		{
			HitManager.CheckHit(c, p, map, p.netOwner);
			p.ground = true;
		}
		p.frame -= fTime;
	}
}
