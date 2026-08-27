using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ZP2K9.net;
using ZP2K9.ai;
using ZP2K9.characters;
using ZP2K9.map;

namespace ZP2K9.particles.grenades;

public class ZapGren
{
	public static void Init(Particle p, Vector2 loc, Vector2 traj, int owner)
	{
		p.netSprite = 32;
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
		float num = p.frame - fTime;
		if (p.frame < 5f && (int)(p.frame * 5f) != (int)(num * 5f))
		{
			for (int i = 0; i < Game1.character.Length; i++)
			{
				if (Game1.character[i] != null && i != p.netOwner && HitManager.GetHostile(p.netOwner, i) && (p.loc - Game1.character[i].loc).LengthSquared() < 250000f && AI.GetVis(p.loc, Game1.character[i].loc + new Vector2(0f, -40f), map))
				{
					Vector2 traj = Game1.character[i].loc + new Vector2(0f, -40f) - p.loc;
					traj.Normalize();
					traj *= 1000f;
					Game1.pMan.AddParticle(45, p.loc, traj, 0f, 10, p.netOwner);
					if ((p.loc - Scroll.scroll).Length() < 900f)
					{
						Sound.PlayCue("plasma");
					}
					if (p.ground)
					{
						p.ground = false;
						p.traj = new Vector2(0f, -700f);
					}
				}
			}
		}
		if (p.frame < 1f)
		{
			p.frame = -1f;
			int damage = 200;
			float range = 200f;
			if (Game1.character[p.netOwner] != null && Game1.character[p.netOwner].perk[1] == 7)
			{
				damage = 250;
				range = 250f;
			}
			Game1.pMan.Explode(p.loc, p.netOwner, damage, range);
		}
		p.BaseUpdate(map, c, fTime);
	}

	public static void Draw(Particle p, SpriteBatch sprite)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc), (Rectangle?)new Rectangle(576, 448, 64, 64), new Color(new Vector4(1f, 1f, 1f, p.frame)), p.angle, new Vector2(32f, 32f), Scroll.zoom * 0.55f, (SpriteEffects)0, 1f);
		if (p.frame < 5f)
		{
			Game1.postGlowMgr.Add(Scroll.GetLoc(p.loc), 0.1f, 0.2f, 1f, Rand.GetRandomFloat(0.5f, 1f), Rand.GetRandomFloat(0.8f, 1f), 1f);
		}
	}
}
