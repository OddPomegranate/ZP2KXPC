using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ZP2K9.net;
using Yuki_Win;
using ZP2K9.characters;
using ZP2K9.map;

namespace ZP2K9.particles.shot;

public class Laser
{
	public static void Init(Particle p, Vector2 loc, Vector2 traj, int owner, int damage, float splash)
	{
		p.netSprite = 27;
		p.orig = loc;
		p.loc = loc;
		p.traj = traj;
		p.netOwner = owner;
		p.alpha = false;
		p.frame = 0.5f;
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
		p.alpha = true;
		p.frame = 0.5f;
	}

	public static void Update(Particle p, GameMap map, Character[] c, float fTime)
	{
		Vector2 loc = p.loc;
		float frame = p.frame;
		for (int i = 0; i < 10; i++)
		{
			p.BaseUpdate(map, c, fTime);
			if ((int)(frame * 60f) != (int)(p.frame * 60f))
			{
				Game1.pMan.AddParticle(48, p.loc - p.traj * 0.05f, p.traj * -0.03f, Rand.GetRandomFloat(0.15f, 0.2f), 0, 0);
			}
			if (HitManager.CheckHit(c, p, map, p.netOwner))
			{
				for (int j = 0; j < 5; j++)
				{
					Game1.pMan.AddParticle(48, p.loc, Rand.GetRandomVec2(-100f, 100f, -100f, 100f), Rand.GetRandomFloat(0.1f, 0.3f), 0, 0);
				}
			}
			else if (map.GetIsCol(p.loc) && p.frame > 0f)
			{
				p.frame = -1f;
				Game1.pMan.Explode(p.loc, p.netOwner, 0, 0f);
			}
			if (p.frame == -1f)
			{
				p.loc = loc;
				Game1.pMan.AddParticle(48, p.loc, p.traj * -0.03f, Rand.GetRandomFloat(0.15f, 0.2f), 0, 0);
				if ((p.loc - Scroll.scroll).LengthSquared() < 810000f)
				{
					Sound.PlayCue("shrinksplash");
				}
			}
			frame = p.frame;
			loc = p.loc;
			if (p.frame < 0f)
			{
				break;
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
