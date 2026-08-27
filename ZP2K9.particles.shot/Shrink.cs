using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ZP2K9.net;
using Yuki_Win;
using ZP2K9.characters;
using ZP2K9.map;

namespace ZP2K9.particles.shot;

public class Shrink
{
	public static void Init(Particle p, Vector2 loc, Vector2 traj, int owner, int damage, float splash)
	{
		p.netSprite = 19;
		p.loc = loc;
		p.traj = traj;
		p.netOwner = owner;
		p.alpha = false;
		p.frame = 1f;
		p.flags = damage;
		p.size = splash;
		p.alpha = true;
	}

	public static void NetWrite(Particle p, PacketWriter writer)
	{
		NetPacker.WriteByte(writer, p.netSprite);
		NetPacker.WriteByte(writer, p.netOwner);
		NetPacker.WriteVec2(writer, p.loc);
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
		p.alpha = true;
		p.frame = 0.35f;
	}

	public static void Update(Particle p, GameMap map, Character[] c, float fTime)
	{
		Vector2 loc = p.loc;
		float frame = p.frame;
		p.BaseUpdate(map, c, fTime);
		if ((int)(frame * 60f) != (int)(p.frame * 60f))
		{
			Game1.pMan.AddParticle(36, p.loc - p.traj * 0.01f, p.traj * -0.3f, Rand.GetRandomFloat(0.15f, 0.2f), 0, 0);
			Game1.pMan.AddParticle(41, p.loc - p.traj * 0.01f, p.traj * -0.3f + Rand.GetRandomVec2(-100f, 100f, -100f, 100f), Rand.GetRandomFloat(0.15f, 0.2f), 0, 0);
		}
		if (HitManager.CheckHit(c, p, map, p.netOwner))
		{
			p.frame = -1f;
		}
		else if (map.GetIsCol(p.loc))
		{
			p.frame = -1f;
		}
		if (p.frame == -1f)
		{
			p.loc = loc;
			for (int i = 0; i < 32; i++)
			{
				Game1.pMan.AddParticle(36, p.loc, Rand.GetRandomVec2(-100f, 100f, -200f, 200f), Rand.GetRandomFloat(0.1f, 0.2f), 0, 0);
				Game1.pMan.AddParticle(41, p.loc, Rand.GetRandomVec2(-100f, 100f, -200f, 200f), Rand.GetRandomFloat(0.1f, 0.2f), 0, 0);
			}
			if ((p.loc - Scroll.scroll).LengthSquared() < 810000f)
			{
				Sound.PlayCue("shrinksplash");
			}
		}
	}

	public static void Draw(Particle p, SpriteBatch sprite)
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		p.angle = Trig.GetAngle(default(Vector2), p.traj) + 3.14f;
		sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc), (Rectangle?)new Rectangle(64, 0, 0, 0), new Color(new Vector4(0.1f, 0.5f, 1f, 1f)), Rand.GetRandomRadian(), new Vector2(32f, 32f), Scroll.zoom * 0.4f, (SpriteEffects)0, 1f);
	}
}
