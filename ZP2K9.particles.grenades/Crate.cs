using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ZP2K9.net;
using ZP2K9.characters;
using ZP2K9.map;

namespace ZP2K9.particles.grenades;

public class Crate
{
	public static void Init(Particle p, Vector2 loc, Vector2 traj, int owner)
	{
		p.netSprite = 24;
		p.loc = loc;
		p.netOwner = owner;
		p.frame = 120f;
		p.size = 1f;
	}

	public static void NetWrite(Particle p, PacketWriter writer)
	{
		NetPacker.WriteByte(writer, p.netSprite);
		NetPacker.WriteByte(writer, p.netOwner);
		NetPacker.WriteVec2(writer, p.loc);
	}

	public static void NetInit(Particle p, PacketReader reader)
	{
		p.netInduced = true;
		p.netOwner = NetPacker.ReadByte(reader);
		p.loc = NetPacker.ReadVec2(reader);
		p.frame = 120f;
		p.size = 1f;
	}

	public static void Update(Particle p, GameMap map, Character[] c, float fTime)
	{
		Vector2 loc = p.loc;
		p.loc += fTime * p.traj;
		if (p.ground)
		{
			p.traj = default(Vector2);
			if (p.size > 0f)
			{
				p.size -= fTime;
			}
		}
		else
		{
			p.traj = new Vector2(0f, 100f);
			if (map.GetIsCol(p.loc))
			{
				p.loc = loc;
				p.ground = true;
				p.traj = default(Vector2);
			}
		}
		for (int i = 0; i < c.Length; i++)
		{
			if (c[i] == null || c[i].hp < 0)
			{
				continue;
			}
			bool flag = true;
			if (GameState.gameType == 4 && Game1.character[i].team == 1)
			{
				flag = false;
			}
			if (!((c[i].loc - new Vector2(0f, 32f) - p.loc).LengthSquared() < 2400f) || !flag)
			{
				continue;
			}
			if (c[i].charKeys.KeyPickup())
			{
				p.frame = -1f;
				c[i].GiveGoodies();
				if ((p.loc - Scroll.scroll).Length() < 700f)
				{
					Sound.PlayCue("suit");
				}
				for (int j = 0; j < 32; j++)
				{
					Game1.pMan.AddParticle(38, c[i].loc + Rand.GetRandomVec2(-32f, 32f, -90f, 0f), new Vector2(0f, -30f), Rand.GetRandomFloat(0.2f, 0.5f), 0, 0);
				}
			}
			else if (Game1.netSession.GetPlayerOne() == i)
			{
				Game1.hud.AddPickup(0, 4);
			}
		}
		p.frame -= fTime;
	}

	public static void Draw(Particle p, SpriteBatch sprite)
	{
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		int playerOne = Game1.netSession.GetPlayerOne();
		if (GameState.gameType != 4 || playerOne <= -1 || Game1.character[playerOne] == null || Game1.character[playerOne].team != 1)
		{
			if (p.size > 0f)
			{
				sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc - new Vector2(0f, 36f)), (Rectangle?)new Rectangle(832, 0, 96, 96), new Color(new Vector4(1f, 1f, 1f, p.size)), (float)Math.Cos(p.frame * 4f) * 0.1f, new Vector2(48f, 62f), Scroll.zoom * new Vector2(1f, p.size) * 0.55f, (SpriteEffects)0, 1f);
			}
			sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc), (Rectangle?)new Rectangle(762, 32, 64, 64), new Color(new Vector4(1f, 1f, 1f, p.frame)), 0f, new Vector2(32f, 58f), Scroll.zoom * 0.55f, (SpriteEffects)0, 1f);
		}
	}
}
