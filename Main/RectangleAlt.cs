using Microsoft.Xna.Framework;

namespace Stellaris
{
    public struct RectangleAlt
    {
        public float X;
        public float Y;
        public float Width;
        public float Height;
        public Vector2 Position => new Vector2(X, Y);
        public Vector2 Size => new Vector2(Width, Height);
        public RectangleAlt(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
        public RectangleAlt(Vector2 position, Vector2 size)
        {
            X = position.X;
            Y = position.Y;
            Width = size.X;
            Height = size.Y;
        }
        public bool Intersects(RectangleAlt rectangle)
        {
            if ((rectangle.X + rectangle.Width <= X) || (rectangle.Y + rectangle.Height <= Y)) return false;
            if ((X + Width <= rectangle.X) || (Y + Height <= rectangle.Y)) return false;
            return true;
        }
        public bool Contains(Vector2 vector)
        {
            if (vector.X < X) return false;
            if (vector.Y < Y) return false;
            if (vector.X > X + Width) return false;
            if (vector.Y > Y + Height) return false;
            return true;
        }
        public Rectangle ToXNA()
        {
            return new Rectangle((int)X, (int)Y, (int)(Width), (int)Height);
        }
    }
}
