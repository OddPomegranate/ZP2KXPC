namespace xCharEdit.Character;

public class Animation
{
	public string name;

	private KeyFrame[] keyFrame;

	public Animation()
	{
		name = "";
		keyFrame = new KeyFrame[256];
		for (int i = 0; i < keyFrame.Length; i++)
		{
			keyFrame[i] = new KeyFrame();
		}
	}

	public void ClearKey(int idx)
	{
		keyFrame[idx] = new KeyFrame();
	}

	public KeyFrame GetKeyFrame(int idx)
	{
		if (idx < 0)
		{
			idx = 0;
		}
		return keyFrame[idx];
	}

	public void SetKeyFrame(int idx, KeyFrame _keyFrame)
	{
		keyFrame[idx] = _keyFrame;
	}

	public KeyFrame[] getKeyFrameArray()
	{
		return keyFrame;
	}
}
