using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZP2K9.hud.hudBlood;

public class HUDBlood
{
	private HUDBloodDrop[] drop = new HUDBloodDrop[128];

	private float frame;

	private float pA;

	public HUDBlood()
	{
		Reset();
	}

	public void Reset()
	{
		for (int i = 0; i < drop.Length; i++)
		{
			Vector2 randomVec = Rand.GetRandomVec2(0f, 1280f, 0f, 720f);
			float num = (randomVec - new Vector2(640f, 360f)).Length() / 800f;
			num -= 0.2f;
			num *= 1.5f;
			drop[i] = new HUDBloodDrop(randomVec, Rand.GetRandomFloat(4f, 6f), Rand.GetRandomRadian(), num, Rand.GetRandomInt(0, 5));
		}
	}

	public void Update()
	{
		frame += Game1.frameTime * 5f;
		if (frame > 6.28f)
		{
			frame -= 6.28f;
		}
	}

	public void Draw(SpriteBatch sprite, float alpha)
	{
		if (pA <= 0f && alpha > 0f)
		{
			Reset();
		}
		if (alpha > 1f)
		{
			alpha = 1f;
		}
		for (int i = 0; i < drop.Length; i++)
		{
			drop[i].Draw(sprite, frame + (float)i, alpha);
		}
		pA = alpha;
	}
}
