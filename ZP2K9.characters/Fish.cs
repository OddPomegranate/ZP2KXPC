using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using xCharEdit.Character;
using yMapEdit.map;
using ZP2K9.particles;

namespace ZP2K9.characters;

public class Fish
{
	public Vector2 loc;

	public Vector2 traj;

	public int frameIdx;

	public int face;

	public int anim;

	public int key;

	public float animFrame;

	public bool exists;

	public void Update(Character c)
	{
		if (c == null)
		{
			return;
		}
		loc += traj * Game1.frameTime;
		traj.Y += Game1.frameTime * 500f;
		if (loc.Y > 8192f)
		{
			exists = false;
		}
		if (anim == 2)
		{
			Game1.pMan.AddParticle(50, loc + Rand.GetRandomVec2(-80f, 80f, -100f, 40f), Rand.GetRandomVec2(0f, 0f, -50f, 0f), 0.1f, 0, 0);
		}
		animFrame += Game1.frameTime * 30f;
		Animation animation = Game1.charDef[2].GetAnimation(anim);
		KeyFrame keyFrame = animation.GetKeyFrame(key);
		if (animFrame > (float)keyFrame.duration)
		{
			animFrame -= keyFrame.duration;
			key++;
			keyFrame = animation.GetKeyFrame(key);
			if (key >= animation.getKeyFrameArray().Length)
			{
				key = 0;
			}
		}
		if (keyFrame.frameRef >= 0)
		{
			return;
		}
		key = 0;
		switch (anim)
		{
		case 1:
		{
			anim = 2;
			Sound.PlayCue("hit1");
			Sound.PlayCue("hit2");
			Sound.PlayCue("hit3");
			if (c.hp >= 0)
			{
				KillManager.DoKill(c.lastHitBy, c.ID, 11);
			}
			c.hp = -50;
			c.StartKill(default(Vector2));
			for (int i = 0; i < 50; i++)
			{
				Game1.pMan.AddParticle(50, loc + Rand.GetRandomVec2(-80f, 80f, -100f, 40f), Rand.GetRandomVec2(0f, 0f, -50f, 0f), 0.1f, 0, 0);
			}
			break;
		}
		case 2:
			anim = 0;
			break;
		case 0:
			break;
		}
	}

	public void Draw(SpriteBatch sprite)
	{
		CharDef charDef = Game1.charDef[2];
		if (charDef.GetAnimation(anim).GetKeyFrame(key).lerp)
		{
			frameIdx = charDef.GetAnimation(anim).GetKeyFrame(key).frameRef;
			if (frameIdx < 0)
			{
				frameIdx = 0;
			}
			int idx = key + 1;
			if (charDef.GetAnimation(anim).GetKeyFrame(idx).duration <= 0)
			{
				idx = 0;
			}
			Draw(sprite, charDef.GetAnimation(anim).GetKeyFrame(idx).frameRef);
		}
		else
		{
			frameIdx = charDef.GetAnimation(anim).GetKeyFrame(key).frameRef;
			if (frameIdx < 0)
			{
				frameIdx = 0;
			}
			Draw(sprite, -1);
		}
	}

	public void Draw(SpriteBatch spriteBatch, int next)
	{
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		Rectangle value = default(Rectangle);
		CharDef charDef = Game1.charDef[2];
		Frame frame = charDef.GetFrame(frameIdx);
		float num = 0.9f * ScrollManager.zoom;
		Vector2 screenLoc = ScrollManager.GetScreenLoc(loc, 1f);
		int num2 = 1 - face;
		float num3 = 1f;
		Vector2 vector = Scroll.GetLoc(loc);
		if (vector.Y > 400f)
		{
			num3 = 1f - (vector.Y - 400f) * 0.01f;
		}
		vector = Scroll.GetLoc(loc + new Vector2(0f, -80f));
		if (vector.Y > 350f)
		{
			float num4 = (vector.Y - 350f) * 0.01f;
			if (num4 > 1f)
			{
				num4 = 1f;
			}
			if (vector.Y > 500f)
			{
				num4 -= (vector.Y - 500f) * 0.04f;
			}
			if (num4 > 0f)
			{
				Game1.postGlowMgr.Add(vector, 1f * num4, 0.6f * num4, 0.4f * num4, 0.1f, 4f, default(Vector2), 0f);
			}
		}
		Color val = default(Color);
		for (int i = 0; i < frame.GetPartArray().Length; i++)
		{
			Part part = frame.GetPart(i);
			if (part.idx <= -1)
			{
				continue;
			}
			float num5 = part.rotation;
			Vector2 vector2 = part.location * num + screenLoc;
			Vector2 vector3 = part.scaling * num;
			bool flag = false;
			if ((num2 == 1 && part.flip == 0) || (num2 == 0 && part.flip == 1))
			{
				flag = true;
			}
			if (num2 == 0)
			{
				num5 = 0f - num5;
				vector2.X -= part.location.X * num * 2f;
			}
			if (next > -1)
			{
				Frame frame2 = charDef.GetFrame(next);
				if (Frame.CanLerp(frame, frame2, i))
				{
					Part part2 = frame2.GetPart(i);
					Animation animation = charDef.GetAnimation(anim);
					KeyFrame keyFrame = animation.GetKeyFrame(key);
					float progress = animFrame / (float)keyFrame.duration;
					Vector2 location = part.location;
					Vector2 location2 = part2.location;
					float num6 = part.rotation;
					float num7 = part2.rotation;
					if (num2 == 0)
					{
						num6 = 0f - num6;
						num7 = 0f - num7;
						location.X -= part.location.X * 2f;
						location2.X -= part2.location.X * 2f;
					}
					vector2 = Frame.LerpLoc(location, location2, progress) * num + screenLoc;
					num5 = Frame.LerpRotation(num6, num7, progress);
					vector3 = Frame.LerpScale(part.scaling, part2.scaling, progress) * num;
				}
			}
			val = new Color(new Vector4(num3, num3, num3, 1f));
			if (part.idx >= 1000)
			{
				continue;
			}
			Texture2D val2;
			switch (part.idx / 64)
			{
			case 0:
				val2 = Game1.charTex[charDef.charIdx].tex;
				value = Game1.charTex[charDef.charIdx].GetRect(part.idx);
				break;
			case 1:
				val2 = Game1.weapTex[charDef.weaponIdx].tex;
				value = Game1.weapTex[charDef.weaponIdx].GetRect(part.idx - 64);
				break;
			case 2:
				val2 = Game1.pteroTex[1].tex;
				value = Game1.pteroTex[1].GetRect(part.idx - 128);
				break;
			default:
				val2 = null;
				break;
			}
			if (val2 != null)
			{
				spriteBatch.Draw(val2, vector2, (Rectangle?)value, val, num5, new Vector2((float)value.Width / 2f, (float)value.Height / 2f), vector3, (!flag ? SpriteEffects.FlipHorizontally : SpriteEffects.None), 1f);
			}
			if (part.idx % 64 == 4)
			{
				float num8 = 1f;
				if (vector2.Y > 500f)
				{
					num8 -= (vector2.Y - 500f) * 0.04f;
				}
				if (num8 > 0f)
				{
					Game1.postGlowMgr.Add(vector2, 1f, 1f, 1f, 0.2f * num8, 1f);
				}
			}
		}
	}
}
