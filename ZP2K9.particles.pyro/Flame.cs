using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ZP2K9.net;
using Yuki_Win;
using ZP2K9.characters;
using ZP2K9.map;

namespace ZP2K9.particles.pyro;

public class Flame
{
	public static void Init(Particle p, Vector2 loc, Vector2 traj, int owner)
	{
		p.netSprite = 21;
		p.loc = loc;
		p.traj = traj;
		p.frame = 0.45f;
		p.netOwner = owner;
		p.size = Rand.GetRandomFloat(6f, 8f);
		p.dir = 0.6f;
		p.netWeak = true;
	}

	public static void NetWrite(Particle p, PacketWriter writer)
	{
		NetPacker.WriteByte(writer, p.netSprite);
		NetPacker.WriteByte(writer, p.netOwner);
		NetPacker.WriteVec2(writer, p.loc);
		NetPacker.WriteVec2(writer, p.traj);
		((BinaryWriter)(object)writer).Write(NetPacker.SmallFloatToByte(p.frame));
	}

	public static void NetInit(Particle p, PacketReader reader)
	{
		p.netInduced = true;
		p.netOwner = NetPacker.ReadByte(reader);
		p.loc = NetPacker.ReadVec2(reader);
		p.traj = NetPacker.ReadVec2(reader);
		p.frame = NetPacker.ByteToSmallFloat(((BinaryReader)(object)reader).ReadByte());
		p.size = Rand.GetRandomFloat(6f, 8f);
		p.dir = 0.6f;
	}

	public static void Update(Particle p, GameMap map, Character[] c, float fTime)
	{
		if (p.frame > 0.1f)
		{
			if (HitManager.CheckHit(c, p, map, p.netOwner))
			{
				p.frame = -1f;
			}
			else if (map.GetIsCol(p.loc))
			{
				p.frame = -1f;
				Game1.pMan.AddParticle(15, p.loc - p.traj * 0.02f, default(Vector2), 1f, 0, p.netOwner);
			}
		}
		p.traj.Y += Game1.frameTime * 900f;
		int num = (int)(p.loc.X / 64f);
		int num2 = (int)(p.loc.Y / 32f);
		if (num > 0 && num > 0 && num2 < 256 && num2 < 256 && map.water.water[num, num2])
		{
			p.frame -= fTime * 10f;
		}
		p.BaseUpdate(map, c, fTime);
	}

	public static void Draw(Particle p, SpriteBatch sprite)
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		int num = (int)((0.9f - p.frame * 2f) * 10f);
		if (num <= 8)
		{
			sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc), (Rectangle?)new Rectangle(num * 64, 160, 64, 64), new Color(new Vector4(1f, 1f, 1f, 1f)), Trig.GetAngle(default(Vector2), p.traj), new Vector2(32f, 32f), (0.6f - p.frame) * new Vector2(1f, 0.5f) * Scroll.zoom * p.size, (SpriteEffects)0, 1f);
			sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc), (Rectangle?)new Rectangle(num * 64, 160, 64, 64), new Color(new Vector4(1f, 1f, 1f, 0.2f)), Trig.GetAngle(default(Vector2), p.traj) + Rand.GetRandomFloat(-0.5f, 0.5f), new Vector2(32f, 32f), (0.5f - p.frame) * new Vector2(1f + p.frame, 0.7f) * Scroll.zoom * p.size * 2f, (SpriteEffects)0, 1f);
			float num2 = p.frame * 0.2f;
			if (num2 > 0.1f)
			{
				num2 = 0.1f;
			}
			Game1.postGlowMgr.Add(Scroll.GetLoc(p.loc), 1f, 0.5f, 0.2f, num2, 2f);
		}
	}
}
