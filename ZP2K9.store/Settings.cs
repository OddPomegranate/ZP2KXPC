using System.IO;

namespace ZP2K9.store;

public class Settings
{
	public const int VERSION = 124;

	public bool rumble = true;

	public int curTeam;

	public bool showNames = true;

	public bool vibration = true;

	public bool autoSwitch = true;

	public bool allowHandicapping = true;

	public bool upToJetpack = true;

	public bool twinStickShooter = true;

	// Default volume, both channels: 6/10 = 60% (see GameSettings.cs's
	// ITEM_SFX/ITEM_BGM MenuItem strings - selX 6 is "60%" on the 11-step
	// Off/10%/.../Max scale) rather than maxed out on first run.
	public int sfx = 6;

	public int bgm = 6;

	public void Write(BinaryWriter writer)
	{
		writer.Write(rumble);
		writer.Write(showNames);
		writer.Write(vibration);
		writer.Write(autoSwitch);
		writer.Write(allowHandicapping);
		writer.Write(upToJetpack);
		writer.Write(twinStickShooter);
		writer.Write(sfx);
		writer.Write(bgm);
		Game1.zProfile.Write(writer);
	}

	public void Read(BinaryReader reader)
	{
		rumble = reader.ReadBoolean();
		showNames = reader.ReadBoolean();
		vibration = reader.ReadBoolean();
		autoSwitch = reader.ReadBoolean();
		allowHandicapping = reader.ReadBoolean();
		upToJetpack = reader.ReadBoolean();
		twinStickShooter = reader.ReadBoolean();
		sfx = reader.ReadInt32();
		bgm = reader.ReadInt32();
		Game1.zProfile.Read(reader);
	}
}
