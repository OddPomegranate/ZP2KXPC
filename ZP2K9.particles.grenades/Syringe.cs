using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ZP2K9.net;
using Yuki_Win;
using ZP2K9.characters;
using ZP2K9.map;

namespace ZP2K9.particles.grenades;

public class Syringe
{
	public static void Init(Particle p, Vector2 loc, Vector2 traj, int owner, int damage)
	{
		p.netSprite = 29;
		p.loc = loc;
		p.orig = loc;
		p.traj = traj;
		p.netOwner = owner;
		p.frame = 1f;
		p.flags = damage;
	}

	public static void NetWrite(Particle p, PacketWriter writer)
	{
		NetPacker.WriteByte(writer, p.netSprite);
		NetPacker.WriteByte(writer, p.netOwner);
		NetPacker.WriteVec2(writer, p.orig);
		NetPacker.WriteVec2(writer, p.traj);
		NetPacker.WriteByte(writer, p.flags);
		((BinaryWriter)(object)writer).Write(NetPacker.SmallFloatToByte(p.frame));
	}

	public static void NetInit(Particle p, PacketReader reader)
	{
		p.netInduced = true;
		p.netOwner = NetPacker.ReadByte(reader);
		p.loc = NetPacker.ReadVec2(reader);
		p.traj = NetPacker.ReadVec2(reader);
		p.flags = NetPacker.ReadByte(reader);
		p.frame = NetPacker.ByteToSmallFloat(((BinaryReader)(object)reader).ReadByte());
	}

	public static void Update(Particle p, GameMap map, Character[] c, float fTime)
	{
		_ = p.frame;
		p.traj.Y += fTime * Game1.gravity;
		if (HitManager.CheckHit(c, p, map, p.netOwner))
		{
			p.frame = -1f;
		}
		else if (map.GetIsCol(p.loc))
		{
			p.frame = -1f;
			Game1.pMan.AddParticle(57, p.loc - p.traj * 0.03f, p.traj * -0.2f, 0f, 0, -1);
		}
		int num = (int)(p.loc.X / 64f);
		int num2 = (int)(p.loc.Y / 32f);
		if (num > 0 && num > 0 && num2 < 256 && num2 < 256 && map.water.water[num, num2])
		{
			if (p.traj.X > 50f)
			{
				p.traj.X = 50f;
			}
			if (p.traj.X < -50f)
			{
				p.traj.X = -50f;
			}
			if (p.traj.Y > 50f)
			{
				p.traj.Y = 50f;
			}
		}
		if (p.frame < 0f)
		{
			p.frame = -1f;
		}
		p.BaseUpdate(map, c, fTime);
	}

	public static void Draw(Particle p, SpriteBatch sprite)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc), (Rectangle?)new Rectangle(832, 192, 48, 32), Color.White, Trig.GetAngle(default(Vector2), p.traj), new Vector2(24f, 16f), Scroll.zoom * 0.65f, (SpriteEffects)0, 1f);
	}
}
