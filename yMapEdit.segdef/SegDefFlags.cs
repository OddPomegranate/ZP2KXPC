namespace yMapEdit.segdef;

internal class SegDefFlags
{
	public const int FLAG_NONE = 0;

	public const int FLAG_DRIPPABLE = 1;

	public const int FLAG_LIGHT_SPOT = 2;

	public const int FLAG_LIGHT_LANTERN = 3;

	public const int FLAG_LIGHT_CANDLE = 4;

	public const int FLAG_LIGHT_RED = 5;

	public const int FLAG_SWAY = 6;

	public const int FLAG_SLOW_SWAY = 7;

	public const int FLAG_LAYER_BUMP = 8;

	public const int FLAG_FOG = 9;

	public const int FLAG_LIGHT_GREEN = 10;

	public const int FLAG_LIGHT_WHITE = 11;

	public const int FLAG_TV_1 = 12;

	public const int FLAG_TV_2 = 13;

	public const int FLAG_TV_3 = 14;

	public const int FLAG_TV_4 = 15;

	public const int FLAG_SPIN = 16;

	public const int FLAG_BUBBLES = 17;

	public const int FLAG_SOFTGLOW = 18;

	public const int FLAG_BLOOD = 19;

	public const int FLAG_RED_LINE = 20;

	public const int FLAG_SUNBEAM = 21;

	public const int FLAG_BIGTV = 22;

	public const int FLAG_LIGHTFAN_BIG = 23;

	public const int FLAG_LIGHTFAN_SMALL = 24;

	public const int FLAG_SCROLL_MONITOR_FULL = 25;

	public const int FLAG_SCROLL_MONITOR2_FULL = 26;

	public const int FLAG_SCROLL_MONITOR_MID = 27;

	public const int FLAG_SCROLL_MONITOR2_MID = 28;

	public const int FLAG_SCROLL_MONITOR_SMALL = 29;

	public const int FLAG_SCROLL_MONITOR2_SMALL = 30;

	public const int FLAG_SCROLL_MONITOR_TINY = 31;

	public const int FLAG_SCROLL_MONITOR2_TINY = 32;

	public const int FLAG_SCROLL_MONITOR_PUNY = 33;

	public const int FLAG_SCROLL_MONITOR2_PUNY = 34;

	public const int FLAG_CANDLE = 35;

	public const int FLAG_CHAIN = 36;

	public const int FLAG_GEAR_CW = 37;

	public const int FLAG_GEAR_CCW = 38;

	public const int FLAG_SMALLGEAR_CW = 39;

	public const int FLAG_SMALLGEAR_CCW = 40;

	public const int FLAG_WHEEL = 41;

	public const int FLAG_GRASS = 42;

	public const int FLAG_LEAVES = 43;

	public const int FLAG_TREAD_WHEEL = 44;

	public const int FLAG_TREADS = 45;

	public const int FLAG_ROBOTSWING = 46;

	public const int FLAG_LEFT_SENSOR = 47;

	public const int FLAG_RIGHT_SENSOR = 48;

	public const int FLAG_TREAD_MIDWHEEL = 49;

	public const int FLAG_LIGHTBOARD = 50;

	public const int FLAG_WATERFALL = 51;

	public const int FLAG_LIFT = 52;

	public const int FLAG_LIFT_GLASS = 53;

	public const int FLAG_LIFT_GLASS_TRIG = 54;

	public const int FLAG_GLOW_SRC = 55;

	public const int FLAG_MAP_MONITOR = 56;

	public const int FLAG_GO_UP = 57;

	public const int FLAG_GO_LEFT = 58;

	public const int FLAG_GO_DOWN = 59;

	public const int FLAG_GO_RIGHT = 60;

	public const int FLAG_SMALL_PRISON_TV = 61;

	public const int FLAG_BIG_PRISON_TV = 62;

	public const int FLAG_TESLA = 63;

	public const int FLAG_BANKER_PORTRAIT = 64;

	public const int FLAG_GENERAL_PORTRAIT = 65;

	public const int FLAG_JUDGE_PORTRAIT = 66;

	public const int FLAG_CHURCH_MAINLIGHT = 67;

	public const int FLAG_CHURCH_SLATLIGHT = 68;

	public const int FLAG_RED_CRYSTAL_L = 69;

	public const int FLAG_YELLOW_CRYSTAL_L = 70;

	public const int FLAG_BLUE_CRYSTAL_L = 71;

	public const int FLAG_GREEN_CRYSTAL_L = 72;

	public const int FLAG_RED_CRYSTAL_M = 73;

	public const int FLAG_YELLOW_CRYSTAL_M = 74;

	public const int FLAG_BLUE_CRYSTAL_M = 75;

	public const int FLAG_GREEN_CRYSTAL_M = 76;

	public const int FLAG_GLACIER = 77;

	public const int FLAG_FLOODLIGHTS_LEFT = 78;

	public const int FLAG_FLOODLIGHTS_RIGHT = 79;

	public static string[] flagNames = new string[80]
	{
		"none", "drippable", "light spot", "light lantern", "light candle", "light red", "sway", "slow sway", "layer bump", "fog",
		"light green", "light white", "TV1", "TV2", "TV3", "TV4", "spin", "bubbles", "soft glow", "blood",
		"red line", "sunbeam", "bigtv", "lightfan big", "lightfan small", "scrollmonitor full", "scrollmonitor2 full", "scrollmonitor mid", "scrollmonitor2 mid", "scrollmonitor small",
		"scrollmonitor2 small", "scrollmonitor tiny", "scrollmonitor2 tiny", "scrollmonitor puny", "scrollmonitor2 puny", "candle", "chain", "gear cw", "gear ccw", "smallgear cw",
		"smallgear ccw", "wheel", "grass", "leaves", "treadwheel", "treads", "robotswing", "left sensor", "right sensor", "tread midwheel",
		"lightboard", "waterfall", "lift", "lift glass", "lift glass trig", "glow src", "map monitor", "go up", "go left", "go down",
		"go right", "small prison tv", "big prison tv", "tesla", "banker portrait", "general portrait", "judge portrait", "church mainlight", "church slatlight", "red crystal l",
		"yellow crystal l", "blue crystal l", "green crystal l", "red crystal m", "yellow crystal m", "blue crystal m", "green crystal m", "glacier", "floodlights left", "floodlights right"
	};
}
