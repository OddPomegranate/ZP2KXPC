using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ZP2K9.net;
using Yuki_Win;
using ZP2K9.characters;
using ZP2K9.map;

namespace ZP2K9.particles.shot;

public class Flare
{
	public static void Init(Particle p, Vector2 loc, Vector2 traj, int owner, int damage)
	{
		p.netSprite = 18;
		p.loc = loc;
		p.traj = traj;
		p.netOwner = owner;
		p.alpha = true;
		p.frame = 1f;
		p.flags = damage;
		p.netWeak = true;
		p.bounce = true;
	}

	public static void NetWrite(Particle p, PacketWriter writer)
	{
		NetPacker.WriteByte(writer, p.netSprite);
		NetPacker.WriteByte(writer, p.netOwner);
		NetPacker.WriteVec2(writer, p.loc);
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
		p.frame = 1f;
		p.bounce = true;
		p.alpha = true;
	}

	public static void Update(Particle p, GameMap map, Character[] c, float fTime)
	{
		float num = p.frame - fTime;
		if ((int)(num * 60f) != (int)(p.frame * 60f))
		{
			Game1.pMan.AddParticle(32, p.loc, Rand.GetRandomVec2(-10f, 10f, -100f, 0f), 0f, 0, 0);
		}
		if (HitManager.CheckHit(c, p, map, p.netOwner))
		{
			p.frame = -1f;
		}
		int num2 = (int)(p.loc.X / 64f);
		int num3 = (int)(p.loc.Y / 32f);
		if (num2 > 0 && num2 > 0 && num3 < 256 && num3 < 256 && map.water.water[num2, num3])
		{
			p.frame -= fTime * 10f;
		}
		p.BaseUpdate(map, c, fTime);
	}

	public static void Draw(Particle p, SpriteBatch sprite)
	{
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		p.angle = Trig.GetAngle(default(Vector2), p.traj) + 3.14f;
		for (int i = 0; i < 2; i++)
		{
			sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc), (Rectangle?)new Rectangle(0, 0, 64, 64), new Color(Rand.GetRandomFloat(0.3f, 0.9f) * new Vector4(1f, 0.9f, 0.8f, 1f)), Rand.GetRandomRadian(), new Vector2(32f, 32f), Scroll.zoom * 0.6f, (SpriteEffects)0, 1f);
		}
		float num = p.frame;
		if (p.frame > 0.95f)
		{
			num = (1f - p.frame) * 4f;
		}
		if (num > 0.2f)
		{
			num = 0.2f;
		}
		Game1.postGlowMgr.Add(Scroll.GetLoc(p.loc), 1f, 0.2f, 0.1f, num, Rand.GetRandomFloat(1.5f, 2f));
		Game1.postGlowMgr.Add(Scroll.GetLoc(p.loc), 1f, 0.5f, 1f, num, Rand.GetRandomFloat(0.75f, 1f));
	}
}
