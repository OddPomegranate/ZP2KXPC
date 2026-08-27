using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ZP2K9.net;
using ZP2K9.characters;
using ZP2K9.map;

namespace ZP2K9.particles.grenades;

public class Mine
{
	public static void Init(Particle p, Vector2 loc, Vector2 traj, int owner)
	{
		p.netSprite = 4;
		p.loc = loc;
		p.traj = traj;
		p.netOwner = owner;
		p.frame = 60f;
		p.dir = Rand.GetRandomFloat(-10f, 10f);
	}

	public static void NetWrite(Particle p, PacketWriter writer)
	{
		NetPacker.WriteByte(writer, p.netSprite);
		NetPacker.WriteByte(writer, p.netOwner);
		NetPacker.WriteVec2(writer, p.loc);
		NetPacker.WriteVec2(writer, p.traj);
		((BinaryWriter)(object)writer).Write(p.ground);
	}

	public static void NetInit(Particle p, PacketReader reader)
	{
		p.netInduced = true;
		p.netOwner = NetPacker.ReadByte(reader);
		p.loc = NetPacker.ReadVec2(reader);
		p.traj = NetPacker.ReadVec2(reader);
		p.ground = ((BinaryReader)(object)reader).ReadBoolean();
		p.frame = 5.1f;
		p.angle = Rand.GetRandomRadian();
		p.dir = Rand.GetRandomRadian();
	}

	public static void Update(Particle p, GameMap map, Character[] c, float fTime)
	{
		float frame = p.frame;
		p.frame -= fTime;
		if ((int)(p.frame / 2f) != (int)(frame / 2f))
		{
			p.netSprite = 4;
		}
		if (!p.ground)
		{
			Vector2 loc = p.loc;
			p.loc += p.traj * fTime;
			p.traj.Y += fTime * Game1.gravity;
			if (map.GetIsCol(p.loc))
			{
				p.ground = true;
				p.loc = loc;
			}
			p.angle += p.dir * fTime;
		}
		else if (p.frame > 2f && p.frame < 58f)
		{
			for (int i = 0; i < c.Length; i++)
			{
				if (c[i] != null && i != p.netOwner && HitManager.GetHostile(i, p.netOwner) && (p.loc - (c[i].loc - new Vector2(0f, 32f))).Length() < 100f)
				{
					p.frame = 2f;
				}
			}
		}
		if (p.frame < 3f && p.netInduced && p.frame < 3f)
		{
			p.frame = -1f;
			p.exists = false;
		}
		else if (p.frame < 1f)
		{
			p.frame = -1f;
			int damage = 300;
			float range = 200f;
			if (Game1.character[p.netOwner] != null && Game1.character[p.netOwner].perk[1] == 7)
			{
				damage = 400;
				range = 250f;
			}
			Game1.pMan.Explode(p.loc, p.netOwner, damage, range);
		}
	}

	public static void Draw(Particle p, SpriteBatch sprite)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc), (Rectangle?)new Rectangle(256, 448, 64, 64), Color.White, p.angle, new Vector2(32f, 32f), Scroll.zoom * 0.5f, (SpriteEffects)0, 1f);
		if (p.frame < 2f && (int)(p.frame * 20f) % 2 == 0)
		{
			sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc), (Rectangle?)new Rectangle(256, 448, 64, 64), new Color(new Vector4(1f, 0f, 0f, 1f)), p.angle, new Vector2(32f, 32f), Scroll.zoom * 0.55f, (SpriteEffects)0, 1f);
		}
		if (!(p.frame < 58f) || (int)(p.frame * 20f) % 2 != 0)
		{
			return;
		}
		Color val = default(Color);
		val = new Color(new Vector4(1f, 0f, 1f, 0.2f));
		if (p.netOwner >= 0 && p.netOwner < Game1.character.Length && Game1.character[p.netOwner] != null)
		{
			switch (Game1.character[p.netOwner].GetTeam())
			{
			case 1:
				val = new Color(new Vector4(0f, 0f, 1f, 0.2f));
				break;
			case 2:
				val = new Color(new Vector4(1f, 0f, 1f, 0.2f));
				break;
			}
		}
		sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc), (Rectangle?)new Rectangle(0, 0, 64, 64), val, p.angle, new Vector2(32f, 32f), Scroll.zoom * Rand.GetRandomFloat(0.5f, 1.5f), (SpriteEffects)0, 1f);
	}
}
