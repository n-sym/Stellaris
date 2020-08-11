using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stellaris
{
    public static class FontHelper
    {
        public static string WinFontFolder = Environment.GetEnvironmentVariable("windir") + "\\Fonts\\";
        public static string AndroidFontFolder = "/System/fonts/";
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
    }
}
