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
        Glygh[] glyghs;
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
            glyghs = font.GetGlyphsFromCodepoint(height, text.ToCodePointArray(), 1f, 1f);
        }
        protected override void PrivateDraw(SpriteBatch spriteBatch, int frame, Vector2 position, Rectangle? source, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
        {
            int defaultX = (int)((glyghs[0].x1 + glyghs[0].x0) * 0.25f);
            int x = defaultX;
            int y = (int)(height * 0.75f);
            char[] chars = text.ToArray();
            for (int i = 0; i < glyghs.Length; i++)
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
                else
                {
                    Glygh glygh = glyghs[i];
                    spriteBatch.Draw(glygh.texture, new Vector2(x + glygh.x0, y + glygh.y0) * scale + position, null, color, 0, origin + new Vector2(glygh.x0, glygh.x1), scale, SpriteEffects.None, 1f);
                    x += glygh.x1 + glygh.x0;
                }
            }
        }
    }
}
