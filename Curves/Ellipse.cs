using Microsoft.Xna.Framework;
using System;

namespace Stellaris.Curves
{
    public class Ellipse : CurveBase
    {
        public float a;
        public float b;
        public Ellipse(int a, int b, Vector2 position)
        {
            this.a = a;
            this.b = b;
            this.position = position;
        }
        public Ellipse(int a, int b)
        {
            this.a = a;
            this.b = b;
            this.position = Vector2.Zero;
        }
        protected override void Make()
        {
            curve = delegate (float t)
            {
                return new Vector2((float)Math.Cos(t) * a, (float)Math.Sin(t) * b) + position;
            };
        }
    }
}
