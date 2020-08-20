using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System;
using System.IO;

namespace Stellaris.Graphics
{
    public class DynamicTextureTextStb : DynamicTexture
    {
        FontStb font;
        string text;
        float height;
        Glyph[] Glyphs;
        Glyph defaultGlyph;
        Glyph defaultGlyphCn;
        public DynamicTextureTextStb(GraphicsDevice graphicsDevice, FontStb font, float height, string text) : base(graphicsDevice, ((text + height.ToString()).GetHashCode()).ToString())
        {
            Initialize(graphicsDevice, font, height, text);
        }
        public DynamicTextureTextStb(GraphicsDevice graphicsDevice, string path, float height, string text) : base(graphicsDevice)
        {
            using (FileStream fileStream = File.OpenRead(path))
            {
                Initialize(graphicsDevice, new FontStb(path, graphicsDevice), height, text);
            }
        }
        public DynamicTextureTextStb(GraphicsDevice graphicsDevice, Stream stream, float height, string text) : base(graphicsDevice)
        {
            Initialize(graphicsDevice, new FontStb(stream, graphicsDevice), height, text);
        }
        private void Initialize(GraphicsDevice graphicsDevice, FontStb font, float height, string text)
        {
            name = (text + height.ToString()).GetHashCode().ToString();
            this.graphicsDevice = graphicsDevice;
            this.font = font; 
            this.text = text;
            this.height = height;
            GetGlyph();
            Glyph[] Glyph = font.GetGlyphsFromCodepoint(height, new int[] { 'A', '国' }, 1f, 1f);
            defaultGlyph = Glyph[0];
            defaultGlyphCn = Glyph[1];
        }
        public void Refresh(FontStb font, float height, string text)
        {
            Initialize(graphicsDevice, font, height, text);
        }
        public void Refresh(string path, float height, string text)
        {
            using (FileStream fileStream = File.OpenRead(path))
            {
                Initialize(graphicsDevice, new FontStb(path, graphicsDevice), height, text);
            }
        }
        public void ReFresh(Stream stream, float height, string text)
        {
            Initialize(graphicsDevice, new FontStb(stream, graphicsDevice), height, text);
        }
        private void GetGlyph()
        {
            Glyphs = font.GetGlyphsFromCodepoint(height, text.ToCodePointArray(), 1f, 1f);
        }
        protected override void PrivateDraw(SpriteBatch spriteBatch, int frame, Vector2 position, Rectangle? source, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
        {
            char[] chars = text.ToArray();
            int defaultX = 0;
            int x = defaultX;
            int y = (int)(height * 0.6f);
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
                    x += (int)(defaultGlyph.WidthAlt * 0.8f);
                }
                else
                {
                    Glyph Glyph = Glyphs[i];
                    spriteBatch.Draw(Glyph.texture, new Vector2(x + Glyph.x0, y + Glyph.y0).MutiplyXY(scale) + position, null, color, 0, origin, scale, SpriteEffects.None, 1f);
                    if (FontHelper.IsCn(chars[i])) x += (int)(defaultGlyphCn.Width * 1.2f);
                    else if (FontHelper.IsRu(chars[i])) x += Glyph.Width + Glyph.x0 + Glyph.x0;
                    else x += Glyph.WidthAlt;
                }
            }
            _width[0] = x;
            _height[0] = y;
        }
    }
}
