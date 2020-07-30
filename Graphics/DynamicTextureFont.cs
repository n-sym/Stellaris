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
        Dictionary<char, Glygh> glyghs;
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
            glyghs = new Dictionary<char, Glygh>();
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
        public void ReFresh(Stream stream, float height, string text)
        {
            Initialize(graphicsDevice, new FontStb(stream, graphicsDevice), height);
        }
        private void GetGlygh(char[] charArray)
        {
            List<char> chars = charArray.ToList();
            for (int i = 0; i < chars.Count; i++)
            {
                if (glyghs.ContainsKey(chars[i]) || chars.IndexOf(chars[i]) < i)
                {
                    chars.RemoveAt(i);
                    i--;
                }
            }
            if (chars.Count == 0) return;
            Glygh[] glyghArray = font.GetGlyphsFromCodepoint(height, chars.ToCodePointArray(), 1f, 1f);
            for (int i = 0; i < chars.Count; i++)
            {
                glyghs.Add(chars[i], glyghArray[i]);
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
        public void DrawString(SpriteBatch spriteBatch, string text, Vector2 position, Color color,  Vector2 origin, Vector2 scale, SpriteEffects effects = SpriteEffects.None, float layerDepth = 1f)
        {
            char[] chars = text.ToCharArray();
            GetGlygh(chars);
            int defaultX = (int)((glyghs[text[0]].x1 + glyghs[text[0]].x0) * 0.25f);
            int x = defaultX;
            int y = (int)(height * 0.75f);
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
                else
                {
                    Glygh glygh = glyghs[chars[i]];
                    spriteBatch.Draw(glygh.texture, new Vector2(x + glygh.x0, y + glygh.y0) * scale + position, null, color, 0, origin, scale, SpriteEffects.None, 1f);
                    x += glygh.x0 + glygh.x1;
                }
            }
        }
        protected override void PrivateDraw(SpriteBatch spriteBatch, int frame, Vector2 position, Rectangle? source, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
        {
        }
    }
}
