using System;
using System.IO;
using System.Threading;
using Microsoft.Xna.Framework;
using ZP2K9.platform;

namespace ZP2K9.store;

public class Store
{
	public const int STORE_SETTINGS = 0;

	public const int STORE_GAME = 1;

	public const int STORE_MAP = 2;

	public StorageDevice device;

	private IAsyncResult deviceResult;

	public StorageContainer container;

	public bool pendingDevice;

	private bool containerOpen;

	public bool retry;

	public bool failed;

	private string[] storeStr = new string[2] { "settings.sav", "game.sav" };

	private int threadedWriteType;

	public string mapPath = "map";

	public void GetDevice()
	{
		if (!Guide.IsVisible)
		{
			deviceResult = Guide.BeginShowStorageDeviceSelector((PlayerIndex)Game1.mainPlayerIndex, (AsyncCallback)null, (object)null);
			pendingDevice = true;
		}
		else
		{
			retry = true;
		}
	}

	public void Update()
	{
		if (retry)
		{
			GetDevice();
		}
		if (pendingDevice && deviceResult.IsCompleted)
		{
			device = Guide.EndShowStorageDeviceSelector(deviceResult);
			pendingDevice = false;
			if (!CheckDeviceFail())
			{
				Read(0);
			}
			else
			{
				Game1.menu.DoError("Storage device failure! Saving is disabled.", 0);
			}
		}
	}

	private bool CheckDeviceFail()
	{
		if (pendingDevice)
		{
			return true;
		}
		if (device == null)
		{
			return true;
		}
		if (!device.IsConnected)
		{
			return true;
		}
		return false;
	}

	private void OpenContainer()
	{
		if (!containerOpen)
		{
			container = device.OpenContainer("zp2k5");
		}
		containerOpen = true;
	}

	private void CommitContainer()
	{
		if (containerOpen)
		{
			try
			{
				container.Dispose();
				containerOpen = false;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}
	}

	public void Write(int type)
	{
		if (device == null)
		{
			return;
		}
		Thread thread = new Thread(ThreadedWrite);
		lock (device)
		{
			threadedWriteType = type;
			thread.Start();
		}
	}

	public void ThreadedWrite()
	{
		int num = threadedWriteType;
		if (CheckDeviceFail())
		{
			return;
		}
		try
		{
			OpenContainer();
		}
		catch
		{
			return;
		}
		string path = ((num != 2) ? Path.Combine(container.Path, storeStr[num]) : Path.Combine(container.Path, "map_" + mapPath + ".zdx"));
		try
		{
			FileStream fileStream = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write);
			BinaryWriter binaryWriter = new BinaryWriter(fileStream);
			try
			{
				switch (num)
				{
				case 0:
					// BUG FIX (2026-08-25): was a hardcoded literal 123, so
					// bumping Settings.VERSION (done earlier for the human48/
					// zombie48 costume's clothesList/hatList length change)
					// never actually took effect here - every save still got
					// tagged with the old literal, so an old save's mismatched
					// unlock-array lengths would never trigger the "wrong
					// version, reset the save" path below. Writing the real
					// constant is what makes that VERSION bump (or any future
					// one) actually do something.
					binaryWriter.Write(Settings.VERSION);
					Game1.settings.Write(binaryWriter);
					break;
				case 1:
				case 2:
					break;
				}
			}
			catch
			{
			}
			fileStream.Close();
			CommitContainer();
			Game1.menu.saving.Set();
		}
		catch (Exception)
		{
		}
	}

	public void Read(int type)
	{
		if (CheckDeviceFail())
		{
			return;
		}
		try
		{
			OpenContainer();
		}
		catch
		{
			return;
		}
		string path = ((type != 2) ? Path.Combine(container.Path, storeStr[type]) : Path.Combine(container.Path, "map_" + mapPath + ".zdx"));
		if (!File.Exists(path))
		{
			if (type == 0)
			{
				Game1.settings = new Settings();
			}
			return;
		}
		FileStream fileStream = File.Open(path, FileMode.Open, FileAccess.Read);
		try
		{
			BinaryReader binaryReader = new BinaryReader(fileStream);
			try
			{
				switch (type)
				{
				case 0:
				{
					int num = binaryReader.ReadInt32();
					if (num != Settings.VERSION)
					{
						Game1.menu.DoError("Save data wrong version: " + num, 0);
						Game1.settings = new Settings();
					}
					else
					{
						Game1.settings.Read(binaryReader);
					}
					Game1.menu.InitInfoBox();
					break;
				}
				case 1:
				case 2:
					break;
				}
			}
			catch
			{
			}
			binaryReader.Close();
			fileStream.Close();
			CommitContainer();
		}
		catch
		{
		}
	}
}
