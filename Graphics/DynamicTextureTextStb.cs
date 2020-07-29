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
        Glygh[] glyths;
        public DynamicTextureTextStb(GraphicsDevice graphicsDevice, FontStb font, float height, string text) : base(graphicsDevice, ((text + height.ToString()).GetHashCode()).ToString())
        {
            Initialize(graphicsDevice, font, height, text);
        }
        public DynamicTextureTextStb(GraphicsDevice graphicsDevice, string path, float height, string text) : base(graphicsDevice, ((text + height.ToString()).GetHashCode()).ToString())
        {
            using (FileStream fileStream = File.OpenRead(path))
            {
                Initialize(graphicsDevice, new FontStb(path, graphicsDevice), height, text);
            }
        }
        public DynamicTextureTextStb(GraphicsDevice graphicsDevice, Stream stream, float height, string text) : base(graphicsDevice, ((text + height.ToString()).GetHashCode()).ToString())
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
            GetGlygh();
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
        private void GetGlygh()
        {
            glyths = font.GetGlyphs(height, font.GetGlyphIndex(text), 1f, 1f);
        }
        protected override void PrivateDraw(SpriteBatch spriteBatch, int frame, Vector2 position, Rectangle? source, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
        {
            int x = 0;
            int y = 0;
            char[] chars = text.ToCharArray();
            for (int i = 0; i < glyths.Length; i++)
            {
                if (chars[i] == '\\' && chars.TryGetValue(i + 1) == 'n')
                {
                    y += (int)height;
                    i++;
                }
                else
                {
                    spriteBatch.Draw(glyths[i].texture, new Vector2(x + glyths[i].x0, y + glyths[i].y0) * scale + position, null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 1f);
                    x += glyths[i].x1 + glyths[i].x0;
                }
            }
        }
    }
}
