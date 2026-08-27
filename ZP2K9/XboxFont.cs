using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ZP2K9;

// Rebuilds a SpriteFont from the original Xbox 360-built .xnb font data instead
// of MGCB's FontDescriptionProcessor (which can only render from an installed
// system font, not the game's actual original glyph atlas).
//
// The original Content/{Arial,Impact,Segoe}.xnb files store their glyph atlas
// texture in a custom Xbox 360 GPU surface format (tag 30 = DXT3, confirmed by
// decoding and visually reading the glyph shapes) that MonoGame can't load, the
// same problem the regular gfx/*.xnb textures had. Rather than write a custom
// MGCB importer for that, the atlas was decoded once (see
// Claude Temp Here/xnb_tool/font_export.py) to a normal RGBA PNG - built through
// the ordinary MGCB texture pipeline like every other sprite - plus a small JSON
// sidecar (data/fonts/*.json, copied as plain data like data/scenes/*.zcx) with
// the per-character glyph bounds/cropping/kerning that MonoGame's own
// FontDescriptionProcessor would normally bake into the .xnb. This class just
// re-assembles those two pieces into a real SpriteFont at load time via
// SpriteFont's public constructor, which MonoGame exposes for exactly this kind
// of custom-built-font scenario.
internal static class XboxFont
{
    private class GlyphData
    {
        public int Char { get; set; }
        public int[] Bounds { get; set; }
        public int[] Crop { get; set; }
        public float[] Kerning { get; set; }
    }

    private class FontData
    {
        public int LineSpacing { get; set; }
        public float Spacing { get; set; }
        public int? DefaultChar { get; set; }
        public List<GlyphData> Glyphs { get; set; }
    }

    public static SpriteFont Load(ContentManager content, string textureAssetName, string jsonPath)
    {
        Texture2D texture = content.Load<Texture2D>(textureAssetName);
        string json = File.ReadAllText(jsonPath);
        FontData data = JsonSerializer.Deserialize<FontData>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        List<Rectangle> glyphBounds = new List<Rectangle>();
        List<Rectangle> cropping = new List<Rectangle>();
        List<char> characters = new List<char>();
        List<Vector3> kerning = new List<Vector3>();

        foreach (GlyphData g in data.Glyphs)
        {
            characters.Add((char)g.Char);
            glyphBounds.Add(new Rectangle(g.Bounds[0], g.Bounds[1], g.Bounds[2], g.Bounds[3]));
            cropping.Add(new Rectangle(g.Crop[0], g.Crop[1], g.Crop[2], g.Crop[3]));
            kerning.Add(new Vector3(g.Kerning[0], g.Kerning[1], g.Kerning[2]));
        }

        char? defaultChar = data.DefaultChar.HasValue ? (char)data.DefaultChar.Value : (char?)null;

        return new SpriteFont(texture, glyphBounds, cropping, characters, data.LineSpacing, data.Spacing, kerning, defaultChar);
    }
}
