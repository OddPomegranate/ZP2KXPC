using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ZP2K9.net;
using Yuki_Win;
using ZP2K9.characters;
using ZP2K9.map;

namespace ZP2K9.particles.shot;

public class Kick
{
	private const float FRAME_NORMAL = 0.09f;

	private const float FRAME_KUNG_FU = 0.12f;

	public static void Init(Particle p, Vector2 loc, Vector2 traj, int owner, int damage)
	{
		p.netSprite = 8;
		p.loc = loc;
		p.traj = traj;
		p.netOwner = owner;
		p.alpha = true;
		SetFrame(p);
		p.flags = damage;
		p.orig = loc;
	}

	private static void SetFrame(Particle p)
	{
		if (p.netOwner > -1 && Game1.character[p.netOwner] != null && Game1.character[p.netOwner].perk[0] == 2)
		{
			p.frame = 0.12f;
		}
		else
		{
			p.frame = 0.09f;
		}
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
		SetFrame(p);
		p.alpha = true;
	}

	public static void Update(Particle p, GameMap map, Character[] c, float fTime)
	{
		if (HitManager.CheckHit(c, p, map, p.netOwner))
		{
			p.frame = -1f;
		}
		p.BaseUpdate(map, c, fTime);
	}

	public static void Draw(Particle p, SpriteBatch sprite)
	{
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		if (p.netOwner > -1 && Game1.character[p.netOwner] != null && Game1.character[p.netOwner].perk[0] == 2)
		{
			sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc), (Rectangle?)new Rectangle(0, 0, 64, 64), new Color(0.1f, 0.5f, 1f, p.frame * 7f), Trig.GetAngle(default(Vector2), p.traj), new Vector2(16f, 32f), Scroll.zoom * new Vector2(3f, 0.4f), (SpriteEffects)0, 1f);
			Quake.SetQuake(0.2f);
		}
	}
}
