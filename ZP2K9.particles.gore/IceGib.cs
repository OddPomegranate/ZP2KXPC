using Microsoft.Xna.Framework;
using ZP2K9.net;
using ZP2K9.characters;
using ZP2K9.map;

namespace ZP2K9.particles.gore;

public class IceGib
{
	public static void Init(Particle p, Vector2 loc, Vector2 traj, int owner)
	{
		p.netSprite = 23;
		p.loc = loc;
		p.traj = traj;
		p.netOwner = owner;
		p.frame = 3f;
		p.bounce = true;
		p.dir = Rand.GetRandomFloat(0f, 6.28f);
	}

	public static void NetWrite(Particle p, PacketWriter writer)
	{
		NetPacker.WriteByte(writer, p.netSprite);
		NetPacker.WriteByte(writer, p.netOwner);
		NetPacker.WriteVec2(writer, p.loc);
		NetPacker.WriteVec2(writer, p.traj);
	}

	public static void NetInit(Particle p, PacketReader reader)
	{
		p.netInduced = true;
		p.netOwner = NetPacker.ReadByte(reader);
		p.loc = NetPacker.ReadVec2(reader);
		p.traj = NetPacker.ReadVec2(reader);
		p.frame = 3f;
		p.bounce = true;
		p.angle = Rand.GetRandomRadian();
		p.dir = Rand.GetRandomRadian();
	}

	public static void Update(Particle p, GameMap map, Character[] c, float fTime)
	{
		float num = p.frame - fTime;
		if (!p.ground && (int)(num * 60f) != (int)(p.frame * 60f))
		{
			Game1.pMan.AddParticle(38, p.loc, Rand.GetRandomVec2(-20f, 20f, 0f, 100f), Rand.GetRandomFloat(0.15f, 0.8f), 0, 0);
		}
		p.BaseUpdate(map, c, fTime);
	}
}
