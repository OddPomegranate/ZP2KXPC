using System;
using System.IO;
using System.Threading;
using Microsoft.Xna.Framework;
using ZP2K9.platform;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ZP2K9.net;
using SceneEdit.scene;
using xCharEdit;
using xCharEdit.Character;
using yMapEdit.map;
using yMapEdit.map.postglow;
using yMapEdit.segdef;
using ZP2K9.ai;
using ZP2K9.characters;
using ZP2K9.characters.weapons;
using ZP2K9.debug;
using ZP2K9.hud;
using ZP2K9.map;
using ZP2K9.menu;
using ZP2K9.menu.levels;
using ZP2K9.particles;
using ZP2K9.store;

namespace ZP2K9;

public class Game1 : Game
{
	public const int DEF_HUMAN = 0;

	public const int DEF_PTERO = 1;

	public const int DEF_FISH = 2;

	private GraphicsDeviceManager graphics;

	private SpriteBatch spriteBatch;

	private RenderTarget2D rTarg;

	private RenderTarget2D backTarg;

	private RenderTarget2D liteTarg;

	// Fixed-logical-resolution scaling/fullscreen support (added 2026-08-23) -
	// see the header comment on RecalculateScreenDestRect() below for the full
	// design. uiTarg is the same idea as rTarg/liteTarg/backTarg above (a fixed
	// 1280x720 offscreen buffer everything draws into) but one level further
	// out: EVERYTHING that used to draw straight to the real backbuffer (the
	// final gameplay composite, HUD, main menu, in-game menu, keyboard-entry
	// overlay) now draws into uiTarg instead, and uiTarg itself gets blitted to
	// the real backbuffer - scaled and letterboxed/pillarboxed to whatever the
	// actual window size is - as the very last step of Draw(). Every other file
	// in the project (MainMenu.cs, HUD.cs, Menu.cs, KeyboardOverlay.cs, etc.)
	// needed zero changes for this - they already only ever assumed a fixed
	// 1280x720 logical canvas, which is still exactly what they're drawing into.
	private RenderTarget2D uiTarg;

	// Destination rectangle (in real backbuffer pixels) that the fixed 1280x720
	// uiTarg gets blitted into - see RecalculateScreenDestRect(). Exposed as a
	// public static so KeyboardMouseInput.cs can convert raw window mouse
	// coordinates into the game's logical 1280x720 space for mouse-aim. Starts
	// equal to the old fixed 1:1 window size so nothing divides by zero before
	// the first real resize/ApplyChanges happens.
	public static Rectangle ScreenDestRect = new Rectangle(0, 0, 1280, 720);

	private bool _handlingClientSizeChanged;

	private bool _prevFullscreenToggleDown;

	private Effect blurEffect;

	private Effect mainEffect;

	private Effect mainliteEffect;

	public static FlashLight flashLight;

	public static PostGlowManager postGlowMgr;

	public static ZProfile zProfile;

	public static PerkDescriptions perkDescriptions;

	public static BodyCatalog bodyCatalog;

	public static CharTexture[] charTex;

	public static CharTexture[] weapTex;

	public static CharTexture[] pteroTex;

	public static Texture2D jetpacks;

	public static Texture2D zp2kxTex;

	public static Texture2D[] mapTex;

	public static Texture2D iconsTex;

	public static Texture2D nullTex;

	public static Texture2D spritesTex;

	public static Texture2D[] backTex;

	public static Texture2D[] foreBackTex;

	public static Texture2D logoTex;

	public static Texture2D skaLogoTex;

	public static Texture2D controlsTex;

	public static NodeMgr nodeMgr;

	public static Texture2D perksTex;

	public static float frameTime = 0f;

	public static CharDef[] charDef;

	public static Character[] character = new Character[32];

	public static Pterodactyl[] pterodactyl = new Pterodactyl[64];

	public static Fish[] fish = new Fish[32];

	public static Loader loader;

	public static Character rosterChar;

	public static GameMap gameMap;

	public static HUD hud;

	public static InterfaceKeys[] iKeys = new InterfaceKeys[4];

	public static SegDefManager segDefMgr;

	public static ParticleManager pMan;

	public static SceneMgr sceneMgr;

	public static Text text;

	public static SpriteFont impact;

	public static Texture2D badgesTex;

	public static Ticker ticker;

	public static Settings settings;

	public static Store store;

	public static Menu menu;

	public static float gravity = 1500f;

	public static NetSession netSession;

	public static bool needsExit;

	public static int mainPlayerIndex = -1;

	private MainMenu mainMenu;

	private Thread loaderThread;

	public static bool handlingInvite = false;

	public static InviteAcceptedEventArgs ie;

	public static bool inviteHandled = false;

	public static BotBag botBag;

	public Game1()
	{
		graphics = new GraphicsDeviceManager(this);
		base.Content.RootDirectory = "Content";
		// No GamerServicesComponent on PC - MonoGame doesn't have Xbox Live/GFWL
		// services to pump each frame, so there's nothing to add here.
		base.IsFixedTimeStep = false;
		// Scaling/fullscreen window support (2026-08-23) - the OS window can now
		// be freely resized (and toggled fullscreen, see UpdateFullscreenToggle()
		// in Update()); the game itself still always renders at a fixed logical
		// 1280x720 (see uiTarg above), so resizing the window just changes how
		// big that fixed image is drawn, not what's drawn. ClientSizeChanged
		// fires both for a real user drag AND as a side effect of our own
		// ApplyChanges() call, so OnClientSizeChanged guards against re-entering
		// itself via _handlingClientSizeChanged.
		Window.AllowUserResizing = true;
		Window.ClientSizeChanged += OnClientSizeChanged;
	}

	protected override void Initialize()
	{
		Rand.rand = new Random();
		ScrollManager.screenSize = new Vector2(1280f, 720f);
		loader = new Loader();
		Special.InitNames();
		WeaponCatalog.Initialize();
		flashLight = new FlashLight();
		postGlowMgr = new PostGlowManager();
		perkDescriptions = new PerkDescriptions();
		graphics.PreferMultiSampling = false;
		graphics.PreferredBackBufferWidth = 1280;
		graphics.PreferredBackBufferHeight = 720;
		graphics.SynchronizeWithVerticalRetrace = true;
		// Storage optimization (2026-08-24): must match Content.mgcb's /profile:
		// directive (also switched to HiDef alongside this). MonoGame throws at
		// content-load time if the runtime GraphicsProfile doesn't match what
		// the content was built for. HiDef only requires DXT-compressed texture
		// dimensions to be a multiple of 4 (Reach requires full power-of-two),
		// which is what unlocked compressing 144 of this game's 185 textures -
		// they're multiple-of-4 but not power-of-two. HiDef is a strict
		// superset of Reach's capabilities, so nothing that worked before is
		// lost, only unlocked (Reach exists for old mobile/Xbox 360-era
		// hardware constraints that don't apply to a Windows desktop game).
		graphics.GraphicsProfile = GraphicsProfile.HiDef;
		graphics.ApplyChanges();
		RecalculateScreenDestRect();
		bodyCatalog = new BodyCatalog();
		pterodactyl = new Pterodactyl[64];
		for (int i = 0; i < pterodactyl.Length; i++)
		{
			pterodactyl[i] = new Pterodactyl();
		}
		fish = new Fish[32];
		for (int j = 0; j < fish.Length; j++)
		{
			fish[j] = new Fish();
		}
		Numbers.Init();
		Leveling.Init();
		zProfile = new ZProfile();
		zProfile.unlocks.LockAll();
		zProfile.unlocks.UpdateUnlocks();
		zProfile.ApplyDebugMaxLevel();
		botBag = new BotBag();
		// Real PC keyboard-input overlay (ZP2K9.platform/KeyboardOverlay.cs) -
		// needs the live GameWindow to subscribe TextInput once, up front.
		KeyboardOverlay.Initialize(Window);
		base.Initialize();
	}

	// Re-syncs the actual backbuffer to match a user window-drag, then
	// recomputes ScreenDestRect. No-ops while fullscreen (fullscreen changes
	// come through UpdateFullscreenToggle() instead, which calls
	// RecalculateScreenDestRect() itself) and guards against re-entering
	// itself, since ApplyChanges() below raises another ClientSizeChanged.
	private void OnClientSizeChanged(object sender, EventArgs e)
	{
		if (_handlingClientSizeChanged || graphics.IsFullScreen)
		{
			return;
		}
		int w = Window.ClientBounds.Width;
		int h = Window.ClientBounds.Height;
		if (w <= 0 || h <= 0)
		{
			return;
		}
		_handlingClientSizeChanged = true;
		graphics.PreferredBackBufferWidth = w;
		graphics.PreferredBackBufferHeight = h;
		graphics.ApplyChanges();
		RecalculateScreenDestRect();
		_handlingClientSizeChanged = false;
	}

	// Computes the largest 1280x720-aspect (16:9) rectangle, in real backbuffer
	// pixels, that fits inside the current backbuffer - i.e. "scale to fit,
	// keep aspect ratio, letterbox/pillarbox the rest in black". uiTarg (the
	// fixed 1280x720 offscreen buffer everything actually draws into, see the
	// field comment above) gets blitted into exactly this rectangle as the
	// last step of Draw(); the backbuffer is cleared to black first every
	// frame, so the letterbox/pillarbox bars just fall out of that for free.
	private void RecalculateScreenDestRect()
	{
		int bbWidth = graphics.GraphicsDevice.PresentationParameters.BackBufferWidth;
		int bbHeight = graphics.GraphicsDevice.PresentationParameters.BackBufferHeight;
		if (bbWidth <= 0 || bbHeight <= 0)
		{
			return;
		}
		float scale = Math.Min((float)bbWidth / 1280f, (float)bbHeight / 720f);
		int destWidth = (int)(1280f * scale);
		int destHeight = (int)(720f * scale);
		int destX = (bbWidth - destWidth) / 2;
		int destY = (bbHeight - destHeight) / 2;
		ScreenDestRect = new Rectangle(destX, destY, destWidth, destHeight);
	}

	// F11 or Alt+Enter toggles fullscreen - both are checked directly here (not
	// routed through CharKeys/InterfaceKeys or KeyboardMouseInput) since this is
	// OS window chrome, not a gameplay/menu action, matching the existing
	// Enter/Space "press start" carve-out further down in this file. Uses
	// borderless/"soft" fullscreen (HardwareModeSwitch = false) rather than an
	// exclusive display-mode change - fills the current monitor's desktop
	// resolution, alt-tabs instantly, and needs no display-mode enumeration.
	private void UpdateFullscreenToggle()
	{
		KeyboardState kb = Keyboard.GetState();
		bool toggleDown = kb.IsKeyDown(Keys.F11) || ((kb.IsKeyDown(Keys.LeftAlt) || kb.IsKeyDown(Keys.RightAlt)) && kb.IsKeyDown(Keys.Enter));
		if (toggleDown && !_prevFullscreenToggleDown)
		{
			graphics.HardwareModeSwitch = false;
			graphics.IsFullScreen = !graphics.IsFullScreen;
			if (graphics.IsFullScreen)
			{
				graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
				graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
			}
			else
			{
				graphics.PreferredBackBufferWidth = 1280;
				graphics.PreferredBackBufferHeight = 720;
			}
			graphics.ApplyChanges();
			RecalculateScreenDestRect();
		}
		_prevFullscreenToggleDown = toggleDown;
	}

	protected override void LoadContent()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Expected O, but got Unknown
		spriteBatch = new SpriteBatch(base.GraphicsDevice);
		skaLogoTex = base.Content.Load<Texture2D>("gfx/skalogo");
		rTarg = new RenderTarget2D(graphics.GraphicsDevice, 1280, 720, false, SurfaceFormat.Color, DepthFormat.None);
		liteTarg = new RenderTarget2D(graphics.GraphicsDevice, 1280, 720, false, SurfaceFormat.Color, DepthFormat.None);
		uiTarg = new RenderTarget2D(graphics.GraphicsDevice, 1280, 720, false, SurfaceFormat.Color, DepthFormat.None);
		mainMenu = new MainMenu(base.GraphicsDevice, base.Content);
		text = new Text();
		impact = base.Content.Load<SpriteFont>("Segoe");
		nullTex = base.Content.Load<Texture2D>("gfx/1x1");
		spritesTex = base.Content.Load<Texture2D>("gfx/sprites");
		zp2kxTex = base.Content.Load<Texture2D>("gfx/zp2kx");
		ticker = new Ticker();
	}

	private void Load()
	{
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Expected O, but got Unknown
		// Xbox 360-specific hardware-thread pinning; no PC equivalent, safe to drop.
		charDef = new CharDef[16];
		segDefMgr = new SegDefManager();
		for (int i = 0; i < 3; i++)
		{
			charDef[i] = new CharDef();
		}
		nodeMgr = new NodeMgr();
		settings = new Settings();
		netSession = new NetSession();
		charDef[0].path = "charDef/civ";
		charDef[0].Read();
		charDef[1].path = "charDef/ptero";
		charDef[1].Read();
		charDef[2].path = "charDef/fish";
		charDef[2].Read();
		for (int j = 0; j < iKeys.Length; j++)
		{
			iKeys[j] = new InterfaceKeys();
		}
		Sound.Initialize(Content);
		Music.Init(Content);
		hud = new HUD();
		gameMap = new GameMap(segDefMgr);
		pMan = new ParticleManager();
		store = new Store();
		menu = new Menu();
		menu.menuLevel[13].active = true;
		GameState.mode = 2;
		backTarg = new RenderTarget2D(graphics.GraphicsDevice, 640, 360, false, SurfaceFormat.Color, DepthFormat.None);
		blurEffect = base.Content.Load<Effect>("fx/blur");
		mainEffect = base.Content.Load<Effect>("fx/main");
		mainliteEffect = base.Content.Load<Effect>("fx/mainlite");
		controlsTex = base.Content.Load<Texture2D>("gfx/controls");
		int num = 49;
		charTex = new CharTexture[num * 2];
		weapTex = new CharTexture[5];
		pteroTex = new CharTexture[2];
		badgesTex = base.Content.Load<Texture2D>("gfx/badges");
		sceneMgr = new SceneMgr(base.Content);
		sceneMgr.Read("data/scenes/main.zcx");
		perksTex = base.Content.Load<Texture2D>("gfx/perks");
		logoTex = base.Content.Load<Texture2D>("gfx/logo");
		for (int k = 0; k < num; k++)
		{
			charTex[k * 2] = new CharTexture("human", k, 0, base.Content, game: true);
			charTex[k * 2 + 1] = new CharTexture("zombie", k, 0, base.Content, game: true);
		}
		for (int l = 0; l < weapTex.Length; l++)
		{
			weapTex[l] = new CharTexture("weap", l, 0, base.Content, game: true);
		}
		for (int m = 0; m < pteroTex.Length; m++)
		{
			pteroTex[m] = new CharTexture("ptero", m, m, base.Content, game: true);
		}
		backTex = (Texture2D[])(object)new Texture2D[10];
		backTex[0] = base.Content.Load<Texture2D>("gfx/maps/back0");
		backTex[1] = base.Content.Load<Texture2D>("gfx/maps/foreback");
		backTex[2] = base.Content.Load<Texture2D>("gfx/maps/cityback");
		backTex[3] = base.Content.Load<Texture2D>("gfx/maps/cityback2");
		backTex[4] = base.Content.Load<Texture2D>("gfx/maps/mtnback");
		backTex[5] = base.Content.Load<Texture2D>("gfx/maps/mtnback2");
		backTex[6] = base.Content.Load<Texture2D>("gfx/maps/pinkback");
		backTex[7] = base.Content.Load<Texture2D>("gfx/maps/pinkback2");
		backTex[8] = base.Content.Load<Texture2D>("gfx/maps/lemon");
		backTex[9] = base.Content.Load<Texture2D>("gfx/maps/lemon2");
		jetpacks = base.Content.Load<Texture2D>("gfx/chars/jetpacks");
		segDefMgr.Read("map/data/segdef.zdx");
		int num2 = 5;
		mapTex = (Texture2D[])(object)new Texture2D[num2];
		for (int n = 0; n < num2; n++)
		{
			mapTex[n] = base.Content.Load<Texture2D>("gfx/maps/maps" + (n + 1));
		}
		iconsTex = base.Content.Load<Texture2D>("gfx/icons");
		MapList.Init();
		StartServer startServer = (StartServer)menu.menuLevel[11];
		startServer.SetMapList();
		gameMap.Read(new BinaryReader(File.Open("map/data/" + MapList.mapCatalog[MapList.maplist[0]].path + ".zkx", FileMode.Open, FileAccess.Read)));
		nodeMgr.Refresh(gameMap);
		rosterChar = new Character(0, 0, default(Vector2));
		loader.loadComplete = true;
	}

	protected override void UnloadContent()
	{
	}

	protected override void Update(GameTime gameTime)
	{
		frameTime = (float)gameTime.ElapsedGameTime.Milliseconds / 1000f;
		KeyboardOverlay.Update(frameTime);
		KeyboardMouseInput.Update();
		UpdateFullscreenToggle();
		postGlowMgr.Update();
		if (handlingInvite && !Guide.IsVisible)
		{
			FinishHandleInvite();
		}
		if (loader.splashFrame > 1f && !loader.loadBegin)
		{
			loader.loadBegin = true;
			loaderThread = new Thread(Load);
			loaderThread.Start();
		}
		if (loader.IsDone() && !inviteHandled)
		{
			NetworkBackend.InviteAccepted += HandleInvite;
			inviteHandled = true;
		}
		if (!loader.IsDone())
		{
			loader.Update();
		}
		else
		{
			store.Update();
			netSession.Update(character);
			if (mainPlayerIndex > -1 && Gamer.SignedInGamers[(PlayerIndex)mainPlayerIndex] == null)
			{
				mainPlayerIndex = -1;
				if (netSession.netSession != null)
				{
					netSession.netSession.Dispose();
					while (!netSession.netSession.IsDisposed)
					{
					}
					netSession.netSession = null;
				}
				GameState.mode = 2;
				menu.Close();
				menu.menuLevel[13].active = true;
			}
			if (mainPlayerIndex < 0)
			{
				if (!Guide.IsVisible)
				{
					// PC has no per-controller Xbox Live sign-in, and not everyone has a
					// gamepad plugged in, so Enter/Space also count as "press Start" for
					// the keyboard player (slot 0 - see ZP2K9.platform.Gamer.SignedInGamers).
					bool keyboardPressed = Keyboard.GetState().IsKeyDown(Keys.Enter) || Keyboard.GetState().IsKeyDown(Keys.Space);
					for (int i = 0; i < 4; i++)
					{
						bool pressed = GamePad.GetState((PlayerIndex)i).Buttons.A == ButtonState.Pressed || GamePad.GetState((PlayerIndex)i).Buttons.Start == ButtonState.Pressed || (i == 0 && keyboardPressed);
						if (!pressed)
						{
							continue;
						}
						if (Gamer.SignedInGamers[(PlayerIndex)i] != null)
						{
							mainPlayerIndex = i;
							store.GetDevice();
							menu.menuLevel[13].active = false;
							menu.menuLevel[0].active = true;
							try
							{
								Guide.NotificationPosition = (NotificationPosition)8;
							}
							catch
							{
							}
						}
						else
						{
							Guide.ShowSignIn(1, false);
						}
					}
				}
			}
			else
			{
				GamePad.GetState((PlayerIndex)mainPlayerIndex, GamePadDeadZone.None);
				// Merge the real controller with a synthetic keyboard/mouse GamePadState
				// (KeyboardMouseInput.cs) so menu/HUD navigation works from either.
				GamePadState realMenuPad = GamePad.GetState((PlayerIndex)mainPlayerIndex);
				GamePadState kbmMenuPad = KeyboardMouseInput.GetMenuState();
				iKeys[0].Update(KeyboardMouseInput.Merge(realMenuPad, kbmMenuPad));
				if (GameState.mode == 1)
				{
					hud.Update(iKeys[0], character[netSession.GetPlayerOne()]);
				}
			}
			menu.Update(iKeys[0]);
			if (GameState.mode == 2)
			{
				ticker.Update();
			}
			Sound.Update();
			Music.Update();
			zProfile.second += frameTime;
			if (zProfile.second > 1f)
			{
				zProfile.second--;
				zProfile.time++;
			}
			if (GameState.mode == 0)
			{
				if (iKeys[0].keySelect)
				{
					netSession.netType = 1;
					nodeMgr.Refresh(gameMap);
					GameState.mode = 1;
					for (int j = 0; j < 8; j++)
					{
						character[j] = new Character(j, (j != 0) ? (-1) : 0, new Vector2(300f, 300f));
						gameMap.GetSpawn(0, character[j]);
					}
				}
			}
			else if (GameState.mode == 1)
			{
				if (netSession.netType == 1 && iKeys[0].keySelect)
				{
					GameState.mode = 0;
				}
				if (iKeys[0].keyStart)
				{
					if (menu.IsActive())
					{
						menu.Close();
					}
					else
					{
						menu.menuLevel[9].active = true;
					}
				}
			}
			if (GameState.mode == 1)
			{
				if (!Music.playing)
				{
					Music.playing = true;
					Music.Reset();
				}
				int playerOne = netSession.GetPlayerOne();
				flashLight.active = false;
				if (playerOne < character.Length && character[playerOne] != null)
				{
					bool flag = false;
					if (character[playerOne].hp < 0)
					{
						hud.red = 1f;
						if (character[playerOne].lastHitBy > -1 && character[playerOne].lastHitBy < character.Length && character[character[playerOne].lastHitBy] != null && character[playerOne].dyingFrame > 2f)
						{
							character[playerOne].loc = character[character[playerOne].lastHitBy].loc;
							flag = true;
						}
					}
					else
					{
						hud.red = 0f;
						if (character[playerOne].hp < 100)
						{
							hud.red = (float)(100 - character[playerOne].hp) / 100f;
						}
					}
					Vector2 loc = character[playerOne].loc;
					if (character[playerOne].spawnFrame > 0f)
					{
						float num = character[playerOne].spawnFrame / 2f;
						loc += new Vector2(num * 300f * ((character[playerOne].face == 1) ? (-1f) : 1f), (float)Math.Pow(num * 10f, 2.0) * -1f);
					}
					if (hud.IsPopupActive())
					{
						loc.Y -= 70f;
					}
					Vector2 vector = Scroll.scroll - (loc + character[playerOne].traj * 0.05f + character[playerOne].charKeys.shootVec * 50f);
					if (vector.LengthSquared() > 100f)
					{
						Scroll.scroll -= vector * frameTime * 10f;
					}
					float num2 = character[playerOne].traj.Length() / 500f;
					if (num2 > 1f)
					{
						num2 = 1f;
					}
					float num3 = 1f - num2 / 6f;
					num3 *= 1.1f;
					if (character[playerOne].weapon[character[playerOne].curWeap] > -1)
					{
						try
						{
							if (WeaponCatalog.weapons[character[playerOne].weapon[character[playerOne].curWeap]].type == 5)
							{
								num3 *= 1.25f;
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.StackTrace);
						}
					}
					if (flag)
					{
						num3 = 2f;
					}
					if (GameState.gameType == 4 && character[playerOne].team == 0 && character[playerOne].dyingFrame <= 0f)
					{
						flashLight.active = true;
					}
					if (character[playerOne].shrink > 0f)
					{
						num3 = 5f;
					}
					Scroll.zoom += (num3 - Scroll.zoom) * frameTime * ((num3 > Scroll.zoom) ? 0.5f : 3f);
					if (Scroll.scroll.Y > 7040f)
					{
						Scroll.scroll.Y = 7040f;
					}
					if (Scroll.scroll.Y < 256f)
					{
						Scroll.scroll.Y = 256f;
					}
					if (Scroll.scroll.X < 512f)
					{
						Scroll.scroll.X = 512f;
					}
					if (Scroll.scroll.X > 15872f)
					{
						Scroll.scroll.X = 15872f;
					}
				}
			}
			else
			{
				Music.playing = false;
			}
			mainMenu.active = GameState.mode == 2;
			mainMenu.Update();
			rosterChar.respawnFrame = 0f;
			rosterChar.bodySec[0].Update(rosterChar, frameTime);
			if (GameState.mode == 1)
			{
				pMan.Update(gameMap, character);
				for (int k = 0; k < character.Length; k++)
				{
					if (character[k] != null)
					{
						character[k].Update(gameMap, character, frameTime);
					}
				}
				if (DebugManager.jumpToNullMe && character[0] != null && character[0].charKeys.keyJump)
				{
					character[0] = null;
				}
				for (int l = 0; l < pterodactyl.Length; l++)
				{
					if (pterodactyl[l].exists)
					{
						pterodactyl[l].Update();
					}
				}
				for (int m = 0; m < fish.Length; m++)
				{
					if (fish[m].exists)
					{
						fish[m].Update(character[m]);
					}
				}
				if (flashLight.active)
				{
					flashLight.Update();
				}
				pMan.NetCleanup(netSession.GetPlayerOne());
				pMan.ResetChronos();
				gameMap.Update();
				Quake.UpdateScroll();
			}
			Quake.UpdateQuake();
			if (needsExit)
			{
				Exit();
			}
		}
		base.Update(gameTime);
	}

	public void HandleInvite(object sender, EventArgs e)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		handlingInvite = true;
		ie = (InviteAcceptedEventArgs)e;
	}

	internal static void DestroyChar(int i)
	{
		character[i] = null;
	}

	public void FinishHandleInvite()
	{
		int num = mainPlayerIndex;
		mainPlayerIndex = (int)ie.Gamer.PlayerIndex;
		if (mainPlayerIndex != num)
		{
			store.GetDevice();
		}
		GameState.mode = 2;
		menu.Close();
		menu.menuLevel[4].active = true;
		netSession.JoinInvite(ie);
		menu.menuLevel[4] = new Lobby(host: false);
		menu.menuLevel[4].active = true;
		handlingInvite = false;
	}

	protected override void Draw(GameTime gameTime)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_064b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0500: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d7: Unknown result type (might be due to invalid IL or missing references)
		postGlowMgr.Reset();
		// Draw everything into the fixed 1280x720 uiTarg, not the real backbuffer
		// directly - see the uiTarg field comment above. The real backbuffer is
		// only ever touched once, at the very end of this method, for the final
		// scaled/letterboxed blit.
		graphics.GraphicsDevice.SetRenderTarget(uiTarg);
		graphics.GraphicsDevice.Clear(Color.Black);
		if (!loader.IsDone())
		{
			loader.Draw(spriteBatch);
		}
		else
		{
			mainMenu.Prepare(spriteBatch, graphics.GraphicsDevice, uiTarg);
			if (!mainMenu.IsSolid())
			{
				graphics.GraphicsDevice.SetRenderTarget(backTarg);
				graphics.GraphicsDevice.Clear(Color.Black);
				spriteBatch.Begin(blendState: BlendState.AlphaBlend);
				gameMap.Draw(spriteBatch, 0, 2, nullTex, mapTex, backTex, 0.5f);
				spriteBatch.End();
				graphics.GraphicsDevice.SetRenderTarget(rTarg);
				graphics.GraphicsDevice.Clear(Color.Black);
				blurEffect.Parameters["v"].SetValue(0.005f);
				blurEffect.Parameters["briteGradientR"].SetValue(0.2f);
				blurEffect.Parameters["briteGradientR"].SetValue(0.15f);
				blurEffect.Parameters["briteGradientG"].SetValue(0.1f);
				spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, blurEffect);
				spriteBatch.Draw(backTarg, new Rectangle(0, 0, 1280, 720), Color.White);
				spriteBatch.End();
				spriteBatch.Begin(blendState: BlendState.AlphaBlend);
				gameMap.Draw(spriteBatch, 2, 3, nullTex, mapTex, backTex, 1f);
				if (GameState.mode == 1)
				{
					for (int i = 0; i < character.Length; i++)
					{
						if (character[i] == null)
						{
							continue;
						}
						try
						{
							if (character[i].spawnFrame <= 0f)
							{
								character[i].Draw(spriteBatch);
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.StackTrace);
						}
					}
				}
				pMan.Draw(spriteBatch, alpha: false);
				gameMap.DrawEntities(spriteBatch, spritesTex);
				spriteBatch.End();
				spriteBatch.Begin(blendState: BlendState.Additive);
				pMan.Draw(spriteBatch, alpha: true);
				if (DebugManager.showAIDest)
				{
					gameMap.DrawAIPaths(character, spriteBatch);
				}
				spriteBatch.End();
				spriteBatch.Begin(blendState: BlendState.AlphaBlend);
				gameMap.Draw(spriteBatch, 3, 5, nullTex, mapTex, backTex, 1f);
				for (int j = 0; j < character.Length; j++)
				{
					if (character[j] != null && character[j].spawnFrame > 0f)
					{
						character[j].Draw(spriteBatch);
					}
				}
				for (int k = 0; k < pterodactyl.Length; k++)
				{
					if (pterodactyl[k].exists)
					{
						pterodactyl[k].Draw(spriteBatch);
					}
				}
				for (int l = 0; l < fish.Length; l++)
				{
					if (fish[l].exists)
					{
						fish[l].Draw(spriteBatch);
					}
				}
				spriteBatch.End();
				spriteBatch.Begin(blendState: BlendState.Additive);
				if (DebugManager.showAIPaths)
				{
					for (int m = 0; m < character.Length; m++)
					{
						if (character[m] != null)
						{
							character[m].DrawPaths(spriteBatch);
						}
					}
				}
				postGlowMgr.Draw(spriteBatch, spritesTex);
				spriteBatch.End();
				float num = hud.red * 0.8f;
				if (character[netSession.GetPlayerOne()] != null && character[netSession.GetPlayerOne()].hp < 0)
				{
					num = 1f;
				}
				if (flashLight.active)
				{
					graphics.GraphicsDevice.SetRenderTarget(liteTarg);
					graphics.GraphicsDevice.Clear(Color.Black);
					flashLight.Draw(spriteBatch);
					graphics.GraphicsDevice.SetRenderTarget(uiTarg);
					graphics.GraphicsDevice.Clear(Color.Black);
					graphics.GraphicsDevice.Textures[1] = liteTarg;
					if (num < 0.7f)
					{
						num = 0.7f;
					}
					mainliteEffect.Parameters["red"].SetValue(hud.red * 3f);
					mainliteEffect.Parameters["gray"].SetValue(num);
					spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, mainliteEffect);
					spriteBatch.Draw(rTarg, new Rectangle(0, 0, 1280, 720), Color.White);
					spriteBatch.End();
					graphics.GraphicsDevice.Textures[1] = null;
				}
				else
				{
					graphics.GraphicsDevice.SetRenderTarget(uiTarg);
					graphics.GraphicsDevice.Clear(Color.Black);
					mainEffect.Parameters["red"].SetValue(hud.red * 3f);
					mainEffect.Parameters["gray"].SetValue(num);
					if (character[netSession.GetPlayerOne()] != null)
					{
						if (GameState.gameType == 4 && character[netSession.GetPlayerOne()].team == 1)
						{
							mainEffect.Parameters["tR"].SetValue(1f);
							mainEffect.Parameters["tG"].SetValue(0.5f);
							mainEffect.Parameters["tB"].SetValue(0.5f);
							mainEffect.Parameters["bR"].SetValue(0.6f);
							mainEffect.Parameters["bG"].SetValue(0.6f);
							mainEffect.Parameters["bB"].SetValue(0.6f);
						}
						else
						{
							mainEffect.Parameters["tR"].SetValue(gameMap.tR);
							mainEffect.Parameters["tG"].SetValue(gameMap.tG);
							mainEffect.Parameters["tB"].SetValue(gameMap.tB);
							mainEffect.Parameters["bR"].SetValue(gameMap.bR);
							mainEffect.Parameters["bG"].SetValue(gameMap.bG);
							mainEffect.Parameters["bB"].SetValue(gameMap.bB);
						}
					}
					spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, mainEffect);
					spriteBatch.Draw(rTarg, new Rectangle(0, 0, 1280, 720), Color.White);
					spriteBatch.End();
				}
				if (GameState.mode == 1)
				{
					spriteBatch.Begin(blendState: BlendState.AlphaBlend);
					hud.Draw(character[netSession.GetPlayerOne()], spriteBatch);
					spriteBatch.End();
				}
			}
			mainMenu.Draw(spriteBatch);
			menu.Draw(spriteBatch);
		}
		KeyboardOverlay.Draw(spriteBatch);
		// Final composite: blit the fixed 1280x720 uiTarg to the real backbuffer,
		// scaled and letterboxed/pillarboxed to whatever the actual window size is
		// right now (see RecalculateScreenDestRect). This is the ONLY place in the
		// whole draw pipeline that needs to know the real window size - every
		// gameplay/HUD/menu/overlay draw above this line just drew into the fixed
		// 1280x720 uiTarg exactly as it always drew straight to the screen before,
		// completely unaware the window might now be a different size or fullscreen.
		graphics.GraphicsDevice.SetRenderTarget(null);
		graphics.GraphicsDevice.Clear(Color.Black);
		spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.LinearClamp);
		spriteBatch.Draw(uiTarg, ScreenDestRect, Color.White);
		spriteBatch.End();
		base.Draw(gameTime);
	}
}
