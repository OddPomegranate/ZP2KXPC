using System;
using System.IO;

namespace xCharEdit.Character;

public class CharDef
{
	private Animation[] animation;

	private Frame[] frame;

	public string path;

	public int charIdx;

	public int pteroIdx;

	public int weaponIdx;

	public CharDef()
	{
		animation = new Animation[128];
		for (int i = 0; i < animation.Length; i++)
		{
			animation[i] = new Animation();
		}
		frame = new Frame[512];
		for (int j = 0; j < frame.Length; j++)
		{
			frame[j] = new Frame();
		}
		path = "char";
	}

	public CharDef(string path)
	{
		animation = new Animation[128];
		frame = new Frame[512];
		this.path = path;
		Read(abs: true);
	}

	public Animation GetAnimation(int idx)
	{
		return animation[idx];
	}

	public void SetAnimation(int idx, Animation _animation)
	{
		animation[idx] = _animation;
	}

	public Animation[] GetAnimationArray()
	{
		return animation;
	}

	public Frame GetFrame(int idx)
	{
		if (idx < 0)
		{
			idx = 0;
		}
		return frame[idx];
	}

	public void SetFrame(int idx, Frame _frame)
	{
		frame[idx] = _frame;
	}

	public Frame[] GetFrameArray()
	{
		return frame;
	}

	public void WriteBackup()
	{
		Write("data/" + path + ".zmx");
		Write("data/" + path + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + "-" + DateTime.Now.Year + "-" + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second + ".zmx");
	}

	public void Write()
	{
		Write("data/" + path + ".zmx");
	}

	public void Write(string writePath)
	{
		BinaryWriter binaryWriter = new BinaryWriter(File.Open(writePath, FileMode.Create));
		binaryWriter.Write(path);
		binaryWriter.Write(charIdx);
		binaryWriter.Write(weaponIdx);
		for (int i = 0; i < animation.Length; i++)
		{
			binaryWriter.Write(animation[i].name);
			int num = 0;
			for (int j = 0; j < animation[i].getKeyFrameArray().Length; j++)
			{
				if (animation[i].GetKeyFrame(j).frameRef < 0)
				{
					num = j;
					break;
				}
			}
			binaryWriter.Write(num);
			for (int k = 0; k < num; k++)
			{
				KeyFrame keyFrame = animation[i].GetKeyFrame(k);
				binaryWriter.Write(keyFrame.frameRef);
				binaryWriter.Write(keyFrame.duration);
				binaryWriter.Write(keyFrame.lerp);
				string[] scriptArray = keyFrame.getScriptArray();
				byte b = (byte)keyFrame.GetScriptLength();
				binaryWriter.Write(b);
				for (byte b2 = 0; b2 < b; b2++)
				{
					binaryWriter.Write(scriptArray[b2]);
				}
			}
		}
		for (int l = 0; l < frame.Length; l++)
		{
			binaryWriter.Write(frame[l].name);
			for (int m = 0; m < frame[l].GetPartArray().Length; m++)
			{
				Part part = frame[l].GetPart(m);
				binaryWriter.Write(part.idx);
				binaryWriter.Write(part.location.X);
				binaryWriter.Write(part.location.Y);
				binaryWriter.Write(part.rotation);
				binaryWriter.Write(part.scaling.X);
				binaryWriter.Write(part.scaling.Y);
				binaryWriter.Write(part.flip);
			}
		}
		binaryWriter.Close();
		Console.WriteLine("Saved: " + writePath);
	}

	public void Read()
	{
		Read(abs: false);
	}

	public void Read(bool abs)
	{
		BinaryReader binaryReader = (abs ? new BinaryReader(File.Open(path, FileMode.Open, FileAccess.Read)) : new BinaryReader(File.Open("data/" + path + ".zmx", FileMode.Open, FileAccess.Read)));
		path = binaryReader.ReadString();
		charIdx = binaryReader.ReadInt32();
		weaponIdx = binaryReader.ReadInt32();
		for (int i = 0; i < animation.Length; i++)
		{
			animation[i] = new Animation();
			animation[i].name = binaryReader.ReadString();
			int num = binaryReader.ReadInt32();
			for (int j = 0; j < num; j++)
			{
				KeyFrame keyFrame = animation[i].GetKeyFrame(j);
				keyFrame.frameRef = binaryReader.ReadInt32();
				keyFrame.duration = binaryReader.ReadInt32();
				keyFrame.lerp = binaryReader.ReadBoolean();
				byte b = binaryReader.ReadByte();
				for (byte b2 = 0; b2 < b; b2++)
				{
					binaryReader.ReadString();
				}
			}
			for (int k = num; k < animation[i].getKeyFrameArray().Length; k++)
			{
				animation[i].ClearKey(k);
			}
		}
		for (int l = 0; l < frame.Length; l++)
		{
			frame[l] = new Frame();
			frame[l].name = binaryReader.ReadString();
			for (int m = 0; m < frame[l].GetPartArray().Length; m++)
			{
				Part part = frame[l].GetPart(m);
				part.idx = binaryReader.ReadInt32();
				part.location.X = binaryReader.ReadSingle();
				part.location.Y = binaryReader.ReadSingle();
				part.rotation = binaryReader.ReadSingle();
				part.scaling.X = binaryReader.ReadSingle();
				part.scaling.Y = binaryReader.ReadSingle();
				part.flip = binaryReader.ReadInt32();
			}
		}
		binaryReader.Close();
	}
}
