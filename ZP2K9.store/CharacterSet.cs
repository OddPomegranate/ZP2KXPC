using System.IO;
using System.Text;

namespace ZP2K9.store;

public class CharacterSet
{
	private CharacterClass[] characterClass;

	public int bodyType;

	public string name;

	public StringBuilder nameStr;

	public int[] perk;

	public int defaultTeam;

	public void SetName(string name)
	{
		this.name = name;
		nameStr = new StringBuilder(name);
	}

	public void Write(BinaryWriter writer)
	{
		writer.Write(name);
		writer.Write(bodyType);
		for (int i = 0; i < characterClass.Length; i++)
		{
			characterClass[i].Write(writer);
		}
		for (int j = 0; j < 3; j++)
		{
			writer.Write(perk[j]);
		}
		writer.Write(defaultTeam);
	}

	public void Read(BinaryReader reader)
	{
		name = reader.ReadString();
		bodyType = reader.ReadInt32();
		for (int i = 0; i < characterClass.Length; i++)
		{
			characterClass[i].Read(reader);
		}
		for (int j = 0; j < 3; j++)
		{
			perk[j] = reader.ReadInt32();
		}
		defaultTeam = reader.ReadInt32();
	}

	public CharacterSet()
	{
		characterClass = new CharacterClass[2];
		for (int i = 0; i < characterClass.Length; i++)
		{
			characterClass[i] = new CharacterClass();
		}
		perk = new int[3];
	}

	public CharacterClass CharClass()
	{
		if (bodyType == 0)
		{
			return characterClass[0];
		}
		return characterClass[1];
	}

	public void SetDefaultLoadout(int i)
	{
		switch (i)
		{
		case 0:
			SetName("Airborne");
			perk[0] = 0;
			perk[1] = 5;
			perk[2] = 2;
			bodyType = 0;
			characterClass[0].skinTex = 0;
			characterClass[0].headTex = 2;
			characterClass[0].torsoTex = 2;
			characterClass[0].legsTex = 2;
			characterClass[0].hatTex = 2;
			defaultTeam = 0;
			break;
		case 1:
			SetName("Assassin");
			perk[0] = 2;
			perk[1] = 0;
			perk[2] = 6;
			bodyType = 0;
			characterClass[0].skinTex = 0;
			characterClass[0].headTex = 1;
			characterClass[0].torsoTex = 1;
			characterClass[0].legsTex = 1;
			characterClass[0].hatTex = 1;
			defaultTeam = 1;
			break;
		case 2:
			SetName("Tank");
			perk[0] = 6;
			perk[1] = 2;
			perk[2] = 5;
			bodyType = 0;
			characterClass[0].skinTex = 1;
			characterClass[0].headTex = 3;
			characterClass[0].torsoTex = 3;
			characterClass[0].legsTex = 3;
			characterClass[0].hatTex = 3;
			defaultTeam = 0;
			break;
		case 3:
			SetName("Bomber");
			perk[0] = 5;
			perk[1] = 7;
			perk[2] = 1;
			bodyType = 0;
			characterClass[0].skinTex = 0;
			characterClass[0].headTex = 4;
			characterClass[0].torsoTex = 4;
			characterClass[0].legsTex = 4;
			characterClass[0].hatTex = 4;
			defaultTeam = 1;
			break;
		case 4:
			SetName("Shogun");
			perk[0] = 3;
			perk[1] = 6;
			perk[2] = 4;
			bodyType = 0;
			characterClass[0].skinTex = 0;
			characterClass[0].headTex = 10;
			characterClass[0].torsoTex = 10;
			characterClass[0].legsTex = 10;
			characterClass[0].hatTex = 15;
			defaultTeam = 0;
			break;
		case 5:
			SetName("Gunner");
			perk[0] = 4;
			perk[1] = 3;
			perk[2] = 3;
			bodyType = 0;
			characterClass[0].skinTex = 1;
			characterClass[0].headTex = 6;
			characterClass[0].torsoTex = 6;
			characterClass[0].legsTex = 6;
			characterClass[0].hatTex = 1;
			defaultTeam = 1;
			break;
		case 6:
			SetName("Agent");
			perk[0] = 1;
			perk[1] = 4;
			perk[2] = 7;
			bodyType = 0;
			characterClass[0].skinTex = 2;
			characterClass[0].headTex = 9;
			characterClass[0].torsoTex = 9;
			characterClass[0].legsTex = 9;
			characterClass[0].hatTex = 14;
			defaultTeam = 0;
			break;
		case 7:
			SetName("Samurai");
			perk[0] = 7;
			perk[1] = 1;
			perk[2] = 0;
			bodyType = 0;
			characterClass[0].skinTex = 0;
			characterClass[0].headTex = 5;
			characterClass[0].torsoTex = 5;
			characterClass[0].legsTex = 5;
			characterClass[0].hatTex = 5;
			defaultTeam = 1;
			break;
		}
		characterClass[0].headTex++;
		characterClass[0].torsoTex++;
		characterClass[0].legsTex++;
		characterClass[0].hatTex++;
		characterClass[1].headTex = 1;
		characterClass[1].legsTex = 1;
		characterClass[1].torsoTex = 1;
		characterClass[1].hatTex = 0;
		characterClass[1].skinTex = 0;
	}
}
