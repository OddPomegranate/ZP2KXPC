namespace ZP2K9.store.body;

public class BodyType
{
	public int[] skinList;

	public int[] clothesList;

	public int[] hatList;

	public BodyType(int[] skinList, int[] clothesList, int[] hatList)
	{
		if (skinList != null)
		{
			this.skinList = new int[skinList.Length];
			for (int i = 0; i < skinList.Length; i++)
			{
				this.skinList[i] = skinList[i];
			}
		}
		if (clothesList != null)
		{
			this.clothesList = new int[clothesList.Length];
			for (int j = 0; j < clothesList.Length; j++)
			{
				this.clothesList[j] = clothesList[j];
			}
		}
		if (hatList != null)
		{
			this.hatList = new int[hatList.Length];
			for (int k = 0; k < hatList.Length; k++)
			{
				this.hatList[k] = hatList[k];
			}
		}
	}
}
