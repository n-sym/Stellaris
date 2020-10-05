using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Runtime.InteropServices;
using static StbTrueTypeSharp.StbTrueType;

namespace Stellaris
{
    public unsafe class FontStb : IFont, IDisposable
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
        public Glyph[] GetGlyphsFromCodepoint(float height, int[] codepoint, float scaleX, float scaleY)
        {
            float scale = stbtt_ScaleForPixelHeight(font, height);
            Glyph[] result = new Glyph[codepoint.Length];
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
                result[i] = new Glyph(FontHelper.ByteDataToTexture2D(graphicsDevice, bitmap, w, h), x0, x1, y0, y1);
            }
            GC.Collect();
            return result;
        }
        public void Dispose()
        {
            font = null;
            GC.Collect();
        }
    }
}
