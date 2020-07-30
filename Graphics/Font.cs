using System;
using static StbTrueTypeSharp.StbTrueType;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Stellaris.Graphics
{
    public class Glygh
    {
        public Texture2D texture;
        public int x0, x1, y0, y1;
        public int Width => x1 - x0;
        public int Height => y1 - y0;
        public Glygh(Texture2D texture, int x0, int x1, int y0, int y1)
        {
            this.texture = texture;
            this.x0 = x0;
            this.x1 = x1;
            this.y0 = y0;
            this.y1 = y1;
        }
    }
    public unsafe class FontStb : IDisposable
    {
        stbtt_fontinfo font;
        GraphicsDevice graphicsDevice;
        public FontStb(byte[] ttf, GraphicsDevice graphicsDevice = null)
        {
            Initialize(graphicsDevice, ttf);
        }
        public FontStb(Stream stream, GraphicsDevice graphicsDevice = null)
        {
            Initialize(graphicsDevice, stream.ToByteArray());
        }
        public FontStb(string path, GraphicsDevice graphicsDevice = null)
        {
            using (FileStream fileStream = File.OpenRead(path))
            {
                Initialize(graphicsDevice, fileStream.ToByteArray());
            }
        }
        private void Initialize(GraphicsDevice graphicsDevice, byte[] ttf)
        {
            this.graphicsDevice = graphicsDevice;
            byte* bytePtr = (byte*)GCHandle.Alloc(ttf, GCHandleType.Pinned).AddrOfPinnedObject();
            font = new stbtt_fontinfo();
            stbtt_InitFont(font, bytePtr, 0);
            GC.Collect();
        }
        public int[] GetGlyphIndex(IList<char> chars)
        {
            int[] result = new int[chars.Count];
            for (int i = 0; i < chars.Count; i++)
            {
                result[i] = stbtt_FindGlyphIndex(font, (int)chars[i]);
            }
            return result;
        }
        public int[] GetGlyphIndex(string str)
        {
            return GetGlyphIndex(str.ToCharArray());
        }
        public int GetGlyphIndex(char c)
        {
            return stbtt_FindGlyphIndex(font, (int)c);
        }
        public Glygh[] GetGlyphsFromIndex(float height, int[] glyph, float scaleX, float scaleY)
        {
            float scale = stbtt_ScaleForPixelHeight(font, height);
            Glygh[] result = new Glygh[glyph.Length];
            if (graphicsDevice == null) return result;
            for (int i = 0; i < glyph.Length; i++)
            {
                int x0, x1, y0, y1;
                stbtt_GetGlyphBitmapBox(font, glyph[i], scale * scaleX, scale * scaleY, &x0, &y0, &x1, &y1);
                int w = x1 - x0;
                int h = y1 - y0;
                if(w == 0 && h == 0)
                {
                    w = 1;
                    h = 1;
                }
                byte[] bitmap = new byte[w * h];
                fixed (byte* bytePtr = bitmap)
                {
                    stbtt_MakeGlyphBitmap(font, bytePtr, w, h, w, scale * scaleX, scale * scaleY, glyph[i]);
                }
                result[i] = new Glygh(Helper.ByteDataToTexture2D(graphicsDevice, bitmap, w, h), x0, x1, y0, y1);
            }
            GC.Collect();
            return result;
        }
        public Glygh[] GetGlyphsFromCodepoint(float height, int[] codepoint, float scaleX, float scaleY)
        {
            float scale = stbtt_ScaleForPixelHeight(font, height);
            Glygh[] result = new Glygh[codepoint.Length];
            if (graphicsDevice == null) return result;
            for (int i = 0; i < codepoint.Length; i++)
            {
                int x0, x1, y0, y1;
                stbtt_GetCodepointBitmapBox(font, codepoint[i], scale * scaleX, scale * scaleY, &x0, &y0, &x1, &y1);
                int w = x1 - x0;
                int h = y1 - y0;
                if (w == 0 && h == 0)
                {
                    w = 1;
                    h = 1;
                }
                byte[] bitmap = new byte[w * h];
                fixed (byte* bytePtr = bitmap)
                {
                    stbtt_MakeCodepointBitmap(font, bytePtr, w, h, w, scale * scaleX, scale * scaleY, codepoint[i]);
                }
                result[i] = new Glygh(Helper.ByteDataToTexture2D(graphicsDevice, bitmap, w, h), x0, x1, y0, y1);
            }
            GC.Collect();
            return result;
        }
        public void Dispose()
        {
            font = null;
        }
    }
}
