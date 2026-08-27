using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ZP2K9.net;
using ZP2K9.characters;
using ZP2K9.map;

namespace ZP2K9.particles.shot;

public class FireRocket
{
	public static void Init(Particle p, Vector2 loc, Vector2 traj, int owner, int damage, float splash)
	{
		p.netSprite = 36;
		p.loc = loc;
		p.orig = loc;
		p.traj = traj;
		p.netOwner = owner;
		p.alpha = false;
		p.frame = 1f;
		p.flags = damage;
		p.size = splash;
	}

	public static void NetWrite(Particle p, PacketWriter writer)
	{
		NetPacker.WriteByte(writer, p.netSprite);
		NetPacker.WriteByte(writer, p.netOwner);
		NetPacker.WriteVec2(writer, p.orig);
		NetPacker.WriteVec2(writer, p.traj);
		NetPacker.WriteByte(writer, p.flags);
		NetPacker.WriteByte(writer, (int)p.size);
	}

	public static void NetInit(Particle p, PacketReader reader)
	{
		p.netInduced = true;
		p.netOwner = NetPacker.ReadByte(reader);
		p.loc = NetPacker.ReadVec2(reader);
		p.traj = NetPacker.ReadVec2(reader);
		p.flags = NetPacker.ReadByte(reader);
		p.size = NetPacker.ReadByte(reader);
		p.frame = 1f;
	}

	public static void Update(Particle p, GameMap map, Character[] c, float fTime)
	{
		Vector2 loc = p.loc;
		p.traj.Y += fTime * Game1.gravity * 0.6f;
		float frame = p.frame;
		p.BaseUpdate(map, c, fTime);
		if ((int)(frame * 60f) != (int)(p.frame * 60f))
		{
			Game1.pMan.AddParticle(2, p.loc - p.traj * 0.01f, p.traj * -0.3f, Rand.GetRandomFloat(0.5f, 0.7f), 0, 0);
			Game1.pMan.AddParticle(1, p.loc - p.traj * 0.01f, p.traj * -0.2f - Rand.GetRandomVec2(-10f, 10f, -10f, 40f), Rand.GetRandomFloat(0.25f, 0.5f), 0, 0);
			Game1.pMan.AddParticle(1, p.loc - p.traj * 0.02f, p.traj * -0.2f, Rand.GetRandomFloat(0.25f, 0.5f), 0, 0);
		}
		if (HitManager.CheckHit(c, p, map, p.netOwner))
		{
			p.frame = -1f;
		}
		else if (map.GetIsCol(p.loc))
		{
			p.frame = -1f;
		}
		if (p.frame != -1f)
		{
			return;
		}
		p.loc = loc;
		Game1.pMan.Explode(p.loc, p.netOwner, p.flags, p.size);
		for (int i = 0; i < 16; i++)
		{
			float num = (float)i / 16f * 6.28f;
			Vector2 traj = new Vector2((float)Math.Cos(num), (float)Math.Sin(num));
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
		Game1.postGlowMgr.Add(Scroll.GetLoc(p.loc), 1f, 0.25f, 0.1f, 0.4f, 1f);
		Game1.postGlowMgr.Add(Scroll.GetLoc(p.loc), 1f, 0.7f, 0.2f, 0.4f, 0.5f);
	}
}
