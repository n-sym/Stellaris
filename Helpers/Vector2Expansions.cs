using System;
using Microsoft.Xna.Framework;

namespace Stellaris
{
    public static class Vector2Expansions
    {
        public static Vector2 RotateTo(this Vector2 vec, float radian)
        {
            float l = vec.Length();
            return new Vector2((float)Math.Cos(radian) * l, (float)Math.Sin(radian) * l);
        }
        public static Vector2 RotateTo(this Vector2 vec, float radian, Vector2 center)
        {
            vec -= center;
            float l = vec.Length();
            return new Vector2((float)Math.Cos(radian) * l, (float)Math.Sin(radian) * l) + center;
        }
        public static Vector2 Rotate(this Vector2 vec, float radian)
        {
            float c = (float)Math.Cos(radian);
            float s = (float)Math.Sin(radian);
            return new Vector2(c * vec.X - s * vec.Y, s * vec.X + c * vec.Y);
        }
        public static Vector2 Rotate(this Vector2 vec, float radian, Vector2 center = default)
        {
            vec -= center;
            float c = (float)Math.Cos(radian);
            float s = (float)Math.Sin(radian);
            return new Vector2(c * vec.X - s * vec.Y, s * vec.X + c * vec.Y) + center;
        }
        public static float Angle(this Vector2 vec)
        {
            float result = (float)Math.Atan(vec.Y / vec.X);
            if (vec.X < 0) result += 3.1415926f;
            return result;
        }
        public static float Slop(this Vector2 vec)
        {
            return vec.Y / vec.X;
        }
        public static Vector2 LinearInterpolationTo(this Vector2 a, Vector2 b, float progress, float max)
        {
            return progress / max * b + (max - progress) / max * a;
        }
        public static Vector2 FlipHorizontally(this Vector2 vec)
        {
            vec.X *= -1;
            return vec;
        }
        public static Vector2 FlipVertically(this Vector2 vec)
        {
            vec.Y *= -1;
            return vec;
        }
        public static Vector2 NormalizeAlt(this Vector2 vec)
        {
            vec.Normalize();
            return vec;
        }
        public static Vector2 Floor(this Vector2 vec)
        {
            return new Vector2((float)Math.Floor(vec.X), (float)Math.Floor(vec.Y));
        }
        public static Vector2 Ceiling(this Vector2 vec)
        {
            return new Vector2((float)Math.Ceiling(vec.X), (float)Math.Ceiling(vec.Y));
        }
        public static Point ToPoint(this Vector2 vec)
        {
            return new Point((int)vec.X, (int)vec.Y);
        }
    }
}
