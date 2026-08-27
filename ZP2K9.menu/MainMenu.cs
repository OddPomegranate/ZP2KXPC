using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ZP2K9.menu;

public class MainMenu
{
	private float frame;

	private float scroll;

	private float alpha;

	private float inAlpha;

	public bool active;

	private RenderTarget2D sceneTarg;

	private Effect sceneEffect;

	private float sat;

	private float brite;

	public MainMenu(GraphicsDevice dev, ContentManager Content)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Expected O, but got Unknown
		frame = 1024f;
		scroll = 1024f;
		alpha = 1f;
		active = true;
		sceneEffect = Content.Load<Effect>("fx/scene");
		sceneTarg = new RenderTarget2D(dev, 1280, 720, false, SurfaceFormat.Color, DepthFormat.None);
	}

	public bool IsSolid()
	{
		if (alpha >= 1f)
		{
			return true;
		}
		return false;
	}

	public void Update()
	{
		if (inAlpha < 1f)
		{
			inAlpha += Game1.frameTime;
			if (inAlpha > 1f)
			{
				inAlpha = 1f;
			}
		}
		if (active)
		{
			if (alpha < 1f)
			{
				alpha += Game1.frameTime * 2f;
			}
			if (alpha >= 1f)
			{
				alpha = 1f;
			}
		}
		else
		{
			if (alpha > 0f)
			{
				alpha -= Game1.frameTime * 2f;
			}
			if (alpha < 0f)
			{
				alpha = 0f;
			}
		}
		if (brite < 1f)
		{
			brite += Game1.frameTime;
			if (brite > 1f)
			{
				brite = 1f;
			}
		}
		if (!(alpha > 0f))
		{
			return;
		}
		float num = ((!Game1.menu.menuLevel[13].active) ? 1f : 0f);
		if (sat > num)
		{
			sat -= Game1.frameTime;
			if (sat < num)
			{
				sat = num;
			}
		}
		if (sat < num)
		{
			sat += Game1.frameTime;
			if (sat > num)
			{
				sat = num;
			}
		}
		Game1.sceneMgr.Update();
		frame += Game1.frameTime;
		scroll += Game1.frameTime * 10f;
		if (scroll > 1536f)
		{
			scroll -= 1024f;
		}
	}

	// returnTarget: the render target that should be active again once this
	// returns, i.e. whatever Draw()'s caller wants subsequent drawing to land
	// in - added 2026-08-23 for the scaling/fullscreen window support (see
	// Game1.cs's uiTarg field comment). Used to be a hardcoded
	// SetRenderTarget((RenderTarget2D)null) ("go back to the real backbuffer"),
	// which broke once Game1.Draw() started compositing everything into a
	// fixed 1280x720 uiTarg instead of drawing straight to the backbuffer.
	public void Prepare(SpriteBatch sprite, GraphicsDevice dev, RenderTarget2D returnTarget)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (!(alpha <= 0f))
		{
			dev.SetRenderTarget(sceneTarg);
			dev.Clear(Color.Black);
			sprite.Begin(blendState: BlendState.AlphaBlend);
			Game1.sceneMgr.Draw(sprite);
			sprite.End();
			dev.SetRenderTarget(returnTarget);
		}
	}

	public void Draw(SpriteBatch sprite)
	{
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		if (alpha <= 0f)
		{
			return;
		}
		sceneEffect.Parameters["alpha"].SetValue(alpha);
		sceneEffect.Parameters["burn"].SetValue(1.5f + sat * 1f);
		sceneEffect.Parameters["add"].SetValue(sat * 0.4f - 1f + brite);
		sceneEffect.Parameters["sat"].SetValue(1f + sat * 0.99f);
		sprite.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, sceneEffect);
		sprite.Draw(sceneTarg, new Rectangle(0, 0, 1280, 720), Color.White);
		sprite.End();
		sprite.Begin(blendState: BlendState.Additive);
		Game1.text.size = 1f;
		Game1.text.color = new Color(1f, 1f, 1f, alpha);
		Game1.text.DrawString(new Vector2(1150f, 560f), Game1.netSession.version, 2, -1f, Game1.impact, sprite);
		if (Game1.netSession.newVersAvailable)
		{
			Game1.text.color = new Color(1f, 0.4f, 0.4f, alpha);
			for (int i = 0; i < Game1.netSession.newAvail.Length; i++)
			{
				Game1.text.DrawString(new Vector2(1150f, 420f + (float)i * 20f), Game1.netSession.newAvail[i], 2, -1f, Game1.impact, sprite);
			}
		}
		sprite.End();
		if (GameState.mode == 2 && alpha > 0f)
		{
			sprite.Begin(blendState: BlendState.AlphaBlend);
			Game1.ticker.Draw(sprite, alpha);
			sprite.End();
		}
	}
}
