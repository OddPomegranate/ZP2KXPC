using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ZP2K9;
using ZP2K9.menu;

namespace ZP2K9.platform;

// Real PC keyboard-driven replacement for the Xbox 360 system "Guide"
// on-screen keyboard that Guide.BeginShowKeyboardInput/EndShowKeyboardInput
// (PlatformServices.cs) used to fake by resolving synchronously with the
// default text unchanged. Every call site - Menu.cs (class name confirm),
// ClassList.cs (clan tag), Editor.cs (map rename), PlayerSetup.cs (class
// name entry) - already polls the returned IAsyncResult.IsCompleted once
// per frame and only calls EndShowKeyboardInput after it flips true, the
// same pattern NetSession.cs uses for LAN create/find/join, so this drops
// in purely by making Begin/EndShowKeyboardInput real: nothing about those
// 4 call sites needs to change.
//
// Input design: GameWindow.TextInput (subscribed once, from Initialize()
// below, called from Game1.Initialize()) hands over correctly shifted,
// layout-aware typed characters for free - far more correct than hand-
// rolling a Keys -> char table. Backspace/Delete/Left/Right/Home/End/
// Enter/Escape aren't "characters" TextInput reports, so those are read via
// plain Keyboard.GetState() edge detection instead, with simple
// press-then-repeat timing on the ones that make sense to hold down
// (Backspace/Delete/Left/Right) so clearing a field doesn't mean mashing a
// key once per character.
//
// Guide.IsVisible now reflects KeyboardOverlay.IsActive - which turns out
// to matter for free: both CharKeys.Update (gameplay input) and
// InterfaceKeys.Update (menu navigation) already gate themselves on
// `Guide.IsVisible` in the original decompiled code (written for the real
// Xbox Guide blocking input while its system keyboard was up) - that gate
// has just been dead code this whole port since IsVisible was hardcoded
// false. Making it real means the menu/gameplay underneath a text-entry box
// correctly stops responding to gamepad input while typing, with no
// changes needed anywhere outside this file plus the 3 wiring lines in
// Game1.cs and the Guide class itself.
public sealed class KeyboardInputResult : IAsyncResult
{
    public object AsyncState { get; }
    public System.Threading.WaitHandle AsyncWaitHandle => null;
    public bool CompletedSynchronously => false;
    public bool IsCompleted { get; internal set; }

    // Matches real XNA Guide semantics: null means the player cancelled
    // (Escape here; the real Xbox keyboard's B button) rather than confirmed
    // empty text.
    public string Value { get; internal set; }

    internal KeyboardInputResult(object asyncState)
    {
        AsyncState = asyncState;
    }
}

public static class KeyboardOverlay
{
    public static bool IsActive => _session != null;

    private sealed class Session
    {
        public string Title;
        public string Description;
        public StringBuilder Text;
        public int Cursor;
        public KeyboardInputResult Result;
        public AsyncCallback Callback;
    }

    private static Session _session;
    private static bool _subscribed;
    private static KeyboardState _prevKeyboard;
    private static float _blinkTimer;

    // Held-key repeat: first press fires immediately, then repeats at
    // RepeatRate once RepeatDelay has elapsed - standard text-field
    // behavior, applied only to Backspace/Delete/Left/Right below (Enter/
    // Escape/Home/End are one-shot actions, no reason to repeat those).
    private const float RepeatDelay = 0.45f;
    private const float RepeatRate = 0.035f;
    private static Keys _repeatKey = Keys.None;
    private static float _repeatTimer;

    public static void Initialize(GameWindow window)
    {
        if (_subscribed || window == null)
        {
            return;
        }
        window.TextInput += OnTextInput;
        _subscribed = true;
    }

    public static IAsyncResult Begin(string title, string description, string defaultText, AsyncCallback callback, object asyncState)
    {
        string initial = defaultText ?? "";
        KeyboardInputResult result = new KeyboardInputResult(asyncState);
        _session = new Session
        {
            Title = title ?? "",
            Description = description ?? "",
            Text = new StringBuilder(initial),
            Cursor = initial.Length,
            Result = result,
            Callback = callback
        };
        _repeatKey = Keys.None;
        _repeatTimer = 0f;
        _blinkTimer = 0f;
        // Snapshot right now so whatever key (usually gamepad A, but Enter
        // works too since keyboard doubles for menu confirm in a couple of
        // places) opened this box doesn't immediately register as a fresh
        // Enter/Escape edge against the session that's about to start.
        _prevKeyboard = Keyboard.GetState();
        return result;
    }

    private static void OnTextInput(object sender, TextInputEventArgs e)
    {
        if (_session == null)
        {
            return;
        }
        char c = e.Character;
        // Enter/Backspace/Escape/Tab and friends can also arrive here on
        // some platforms - they're all handled explicitly via
        // Keyboard.GetState() edge detection in Update() instead, so any
        // control character is ignored here to avoid double-handling or a
        // stray glyph getting inserted.
        if (char.IsControl(c))
        {
            return;
        }
        _session.Text.Insert(_session.Cursor, c);
        _session.Cursor++;
    }

    public static void Update(float dt)
    {
        if (_session == null)
        {
            return;
        }
        _blinkTimer += dt;
        KeyboardState ks = Keyboard.GetState();

        HandleRepeatable(ks, Keys.Back, dt, RemoveBefore);
        HandleRepeatable(ks, Keys.Delete, dt, RemoveAfter);
        HandleRepeatable(ks, Keys.Left, dt, MoveLeft);
        HandleRepeatable(ks, Keys.Right, dt, MoveRight);

        if (Edge(ks, Keys.Home))
        {
            _session.Cursor = 0;
        }
        if (Edge(ks, Keys.End))
        {
            _session.Cursor = _session.Text.Length;
        }

        if (Edge(ks, Keys.Enter))
        {
            Complete(_session.Text.ToString());
        }
        else if (Edge(ks, Keys.Escape))
        {
            Complete(null);
        }

        _prevKeyboard = ks;
    }

    private static bool Edge(KeyboardState ks, Keys key)
    {
        return ks.IsKeyDown(key) && !_prevKeyboard.IsKeyDown(key);
    }

    private static void HandleRepeatable(KeyboardState ks, Keys key, float dt, Action action)
    {
        bool down = ks.IsKeyDown(key);
        bool wasDown = _prevKeyboard.IsKeyDown(key);
        if (down && !wasDown)
        {
            action();
            _repeatKey = key;
            _repeatTimer = RepeatDelay;
        }
        else if (down && key == _repeatKey)
        {
            _repeatTimer -= dt;
            if (_repeatTimer <= 0f)
            {
                action();
                _repeatTimer = RepeatRate;
            }
        }
        else if (!down && key == _repeatKey)
        {
            _repeatKey = Keys.None;
        }
    }

    private static void RemoveBefore()
    {
        if (_session.Cursor > 0)
        {
            _session.Text.Remove(_session.Cursor - 1, 1);
            _session.Cursor--;
        }
    }

    private static void RemoveAfter()
    {
        if (_session.Cursor < _session.Text.Length)
        {
            _session.Text.Remove(_session.Cursor, 1);
        }
    }

    private static void MoveLeft()
    {
        if (_session.Cursor > 0)
        {
            _session.Cursor--;
        }
    }

    private static void MoveRight()
    {
        if (_session.Cursor < _session.Text.Length)
        {
            _session.Cursor++;
        }
    }

    private static void Complete(string value)
    {
        Session s = _session;
        // Close the overlay before invoking the callback / flipping
        // IsCompleted, so IsActive (and therefore Guide.IsVisible) drops
        // the same frame Enter/Escape was pressed - the underlying menu's
        // gamepad input un-freezes immediately rather than a frame late.
        _session = null;
        s.Result.Value = value;
        s.Result.IsCompleted = true;
        s.Callback?.Invoke(s.Result);
    }

    public static void Draw(SpriteBatch sprite)
    {
        if (_session == null)
        {
            return;
        }

        // Full-screen dim so the box reads as modal - the closest PC
        // equivalent of the real Xbox Guide blacking out everything else
        // while its keyboard was up.
        sprite.Begin(blendState: BlendState.AlphaBlend);
        sprite.Draw(Game1.nullTex, new Rectangle(0, 0, 1280, 720), new Color(0, 0, 0, 180));
        sprite.End();

        Rectangle box = new Rectangle(640 - 260, 360 - 80, 520, 160);
        MenuLevel.DrawBox(sprite, box, new Color(0f, 0f, 0f, 0.9f), new Color(0.3f, 0.3f, 0.3f, 1f));

        string shown = _session.Text.ToString();
        Vector2 textPos = new Vector2(box.X + 20f, box.Y + 88f);

        sprite.Begin(blendState: BlendState.Additive);
        Game1.text.size = 1f;
        Game1.text.color = Color.White;
        Game1.text.DrawString(new Vector2(box.Center.X, box.Y + 12f), _session.Title, 1, -1f, Game1.impact, sprite);
        Game1.text.size = 0.65f;
        Game1.text.color = new Color(0.65f, 0.65f, 0.7f, 1f);
        Game1.text.DrawString(new Vector2(box.Center.X, box.Y + 42f), _session.Description, 1, box.Width - 40f, Game1.impact, sprite);
        Game1.text.size = 1f;
        Game1.text.color = Color.White;
        Game1.text.DrawString(textPos, shown, 0, box.Width - 40f, Game1.impact, sprite);
        Game1.text.size = 0.5f;
        Game1.text.color = new Color(0.5f, 0.5f, 0.55f, 1f);
        Game1.text.DrawString(new Vector2(box.Center.X, box.Bottom - 30f), "Enter to confirm  -  Esc to cancel", 1, -1f, Game1.impact, sprite);
        sprite.End();

        // Blinking caret, drawn as a thin bar at the cursor position via the
        // same nullTex 1x1 white pixel every other simple-rect draw in this
        // project already uses.
        if (_blinkTimer % 1f < 0.5f)
        {
            string beforeCursor = shown.Substring(0, Math.Min(_session.Cursor, shown.Length));
            float caretX = Game1.impact.MeasureString(beforeCursor).X;
            float caretH = Game1.impact.MeasureString("Wy").Y;
            Rectangle caret = new Rectangle((int)(textPos.X + caretX), (int)textPos.Y, 2, (int)caretH);
            sprite.Begin(blendState: BlendState.AlphaBlend);
            sprite.Draw(Game1.nullTex, caret, Color.White);
            sprite.End();
        }
    }
}
