using System.IO;
using Microsoft.Xna.Framework;

namespace yMapEdit.segdef;

public class SegDefManager
{
	public SegDef[] segDef;

	public SegDefManager()
	{
		segDef = new SegDef[1024];
	}

	public void Read(string path)
	{
		BinaryReader binaryReader = new BinaryReader(File.Open(path, FileMode.Open, FileAccess.Read));
		for (int i = 0; i < segDef.Length; i++)
		{
			if (binaryReader.ReadBoolean())
			{
				if (segDef[i] == null)
				{
					segDef[i] = new SegDef();
				}
				segDef[i].flags = binaryReader.ReadInt32();
				segDef[i].lockLoc.X = binaryReader.ReadSingle();
				segDef[i].lockLoc.Y = binaryReader.ReadSingle();
				segDef[i].material = binaryReader.ReadInt32();
				segDef[i].name = binaryReader.ReadString();
				segDef[i].sRect = default(Rectangle);
				segDef[i].sRect.X = binaryReader.ReadInt32();
				segDef[i].sRect.Y = binaryReader.ReadInt32();
				segDef[i].sRect.Width = binaryReader.ReadInt32();
				segDef[i].sRect.Height = binaryReader.ReadInt32();
				segDef[i].texIdx = binaryReader.ReadInt32();
				segDef[i].UpdateOrigLoc();
			}
			else if (segDef[i] != null)
			{
				segDef[i] = null;
			}
		}
		binaryReader.Close();
	}

	public void Write(string path)
	{
		BinaryWriter binaryWriter = new BinaryWriter(File.Open(path, FileMode.OpenOrCreate, FileAccess.Write));
		for (int i = 0; i < segDef.Length; i++)
		{
			if (segDef[i] != null)
			{
				binaryWriter.Write(value: true);
				binaryWriter.Write(segDef[i].flags);
				binaryWriter.Write(segDef[i].lockLoc.X);
				binaryWriter.Write(segDef[i].lockLoc.Y);
				binaryWriter.Write(segDef[i].material);
				binaryWriter.Write(segDef[i].name);
				binaryWriter.Write(segDef[i].sRect.X);
				binaryWriter.Write(segDef[i].sRect.Y);
				binaryWriter.Write(segDef[i].sRect.Width);
				binaryWriter.Write(segDef[i].sRect.Height);
				binaryWriter.Write(segDef[i].texIdx);
			}
			else
			{
				binaryWriter.Write(value: false);
			}
		}
		binaryWriter.Close();
	}
}
