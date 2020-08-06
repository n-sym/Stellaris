using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellaris.Graphics;
using System;
using System.Collections.Generic;

namespace Stellaris.Test
{
    class FlareFxAlt : DynamicTexture
    {
        static int[] a = { 40, 40 };
        static int[] b = { 200, 200 };
        public FlareFxAlt(GraphicsDevice graphicsDevice, int width, int height, string name = "") : base(graphicsDevice, width, height, name)
        {
        }
        protected override List<Color[]> Generating()
        {
            var data = new List<Color[]>();
            data.Add(new Color[Width * Height]);
            var r = (Width + Height) / 2;
            var c = new Vector2(Width, Height) / 2;
            for (int i = 0; i < data[0].Length; i++)
            {
                var p = IndexToVector(i) - c;
                var k = p.Slop();
                if (k <= 1 && k > -1)
                {
                    var t = 0.5f - Math.Abs(p.Length() / (r * p.Length() / p.X));
                    data[0][i] = Color.White * (0.5f * t * t + 0.5f * t) * 1.4f;
                }
                else
                {
                    var t = 0.5f - Math.Abs(p.Length() / (r * p.Length() / p.Y));
                    data[0][i] = Color.White * (0.5f * t * t + 0.5f * t) * 1.4f;
                }
                if (k == 1 || k == -1) data[0][i] *= 0.95f;
                if (p.Length() <= 1) data[0][i] *= 0.95f;
                if (p == Vector2.Zero) data[0][i] = Color.White * 0.6f;
                data[0][i] = data[0][i].LinearTo(Color.White * ((1 - (p.Length() / c.Length())) / 2), 2, 5);
            }
            return data;
        }
    }
}
