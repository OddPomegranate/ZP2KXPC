using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ZP2K9;

namespace SceneEdit.scene;

public class SceneMgr
{
	public Dictionary<string, Texture2D> texture;

	public Video video;

	public int curScene;

	public int selLayer;

	public int selBubble;

	public int selKeyframe;

	public string path;

	private EffectPass pass;

	private bool flicker;

	private bool spazz;

	private bool strobe;

	private bool creep;

	public bool smoothcam;

	public bool miniAdjust;

	private Vector3 camLoc;

	private Vector2 camAngle;

	private bool hasMask;

	public SceneMgr(ContentManager Content)
	{
		video = new Video();
		texture = new Dictionary<string, Texture2D>();
		DirectoryInfo directoryInfo = new DirectoryInfo("Content/gfx/scene/");
		FileInfo[] files = directoryInfo.GetFiles("*.xnb");
		FileInfo[] array = files;
		foreach (FileInfo fileInfo in array)
		{
			string key = fileInfo.Name.Substring(0, fileInfo.Name.Length - 4);
			texture.Add(key, Content.Load<Texture2D>(fileInfo.FullName.Substring(0, fileInfo.FullName.Length - 4)));
		}
		SceneCam.location.Z = 1f;
	}

	private void DrawLayers(bool mask, SpriteBatch sprite, Scene scene)
	{
		//IL_05d0: Unknown result type (might be due to invalid IL or missing references)
		Vector3 location = SceneCam.location;
		foreach (Layer item in scene.layer)
		{
			if (item.keyframe.Count <= 0)
			{
				continue;
			}
			switch (item.name)
			{
			case "cam":
			case "master":
				continue;
			}
			if ((!mask || !(item.name == "mask")) && (mask || !(item.name != "mask")))
			{
				continue;
			}
			if (mask)
			{
				hasMask = true;
			}
			int num = 0;
			float num2 = 0f;
			for (int i = 0; i < item.keyframe.Count; i++)
			{
				Keyframe keyframe = item.keyframe[i];
				if (keyframe.time <= video.time && keyframe.time > num2)
				{
					num = i;
					num2 = keyframe.time;
				}
			}
			Keyframe keyframe2 = item.keyframe[num];
			if (keyframe2.texture == null)
			{
				continue;
			}
			Vector3 loc = keyframe2.loc;
			if (mask)
			{
				loc.Z = 0.1f;
				if (miniAdjust)
				{
					loc.Z = 0.01f;
				}
			}
			Vector2 vector = keyframe2.scale;
			float num3 = keyframe2.r;
			float num4 = keyframe2.g;
			float num5 = keyframe2.b;
			float num6 = keyframe2.a;
			float num7 = keyframe2.angle;
			if (keyframe2.tween && num < item.keyframe.Count - 1)
			{
				Keyframe keyframe3 = item.keyframe[num + 1];
				Vector3 loc2 = keyframe3.loc;
				if (mask)
				{
					loc.Z = 0.1f;
				}
				float num8 = (video.time - keyframe2.time) / (keyframe3.time - keyframe2.time);
				loc += (loc2 - loc) * num8;
				vector = keyframe2.scale + (keyframe3.scale - keyframe2.scale) * num8;
				num3 = keyframe2.r + (keyframe3.r - keyframe2.r) * num8;
				num4 = keyframe2.g + (keyframe3.g - keyframe2.g) * num8;
				num5 = keyframe2.b + (keyframe3.b - keyframe2.b) * num8;
				num6 = keyframe2.a + (keyframe3.a - keyframe2.a) * num8;
				num7 = keyframe2.angle + (keyframe3.angle - keyframe2.angle) * num8;
			}
			num3 *= scene.r;
			num4 *= scene.g;
			num5 *= scene.b;
			// The original Xbox 360 scene.fx post-process (its compiled bytecode is
			// not portable to PC and could not be recovered) evidently compressed
			// bright highlights, because these large ambient "glow"/"glare" sprites
			// read as a soft, subtle haze on the original hardware. Our best-effort
			// replacement shader is a plain color grade, not a highlight
			// compressor, and by the time these layers reach it they're already
			// baked into an 8-bit render target - once several overlapping
			// semi-transparent white sprites blend up to solid white there is no
			// getting that detail back in a later post-process pass. So instead we
			// tame these specific decorative layers at the source: a lower alpha
			// and a slightly smaller footprint keeps the intended soft ambient
			// glow look instead of a blown-out white blob. Tune GLOW_ALPHA_SCALE /
			// GLOW_SIZE_SCALE below if it still needs adjusting.
			if (keyframe2.texture == "glow" || keyframe2.texture == "glare")
			{
				const float GLOW_ALPHA_SCALE = 0.2f;
				const float GLOW_SIZE_SCALE = 0.65f;
				num6 *= GLOW_ALPHA_SCALE;
				vector *= GLOW_SIZE_SCALE;
			}
			if (item.name.Length > 4)
			{
				try
				{
					if (item.name.Substring(0, 4) == "rot-")
					{
						loc.X += (float)Math.Cos(video.time * 3.14f + keyframe2.loc.X + keyframe2.loc.Y) * 20f;
						loc.Y += (float)Math.Sin(video.time * 3.14f + keyframe2.loc.X + keyframe2.loc.Y) * 20f;
					}
				}
				catch
				{
				}
			}
			Vector2 screenLoc = SceneCam.GetScreenLoc(loc);
			Vector2 vector2 = screenLoc - new Vector2(640f, 360f);
			num7 += SceneCam.rotation;
			screenLoc = new Vector2(640f, 360f) + new Vector2((float)Math.Cos(SceneCam.rotation) * vector2.X, (float)Math.Sin(SceneCam.rotation) * vector2.X) + new Vector2((float)Math.Cos(SceneCam.rotation + 1.57f) * vector2.Y, (float)Math.Sin(SceneCam.rotation + 1.57f) * vector2.Y);
			vector *= loc.Z;
			try
			{
				if (flicker)
				{
					float randomFloat = Rand.GetRandomFloat(0.5f, 1f);
					num3 *= randomFloat;
					num4 *= randomFloat;
					num5 *= randomFloat;
				}
				if (spazz)
				{
					screenLoc += Rand.GetRandomVec2(-10f, 10f, -10f, 10f);
				}
				if (item.name == "zap")
				{
					screenLoc += Rand.GetRandomVec2(-10f, 10f, -10f, 10f);
					num7 += Rand.GetRandomFloat(0f, 6.28f);
				}
				if (item.name == "mask" && miniAdjust)
				{
					vector *= 1f + SceneCam.location.Z * 0.02f;
					num7 *= 0.3f;
				}
				else
				{
					vector *= SceneCam.location.Z;
				}
				if (!video.playing && scene.name != video.scenes[curScene].name)
				{
					num6 /= 10f;
				}
				sprite.Draw(texture[keyframe2.texture], screenLoc, (Rectangle?)new Rectangle(0, 0, texture[keyframe2.texture].Width, texture[keyframe2.texture].Height), new Color(num3, num4, num5, num6), num7, new Vector2((float)texture[keyframe2.texture].Width / 2f, (float)texture[keyframe2.texture].Height / 2f), (vector.X < 0f) ? new Vector2(0f - vector.X, vector.Y) : vector, ((vector.X < 0f) ? SpriteEffects.FlipHorizontally : SpriteEffects.None), 1f);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}
		SceneCam.location = location;
	}

	public void Draw(SpriteBatch sprite)
	{
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		if (video.scenes.Count > 0)
		{
			Scene scene = video.scenes[curScene];
			hasMask = false;
			DrawLayers(mask: false, sprite, scene);
			if (strobe && (int)(video.time * 20f) % 2 == 0)
			{
				sprite.Draw(Game1.nullTex, new Rectangle(0, 0, 1280, 720), new Color(1f, 1f, 1f, 0.8f));
			}
			if (creep)
			{
				sprite.Draw(Game1.nullTex, new Rectangle(0, 0, 1280, 720), new Color(1f, 1f, 1f, Rand.GetRandomFloat(0f, 0.2f)));
			}
			flicker = false;
			spazz = false;
			strobe = false;
			creep = false;
			smoothcam = true;
			miniAdjust = false;
		}
	}

	internal void Read(string path)
	{
		this.path = path;
		BinaryReader binaryReader = new BinaryReader(File.Open(path, FileMode.Open, FileAccess.Read));
		video.Read(binaryReader);
		binaryReader.Close();
	}

	internal void Append(string path)
	{
		this.path = path;
		BinaryReader binaryReader = new BinaryReader(File.Open(path, FileMode.Open, FileAccess.Read));
		video.Append(binaryReader);
		binaryReader.Close();
	}

	internal void Write(string path)
	{
		this.path = path;
		Write();
	}

	internal void Write()
	{
		BinaryWriter binaryWriter = new BinaryWriter(File.Open(path, FileMode.OpenOrCreate, FileAccess.Write));
		video.Write(binaryWriter);
		binaryWriter.Close();
	}

	internal void Update()
	{
		video.playing = true;
		if (video.scenes.Count > 0)
		{
			SceneCam.Update(video, video.scenes[curScene], video.playing && smoothcam, Game1.frameTime);
			SceneMaster.Update(video, video.scenes[curScene]);
			if (!video.playing)
			{
				foreach (Scene scene in video.scenes)
				{
					scene.r = (scene.g = (scene.b = 1f));
				}
			}
		}
		if (!video.playing)
		{
			return;
		}
		video.time += Game1.frameTime;
		if (video.time > video.scenes[curScene].duration)
		{
			curScene++;
			selLayer = 0;
			selBubble = 0;
			selKeyframe = 0;
			if (curScene >= video.scenes.Count)
			{
				curScene = 0;
			}
			video.time = 0f;
		}
	}
}
