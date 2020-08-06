using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellaris.Graphics;
using System;
using System.Collections.Generic;

namespace Stellaris.Test
{
    class TestTex : DynamicTexture
    {
        public TestTex(GraphicsDevice graphicsDevice, int width, int height, string name = "") : base(graphicsDevice, width, height, name)
        {
        }
        protected override List<Color[]> Generating()
        {
            var data = new List<Color[]>();
            data.Add(new Color[Width * Height]);
            bool d = false;
            int seed = 0;
            float ly = 1;
            for (int i = 0; i < data[0].Length; i++)
            {
                var p = IndexToVector(i);
                if(p.Y != ly)
                {
                    ly = p.Y;
                    d = new Random(seed).Next(0, 2) == 1;
                    seed = new Random(seed).Next();
                }
                if (d) data[0][i] = Color.White;
                else data[0][i] = Color.White * 0.33f;
            }
            return data;
        }
    }
}
