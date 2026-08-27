using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ZP2K9.debug;

public class DebugManager
{
	public enum JoinQuitPhase
	{
		Join,
		Play,
		Quit
	}

	public static bool joinQuitMode;

	public static bool mapTestMode;

	public static float joinQuitFrame;

	public static JoinQuitPhase joinQuitPhase;

	public static bool showAIDest;

	public static bool showNodeIndices;

	public static bool showAIPaths;

	public static bool godMode;

	public static bool botsIgnore;

	public static bool jumpToLevUp;

	public static bool aiFollow;

	public static bool fakeRealPlayers;

	public static bool hideHud;

	public static bool jumpToNullMe;

	public static void Update()
	{
		if (Game1.mainPlayerIndex <= -1 || !joinQuitMode)
		{
			return;
		}
		if (Game1.menu.menuLevel[12].active)
		{
			TryClick(12, 0);
		}
		if (GamePad.GetState((PlayerIndex)Game1.mainPlayerIndex).Buttons.Back == ButtonState.Pressed)
		{
			joinQuitMode = false;
		}
		switch (joinQuitPhase)
		{
		case JoinQuitPhase.Join:
			if (Game1.menu.IsActive())
			{
				TryClick(0, 0);
				TryClick(6, 1);
				TryClick(8, 1);
			}
			else
			{
				joinQuitPhase = JoinQuitPhase.Play;
				joinQuitFrame = Rand.GetRandomFloat(-10f, 0f);
			}
			break;
		case JoinQuitPhase.Play:
			if (joinQuitFrame > 5f)
			{
				Game1.menu.menuLevel[9].active = true;
				if (TryClick(9, 9))
				{
					joinQuitPhase = JoinQuitPhase.Quit;
					joinQuitFrame = 1f;
				}
			}
			else
			{
				Game1.menu.menuLevel[9].active = true;
				Game1.menu.menuLevel[9].selected = 3;
				Game1.menu.menuLevel[9].item[3].selX = 1;
				Game1.menu.menuLevel[9].item[2].selX = Game1.zProfile.defaultTeam;
			}
			joinQuitFrame += Game1.frameTime;
			break;
		case JoinQuitPhase.Quit:
			joinQuitFrame -= Game1.frameTime;
			if (joinQuitFrame < 0f)
			{
				joinQuitPhase = JoinQuitPhase.Join;
			}
			break;
		}
	}

	private static bool TryClick(int level, int idx)
	{
		if (Game1.menu.menuLevel[level].active && Game1.menu.menuLevel[level].alpha >= 1f)
		{
			Game1.menu.menuLevel[level].selected = idx;
			Game1.menu.menuLevel[level].SelectItem(Game1.menu);
			return true;
		}
		return false;
	}

	internal static void StartAutoJoin()
	{
		joinQuitMode = true;
		joinQuitFrame = 0f;
	}
}
