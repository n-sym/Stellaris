using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.XPath;

namespace Stellaris.Graphics
{
    public class SpriteBatchS : SpriteBatch, IDrawAPI
    {
        public SpriteBatchS(GraphicsDevice graphicsDevice) : base(graphicsDevice)
        {

        }

        public void Draw(SpriteDrawInfo info)
        {
            Draw(info.texture, info.position, info.sourceRectangle, info.color, info.rotation, info.origin, info.scale, info.effects, info.layerDepth);
        }
        public void Draw(VertexDrawInfo info)
        {

        }

        public SpriteBatch ToXNA()
        {
            return this;
        }
    }

    public class SpriteBatchC
    {
        public Action<SpriteFont, string, Vector2, Color, float, Vector2, Vector2, SpriteEffects, float> DrawStringAction;
        public VertexBatch vertexBatch;
        Dictionary<SpriteFont, DynamicSpriteFont> fonts;
        public GraphicsDevice GraphicsDevice => vertexBatch.graphicsDevice;
        public SpriteBatchC(GraphicsDevice graphicsDevice)
        {
            vertexBatch = new VertexBatch(graphicsDevice);
            fonts = new Dictionary<SpriteFont, DynamicSpriteFont>();
        }
        public SpriteBatchC(GraphicsDevice graphicsDevice, int capacity)
        {
            vertexBatch = new VertexBatch(graphicsDevice);
            fonts = new Dictionary<SpriteFont, DynamicSpriteFont>();
        }
        public void Begin(SpriteSortMode sortMode = SpriteSortMode.Deferred, BlendState blendState = null, SamplerState samplerState = null, DepthStencilState depthStencilState = null, RasterizerState rasterizerState = null, Effect effect = null, Matrix? transformMatrix = null)
        {
            if (blendState == null) blendState = BlendState.AlphaBlend;
            vertexBatch.Begin(blendState, PrimitiveType.TriangleStrip);
            if (sortMode == SpriteSortMode.Immediate) vertexBatch.SetDrawImmediately(true);
            else vertexBatch.SetDrawImmediately(false);
        }
        public void Draw(Texture2D texture, Rectangle destinationRectangle, Color color)
        {
            Draw(texture, destinationRectangle, null, color, 0, default, SpriteEffects.None, 0f);
        }
        public void Draw(Texture2D texture, Vector2 position, Color color)
        {
            Draw(texture, position, null, color, 0, default, 0f, SpriteEffects.None, 0f);
        }
        public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color)
        {
            Draw(texture, position, sourceRectangle, color, 0, default, 0f, SpriteEffects.None, 0f);
        }
        public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color)
        {
            Draw(texture, destinationRectangle, sourceRectangle, color, 0, default, SpriteEffects.None, 0f);
        }
        public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
        {
            Draw(texture, position, sourceRectangle, color, rotation, origin, new Vector2(scale, scale), effects, layerDepth);
        }
        public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
        {
            vertexBatch.Draw(new SpriteDrawInfo(texture, position, sourceRectangle, color, origin, scale, rotation, effects, layerDepth));
        }
        public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth)
        {
            Draw(texture, destinationRectangle.Location.ToVector2(), sourceRectangle, color, rotation, origin, new Vector2(destinationRectangle.Width / (float)texture.Width, destinationRectangle.Height / (float)texture.Height), effects, layerDepth);
        }
        public void DrawString(SpriteFont spriteFont, StringBuilder text, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
        {
            DrawString(spriteFont, text.ToString(), position, color, rotation, origin, scale, effects, layerDepth);
        }
        public void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color)
        {
            DrawString(spriteFont, text, position, color, 0f, default, Vector2.One, SpriteEffects.None, 1f);
        }
        public void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
        {
            DrawString(spriteFont, text, position, color, rotation, origin, new Vector2(scale, scale), effects, layerDepth);
        }
        public void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
        {
            if (DrawStringAction == null)
            {
                CacheFont(spriteFont);
                if (origin == spriteFont.MeasureString(text) / 2) origin = fonts[spriteFont].MeasureString(text, scale) / 2;
                fonts[spriteFont].DrawString(vertexBatch, text, position, color, origin, scale, rotation, effects, layerDepth);
            }
            else
            {
                DrawStringAction(spriteFont, text, position, color, rotation, origin, scale, effects, layerDepth);
            }
        }
        public void DrawString(SpriteFont spriteFont, StringBuilder text, Vector2 position, Color color)
        {
            DrawString(spriteFont, text.ToString(), position, color, 0f, default, Vector2.One, SpriteEffects.None, 1f);
        }
        public void DrawString(SpriteFont spriteFont, StringBuilder text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
        {
            DrawString(spriteFont, text.ToString(), position, color, rotation, origin, new Vector2(scale, scale), effects, layerDepth);
        }
        private void CacheFont(SpriteFont spriteFont)
        {
            if (fonts.ContainsKey(spriteFont)) return;
            fonts.Add(spriteFont, new DynamicSpriteFont(vertexBatch.graphicsDevice, new SpriteFontCoventer(spriteFont), spriteFont.MeasureString("A").Y, new Vector2(spriteFont.Spacing, spriteFont.LineSpacing)));
        }
        public void End()
        {
            vertexBatch.End();
        }
    }
    public class SpriteFontCoventer : IFont
    {
        Point size;
        byte[] fontBitmap;
        SpriteFont.Glyph[] glyphs;
        char defaultChar;
        public SpriteFontCoventer(SpriteFont spriteFont)
        {
            glyphs = spriteFont.Glyphs;
            size = new Point(spriteFont.Texture.Width, spriteFont.Texture.Height);
            fontBitmap = new byte[spriteFont.Texture.Width * spriteFont.Texture.Height];
            spriteFont.Texture.GetData(fontBitmap);
            fontBitmap = ReflectionHelper.GetMGClass("Microsoft.Xna.Framework.Graphics.DxtUtil").GetMethod("DecompressDxt5", BindingFlags.NonPublic | BindingFlags.Static
                , null, new Type[] { typeof(byte[]), typeof(int), typeof(int) }, null)
                .Invoke(null, new object[] { fontBitmap, size.X, size.Y }) as byte[];
            byte[] cache = new byte[size.X * size.Y];
            for (int j = 0; j < cache.Length; j++)
            {
                cache[j] = (byte)((fontBitmap[j * 4 + 0] + fontBitmap[j * 4 + 1] + fontBitmap[j * 4 + 2]) / 3);
            }
            fontBitmap = cache;
            defaultChar = spriteFont.DefaultCharacter.GetValueOrDefault();
        }
        public bool HaveGlyph(int codepoint)
        {
            try
            {
                glyphs.First(g => g.Character == codepoint);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public Glyph[] GetGlyphsFromCodepoint(float height, int[] codepoint, float scaleX, float scaleY)
        {
            Glyph[] result = new Glyph[codepoint.Length];
            for (int i = 0; i < codepoint.Length; i++)
            {
                SpriteFont.Glyph g = glyphs.FirstOrDefault(g => g.Character == codepoint[i]);
                if (g.Character == default(char)) glyphs.FirstOrDefault(g => g.Character == defaultChar);
                byte[] b = new byte[g.BoundsInTexture.Width * g.BoundsInTexture.Height];
                b = Helper.CutBitmap(fontBitmap, size.X, g.BoundsInTexture.X, g.BoundsInTexture.Y, g.BoundsInTexture.Width, g.BoundsInTexture.Height);
                int f = (int)((g.LeftSideBearing + g.RightSideBearing) / 1.33f);
                int h = (int)(g.Cropping.Height / 1.5f);
                result[i] = new Glyph(b, f, g.BoundsInTexture.Width + f, g.Cropping.Y - h, g.BoundsInTexture.Height + g.Cropping.Y - h);
            }
            return result;
        }
    }
}
