using Microsoft.Xna.Framework;

namespace SceneEdit.scene;

public class SceneCam
{
	public static Vector3 location;

	public static float rotation;

	public static Vector2 GetScreenLoc(Vector3 loc)
	{
		return (new Vector2(loc.X, loc.Y) - new Vector2(location.X, location.Y)) * loc.Z * location.Z + new Vector2(640f, 360f);
	}

	internal static void Update(Video video, Scene scene, float frameTime)
	{
		Update(video, scene, smooth: false, frameTime);
	}

	internal static void Update(Video video, Scene scene, bool smooth, float frameTime)
	{
		Update(video, scene, smooth, frameTime, 1f, default(Vector4));
	}

	internal static void Update(Video video, Scene scene, bool smooth, float frameTime, float rotSpeed, Vector4 delta)
	{
		if (!smooth)
		{
			location = new Vector3(0f, 0f, 1f);
			rotation = 0f;
		}
		for (int i = 0; i < scene.layer.Count; i++)
		{
			Layer layer = scene.layer[i];
			if (layer == null || !(layer.name == "cam") || layer.keyframe.Count <= 0)
			{
				continue;
			}
			int num = 0;
			float num2 = 0f;
			for (int j = 0; j < layer.keyframe.Count; j++)
			{
				Keyframe keyframe = layer.keyframe[j];
				if (keyframe.time <= video.time && keyframe.time > num2)
				{
					num = j;
					num2 = keyframe.time;
				}
			}
			Keyframe keyframe2 = layer.keyframe[num];
			Vector3 vector = keyframe2.loc;
			_ = keyframe2.scale;
			_ = keyframe2.r;
			_ = keyframe2.g;
			_ = keyframe2.b;
			_ = keyframe2.a;
			float num3 = keyframe2.angle;
			_ = keyframe2.tween;
			if (num < layer.keyframe.Count - 1)
			{
				Keyframe keyframe3 = layer.keyframe[num + 1];
				float num4 = (video.time - keyframe2.time) / (keyframe3.time - keyframe2.time);
				vector = keyframe2.loc + (keyframe3.loc - keyframe2.loc) * num4;
				num3 = keyframe2.angle + (keyframe3.angle - keyframe2.angle) * num4;
			}
			if (smooth)
			{
				if (rotSpeed < 1f)
				{
					vector += new Vector3(delta.X, delta.Y, delta.Z);
					location += (vector - location) * frameTime;
				}
				else
				{
					location += (vector - location) * frameTime * 2f;
				}
				float num5;
				for (num5 = num3 - rotation; num5 > 3.14f; num5 -= 3.14f)
				{
				}
				for (; num5 < -3.14f; num5 += 3.14f)
				{
				}
				if (rotSpeed < 1f)
				{
					num5 += delta.W;
				}
				rotation += num5 * frameTime * 1f;
			}
			else
			{
				location = vector;
				rotation = num3;
			}
			break;
		}
	}
}
