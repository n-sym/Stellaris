using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Runtime.InteropServices;
using static StbTrueTypeSharp.StbTrueType;

namespace Stellaris
{
    public unsafe class FontStb : IFont, IDisposable
    {
        public IFont? ReverseFont;
        stbtt_fontinfo font;
        public FontStb(byte[] ttf)
        {
            Initialize(ttf);
        }
        public FontStb(Stream stream)
        {
            Initialize(stream.ToByteArray());
        }
        public FontStb(string path)
        {
            using (FileStream fileStream = File.OpenRead(path))
            {
                Initialize(fileStream.ToByteArray());
            }
        }
        private void Initialize(byte[] ttf)
        {
            byte* bytePtr = (byte*)GCHandle.Alloc(ttf, GCHandleType.Pinned).AddrOfPinnedObject();
            font = new stbtt_fontinfo();
            stbtt_InitFont(font, bytePtr, 0);
            HaveGlyph(int.MaxValue);
            GC.Collect();
        }
        public bool HaveGlyph(int codepoint)
        {
            return stbtt_FindGlyphIndex(font, codepoint) != 0;
        }
        public Glyph[] GetGlyphsFromCodepoint(float height, int[] codepoint, float scaleX, float scaleY)
        {
            float scale = stbtt_ScaleForPixelHeight(font, height);
            Glyph[] result = new Glyph[codepoint.Length];
            for (int i = 0; i < codepoint.Length; i++)
            {
                if (HaveGlyph(codepoint[i]))
                {
                    int x0, x1, y0, y1;
                    stbtt_GetCodepointBitmapBox(font, codepoint[i], scale * scaleX, scale * scaleY, &x0, &y0, &x1, &y1);
                    int w = x1 - x0;
                    int h = y1 - y0;
                    byte[] bitmap = new byte[w * h];
                    fixed (byte* bytePtr = bitmap)
                    {
                        stbtt_MakeCodepointBitmap(font, bytePtr, w, h, w, scale * scaleX, scale * scaleY, codepoint[i]);
                    }
                    result[i] = new Glyph(bitmap, x0, x1, y0, y1);
                }
                else
                {
                    if (ReverseFont == null) result[i] = new Glyph(new byte[0], 0, 0, 0, 0);
                    else result[i] = ReverseFont.GetGlyphsFromCodepoint(height, new int[] { codepoint[i] }, scaleX, scaleY)[0];
                }
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
