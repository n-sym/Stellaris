using Microsoft.Xna.Framework;
using System;

namespace Stellaris.Curves
{
    public abstract class CurveBase
    {
        public Vector2 position;
        public Func<float, Vector2> curve;
        public CurveBase()
        {
            Make();
        }
        public Vector2 Get(float t)
        {
            if (curve != null) return curve(t);
            else return position;
        }
        protected virtual void Make()
        {

        }
        public static CurveBase operator +(CurveBase c, float n)
        {
            c.position.Y += n;
            return c;
        }
        public static CurveBase operator +(CurveBase c, Vector2 v)
        {
            c.position += v;
            return c;
        }
        public static CurveBase operator -(CurveBase c, float n)
        {
            c.position.Y -= n;
            return c;
        }
        public static CurveBase operator -(CurveBase c, Vector2 v)
        {
            c.position -= v;
            return c;
        }
    }
}
