using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellaris.Graphics;

namespace Stellaris.Test
{
    class FlareFx : DynamicTexture
    {
        static int[] a = { 40, 40 };
        static int[] b = { 200, 200 };
        public FlareFx(GraphicsDevice graphicsDevice, int width, int height, string name = "") : base(graphicsDevice, width, height,  name)
        {
        }
        protected override List<Color[]> Generate()
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
                    data[0][i] = Color.White * (0.25f * t * t + 0.75f * t) * 1.5f;
                }
                else
                {
                    var t = 0.5f - Math.Abs(p.Length() / (r * p.Length() / p.Y));
                    data[0][i] = Color.White * (0.25f * t * t + 0.75f * t) * 1.5f;
                }
                if (k == 1 || k == -1) data[0][i] *= 1f - ((p.Length() / c.Length()) * (p.Length() / c.Length()) * (p.Length() / c.Length()) - 1) * 0.35f;
                if (Math.Abs(p.Y - p.X) == 1 || Math.Abs(p.Y + p.X) == 1) data[0][i] *= 1f - ((p.Length() / c.Length()) * (p.Length() / c.Length()) * (p.Length() / c.Length()) - 1) * 0.15f;
                if (p.Length() <= c.Length() / 2) data[0][i] *= 1f - (p.Length() / (c.Length() / 2) - 1) * 0.5f;
                if (p == Vector2.Zero) data[0][i] = Color.White;
                data[0][i] *= (1 - (p.Length() / c.Length())) * 0.25f + 0.75f;
            }
            return data;
        }
    }
}
