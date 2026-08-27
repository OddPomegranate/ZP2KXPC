using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ZP2K9.net;
using ZP2K9.characters;
using ZP2K9.map;

namespace ZP2K9.particles.grenades;

public class GLaunch
{
	public static void Init(Particle p, Vector2 loc, Vector2 traj, int owner, int damage, float splash)
	{
		p.netSprite = 2;
		p.loc = loc;
		p.orig = loc;
		p.traj = traj;
		p.netOwner = owner;
		p.frame = 3f;
		p.flags = damage;
		p.bounce = true;
		p.size = splash;
		p.dir = Rand.GetRandomFloat(0f, 6.28f);
	}

	public static void NetWrite(Particle p, PacketWriter writer)
	{
		NetPacker.WriteByte(writer, p.netSprite);
		NetPacker.WriteByte(writer, p.netOwner);
		NetPacker.WriteVec2(writer, p.orig);
		NetPacker.WriteVec2(writer, p.traj);
		NetPacker.WriteByte(writer, p.flags);
		((BinaryWriter)(object)writer).Write(NetPacker.SmallFloatToByte(p.frame));
		((BinaryWriter)(object)writer).Write(NetPacker.SmallFloatToByte(p.size));
	}

	public static void NetInit(Particle p, PacketReader reader)
	{
		p.netInduced = true;
		p.netOwner = NetPacker.ReadByte(reader);
		p.loc = NetPacker.ReadVec2(reader);
		p.traj = NetPacker.ReadVec2(reader);
		p.flags = NetPacker.ReadByte(reader);
		p.frame = NetPacker.ByteToSmallFloat(((BinaryReader)(object)reader).ReadByte());
		p.size = NetPacker.ByteToSmallFloat(((BinaryReader)(object)reader).ReadByte());
		p.bounce = true;
		p.angle = Rand.GetRandomRadian();
		p.dir = Rand.GetRandomRadian();
	}

	public static void Update(Particle p, GameMap map, Character[] c, float fTime)
	{
		float num = p.frame - fTime;
		if (!p.ground && (int)(num * 60f) != (int)(p.frame * 60f))
		{
			Game1.pMan.AddParticle(2, p.loc, Rand.GetRandomVec2(-20f, 20f, -100f, 0f), Rand.GetRandomFloat(0.15f, 0.3f), 0, 0);
		}
		if (HitManager.CheckHit(c, p, map, p.netOwner))
		{
			p.frame = -1f;
		}
		if (p.frame < 1f)
		{
			p.frame = -1f;
		}
		if (p.frame == -1f)
		{
			Game1.pMan.Explode(p.loc, p.netOwner, p.flags, p.size);
		}
		p.BaseUpdate(map, c, fTime);
	}

	public static void Draw(Particle p, SpriteBatch sprite)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc), (Rectangle?)new Rectangle(192, 512, 64, 64), Color.White, p.angle, new Vector2(32f, 32f), Scroll.zoom * 0.65f, (SpriteEffects)0, 1f);
		Game1.postGlowMgr.Add(Scroll.GetLoc(p.loc), 1f, 0f, 0f, 1f, 0f, 0.05f);
	}
}
