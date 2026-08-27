using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ZP2K9.net;
using ZP2K9.characters;
using ZP2K9.map;

namespace ZP2K9.particles.shot;

public class Freeze
{
	public static void Init(Particle p, Vector2 loc, Vector2 traj, int owner, int damage)
	{
		p.netSprite = 20;
		p.loc = loc;
		p.traj = traj;
		p.netOwner = owner;
		p.alpha = true;
		p.frame = 0.3f;
		p.flags = damage;
		p.dir = Rand.GetRandomFloat(-10f, 10f);
		p.angle = Rand.GetRandomRadian();
		p.netWeak = true;
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
		p.frame = 0.3f;
		p.dir = Rand.GetRandomFloat(-10f, 10f);
		p.angle = Rand.GetRandomRadian();
		p.alpha = true;
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
		}
		if (p.frame == -1f)
		{
			for (int i = 0; i < 10; i++)
			{
				Game1.pMan.AddParticle(38, p.loc, Rand.GetRandomVec2(-100f, 100f, -100f, 100f) - p.traj * 0.1f, 0f, 0, 0);
			}
			for (int j = 0; j < 3; j++)
			{
				Game1.pMan.AddParticle(38, p.loc, Rand.GetRandomVec2(-100f, 100f, -100f, 100f) - p.traj * 0.1f, 0f, 0, 0);
			}
		}
		p.BaseUpdate(map, c, fTime);
	}

	public static void Draw(Particle p, SpriteBatch sprite)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc), (Rectangle?)new Rectangle(0, 0, 64, 64), new Color(new Vector4(1f, 1f, 1f, 0.5f)), p.angle + p.frame * p.dir, new Vector2(32f, 32f), Scroll.zoom * 1f * p.frame, (SpriteEffects)0, 1f);
		sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc), (Rectangle?)new Rectangle(0, 0, 64, 64), new Color(new Vector4(0f, 0f, 1f, 0.5f)), p.angle - p.frame * p.dir, new Vector2(32f, 32f), Scroll.zoom * 1f * p.frame, (SpriteEffects)0, 1f);
	}
}
