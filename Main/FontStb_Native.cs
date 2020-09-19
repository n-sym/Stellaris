using Microsoft.Xna.Framework.Graphics;
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
        FontInfo font;
        GraphicsDevice graphicsDevice;
        public FontStb_Native(byte[] ttf, GraphicsDevice graphicsDevice = null)
        {
            Initialize(graphicsDevice, ttf);
        }
        public FontStb_Native(Stream stream, GraphicsDevice graphicsDevice = null)
        {
            Initialize(graphicsDevice, stream.ToByteArray());
        }
        public FontStb_Native(string path, GraphicsDevice graphicsDevice = null)
        {
            using (FileStream fileStream = File.OpenRead(path))
            {
                Initialize(graphicsDevice, fileStream.ToByteArray());
            }
        }
        private delegate FontInfo _helper_getfontinfo(byte* data, int offset);
        private delegate float _stbtt_ScaleForPixelHeight(FontInfo font, float height);
        private delegate void _stbtt_GetCodepointBitmapBox(FontInfo font, int codepoint, float scale_x, float scale_y, int* ix0, int* iy0, int* ix1, int* iy1);
        private delegate void _stbtt_MakeCodepointBitmap(FontInfo info, byte* output, int out_w, int out_h, int out_stride, float scale_x, float scale_y, int codepoint);
        static _helper_getfontinfo helper_getfontinfo;
        static _stbtt_ScaleForPixelHeight stbtt_ScaleForPixelHeight;
        static _stbtt_GetCodepointBitmapBox stbtt_GetCodepointBitmapBox;
        static _stbtt_MakeCodepointBitmap stbtt_MakeCodepointBitmap;
        private void Initialize(GraphicsDevice graphicsDevice, byte[] ttf)
        {
            this.graphicsDevice = graphicsDevice;
            byte* bytePtr = (byte*)GCHandle.Alloc(ttf, GCHandleType.Pinned).AddrOfPinnedObject();
            if (helper_getfontinfo == null)
            {
                helper_getfontinfo = NativeMethods.GetMethod<_helper_getfontinfo>("helper_getfontinfo");
                stbtt_ScaleForPixelHeight = NativeMethods.GetMethod<_stbtt_ScaleForPixelHeight>("stbtt_ScaleForPixelHeight");
                stbtt_GetCodepointBitmapBox = NativeMethods.GetMethod<_stbtt_GetCodepointBitmapBox>("stbtt_GetCodepointBitmapBox");
                stbtt_MakeCodepointBitmap = NativeMethods.GetMethod<_stbtt_MakeCodepointBitmap>("stbtt_MakeCodepointBitmap");
            }
            font = helper_getfontinfo(bytePtr, 0);
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
            //font = null;
            GC.Collect();
        }
    }
}
