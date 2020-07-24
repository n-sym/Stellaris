using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = Microsoft.Xna.Framework.Color;
using ColorS = System.Drawing.Color;
using Graphic = System.Drawing.Graphics;
using Point = Microsoft.Xna.Framework.Point;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing;

namespace Stellaris.Graphics
{
    public class DynamicTextureText : DynamicTexture
    {
        Font font;
        string text;
        public DynamicTextureText(GraphicsDevice graphicsDevice, Font font, string text) : base(graphicsDevice, (text.GetHashCode() * font.Size).ToString())
        {
            this.font = font;
            this.text = text;
            FontConversion();
        }
        
        private void FontConversion()
        {
            if (!LoadCache())
            {
                Graphic graphic = Graphic.FromImage(new Bitmap(1, 1));
                SizeF size = graphic.MeasureString(text, font);
                Bitmap bitmap = new Bitmap((int)size.Width + 1, (int)size.Height + 1);
                int width = bitmap.Width;
                int height = bitmap.Height;
                _width[0] = width;
                _height[0] = height;
                Brush brush = new SolidBrush(ColorS.White);
                graphic.Dispose();
                graphic = Graphic.FromImage(bitmap);
                graphic.DrawString(text, font, brush, 0, 0);
                graphic.Save();
                Color[] data = new Color[width * height];
                for (int i = 0; i < width * height; i++)
                {
                    Point p = IndexToPoint(i);
                    data[i] = ColorConversion(bitmap.GetPixel(p.X - 1, p.Y - 1));
                }
                Texture2D texture2D = new Texture2D(graphicsDevice, width, height);
                texture2D.SetData(data);
                _texture = new Texture2D[] { texture2D };
                DoCache();
            }
        }
        private Color ColorConversion(ColorS color)
        {
            return new Color(color.R, color.G, color.B);
        }
    }
}
