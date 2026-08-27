namespace SceneEdit.scene;

internal class SceneMaster
{
	internal static void Update(Video video, Scene scene)
	{
		scene.r = (scene.g = (scene.b = 1f));
		for (int i = 0; i < scene.layer.Count; i++)
		{
			Layer layer = scene.layer[i];
			if (layer == null || !(layer.name == "master") || layer.keyframe.Count <= 0)
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
			scene.r = keyframe2.r;
			scene.g = keyframe2.g;
			scene.b = keyframe2.b;
			_ = keyframe2.tween;
			if (num < layer.keyframe.Count - 1)
			{
				Keyframe keyframe3 = layer.keyframe[num + 1];
				float num3 = (video.time - keyframe2.time) / (keyframe3.time - keyframe2.time);
				scene.r = keyframe2.r + (keyframe3.r - keyframe2.r) * num3;
				scene.g = keyframe2.g + (keyframe3.g - keyframe2.g) * num3;
				scene.b = keyframe2.b + (keyframe3.b - keyframe2.b) * num3;
			}
			break;
		}
	}
}
