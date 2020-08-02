using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System;
using System.IO;

namespace Stellaris.Graphics
{
    public class DynamicTextureFont : DynamicTexture
    {
        FontStb font;
        float height;
        Dictionary<char, Glyph> Glyphs;
        Glyph defaultGlyph;
        Glyph defaultGlyphCn;
        public DynamicTextureFont(GraphicsDevice graphicsDevice, FontStb font, float height) : base(graphicsDevice)
        {
            Initialize(graphicsDevice, font, height);
        }
        public DynamicTextureFont(GraphicsDevice graphicsDevice, string path, float height) : base(graphicsDevice)
        {
            using (FileStream fileStream = File.OpenRead(path))
            {
                Initialize(graphicsDevice, new FontStb(path, graphicsDevice), height);
            }
        }
        public DynamicTextureFont(GraphicsDevice graphicsDevice, Stream stream, float height) : base(graphicsDevice)
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
        public void DrawString(SpriteBatch spriteBatch, string text, Vector2 position, Color color, Vector2 origin, Vector2 scale, SpriteEffects effects = SpriteEffects.None, float layerDepth = 1f)
        {
            char[] chars = text.ToCharArray();
            GetGlyph(chars);
            int defaultX = (int)((defaultGlyph.WidthAlt) * 0.25f);
            int x = defaultX;
            int y = (int)(height * 0.5f);
            for (int i = 0; i < chars.Length; i++)
            {
                if (chars[i] == '\\' && chars.TryGetValue(i + 1) == 'n')
                {
                    y += (int)height;
                    x = defaultX;
                    i++;
                }
                else if (chars[i] == '\n')
                {
                    y += (int)height;
                    x = defaultX;
                }
                else if (chars[i] == ' ')
                {
                    x += defaultGlyph.WidthAlt;
                }
                else
                {
                    Glyph Glyph = Glyphs[chars[i]];
                    spriteBatch.Draw(Glyph.texture, new Vector2(x + Glyph.x0, y + Glyph.y0).MutiplyXY(scale) + position, null, color, 0, origin, scale, SpriteEffects.None, 1f);
                    if (FontHelper.IsCn(chars[i])) x += (int)(defaultGlyphCn.Width * 1.2f);
                    else if (FontHelper.IsRu(chars[i])) x += (int)(height * 0.04f) + Glyph.x1;
                    else x += Glyph.x0 + Glyph.x1;
                }
            }
        }
        public Vector2 MeasureString(string text, Vector2 scale)
        {
            char[] chars = text.ToCharArray();
            GetGlyph(chars); 
            int defaultX = (int)((defaultGlyph.WidthAlt) * 0.25f);
            int x = defaultX;
            int maxX = 0;
            int y = (int)(height * 0.5f); 
            for (int i = 0; i < chars.Length; i++)
            {
                if (chars[i] == '\\' && chars.TryGetValue(i + 1) == 'n')
                {
                    y += (int)height;
                    x = defaultX;
                    i++;
                }
                else if (chars[i] == '\n')
                {
                    y += (int)height;
                    x = defaultX;
                }
                else if (chars[i] == ' ')
                {
                    x += defaultGlyph.WidthAlt;
                }
                else
                {
                    Glyph Glyph = Glyphs[chars[i]];
                    if (FontHelper.IsCn(chars[i])) x += (int)(defaultGlyphCn.Width * 1.05f);
                    else if (FontHelper.IsRu(chars[i])) x += (int)(height * 0.04f) + Glyph.x1;
                    else x += Glyph.x0 + Glyph.x1;
                }
                if (x > maxX) maxX = x;
            }
            return new Vector2(maxX, y).MutiplyXY(scale);
        }
        public Vector2 MeasureString(string text, float scale)
        {
            return MeasureString(text, new Vector2(scale, scale));
        }
        protected override void PrivateDraw(SpriteBatch spriteBatch, int frame, Vector2 position, Rectangle? source, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
        {
        }
        protected override List<Color[]> Generating()
        {
            return new List<Color[]>();
        }
    }
}
