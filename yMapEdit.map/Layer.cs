namespace yMapEdit.map;

public class Layer
{
	public enum Level
	{
		Back2,
		Back1,
		Mid,
		Fore1,
		Fore2
	}

	public Segment[] segment;

	public Level level;

	public float zoom;

	public float adjustedZoom;

	public Layer(Level level)
	{
		segment = new Segment[1024];
		this.level = level;
	}
}
