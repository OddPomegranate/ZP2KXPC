using System.Text;

namespace ZP2K9.hud;

public class Numbers
{
	public static StringBuilder[] number;

	public static StringBuilder[] time;

	public static void Init()
	{
		number = new StringBuilder[10000];
		for (int i = 0; i < number.Length; i++)
		{
			number[i] = new StringBuilder(i.ToString());
		}
		time = new StringBuilder[600];
		for (int j = 0; j < time.Length; j++)
		{
			int num = j / 60;
			int num2 = j % 60;
			time[j] = new StringBuilder(num + ((num2 < 10) ? ":0" : ":") + num2);
		}
	}

	public static StringBuilder GetNumber(int i)
	{
		if (i < 0)
		{
			return number[i];
		}
		if (i < number.Length)
		{
			return number[i];
		}
		return number[number.Length - 1];
	}

	public static StringBuilder GetTime(int i)
	{
		if (i < 0)
		{
			return time[0];
		}
		if (i < time.Length)
		{
			return time[i];
		}
		return time[time.Length - 1];
	}
}
