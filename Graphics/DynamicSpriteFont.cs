using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System;
using System.IO;

namespace Stellaris.Graphics
{
    public class DynamicSpriteFont
    {
        GraphicsDevice graphicsDevice;
        FontStb font;
        float height;
        Dictionary<char, Glyph> Glyphs;
        Glyph defaultGlyph;
        Glyph defaultGlyphCn;
        public DynamicSpriteFont(GraphicsDevice graphicsDevice, FontStb font, float height)
        {
            Initialize(graphicsDevice, font, height);
        }
        public DynamicSpriteFont(GraphicsDevice graphicsDevice, string path, float height)
        {
            using (FileStream fileStream = File.OpenRead(path))
            {
                Initialize(graphicsDevice, new FontStb(path, graphicsDevice), height);
            }
        }
        public DynamicSpriteFont(GraphicsDevice graphicsDevice, Stream stream, float height)
        {
            Initialize(graphicsDevice, new FontStb(stream, graphicsDevice), height);
        }
        private void Initialize(GraphicsDevice graphicsDevice, FontStb font, float height)
        {
            this.graphicsDevice = graphicsDevice;
            this.font = font; 
            this.height = height;
            Glyphs = new Dictionary<char, Glyph>();
            Glyph[] Glyph = font.GetGlyphsFromCodepoint(height, new int[] { 'A', '国' }, 1f, 1f);
            defaultGlyph = Glyph[0];
            defaultGlyphCn = Glyph[1];
        }
        public void Refresh(FontStb font, float height)
        {
            Initialize(graphicsDevice, font, height);
        }
        public void Refresh(string path, float height)
        {
            using (FileStream fileStream = File.OpenRead(path))
            {
                Initialize(graphicsDevice, new FontStb(path, graphicsDevice), height);
            }
        }
        public void ReFresh(Stream stream, float height)
        {
            Initialize(graphicsDevice, new FontStb(stream, graphicsDevice), height);
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
        public void DrawString(SpriteBatch spriteBatch, string text, Vector2 position, Color color, Vector2 origin, float scale, SpriteEffects effects = SpriteEffects.None, float layerDepth = 1f)
        {
            DrawString(spriteBatch, text, position, color, origin, new Vector2(scale, scale), effects, layerDepth);
        }
        public void DrawString(SpriteBatch spriteBatch, string text, Vector2 position, Color color, CenterType centerType, float scale, SpriteEffects effects = SpriteEffects.None, float layerDepth = 1f)
        {
            DrawString(spriteBatch, text, position, color, MeasureString(text, 1).MutiplyXY(Helper.CenterTypeToVector2(centerType)), new Vector2(scale, scale), effects, layerDepth);
        }
        public void DrawString(SpriteBatch spriteBatch, string text, Vector2 position, Color color, Vector2 origin, Vector2 scale, SpriteEffects effects = SpriteEffects.None, float layerDepth = 1f)
        {
            char[] chars = text.ToCharArray();
            GetGlyph(chars);
            int defaultX = 0;
            int x = defaultX;
            int y = (int)(height * 0.6f);
            for (int i = 0; i < chars.Length; i++)
            {
                if (chars[i] == '\\' && chars.TryGetValue(i + 1) == 'n')
                {
                    y += (int)(height * 0.8f);
                    x = defaultX;
                    i++;
                }
                else if (chars[i] == '\n')
                {
                    y += (int)(height * 0.8f);
                    x = defaultX;
                }
                else if (chars[i] == ' ')
                {
                    x += (int)(defaultGlyph.WidthAlt * 0.8f);
                }
                else
                {
                    Glyph Glyph = Glyphs[chars[i]];
                    spriteBatch.Draw(Glyph.texture, new Vector2(x + Glyph.x0, y + Glyph.y0).MutiplyXY(scale) + position, null, color, 0, origin, scale, SpriteEffects.None, 1f);
                    if (FontHelper.IsCn(chars[i])) x += (int)(defaultGlyphCn.Width * 1.2f);
                    else if (FontHelper.IsRu(chars[i])) x += Glyph.Width + Glyph.x0 + Glyph.x0;
                    else x += Glyph.WidthAlt;
                }
            }
        }
        public Vector2 MeasureString(string text, Vector2 scale)
        {
            char[] chars = text.ToCharArray();
            GetGlyph(chars); 
            int defaultX = 0;
            int x = defaultX;
            int maxX = 0;
            int y = (int)(height * 0.6f); 
            for (int i = 0; i < chars.Length; i++)
            {
                if (chars[i] == '\\' && chars.TryGetValue(i + 1) == 'n')
                {
                    y += (int)(height * 0.6f);
                    x = defaultX;
                    i++;
                }
                else if (chars[i] == '\n')
                {
                    y += (int)(height * 0.6f);
                    x = defaultX;
                }
                else if (chars[i] == ' ')
                {
                    x += (int)(defaultGlyph.WidthAlt * 0.8f);
                }
                else
                {
                    Glyph Glyph = Glyphs[chars[i]];
                    if (FontHelper.IsCn(chars[i])) x += (int)(defaultGlyphCn.Width * 1.2f);
                    else if (FontHelper.IsRu(chars[i])) x += Glyph.Width + Glyph.x0 + Glyph.x0;
                    else x += Glyph.WidthAlt;
                }
                if (x > maxX) maxX = x;
            }
            return new Vector2(maxX, y).MutiplyXY(scale);
        }
        public Vector2 MeasureString(string text, float scale)
        {
            return MeasureString(text, new Vector2(scale, scale));
        }
    }
}
