using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Stellaris
{
    public unsafe class FontStb_Native : IFont, IDisposable
    {
        [StructLayout(LayoutKind.Sequential, Size = 160)]
        struct FontInfo
        {

        }

        public IFont? ReverseFont;
        FontInfo font;
        public FontStb_Native(byte[] ttf)
        {
            Initialize(ttf);
        }
        public FontStb_Native(Stream stream)
        {
            Initialize(stream.ToByteArray());
        }
        public FontStb_Native(string path)
        {
            using (FileStream fileStream = File.OpenRead(path))
            {
                Initialize(fileStream.ToByteArray());
            }
        }
        private static delegate*<byte*, int, FontInfo> helper_GetFontInfo;
        private static delegate*<FontInfo, float, float> stbtt_ScaleForPixelHeight;
        private static delegate*<FontInfo, int, float, float, int*, int*, int*, int*, void> stbtt_GetCodepointBitmapBox;
        private static delegate*<FontInfo, byte*, int, int, int, float, float, int, void> stbtt_MakeCodepointBitmap;
        private static delegate*<FontInfo, int, int> stbtt_FindGlyphIndex;

        private void Initialize(byte[] ttf)
        {
            byte* bytePtr = (byte*)GCHandle.Alloc(ttf, GCHandleType.Pinned).AddrOfPinnedObject();
            if (helper_GetFontInfo == null)
            {
                helper_GetFontInfo = (delegate*<byte*, int, FontInfo>)Ste.Native.GetMethodPtr("helper_GetFontInfo");
                stbtt_ScaleForPixelHeight = (delegate*<FontInfo, float, float>)Ste.Native.GetMethodPtr("stbtt_ScaleForPixelHeight");
                stbtt_GetCodepointBitmapBox = (delegate*<FontInfo, int, float, float, int*, int*, int*, int*, void>)Ste.Native.GetMethodPtr("stbtt_GetCodepointBitmapBox");
                stbtt_MakeCodepointBitmap = (delegate*<FontInfo, byte*, int, int, int, float, float, int, void>)Ste.Native.GetMethodPtr("stbtt_MakeCodepointBitmap");
                stbtt_FindGlyphIndex = (delegate*<FontInfo, int, int>)Ste.Native.GetMethodPtr("stbtt_FindGlyphIndex");
            }
            font = helper_GetFontInfo(bytePtr, 0);
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
            //font = null;
            GC.Collect();
        }
    }
}
