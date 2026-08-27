using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZP2K9.hud;

public class Popup
{
	public const float DEATH_TIME = 5f;

	public const float KILL_TIME = 1f;

	public const float KILLSTREAK_TIME = 1f;

	public const float MULTIKILL_TIME = 1f;

	public const float LEVELUP_TIME = 2f;

	private float frame;

	private PopupBlip[] stringQueue;

	private int cur;

	private int start;

	private int total;

	public Popup()
	{
		stringQueue = new PopupBlip[8];
		for (int i = 0; i < stringQueue.Length; i++)
		{
			stringQueue[i] = new PopupBlip();
		}
	}

	public bool IsActive()
	{
		if (frame > 0f)
		{
			return stringQueue[start].unlockType > -1;
		}
		return false;
	}

	public void Add(string add, int points, HUD hUD, float duration)
	{
		if (total < stringQueue.Length)
		{
			stringQueue[cur].level = -1;
			stringQueue[cur].str = new StringBuilder(add);
			stringQueue[cur].str2 = null;
			stringQueue[cur].points = points;
			stringQueue[cur].duration = duration;
			stringQueue[cur].unlockType = -1;
			stringQueue[cur].unlockIdx = -1;
			if (frame <= 0f)
			{
				frame = duration;
				hUD.AddPopScore(stringQueue[cur].points);
				total = 0;
			}
			cur = (cur + 1) % stringQueue.Length;
			total++;
		}
	}

	public void Add(string add, int unlockType, int unlockIdx, int level, HUD hUD, float duration)
	{
		if (total >= stringQueue.Length)
		{
			return;
		}
		stringQueue[cur].str = new StringBuilder(add);
		stringQueue[cur].unlockIdx = unlockIdx;
		stringQueue[cur].unlockType = unlockType;
		stringQueue[cur].points = -1;
		stringQueue[cur].level = level;
		switch (unlockType)
		{
		case -1:
			stringQueue[cur].str2 = new StringBuilder("");
			break;
		case 8:
			stringQueue[cur].str2 = new StringBuilder("Unlocked appearance editor!");
			break;
		case 9:
			stringQueue[cur].str2 = new StringBuilder("Unlocked skill editor!");
			break;
		case 10:
			stringQueue[cur].str2 = new StringBuilder("Unlocked class renaming!");
			break;
		case 11:
			stringQueue[cur].str2 = new StringBuilder("Unlocked clan tags!");
			break;
		case 1:
		case 3:
			stringQueue[cur].str2 = new StringBuilder("Unlocked new clothes!");
			break;
		case 12:
			stringQueue[cur].str2 = new StringBuilder("Unlocked new jetpack!");
			break;
		case 2:
			stringQueue[cur].str2 = new StringBuilder("Unlocked new hat!");
			break;
		case 5:
		case 6:
		case 7:
			stringQueue[cur].str2 = new StringBuilder("Unlocked new skill!");
			break;
		case 0:
			if (Game1.zProfile.unlocks.perkEditorUnlocked > 0)
			{
				stringQueue[cur].str2 = new StringBuilder("Unlocked new class slot!");
			}
			else
			{
				stringQueue[cur].str2 = new StringBuilder("Unlocked new class!");
			}
			break;
		}
		stringQueue[cur].duration = duration;
		if (frame <= 0f)
		{
			frame = duration;
			hUD.AddPopScore(stringQueue[cur].points);
			total = 0;
			Sound.DoLevup();
		}
		cur = (cur + 1) % stringQueue.Length;
		total++;
	}

	internal void Update(HUD hUD)
	{
		if (!(frame > 0f))
		{
			return;
		}
		frame -= Game1.frameTime;
		if (!(frame <= 0f))
		{
			return;
		}
		start = (start + 1) % stringQueue.Length;
		total--;
		if (start == cur)
		{
			frame = 0f;
			return;
		}
		if (stringQueue[start].points > -1)
		{
			hUD.AddPopScore(stringQueue[start].points);
		}
		frame = stringQueue[start].duration;
		if (stringQueue[start].unlockType > -1)
		{
			Sound.DoLevup();
		}
	}

	internal void Draw(SpriteBatch sprite)
	{
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0523: Unknown result type (might be due to invalid IL or missing references)
		//IL_0655: Unknown result type (might be due to invalid IL or missing references)
		//IL_0492: Unknown result type (might be due to invalid IL or missing references)
		//IL_040c: Unknown result type (might be due to invalid IL or missing references)
		if (frame <= 0f || stringQueue[start].str == null)
		{
			return;
		}
		float num = 1f;
		Game1.text.size = 2f;
		if (frame < 0.25f)
		{
			Game1.text.size -= (0.25f - frame) * 2f;
			num = frame * 4f;
		}
		if (frame > stringQueue[start].duration - 0.25f)
		{
			Game1.text.size += (frame - (stringQueue[start].duration - 0.25f)) * 4f;
			num = (stringQueue[start].duration - frame) * 4f;
		}
		if (stringQueue[start].level > -1)
		{
			sprite.Draw(Game1.badgesTex, new Vector2(640f, 100f), (Rectangle?)new Rectangle(stringQueue[start].level % 10 * 128, stringQueue[start].level / 10 * 128, 128, 128), new Color(0f, 0f, 0f, num * 0.1f), 0f, new Vector2(64f, 64f), Game1.text.size * 2f, (SpriteEffects)0, 1f);
			sprite.Draw(Game1.badgesTex, new Vector2(640f, 100f), (Rectangle?)new Rectangle(stringQueue[start].level % 10 * 128, stringQueue[start].level / 10 * 128, 128, 128), new Color(1f, 1f, 1f, num), 0f, new Vector2(64f, 64f), Game1.text.size * 0.3f, (SpriteEffects)0, 1f);
		}
		Game1.text.color = new Color(1f, 1f, 1f, num);
		float num2 = ((stringQueue[start].level > -1) ? 30f : 0f);
		Game1.text.DrawString(new Vector2(640f, 100f + num2), stringQueue[start].str, 1, -1f, Game1.impact, sprite);
		if (stringQueue[start].str2 == null)
		{
			return;
		}
		Game1.text.DrawString(new Vector2(640f, 150f + num2), stringQueue[start].str2, 1, -1f, Game1.impact, sprite);
		switch (stringQueue[start].unlockType)
		{
		case 5:
		case 6:
		case 7:
		{
			Rectangle value = new Rectangle(768, 0, 128, 128);
			if (stringQueue[start].unlockType == 5)
			{
				value.X = 896;
			}
			if (stringQueue[start].unlockType == 7)
			{
				value.X = 1024;
			}
			value.Y = stringQueue[start].unlockIdx * 128;
			sprite.Draw(Game1.perksTex, new Vector2(640f, 250f + num2), (Rectangle?)value, new Color(1f, 1f, 1f, num), 0f, new Vector2(64f, 64f), Game1.text.size / 4f, (SpriteEffects)0, 1f);
			break;
		}
		case 12:
			sprite.Draw(Game1.jetpacks, new Vector2(640f, 250f + num2), (Rectangle?)new Rectangle(stringQueue[start].unlockIdx * 160 + 80, 0, 80, 80), new Color(1f, 1f, 1f, num), 0f, new Vector2(64f, 64f), 1f, (SpriteEffects)0, 1f);
			break;
		case 2:
			sprite.Draw(Game1.charTex[Game1.bodyCatalog.bodyType[0].hatList[stringQueue[start].unlockIdx] * 2].tex, new Vector2(640f, 230f + num2), (Rectangle?)new Rectangle(256, 0, 64, 64), new Color(1f, 1f, 1f, num), 0f, new Vector2(32f, 32f), Game1.text.size * 0.6f, (SpriteEffects)0, 1f);
			break;
		case 1:
			sprite.Draw(Game1.charTex[Game1.bodyCatalog.bodyType[0].clothesList[stringQueue[start].unlockIdx] * 2].tex, new Vector2(640f, 230f + num2), (Rectangle?)new Rectangle(0, 64, 64, 64), new Color(1f, 1f, 1f, num), 0f, new Vector2(32f, 32f), Game1.text.size * 0.6f, (SpriteEffects)0, 1f);
			break;
		case 3:
			sprite.Draw(Game1.charTex[Game1.bodyCatalog.bodyType[1].clothesList[stringQueue[start].unlockIdx] * 2].tex, new Vector2(640f, 230f + num2), (Rectangle?)new Rectangle(0, 64, 64, 64), new Color(1f, 1f, 1f, num), 0f, new Vector2(32f, 32f), Game1.text.size * 0.6f, (SpriteEffects)0, 1f);
			break;
		case 4:
		case 8:
		case 9:
		case 10:
		case 11:
			break;
		}
	}
}
