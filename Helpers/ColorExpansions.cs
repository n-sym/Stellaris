using System;
using Microsoft.Xna.Framework;

namespace Stellaris
{
    public static class ColorExpansions
    {
        public static Color LinearInterpolationTo(this Color a, Color b, float progress, float max)
        {
            return (progress / max * b.ToVector4() + (max - progress) / max * a.ToVector4()).ToColor();
        }
    }
}
