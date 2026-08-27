using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ZP2K9;

namespace ZP2K9.platform;

// Synthetic GamePadState builder for keyboard + mouse play.
//
// CharKeys.Update and InterfaceKeys.Update (ZP2K9.characters/CharKeys.cs,
// ZP2K9.hud/InterfaceKeys.cs) - the only two places that ever turn a controller
// into gameplay/menu state - both take nothing but a plain
// Microsoft.Xna.Framework.Input.GamePadState and read it via ordinary field access
// (gs.ThumbSticks.Left, gs.Buttons.A, gs.Triggers.Left, ...). Neither one knows or
// cares where that struct actually came from, so rather than re-implementing their
// deadzone/edge-detection/clamp logic a second time for a keyboard+mouse path, this
// builds a real GamePadState out of keyboard+mouse every frame and hands it to the
// exact same call sites (Character.cs, Game1.cs) unchanged.
//
// Bindings (as specified 2026-08-23, extended same day):
//   Left stick    - WASD
//   Right stick   - direction from the character to the mouse cursor, but only
//                   while the left mouse button is held. Releasing LMB leaves the
//                   right stick at zero, which is exactly what CharKeys.Update
//                   already treats as "aim from movement/facing instead" for
//                   non-twin-stick play - no special case needed here for that.
//   DPad          - Arrow keys
//   Left trigger  - Left Shift (Roll / Float in CharKeys.Update)
//   Right trigger - Right mouse button (Grenade in CharKeys.Update)
//   Left shoulder - NOT bound (Jump/Jetpack) - not part of the requested list
//   Right shoulder- E (gren2-alt in CharKeys.Update)
//   Left stick click  - Z (Squat in CharKeys.Update)
//   Right stick click - Q (Kick in CharKeys.Update)
//   Start         - Escape (pause menu / InterfaceKeys.keyStart)
//   Back          - NOT bound - not part of the requested list
//   A             - Space
//   B             - Tab
//   Y             - Mouse scroll wheel (either direction counts as one tap/frame)
//   X             - R
//
// Left shoulder and Back are still NOT bound - jump/jetpack and select weren't part
// of the requested binding list, so those stay controller-only (or the existing
// Enter/Space "Start" carve-out in Game1.cs) until asked for.
public static class KeyboardMouseInput
{
    private static KeyboardState _keyboard;
    private static MouseState _mouse;
    private static int _prevScrollValue;
    private static bool _scrolledThisFrame;

    // The mouse cursor's position converted into the game's fixed logical
    // 1280x720 space - see ConvertToLogical() below. Computed once per frame
    // in Update(), same as everything else here.
    private static Vector2 _mouseLogical;

    // Call exactly once per frame (Game1.Update, right alongside
    // KeyboardOverlay.Update) - everything below reads from this single per-frame
    // snapshot so GetGameplayState/GetMenuState can each be called (or not called)
    // in any combination in a given frame without re-diffing the scroll wheel twice
    // and double- or under-counting a tick.
    public static void Update()
    {
        _keyboard = Keyboard.GetState();
        _mouse = Mouse.GetState();
        _mouseLogical = ConvertToLogical(_mouse.X, _mouse.Y);
        int scrollValue = _mouse.ScrollWheelValue;
        _scrolledThisFrame = scrollValue != _prevScrollValue;
        _prevScrollValue = scrollValue;
    }

    // Converts a raw window-client mouse coordinate into the game's fixed
    // logical 1280x720 space, via Game1.ScreenDestRect - added 2026-08-23
    // alongside the scaling/fullscreen window support in Game1.cs. Before that
    // change the window was always exactly 1280x720, so raw mouse coordinates
    // and logical screen coordinates (what Scroll.GetLoc below returns) were
    // identical; now that the window can be resized or fullscreened and the
    // game is scaled/letterboxed to fit, they diverge and this conversion is
    // required for mouse-aim to still point at the right place. Falls back to
    // a straight passthrough if ScreenDestRect is ever degenerate (shouldn't
    // happen - it starts as the full 1280x720 rect - but avoids a divide by
    // zero if it somehow is).
    private static Vector2 ConvertToLogical(int rawX, int rawY)
    {
        Rectangle dest = Game1.ScreenDestRect;
        if (dest.Width <= 0 || dest.Height <= 0)
        {
            return new Vector2(rawX, rawY);
        }
        float lx = (rawX - dest.X) / (float)dest.Width * 1280f;
        float ly = (rawY - dest.Y) / (float)dest.Height * 720f;
        return new Vector2(lx, ly);
    }

    private static Vector2 WasdVector()
    {
        Vector2 v = Vector2.Zero;
        if (_keyboard.IsKeyDown(Keys.A))
        {
            v.X -= 1f;
        }
        if (_keyboard.IsKeyDown(Keys.D))
        {
            v.X += 1f;
        }
        if (_keyboard.IsKeyDown(Keys.S))
        {
            v.Y -= 1f;
        }
        if (_keyboard.IsKeyDown(Keys.W))
        {
            v.Y += 1f;
        }
        if (v != Vector2.Zero)
        {
            v.Normalize();
        }
        return v;
    }

    private static void AddDPad(List<Buttons> buttons)
    {
        if (_keyboard.IsKeyDown(Keys.Up))
        {
            buttons.Add(Buttons.DPadUp);
        }
        if (_keyboard.IsKeyDown(Keys.Down))
        {
            buttons.Add(Buttons.DPadDown);
        }
        if (_keyboard.IsKeyDown(Keys.Left))
        {
            buttons.Add(Buttons.DPadLeft);
        }
        if (_keyboard.IsKeyDown(Keys.Right))
        {
            buttons.Add(Buttons.DPadRight);
        }
    }

    // characterWorldLoc: the local player's Character.loc (world space) - needed to
    // turn the mouse cursor's screen position into an aim direction relative to
    // where the character actually is, via the same Scroll.GetLoc transform the
    // game already uses to place everything else on screen.
    public static GamePadState GetGameplayState(Vector2 characterWorldLoc)
    {
        List<Buttons> buttons = new List<Buttons>();
        AddDPad(buttons);
        if (_keyboard.IsKeyDown(Keys.Space))
        {
            buttons.Add(Buttons.A);
        }
        if (_keyboard.IsKeyDown(Keys.Tab))
        {
            buttons.Add(Buttons.B);
        }
        if (_keyboard.IsKeyDown(Keys.R))
        {
            buttons.Add(Buttons.X);
        }
        if (_scrolledThisFrame)
        {
            buttons.Add(Buttons.Y);
        }
        if (_keyboard.IsKeyDown(Keys.E))
        {
            buttons.Add(Buttons.RightShoulder);
        }
        if (_keyboard.IsKeyDown(Keys.Z))
        {
            buttons.Add(Buttons.LeftStick);
        }
        if (_keyboard.IsKeyDown(Keys.Q))
        {
            buttons.Add(Buttons.RightStick);
        }
        if (_keyboard.IsKeyDown(Keys.Escape))
        {
            buttons.Add(Buttons.Start);
        }

        Vector2 leftStick = WasdVector();

        Vector2 rightStick = Vector2.Zero;
        if (_mouse.LeftButton == ButtonState.Pressed)
        {
            Vector2 charScreen = Scroll.GetLoc(characterWorldLoc);
            Vector2 toMouse = _mouseLogical - charScreen;
            if (toMouse != Vector2.Zero)
            {
                toMouse.Normalize();
                // CharKeys.Update negates ThumbSticks.Right.Y to land on a
                // screen-space (Y-down) shootVec, so pre-negate here to undo that
                // and end up pointing at the cursor as intended.
                rightStick = new Vector2(toMouse.X, 0f - toMouse.Y);
            }
        }

        float leftTrigger = _keyboard.IsKeyDown(Keys.LeftShift) ? 1f : 0f;
        float rightTrigger = _mouse.RightButton == ButtonState.Pressed ? 1f : 0f;

        return new GamePadState(leftStick, rightStick, leftTrigger, rightTrigger, buttons.ToArray());
    }

    // Menu/HUD navigation context - same physical bindings minus mouse-look, which
    // has no meaning without a character on screen to aim from.
    public static GamePadState GetMenuState()
    {
        List<Buttons> buttons = new List<Buttons>();
        AddDPad(buttons);
        if (_keyboard.IsKeyDown(Keys.Space))
        {
            buttons.Add(Buttons.A);
        }
        if (_keyboard.IsKeyDown(Keys.Tab))
        {
            buttons.Add(Buttons.B);
        }
        if (_scrolledThisFrame)
        {
            buttons.Add(Buttons.Y);
        }
        if (_keyboard.IsKeyDown(Keys.E))
        {
            buttons.Add(Buttons.RightShoulder);
        }
        if (_keyboard.IsKeyDown(Keys.Escape))
        {
            buttons.Add(Buttons.Start);
        }

        Vector2 leftStick = WasdVector();
        return new GamePadState(leftStick, Vector2.Zero, 0f, 0f, buttons.ToArray());
    }

    // ORs a real controller's state together with a synthetic keyboard/mouse one so
    // both work at once, with no settings toggle needed - same spirit as the
    // existing "Enter/Space also count as pressing Start" carve-out on Game1.cs's
    // join-game screen. Sticks/triggers: whichever side is contributing more wins
    // (summing them could push a combined stick past 1.0); buttons/DPad: plain OR.
    public static GamePadState Merge(GamePadState real, GamePadState synthetic)
    {
        Vector2 left = (real.ThumbSticks.Left.LengthSquared() >= synthetic.ThumbSticks.Left.LengthSquared())
            ? real.ThumbSticks.Left
            : synthetic.ThumbSticks.Left;
        Vector2 right = (real.ThumbSticks.Right.LengthSquared() >= synthetic.ThumbSticks.Right.LengthSquared())
            ? real.ThumbSticks.Right
            : synthetic.ThumbSticks.Right;
        float leftTrigger = Math.Max(real.Triggers.Left, synthetic.Triggers.Left);
        float rightTrigger = Math.Max(real.Triggers.Right, synthetic.Triggers.Right);

        List<Buttons> buttons = new List<Buttons>();
        AddIfEitherDown(buttons, real, synthetic, Buttons.A);
        AddIfEitherDown(buttons, real, synthetic, Buttons.B);
        AddIfEitherDown(buttons, real, synthetic, Buttons.X);
        AddIfEitherDown(buttons, real, synthetic, Buttons.Y);
        AddIfEitherDown(buttons, real, synthetic, Buttons.Back);
        AddIfEitherDown(buttons, real, synthetic, Buttons.Start);
        AddIfEitherDown(buttons, real, synthetic, Buttons.LeftShoulder);
        AddIfEitherDown(buttons, real, synthetic, Buttons.RightShoulder);
        AddIfEitherDown(buttons, real, synthetic, Buttons.LeftStick);
        AddIfEitherDown(buttons, real, synthetic, Buttons.RightStick);
        AddIfEitherDown(buttons, real, synthetic, Buttons.BigButton);
        if (real.DPad.Up == ButtonState.Pressed || synthetic.DPad.Up == ButtonState.Pressed)
        {
            buttons.Add(Buttons.DPadUp);
        }
        if (real.DPad.Down == ButtonState.Pressed || synthetic.DPad.Down == ButtonState.Pressed)
        {
            buttons.Add(Buttons.DPadDown);
        }
        if (real.DPad.Left == ButtonState.Pressed || synthetic.DPad.Left == ButtonState.Pressed)
        {
            buttons.Add(Buttons.DPadLeft);
        }
        if (real.DPad.Right == ButtonState.Pressed || synthetic.DPad.Right == ButtonState.Pressed)
        {
            buttons.Add(Buttons.DPadRight);
        }

        return new GamePadState(left, right, leftTrigger, rightTrigger, buttons.ToArray());
    }

    private static void AddIfEitherDown(List<Buttons> list, GamePadState a, GamePadState b, Buttons button)
    {
        if (a.IsButtonDown(button) || b.IsButtonDown(button))
        {
            list.Add(button);
        }
    }
}
