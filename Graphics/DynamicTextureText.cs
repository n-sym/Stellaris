using Microsoft.Xna.Framework.Graphics;
using System.Drawing;
using Color = Microsoft.Xna.Framework.Color;
using ColorS = System.Drawing.Color;
using Graphic = System.Drawing.Graphics;
using Point = Microsoft.Xna.Framework.Point;

namespace Stellaris.Graphics
{
    public class DynamicTextureTextGDI : DynamicTexture
    {
        Font font;
        string text;
        public DynamicTextureTextGDI(GraphicsDevice graphicsDevice, Font font, string text) : base(graphicsDevice, ((text + font.Size.ToString()).GetHashCode()).ToString())
        {
            this.font = font;
            this.text = text;
            GetTexture();
        }
        public static Bitmap GetBitmap(Font font, string text)
        {
            Graphic graphic = Graphic.FromImage(new Bitmap(1, 1));
            SizeF size = graphic.MeasureString(text, font);
            Bitmap bitmap = new Bitmap((int)size.Width + 1, (int)size.Height + 1);
            Brush brush = new SolidBrush(ColorS.White);
            graphic = Graphic.FromImage(bitmap);
            graphic.DrawString(text, font, brush, 0, 0);
            graphic.Dispose();
            return bitmap;
        }
        private void GetTexture()
        {
            if (!LoadCache())
            {
                Bitmap bitmap = GetBitmap(font, text);
                int width = bitmap.Width;
                int height = bitmap.Height;
                Color[] data = new Color[width * height];
                for (int i = 0; i < width * height; i++)
                {
                    Point p = IndexToPoint(i, width, height);
                    data[i] = ColorConversion(bitmap.GetPixel(p.X, p.Y));
                }
                Texture2D texture2D = new Texture2D(graphicsDevice, width, height);
                texture2D.SetData(data);
                _texture = new Texture2D[] { texture2D };
                _width[0] = width;
                _height[0] = height;
                DoCache();
            }
        }
        private Point IndexToPoint(int index, int width, int height)
        {
            int y = index / width;
            int x = y * width;
            return new Point(index - x, y);
        }
        private Color ColorConversion(ColorS color)
        {
            return new Color(color.R, color.G, color.B);
        }
    }
}
