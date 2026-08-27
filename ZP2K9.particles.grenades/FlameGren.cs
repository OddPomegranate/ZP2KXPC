using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ZP2K9.net;
using ZP2K9.characters;
using ZP2K9.map;

namespace ZP2K9.particles.grenades;

public class FlameGren
{
	public static void Init(Particle p, Vector2 loc, Vector2 traj, int owner)
	{
		p.netSprite = 1;
		p.loc = loc;
		p.traj = traj;
		p.netOwner = owner;
		p.frame = 2f;
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
		p.angle = Rand.GetRandomRadian();
		p.dir = Rand.GetRandomRadian();
	}

	public static void Update(Particle p, GameMap map, Character[] c, float fTime)
	{
		Vector2 loc = p.loc;
		p.traj.Y += fTime * Game1.gravity;
		p.angle += fTime * 5f;
		p.BaseUpdate(map, c, fTime);
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
		if (!map.GetIsCol(p.loc))
		{
			return;
		}
		Sound.PlayCue("explode");
		p.frame = -1f;
		Game1.pMan.AddParticle(1, loc, default(Vector2), 4f, 0, p.netOwner);
		for (int i = 0; i < 16; i++)
		{
			float num3 = (float)i / 16f * 6.28f;
			Vector2 traj = new Vector2((float)Math.Cos(num3), (float)Math.Sin(num3));
			if (i % 2 == 0)
			{
				traj *= 150f;
			}
			else
			{
				traj *= 300f;
			}
			if (Game1.character[p.netOwner] != null && Game1.character[p.netOwner].perk[1] == 7)
			{
				traj *= 1.2f;
			}
			Game1.pMan.AddParticle(15, loc, traj, 1f, 0, p.netOwner);
		}
	}

	public static void Draw(Particle p, SpriteBatch sprite)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc), (Rectangle?)new Rectangle(64, 448, 64, 64), Color.White, p.angle, new Vector2(32f, 32f), Scroll.zoom * 0.55f, (SpriteEffects)0, 1f);
	}
}
