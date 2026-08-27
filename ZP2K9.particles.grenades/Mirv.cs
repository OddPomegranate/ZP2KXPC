using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ZP2K9.net;
using ZP2K9.characters;
using ZP2K9.map;

namespace ZP2K9.particles.grenades;

public class Mirv
{
	public static void Init(Particle p, Vector2 loc, Vector2 traj, int owner)
	{
		p.netSprite = 5;
		p.loc = loc;
		p.traj = traj;
		p.netOwner = owner;
		p.frame = 2f;
		p.bounce = true;
		p.dir = Rand.GetRandomFloat(0f, 6.28f);
	}

	public static void NetWrite(Particle p, PacketWriter writer)
	{
		NetPacker.WriteByte(writer, p.netSprite);
		NetPacker.WriteByte(writer, p.netOwner);
		NetPacker.WriteVec2(writer, p.loc);
		NetPacker.WriteVec2(writer, p.traj);
		((BinaryWriter)(object)writer).Write(NetPacker.SmallFloatToByte(p.frame));
	}

	public static void NetInit(Particle p, PacketReader reader)
	{
		p.netInduced = true;
		p.netOwner = NetPacker.ReadByte(reader);
		p.loc = NetPacker.ReadVec2(reader);
		p.traj = NetPacker.ReadVec2(reader);
		p.frame = NetPacker.ByteToSmallFloat(((BinaryReader)(object)reader).ReadByte());
		p.bounce = true;
		p.angle = Rand.GetRandomRadian();
		p.dir = Rand.GetRandomRadian();
	}

	public static void Update(Particle p, GameMap map, Character[] c, float fTime)
	{
		if (p.frame < 1f)
		{
			p.frame = -1f;
			Game1.pMan.Explode(p.loc, p.netOwner, 150, 200f);
			for (int i = 0; i < 5; i++)
			{
				Game1.pMan.AddParticle(8, p.loc, Rand.GetRandomVec2(-900f, 900f, -900f, 200f), 0f, 0, p.netOwner);
			}
		}
		p.BaseUpdate(map, c, fTime);
	}

	public static void Draw(Particle p, SpriteBatch sprite)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc), (Rectangle?)new Rectangle(192, 448, 64, 64), Color.White, p.angle, new Vector2(32f, 32f), Scroll.zoom * 0.55f, (SpriteEffects)0, 1f);
		Game1.postGlowMgr.Add(Scroll.GetLoc(p.loc), 1f, 0f, 0f, 1f, 0f, 0.05f);
	}
}
