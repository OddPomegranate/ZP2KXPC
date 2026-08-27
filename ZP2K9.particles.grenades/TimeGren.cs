using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ZP2K9.net;
using ZP2K9.characters;
using ZP2K9.map;

namespace ZP2K9.particles.grenades;

public class TimeGren
{
	public static void Init(Particle p, Vector2 loc, Vector2 traj, int owner)
	{
		p.netSprite = 31;
		p.loc = loc;
		p.traj = traj;
		p.netOwner = owner;
		p.frame = 6f;
		p.bounce = true;
		p.dir = Rand.GetRandomFloat(0f, 6.28f);
	}

	public static void NetWrite(Particle p, PacketWriter writer)
	{
		NetPacker.WriteByte(writer, p.netSprite);
		NetPacker.WriteByte(writer, p.netOwner);
		NetPacker.WriteVec2(writer, p.loc);
		NetPacker.WriteVec2(writer, p.traj);
	}

	public static void NetInit(Particle p, PacketReader reader)
	{
		p.netInduced = true;
		p.netOwner = NetPacker.ReadByte(reader);
		p.loc = NetPacker.ReadVec2(reader);
		p.traj = NetPacker.ReadVec2(reader);
		p.frame = 6f;
		p.bounce = true;
		p.angle = Rand.GetRandomRadian();
		p.dir = Rand.GetRandomRadian();
	}

	public static void Update(Particle p, GameMap map, Character[] c, float fTime)
	{
		_ = p.frame;
		p.frame -= Game1.frameTime;
		if (!p.ground)
		{
			Vector2 loc = p.loc;
			p.loc += p.traj * Game1.frameTime;
			p.traj.Y += Game1.frameTime * Game1.gravity;
			if (map.GetIsCol(p.loc))
			{
				p.ground = true;
				p.loc = loc;
			}
			p.angle += p.dir * Game1.frameTime;
		}
		if (p.frame < 5f)
		{
			Game1.pMan.AddChrono(p.loc);
		}
		if (p.frame < 1f)
		{
			p.frame = -1f;
			Game1.pMan.Explode(p.loc, p.netOwner, 20, 20f);
			p.exists = false;
		}
	}

	public static void Draw(Particle p, SpriteBatch sprite)
	{
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		if (p.frame < 5f)
		{
			float num = (5f - p.frame) * 10f;
			if (num > 1f)
			{
				num = 1f;
			}
			if (p.frame < 1.1f)
			{
				num = (p.frame - 1f) * 10f;
			}
			num *= 1.5f;
			sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc), (Rectangle?)new Rectangle(256, 672, 128, 128), new Color(new Vector4(0f, 0f, 0f, 0.2f)), p.angle, new Vector2(64f, 64f), Scroll.zoom * num * 3.125f, (SpriteEffects)0, 1f);
			sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc), (Rectangle?)new Rectangle(384, 672, 64, 64), new Color(new Vector4(1f, 1f, 0f, 0.1f)), p.angle, new Vector2(32f, 32f), Scroll.zoom * num * 9.375f, (SpriteEffects)0, 1f);
			sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc), (Rectangle?)new Rectangle(544, 0, 96, 96), new Color(new Vector4(0f, 1f, 0f, 0.5f)), p.frame, new Vector2(48f, 48f), Scroll.zoom * num * 4.6875f, (SpriteEffects)0, 1f);
			sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc), (Rectangle?)new Rectangle(544, 0, 96, 96), new Color(new Vector4(0f, 0f, 1f, 0.5f)), 0f - p.frame, new Vector2(48f, 48f), Scroll.zoom * num * 4.6875f, (SpriteEffects)0, 1f);
		}
		sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc), (Rectangle?)new Rectangle(512, 448, 64, 64), new Color(new Vector4(1f, 1f, 1f, p.frame)), p.angle, new Vector2(32f, 32f), Scroll.zoom * 0.55f, (SpriteEffects)0, 1f);
	}
}
