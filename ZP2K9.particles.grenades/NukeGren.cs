using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ZP2K9.net;
using ZP2K9.characters;
using ZP2K9.map;

namespace ZP2K9.particles.grenades;

public class NukeGren
{
	public static void Init(Particle p, Vector2 loc, Vector2 traj, int owner)
	{
		p.netSprite = 33;
		p.loc = loc;
		p.traj = traj;
		p.netOwner = owner;
		p.frame = 5f;
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
		p.frame = 5f;
		p.bounce = true;
		p.angle = Rand.GetRandomRadian();
		p.dir = Rand.GetRandomRadian();
	}

	public static void Update(Particle p, GameMap map, Character[] c, float fTime)
	{
		float num = p.frame - fTime;
		if ((int)p.frame != (int)num && (Scroll.scroll - p.loc).LengthSquared() < 640000f)
		{
			switch ((int)p.frame)
			{
			case 3:
			case 4:
				Sound.PlayCue("beep");
				break;
			case 2:
				Sound.PlayCue("beep2");
				break;
			}
		}
		if (p.frame < 1f)
		{
			p.frame = -1f;
			int damage = 400;
			float range = 650f;
			if (Game1.character[p.netOwner] != null && Game1.character[p.netOwner].perk[1] == 7)
			{
				damage = 500;
				range = 700f;
			}
			Game1.pMan.Explode(p.loc + new Vector2(0f, -64f), p.netOwner, damage, range);
		}
		p.BaseUpdate(map, c, fTime);
	}

	public static void Draw(Particle p, SpriteBatch sprite)
	{
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		Vector2 loc = p.loc;
		float num = 1f;
		if (p.frame < 2f)
		{
			loc += (2f - p.frame) * Rand.GetRandomVec2(-4f, 4f, -4f, 4f);
			num += Rand.GetRandomFloat(0f, 2f - p.frame);
		}
		sprite.Draw(Game1.spritesTex, Scroll.GetLoc(loc), (Rectangle?)new Rectangle(640, 448, 64, 64), new Color(new Vector4(1f, 1f, 1f, p.frame)), p.angle, new Vector2(32f, 32f), Scroll.zoom * 0.55f * num, (SpriteEffects)0, 1f);
		if (p.frame < 4f)
		{
			float num2 = p.frame - (float)(int)p.frame - 0.8f;
			if (num2 > 0f)
			{
				Game1.postGlowMgr.Add(Scroll.GetLoc(p.loc), 1f, 0.1f, 0.1f, num2 * 10f, 1f);
			}
		}
	}
}
