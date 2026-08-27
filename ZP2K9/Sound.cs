using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;

namespace ZP2K9;

// Rewritten 2026-08-22 to drop the Xbox-360-only AudioEngine/WaveBank/SoundBank/Cue
// (XACT3) API, which MonoGame cannot load this project's sfxproj.xgs/snd.xsb/wav.xwb
// from ("XGS format not recognized"). Every cue is now a plain MonoGame SoundEffect
// (or a short list of them, for cues the original game picked randomly between).
//
// Each entry below was resolved from the original wav.xwb/snd.xsb by parsing the
// XACT3 binary format directly - see Claude Temp Here/xnb_tool/xwb_extract_all.py
// and the audio section of the codebase-overview project memory for how. The
// asset names (e.g. "track22_bhit") are exactly the .wav filenames produced by
// that extractor, minus the extension - copy the files from
// Claude Temp Here/audio_extracted/ into Content/sfx/ under those same names
// (rename if you like, just keep the strings below in sync) and add a
// #begin/#end SoundEffect block to Content.mgcb for each one so MGCB builds them.
// The 7 background-music tracks (ravey/blar/jungo/goa/whoami/slow/drone) are
// NOT listed here - they're loaded as Song objects by Music.cs instead, since
// they're multi-minute loops that don't belong fully decompressed in memory
// the way short SFX do.
public class Sound
{
	private const string DEFAULT = "Default";

	private const string MUSIC = "Music";

	public const float MAX_CONFIRM_TIME = 0.15f;

	private const string BRASS_SND = "brass";

	public static float confirmTime;

	private static float brassFrame;

	private static Dictionary<string, List<SoundEffect>> cueSounds;

	// Remembers the last variant index played per cue, so a cue with multiple
	// variants (e.g. "bhit") doesn't play the exact same clip twice in a row.
	// The original XACT data authored every variant with equal (0-255) weight,
	// i.e. plain uniform random selection - this is a small, easily-removable
	// nicety on top of that, not something the original data specified.
	private static Dictionary<string, int> lastVariant = new Dictionary<string, int>();

	private static readonly Random rng = new Random();

	public static void Initialize(ContentManager content)
	{
		SoundEffect L(string name) => content.Load<SoundEffect>("sfx/" + name);

		cueSounds = new Dictionary<string, List<SoundEffect>>();
		cueSounds["auto"] = new List<SoundEffect> { L("track21_auto") };
		// Storage optimization (2026-08-24): "beat" (track54_beat, 50.8MB) removed -
		// confirmed via a full-codebase search that nothing anywhere calls
		// PlayCue("beat") or references it as a WeaponCatalog .snd value. Was
		// costing ~50MB of shipped content and startup load time for a cue
		// nothing ever plays. Music.cs's "Beat" phase is unrelated - it plays
		// the "drone" Song track, not this. Re-add if this ever gets wired up
		// to something; the source track54_beat.wav is still in Content/sfx/.
		cueSounds["bee"] = new List<SoundEffect> { L("track76_bee") };
		cueSounds["beep"] = new List<SoundEffect> { L("track81_beep") };
		cueSounds["beep2"] = new List<SoundEffect> { L("track83_beep2") };
		cueSounds["beewing"] = new List<SoundEffect> { L("track84_beewing"), L("track85_beewing") };
		cueSounds["bhit"] = new List<SoundEffect> { L("track22_bhit"), L("track23_bhit"), L("track24_bhit") };
		cueSounds["bomb"] = new List<SoundEffect> { L("track25_bomb"), L("track26_bomb_boom") };
		cueSounds["boom"] = new List<SoundEffect> { L("track26_bomb_boom"), L("track27_boom_brass") };
		cueSounds["brass"] = new List<SoundEffect> { L("track27_boom_brass"), L("track28_brass"), L("track29_brass"), L("track30_brass"), L("track31_brass"), L("track32_brass") };
		cueSounds["chime"] = new List<SoundEffect> { L("track57_chime") };
		cueSounds["click1"] = new List<SoundEffect> { L("track35_click1") };
		cueSounds["click2"] = new List<SoundEffect> { L("track36_click2") };
		cueSounds["click3"] = new List<SoundEffect> { L("track37_click3") };
		cueSounds["cockit"] = new List<SoundEffect> { L("track00_cockit") };
		cueSounds["confirm"] = new List<SoundEffect> { L("track63_confirm") };
		cueSounds["deagle"] = new List<SoundEffect> { L("track78_deagle") };
		cueSounds["explode"] = new List<SoundEffect> { L("track01_explode"), L("track02_flame_explode") };
		cueSounds["flame"] = new List<SoundEffect> { L("track02_flame_explode") };
		cueSounds["flare"] = new List<SoundEffect> { L("track03_flare_rflare") };
		cueSounds["flaunch"] = new List<SoundEffect> { L("track80_flaunch") };
		cueSounds["fsword"] = new List<SoundEffect> { L("track79_fsword") };
		cueSounds["glass"] = new List<SoundEffect> { L("track44_glass") };
		cueSounds["handcan"] = new List<SoundEffect> { L("track04_handcan") };
		cueSounds["hit"] = new List<SoundEffect> { L("track05_hit") };
		cueSounds["hit1"] = new List<SoundEffect> { L("track38_hit1") };
		cueSounds["hit2"] = new List<SoundEffect> { L("track39_hit2") };
		cueSounds["hit3"] = new List<SoundEffect> { L("track40_hit3") };
		cueSounds["ice"] = new List<SoundEffect> { L("track41_ice") };
		cueSounds["infector"] = new List<SoundEffect> { L("track70_infector") };
		cueSounds["jet"] = new List<SoundEffect> { L("track56_jet") };
		cueSounds["jetstart"] = new List<SoundEffect> { L("track55_jetstart") };
		cueSounds["launch"] = new List<SoundEffect> { L("track34_launch_plasma") };
		cueSounds["levup"] = new List<SoundEffect> { L("track64_levup") };
		cueSounds["mp5"] = new List<SoundEffect> { L("track06_mp5") };
		cueSounds["nukesplode"] = new List<SoundEffect> { L("track82_nukesplode") };
		cueSounds["parafire"] = new List<SoundEffect> { L("track07_parafire") };
		cueSounds["pistol"] = new List<SoundEffect> { L("track08_pistol") };
		cueSounds["plasma"] = new List<SoundEffect> { L("track33_plasma"), L("track34_launch_plasma") };
		cueSounds["plasmahit"] = new List<SoundEffect> { L("track45_plasmahit") };
		cueSounds["pop"] = new List<SoundEffect> { L("track50_pop") };
		cueSounds["rainbow"] = new List<SoundEffect> { L("track69_rainbow") };
		cueSounds["rainjet"] = new List<SoundEffect> { L("track88_rainjet"), L("track89_rainjet") };
		cueSounds["revol"] = new List<SoundEffect> { L("track09_revol") };
		cueSounds["rflare"] = new List<SoundEffect> { L("track03_flare_rflare") };
		cueSounds["rico"] = new List<SoundEffect> { L("track10_rico_shell") };
		cueSounds["rifle"] = new List<SoundEffect> { L("track11_rifle_shell") };
		cueSounds["saber"] = new List<SoundEffect> { L("track71_saber"), L("track72_saber"), L("track77_saber") };
		cueSounds["shell"] = new List<SoundEffect> { L("track10_rico_shell"), L("track11_rifle_shell"), L("track12_shell") };
		cueSounds["shotgun"] = new List<SoundEffect> { L("track13_shotgun") };
		cueSounds["shrink"] = new List<SoundEffect> { L("track42_shrink") };
		cueSounds["shrinksplash"] = new List<SoundEffect> { L("track43_shrinksplash") };
		cueSounds["silen"] = new List<SoundEffect> { L("track14_silen") };
		// Storage optimization (2026-08-24): "soft" (track49_soft, 43.8MB) removed
		// for the same reason as "beat" above - never called by PlayCue() or
		// referenced anywhere as a .snd value. Source wav left in place in case
		// this gets wired up later.
		cueSounds["splash"] = new List<SoundEffect> { L("track47_splash") };
		cueSounds["suit"] = new List<SoundEffect> { L("track46_suit") };
		cueSounds["swarm"] = new List<SoundEffect> { L("track73_swarm"), L("track74_swarm"), L("track75_swarm") };
		cueSounds["swing"] = new List<SoundEffect> { L("track15_swing") };
		cueSounds["sword"] = new List<SoundEffect> { L("track51_sword") };
		cueSounds["tase"] = new List<SoundEffect> { L("track16_tase") };
		cueSounds["tec9fire"] = new List<SoundEffect> { L("track17_tec9fire") };
		cueSounds["throw"] = new List<SoundEffect> { L("track18_throw") };
		cueSounds["thud"] = new List<SoundEffect> { L("track19_thud") };
		cueSounds["uzi"] = new List<SoundEffect> { L("track20_uzi") };
		cueSounds["wing"] = new List<SoundEffect> { L("track86_wing"), L("track87_wing") };
		cueSounds["zbomb"] = new List<SoundEffect> { L("track62_zbomb") };
		cueSounds["zcharge1"] = new List<SoundEffect> { L("track58_zcharge1") };
		cueSounds["zcharge2"] = new List<SoundEffect> { L("track59_zcharge2") };
		cueSounds["zcharge3"] = new List<SoundEffect> { L("track60_zcharge3") };
		cueSounds["zexplode"] = new List<SoundEffect> { L("track61_zexplode") };
	}

	public static void PlayCue(string cue)
	{
		try
		{
			if (cueSounds == null || !cueSounds.TryGetValue(cue, out List<SoundEffect> variants) || variants.Count == 0)
			{
				return;
			}

			int index;
			if (variants.Count == 1)
			{
				index = 0;
			}
			else
			{
				lastVariant.TryGetValue(cue, out int last);
				do
				{
					index = rng.Next(variants.Count);
				}
				while (index == last && variants.Count > 1);
				lastVariant[cue] = index;
			}

			float volume = (float)Game1.settings.sfx / 10f;
			if (volume < 0f) volume = 0f;
			if (volume > 1f) volume = 1f;
			variants[index].Play(volume, 0f, 0f);
		}
		catch
		{
		}
	}

	public static void PlayBrass()
	{
		if (brassFrame <= 0f)
		{
			PlayCue("brass");
			brassFrame = 0.1f;
		}
	}

	public static void Update()
	{
		if (confirmTime > 0f)
		{
			confirmTime -= Game1.frameTime;
		}
		if (brassFrame > 0f)
		{
			brassFrame -= Game1.frameTime;
		}
	}

	internal static void PlayConfirm()
	{
		if (confirmTime <= 0f)
		{
			PlayCue("confirm");
			confirmTime = 0.15f;
		}
	}

	internal static void DoLevup()
	{
		PlayCue("levup");
		Music.Reset();
	}
}
