using Microsoft.Xna.Framework;

namespace Stellaris
{
    public static class PointExpansions
    {
        public static Vector2 ToVector2(this Point p)
        {
            return new Vector2(p.X, p.Y);
        }
    }
}
