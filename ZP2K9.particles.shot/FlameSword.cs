using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ZP2K9.net;
using ZP2K9.characters;
using ZP2K9.map;

namespace ZP2K9.particles.shot;

public class FlameSword
{
	public static void Init(Particle p, Vector2 loc, Vector2 traj, int owner, int damage)
	{
		p.netSprite = 37;
		p.loc = loc;
		p.traj = traj;
		p.netOwner = owner;
		p.alpha = true;
		p.frame = 0.09f;
		p.flags = damage;
		p.orig = loc;
		p.size = 80f;
	}

	public static void NetWrite(Particle p, PacketWriter writer)
	{
		NetPacker.WriteByte(writer, p.netSprite);
		NetPacker.WriteByte(writer, p.netOwner);
		NetPacker.WriteVec2(writer, p.orig);
		NetPacker.WriteVec2(writer, p.traj);
		NetPacker.WriteByte(writer, p.flags);
	}

	public static void NetInit(Particle p, PacketReader reader)
	{
		p.netInduced = true;
		p.netOwner = NetPacker.ReadByte(reader);
		p.loc = NetPacker.ReadVec2(reader);
		p.traj = NetPacker.ReadVec2(reader);
		p.flags = NetPacker.ReadByte(reader);
		p.frame = 0.09f;
		p.alpha = true;
		p.size = 80f;
	}

	public static void Update(Particle p, GameMap map, Character[] c, float fTime)
	{
		if (map.GetIsCol(p.loc))
		{
			p.frame = -1f;
		}
		else if (HitManager.CheckHit(c, p, map, p.netOwner))
		{
			p.frame = -1f;
		}
		p.BaseUpdate(map, c, fTime);
	}

	public static void Draw(Particle p, SpriteBatch sprite)
	{
	}
}
