using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellaris.Graphics;
using System.Collections.Generic;

namespace Stellaris.Test
{
    class SwordFx : DynamicTexture
    {
        static int[] a = { 40, 40 };
        static int[] b = { 200, 200 };
        public SwordFx(GraphicsDevice graphicsDevice, int maxFrame, string name = "") : base(graphicsDevice, a, b, maxFrame, name)
        {
        }
        protected override List<Color[]> Generating()
        {
            var data = new List<Color[]>();
            data.Add(new Color[Width * Height]);
            data.Add(new Color[Width * Height]);
            float k = Height / Width * 2;
            for (int i = 0; i < data[0].Length; i++)
            {
                Point point = IndexToPoint(i);
                float z = (float)point.Y * point.Y * point.Y / (Height * Height * Height);
                if ((point.X >= Width / 2 && point.Y > k * (point.X - (Width / 2))) || (point.X < Width / 2 && point.Y > -k * point.X + Height)) data[0][i] = Color.White * z;

            }
            for (int i = 0; i < data[1].Length; i++)
            {
                Point point = IndexToPoint(i);
                if ((point.X >= Width / 2 && point.Y > k * (point.X - (Width / 2))) || (point.X < Width / 2 && point.Y > -k * point.X + Height)) data[1][i] = Color.LightGoldenrodYellow * ((float)point.Y * point.Y * point.Y / (Height * Height * Height));
            }

            return data;
        }
    }
}
