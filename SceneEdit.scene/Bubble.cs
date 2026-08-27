using System.IO;
using System.Text;
using Microsoft.Xna.Framework;

namespace SceneEdit.scene;

public class Bubble
{
	public enum BubbleType
	{
		Box,
		LBubble,
		RBubble,
		LExclaim,
		RExclaim,
		Oval,
		Trig_FadeOut,
		Trig_Glow,
		Trig_Speckles,
		Trig_GlowOut,
		Trig_GlowIn,
		Trig_Flicker,
		Trig_Spazz,
		Trig_Endlevel,
		Trig_Strobe,
		Trig_CreepFilter,
		Trig_Smoothcam,
		Trig_GlowOutCrazy,
		Trig_EndGame,
		Trig_Miniadjust
	}

	public enum BubbleColor
	{
		White,
		Black,
		Gray,
		Clear
	}

	public enum BubbleJustify
	{
		Left,
		Middle,
		Right
	}

	public float time;

	public float timeOff;

	public string name = "";

	public StringBuilder[] text;

	public Vector2 loc;

	public BubbleType type;

	public BubbleColor color;

	public BubbleJustify justify;

	internal void Write(BinaryWriter writer)
	{
		writer.Write(time);
		writer.Write(timeOff);
		writer.Write(name);
		if (text == null)
		{
			text = new StringBuilder[1]
			{
				new StringBuilder("")
			};
		}
		writer.Write(text.Length);
		for (int i = 0; i < text.Length; i++)
		{
			writer.Write(text[i].ToString());
		}
		writer.Write(loc.X);
		writer.Write(loc.Y);
		writer.Write((int)type);
		writer.Write((int)color);
		writer.Write((int)justify);
	}

	internal void Read(BinaryReader reader)
	{
		time = reader.ReadSingle();
		timeOff = reader.ReadSingle();
		name = reader.ReadString();
		int num = reader.ReadInt32();
		text = new StringBuilder[num];
		for (int i = 0; i < num; i++)
		{
			text[i] = new StringBuilder(reader.ReadString());
		}
		loc = new Vector2(reader.ReadSingle(), reader.ReadSingle());
		type = (BubbleType)reader.ReadInt32();
		color = (BubbleColor)reader.ReadInt32();
		justify = (BubbleJustify)reader.ReadInt32();
	}
}
