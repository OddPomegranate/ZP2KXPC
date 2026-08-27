using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ZP2K9.net;
using ZP2K9.characters;
using ZP2K9.map;

namespace ZP2K9.particles.shot;

public class Plasma
{
	public static void Init(Particle p, Vector2 loc, Vector2 traj, int owner, int damage)
	{
		p.netSprite = 10;
		p.loc = loc;
		p.orig = loc;
		p.traj = traj;
		p.netOwner = owner;
		p.alpha = true;
		p.frame = 1.2f;
		p.flags = damage;
		p.dir = Rand.GetRandomFloat(-10f, 10f);
		p.angle = Rand.GetRandomRadian();
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
		p.frame = 1.2f;
		p.dir = Rand.GetRandomFloat(-10f, 10f);
		p.angle = Rand.GetRandomRadian();
		p.alpha = true;
	}

	public static void Update(Particle p, GameMap map, Character[] c, float fTime)
	{
		float num = p.frame - fTime;
		if ((int)(p.frame * 20f) != (int)(num * 20f))
		{
			Game1.pMan.AddParticle(17, p.loc, p.traj * 0.1f + Rand.GetRandomVec2(-50f, 50f, -50f, 50f), 0f, 0, 0);
		}
		Vector2 loc = p.loc;
		p.frame -= fTime;
		if (HitManager.CheckHit(c, p, map, p.netOwner))
		{
			p.frame = -1f;
		}
		else
		{
			p.loc.X += p.traj.X * fTime;
			if (map.GetIsCol(p.loc))
			{
				p.loc.X = loc.X;
				p.traj.X = 0f - p.traj.X;
			}
			p.loc.Y += p.traj.Y * fTime;
			if (map.GetIsCol(p.loc))
			{
				p.loc.Y = loc.Y;
				p.traj.Y = 0f - p.traj.Y;
			}
		}
		if (p.frame == -1f)
		{
			for (int i = 0; i < 10; i++)
			{
				Game1.pMan.AddParticle(16, p.loc, Rand.GetRandomVec2(-100f, 100f, -100f, 100f) - p.traj * 0.1f, 0f, 0, 0);
			}
			for (int j = 0; j < 3; j++)
			{
				Game1.pMan.AddParticle(17, p.loc, Rand.GetRandomVec2(-100f, 100f, -100f, 100f) - p.traj * 0.1f, 0f, 0, 0);
			}
			if ((p.loc - Scroll.scroll).LengthSquared() < 810000f)
			{
				Sound.PlayCue("plasmahit");
			}
		}
	}

	public static void Draw(Particle p, SpriteBatch sprite)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc), (Rectangle?)new Rectangle(0, 0, 64, 64), new Color(new Vector4(0.3f, 1f, 0.3f, 0.5f)), p.angle + p.frame * p.dir, new Vector2(32f, 32f), Scroll.zoom * 0.8f, (SpriteEffects)0, 1f);
		sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc), (Rectangle?)new Rectangle(0, 0, 64, 64), new Color(new Vector4(0.3f, 0.3f, 1f, 0.5f)), p.angle + p.frame * (0f - p.dir), new Vector2(32f, 32f), Scroll.zoom * 0.8f, (SpriteEffects)0, 1f);
	}
}
