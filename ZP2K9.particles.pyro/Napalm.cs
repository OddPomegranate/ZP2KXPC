using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ZP2K9.net;
using ZP2K9.characters;
using ZP2K9.map;

namespace ZP2K9.particles.pyro;

public class Napalm
{
	public static void Init(Particle p, Vector2 loc, Vector2 traj, int owner)
	{
		p.netSprite = 14;
		p.loc = loc;
		p.traj = traj;
		p.frame = 4f;
		p.netOwner = owner;
		p.alpha = true;
		p.size = Rand.GetRandomFloat(2f, 3f);
		p.dir = 0.6f;
		p.angle = Rand.GetRandomFloat(0f, 2f);
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
		p.alpha = true;
		p.size = Rand.GetRandomFloat(2f, 3f);
		p.dir = 0.6f;
	}

	public static void Update(Particle p, GameMap map, Character[] c, float fTime)
	{
		HitManager.CheckHit(c, p, map, p.netOwner);
		if (!p.ground)
		{
			p.traj.Y += fTime * Game1.gravity;
			if (map.GetIsCol(p.loc))
			{
				p.ground = true;
				p.traj.X = 0f;
				p.traj.Y = 0f;
			}
			int num = (int)(p.loc.X / 64f);
			int num2 = (int)((p.loc.Y - 10f) / 32f);
			if (num > 0 && num > 0 && num2 < 256 && num2 < 256 && map.water.water[num, num2])
			{
				p.frame -= fTime * 80f;
			}
		}
		else if (p.dir < 1f)
		{
			p.dir += fTime;
		}
		p.BaseUpdate(map, c, fTime);
	}

	public static void Draw(Particle p, SpriteBatch sprite)
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		int num = (int)((10f + p.angle - p.frame) / 0.4f * 9f);
		num %= 9;
		sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc), (Rectangle?)new Rectangle(num * 32, 224, 32, 64), new Color(new Vector4(1f, 1f, 1f, p.frame * p.dir)), (float)Math.Sin(p.frame * 5f + p.size) * 0.2f - 0.1f, new Vector2(16f, 50f), Scroll.zoom * p.size * new Vector2(1f, p.dir * (1f + (float)Math.Cos((double)p.frame * 8.0) * 0.2f)), (SpriteEffects)0, 1f);
		float num2 = p.frame * 0.2f;
		if (p.frame > 3.8f)
		{
			num2 = (4f - p.frame) * 0.5f;
		}
		if (num2 > 0.04f)
		{
			num2 = 0.04f;
		}
		if (Game1.postGlowMgr.totalGlows < 50)
		{
			Game1.postGlowMgr.Add(Scroll.GetLoc(p.loc + new Vector2(0f, -40f)), 0.5f, 0.25f, 0.12f, num2, Rand.GetRandomFloat(2f, 3f));
		}
	}
}
