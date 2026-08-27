using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;

namespace ZP2K9;

// Rewritten 2026-08-22 alongside Sound.cs to drop the XACT3 AudioEngine/Cue API.
// The 7 background-music tracks are long (the shortest is several minutes), so
// unlike the short SFX in Sound.cs these are loaded as MonoGame Song objects and
// played through the single global MediaPlayer instead of as in-memory
// SoundEffects - that matches how they were actually used here: Update() below
// only ever wants at most one of {rock track, beat/drone, menu/slow} playing at
// a time, which is exactly what MediaPlayer (one active song at a time) models.
//
// Asset names below are the extracted .wav filenames (minus extension) from
// Claude Temp Here/audio_extracted/ - copy them into Content/sfx/ (renaming if
// you like, just keep the strings in sync) and add a #begin/#end Song block to
// Content.mgcb for each one.
internal class Music
{
	private enum MusicPhase
	{
		StartRock,
		Rock,
		StartBeat,
		Beat,
		Quiet
	}

	public static bool playing = false;

	public static bool ready = false;

	private static string[] rockString = new string[5] { "ravey", "blar", "jungo", "goa", "whoami" };

	public static bool runOut = false;

	public static int curRock = 0;

	private static MusicPhase musicPhase;

	private static Dictionary<string, Song> songs;

	private static Song currentRockSong;

	private static Song beatSong;

	private static Song menuSong;

	// The song MediaPlayer was last told to play. Since MediaPlayer only ever
	// plays one Song at a time, comparing against this (plus State) tells us
	// whether "the song we care about" is still the one actually playing,
	// as opposed to just "is anything playing" (which broke the StartRock ->
	// menu-song-still-active transition when it was tried).
	private static Song activeSong;

	public static void Init(ContentManager content)
	{
		songs = new Dictionary<string, Song>
		{
			{ "ravey", content.Load<Song>("sfx/track65_ravey") },
			{ "blar", content.Load<Song>("sfx/track66_blar") },
			{ "jungo", content.Load<Song>("sfx/track48_jungo") },
			{ "goa", content.Load<Song>("sfx/track52_goa") },
			{ "whoami", content.Load<Song>("sfx/track53_whoami") },
			{ "slow", content.Load<Song>("sfx/track67_slow") },
			{ "drone", content.Load<Song>("sfx/track68_drone") },
		};

		for (int i = 0; i < 8; i++)
		{
			int randomInt = Rand.GetRandomInt(0, rockString.Length);
			int randomInt2 = Rand.GetRandomInt(0, rockString.Length);
			string text = rockString[randomInt2];
			rockString[randomInt2] = rockString[randomInt];
			rockString[randomInt] = text;
		}

		beatSong = songs["drone"];
		menuSong = songs["slow"];
		currentRockSong = songs[rockString[0]];
		musicPhase = MusicPhase.Quiet;
		ready = true;
	}

	public static void Reset()
	{
		switch (musicPhase)
		{
		case MusicPhase.StartBeat:
		case MusicPhase.Beat:
		case MusicPhase.Quiet:
			musicPhase = MusicPhase.StartRock;
			break;
		}
	}

	private static bool IsActive(Song song)
	{
		return activeSong == song && MediaPlayer.State == MediaState.Playing;
	}

	private static void PlaySong(Song song)
	{
		MediaPlayer.Play(song);
		activeSong = song;
	}

	public static void Update()
	{
		try
		{
			float volume = (float)Game1.settings.bgm / 10f;
			if (volume < 0f) volume = 0f;
			if (volume > 1f) volume = 1f;
			MediaPlayer.Volume = volume;

			if (playing)
			{
				switch (musicPhase)
				{
				case MusicPhase.StartRock:
					if (!IsActive(currentRockSong))
					{
						currentRockSong = songs[rockString[curRock]];
						curRock = (curRock + 1) % rockString.Length;
						PlaySong(currentRockSong);
					}
					if (IsActive(currentRockSong))
					{
						musicPhase = MusicPhase.Rock;
					}
					break;
				case MusicPhase.Rock:
					if (!IsActive(currentRockSong))
					{
						musicPhase = MusicPhase.StartBeat;
					}
					break;
				case MusicPhase.StartBeat:
					if (!IsActive(beatSong))
					{
						PlaySong(beatSong);
						musicPhase = MusicPhase.Beat;
					}
					break;
				case MusicPhase.Beat:
					if (!IsActive(beatSong))
					{
						musicPhase = MusicPhase.Quiet;
					}
					break;
				}
			}
			else
			{
				if (!IsActive(menuSong))
				{
					PlaySong(menuSong);
				}
				musicPhase = MusicPhase.Quiet;
			}
		}
		catch
		{
		}
	}
}
