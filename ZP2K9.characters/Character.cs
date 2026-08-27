using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using xCharEdit.Character;
using ZP2K9.platform;
using yMapEdit.map;
using Yuki_Win;
using ZP2K9.ai;
using ZP2K9.characters.weapons;
using ZP2K9.debug;
using ZP2K9.hud.messageHud;
using ZP2K9.map;
using ZP2K9.particles;
using ZP2K9.store;

namespace ZP2K9.characters;

public class Character
{
	public const int FACE_LEFT = 0;

	public const int FACE_RIGHT = 1;

	public const float KILL_REFRESH_VAL = 3f;

	public const int BODY_ALL = 0;

	public const int BODY_UPPER = 1;

	public const int STATE_AIR = 0;

	public const int STATE_GROUNDED = 1;

	public const int STATE_LWALL = 2;

	public const int STATE_RWALL = 3;

	public const int STATE_UWALL = 4;

	public const int MAX_GRENS = 5;

	public const int IDLE_G = 0;

	public const int RUN_G = 1;

	public const int FLY_G = 2;

	public const int FIRE_G_L = 3;

	public const int FIRE_G_UL = 4;

	public const int FIRE_G_U = 5;

	public const int FIRE_G_DL = 6;

	public const int FIRE_G_D = 7;

	public const int LAND_G = 8;

	public const int JUMP_G = 9;

	public const int RELOAD_G = 10;

	public const int SWITCH_G = 11;

	public const int FIREX_G_L = 12;

	public const int FIREX_G_UL = 13;

	public const int FIREX_G_U = 14;

	public const int FIREX_G_DL = 15;

	public const int FIREX_G_D = 16;

	public const int IDLE_W = 0;

	public const int RUN_W = 1;

	public const int FLY_W = 2;

	public const int FIRE_W_L = 3;

	public const int FIRE_W_UL = 4;

	public const int FIRE_W_U = 5;

	public const int FIRE_W_DL = 6;

	public const int FIRE_W_D = 7;

	public const int LAND_W = 8;

	public const int JUMP_W = 9;

	public const int RELOAD_W = 10;

	public const int SWITCH_W = 11;

	public const int IDLE_M = 12;

	public const int RUN_M = 13;

	public const int FLY_M = 14;

	public const int FIRE_M_L = 15;

	public const int FIRE_M_UL = 16;

	public const int FIRE_M_U = 17;

	public const int FIRE_M_DL = 18;

	public const int FIRE_M_D = 19;

	public const int LAND_M = 20;

	public const int JUMP_M = 21;

	public const int RELOAD_M = 22;

	public const int SWITCH_M = 23;

	public const int IDLE_S = 24;

	public const int RUN_S = 25;

	public const int FLY_S = 26;

	public const int FIRE_S_L = 27;

	public const int FIRE_S_UL = 28;

	public const int FIRE_S_U = 29;

	public const int FIRE_S_DL = 30;

	public const int FIRE_S_D = 31;

	public const int LAND_S = 32;

	public const int JUMP_S = 33;

	public const int RELOAD_S = 34;

	public const int SWITCH_S = 35;

	public const int JHIT = 36;

	public const int HITLAND = 37;

	public const int GREN = 38;

	public const int GRENPRIME = 39;

	public const int IDLE_A = 40;

	public const int RUN_A = 41;

	public const int FLY_A = 42;

	public const int FIRE_A_L = 43;

	public const int FIRE_A_UL = 44;

	public const int FIRE_A_U = 45;

	public const int FIRE_A_DL = 46;

	public const int FIRE_A_D = 47;

	public const int LAND_A = 48;

	public const int JUMP_A = 49;

	public const int RELOAD_A = 50;

	public const int SWITCH_A = 51;

	public const int FIREX_A_L = 52;

	public const int FIREX_A_UL = 53;

	public const int FIREX_A_U = 54;

	public const int FIREX_A_DL = 55;

	public const int FIREX_A_D = 56;

	public const int CART = 57;

	public const int KICK = 58;

	public const int ROLL = 59;

	public const int IDLE_R = 60;

	public const int RUN_R = 61;

	public const int FLY_R = 62;

	public const int FIRE_R_L = 63;

	public const int FIRE_R_UL = 64;

	public const int FIRE_R_U = 65;

	public const int FIRE_R_DL = 66;

	public const int FIRE_R_D = 67;

	public const int LAND_R = 68;

	public const int JUMP_R = 69;

	public const int RELOAD_R = 70;

	public const int SWITCH_R = 71;

	public const int IDLE_X = 72;

	public const int RUN_X = 73;

	public const int FLY_X = 74;

	public const int FIRE_X_L = 75;

	public const int FIRE_X_UL = 76;

	public const int FIRE_X_U = 77;

	public const int FIRE_X_DL = 78;

	public const int FIRE_X_D = 79;

	public const int LAND_X = 80;

	public const int JUMP_X = 81;

	public const int RELOAD_X = 82;

	public const int SWITCH_X = 83;

	public const int ROLL_X = 84;

	public const int KICK_X = 85;

	public const int SQUAT = 86;

	public const int SPAWN = 87;

	public const int SUICIDE = 88;

	public const int WEAP_W = 0;

	public const int WEAP_M = 1;

	public const int WEAP_S = 2;

	public const int WEAP_A = 3;

	public const int WEAP_R = 4;

	public const int WEAP_X = 5;

	public const int SPECIAL_NONE = 0;

	public const int SPECIAL_JETPACK = 1;

	public const int SPECIAL_WALLRUNNER = 2;

	public const int TEAM_ALL = 0;

	public const int TEAM_BLUE = 1;

	public const int TEAM_RED = 2;

	public const int META_WRITE_APPEARANCE = 0;

	public const int META_WRITE_APPEARANCE_2 = 1;

	public const int META_WRITE_PERKS = 2;

	public const int META_WRITE_CLANTAG = 3;

	public const int META_WRITE_LEVEL_AND_SCORE = 4;

	public const int TOTAL_META_WRITES = 5;

	private const float LAG_SNAPSHOT_RESOLUTION = 0.02f;

	private const float GAS_LIM = 1.25f;

	private const float GAS_LIM2 = 1.75f;

	public const string S_CART = "cart";

	public const string S_SUICIDE = "suicide";

	public int face;

	public int legsTex;

	public int torsoTex;

	public int headTex;

	public int hatTex;

	public int jetpack;

	public int bodyType;

	public int skinTex;

	private float noStick;

	public int[] perk = new int[3];

	public char[] clanChar = new char[3];

	public bool needsClantagUpdate;

	public int level;

	public bool[] hitByChar;

	public Vector2 hitVec;

	public Vector2 hitTraj;

	public int hitType;

	public int team;

	public float killRefreshFrame;

	public bool netJetpack;

	public float nameAlpha;

	public float scale = 1f;

	public Vector2 loc = default(Vector2);

	public Vector2 traj = default(Vector2);

	public Vector2 radarTraj = default(Vector2);

	public Vector2 goalLoc = default(Vector2);

	public int lastNode = -1;

	public int defIdx;

	public float capFixFrame;

	public BodySec[] bodySec = new BodySec[2];

	public bool splitAnim;

	public float kickFrame;

	private Vector2 torsoVec;

	public int score;

	public int kills;

	public int deaths;

	public int killStreak;

	public int multikill;

	public float multikillframe;

	public int killedBy = -1;

	public int killType = 1;

	public int state;

	public CharKeys charKeys;

	public int keySrc;

	public AI ai;

	private float rollFrame;

	private float kickRecoverFrame;

	public float timeSinceHit;

	public int[] grenType = new int[10] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

	public int[] grenAmmo = new int[10];

	public int[] magazine = new int[4];

	public Vector2 drawVec;

	public Vector2 drawTraj = default(Vector2);

	private float jetVal;

	public bool submerged;

	public float submergedFrame;

	public float charge;

	public bool isRosterChar;

	public float latency;

	public float fishFrame;

	private string[] animList = new string[89]
	{
		"idlew", "runw", "flyw", "firewl", "firewul", "firewu", "firewdl", "firewd", "landw", "jumpw",
		"reloadw", "switchw", "idlem", "runm", "flym", "fireml", "firemul", "firemu", "firemdl", "firemd",
		"landm", "jumpm", "reloadm", "switchm", "idles", "runs", "flys", "firesl", "firesul", "firesu",
		"firesdl", "firesd", "lands", "jumps", "reloads", "switchs", "jhit", "hitland", "gren", "grenprime",
		"idlea", "runa", "flya", "fireal", "fireaul", "fireau", "fireadl", "firead", "landa", "jumpa",
		"reloada", "switcha", "firealx", "fireaulx", "fireaux", "fireadlx", "fireadx", "cart", "kick", "roll",
		"idler", "runr", "flyr", "firerl", "firerul", "fireru", "firerdl", "firerd", "landr", "jumpr",
		"reloadr", "switchr", "idlex", "runx", "flyx", "firexl", "firexl", "firexu", "firexl", "firexd",
		"landx", "jumpx", "reloadx", "reloadx", "rollx", "kickx", "squat", "spawn", "suicide"
	};

	public int[] ammo = new int[16];

	public int rollFace;

	public Vector2 grenVec;

	private int[] weapOffset = new int[6] { 0, 12, 24, 40, 60, 72 };

	public int hp = 100;

	private bool aFire;

	public int lastHitBy;

	public int weapIdx;

	public int lastGren;

	public float reloadFrame;

	private float shootFrame;

	private float cockitFrame;

	private float shellsFrame;

	private float grenFrame;

	public int[] weapon = new int[4] { 17, -1, -1, -1 };

	public int curWeap;

	public float angle;

	public float freeze;

	public float shrink;

	public float rainbowed;

	public float vamped;

	public int vampOwner;

	public float fire;

	public int fireOwner;

	public float poison;

	public int poisonOwner;

	public Vector2 lastNetLoc;

	public Rectangle[] hitRects = new Rectangle[2]
	{
		default(Rectangle),
		default(Rectangle)
	};

	public bool recentShortUpdate;

	public int suit;

	private float beeFrame;

	public float jetFrame;

	public float jetGas = 1f;

	public float jetRecover;

	private float skunkFrame;

	private float teslaFrame;

	public int special;

	public int ID;

	public float spawnFrame;

	public float respawnFrame;

	public float dyingFrame;

	public bool gibbed;

	public Vector2 lastRadarLoc;

	public float deltaSinceUpdate;

	private Vector2[] trailVec;

	private float trailWriteFrame;

	public int metaWriteMode;

	private bool jetGo;

	private float jetMush;

	private float jetPackFrame;

	public Vector2 GetTrailVec(float latency)
	{
		int num = (int)(latency / 0.02f);
		if (num < trailVec.Length)
		{
			return trailVec[num];
		}
		return trailVec[trailVec.Length - 1];
	}

	public bool GetReloadBusy()
	{
		return true;
	}

	public void AddScore(int points)
	{
		score += points;
		if (Game1.netSession.GetPlayerOne() == ID)
		{
			Game1.zProfile.AddCareerScore(points);
		}
	}

	private void AddPoints(string msg, int points, float time)
	{
		AddScore(points);
		if (Game1.netSession.GetPlayerOne() == ID)
		{
			Game1.hud.AddPopup(msg, points, time);
		}
	}

	public void AddKill()
	{
		kills++;
		killStreak++;
		multikill++;
		multikillframe = 2f;
		AddScore(10);
		bool flag = Game1.netSession.GetPlayerOne() == ID;
		if (flag && perk[0] == 7 && weapon[curWeap] > -1)
		{
			ammo[WeaponCatalog.weapons[weapon[curWeap]].ammoType] += WeaponCatalog.weapons[weapon[curWeap]].maxClip;
			Sound.PlayCue("click1");
		}
		if (multikill > 1)
		{
			switch (multikill)
			{
			case 2:
				AddPoints((!flag) ? null : "Double Kill!", 10, 1f);
				break;
			case 3:
				AddPoints((!flag) ? null : "Triple Kill!", 50, 1f);
				break;
			case 4:
				AddPoints((!flag) ? null : "Quadruple Kill!", 100, 1f);
				break;
			case 5:
			case 6:
			case 7:
			case 8:
			case 9:
			case 10:
			case 11:
			case 12:
			case 13:
			case 14:
			case 15:
			case 16:
				AddPoints((!flag) ? null : "Multi Kill!", 200, 1f);
				break;
			}
		}
		if (killStreak >= 3)
		{
			int points = ((killStreak - 3) / 4 + 1) * 10;
			AddPoints((!flag) ? null : (killStreak + " Kill Streak!"), points, 1f);
		}
	}

	public int GetTeam()
	{
		if (GameState.gameType == 0)
		{
			return 0;
		}
		return team + 1;
	}

	public int GetMaxHP()
	{
		if (perk[2] == 5)
		{
			return 120;
		}
		return 100;
	}

	public void SetHitBy(int i)
	{
		hitByChar[i] = true;
	}

	public void DebuffHitby(int owner)
	{
		if (owner < 0 || hp < 0)
		{
			return;
		}
		if (owner == Game1.netSession.GetPlayerOne())
		{
			if (ID != owner)
			{
				Sound.PlayConfirm();
			}
		}
		else
		{
			hitType = 0;
			SetHitBy(owner);
		}
	}

	public Character(int ID, int keySrc, Vector2 loc)
	{
		trailVec = new Vector2[40];
		bodySec = new BodySec[2];
		for (int i = 0; i < bodySec.Length; i++)
		{
			bodySec[i] = new BodySec(i);
		}
		SetBodyAnim(GetAnimName(2), sync: true);
		special = 2;
		hitByChar = new bool[Game1.character.Length];
		charKeys = new CharKeys();
		this.keySrc = keySrc;
		this.ID = ID;
		this.loc = loc;
		if (this.keySrc == -9)
		{
			ai = null;
		}
		else if (this.keySrc < 0)
		{
			ai = new AI(ID);
		}
		else
		{
			ai = null;
		}
		Reset();
	}

	public bool PickupGren(int gren, int amt)
	{
		if (GameState.gameType == 4 && team == 1)
		{
			return false;
		}
		if (perk[1] == 6)
		{
			amt *= 2;
		}
		for (int i = 0; i < grenType.Length; i++)
		{
			if (grenType[i] != gren)
			{
				continue;
			}
			if (grenAmmo[i] < 5)
			{
				grenAmmo[i] += amt;
				if (grenAmmo[i] > 5)
				{
					grenAmmo[i] = 5;
				}
				return true;
			}
			return false;
		}
		for (int j = 0; j < grenType.Length; j++)
		{
			if (grenType[j] == -1 || grenAmmo[j] <= 0 || (j == 0 && charKeys.keyGrenade && charKeys.keyY) || (j == 1 && charKeys.keyGren2 && charKeys.keyY))
			{
				grenType[j] = gren;
				grenAmmo[j] = amt;
				return true;
			}
		}
		return false;
	}

	public bool Pickup(int w)
	{
		Weapon weapon = WeaponCatalog.weapons[w];
		_ = ammo[weapon.ammoType];
		ammo[weapon.ammoType] += weapon.maxClip * 5;
		if (perk[1] == 3)
		{
			ammo[weapon.ammoType] += weapon.maxClip * 5;
		}
		if (ammo[weapon.ammoType] > 999)
		{
			ammo[weapon.ammoType] = 999;
		}
		if (GameState.gameType == 4 && team == 1)
		{
			return false;
		}
		for (int i = 0; i < 4; i++)
		{
			if (this.weapon[i] <= -1)
			{
				continue;
			}
			if (WeaponCatalog.weapons[this.weapon[i]].isAkimbo && w == this.weapon[i] - 100)
			{
				return true;
			}
			if (this.weapon[i] != w)
			{
				continue;
			}
			if (WeaponCatalog.weapons[this.weapon[i]].canAkimbo)
			{
				int num = magazine[i] + WeaponCatalog.weapons[this.weapon[i]].maxClip;
				ammo[WeaponCatalog.weapons[this.weapon[i]].ammoType] -= WeaponCatalog.weapons[this.weapon[i]].maxClip;
				this.weapon[i] = w + 100;
				magazine[i] = num;
				if (ai != null)
				{
					curWeap = i;
				}
				return true;
			}
			return true;
		}
		int num2 = -1;
		for (int j = 0; j < 4; j++)
		{
			if (this.weapon[j] == -1 && num2 < 0)
			{
				num2 = j;
			}
		}
		if (num2 < 0)
		{
			num2 = curWeap;
		}
		if (Game1.netSession.GetPlayerOne() == ID && Game1.settings.autoSwitch && !(cockitFrame > 0f) && !(shellsFrame > 0f) && !(reloadFrame > 0f) && !(shootFrame > 0f) && !(grenFrame > 0f))
		{
			curWeap = num2;
			weapIdx = weapon.type;
			bodySec[1].SetAnim(GetAnimName(11), this, overRide: true);
			reloadFrame = 0.01f;
			splitAnim = true;
		}
		this.weapon[num2] = w;
		int num3 = WeaponCatalog.weapons[this.weapon[num2]].maxClip;
		if (num3 > 1 && perk[2] == 7)
		{
			num3 *= 3;
		}
		magazine[num2] = num3;
		ammo[WeaponCatalog.weapons[this.weapon[num2]].ammoType] -= magazine[num2];
		if (ai != null)
		{
			curWeap = num2;
		}
		return true;
	}

	public void GiveGoodies()
	{
		int num = Rand.GetRandomInt(0, 14);
		int num2 = -1;
		if (Game1.netSession.mutator == 15)
		{
			switch (num)
			{
			case 2:
				num = 7;
				break;
			case 9:
				num = 8;
				break;
			}
		}
		switch (num)
		{
		case 0:
			Pickup(24);
			num2 = 24;
			break;
		case 1:
			Pickup(39);
			num2 = 39;
			break;
		case 2:
			Pickup(40);
			num2 = 40;
			break;
		case 3:
			Pickup(32);
			num2 = 32;
			break;
		case 4:
			Pickup(31);
			num2 = 31;
			break;
		case 5:
			Pickup(48);
			num2 = 48;
			break;
		case 6:
			Pickup(49);
			num2 = 49;
			break;
		case 7:
			Pickup(47);
			num2 = 47;
			break;
		case 8:
			Pickup(45);
			num2 = 45;
			break;
		case 9:
			Pickup(46);
			num2 = 46;
			break;
		case 10:
			PickupGren(44, 2);
			num2 = 44;
			break;
		case 11:
			PickupGren(43, 1);
			num2 = 43;
			break;
		case 12:
			PickupGren(42, 3);
			num2 = 42;
			break;
		case 13:
			PickupGren(41, 3);
			num2 = 41;
			break;
		}
		if (num2 > -1 && Game1.netSession.GetPlayerOne() == ID)
		{
			Game1.hud.DoPickup(num2);
		}
	}

	public void Reset()
	{
		SetBodyAnim(GetAnimName(2), sync: true);
		state = 0;
		traj = default(Vector2);
		hp = 100;
		respawnFrame = 3f;
		dyingFrame = 0f;
		gibbed = false;
		vamped = 0f;
		fire = 0f;
		poison = 0f;
		shrink = 0f;
		freeze = 0f;
		rainbowed = 0f;
		fishFrame = 0f;
		if (ai != null)
		{
			ai = new AI(ID);
		}
		weapon[0] = 17;
		weapon[1] = (weapon[2] = (weapon[3] = -1));
		for (int i = 0; i < perk.Length; i++)
		{
			perk[i] = Game1.zProfile.ClassSet().perk[i];
		}
		if (perk[1] == 4)
		{
			weapon[0] = 117;
		}
		lastHitBy = ID;
		grenAmmo = new int[12];
		int[] array = new int[12];
		grenType = array;
		curWeap = 0;
		magazine[0] = WeaponCatalog.weapons[weapon[0]].maxClip;
		if (perk[2] == 8)
		{
			grenType[0] = 33;
			grenAmmo[0] = 3;
		}
		if (perk[2] == 7)
		{
			magazine[0] *= 3;
		}
		magazine[1] = 0;
		magazine[2] = 0;
		magazine[3] = 0;
		for (int j = 0; j < ammo.Length; j++)
		{
			ammo[j] = 0;
		}
		ammo[0] = 500;
		if (perk[0] == 3)
		{
			if (Game1.netSession.mutator == 15)
			{
				weapon[1] = 18;
				ammo[WeaponCatalog.weapons[weapon[1]].ammoType] = WeaponCatalog.weapons[weapon[1]].maxClip * 5;
				magazine[1] = WeaponCatalog.weapons[weapon[1]].maxClip;
			}
			else
			{
				weapon[1] = 29;
			}
		}
		if (perk[1] == 8)
		{
			int randomInt = Rand.GetRandomInt(0, 12);
			int num = 1;
			if (weapon[1] != -1)
			{
				num = 2;
			}
			switch (randomInt)
			{
			case 0:
				weapon[num] = 32;
				break;
			case 1:
				weapon[num] = 20;
				break;
			case 2:
				weapon[num] = 18;
				break;
			case 3:
				weapon[num] = 27;
				break;
			case 4:
				weapon[num] = 25;
				break;
			case 5:
				weapon[num] = 26;
				break;
			case 6:
				weapon[num] = 23;
				break;
			case 7:
				weapon[num] = 30;
				break;
			case 8:
				weapon[num] = 21;
				break;
			case 9:
				weapon[num] = 22;
				break;
			case 10:
				weapon[num] = 19;
				break;
			case 11:
				weapon[num] = 24;
				break;
			}
			if (WeaponCatalog.weapons[weapon[num]].ammoType != 0)
			{
				ammo[WeaponCatalog.weapons[weapon[num]].ammoType] = WeaponCatalog.weapons[weapon[num]].maxClip * 5;
			}
			magazine[num] = WeaponCatalog.weapons[weapon[num]].maxClip;
		}
		for (int k = 0; k < trailVec.Length; k++)
		{
			ref Vector2 reference = ref trailVec[k];
			reference = loc;
		}
		switch (Game1.netSession.mutator)
		{
		case 7:
		{
			weapon[0] = 22;
			weapon[1] = 23;
			for (int n = 0; n < 2; n++)
			{
				ammo[WeaponCatalog.weapons[weapon[n]].ammoType] = WeaponCatalog.weapons[weapon[n]].maxClip * 8;
				magazine[n] = WeaponCatalog.weapons[weapon[n]].maxClip;
			}
			grenType[0] = 33;
			grenAmmo[0] = 5;
			grenType[1] = 36;
			grenAmmo[1] = 1;
			perk[0] = 0;
			perk[1] = 7;
			perk[2] = 4;
			break;
		}
		case 9:
		{
			weapon[0] = 25;
			weapon[1] = 27;
			weapon[2] = 46;
			for (int num5 = 0; num5 < 3; num5++)
			{
				ammo[WeaponCatalog.weapons[weapon[num5]].ammoType] = WeaponCatalog.weapons[weapon[num5]].maxClip * 8;
				magazine[num5] = WeaponCatalog.weapons[weapon[num5]].maxClip;
			}
			grenType[0] = 34;
			grenAmmo[0] = 5;
			perk[0] = 0;
			perk[1] = 5;
			perk[2] = 2;
			break;
		}
		case 5:
		{
			grenType[0] = 33;
			grenAmmo[0] = 5;
			grenType[1] = 34;
			grenAmmo[1] = 3;
			grenType[2] = 38;
			grenAmmo[2] = 2;
			grenType[3] = 35;
			grenAmmo[3] = 2;
			grenType[4] = 37;
			grenAmmo[4] = 2;
			grenType[5] = 36;
			grenAmmo[5] = 1;
			grenType[6] = 41;
			grenAmmo[6] = 2;
			grenType[7] = 44;
			grenAmmo[7] = 2;
			grenType[8] = 42;
			grenAmmo[8] = 2;
			grenType[9] = 43;
			grenAmmo[9] = 1;
			perk[0] = 0;
			perk[1] = 7;
			perk[2] = 4;
			weapon[0] = 17;
			weapon[1] = -1;
			weapon[2] = -1;
			weapon[3] = -1;
			for (int num6 = 0; num6 < 1; num6++)
			{
				ammo[WeaponCatalog.weapons[weapon[num6]].ammoType] = WeaponCatalog.weapons[weapon[num6]].maxClip * 8;
				magazine[num6] = WeaponCatalog.weapons[weapon[num6]].maxClip;
			}
			break;
		}
		case 4:
		{
			weapon[0] = 147;
			weapon[1] = 145;
			weapon[2] = 19;
			for (int num8 = 0; num8 < 3; num8++)
			{
				ammo[WeaponCatalog.weapons[weapon[num8]].ammoType] = WeaponCatalog.weapons[weapon[num8]].maxClip * 8;
				magazine[num8] = WeaponCatalog.weapons[weapon[num8]].maxClip;
			}
			perk[0] = 9;
			perk[1] = 3;
			perk[2] = 4;
			break;
		}
		case 10:
		{
			weapon[0] = 30;
			for (int num2 = 0; num2 < 1; num2++)
			{
				ammo[WeaponCatalog.weapons[weapon[num2]].ammoType] = WeaponCatalog.weapons[weapon[num2]].maxClip * 8;
				magazine[num2] = WeaponCatalog.weapons[weapon[num2]].maxClip;
			}
			weapon[1] = -1;
			grenType[0] = 0;
			grenAmmo[0] = 0;
			perk[0] = 0;
			perk[1] = 5;
			perk[2] = 9;
			break;
		}
		case 2:
		{
			weapon[0] = 117;
			weapon[1] = 18;
			weapon[2] = 23;
			weapon[3] = 29;
			for (int num7 = 0; num7 < 4; num7++)
			{
				ammo[WeaponCatalog.weapons[weapon[num7]].ammoType] = WeaponCatalog.weapons[weapon[num7]].maxClip * 8;
				magazine[num7] = WeaponCatalog.weapons[weapon[num7]].maxClip;
			}
			grenType[0] = 33;
			grenAmmo[0] = 5;
			perk[0] = 4;
			perk[1] = 3;
			perk[2] = 5;
			break;
		}
		case 8:
		{
			weapon[0] = 21;
			weapon[1] = 40;
			weapon[2] = 30;
			for (int num3 = 0; num3 < 3; num3++)
			{
				ammo[WeaponCatalog.weapons[weapon[num3]].ammoType] = WeaponCatalog.weapons[weapon[num3]].maxClip * 8;
				magazine[num3] = WeaponCatalog.weapons[weapon[num3]].maxClip;
			}
			grenType[0] = 42;
			grenAmmo[0] = 5;
			perk[0] = 6;
			perk[1] = 5;
			perk[2] = 9;
			break;
		}
		case 3:
		{
			weapon[0] = 29;
			weapon[1] = -1;
			for (int m = 0; m < 1; m++)
			{
				ammo[WeaponCatalog.weapons[weapon[m]].ammoType] = WeaponCatalog.weapons[weapon[m]].maxClip * 8;
				magazine[m] = WeaponCatalog.weapons[weapon[m]].maxClip;
			}
			perk[0] = 9;
			perk[1] = 0;
			perk[2] = 6;
			break;
		}
		case 6:
			grenType[0] = 43;
			grenAmmo[0] = 5;
			if (perk[2] == 8)
			{
				perk[2] = 4;
			}
			break;
		case 11:
		{
			weapon[0] = 31;
			weapon[1] = 32;
			weapon[2] = 40;
			for (int num4 = 0; num4 < 3; num4++)
			{
				ammo[WeaponCatalog.weapons[weapon[num4]].ammoType] = WeaponCatalog.weapons[weapon[num4]].maxClip * 8;
				magazine[num4] = WeaponCatalog.weapons[weapon[num4]].maxClip;
			}
			grenType[0] = 42;
			grenAmmo[0] = 2;
			grenType[1] = 35;
			grenAmmo[1] = 2;
			perk[0] = 5;
			if (perk[2] == 8)
			{
				perk[2] = 4;
			}
			break;
		}
		case 12:
			grenType[0] = 41;
			grenAmmo[0] = 5;
			if (perk[2] == 8)
			{
				perk[2] = 4;
			}
			break;
		case 13:
		{
			weapon[0] = 48;
			weapon[1] = 39;
			weapon[2] = 24;
			for (int l = 0; l < 3; l++)
			{
				ammo[WeaponCatalog.weapons[weapon[l]].ammoType] = WeaponCatalog.weapons[weapon[l]].maxClip * 8;
				magazine[l] = WeaponCatalog.weapons[weapon[l]].maxClip;
			}
			grenType[0] = 43;
			grenAmmo[0] = 1;
			grenType[1] = 44;
			grenAmmo[1] = 2;
			perk[0] = 7;
			perk[1] = 1;
			if (perk[2] == 8)
			{
				perk[2] = 4;
			}
			break;
		}
		case 16:
			GiveGoodies();
			GiveGoodies();
			break;
		}
		if (GameState.gameType == 4 && team == 1)
		{
			weapon[0] = 28;
			weapon[1] = -1;
			weapon[2] = -1;
			weapon[3] = -1;
			for (int num9 = 0; num9 < grenType.Length; num9++)
			{
				grenType[num9] = 0;
				grenAmmo[num9] = 0;
			}
		}
		suit = 0;
		spawnFrame = 3f;
		gibbed = false;
		face = Rand.GetRandomInt(0, 2);
		SetBodyAnim(animList[87], sync: true);
		weapIdx = WeaponCatalog.weapons[weapon[curWeap]].type;
	}

	public string GetAnimName(int t)
	{
		switch (t + weapOffset[weapIdx])
		{
		case 43:
		case 44:
		case 45:
		case 46:
		case 47:
			if (aFire)
			{
				t += 9;
				aFire = false;
			}
			else
			{
				aFire = true;
			}
			break;
		}
		return animList[t + weapOffset[weapIdx]];
	}

	public void LevelUp()
	{
		if (Game1.zProfile.level < 100)
		{
			Game1.zProfile.careerScore = Leveling.level[Game1.zProfile.level].score;
			Game1.zProfile.AddCareerScore(10L);
		}
	}

	public void LevelMax()
	{
		if (Game1.zProfile.level < 90)
		{
			Game1.zProfile.level = 88;
			Game1.zProfile.unlocks.UpdateUnlocks();
			Game1.zProfile.careerScore = Leveling.level[89].score;
			Game1.zProfile.AddCareerScore(10L);
		}
	}

	public void Update(GameMap map, Character[] c, float fTime)
	{
		if (ai != null)
		{
			ai.ResetHitWall();
		}
		if (Game1.netSession.GetNetworkOwner(ID))
		{
			radarTraj = traj;
		}
		if (killRefreshFrame > 0f)
		{
			killRefreshFrame -= fTime;
		}
		if (multikillframe > 0f)
		{
			multikillframe -= fTime;
			if (multikillframe <= 0f)
			{
				multikill = 0;
			}
		}
		if (noStick > 0f)
		{
			noStick -= fTime;
		}
		if (Game1.pMan.GetChronod(loc))
		{
			fTime *= 0.1f;
		}
		if (Game1.netSession.GetPlayerOne() == ID && DebugManager.godMode)
		{
			hp = GetMaxHP();
		}
		if (DebugManager.jumpToLevUp && Game1.netSession.GetPlayerOne() == ID && charKeys.keyA)
		{
			LevelUp();
		}
		if (loc.X > 16256f)
		{
			loc.X = 16256f;
		}
		if (loc.X < 256f)
		{
			loc.X = 256f;
		}
		if (fishFrame >= 1f)
		{
			float num = fishFrame;
			fishFrame += fTime;
			if (!Game1.fish[ID].exists)
			{
				Game1.fish[ID].exists = true;
				Game1.fish[ID].face = Rand.GetRandomInt(0, 2);
			}
			float num2 = 2.1f;
			if (fishFrame >= num2 && num < num2)
			{
				Game1.fish[ID].anim = 1;
				Game1.fish[ID].key = 0;
				Game1.fish[ID].animFrame = 0f;
			}
			if (fishFrame < 3f)
			{
				Game1.fish[ID].loc = loc + new Vector2((Game1.fish[ID].face == 0) ? 1f : (-1f), 0.7f) * (3f - fishFrame) * 200f;
				Game1.fish[ID].traj = new Vector2((Game1.fish[ID].face == 0) ? (-1f) : 1f, -0.7f) * 200f;
			}
		}
		if (Game1.netSession.GetNetworkOwner(ID))
		{
			latency = 0f;
		}
		trailWriteFrame += fTime;
		if (trailWriteFrame > 0.02f)
		{
			trailWriteFrame -= 0.02f;
			for (int num3 = trailVec.Length - 1; num3 > 0; num3--)
			{
				ref Vector2 reference = ref trailVec[num3];
				reference = trailVec[num3 - 1];
			}
			ref Vector2 reference2 = ref trailVec[0];
			reference2 = loc;
		}
		if (Game1.netSession.GetPlayerOne() == ID)
		{
			if (Game1.zProfile.clanTag != null)
			{
				if (Game1.zProfile.clanTag.Length > 0)
				{
					string text = Game1.zProfile.clanTag.ToString();
					clanChar[0] = text[0];
					if (text.Length > 1)
					{
						clanChar[1] = text[1];
					}
					else
					{
						clanChar[1] = '\0';
					}
					if (text.Length > 2)
					{
						clanChar[2] = text[2];
					}
					else
					{
						clanChar[2] = '\0';
					}
				}
				else
				{
					clanChar[0] = '\0';
					clanChar[1] = '\0';
					clanChar[2] = '\0';
				}
			}
			else
			{
				clanChar[0] = '\0';
				clanChar[1] = '\0';
				clanChar[2] = '\0';
			}
		}
		if (weapon[curWeap] > -1)
		{
			try
			{
				weapIdx = WeaponCatalog.weapons[weapon[curWeap]].type;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.StackTrace);
			}
		}
		if (!(reloadFrame > 0f))
		{
			int num4 = curWeap;
			if (charKeys.keyDLeft)
			{
				curWeap = 0;
			}
			if (charKeys.keyDRight)
			{
				curWeap = 1;
			}
			if (charKeys.keyDUp)
			{
				curWeap = 2;
			}
			if (charKeys.keyDDown)
			{
				curWeap = 3;
			}
			if (charKeys.keyY)
			{
				for (int i = 0; i < 4; i++)
				{
					curWeap = (curWeap + 1) % 4;
					if (weapon[curWeap] > -1)
					{
						break;
					}
				}
			}
			if (weapon[curWeap] == -1)
			{
				curWeap = num4;
			}
			if (num4 != curWeap && (loc - Scroll.scroll).LengthSquared() < 250000f)
			{
				Sound.PlayCue("click1");
				weapIdx = WeaponCatalog.weapons[weapon[curWeap]].type;
				bodySec[1].SetAnim(GetAnimName(11), this, overRide: true);
				reloadFrame = 0.01f;
				splitAnim = true;
				if (Game1.netSession.GetPlayerOne() == ID)
				{
					int imgIdx = WeaponCatalog.weapons[weapon[curWeap]].imgIdx;
					imgIdx %= 64;
					imgIdx += 17;
					Game1.hud.DoName(imgIdx);
				}
			}
		}
		if (Game1.netSession.netType == 3 || Game1.netSession.netType == 2)
		{
			deltaSinceUpdate += fTime;
		}
		if (killedBy > -1)
		{
			KillManager.DoKill(killedBy, ID, killType);
			killedBy = -1;
		}
		if (rollFrame > 0f)
		{
			rollFrame -= fTime;
		}
		if (spawnFrame > 0f)
		{
			angle = 0f;
			gibbed = false;
			freeze = 0f;
			spawnFrame -= fTime;
			bodySec[0].SetAnim(animList[87], this);
			jetGas = 1.25f;
			if (perk[0] == 0)
			{
				jetGas = 1.75f;
			}
			if (spawnFrame <= 0f)
			{
				respawnFrame = 1f;
				bodySec[0].SetAnim(GetAnimName(2), this, overRide: true);
				state = 0;
				traj = default(Vector2);
				Game1.pterodactyl[ID].exists = true;
				Game1.pterodactyl[ID].anim = 0;
				Game1.pterodactyl[ID].traj = new Vector2(((face == 1) ? 1f : (-1f)) * 300f, 0f);
			}
		}
		if (Game1.netSession.GetPlayerOne() == ID)
		{
			headTex = Game1.zProfile.Class().headTex;
			torsoTex = Game1.zProfile.Class().torsoTex;
			legsTex = Game1.zProfile.Class().legsTex;
			hatTex = Game1.zProfile.Class().hatTex;
			bodyType = Game1.zProfile.ClassSet().bodyType;
			skinTex = Game1.zProfile.Class().skinTex;
			jetpack = Game1.zProfile.Class().jetpack;
		}
		for (int j = 0; j < grenAmmo.Length; j++)
		{
			if (grenAmmo[j] > 99)
			{
				grenAmmo[j] = 99;
			}
		}
		if (capFixFrame > 0f)
		{
			capFixFrame -= fTime;
		}
		if (respawnFrame > 0f)
		{
			respawnFrame -= fTime;
		}
		if (hp < 0 && perk[2] == 2)
		{
			for (int k = 0; k < 2; k++)
			{
				if (grenAmmo[k] > 0)
				{
					int type = 11;
					switch (grenType[k])
					{
					case 33:
						type = 11;
						break;
					case 34:
						type = 9;
						break;
					case 38:
						type = 29;
						break;
					case 35:
						type = 14;
						break;
					case 37:
						type = 12;
						break;
					case 36:
						type = 13;
						break;
					}
					Game1.pMan.AddParticle(type, loc + new Vector2(0f, -60f), default(Vector2), 0f, 0, ID);
					grenAmmo[0] = 0;
					grenAmmo[1] = 0;
					break;
				}
			}
		}
		if (dyingFrame >= 1f)
		{
			dyingFrame += fTime;
			if (dyingFrame >= 5f && (Game1.netSession.GetPlayerOne() != ID || !Game1.menu.IsActive() || ai != null))
			{
				if (Game1.netSession.GetPlayerOne() == ID)
				{
					SetNewClass();
				}
				if (ai != null)
				{
					int num5 = 0;
					int num6 = 0;
					for (int l = 0; l < c.Length; l++)
					{
						if (c[l] != null)
						{
							switch (c[l].GetTeam())
							{
							case 2:
								num5++;
								break;
							case 1:
								num6++;
								break;
							}
						}
					}
					if (num6 > num5 + 1 && team == 0)
					{
						team = 1;
					}
					if (num5 > num6 + 1 && team == 1)
					{
						team = 0;
					}
				}
				map.GetSpawn(0, this);
			}
		}
		if (timeSinceHit < 5f)
		{
			timeSinceHit += fTime;
		}
		else if (hp < GetMaxHP() && hp >= 0)
		{
			while (timeSinceHit >= 5f)
			{
				timeSinceHit -= 0.02f;
				if (perk[2] == 4)
				{
					hp++;
				}
				hp++;
				if (hp > GetMaxHP())
				{
					hp = GetMaxHP();
				}
			}
		}
		if (GameState.gameType == 2 && hp < 0 && (Game1.netSession.redFlagState == ID || Game1.netSession.blueFlagState == ID) && Game1.netSession.GetNetworkOwner(ID))
		{
			if (Game1.netSession.IsHost())
			{
				Game1.hud.AddMessage(KillManager.GetPlayerName(ID), Message.msgDroppedFlag, GetTeam(), 0, -1);
			}
			if (Game1.netSession.redFlagState == ID)
			{
				Game1.netSession.redFlagState = 200;
			}
			if (Game1.netSession.blueFlagState == ID)
			{
				Game1.netSession.blueFlagState = 200;
			}
		}
		if (freeze > 0f && hp >= 0)
		{
			freeze -= fTime;
			for (int m = 0; m < 2; m++)
			{
				if (Rand.CointToss(0.5f))
				{
					Game1.pMan.AddParticle(38, Rand.GetRandomVec2(hitRects[m].X, hitRects[m].Right, hitRects[m].Top, hitRects[m].Bottom), new Vector2(0f, 30f), Rand.GetRandomFloat(0.2f, 0.5f), 0, 0);
				}
			}
		}
		if (rainbowed > 0f && hp >= 0)
		{
			rainbowed -= fTime;
			for (int n = 0; n < 2; n++)
			{
				if (Rand.CointToss(0.5f))
				{
					Game1.pMan.AddParticle(56, Rand.GetRandomVec2(hitRects[n].X, hitRects[n].Right, hitRects[n].Top, hitRects[n].Bottom), new Vector2(0f, 30f), Rand.GetRandomFloat(0.2f, 0.5f), 0, 0);
				}
			}
		}
		if (shrink > 0f)
		{
			shrink -= fTime;
			if (hp >= 0)
			{
				for (int num7 = 0; num7 < Game1.character.Length; num7++)
				{
					if (Game1.character[num7] != null && num7 != ID && HitManager.GetHostile(num7, ID) && Game1.character[num7].hp >= 0 && Game1.character[num7].shrink <= 0f && (Game1.character[num7].charKeys.keyLeft || Game1.character[num7].charKeys.keyRight) && (Game1.character[num7].loc - loc).Length() < 30f)
					{
						hp = -50;
						killedBy = num7;
						killType = 6;
						StartKill(default(Vector2));
					}
				}
			}
		}
		if (suit == 7 || suit == 100)
		{
			fire = 0f;
		}
		if (fire > 0f)
		{
			float num8 = fire;
			fire -= fTime;
			if ((int)(num8 * 30f) != (int)(fire * 30f))
			{
				DebuffHitby(fireOwner);
				killType = 2;
				hp--;
				timeSinceHit = 0f;
				if (hp == -1)
				{
					killedBy = fireOwner;
					HitManager.DoKill(this);
				}
			}
		}
		if (vamped > 0f)
		{
			float num9 = vamped;
			vamped -= fTime;
			if ((int)(num9 * 10f) != (int)(vamped * 10f))
			{
				killType = 16;
				hp -= 4;
				if (vampOwner > -1 && Game1.character[vampOwner] != null)
				{
					DebuffHitby(vampOwner);
					Game1.character[vampOwner].hp += 4;
					if (Game1.character[vampOwner].hp > Game1.character[vampOwner].GetMaxHP())
					{
						Game1.character[vampOwner].hp = Game1.character[vampOwner].GetMaxHP();
					}
				}
				Game1.pMan.AddParticle(6, loc + Rand.GetRandomVec2(-30f, 30f, -80f, -20f), default(Vector2), 1f, 0, 0);
				if ((loc - Scroll.scroll).LengthSquared() < 90000f)
				{
					Sound.PlayCue("hit2");
				}
				timeSinceHit = 0f;
				if (hp < 0)
				{
					vamped = 0f;
					killedBy = vampOwner;
					HitManager.DoKill(this);
				}
			}
		}
		if (kickFrame > 0f)
		{
			kickFrame -= fTime;
			if (kickFrame < 0f)
			{
				traj = default(Vector2);
				kickRecoverFrame = 0.3f;
				if (bodySec[0].animName == "firexu" || bodySec[0].animName == "firexl" || bodySec[0].animName == "firexd")
				{
					if ((loc - Scroll.scroll).LengthSquared() < 90000f)
					{
						Quake.SetQuake(0.3f);
					}
					kickRecoverFrame = WeaponCatalog.weapons[weapon[curWeap]].reloadTime;
				}
			}
		}
		else if (kickRecoverFrame > 0f)
		{
			kickRecoverFrame -= fTime;
			if (perk[0] == 6)
			{
				kickRecoverFrame -= fTime * 0.5f;
			}
		}
		if (poison > 0f)
		{
			float num10 = poison;
			poison -= fTime;
			killType = 4;
			if (state == 4)
			{
				poison -= fTime * 2f;
			}
			else if ((int)(num10 * 10f) != (int)(poison * 10f))
			{
				DebuffHitby(poisonOwner);
				hp--;
				timeSinceHit = 0f;
				if (hp == -1)
				{
					killedBy = poisonOwner;
					HitManager.DoKill(this);
				}
			}
		}
		if (hp < 0 && bodySec[0].animName != "jhit" && bodySec[0].animName != "hitland")
		{
			HitManager.DoKill(this);
		}
		if (perk[2] == 0)
		{
			if (poison > 0f)
			{
				poison -= fTime * 1.5f;
			}
			if (freeze > 0f)
			{
				freeze -= fTime * 1.5f;
			}
			if (fire > 0f)
			{
				fire -= fTime * 1.5f;
			}
		}
		if ((state == 1 || state == 0) && charKeys.keyUp && (GameState.gameType != 4 || team != 1) && ai == null && Game1.netSession.GetNetworkOwner(ID) && Game1.settings.upToJetpack)
		{
			charKeys.keyJetpack = true;
			charKeys.keyJump = true;
		}
		if (perk[1] == 1)
		{
			if (state == 2 && charKeys.keyRight)
			{
				charKeys.keyJump = true;
			}
			if (state == 3 && charKeys.keyLeft)
			{
				charKeys.keyJump = true;
			}
			if (state == 4 && charKeys.keyDown)
			{
				charKeys.keyJump = true;
			}
		}
		if (GameState.gameType == 4 && team == 1)
		{
			charKeys.keyJetpack = false;
		}
		if (bodySec[0].animName == "cart")
		{
			float num11 = 0f;
			switch (state)
			{
			case 2:
				num11 = 1.57f;
				break;
			case 3:
				num11 = 4.71f;
				break;
			case 4:
				num11 = 3.14f;
				break;
			}
			if (angle > num11)
			{
				angle -= fTime * 20f;
				if (angle < num11)
				{
					angle = num11;
					bodySec[0].SetAnim(GetAnimName(8), this);
					bodySec[0].endAction = 1;
					traj /= 2f;
				}
			}
			if (angle < num11)
			{
				angle += fTime * 20f;
				if (angle > num11)
				{
					angle = num11;
					bodySec[0].SetAnim(GetAnimName(8), this);
					bodySec[0].endAction = 1;
					traj /= 2f;
				}
			}
		}
		else
		{
			switch (state)
			{
			default:
				if (bodySec[0].animName == "jhit")
				{
					angle = Trig.GetAngle(default(Vector2), traj);
					angle -= 1.57f;
				}
				else if (!(bodySec[0].animName == "kick") && !(bodySec[0].animName == "kickx") && !(bodySec[0].animName == "firexu") && !(bodySec[0].animName == "firexl"))
				{
					FixAngle(fTime);
				}
				break;
			case 2:
			case 3:
			case 4:
				break;
			}
		}
		ammo[0] = 500;
		scale = 0.48f;
		if (GameState.mode == 1)
		{
			Move(map, fTime);
			if ((drawVec - loc).LengthSquared() < 40000f)
			{
				drawVec += (loc - drawVec) * fTime * 20f;
			}
			else
			{
				drawVec = loc;
			}
			drawTraj += (traj - drawTraj) * fTime * 1f;
		}
		try
		{
			if (keySrc > -1 && Game1.mainPlayerIndex > -1)
			{
				// Merge the real controller (if any) with a synthetic keyboard/mouse
				// GamePadState (KeyboardMouseInput.cs) so both work at once - see that
				// file for the exact bindings. `loc` is this character's own world
				// position, needed to turn the mouse cursor into a right-stick aim
				// direction relative to where the character actually is on screen.
				GamePadState realPad = GamePad.GetState((PlayerIndex)Game1.mainPlayerIndex, GamePadDeadZone.Circular);
				GamePadState kbmPad = KeyboardMouseInput.GetGameplayState(loc);
				charKeys.Update(KeyboardMouseInput.Merge(realPad, kbmPad), this);
				level = Game1.zProfile.level;
			}
			else if (ai != null)
			{
				ai.Update(c, map);
			}
		}
		catch (Exception ex2)
		{
			Console.WriteLine(ex2.StackTrace);
		}
		if (splitAnim)
		{
			for (int num12 = 0; num12 < 2; num12++)
			{
				bodySec[num12].Update(this, fTime);
			}
		}
		else
		{
			bodySec[0].Update(this, fTime);
		}
	}

	internal void SetNewClass()
	{
		Game1.zProfile.UpdateClass();
		if (Game1.zProfile.defaultTeam == 0)
		{
			if (Game1.zProfile.ClassSet().defaultTeam != team)
			{
				team = Game1.zProfile.ClassSet().defaultTeam;
			}
		}
		else
		{
			team = Game1.zProfile.defaultTeam - 1;
		}
		for (int i = 0; i < perk.Length; i++)
		{
			perk[i] = Game1.zProfile.ClassSet().perk[i];
		}
		bodyType = Game1.zProfile.ClassSet().bodyType;
		skinTex = Game1.zProfile.Class().skinTex;
		hatTex = Game1.zProfile.Class().hatTex;
		legsTex = Game1.zProfile.Class().legsTex;
		torsoTex = Game1.zProfile.Class().torsoTex;
		headTex = Game1.zProfile.Class().headTex;
		jetpack = Game1.zProfile.Class().jetpack;
		if (GameState.gameType == 4 && team == 1)
		{
			perk[0] = (perk[1] = (perk[2] = -1));
		}
	}

	private void FixAngle(float fTime)
	{
		if (netJetpack && !submerged && hp >= 0)
		{
			return;
		}
		if (angle > 0f)
		{
			angle -= fTime * angle * 2f + fTime * 10f;
			if (angle < 0f)
			{
				angle = 0f;
			}
		}
		if (angle < 0f)
		{
			angle += fTime * (0f - angle) * 2f + fTime * 10f;
			if (angle > 0f)
			{
				angle = 0f;
			}
		}
	}

	private bool GetCanWall()
	{
		if (special == 2)
		{
			switch (bodySec[0].animName)
			{
			case "flyw":
			case "flym":
			case "flyr":
			case "flya":
			case "flys":
			case "flyx":
			case "idlew":
			case "idlem":
			case "idlea":
			case "idles":
			case "idler":
			case "idlex":
			case "runw":
			case "runa":
			case "runm":
			case "runs":
			case "runr":
			case "runx":
				return true;
			default:
				return false;
			}
		}
		return false;
	}

	private void Move(GameMap map, float fTime)
	{
		if (spawnFrame > 0f)
		{
			return;
		}
		if (dyingFrame >= 2f)
		{
			gibbed = true;
		}
		if (gibbed)
		{
			return;
		}
		Vector2 vector = loc;
		for (int i = 0; i < 3; i++)
		{
			Vector2 vector2 = loc + new Vector2(-52f + (float)i * 52f, 0f);
			int num = (int)(vector2.X / 64f);
			int num2 = (int)((vector2.Y - 16f) / 32f);
			if (num <= 0 || num >= 255 || num2 <= 0 || num2 >= 255)
			{
				continue;
			}
			if (map.node[num, num2] > -1)
			{
				lastNode = map.node[num, num2];
			}
			if (map.mapEntity[num, num2] > -1)
			{
				_ = map.mapEntity[num, num2];
				try
				{
					map.entity[map.mapEntity[num, num2]].Pickup(this);
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.StackTrace);
				}
			}
		}
		int num3 = (int)(vector.X / 64f);
		int num4 = (int)((vector.Y - 16f) / 32f);
		bool flag = submerged;
		if (num3 >= 0 && num4 >= 0 && num3 < 256 && num4 < 256)
		{
			submerged = false;
			if (num4 >= map.water.waterLevel && map.water.water[num3, num4])
			{
				submerged = true;
			}
		}
		if (submerged)
		{
			submergedFrame += fTime;
			fire = 0f;
			if (submergedFrame > 60f)
			{
				killType = 14;
				hp--;
				timeSinceHit = 0f;
				if (hp == -1)
				{
					killedBy = ID;
					HitManager.DoKill(this);
				}
			}
			if (!flag)
			{
				if ((loc - Scroll.scroll).LengthSquared() < 490000f)
				{
					Sound.PlayCue("splash");
				}
				for (int j = 0; j < 24; j++)
				{
					Game1.pMan.AddParticle(42, Rand.GetRandomVec2((float)num3 * 64f, (float)(num3 + 1) * 64f, (float)num4 * 32f - 5f, (float)num4 * 32f + 5f), Rand.GetRandomVec2(-100f, 100f, -500f, 0f), Rand.GetRandomFloat(0.6f, 2f), 0, 0);
				}
			}
		}
		else
		{
			submergedFrame = 0f;
		}
		jetVal = 0f;
		bool flag2 = jetGo;
		jetGo = false;
		bool flag3 = false;
		if (Game1.netSession.GetNetworkOwner(ID))
		{
			if (charKeys.keyJetpack && hp >= 0 && jetGas > 0f && jetRecover <= 0f && shrink <= 0f)
			{
				flag3 = true;
			}
		}
		else
		{
			flag3 = netJetpack;
			if (netJetpack)
			{
				jetGas = 1f;
				jetRecover = 0f;
			}
		}
		if (Game1.netSession.GetNetworkOwner(ID))
		{
			netJetpack = false;
		}
		if (jetMush > 0f)
		{
			jetMush -= fTime;
		}
		if (flag3 || jetMush > 0f)
		{
			if (state == 0 && !(bodySec[0].animName == "jhit"))
			{
				float num5 = -150f;
				if (perk[1] == 5)
				{
					num5 = -210f;
				}
				if (freeze > 0f)
				{
					num5 = 50f;
				}
				float num6 = charKeys.jumpPower;
				if (charKeys.keyJetpack && num6 < 0.3f)
				{
					num6 = 1f;
				}
				num6 *= 1.25f;
				if (num6 > 1f)
				{
					num6 = 1f;
				}
				if (traj.Y > num5 && flag3)
				{
					traj.Y -= fTime * Game1.gravity * 2f * num6;
				}
				if (flag3)
				{
					jetMush = 0.2f;
				}
				float num7 = jetFrame;
				jetFrame += fTime * 10f;
				float num8 = traj.X / 900f;
				if (shootFrame <= 0f)
				{
					if (traj.X < 0f)
					{
						face = 0;
					}
					else if (traj.X > 0f)
					{
						face = 1;
					}
				}
				angle += (num8 - angle) * fTime * 20f;
				jetVal = angle;
				netJetpack = true;
				if ((int)(num7 * 10f) != (int)(jetFrame * 10f))
				{
					Vector2 vector3 = new Vector2(20f, 20f);
					if (face == 1)
					{
						vector3.X = 0f - vector3.X;
					}
					Vector2 vector4 = (Game1.netSession.GetNetworkOwner(ID) ? loc : drawVec);
					vector4 = vector4 - new Vector2(0f, 40f) + new Vector2((float)Math.Cos(angle) * vector3.X, (float)Math.Sin(angle) * vector3.X) + new Vector2((float)Math.Cos(angle + 1.57f) * vector3.Y, (float)Math.Sin(angle + 1.57f) * vector3.Y);
					Vector2 randomVec = Rand.GetRandomVec2(-20f, 20f, 100f, 200f);
					if (!flag3)
					{
						randomVec *= jetMush * 5f;
					}
					if (BodyCatalog.GetJetFire(jetpack) == 1)
					{
						if (submerged)
						{
							Game1.pMan.AddParticle(50, vector4, randomVec * 1.3f + traj / 8f, (flag3 ? 1f : (jetMush * 5f)) * Rand.GetRandomFloat(0.1f, 0.2f), 0, ID);
						}
						else
						{
							switch (jetpack)
							{
							case 0:
								Game1.pMan.AddParticle(1, vector4, randomVec + traj / 4f, (flag3 ? 1f : (jetMush * 5f)) * Rand.GetRandomFloat(0.3f, 0.5f), 0, ID);
								break;
							case 5:
								Game1.pMan.AddParticle(49, vector4, randomVec + traj / 4f, (flag3 ? 1f : (jetMush * 5f)) * Rand.GetRandomFloat(0.3f, 0.5f) * 0.2f, 0, ID);
								if (Rand.CointToss(0.4f))
								{
									Game1.pMan.AddParticle(38, vector4, randomVec + traj / 4f, (flag3 ? 1f : (jetMush * 5f)) * Rand.GetRandomFloat(0.3f, 0.5f) * 0.1f, 0, ID);
								}
								break;
							case 7:
								Game1.pMan.AddParticle(56, vector4, randomVec + traj / 4f, (flag3 ? 1f : (jetMush * 5f)) * Rand.GetRandomFloat(0.3f, 0.5f) * 0.1f, 0, ID);
								if (Rand.CointToss(0.4f))
								{
									Game1.pMan.AddParticle(38, vector4, randomVec + traj / 4f, (flag3 ? 1f : (jetMush * 5f)) * Rand.GetRandomFloat(0.3f, 0.5f) * 0.1f, 0, ID);
								}
								break;
							}
						}
						switch (jetpack)
						{
						case 0:
						case 7:
							Game1.postGlowMgr.Add(Scroll.GetLoc(vector4), 1f, 0.5f, 0.3f, 0.1f, 1f);
							break;
						case 5:
							Game1.postGlowMgr.Add(Scroll.GetLoc(vector4), 0.5f, 0.9f, 1f, 0.1f, 1f);
							break;
						}
					}
				}
				jetGo = true;
				if ((int)(num7 * 1.6f) != (int)(jetFrame * 1.6f) && BodyCatalog.GetJetFire(jetpack) == 1 && (loc - Scroll.scroll).LengthSquared() < 490000f)
				{
					if (jetpack == 7)
					{
						Sound.PlayCue("rainjet");
					}
					else
					{
						Sound.PlayCue("jet");
					}
				}
				if (jetFrame > 1f)
				{
					jetFrame--;
				}
				if (flag3 && Game1.netSession.mutator != 14)
				{
					jetGas -= fTime * 0.3f * num6;
					if (jetGas < 0f)
					{
						jetRecover = 0.5f;
					}
				}
			}
		}
		else
		{
			if (Game1.netSession.mutator == 14)
			{
				jetRecover = 0f;
			}
			if (jetRecover > 0f)
			{
				jetRecover -= fTime;
				jetGas += fTime * 0.3f;
			}
			else
			{
				jetGas += fTime * 0.3f;
				if (perk[2] == 9)
				{
					jetGas += fTime * 0.1f;
				}
				float num9 = 1.25f;
				if (perk[0] == 0)
				{
					num9 = 1.75f;
				}
				if (jetGas > num9)
				{
					jetGas = num9;
				}
				if (Game1.netSession.mutator == 14)
				{
					jetGas = num9;
				}
			}
		}
		if (jetGo && !flag2 && (loc - Scroll.scroll).LengthSquared() < 490000f && BodyCatalog.GetJetFire(jetpack) == 1)
		{
			Sound.PlayCue("jetstart");
		}
		if (jetGo)
		{
			jetPackFrame += Game1.frameTime * 2.5f;
			if (jetPackFrame >= 0.4f)
			{
				jetPackFrame -= 0.4f;
				if ((loc - Scroll.scroll).LengthSquared() < 490000f)
				{
					switch (BodyCatalog.GetJetFire(jetpack))
					{
					case 2:
						Sound.PlayCue("wing");
						break;
					case 3:
						Sound.PlayCue("beewing");
						break;
					}
				}
			}
		}
		else
		{
			jetPackFrame = 0f;
		}
		if (loc.Y > 6400f && submerged && fishFrame <= 0f)
		{
			fishFrame = 1f;
			Game1.fish[ID].exists = false;
		}
		if (loc.Y > 7872f)
		{
			if (hp >= 0 && dyingFrame < 1f)
			{
				dyingFrame = 1f;
				KillManager.DoKill(lastHitBy, ID, killType);
			}
			if (dyingFrame < 1f)
			{
				dyingFrame = 1f;
			}
			hp = -1;
			SetBodyAnim("jhit");
		}
		if (freeze > 0.2f)
		{
			if (charKeys.keyKick)
			{
				charKeys.keyKick = false;
			}
			if (WeaponCatalog.weapons[weapon[curWeap]].type == 5)
			{
				charKeys.shootVec = default(Vector2);
			}
		}
		if (rainbowed > 0.2f && (float)hp >= 0f)
		{
			charKeys.ClearKeys();
			if (state != 4)
			{
				state = 0;
				traj = new Vector2(0f, -100f);
				SetBodyAnim(GetAnimName(2));
			}
		}
		float num10 = 1f;
		if (perk[1] == 0)
		{
			num10 = 1.2f;
		}
		num10 *= 1.2f;
		float num11 = 4000f;
		float num12 = 2000f;
		float num13 = 2000f;
		if (ai != null)
		{
			num13 = 8000f;
			num12 = 6000f;
			num11 = 6000f;
			if (state == 0)
			{
				num10 = 1f;
			}
		}
		switch (bodySec[0].animName)
		{
		case "flyw":
		case "flym":
		case "flys":
		case "flya":
		case "flyr":
		case "flyx":
			SetBodyAnim(GetAnimName(2));
			CheckKick();
			_ = charKeys.keyFloat;
			break;
		case "jhit":
			if ((float)hp >= 0f && perk[0] == 9)
			{
				CheckKick();
			}
			break;
		case "hitland":
			if (traj.X > 0f)
			{
				traj.X -= fTime * 900f;
				if (traj.X < 0f)
				{
					traj.X = 0f;
				}
			}
			if (traj.X < 0f)
			{
				traj.X += fTime * 900f;
				if (traj.X > 0f)
				{
					traj.X = 0f;
				}
			}
			break;
		case "roll":
		case "rollx":
		case "cart":
		{
			if (fire > 0f)
			{
				fire -= fTime * 5f;
			}
			float num16 = 400f;
			if (suit == 2)
			{
				num16 *= 1.1f;
			}
			if (perk[0] == 9)
			{
				num16 *= 1.4f;
			}
			num16 *= num10;
			switch (state)
			{
			case 1:
				if (rollFace == 0)
				{
					traj.X = 0f - num16;
				}
				else
				{
					traj.X = num16;
				}
				break;
			case 2:
				if (rollFace == 0)
				{
					traj.Y = 0f - num16;
				}
				else
				{
					traj.Y = num16;
				}
				break;
			case 3:
				if (rollFace == 0)
				{
					traj.Y = num16;
				}
				else
				{
					traj.Y = 0f - num16;
				}
				break;
			case 4:
				if (rollFace == 0)
				{
					traj.X = num16;
				}
				else
				{
					traj.X = 0f - num16;
				}
				break;
			}
			break;
		}
		case "idlew":
		case "idlem":
		case "idles":
		case "idlea":
		case "idler":
		case "idlex":
		case "runw":
		case "runm":
		case "runs":
		case "runa":
		case "runr":
		case "runx":
		{
			Vector2 vector5 = default(Vector2);
			float num14 = 200f;
			if (suit == 2 && charKeys.keyFloat)
			{
				num14 = 400f;
			}
			if (GameState.gameType == 4 && team == 1 && charKeys.keyFloat)
			{
				num14 = 400f;
			}
			num14 *= num10;
			float num15 = charKeys.runSpeed;
			bool flag5;
			bool flag4;
			switch (state)
			{
			case 2:
				flag5 = charKeys.keyUp;
				flag4 = charKeys.keyDown;
				vector5.X = traj.Y;
				num15 = charKeys.runSpeed;
				break;
			case 3:
				flag5 = charKeys.keyDown;
				flag4 = charKeys.keyUp;
				vector5.X = 0f - traj.Y;
				num15 = charKeys.runSpeed;
				break;
			case 4:
				flag5 = charKeys.keyRight;
				flag4 = charKeys.keyLeft;
				vector5.X = 0f - traj.X;
				break;
			default:
				flag4 = charKeys.keyRight;
				flag5 = charKeys.keyLeft;
				vector5.X = traj.X;
				break;
			}
			if (num15 < 0f)
			{
				num15 = 0f - num15;
			}
			num15 *= 1.2f;
			if (num15 > 1f)
			{
				num15 = 1f;
			}
			num14 *= num15;
			if (flag4)
			{
				if (vector5.X < num14)
				{
					vector5.X += num11 * fTime * num15;
				}
				else
				{
					vector5.X = num14;
				}
				SetBodyAnim(GetAnimName(1));
				if (shootFrame <= 0f && grenFrame <= 0f && reloadFrame <= 0f)
				{
					face = 1;
				}
			}
			else if (flag5)
			{
				if (vector5.X > 0f - num14)
				{
					vector5.X -= num11 * fTime * num15;
				}
				else
				{
					vector5.X = 0f - num14;
				}
				SetBodyAnim(GetAnimName(1));
				if (shootFrame <= 0f && grenFrame <= 0f && reloadFrame <= 0f)
				{
					face = 0;
				}
			}
			else
			{
				SetBodyAnim(GetAnimName(0));
				if (vector5.X > 0f)
				{
					vector5.X -= fTime * num12;
					if (vector5.X < 0f)
					{
						vector5.X = 0f;
					}
				}
				if (vector5.X < 0f)
				{
					vector5.X += fTime * num12;
					if (vector5.X > 0f)
					{
						vector5.X = 0f;
					}
				}
			}
			if (charKeys.keyKick && shrink <= 0f)
			{
				if (WeaponCatalog.weapons[weapon[curWeap]].projType == 10 || WeaponCatalog.weapons[weapon[curWeap]].projType == 16)
				{
					bodySec[0].SetAnim(animList[85], this, overRide: true);
				}
				else
				{
					bodySec[0].SetAnim(animList[58], this, overRide: true);
				}
				bodySec[0].CheckTrig(this);
				splitAnim = false;
				bodySec[0].endAction = 1;
				if (face == 0)
				{
					vector5.X = -500f;
				}
				else
				{
					vector5.X = 500f;
				}
				kickFrame = 0.1f;
			}
			if (charKeys.keySquat)
			{
				kickFrame = 0.1f;
				bodySec[0].SetAnim(animList[86], this, overRide: true);
				splitAnim = false;
				bodySec[0].endAction = 1;
				if (poison > 0f)
				{
					poison--;
					if (poison < 0f)
					{
						poison = 0f;
					}
				}
			}
			if (charKeys.keySuicide)
			{
				bodySec[0].SetAnim(animList[88], this, overRide: true);
				splitAnim = false;
				bodySec[0].endAction = 5;
			}
			bool flag6 = false;
			if (GameState.gameType == 4 && team == 1)
			{
				flag6 = true;
			}
			if (charKeys.keyRoll && suit != 2 && !flag6 && (flag5 || flag4) && rollFrame <= 0f)
			{
				rollFrame = 1f;
				if (shootFrame > 0f || shellsFrame > 0f || cockitFrame > 0f || kickFrame > 0f || kickRecoverFrame > 0f || grenFrame > 0f || splitAnim || reloadFrame > 0f || perk[0] == 9)
				{
					rollFrame = 0.5f;
					if (WeaponCatalog.weapons[weapon[curWeap]].projType != 10 && WeaponCatalog.weapons[weapon[curWeap]].projType != 16)
					{
						bodySec[0].SetAnim(animList[57], this, overRide: true);
						bodySec[0].endAction = 1;
						if (flag5)
						{
							angle += 6.28f;
							rollFace = 0;
						}
						if (flag4)
						{
							angle -= 6.28f;
							rollFace = 1;
						}
						if ((loc - Scroll.scroll).LengthSquared() < 250000f)
						{
							Sound.PlayCue("throw");
						}
					}
					else
					{
						bodySec[0].SetAnim(animList[84], this, overRide: true);
						bodySec[0].endAction = 3;
						if (flag5)
						{
							rollFace = 0;
						}
						if (flag4)
						{
							rollFace = 1;
						}
						if ((loc - Scroll.scroll).LengthSquared() < 250000f)
						{
							Sound.PlayCue("swing");
						}
					}
				}
				else
				{
					if (WeaponCatalog.weapons[weapon[curWeap]].projType == 10 || WeaponCatalog.weapons[weapon[curWeap]].projType == 16)
					{
						bodySec[0].SetAnim(animList[84], this, overRide: true);
					}
					else
					{
						bodySec[0].SetAnim(animList[59], this, overRide: true);
					}
					bodySec[0].endAction = 3;
					if (flag5)
					{
						rollFace = 0;
					}
					if (flag4)
					{
						rollFace = 1;
					}
					if ((loc - Scroll.scroll).LengthSquared() < 250000f)
					{
						Sound.PlayCue("swing");
					}
				}
			}
			switch (state)
			{
			case 2:
				_ = charKeys.keyUp;
				_ = charKeys.keyDown;
				traj.Y = vector5.X;
				traj.X = 0f;
				if (perk[1] != 1 && !charKeys.keyUp)
				{
					SetBodyAnim(GetAnimName(9));
					bodySec[0].endAction = 2;
					if (ai != null)
					{
						ai.RedFlag();
					}
				}
				break;
			case 3:
				_ = charKeys.keyDown;
				_ = charKeys.keyUp;
				traj.Y = 0f - vector5.X;
				traj.X = 0f;
				if (perk[1] != 1 && !charKeys.keyUp)
				{
					SetBodyAnim(GetAnimName(9));
					bodySec[0].endAction = 2;
					if (ai != null)
					{
						ai.RedFlag();
					}
				}
				break;
			case 4:
				_ = charKeys.keyRight;
				_ = charKeys.keyRight;
				traj.X = 0f - vector5.X;
				traj.Y = 0f;
				if (perk[1] != 1 && !charKeys.keyLeft && !charKeys.keyRight)
				{
					SetBodyAnim(GetAnimName(9));
					bodySec[0].endAction = 2;
				}
				break;
			default:
				_ = charKeys.keyRight;
				_ = charKeys.keyLeft;
				traj.X = vector5.X;
				traj.Y = 0f;
				break;
			}
			if (charKeys.keyJump || charKeys.keyA)
			{
				SetBodyAnim(GetAnimName(9));
				bodySec[0].endAction = 2;
				bodySec[0].curFrame = 1.99f;
			}
			break;
		}
		case "landw":
		case "landm":
		case "lands":
		case "landa":
		case "landr":
		case "landx":
			if (charKeys.keyJump || charKeys.keyA)
			{
				SetBodyAnim(GetAnimName(9));
				bodySec[0].endAction = 2;
				bodySec[0].curFrame = 1f;
			}
			break;
		}
		if (state != 0 && jetGas < 0.25f)
		{
			jetGas = 0.25f;
		}
		switch (state)
		{
		case 0:
			traj.Y += fTime * Game1.gravity;
			if (traj.Y > 1200f)
			{
				traj.Y = 1200f;
			}
			if (!Game1.netSession.GetNetworkOwner(ID) && recentShortUpdate)
			{
				traj = default(Vector2);
			}
			if (!(bodySec[0].animName == "jhit"))
			{
				float num17 = 200f;
				num17 *= num10;
				if (jetGas > 0f && charKeys.keyJetpack)
				{
					num17 *= 1.5f;
				}
				float num18 = charKeys.runVec.X;
				if (num18 < 0f)
				{
					num18 = 0f - num18;
				}
				num18 *= 1.25f;
				if (num18 > 1f)
				{
					num18 = 1f;
				}
				if (charKeys.keyRight && traj.X < num17)
				{
					traj.X += fTime * num13 * num18;
				}
				if (charKeys.keyLeft && traj.X > 0f - num17)
				{
					traj.X -= fTime * num13 * num18;
				}
			}
			if (state == 0)
			{
				xMove(loc, map, fTime);
				yMove(loc, map, fTime);
			}
			break;
		case 2:
			yMove(loc, map, fTime);
			traj.X = 0f;
			if (!map.GetIsCol(loc + new Vector2(-40f, -34f)) || !map.GetIsCol(loc + new Vector2(-40f, -30f)))
			{
				state = 0;
				if (bodySec[0].animName == "kick" || bodySec[0].animName == "kickx" || bodySec[0].animName == "firexu" || bodySec[0].animName == "firexl")
				{
					bodySec[0].endAction = 4;
				}
				else
				{
					SetBodyAnim(GetAnimName(2));
				}
				if (traj.Y < 0f)
				{
					traj.Y = -420f;
					angle += 6.28f;
				}
			}
			break;
		case 3:
			yMove(loc, map, fTime);
			traj.X = 0f;
			if (!map.GetIsCol(loc + new Vector2(40f, -34f)) || !map.GetIsCol(loc + new Vector2(40f, -30f)))
			{
				state = 0;
				if (bodySec[0].animName == "kick" || bodySec[0].animName == "kickx" || bodySec[0].animName == "firexu" || bodySec[0].animName == "firexl")
				{
					bodySec[0].endAction = 4;
				}
				else
				{
					SetBodyAnim(GetAnimName(2));
				}
				if (traj.Y < 0f)
				{
					traj.Y = -420f;
					angle -= 6.28f;
				}
			}
			break;
		case 4:
			xMove(loc, map, fTime);
			traj.Y = 0f;
			if (noStick > 0f)
			{
				traj.Y = 400f;
			}
			if (!map.GetIsCol(loc + new Vector2(0f, -70f)))
			{
				state = 0;
				if (bodySec[0].animName == "kick" || bodySec[0].animName == "kickx" || bodySec[0].animName == "firexu" || bodySec[0].animName == "firexl")
				{
					bodySec[0].endAction = 4;
				}
				else
				{
					SetBodyAnim(GetAnimName(2));
				}
			}
			if (!charKeys.keyLeft && !charKeys.keyRight && perk[1] != 1)
			{
				state = 0;
				SetBodyAnim(GetAnimName(2));
				jetRecover = 0.1f;
			}
			break;
		case 1:
			xMove(loc, map, fTime);
			if (map.GetIsCol(loc + new Vector2(0f, 10f)))
			{
				loc.Y = map.GetMinY(loc + new Vector2(0f, 10f));
				break;
			}
			if (map.GetIsCol(loc + new Vector2(10f, 10f)))
			{
				loc.Y = map.GetMinY(loc + new Vector2(10f, 10f));
				break;
			}
			if (map.GetIsCol(loc + new Vector2(-10f, 10f)))
			{
				loc.Y = map.GetMinY(loc + new Vector2(-10f, 10f));
				break;
			}
			traj.Y = 0f;
			state = 0;
			if (bodySec[0].animName == "kick" || bodySec[0].animName == "kickx" || bodySec[0].animName == "firexu" || bodySec[0].animName == "firexl")
			{
				bodySec[0].endAction = 4;
			}
			else
			{
				SetBodyAnim(GetAnimName(2));
			}
			break;
		}
		UpdateShoot(fTime);
		UpdateRects();
	}

	private void CheckKick()
	{
		if (charKeys.keyKick && shrink <= 0f)
		{
			angle = 0f;
			if (charKeys.keyLeft)
			{
				face = 0;
			}
			if (charKeys.keyRight)
			{
				face = 1;
			}
			if (WeaponCatalog.weapons[weapon[curWeap]].projType == 10 || WeaponCatalog.weapons[weapon[curWeap]].projType == 16)
			{
				bodySec[0].SetAnim(animList[85], this, overRide: true);
			}
			else
			{
				bodySec[0].SetAnim(animList[58], this, overRide: true);
			}
			bodySec[0].CheckTrig(this);
			splitAnim = false;
			bodySec[0].endAction = 4;
			if (face == 0)
			{
				traj.X = -500f;
			}
			else
			{
				traj.X = 500f;
			}
			traj.Y = -300f;
			kickFrame = 0.1f;
		}
	}

	private void UpdateShoot(float fTime)
	{
		if (spawnFrame > 0f)
		{
			return;
		}
		if (magazine[curWeap] <= 0)
		{
			if (weapon[curWeap] <= -1)
			{
				return;
			}
			if (ammo[WeaponCatalog.weapons[weapon[curWeap]].ammoType] <= 0)
			{
				weapon[curWeap] = -1;
			}
			bool flag = false;
			if (weapon[curWeap] == -1)
			{
				if (curWeap == 0)
				{
					if (perk[1] == 4)
					{
						weapon[0] = 117;
					}
					else
					{
						weapon[0] = 17;
					}
					ammo[0] = 500;
					flag = true;
				}
				bool flag2 = false;
				for (int i = 0; i < 4; i++)
				{
					curWeap = (curWeap + 3) % 4;
					if (weapon[curWeap] > -1)
					{
						flag2 = true;
						flag = true;
						break;
					}
				}
				if (!flag2)
				{
					curWeap = 0;
					weapon[0] = 17;
					ammo[0] = 500;
					weapIdx = WeaponCatalog.weapons[weapon[0]].idx;
					flag = true;
				}
				if (flag)
				{
					bodySec[1].SetAnim(GetAnimName(0), this, overRide: true);
				}
			}
		}
		if (shrink > 0f)
		{
			charKeys.shootVec.X = 0f;
			charKeys.shootVec.Y = 0f;
		}
		if (bodySec[0].animName == "jhit" || bodySec[0].animName == "hitland" || bodySec[0].animName == "roll" || kickFrame > 0f || kickRecoverFrame > 0f)
		{
			return;
		}
		if (weapon[curWeap] > -1)
		{
			try
			{
				if (WeaponCatalog.weapons[weapon[curWeap]].projType == 10 || WeaponCatalog.weapons[weapon[curWeap]].projType == 16)
				{
					magazine[curWeap] = 1;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.StackTrace);
			}
		}
		int num = WeaponCatalog.weapons[weapon[curWeap]].maxClip;
		if (num > 1 && perk[2] == 7)
		{
			num *= 3;
		}
		if (grenFrame > 0f)
		{
			grenFrame -= fTime;
			return;
		}
		if (reloadFrame > 0f)
		{
			if (freeze > 0f)
			{
				reloadFrame -= fTime / 2f;
			}
			else
			{
				reloadFrame -= fTime;
			}
			return;
		}
		if (shootFrame > 0f)
		{
			if (freeze > 0f)
			{
				shootFrame -= fTime / 2f;
			}
			else
			{
				shootFrame -= fTime;
			}
			if (!(shootFrame <= 0f) || weapon[curWeap] <= -1)
			{
				return;
			}
			try
			{
				if (WeaponCatalog.weapons[weapon[curWeap]].shells)
				{
					bodySec[1].SetAnim("cockits", this, overRide: true);
					if ((loc - Scroll.scroll).LengthSquared() < 90000f)
					{
						Sound.PlayCue("cockit");
					}
					splitAnim = true;
					cockitFrame = 0.25f;
				}
				return;
			}
			catch (Exception ex2)
			{
				Console.WriteLine(ex2.StackTrace);
				return;
			}
		}
		if (cockitFrame > 0f)
		{
			cockitFrame -= fTime;
			if (suit == 12)
			{
				cockitFrame -= fTime;
			}
			if (perk[0] == 6)
			{
				cockitFrame -= fTime;
			}
			return;
		}
		if (shellsFrame > 0f)
		{
			shellsFrame -= fTime;
			if (suit == 12)
			{
				shellsFrame -= fTime;
			}
			if (perk[0] == 6)
			{
				shellsFrame -= fTime;
			}
			if (!(shellsFrame <= 0f))
			{
				return;
			}
			ammo[WeaponCatalog.weapons[weapon[curWeap]].ammoType]--;
			magazine[curWeap]++;
			if (ammo[WeaponCatalog.weapons[weapon[curWeap]].ammoType] > 0 && magazine[curWeap] < num)
			{
				shellsFrame = 0.15f;
				bodySec[1].SetAnim("shells", this, overRide: true);
				if ((loc - Scroll.scroll).LengthSquared() < 90000f)
				{
					Sound.PlayCue("click1");
				}
				splitAnim = true;
			}
			else
			{
				bodySec[1].SetAnim("cockits", this, overRide: true);
				if ((loc - Scroll.scroll).LengthSquared() < 90000f)
				{
					Sound.PlayCue("cockit");
				}
				splitAnim = true;
				cockitFrame = 0.25f;
			}
			return;
		}
		bool flag3 = false;
		bool flag4 = false;
		if (charKeys.keyGrenade && grenType[0] > -1 && grenAmmo[0] > 0)
		{
			flag3 = true;
			lastGren = 0;
		}
		if (charKeys.keyGren2 && grenType[1] > -1 && grenAmmo[1] > 0)
		{
			flag4 = true;
			lastGren = 1;
		}
		if (!flag3 && !flag4 && bodySec[1].animName == animList[39] && splitAnim)
		{
			splitAnim = false;
		}
		if (flag3 || flag4)
		{
			bodySec[1].SetAnim(animList[39], this, overRide: true);
			splitAnim = true;
			if (charKeys.shootVec.LengthSquared() > 0.36f)
			{
				grenVec = charKeys.shootVec;
				bodySec[1].SetAnim(animList[38], this, overRide: true);
				grenFrame = 0.4f;
				float num2 = Trig.GetAngle(default(Vector2), charKeys.shootVec) - angle;
				int num3 = (int)((num2 + 0.3925f + 3.14f) / 0.785f);
				switch (num3 % 8)
				{
				case 0:
				case 1:
				case 7:
					face = 1;
					break;
				case 3:
				case 4:
				case 5:
					face = 0;
					break;
				case 2:
				case 6:
					break;
				}
			}
			return;
		}
		if (charKeys.shootVec.Length() > 0.6f)
		{
			if (magazine[curWeap] > 0)
			{
				if ((WeaponCatalog.weapons[weapon[curWeap]].projType == 10 || WeaponCatalog.weapons[weapon[curWeap]].projType == 16) && state == 0)
				{
					angle = 0f;
				}
				float num4 = Trig.GetAngle(default(Vector2), charKeys.shootVec) - angle;
				int num5 = (int)((num4 + 0.3925f + 3.14f) / 0.785f);
				num5 %= 8;
				if (WeaponCatalog.weapons[weapon[curWeap]].charge && charge < 1f)
				{
					float num6 = charge;
					charge += fTime * 0.55f;
					if ((loc - Scroll.scroll).LengthSquared() < 490000f)
					{
						if (num6 <= 0f && charge > 0f)
						{
							Sound.PlayCue("zcharge1");
						}
						if (num6 <= 0.3f && charge > 0.3f)
						{
							Sound.PlayCue("zcharge2");
						}
						if (num6 <= 0.6f && charge > 0.6f)
						{
							Sound.PlayCue("zcharge3");
						}
					}
					SetShootAnim(num5);
					if (charge < 1f)
					{
						bodySec[1].CheckTrig(this);
					}
					return;
				}
				if ((loc - Scroll.scroll).LengthSquared() < 490000f)
				{
					Sound.PlayCue(WeaponCatalog.weapons[weapon[curWeap]].snd);
				}
				if (WeaponCatalog.weapons[weapon[curWeap]].projType == 10 || WeaponCatalog.weapons[weapon[curWeap]].projType == 16)
				{
					splitAnim = false;
					float num7 = 1200f;
					switch (num5)
					{
					case 0:
					case 3:
					case 5:
					case 7:
						num7 = 900f;
						break;
					}
					switch (num5)
					{
					case 0:
					case 1:
					case 7:
						face = 1;
						bodySec[0].SetAnim(GetAnimName(3), this, overRide: true);
						break;
					case 2:
						bodySec[0].SetAnim(GetAnimName(7), this, overRide: true);
						num7 = 300f;
						break;
					case 3:
					case 4:
					case 5:
						bodySec[0].SetAnim(GetAnimName(3), this, overRide: true);
						face = 0;
						break;
					case 6:
						bodySec[0].SetAnim(GetAnimName(5), this, overRide: true);
						num7 = 300f;
						break;
					}
					if (state == 0)
					{
						bodySec[0].endAction = 4;
						if (face == 0)
						{
							traj.X = 0f - num7;
						}
						else
						{
							traj.X = num7;
						}
						traj.Y = -300f;
						if (num5 == 6)
						{
							traj.Y = -900f;
						}
						if (num5 == 2)
						{
							traj.Y = 900f;
						}
					}
					else
					{
						bodySec[0].endAction = 1;
						switch (state)
						{
						case 1:
							if (face == 0)
							{
								traj.X = 0f - num7;
							}
							else
							{
								traj.X = num7;
							}
							break;
						case 2:
							if (face == 0)
							{
								traj.Y = 0f - num7;
							}
							else
							{
								traj.Y = num7;
							}
							break;
						case 3:
							if (face == 0)
							{
								traj.Y = num7;
							}
							else
							{
								traj.Y = 0f - num7;
							}
							break;
						case 4:
							if (face == 0)
							{
								traj.X = num7;
							}
							else
							{
								traj.X = 0f - num7;
							}
							break;
						}
					}
					bodySec[0].CheckTrig(this);
					kickFrame = 0.1f;
				}
				else
				{
					SetShootAnim(num5);
					shootFrame = WeaponCatalog.weapons[weapon[curWeap]].fireRate;
					bodySec[1].CheckTrig(this);
					charge = 0f;
					if (suit == 12)
					{
						shootFrame *= 0.5f;
					}
					if (perk[0] == 1)
					{
						shootFrame *= 0.75f;
					}
					if (shootFrame <= 0.015f)
					{
						shootFrame = 0.015f;
					}
					magazine[curWeap]--;
				}
			}
			else
			{
				if (ammo[WeaponCatalog.weapons[weapon[curWeap]].ammoType] <= 0)
				{
					return;
				}
				if (WeaponCatalog.weapons[weapon[curWeap]].shells)
				{
					bodySec[1].SetAnim("shells", this, overRide: true);
					splitAnim = true;
					if ((loc - Scroll.scroll).LengthSquared() < 90000f)
					{
						Sound.PlayCue("click1");
					}
					shellsFrame = 0.15f;
					return;
				}
				WeaponCatalog.weapons[weapon[curWeap]].Reload(this, curWeap);
				if ((loc - Scroll.scroll).LengthSquared() < 90000f)
				{
					Sound.PlayCue("click2");
				}
				bodySec[1].SetAnim(GetAnimName(10), this, overRide: true);
				reloadFrame = WeaponCatalog.weapons[weapon[curWeap]].reloadTime;
				if (suit == 12)
				{
					reloadFrame *= 0.5f;
				}
				if (perk[0] == 6)
				{
					reloadFrame *= 0.4f;
				}
				splitAnim = true;
			}
			return;
		}
		charge = 0f;
		if (charKeys.keyB)
		{
			CycleGrenades();
		}
		if (!charKeys.keyReload || magazine[curWeap] >= num || ammo[WeaponCatalog.weapons[weapon[curWeap]].ammoType] <= 0)
		{
			return;
		}
		if (WeaponCatalog.weapons[weapon[curWeap]].shells)
		{
			bodySec[1].SetAnim("shells", this, overRide: true);
			splitAnim = true;
			if ((loc - Scroll.scroll).LengthSquared() < 90000f)
			{
				Sound.PlayCue("click1");
			}
			shellsFrame = 0.15f;
			return;
		}
		WeaponCatalog.weapons[weapon[curWeap]].Reload(this, curWeap);
		if ((loc - Scroll.scroll).LengthSquared() < 90000f)
		{
			Sound.PlayCue("click2");
		}
		bodySec[1].SetAnim(GetAnimName(10), this, overRide: true);
		reloadFrame = WeaponCatalog.weapons[weapon[curWeap]].reloadTime;
		if (suit == 12)
		{
			reloadFrame *= 0.5f;
		}
		if (perk[0] == 6)
		{
			reloadFrame *= 0.4f;
		}
		splitAnim = true;
	}

	private void SetShootAnim(int fA)
	{
		switch (fA)
		{
		case 0:
			bodySec[1].SetAnim(GetAnimName(3), this, overRide: true);
			face = 1;
			break;
		case 1:
			bodySec[1].SetAnim(GetAnimName(6), this, overRide: true);
			face = 1;
			break;
		case 2:
			bodySec[1].SetAnim(GetAnimName(7), this, overRide: true);
			break;
		case 3:
			bodySec[1].SetAnim(GetAnimName(6), this, overRide: true);
			face = 0;
			break;
		case 4:
			bodySec[1].SetAnim(GetAnimName(3), this, overRide: true);
			face = 0;
			break;
		case 5:
			bodySec[1].SetAnim(GetAnimName(4), this, overRide: true);
			face = 0;
			break;
		case 6:
			bodySec[1].SetAnim(GetAnimName(5), this, overRide: true);
			break;
		case 7:
			bodySec[1].SetAnim(GetAnimName(4), this, overRide: true);
			face = 1;
			break;
		}
		splitAnim = true;
	}

	private void xMove(Vector2 pLoc, GameMap map, float fTime)
	{
		int num = (int)(fTime / 0.01666f);
		if (num < 1)
		{
			num = 1;
		}
		for (int i = 0; i < num; i++)
		{
			xSplitMove(pLoc, map, fTime / (float)num);
			pLoc = loc;
		}
	}

	private void xSplitMove(Vector2 pLoc, GameMap map, float fTime)
	{
		if (freeze > 0f)
		{
			loc.X += traj.X * fTime * 0.3f;
		}
		else if (shrink > 0f)
		{
			loc.X += traj.X * fTime * 0.6f;
		}
		else if (shrink > 0f)
		{
			loc.X += traj.Y * fTime * 0.75f;
		}
		else
		{
			loc.X += traj.X * fTime;
		}
		if (loc.Y > 8000f)
		{
			return;
		}
		int num;
		if (traj.X > 0f)
		{
			num = (int)((loc.X + 24f) / 64f);
			if (map.GetIsCol(loc + new Vector2(24f, -24f)) || map.GetIsCol(loc + new Vector2(24f, -60f)))
			{
				loc.X = pLoc.X;
				traj.X = 0f;
				byte col = map.GetCol(loc + new Vector2(24f, -24f));
				if (col == 2)
				{
					if (map.GetCol(loc) == 2)
					{
						loc.Y = map.GetMinY(loc);
						state = 1;
						SetBodyAnim(GetAnimName(8));
						bodySec[0].endAction = 1;
					}
				}
				else
				{
					loc.X = (float)num * 64f - 24f;
					if (ai != null)
					{
						ai.HitWall();
					}
				}
				if (!(bodySec[0].animName == "jhit") && special == 2 && shrink <= 0f && !(bodySec[0].animName == "firexd") && !(bodySec[0].animName == "firexu") && !(bodySec[0].animName == "firexl"))
				{
					if (state == 4)
					{
						traj.X = -300f;
						noStick = 0.5f;
						state = 0;
						SetBodyAnim(GetAnimName(2));
					}
					else if (map.GetIsCol(loc + new Vector2(32f, -34f)) && map.GetIsCol(loc + new Vector2(32f, -30f)) && ((charKeys.keyUp && state != 4) || perk[1] == 1))
					{
						state = 3;
						angle = -1.57f;
						SetBodyAnim(GetAnimName(8));
						bodySec[0].endAction = 1;
						loc.X = (float)num * 64f - 32f;
						drawVec = loc;
						traj.Y = 0f;
					}
					else if (ai != null && state == 1)
					{
						ai.RedFlag();
					}
				}
			}
		}
		if (!(traj.X < 0f))
		{
			return;
		}
		num = (int)((loc.X - 24f) / 64f);
		if (!map.GetIsCol(loc + new Vector2(-24f, -24f)) && !map.GetIsCol(loc + new Vector2(-24f, -60f)))
		{
			return;
		}
		loc.X = pLoc.X;
		traj.X = 0f;
		byte col2 = map.GetCol(loc + new Vector2(-24f, -24f));
		if (col2 == 3)
		{
			if (map.GetCol(loc) == 3)
			{
				loc.Y = map.GetMinY(loc);
				state = 1;
				SetBodyAnim(GetAnimName(8));
				bodySec[0].endAction = 1;
			}
		}
		else
		{
			loc.X = (float)num * 64f + 64f + 24f;
			if (ai != null)
			{
				ai.HitWall();
			}
		}
		if (!(bodySec[0].animName == "jhit") && special == 2 && shrink <= 0f && !(bodySec[0].animName == "firexd") && !(bodySec[0].animName == "firexu") && !(bodySec[0].animName == "firexl"))
		{
			if (state == 4)
			{
				traj.X = 300f;
				noStick = 0.5f;
				state = 0;
				SetBodyAnim(GetAnimName(2));
			}
			else if (map.GetIsCol(loc + new Vector2(-32f, -34f)) && map.GetIsCol(loc + new Vector2(-32f, -30f)) && ((charKeys.keyUp && state != 4) || perk[1] == 1))
			{
				state = 2;
				angle = 1.57f;
				SetBodyAnim(GetAnimName(8));
				bodySec[0].endAction = 1;
				loc.X = (float)num * 64f + 64f + 32f;
				drawVec = loc;
				traj.Y = 0f;
			}
			else if (ai != null && state == 1)
			{
				ai.RedFlag();
			}
		}
	}

	private void yMove(Vector2 pLoc, GameMap map, float fTime)
	{
		int num = (int)(fTime / 0.01666f);
		if (num < 1)
		{
			num = 1;
		}
		for (int i = 0; i < num; i++)
		{
			ySplitMove(pLoc, map, fTime / (float)num);
			pLoc = loc;
			if (state != 0)
			{
				break;
			}
		}
	}

	private void ySplitMove(Vector2 pLoc, GameMap map, float fTime)
	{
		if (freeze > 0f)
		{
			loc.Y += traj.Y * fTime * 0.3f;
		}
		else if (shrink > 0f)
		{
			loc.Y += traj.Y * fTime * 0.6f;
		}
		else if (submerged)
		{
			loc.Y += traj.Y * fTime * 0.7f;
		}
		else
		{
			loc.Y += traj.Y * fTime;
		}
		if (fishFrame > 0f && traj.Y > 100f)
		{
			if (traj.Y > 120f && bodySec[0].animName == animList[36])
			{
				float num = 100f / traj.Y;
				traj.X *= num;
			}
			traj.Y = 100f;
		}
		if (loc.Y > 8000f)
		{
			return;
		}
		if (traj.Y > 0f && map.GetIsCol(loc + new Vector2(0f, 1f)))
		{
			loc.Y = map.GetMinY(loc);
			drawVec = loc;
			state = 1;
			angle = 0f;
			if (bodySec[0].animName == "jhit")
			{
				SetBodyAnim(animList[37], sync: true);
				bodySec[0].endAction = 3;
				if (ai != null)
				{
					ai.KillTrail();
				}
			}
			else
			{
				if (!(bodySec[0].animName == "firexd") && !(bodySec[0].animName == "firexu") && !(bodySec[0].animName == "firexl"))
				{
					SetBodyAnim(GetAnimName(8));
				}
				bodySec[0].endAction = 1;
			}
			Game1.pMan.AddParticle(0, loc, default(Vector2), 0.85f, 0, 0);
		}
		if (!(traj.Y < 0f) || !map.GetIsCol(loc + new Vector2(0f, -64f)))
		{
			return;
		}
		int num2 = (int)((loc.Y - 64f) / 32f);
		loc.Y = pLoc.Y;
		traj.Y = 1f;
		if (!(bodySec[0].animName == "jhit") && special == 2 && shrink <= 0f && !(bodySec[0].animName == "firexd") && !(bodySec[0].animName == "firexu") && !(bodySec[0].animName == "firexl") && !(noStick > 0f) && (state == 2 || state == 3 || charKeys.keyLeft || charKeys.keyRight || perk[1] == 1))
		{
			if ((state == 2 || state == 3) && ai != null)
			{
				ai.RedFlag();
			}
			SetBodyAnim(GetAnimName(8));
			bodySec[0].endAction = 1;
			angle = 3.14f;
			traj.Y = 0f;
			loc.Y = (float)num2 * 32f + 64f + 32f;
			drawVec = loc;
			state = 4;
		}
	}

	public void StartKill(Vector2 killTraj)
	{
		if (hp < 0)
		{
			if (suit == 7)
			{
				suit = 100;
				hp = GetMaxHP();
				Vector2 vector = loc - new Vector2(0f, 42f);
				Game1.pMan.AddParticle(1, vector, default(Vector2), 4f, 0, ID);
				for (int i = 0; i < 16; i++)
				{
					float num = (float)i / 16f * 6.28f;
					Vector2 vector2 = new Vector2((float)Math.Cos(num), (float)Math.Sin(num));
					if (i % 2 == 0)
					{
						vector2 *= 150f;
					}
					else
					{
						vector2 *= 300f;
					}
					Game1.pMan.AddParticle(15, vector, vector2, 1f, 0, ID);
				}
				shrink = 0f;
				poison = 0f;
				killedBy = -1;
			}
			else if (suit == 8)
			{
				Game1.pMan.Explode(loc - new Vector2(0f, 42f), ID, 500, 500f);
				hp = -50;
			}
		}
		if (hp < -30 && dyingFrame < 1f)
		{
			if (shrink > 0f)
			{
				for (int j = 0; j < 4; j++)
				{
					Game1.pMan.AddParticle(39, loc - new Vector2(0f, 32f), Rand.GetRandomVec2(-200f, 200f, -400f, 100f), 0f, 0, ID);
				}
				if ((loc - Scroll.scroll).LengthSquared() < 250000f)
				{
					Sound.PlayCue("hit2");
				}
			}
			else
			{
				for (int k = 0; k < 12; k++)
				{
					Game1.pMan.AddParticle(39, loc - new Vector2(0f, 32f), Rand.GetRandomVec2(-700f, 700f, -1200f, 100f), 0f, 0, ID);
				}
				if ((loc - Scroll.scroll).LengthSquared() < 250000f)
				{
					Sound.PlayCue("hit2");
				}
			}
			gibbed = true;
			dyingFrame = 1f;
		}
		if (freeze > 0f && hp < 0 && dyingFrame < 1f)
		{
			for (int l = 0; l < 12; l++)
			{
				Game1.pMan.AddParticle(40, loc - new Vector2(0f, 32f), Rand.GetRandomVec2(-600f, 600f, -600f, 100f), 0f, 0, ID);
			}
			if ((loc - Scroll.scroll).LengthSquared() < 810000f)
			{
				Sound.PlayCue("glass");
			}
			gibbed = true;
			dyingFrame = 1f;
		}
		if (hp >= 0 || (!(bodySec[0].animName == "jhit") && !(bodySec[0].animName == "hitland")))
		{
			SetBodyAnim(animList[36], sync: true);
			if (ai != null)
			{
				ai.KillTrail();
			}
			if (killTraj.X > 0f)
			{
				face = 0;
			}
			if (killTraj.X < 0f)
			{
				face = 1;
			}
			traj.X = killTraj.X;
			traj.Y = -300f;
			if (killTraj.Y < traj.Y)
			{
				traj.Y = killTraj.Y;
			}
			state = 0;
		}
	}

	private void SetBodyAnim(string anim)
	{
		SetBodyAnim(anim, sync: false);
	}

	private void SetBodyAnim(string anim, bool sync)
	{
		bodySec[0].SetAnim(anim, this);
		if (sync)
		{
			splitAnim = false;
		}
	}

	private void SetUpperAnim(string anim)
	{
		bodySec[1].SetAnim(anim, this);
		splitAnim = true;
	}

	public void Draw(SpriteBatch sprite)
	{
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0424: Unknown result type (might be due to invalid IL or missing references)
		if (gibbed)
		{
			return;
		}
		if (spawnFrame > 0f)
		{
			drawVec = loc + new Vector2(spawnFrame * 300f * ((face == 1) ? (-1f) : 1f), (float)Math.Pow(spawnFrame * 10f, 2.0) * -1f);
			Game1.pterodactyl[ID].loc = drawVec + new Vector2(0f, -76f);
			Game1.pterodactyl[ID].exists = true;
			Game1.pterodactyl[ID].face = 1 - face;
			Game1.pterodactyl[ID].anim = 1;
			Game1.pterodactyl[ID].traj = default(Vector2);
		}
		Vector2 vector = Scroll.GetLoc(drawVec);
		if (vector.X < -64f || vector.Y < -64f || vector.X > 1408f || vector.Y > 848f || dyingFrame >= 2f)
		{
			return;
		}
		if (GameState.gameType == 2 && (Game1.netSession.redFlagState == ID || Game1.netSession.blueFlagState == ID))
		{
			sprite.Draw(Game1.spritesTex, Scroll.GetLoc(drawVec + new Vector2(0f, -36f)), (Rectangle?)new Rectangle(448, 0, 96, 96), (Game1.netSession.blueFlagState == ID) ? new Color(new Vector4(0.5f, 0.5f, 1f, 1f)) : new Color(new Vector4(1f, 0.5f, 0.5f, 1f)), 0f, new Vector2(48f, 48f), 0.85f * Scroll.zoom, (SpriteEffects)0, 1f);
		}
		if (state == 1 && spawnFrame <= 0f)
		{
			sprite.Draw(Game1.spritesTex, Scroll.GetLoc(drawVec), (Rectangle?)new Rectangle(0, 832, 192, 192), new Color(0f, 0f, 0f, 0.3f), 0f, new Vector2(96f, 96f), Scroll.zoom * 0.3f * new Vector2(1f, 0.2f), (SpriteEffects)0, 1f);
		}
		if (suit == 9 && Game1.netSession.GetPlayerOne() != ID)
		{
			Vector2 vector2 = drawVec;
			for (int i = -3; i < 4; i++)
			{
				drawVec = vector2 + drawTraj * i * 0.3f;
				if (!splitAnim)
				{
					Draw(sprite, 0, 0, all: true);
					continue;
				}
				Draw(sprite, 0, 0, all: false);
				Draw(sprite, 1, 1, all: false);
			}
			drawVec = vector2;
		}
		else if (!splitAnim)
		{
			Draw(sprite, 0, 0, all: true);
		}
		else
		{
			Draw(sprite, 0, 0, all: false);
			Draw(sprite, 1, 1, all: false);
		}
		if (GameState.gameType == 4 && team == 0 && Game1.netSession.GetPlayerOne() == ID && dyingFrame <= 0f)
		{
			sprite.End();
			sprite.Begin(blendState: BlendState.Additive);
			for (int j = -1; j < 2; j++)
			{
				sprite.Draw(Game1.spritesTex, Scroll.GetLoc(drawVec + new Vector2(0f, -36f)), (Rectangle?)new Rectangle(224, 864, 576, 160), new Color(1f, 1f, 1f, Rand.GetRandomFloat(0.11f, 0.125f)), Trig.GetAngle(default(Vector2), Game1.flashLight.flashVec) + 3.14f + (float)j * 0.05f, new Vector2(80f, 80f), (0.85f + (float)j * 0.5f) * Scroll.zoom, (SpriteEffects)0, 1f);
			}
			sprite.End();
			sprite.Begin(blendState: BlendState.AlphaBlend);
		}
		_ = ai;
	}

	public void DrawPaths(SpriteBatch sprite)
	{
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		if (ai != null && ai.trail.trailLen > 0)
		{
			Game1.text.size = 1f;
			Game1.text.color = new Color(new Vector4(1f, 1f, 0f, 0.8f));
			string text = "";
			for (int i = 0; i < ai.trail.trailLen; i++)
			{
				text = ((i != 0) ? (text + " " + ai.trail.trail[i]) : ai.trail.trail[i].ToString());
			}
			Game1.text.DrawString(Scroll.GetLoc(loc) + new Vector2(0f, -100f), text, 1, -1f, Game1.impact, sprite);
		}
		Game1.text.size = 1f;
		Game1.text.color = new Color(new Vector4(1f, 0f, 1f, 0.8f));
		Game1.text.DrawString(Scroll.GetLoc(loc) + new Vector2(0f, -130f), lastNode.ToString(), 1, -1f, Game1.impact, sprite);
		Game1.text.DrawString(Scroll.GetLoc(loc) + new Vector2(0f, -160f), ID.ToString(), 1, -1f, Game1.impact, sprite);
	}

	public static Vector2 GetAngleAdjustedVec(Vector2 anchor, Vector2 loc, float angle)
	{
		Vector2 v = anchor - loc;
		float num = v.Length();
		float num2 = Trig.GetAngle(default(Vector2), v);
		num2 += angle;
		return anchor + new Vector2((float)Math.Cos(num2), (float)Math.Sin(num2)) * num;
	}

	public void UpdateRects()
	{
		Vector2 vector = loc + new Vector2(0f, -32f);
		if (shrink > 0f)
		{
			Vector2 vector2 = vector + new Vector2((float)Math.Cos((double)angle + 1.57) * 20f, (float)Math.Sin((double)angle + 1.57) * 20f);
			ref Rectangle reference = ref hitRects[0];
			reference = new Rectangle((int)vector2.X - 10, (int)vector2.Y - 10, 20, 20);
			hitRects[1].X = hitRects[0].X;
			hitRects[1].Y = hitRects[0].Y;
			hitRects[1].Width = hitRects[0].Width;
			hitRects[1].Height = hitRects[0].Height;
		}
		else
		{
			for (int i = 0; i < 2; i++)
			{
				Vector2 vector3 = vector + new Vector2((float)Math.Cos((double)angle + 1.57 + (double)i * 3.14) * 20f, (float)Math.Sin((double)angle + 1.57 + (double)i * 3.14) * 20f);
				ref Rectangle reference2 = ref hitRects[i];
				reference2 = new Rectangle((int)vector3.X - 20, (int)vector3.Y - 20, 40, 40);
			}
		}
	}

	public void Draw(SpriteBatch spriteBatch, int ps, int sec, bool all)
	{
		Vector2 vector = drawVec;
		if (bodySec[0].animName == "cart")
		{
			float num = 0f;
			switch (state)
			{
			case 1:
				num = 0f;
				break;
			case 2:
				num = 1.57f;
				break;
			case 3:
				num = 4.71f;
				break;
			case 4:
				num = 3.14f;
				break;
			}
			float num2 = num - angle;
			num += 1.57f;
			if (num2 < 0f)
			{
				num2 = 0f - num2;
			}
			vector += new Vector2((float)Math.Cos(num), (float)Math.Sin(num)) * (float)Math.Sin(num2 / 2f) * -25f;
		}
		Vector2 sLoc = Scroll.GetLoc(vector);
		float sScale = scale * Scroll.zoom;
		Draw(spriteBatch, ps, sec, all, sLoc, sScale);
	}

	public void Draw(SpriteBatch spriteBatch, int ps, int sec, bool all, Vector2 sLoc, float sScale)
	{
		//IL_11eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_11b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1068: Unknown result type (might be due to invalid IL or missing references)
		//IL_1662: Unknown result type (might be due to invalid IL or missing references)
		//IL_10a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_10ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_10e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_10e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1127: Unknown result type (might be due to invalid IL or missing references)
		//IL_1129: Unknown result type (might be due to invalid IL or missing references)
		//IL_1280: Unknown result type (might be due to invalid IL or missing references)
		//IL_1285: Unknown result type (might be due to invalid IL or missing references)
		//IL_12b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_12be: Unknown result type (might be due to invalid IL or missing references)
		//IL_12fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_1300: Unknown result type (might be due to invalid IL or missing references)
		//IL_146d: Unknown result type (might be due to invalid IL or missing references)
		//IL_14d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_1531: Unknown result type (might be due to invalid IL or missing references)
		//IL_1536: Unknown result type (might be due to invalid IL or missing references)
		//IL_156a: Unknown result type (might be due to invalid IL or missing references)
		//IL_156f: Unknown result type (might be due to invalid IL or missing references)
		//IL_15af: Unknown result type (might be due to invalid IL or missing references)
		//IL_15b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a4f: Unknown result type (might be due to invalid IL or missing references)
		//IL_09bf: Unknown result type (might be due to invalid IL or missing references)
		if (bodyType == 1)
		{
			sScale *= 0.92f;
		}
		Rectangle value = default(Rectangle);
		int frameRef = Game1.charDef[defIdx].GetAnimation(bodySec[sec].anim).GetKeyFrame(bodySec[sec].key).frameRef;
		int num = -1;
		if (Game1.charDef[defIdx].GetAnimation(bodySec[sec].anim).GetKeyFrame(bodySec[sec].key).lerp)
		{
			num = bodySec[sec].key + 1;
			if (Game1.charDef[defIdx].GetAnimation(bodySec[sec].anim).GetKeyFrame(num).duration <= 0)
			{
				num = 0;
			}
			num = Game1.charDef[defIdx].GetAnimation(bodySec[sec].anim).GetKeyFrame(num).frameRef;
		}
		Frame frame = Game1.charDef[defIdx].GetFrame(frameRef);
		Vector2 vector = default(Vector2);
		if (shrink > 0f)
		{
			float num2 = (0.5f - shrink) * 2f;
			if (num2 < 0.3f)
			{
				num2 = 0.3f;
			}
			if (num2 > 1f)
			{
				num2 = 1f;
			}
			sScale = scale * num2 * Scroll.zoom;
		}
		sLoc += new Vector2((float)Math.Cos(angle + 1.57f), (float)Math.Sin(angle + 1.57f)) * sScale * 60f - new Vector2(0f, 60f) * sScale;
		if (ps == 1 && sec == 1)
		{
			for (int i = 0; i < frame.GetPartArray().Length; i++)
			{
				Part part = frame.GetPart(i);
				if (part.idx > -1)
				{
					float num3 = part.rotation;
					Vector2 vector2 = part.location * sScale + sLoc;
					_ = part.scaling * sScale;
					if (face == 1)
					{
						num3 = 0f - num3;
						vector2.X -= part.location.X * sScale * 2f;
					}
					vector2 -= new Vector2((float)Math.Sin(num3), (float)Math.Cos(num3)) * sScale * 24f;
					if (part.idx == 8 || part.idx == 9)
					{
						vector = torsoVec - vector2;
					}
				}
			}
		}
		nameAlpha = 1f;
		if (fire > 0f)
		{
			float num4 = fire * 0.2f;
			if (num4 > 0.2f)
			{
				num4 = 0.2f;
			}
			Game1.postGlowMgr.Add(Scroll.GetLoc(drawVec + new Vector2(0f, -40f)), 1f, 0.5f, 0.2f, num4, 3f);
		}
		Color val = default(Color);
		for (int j = 0; j < frame.GetPartArray().Length; j++)
		{
			Part part2 = frame.GetPart(j);
			if (part2.idx <= -1)
			{
				continue;
			}
			float num5 = part2.rotation;
			Vector2 vector3 = part2.location * sScale + sLoc;
			Vector2 vector4 = part2.scaling * sScale;
			bool flag = false;
			if ((face == 0 && part2.flip == 0) || (face == 1 && part2.flip == 1))
			{
				flag = true;
			}
			if (face == 1)
			{
				num5 = 0f - num5;
				vector3.X -= part2.location.X * sScale * 2f;
			}
			if (num > -1)
			{
				Frame frame2 = Game1.charDef[defIdx].GetFrame(num);
				if (Frame.CanLerp(frame, frame2, j))
				{
					Part part3 = frame2.GetPart(j);
					Animation animation = Game1.charDef[defIdx].GetAnimation(bodySec[sec].anim);
					KeyFrame keyFrame = animation.GetKeyFrame(bodySec[sec].key);
					float num6 = bodySec[sec].curFrame / (float)keyFrame.duration;
					if (num6 > 1f)
					{
						num6 = 1f;
					}
					Vector2 orig = part2.location * sScale + sLoc;
					Vector2 next = part3.location * sScale + sLoc;
					Vector2 orig2 = part2.scaling * sScale;
					Vector2 next2 = part3.scaling * sScale;
					float num7 = part2.rotation;
					float num8 = part3.rotation;
					if (face == 1)
					{
						num7 = 0f - num7;
						num8 = 0f - num8;
						orig.X -= part2.location.X * sScale * 2f;
						next.X -= part3.location.X * sScale * 2f;
					}
					vector3 = Frame.LerpLoc(orig, next, num6);
					num5 = Frame.LerpRotation(num7, num8, num6);
					vector4 = Frame.LerpScale(orig2, next2, num6);
				}
			}
			val = new Color(new Vector4(1f, 1f, 1f, 1f));
			if (respawnFrame > 0f && spawnFrame <= 0f)
			{
				val = new Color(1f, 1f, 1f, Rand.GetRandomFloat(0.3f, 0.6f));
			}
			if (suit == 5)
			{
				float num9 = traj.Length() / 400f;
				if (num9 > 1f)
				{
					num9 = 1f;
				}
				val = new Color(new Vector4(1f, 1f, 1f, num9));
				nameAlpha = num9;
			}
			else if (suit == 10)
			{
				if (state == 4)
				{
					float num10 = traj.Length() / 400f;
					if (num10 > 1f)
					{
						num10 = 1f;
					}
					val = new Color(new Vector4(1f, 1f, 1f, num10));
					nameAlpha = num10;
				}
			}
			else if (suit == 9)
			{
				val = new Color(new Vector4(1f, 1f, 1f, 0.4f));
				nameAlpha = 0f;
			}
			if (poison > 0f)
			{
				float num11 = 1f - poison;
				if (num11 < 0f)
				{
					num11 = 0f;
				}
				val = new Color(new Vector4(num11, 1f, num11, 1f));
			}
			if (fire > 0f)
			{
				float num12 = 1f - fire;
				if (num12 < 0f)
				{
					num12 = 0f;
				}
				val = new Color(new Vector4(num12, num12, num12, 1f));
			}
			if (freeze > 0f)
			{
				float num13 = 1f - freeze;
				if (freeze < 0f)
				{
					freeze = 0f;
				}
				val = new Color(new Vector4(num13, num13, 1f, 1f));
			}
			if (dyingFrame >= 1f)
			{
				val = new Color(new Vector4(1f, 1f, 1f, 2f - dyingFrame));
				nameAlpha = 2f - dyingFrame;
			}
			bool flag2 = false;
			if (ps == 0)
			{
				if (part2.idx >= 24 && part2.idx / 64 == 0)
				{
					flag2 = true;
				}
				if (part2.idx == 8 || part2.idx == 9)
				{
					torsoVec = vector3;
					torsoVec -= new Vector2((float)Math.Sin(num5), (float)Math.Cos(num5)) * sScale * 24f;
				}
			}
			else
			{
				if (part2.idx < 24 || part2.idx / 64 != 0)
				{
					flag2 = true;
				}
				vector3 += vector;
			}
			if (all)
			{
				flag2 = true;
			}
			if (part2.idx >= 1000 || !flag2)
			{
				continue;
			}
			vector3 = GetAngleAdjustedVec(sLoc, vector3, angle);
			num5 += angle;
			int num14 = part2.idx / 64;
			int num15 = 2;
			Texture2D val2;
			switch (num14)
			{
			case 0:
			{
				if (ID >= 20)
				{
					perk[0] = (perk[1] = (perk[2] = -1));
				}
				bool flag3 = false;
				if (GameState.gameType == 4 && team == 1)
				{
					flag3 = true;
				}
				if (!flag3 && (part2.idx == 8 || part2.idx == 9))
				{
					int num17 = 0;
					if (jetPackFrame > 0f)
					{
						switch (jetpack)
						{
						case 1:
						case 2:
						case 3:
						case 4:
						case 6:
						case 8:
							num17 = (int)(jetPackFrame * 10f);
							if (num17 > 2)
							{
								num17 = 1;
							}
							break;
						}
					}
					float num18 = ((part2.idx == 8) ? 20f : 24f);
					Rectangle value2 = new Rectangle(((part2.idx != 8) ? 80 : 0) + jetpack * 160, 80 * num17, 80, 80);
					spriteBatch.Draw(Game1.jetpacks, vector3, (Rectangle?)value2, val, num5, new Vector2((!flag) ? (80f - num18) : num18, 40f), vector4, (!flag ? SpriteEffects.FlipHorizontally : SpriteEffects.None), 1f);
				}
				value = Game1.charTex[Game1.charDef[defIdx].charIdx].GetRect(part2.idx);
				spriteBatch.Draw(Game1.charTex[Game1.bodyCatalog.bodyType[bodyType].skinList[skinTex] * 2 + team].tex, vector3, (Rectangle?)value, val, num5, new Vector2((float)value.Width / 2f, (float)value.Height / 2f), vector4, (!flag ? SpriteEffects.FlipHorizontally : SpriteEffects.None), 1f);
				int num19 = -1;
				switch (part2.idx / 8)
				{
				case 0:
					if (Game1.bodyCatalog.bodyType[bodyType].clothesList != null && headTex > 0 && headTex <= Game1.bodyCatalog.bodyType[bodyType].clothesList.Length)
					{
						num19 = Game1.bodyCatalog.bodyType[bodyType].clothesList[headTex - 1] * 2 + team;
					}
					break;
				case 1:
				case 2:
					if (Game1.bodyCatalog.bodyType[bodyType].clothesList != null && torsoTex > 0 && torsoTex <= Game1.bodyCatalog.bodyType[bodyType].clothesList.Length)
					{
						num19 = Game1.bodyCatalog.bodyType[bodyType].clothesList[torsoTex - 1] * 2 + team;
					}
					break;
				case 3:
				case 4:
					if (Game1.bodyCatalog.bodyType[bodyType].clothesList != null && legsTex > 0 && legsTex <= Game1.bodyCatalog.bodyType[bodyType].clothesList.Length)
					{
						num19 = Game1.bodyCatalog.bodyType[bodyType].clothesList[legsTex - 1] * 2 + team;
					}
					break;
				}
				if (isRosterChar)
				{
					if (Game1.zProfile.EditingSet().bodyType == 0)
					{
						switch (part2.idx / 8)
						{
						case 0:
							num15 = Game1.zProfile.unlocks.BoyHeadUnlocked(Game1.menu.menuLevel[10].item[1].selX);
							break;
						case 1:
						case 2:
							num15 = Game1.zProfile.unlocks.BoyTorsoUnlocked(Game1.menu.menuLevel[10].item[2].selX);
							break;
						case 3:
						case 4:
							num15 = Game1.zProfile.unlocks.BoyLegsUnlocked(Game1.menu.menuLevel[10].item[3].selX);
							break;
						}
					}
					else
					{
						switch (part2.idx / 8)
						{
						case 0:
							num15 = Game1.zProfile.unlocks.GirlHeadUnlocked(Game1.menu.menuLevel[10].item[1].selX);
							break;
						case 1:
						case 2:
							num15 = Game1.zProfile.unlocks.GirlTorsoUnlocked(Game1.menu.menuLevel[10].item[2].selX);
							break;
						case 3:
						case 4:
							num15 = Game1.zProfile.unlocks.GirlLegsUnlocked(Game1.menu.menuLevel[10].item[3].selX);
							break;
						}
					}
				}
				try
				{
					val2 = ((num19 >= 0) ? Game1.charTex[num19].tex : null);
				}
				catch (Exception ex)
				{
					val2 = Game1.charTex[0].tex;
					Console.WriteLine(ex.StackTrace);
				}
				value = Game1.charTex[Game1.charDef[defIdx].charIdx].GetRect(part2.idx);
				break;
			}
			case 1:
				if (part2.idx == 73)
				{
					val2 = Game1.spritesTex;
					value = new Rectangle((grenType[lastGren] - 1) % 16 * 64, 320 + (grenType[lastGren] - 1) / 16 * 64, 64, 64);
				}
				else
				{
					val2 = Game1.weapTex[WeaponCatalog.weapons[weapon[curWeap]].idx].tex;
					value = Game1.weapTex[Game1.charDef[defIdx].weaponIdx].GetRect(part2.idx - 64);
					if (bodySec[0].animName == "suicide")
					{
						val2 = Game1.weapTex[0].tex;
						value = Game1.weapTex[0].GetRect(part2.idx - 64);
					}
					if (WeaponCatalog.weapons[weapon[curWeap]].projType == 16 && !submerged)
					{
						Vector2 vector5 = new Vector2((float)Math.Cos(num5), (float)Math.Sin(num5));
						float num16 = Rand.GetRandomFloat(-0.5f, 1f) * (flag ? (-1f) : 1f);
						Game1.pMan.AddParticle(1, ScrollManager.GetRealLoc(vector3 + vector5 * num16 * Scroll.zoom * 40f, 1f), Rand.GetRandomVec2(-10f, 10f, -200f, 0f), 0.6f, 0, -1);
					}
				}
				if (GameState.gameType == 4 && team == 1)
				{
					val2 = null;
				}
				break;
			case 2:
				val2 = Game1.pteroTex[0].tex;
				value = Game1.pteroTex[0].GetRect(part2.idx - 128);
				break;
			default:
				val2 = null;
				break;
			}
			if (val2 != null)
			{
				if (isRosterChar && (part2.idx == 8 || part2.idx == 9))
				{
					switch (Game1.zProfile.unlocks.jetpackUnlocked[Game1.menu.menuLevel[10].item[4].selX])
					{
					case 0:
						spriteBatch.Draw(Game1.spritesTex, vector3 + new Vector2(79f, 4f), (Rectangle?)new Rectangle(864, 128, 32, 32), val, 0f, new Vector2(16f, 16f), vector4, (SpriteEffects)0, 1f);
						break;
					case 1:
					{
						float size = Game1.text.size;
						Color color = Game1.text.color;
						Game1.text.size = 0.8f;
						Game1.text.color = new Color(1f, 1f, 1f, Rand.GetRandomFloat(0.5f, 1f));
						Game1.text.DrawString(vector3 + new Vector2(68f, 0f), Game1.menu.newString, 0, -1f, Game1.impact, spriteBatch);
						Game1.text.color = color;
						Game1.text.size = size;
						break;
					}
					}
				}
				if (num15 == 0)
				{
					switch (part2.idx)
					{
					default:
						spriteBatch.Draw(Game1.spritesTex, vector3 + new Vector2(-38f, 0f), (Rectangle?)new Rectangle(864, 128, 32, 32), val, 0f, new Vector2(16f, 16f), vector4, (SpriteEffects)0, 1f);
						break;
					case 10:
					case 11:
					case 12:
					case 13:
					case 16:
					case 17:
					case 18:
					case 19:
					case 20:
					case 21:
						break;
					}
				}
				else
				{
					spriteBatch.Draw(val2, vector3, (Rectangle?)value, val, num5, new Vector2((float)value.Width / 2f, (float)value.Height / 2f), vector4, (!flag ? SpriteEffects.FlipHorizontally : SpriteEffects.None), 1f);
					if (num15 == 1)
					{
						switch (part2.idx)
						{
						default:
						{
							float size2 = Game1.text.size;
							Color color2 = Game1.text.color;
							Game1.text.size = 0.8f;
							Game1.text.color = new Color(1f, 1f, 1f, Rand.GetRandomFloat(0.5f, 1f));
							Game1.text.DrawString(vector3 + new Vector2(58f, 0f), Game1.menu.newString, 0, -1f, Game1.impact, spriteBatch);
							Game1.text.color = color2;
							Game1.text.size = size2;
							break;
						}
						case 10:
						case 11:
						case 12:
						case 13:
						case 16:
						case 17:
						case 18:
						case 19:
						case 20:
						case 21:
							break;
						}
					}
				}
			}
			if (part2.idx / 8 == 0 && hatTex > 0 && hatTex <= Game1.bodyCatalog.bodyType[bodyType].hatList.Length)
			{
				int num20 = Game1.bodyCatalog.bodyType[bodyType].hatList[hatTex - 1] * 2 + team;
				val2 = Game1.charTex[num20].tex;
				switch (part2.idx)
				{
				case 0:
				case 1:
					value.X = 256;
					break;
				case 2:
				case 3:
					value.X = 320;
					break;
				}
				int num21 = 2;
				if (isRosterChar)
				{
					num21 = ((Game1.zProfile.EditingSet().bodyType != 0) ? Game1.zProfile.unlocks.GirlHatUnlocked(Game1.menu.menuLevel[10].item[0].selX) : Game1.zProfile.unlocks.BoyHatUnlocked(Game1.menu.menuLevel[10].item[0].selX));
				}
				if (num21 == 0)
				{
					spriteBatch.Draw(Game1.spritesTex, vector3 + new Vector2(0f, -32f), (Rectangle?)new Rectangle(864, 128, 32, 32), val, num5, new Vector2(16f, 16f), vector4, (SpriteEffects)0, 1f);
				}
				else
				{
					if (num20 == 76 || num20 == 77)
					{
						value.X = 256;
						if ((int)(bodySec[0].curFrame * 40f) % 2 == 0)
						{
							value.X = 320;
						}
					}
					spriteBatch.Draw(val2, vector3, (Rectangle?)value, val, num5, new Vector2((float)value.Width / 2f, (float)value.Height / 2f + 6f), vector4, (!flag ? SpriteEffects.FlipHorizontally : SpriteEffects.None), 1f);
					if (num21 == 1)
					{
						float size3 = Game1.text.size;
						Color color3 = Game1.text.color;
						Game1.text.size = 0.8f;
						Game1.text.color = new Color(1f, 1f, 1f, Rand.GetRandomFloat(0.5f, 1f));
						Game1.text.DrawString(vector3 + new Vector2(32f, -32f), Game1.menu.newString, 0, -1f, Game1.impact, spriteBatch);
						Game1.text.color = color3;
						Game1.text.size = size3;
					}
				}
			}
			if (fire > 0f)
			{
				if (fire > 20f)
				{
					fire = 20f;
				}
				float num22 = fire + (float)j;
				int num23 = (int)((20f - num22) / 0.4f * 9f);
				num23 %= 9;
				float num24 = fire;
				if (num24 > 0.6f)
				{
					num24 = 0.6f;
				}
				spriteBatch.Draw(Game1.spritesTex, vector3, (Rectangle?)new Rectangle(num23 * 32, 224, 32, 64), new Color(new Vector4(1f, 1f, 1f, num24)), 0f, new Vector2(16f, 50f), Scroll.zoom * 1.3f * new Vector2(1f, 1f + (float)Math.Cos((double)num22 * 8.0) * 0.2f), (SpriteEffects)0, 1f);
			}
		}
	}

	internal void SortGrenades()
	{
		for (int i = 0; i < grenType.Length; i++)
		{
			if (grenAmmo[i] <= 0)
			{
				for (int j = i; j < grenType.Length - 1; j++)
				{
					grenType[j] = grenType[j + 1];
					grenAmmo[j] = grenAmmo[j + 1];
				}
			}
		}
	}

	internal void CycleGrenades()
	{
		int num = grenType[0];
		int num2 = grenAmmo[0];
		for (int i = 0; i < grenType.Length; i++)
		{
			if (grenAmmo[i] <= 0)
			{
				grenAmmo[0] = 0;
				grenType[0] = 0;
				grenAmmo[i] = num2;
				grenType[i] = num;
				SortGrenades();
				break;
			}
		}
	}

	internal int GetHasWeapon(int w)
	{
		for (int i = 0; i < weapon.Length; i++)
		{
			if (weapon[i] > -1)
			{
				if (WeaponCatalog.weapons[weapon[i]].isAkimbo && w == weapon[i] - 100)
				{
					return 1;
				}
				if (weapon[i] == w)
				{
					if (WeaponCatalog.weapons[weapon[i]].canAkimbo)
					{
						return 2;
					}
					if (ammo[WeaponCatalog.weapons[weapon[i]].ammoType] < 999)
					{
						return 1;
					}
					return 3;
				}
				continue;
			}
			return 2;
		}
		return 0;
	}

	internal int CanPickupGren(int p)
	{
		for (int i = 0; i < grenType.Length; i++)
		{
			if (grenAmmo[i] > 0 && grenType[i] == p)
			{
				if (grenAmmo[i] < 5)
				{
					return 1;
				}
				return 3;
			}
		}
		return 2;
	}

	internal StringBuilder GetClanName(StringBuilder s)
	{
		if (ai != null)
		{
			return s;
		}
		if (clanChar[0] != 0 && clanChar[1] != 0 && clanChar[2] != 0)
		{
			s.Insert(0, "[" + clanChar[0] + clanChar[1] + clanChar[2] + "]");
		}
		else if (clanChar[0] != 0 && clanChar[1] != 0)
		{
			s.Insert(0, "[" + clanChar[0] + clanChar[1] + "]");
		}
		else if (clanChar[0] != 0)
		{
			s.Insert(0, "[" + clanChar[0] + "]");
		}
		return s;
	}
}
