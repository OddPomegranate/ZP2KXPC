using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace xCharEdit;

public class CharTexture
{
	public int xSize;

	public int ySize;

	private int cols;

	public string[] rowName = new string[128];

	public int[] cellWidth;

	public string textureName;

	public int textureWidth;

	public Texture2D tex;

	private Rectangle[] sRect = new Rectangle[128];

	private int totalRects;

	public Rectangle GetRect(int idx)
	{
		if (idx < sRect.Length)
		{
			return sRect[idx];
		}
		return default(Rectangle);
	}

	public string GetLineName(int idx)
	{
		if (idx < rowName.Length)
		{
			return rowName[idx];
		}
		return "";
	}

	public CharTexture(string file, int idx, int metaIdx, ContentManager Content)
	{
		Init(file, idx, metaIdx, Content, game: false);
	}

	public CharTexture(string file, int idx, int metaIdx, ContentManager Content, bool game)
	{
		Init(file, idx, metaIdx, Content, game);
	}

	public void Init(string file, int idx, int metaIdx, ContentManager Content, bool game)
	{
		StreamReader streamReader = ((!game) ? new StreamReader("meta/" + file + metaIdx + ".zdx") : new StreamReader("chars/textureMeta/" + file + metaIdx + ".zdx"));
		textureName = streamReader.ReadLine();
		textureWidth = Convert.ToInt32(streamReader.ReadLine());
		cellWidth = new int[16];
		int num = 0;
		string[] array = new string[16];
		while (!streamReader.EndOfStream)
		{
			array[num] = streamReader.ReadLine();
			cellWidth[num] = Convert.ToInt32(streamReader.ReadLine());
			num++;
			if (num > rowName.Length - 1)
			{
				break;
			}
		}
		streamReader.Close();
		int num2 = 64;
		if (textureName == "fish")
		{
			num2 = 180;
		}
		totalRects = 0;
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < textureWidth / cellWidth[i]; j++)
			{
				ref Rectangle reference = ref sRect[totalRects];
				reference = new Rectangle(j * cellWidth[i], i * num2, cellWidth[i], num2);
				if (!game)
				{
					rowName[totalRects] = array[i] + j;
				}
				totalRects++;
			}
		}
		if (game)
		{
			tex = Content.Load<Texture2D>("gfx/chars/" + file + idx);
		}
		else
		{
			tex = Content.Load<Texture2D>("gfx/" + file + idx);
		}
	}
}
