using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Stellaris.Graphics
{
    public class DynamicSpriteFont : IDisposable
    {
        public float height;
        public Vector2 spacing;
        GraphicsDevice graphicsDevice;
        IFont font;
        Dictionary<char, Glyph> Glyphs;
        Glyph defaultGlyph;
        Glyph defaultGlyphCn;
        public DynamicSpriteFont(GraphicsDevice graphicsDevice, IFont font, float height, Vector2 spacing = default)
        {
            Initialize(graphicsDevice, font, height, spacing);
        }
        public DynamicSpriteFont(GraphicsDevice graphicsDevice, string path, float height, Vector2 spacing = default, bool useNative = false)
        {
            using (FileStream fileStream = File.OpenRead(path))
            {
                Initialize(graphicsDevice, useNative ? new FontStb_Native(path, graphicsDevice) as IFont : new FontStb(path, graphicsDevice), height, spacing);
            }
        }
        public DynamicSpriteFont(GraphicsDevice graphicsDevice, Stream stream, float height, Vector2 spacing = default, bool useNative = false)
        {
            Initialize(graphicsDevice, useNative ? new FontStb_Native(stream, graphicsDevice) as IFont : new FontStb(stream, graphicsDevice), height, spacing);
        }
        private void Initialize(GraphicsDevice graphicsDevice, IFont font, float height, Vector2 spacing)
        {
            this.graphicsDevice = graphicsDevice;
            this.font = font;
            this.height = height;
            this.spacing = spacing;
            Glyphs = new Dictionary<char, Glyph>();
            Glyph[] Glyph = font.GetGlyphsFromCodepoint(height, new int[] { 'A', '国' }, 1f, 1f);
            defaultGlyph = Glyph[0];
            defaultGlyphCn = Glyph[1];
        }
        public void Refresh()
        {
            Glyphs.Clear();
        }
        private void GetGlyph(char[] charArray)
        {
            List<char> chars = charArray.ToList();
            for (int i = 0; i < chars.Count; i++)
            {
                if (Glyphs.ContainsKey(chars[i]) || chars.IndexOf(chars[i]) < i)
                {
                    chars.RemoveAt(i);
                    i--;
                }
            }
            if (chars.Count == 0) return;
            Glyph[] GlyphArray = font.GetGlyphsFromCodepoint(height, chars.ToCodePointArray(), 1f, 1f);
            for (int i = 0; i < chars.Count; i++)
            {
                Glyphs.Add(chars[i], GlyphArray[i]);
            }
        }
        public void DrawString(SpriteBatch spriteBatch, string text, Vector2 position)
        {
            DrawString(spriteBatch, text, position, Color.White, default, new Vector2(1, 1));
        }
        public void DrawString(SpriteBatch spriteBatch, string text, Vector2 position, Color color)
        {
            DrawString(spriteBatch, text, position, color, default, new Vector2(1, 1));
        }
        public void DrawString(SpriteBatch spriteBatch, string text, Vector2 position, Color color, Vector2 origin, float scale, float rotation = 0, SpriteEffects effects = SpriteEffects.None, float layerDepth = 1f)
        {
            DrawString(spriteBatch, text, position, color, origin, new Vector2(scale, scale), rotation, effects, layerDepth);
        }
        public void DrawString(SpriteBatch spriteBatch, string text, Vector2 position, Color color, CenterType centerType, float scale, float rotation = 0, SpriteEffects effects = SpriteEffects.None, float layerDepth = 1f)
        {
            DrawString(spriteBatch, text, position, color, MeasureString(text, 1).MutiplyXY(Helper.CenterTypeToVector2(centerType)), new Vector2(scale, scale), rotation, effects, layerDepth);
        }
        public void DrawString(SpriteBatch spriteBatch, string text, Vector2 position, Color color, Vector2 origin, Vector2 scale, float rotation = 0f, SpriteEffects effects = SpriteEffects.None, float layerDepth = 1f)
        {
            char[] chars = text.ToCharArray();
            GetGlyph(chars);
            int defaultX = 0;
            float x = defaultX;
            float y = (int)(height * 0.6f);
            for (int i = 0; i < chars.Length; i++)
            {
                if (chars[i] == '\\' && chars.TryGetValue(i + 1) == 'n')
                {
                    y += height * 0.8f + spacing.Y;
                    x = defaultX;
                    i++;
                }
                else if (chars[i] == '\n')
                {
                    y += height * 0.8f + spacing.Y;
                    x = defaultX;
                }
                else if (chars[i] == ' ')
                {
                    x += defaultGlyph.Width * 0.5f + spacing.X;
                }
                else
                {
                    Glyph Glyph = Glyphs[chars[i]];
                    if (rotation == 0) spriteBatch.Draw(Glyph.texture, new Vector2(x + Glyph.x0, y + Glyph.y0).MutiplyXY(scale) + position, null, color, 0f, scale, scale, effects, layerDepth);
                    else spriteBatch.Draw(Glyph.texture, new Vector2(x + Glyph.x0, y + Glyph.y0).MutiplyXY(scale).Rotate(rotation) + position, null, color, rotation, scale, scale, effects, layerDepth);
                    if (FontHelper.IsCn(chars[i])) x += defaultGlyphCn.Width * 1.2f + spacing.X;
                    else x += Glyph.WidthAlt + spacing.X;
                }
            }
        }
        public Vector2 MeasureString(string text, Vector2 scale)
        {
            char[] chars = text.ToCharArray();
            GetGlyph(chars);
            int defaultX = 0;
            float x = defaultX;
            float maxX = 0;
            float y = height * 0.6f;
            for (int i = 0; i < chars.Length; i++)
            {
                if (chars[i] == '\\' && chars.TryGetValue(i + 1) == 'n')
                {
                    y += height * 0.8f + spacing.Y;
                    x = defaultX;
                    i++;
                }
                else if (chars[i] == '\n')
                {
                    y += height * 0.8f + spacing.Y;
                    x = defaultX;
                }
                else if (chars[i] == ' ')
                {
                    x += defaultGlyph.Width * 0.5f + spacing.X;
                }
                else
                {
                    Glyph Glyph = Glyphs[chars[i]];
                    if (FontHelper.IsCn(chars[i])) x += defaultGlyphCn.Width * 1.2f + spacing.X;
                    else x += Glyph.WidthAlt + spacing.X;
                }
                if (x > maxX) maxX = x;
            }
            return new Vector2(maxX, y).MutiplyXY(scale);
        }
        public Vector2 MeasureString(string text, float scale)
        {
            return MeasureString(text, new Vector2(scale, scale));
        }

        public void Dispose()
        {
            Glyphs.Clear();
            Glyphs = null;
            defaultGlyph = null;
            defaultGlyphCn = null;
            if (font is IDisposable disposable) disposable.Dispose();
        }
    }
}
