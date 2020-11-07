using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Stellaris
{
    public static class FontHelper
    {
        public static string WinFontFolder = Environment.GetEnvironmentVariable("windir") + "\\Fonts\\";
        public static string AndroidFontFolder = "/System/fonts/";
        unsafe static delegate*<byte*, bool*, int, int, void> makeFontBitmapOutline;
        public static bool IsCn(char c)
        {
            if (c >= 0x43ee && c <= 0x9fff) return true;
            return false;
        }
        public static bool IsRu(char c)
        {
            if (c >= 0x0400 && c <= 0x052f) return true;
            return false;
        }
        public static Texture2D ByteDataToTexture2D(GraphicsDevice graphicsDevice, byte[] bitmap, int width, int height)
        {
            Color[] colors = new Color[bitmap.Length];
            for (var i = 0; i < colors.Length; i++)
            {
                byte b = bitmap[i];
                colors[i].R = colors[i].G = colors[i].B = colors[i].A = b;
            }
            Texture2D result = new Texture2D(graphicsDevice, width, height);
            result.SetData(colors);
            return result;
        }
        public static Texture2D ByteDataToTexture2D(GraphicsDevice graphicsDevice, byte[] bitmap, bool[] extra, int width, int height)
        {
            Color[] colors = new Color[bitmap.Length];
            for (var i = 0; i < colors.Length; i++)
            {
                byte b = bitmap[i];
                if (extra[i])
                {
                    colors[i] = new Color(20, 20, 20);
                }
                else colors[i].R = colors[i].G = colors[i].B = colors[i].A = b;
            }
            Texture2D result = new Texture2D(graphicsDevice, width, height);
            result.SetData(colors);
            return result;
        }
        public unsafe static bool[] MakeFontBitmapOutline(byte[] bitmap, int width, int height, bool useNative)
        {
            bool[] bitmap_ = new bool[bitmap.Length];
            if (useNative)
            {
                //if (makeFontBitmapOutline == null) makeFontBitmapOutline = (delegate*<byte*, bool*, int, int, void>)Ste.Native.GetMethodPtr("MakeFontBitmapOutline");
                fixed (byte* p = bitmap)
                {
                    fixed (bool* b = bitmap_)
                    {
                        makeFontBitmapOutline(p, b, width, height);
                    }
                    return bitmap_;
                }
            }
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    if (bitmap[j * width + i] != 0)
                    {
                        if (i == 0 || i == width - 1) bitmap_[j * width + i] = true;
                        else if (bitmap[j * width + i + 1] < 100) bitmap_[j * width + i + 1] = true;
                        else if (bitmap[j * width + i - 1] < 100) bitmap_[j * width + i - 1] = true;
                        if (j == 0 || j == height - 1) bitmap_[j * width + i] = true;
                        else if (bitmap[j * width + i + width] < 100) bitmap_[j * width + i + width] = true;
                        else if (bitmap[j * width + i - width] < 100) bitmap_[j * width + i - width] = true;
                    }
                }
            }
            return bitmap_;
        }
    }
}
