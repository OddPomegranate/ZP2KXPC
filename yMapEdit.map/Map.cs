using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using yMapEdit.segdef;
using ZP2K9;
using ZP2K9.map;

namespace yMapEdit.map;

public class Map
{
	public const int ICELL = 64;

	public const float FCELL = 64f;

	public const byte COLLISION_NONE = 0;

	public const byte COLLISION_FULL = 1;

	public const byte COLLISION_RISE = 2;

	public const byte COLLISION_FALL = 3;

	public const byte COLLISION_WATER = 4;

	public const byte COLLISION_WATER_FALL = 5;

	public const byte COLLISION_WATER_RISE = 6;

	public const byte COLLISION_LEDGE = 7;

	public const byte COLLISION_X = 8;

	public string path = "";

	public string[] script;

	public Layer[] layer;

	private SegDefManager segDefMgr;

	public int xSize = 20;

	public int ySize = 20;

	public int mapWidth = 2;

	public int mapHeight = 2;

	public Special[] special;

	public int bg;

	public float bgR = 1f;

	public float bgG = 1f;

	public float bgB = 1f;

	public byte[,] collision;

	public float delta;

	public void CalculateSegRects()
	{
		for (int i = 0; i < 5; i++)
		{
			for (int j = 0; j < layer[i].segment.Length; j++)
			{
				if (layer[i].segment[j] != null)
				{
					layer[i].segment[j].CalculateRect(segDefMgr.segDef[layer[i].segment[j].idx]);
				}
			}
		}
	}

	public void Update()
	{
		delta += Game1.frameTime;
		if (delta > 6.28f)
		{
			delta -= 6.28f;
		}
	}

	public void SetWidth(int w)
	{
		mapWidth = w;
		xSize = mapWidth * 10;
	}

	public string GetName()
	{
		string[] array = path.Split('\\');
		string text = array[array.Length - 1];
		return text.Substring(0, text.Length - 4);
	}

	public void SetHeight(int h)
	{
		mapHeight = h;
		ySize = mapHeight * 10;
	}

	public byte GetCol(Vector2 v)
	{
		int num = (int)(v.X / 64f);
		int num2 = (int)(v.Y / 32f);
		if (num >= 0 && num2 >= 0 && num < xSize && (float)num2 < (float)ySize * 2f)
		{
			return collision[num, num2];
		}
		return 0;
	}

	public bool TestCol(Vector2 v)
	{
		switch (GetCol(v))
		{
		case 0:
			return false;
		case 1:
			return true;
		case 2:
		case 3:
			if (v.Y > GetMinY(v))
			{
				return true;
			}
			return false;
		case 4:
		case 5:
			if (v.Y < GetMaxY(v))
			{
				return true;
			}
			return false;
		default:
			return false;
		}
	}

	public float GetMinY(Vector2 v)
	{
		int num = (int)(v.X / 64f);
		int num2 = (int)(v.Y / 32f);
		float num3 = (v.X - (float)num * 64f) / 64f;
		float num4 = (v.Y - (float)num2 * 32f) / 32f;
		if (num >= 0 && num2 > 0 && num < xSize && (float)num2 < (float)ySize * 2f && collision[num, num2] == 1 && (collision[num, num2 - 1] == 2 || collision[num, num2 - 1] == 3) && num4 < 10f)
		{
			num2--;
			num4++;
		}
		float num5 = num4;
		if (num >= 0 && num2 >= 0 && num < xSize && (float)num2 < (float)ySize * 2f)
		{
			switch (collision[num, num2])
			{
			case 1:
				num5 = 0f;
				break;
			case 2:
				if (num4 >= 1f - num3)
				{
					num5 = 1f - num3;
				}
				break;
			case 3:
				if (num4 >= num3)
				{
					num5 = num3;
				}
				break;
			case 4:
				if (num4 < 1f - num3)
				{
					num5 = 0f;
				}
				break;
			case 5:
				if (num4 < num3)
				{
					num5 = 0f;
				}
				break;
			}
		}
		return (float)num2 * 32f + num5 * 32f;
	}

	public float GetMaxY(Vector2 v)
	{
		int num = (int)(v.X / 64f);
		int num2 = (int)(v.Y / 32f);
		float num3 = (v.X - (float)num * 64f) / 64f;
		float num4 = (v.Y - (float)num2 * 32f) / 32f;
		float num5 = 1f;
		if (num >= 0 && num2 >= 0 && num < xSize && (float)num2 < (float)ySize * 2f)
		{
			switch (collision[num, num2])
			{
			case 1:
				num5 = 1f;
				break;
			case 2:
				if (num4 > 1f - num3)
				{
					num5 = 1f;
				}
				break;
			case 3:
				if (num4 > num3)
				{
					num5 = 1f;
				}
				break;
			case 4:
				if (num4 < 1f - num3)
				{
					num5 = 1f - num3;
				}
				break;
			case 5:
				if (num4 < num3)
				{
					num5 = num3;
				}
				break;
			}
		}
		return (float)num2 * 32f + num5 * 32f;
	}

	public void Reset()
	{
		path = "";
		layer = new Layer[5];
		for (int i = 0; i < layer.Length; i++)
		{
			layer[i] = new Layer((Layer.Level)i);
		}
		collision = new byte[256, 256];
		layer[0].zoom = 0.6f;
		layer[1].zoom = 0.75f;
		layer[2].zoom = 1f;
		layer[3].zoom = 1f;
		layer[4].zoom = 1.25f;
		script = new string[1024];
		for (int j = 0; j < script.Length; j++)
		{
			script[j] = "";
		}
		xSize = 10;
		ySize = 10;
	}

	public Map(SegDefManager segDefMgr)
	{
		special = new Special[128];
		for (int i = 0; i < special.Length; i++)
		{
			special[i] = new Special();
		}
		layer = new Layer[5];
		for (int j = 0; j < layer.Length; j++)
		{
			layer[j] = new Layer((Layer.Level)j);
		}
		collision = new byte[256, 256];
		layer[0].zoom = 0.6f;
		layer[1].zoom = 0.75f;
		layer[2].zoom = 1f;
		layer[3].zoom = 1f;
		layer[4].zoom = 1.25f;
		script = new string[1024];
		for (int k = 0; k < script.Length; k++)
		{
			script[k] = "";
		}
		this.segDefMgr = segDefMgr;
	}

	public void UpdateLayerZoom()
	{
		for (int i = 0; i < layer.Length; i++)
		{
			layer[i].adjustedZoom = layer[i].zoom;
		}
	}

	public void DrawCol(SpriteBatch sprite, Texture2D colTex)
	{
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		Vector2 realLoc = ScrollManager.GetRealLoc(default(Vector2), 1f);
		Vector2 realLoc2 = ScrollManager.GetRealLoc(new Vector2(1280f, 720f), 1f);
		for (int i = (int)(realLoc.X / 64f); (float)i < realLoc2.X / 64f; i++)
		{
			for (int j = (int)(realLoc.Y * 2f / 64f); (float)j < realLoc2.Y * 2f / 64f; j++)
			{
				if (i >= 0 && j >= 0 && i < 256 && j < 256 && collision[i, j] != 0)
				{
					Vector2 screenLoc = ScrollManager.GetScreenLoc(new Vector2((float)i * 64f, (float)j * 64f * 0.5f), layer[2].adjustedZoom);
					Rectangle value = default(Rectangle);
					switch (collision[i, j])
					{
					case 1:
						value.X = 0;
						break;
					case 2:
						value.X = 128;
						break;
					case 3:
						value.X = 64;
						break;
					case 4:
						value.X = 192;
						break;
					case 5:
						value.X = 256;
						break;
					case 6:
						value.X = 320;
						break;
					case 8:
						value.X = 448;
						break;
					case 7:
						value.X = 0;
						value.Y = 64;
						break;
					}
					value.Width = 64;
					value.Height = 64;
					sprite.Draw(colTex, screenLoc, (Rectangle?)value, new Color(new Vector4(1f, 1f, 1f, 0.3f)), 0f, default(Vector2), new Vector2(ScrollManager.zoom * layer[2].adjustedZoom, ScrollManager.zoom * layer[2].adjustedZoom * 0.5f), (SpriteEffects)0, 1f);
				}
			}
		}
	}

	public void DrawEntities(SpriteBatch sprite, Texture2D spritesTex)
	{
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < special.Length; i++)
		{
			if (special[i].exists)
			{
				sprite.Draw(spritesTex, ScrollManager.GetScreenLoc(new Vector2((float)special[i].x * 64f, (float)special[i].y * 32f), 1f), (Rectangle?)new Rectangle(96, 928, 4, 4), new Color(1f, 1f, 1f, 0.5f), 0f, new Vector2(0f, 0f), ScrollManager.zoom * new Vector2(64f, 32f) / 4f, (SpriteEffects)0, 1f);
				sprite.Draw(spritesTex, ScrollManager.GetScreenLoc(new Vector2((float)special[i].x * 64f, (float)special[i].y * 32f), 1f), (Rectangle?)new Rectangle(special[i].type % 16 * 64, special[i].type / 16 * 64 + 320, 64, 64), Color.White, 0f, new Vector2(0f, 16f), ScrollManager.zoom, (SpriteEffects)0, 1f);
			}
		}
	}

	private void DrawBGTex(SpriteBatch sprite, Texture2D[] bgTex, int idx, float depth, float scale, float bgscale)
	{
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		Vector2 vector = ScrollManager.GetScreenLoc(new Vector2(8192f, 4096f), depth) * scale;
		Vector2 vector2 = new Vector2(1280f, 720f) * scale;
		if (bgscale < 2f)
		{
			bgscale *= 2f / bgscale;
		}
		Vector2 vector3 = vector - new Vector2(320f, 180f) * bgscale * scale;
		Vector2 vector4 = vector + new Vector2(320f, 180f) * bgscale * scale;
		if (vector3.X > 0f)
		{
			vector.X -= vector3.X;
		}
		if (vector3.Y > 0f)
		{
			vector.Y -= vector3.Y;
		}
		if (vector4.X < vector2.X)
		{
			vector.X -= vector4.X - vector2.X;
		}
		if (vector4.Y < vector2.Y)
		{
			vector.Y -= vector4.Y - vector2.Y;
		}
		sprite.Draw(bgTex[idx], vector, (Rectangle?)new Rectangle(0, 0, 640, 360), new Color(bgR, bgG, bgB, 1f), 0f, new Vector2(320f, 180f), bgscale * scale, (SpriteEffects)0, 1f);
	}

	public void Draw(SpriteBatch sprite, int s, int e, Texture2D nullTex, Texture2D[] mapTex, Texture2D[] bgTex, float alpha, float scale)
	{
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b68: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b6a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bf3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c19: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c1b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d0e: Unknown result type (might be due to invalid IL or missing references)
		//IL_118a: Unknown result type (might be due to invalid IL or missing references)
		//IL_112b: Unknown result type (might be due to invalid IL or missing references)
		if (s == 0)
		{
			switch (bg)
			{
			case 0:
			{
				DrawBGTex(sprite, bgTex, 0, 0.03f, scale, 3f * (1f + ScrollManager.zoom) * 0.5f);
				float y = ScrollManager.GetScreenLoc(new Vector2(8192f, 4096f), 0.2f).Y;
				sprite.Draw(nullTex, new Vector2(0f, y) * scale, (Rectangle?)new Rectangle(0, 0, 1, 1), new Color(0f, 0f, 0f, 1f), 0f, default(Vector2), new Vector2(1280f, 720f - y) * scale, (SpriteEffects)0, 1f);
				sprite.Draw(bgTex[1], new Vector2(ScrollManager.GetScreenLoc(new Vector2(8192f, 4096f), 0.07f).X, y) * scale, (Rectangle?)new Rectangle(0, 0, 1280, 100), Color.White, 0f, new Vector2(640f, 100f), 1.5f * scale, (SpriteEffects)0, 1f);
				break;
			}
			case 1:
				DrawBGTex(sprite, bgTex, 2, 0.1f, scale, 3f * (1f + ScrollManager.zoom) * 0.5f);
				DrawBGTex(sprite, bgTex, 3, 0.3f, scale, 3f * (1f + ScrollManager.zoom) * 0.8f);
				break;
			case 2:
				DrawBGTex(sprite, bgTex, 4, 0.03f, scale, 3f * (1f + ScrollManager.zoom) * 0.5f);
				DrawBGTex(sprite, bgTex, 5, 0.1f, scale, 3f * (1f + ScrollManager.zoom) * 0.6f);
				break;
			case 3:
				DrawBGTex(sprite, bgTex, 6, 0.03f, scale, 3f * (1f + ScrollManager.zoom) * 0.5f);
				DrawBGTex(sprite, bgTex, 7, 0.1f, scale, 3f * (1f + ScrollManager.zoom) * 0.6f);
				break;
			case 4:
				DrawBGTex(sprite, bgTex, 8, 0.03f, scale, 3f * (1f + ScrollManager.zoom) * 0.5f);
				DrawBGTex(sprite, bgTex, 9, 0.18f, scale, 3f * (1f + ScrollManager.zoom) * 0.7f);
				break;
			}
		}
		if (e == 5 && alpha >= 1f)
		{
			Vector2 screenLoc = ScrollManager.GetScreenLoc(default(Vector2), layer[2].adjustedZoom);
			Vector2 screenLoc2 = ScrollManager.GetScreenLoc(new Vector2((float)xSize * 64f, (float)ySize * 64f), layer[2].adjustedZoom);
			Vector2 vector = new Vector2(1280f, 720f);
			if (screenLoc.X < 0f)
			{
				screenLoc.X = 0f;
			}
			if (screenLoc.Y < 0f)
			{
				screenLoc.Y = 0f;
			}
			if (screenLoc2.X > vector.X)
			{
				screenLoc2.X = vector.X;
			}
			if (screenLoc2.Y > vector.Y)
			{
				screenLoc2.Y = vector.Y;
			}
		}
		if (s < 0)
		{
			s = 0;
		}
		for (int i = s; i < e; i++)
		{
			if (i == 3)
			{
				DrawWater(sprite, mapTex, nullTex, null);
			}
			Color val = Color.White;
			switch ((Layer.Level)i)
			{
			case Layer.Level.Back2:
				val = Color.DarkGray;
				break;
			case Layer.Level.Back1:
				val = Color.Gray;
				break;
			case Layer.Level.Mid:
				val = Color.White;
				break;
			case Layer.Level.Fore1:
				val = Color.White;
				break;
			case Layer.Level.Fore2:
				val = Color.Black;
				break;
			}
			if (alpha < 1f)
			{
				val.A = (byte)(alpha * 256f);
				val.R = 0;
				val.B = 0;
				if (i != 2)
				{
					val.A = (byte)(val.A / 2);
				}
			}
			float num = layer[i].adjustedZoom;
			for (int j = 0; j < layer[i].segment.Length; j++)
			{
				if (layer[i].segment[j] == null)
				{
					continue;
				}
				Segment segment = layer[i].segment[j];
				if (segment.idx <= -1)
				{
					continue;
				}
				SegDef segDef = segDefMgr.segDef[segment.idx];
				switch (segDef.flags)
				{
				case 2:
					Game1.postGlowMgr.Add(ScrollManager.GetScreenLoc(segment.loc + new Vector2(0f, 100f), num), 0.8f, 0.9f, 1f, 0.3f, 2.4f, ScrollManager.GetScreenLoc(segment.loc, num), 0.5f);
					break;
				case 3:
					Game1.postGlowMgr.Add(ScrollManager.GetScreenLoc(segment.loc, num), 1f, 0.8f, 0.3f, 0.2f, 4f);
					break;
				case 10:
					Game1.postGlowMgr.Add(ScrollManager.GetScreenLoc(segment.loc, num), 0.7f, 1f, 0.7f, 0.2f, 4f);
					break;
				case 11:
					Game1.postGlowMgr.Add(ScrollManager.GetScreenLoc(segment.loc, num), 1f, 1f, 1f, 0.2f, 3f);
					break;
				case 4:
					Game1.postGlowMgr.Add(ScrollManager.GetScreenLoc(segment.loc, num), 1f, 0.8f, 0.2f, Rand.GetRandomFloat(0.18f, 0.2f), 2f, 0f);
					break;
				case 69:
				case 73:
					Game1.postGlowMgr.Add(ScrollManager.GetScreenLoc(segment.loc, num), 1f, 0.5f, 0.4f, (float)Math.Cos(segment.loc.X / 2f + segment.loc.Y / 2f + delta) * 0.1f + 0.1f, 2f);
					break;
				case 72:
				case 76:
					Game1.postGlowMgr.Add(ScrollManager.GetScreenLoc(segment.loc, num), 0.4f, 1f, 0.5f, (float)Math.Cos(segment.loc.X / 2f + segment.loc.Y / 2f + delta) * 0.1f + 0.1f, 2f);
					break;
				case 70:
				case 74:
					Game1.postGlowMgr.Add(ScrollManager.GetScreenLoc(segment.loc, num), 1f, 1f, 0.15f, (float)Math.Cos(segment.loc.X / 2f + segment.loc.Y / 2f + delta) * 0.1f + 0.1f, 2f);
					break;
				case 71:
				case 75:
					Game1.postGlowMgr.Add(ScrollManager.GetScreenLoc(segment.loc, num), 0.4f, 0.5f, 1f, (float)Math.Cos(segment.loc.X / 2f + segment.loc.Y / 2f + delta) * 0.1f + 0.1f, 2f);
					break;
				case 78:
					Game1.postGlowMgr.Add(ScrollManager.GetScreenLoc(segment.loc + new Vector2(0f, -10f), num), 0.7f, 0.9f, 1f, 0.1f, 2f, 1f);
					Game1.postGlowMgr.Add(ScrollManager.GetScreenLoc(segment.loc + new Vector2(0f, 10f), num), 0.7f, 0.9f, 1f, 0.1f, 2f, 1f);
					Game1.postGlowMgr.Add(ScrollManager.GetScreenLoc(segment.loc + new Vector2(-120f, 0f), num), 0.9f, 0.95f, 1f, 0.4f, 2f, 0f);
					break;
				case 79:
					Game1.postGlowMgr.Add(ScrollManager.GetScreenLoc(segment.loc + new Vector2(0f, -10f), num), 0.7f, 0.9f, 1f, 0.1f, 2f, 1f);
					Game1.postGlowMgr.Add(ScrollManager.GetScreenLoc(segment.loc + new Vector2(0f, 10f), num), 0.7f, 0.9f, 1f, 0.1f, 2f, 1f);
					Game1.postGlowMgr.Add(ScrollManager.GetScreenLoc(segment.loc + new Vector2(120f, 0f), num), 0.9f, 0.95f, 1f, 0.4f, 2f, 0f);
					break;
				case 5:
				{
					float num2 = delta + (float)i;
					if (num2 > 6.28f)
					{
						num2 -= 6.28f;
					}
					if ((int)(num2 / 6.28f * 9f) % 3 == j % 3)
					{
						float num3 = num2 / 6.28f * 9f - (float)(int)(num2 / 6.28f * 9f);
						if (num3 > 0.5f)
						{
							num3 = 1f - num3;
						}
						Game1.postGlowMgr.Add(ScrollManager.GetScreenLoc(segment.loc, num), 1f, 0.2f, 0.15f, num3, 1f);
					}
					break;
				}
				}
				bool flag = segment.rect.Width == 0;
				if (!flag)
				{
					Vector2 screenLoc3 = ScrollManager.GetScreenLoc(new Vector2(segment.rect.Left, segment.rect.Top), num);
					Vector2 screenLoc4 = ScrollManager.GetScreenLoc(new Vector2(segment.rect.Right, segment.rect.Bottom), num);
					if (screenLoc3.X < 1280f && screenLoc3.Y < 720f && screenLoc4.X > 0f && screenLoc4.Y > 0f)
					{
						flag = true;
					}
				}
				if (segDef.flags == 8)
				{
					num += 0.05f;
					if (i == 3)
					{
						val.R = 5;
						val.G = 5;
						val.B = 5;
					}
				}
				if (flag)
				{
					switch (segDef.flags)
					{
					case 77:
					{
						Color val2 = val;
						Vector2 screenLoc6 = ScrollManager.GetScreenLoc(segment.loc, num);
						float num7 = (screenLoc6.X + screenLoc6.Y) * 0.003f;
						num7 -= (float)(int)num7;
						if (num7 > 0.5f)
						{
							num7 = 1f - num7;
						}
						num7 = num7 * 0.4f + 0.6f;
						val.A = (byte)((float)(int)val.R * num7);
						sprite.Draw(mapTex[segDef.texIdx], screenLoc6 * scale, (Rectangle?)segDef.sRect, val, segment.rotation, segDef.lockLoc, ScrollManager.zoom * num * scale, (SpriteEffects)0, 0f);
						val = val2;
						flag = false;
						break;
					}
					case 17:
					{
						float num4 = segment.loc.X + segment.loc.Y + delta / 1.57f;
						for (int k = 0; k < 5; k++)
						{
							float num5 = (float)k / 5f + num4;
							num5 -= (float)(int)num5;
							num5 = 1f - num5;
							Vector2 screenLoc5 = ScrollManager.GetScreenLoc(segment.loc + new Vector2((float)Math.Cos(segment.rotation) * num5, (float)Math.Sin(segment.rotation) * num5) * 120f, num);
							float num6 = num5;
							if (num6 > 0.5f)
							{
								num6 = 1f - num6;
							}
							num6 *= 0.7f;
							sprite.Draw(mapTex[segDef.texIdx], screenLoc5 * scale, (Rectangle?)segDef.sRect, new Color(1f, 1f, 1f, num6), segment.rotation, segDef.lockLoc, ScrollManager.zoom * num * scale * (num5 * 0.5f + 0.5f), (SpriteEffects)((k % 2 == 0) ? 256 : 0), 0f);
						}
						flag = false;
						break;
					}
					}
				}
				if (!flag)
				{
					continue;
				}
				Vector2 screenLoc7 = ScrollManager.GetScreenLoc(segment.loc, num);
				float num8 = segment.rotation;
				switch (segDef.flags)
				{
				case 6:
					num8 += (float)Math.Cos(delta * 7f + segment.loc.X + segment.loc.Y) * 0.1f;
					break;
				case 7:
					num8 += (float)Math.Cos(delta * 2f + segment.loc.X + segment.loc.Y) * 0.1f;
					break;
				case 43:
					num8 += (float)Math.Cos(delta * 7f + segment.loc.X + segment.loc.Y + (float)j) * 0.02f;
					break;
				case 46:
					num8 += (float)Math.Cos(delta * 4f + segment.loc.X + segment.loc.Y + (float)j) * 0.02f;
					break;
				case 16:
					num8 -= delta * 5f + segment.loc.X + segment.loc.Y;
					break;
				}
				if (segDef.flags == 9)
				{
					float num9 = delta / 6.28f;
					num9 += (segment.loc.X + segment.loc.Y) / 5f;
					num9 -= (float)(int)num9;
					Vector2 vector2 = new Vector2(50f, 50f);
					if ((int)segment.loc.X % 2 == 0)
					{
						vector2.X = 0f - vector2.X;
					}
					screenLoc7 = ScrollManager.GetScreenLoc(segment.loc - vector2 + vector2 * num9 * 2f, num);
					if (num9 < 0.5f)
					{
						val = new Color(1f, 1f, 1f, num9);
					}
					else
					{
						val = new Color(1f, 1f, 1f, 1f - num9);
					}
				}
				if (segDef.flags == 44 || segDef.flags == 49)
				{
					num8 += delta;
				}
				switch (segDef.flags)
				{
				case 12:
				case 13:
				case 14:
				case 15:
				{
					float num10 = 1f;
					switch (segDef.flags)
					{
					case 12:
						num10 = 0.39f;
						break;
					case 13:
						num10 = 0.57f;
						break;
					case 14:
						num10 = 0.93f;
						break;
					case 15:
						num10 = 1.6f;
						break;
					}
					float num11 = delta / 6.28f + (segment.loc.X + segment.loc.Y) / 3.7f;
					float num12 = num11 - (float)(int)num11;
					float num13 = num12 * 3f;
					int num14 = (int)num13;
					num13 -= (float)(int)num13;
					float num15 = 60f;
					num12 = (float)num14 * num15;
					if (num13 > 0.975f)
					{
						num12 += (num13 - 0.975f) * num15 * 10f * 4f;
					}
					sprite.Draw(mapTex[segDef.texIdx], screenLoc7 * scale, (Rectangle?)new Rectangle(710 + (((int)(num11 * 150f) % 2 == 0) ? 80 : 0), (int)num12, 80, 60), val, num8, default(Vector2), ScrollManager.zoom * num * scale * num10 * new Vector2(1f, 0.9f), (SpriteEffects)0, 0f);
					break;
				}
				}
				Rectangle sRect = segDef.sRect;
				sprite.Draw(mapTex[segDef.texIdx], screenLoc7 * scale, (Rectangle?)sRect, val, num8, segDef.lockLoc, ScrollManager.zoom * num * scale, (SpriteEffects)0, 0f);
			}
		}
	}

	public void DrawWater(SpriteBatch sprite, Texture2D[] mapTex, Texture2D nullTex, MapWater water)
	{
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		Vector2 realLoc = ScrollManager.GetRealLoc(default(Vector2), 1f);
		Vector2 realLoc2 = ScrollManager.GetRealLoc(new Vector2(1280f, 720f), 1f);
		int num = 140;
		if (water != null)
		{
			num = water.waterLevel;
		}
		for (int i = (int)(realLoc.X / 64f); (float)i < realLoc2.X / 64f; i++)
		{
			int num2 = -1;
			for (int j = (int)(realLoc.Y * 2f / 64f) - 1; (float)j < realLoc2.Y * 2f / 64f; j++)
			{
				if (i >= 0 && j > 0 && i < 256 && j < 256 && ((water == null) ? (collision[i, j] == 4 || collision[i, j] == 6 || collision[i, j] == 5 || collision[i, j - 1] == 4 || collision[i, j - 1] == 6 || collision[i, j - 1] == 5) : (water.water[i, j] || water.water[i, j - 1])))
				{
					Vector2 screenLoc = ScrollManager.GetScreenLoc(new Vector2((float)i * 64f, (float)j * 64f * 0.5f), 1f);
					Vector2 screenLoc2 = ScrollManager.GetScreenLoc(new Vector2((float)(i + 1) * 64f, (float)(j + 1) * 64f * 0.5f), 1f);
					float num3 = (float)j - (float)num;
					float num4 = 1f - num3 * 0.03f;
					sprite.Draw(nullTex, new Rectangle((int)screenLoc.X, (int)screenLoc.Y, (int)screenLoc2.X - (int)screenLoc.X, (int)screenLoc2.Y - (int)screenLoc.Y), (Rectangle?)new Rectangle(0, 0, 1, 1), new Color(0.1f * num4, 0.2f * num4, 0.3f * num4, 0.7f), 0f, default(Vector2), (SpriteEffects)0, 1f);
					if (j == num)
					{
						num2 = j;
					}
				}
			}
			if (num2 > -1)
			{
				Vector2 screenLoc3 = ScrollManager.GetScreenLoc(new Vector2((float)i * 64f, (float)num2 * 64f * 0.5f), 1f);
				Vector2 screenLoc4 = ScrollManager.GetScreenLoc(new Vector2((float)(i + 1) * 64f, (float)(num2 + 3) * 64f * 0.5f), 1f);
				int num5 = (int)(delta / 6.28f * 128f);
				sprite.Draw(mapTex[3], new Rectangle((int)screenLoc3.X, (int)screenLoc3.Y, (int)screenLoc4.X - (int)screenLoc3.X, (int)screenLoc4.Y - (int)screenLoc3.Y), (Rectangle?)new Rectangle(961 + num5, 372, 64, 138), new Color(1f, 1f, 1f, 0.5f), 0f, new Vector2(0f, 10f), (SpriteEffects)0, 1f);
				num5 = 128 - (int)(delta / 6.28f * 128f);
				sprite.Draw(mapTex[3], new Rectangle((int)screenLoc3.X, (int)screenLoc3.Y, (int)screenLoc4.X - (int)screenLoc3.X, (int)screenLoc4.Y - (int)screenLoc3.Y), (Rectangle?)new Rectangle(961 + num5, 372, 64, 138), new Color(1f, 1f, 1f, 0.5f), 0f, new Vector2(0f, 10f), (SpriteEffects)0, 1f);
			}
		}
	}

	public void Write()
	{
		BinaryWriter binaryWriter = new BinaryWriter(File.Open(path, FileMode.OpenOrCreate, FileAccess.Write));
		for (int i = 0; i < layer.Length; i++)
		{
			for (int j = 0; j < layer[i].segment.Length; j++)
			{
				if (layer[i].segment[j] != null)
				{
					binaryWriter.Write(value: true);
					binaryWriter.Write(layer[i].segment[j].idx);
					binaryWriter.Write(layer[i].segment[j].loc.X);
					binaryWriter.Write(layer[i].segment[j].loc.Y);
					binaryWriter.Write(layer[i].segment[j].rotation);
				}
				else
				{
					binaryWriter.Write(value: false);
				}
			}
		}
		for (int k = 0; k < 256; k++)
		{
			for (int l = 0; l < 256; l++)
			{
				binaryWriter.Write(collision[k, l]);
			}
		}
		for (int m = 0; m < special.Length; m++)
		{
			binaryWriter.Write(special[m].type);
			binaryWriter.Write(special[m].x);
			binaryWriter.Write(special[m].y);
			binaryWriter.Write(special[m].exists);
		}
		for (int n = 0; n < script.Length; n++)
		{
			binaryWriter.Write(script[n]);
		}
		binaryWriter.Close();
	}

	public void Read(string path)
	{
		this.path = path;
		Read();
	}

	public void Read()
	{
		BinaryReader binaryReader = new BinaryReader(File.Open(path, FileMode.Open, FileAccess.Read));
		Read(binaryReader);
		binaryReader.Close();
	}

	internal void Read(BinaryReader reader)
	{
		for (int i = 0; i < layer.Length; i++)
		{
			for (int j = 0; j < layer[i].segment.Length; j++)
			{
				if (reader.ReadBoolean())
				{
					if (layer[i].segment[j] == null)
					{
						layer[i].segment[j] = new Segment();
					}
					layer[i].segment[j].idx = reader.ReadInt32();
					layer[i].segment[j].loc.X = reader.ReadSingle();
					layer[i].segment[j].loc.Y = reader.ReadSingle();
					layer[i].segment[j].rotation = reader.ReadSingle();
				}
				else if (layer[i].segment[j] != null)
				{
					layer[i].segment[j] = null;
				}
			}
		}
		for (int k = 0; k < 256; k++)
		{
			for (int l = 0; l < 256; l++)
			{
				collision[k, l] = reader.ReadByte();
			}
		}
		for (int m = 0; m < special.Length; m++)
		{
			special[m].type = reader.ReadByte();
			special[m].x = reader.ReadInt32();
			special[m].y = reader.ReadInt32();
			special[m].exists = reader.ReadBoolean();
		}
		for (int n = 0; n < script.Length; n++)
		{
			script[n] = reader.ReadString();
		}
	}
}
