using System.IO;

namespace ZP2K9.store;

public class CharacterClass
{
	public int skinTex;

	public int headTex;

	public int torsoTex;

	public int legsTex;

	public int hatTex;

	public int jetpack;

	internal void Read(BinaryReader reader)
	{
		skinTex = reader.ReadInt32();
		headTex = reader.ReadInt32();
		hatTex = reader.ReadInt32();
		torsoTex = reader.ReadInt32();
		legsTex = reader.ReadInt32();
		jetpack = reader.ReadInt32();
	}

	internal void Write(BinaryWriter writer)
	{
		writer.Write(skinTex);
		writer.Write(headTex);
		writer.Write(hatTex);
		writer.Write(torsoTex);
		writer.Write(legsTex);
		writer.Write(jetpack);
	}
}
