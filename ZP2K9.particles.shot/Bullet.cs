using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ZP2K9.net;
using Yuki_Win;
using ZP2K9.characters;
using ZP2K9.map;

namespace ZP2K9.particles.shot;

public class Bullet
{
	public static void Init(Particle p, Vector2 loc, Vector2 traj, int owner, int damage)
	{
		p.netSprite = 7;
		p.loc = loc;
		p.orig = loc;
		p.traj = traj;
		p.netOwner = owner;
		p.alpha = true;
		p.frame = 0.3f;
		p.flags = damage;
		p.netWeak = true;
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
		p.frame = 0.3f;
		p.alpha = true;
		HitManager.CheckNetFixHit(p);
	}

	public static void Update(Particle p, GameMap map, Character[] c, float fTime)
	{
		if (HitManager.CheckHit(c, p, map, p.netOwner))
		{
			p.frame = -1f;
		}
		else if (map.GetIsCol(p.loc))
		{
			p.frame = -1f;
			for (int i = 0; i < 4; i++)
			{
				Game1.pMan.AddParticle(2, p.loc, -p.traj * Rand.GetRandomFloat(0f, 0.1f), Rand.GetRandomFloat(0.1f, 0.3f), 0, -1);
			}
		}
		for (int j = 0; j < 2; j++)
		{
			Vector2 loc = p.loc + p.traj * Rand.GetRandomFloat(0f, 0.03f);
			int num = (int)(loc.X / 64f);
			int num2 = (int)(loc.Y / 32f);
			if (num > 0 && num > 0 && num2 < 256 && num2 < 256 && map.water.water[num, num2])
			{
				Game1.pMan.AddParticle(50, loc, Rand.GetRandomVec2(-10f, 10f, -80f, -10f), Rand.GetRandomFloat(0.02f, 0.1f), 0, 0);
			}
		}
		p.BaseUpdate(map, c, fTime);
	}

	public static void Draw(Particle p, SpriteBatch sprite)
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		p.angle = Trig.GetAngle(default(Vector2), p.traj) + 3.14f;
		sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc), (Rectangle?)new Rectangle(128, 0, 128, 16), new Color(new Vector4(0.5f, 0.4f, 0.2f, 0.5f)), p.angle, new Vector2(64f, 8f), Scroll.zoom * 0.3f, (SpriteEffects)0, 1f);
	}
}
