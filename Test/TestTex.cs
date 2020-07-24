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
    class TestTex : DynamicTexture
    {
        public TestTex(GraphicsDevice graphicsDevice, int width, int height, string name = "") : base(graphicsDevice, width, height,  name)
        {
        }
        protected override List<Color[]> Generate()
        {
            var data = new List<Color[]>();
            data.Add(new Color[Width * Height]);
            for (int i = 0; i < data[0].Length; i++)
            {
                var p = IndexToVector(i);
                Color c = Color.Blue.LinearInterpolationTo(Color.Red, p.X, Width);
                c = c.LinearInterpolationTo(Color.Green, p.Y, Height);
                data[0][i] = c;
            }
            return data;
        }
    }
}
