using System.Collections.Generic;
using System.IO;

namespace SceneEdit.scene;

public class Video
{
	public List<Scene> scenes;

	public float duration;

	public float time;

	public bool playing;

	public Video()
	{
		scenes = new List<Scene>();
	}

	internal void Write(BinaryWriter writer)
	{
		writer.Write(duration);
		writer.Write(scenes.Count);
		for (int i = 0; i < scenes.Count; i++)
		{
			scenes[i].Write(writer);
		}
	}

	internal void Read(BinaryReader reader)
	{
		duration = reader.ReadSingle();
		int num = reader.ReadInt32();
		scenes = new List<Scene>();
		for (int i = 0; i < num; i++)
		{
			Scene scene = new Scene();
			scene.Read(reader);
			scenes.Add(scene);
		}
	}

	internal void Append(BinaryReader reader)
	{
		reader.ReadSingle();
		int num = reader.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			Scene scene = new Scene();
			scene.Read(reader);
			scenes.Add(scene);
		}
	}
}
