using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Stellaris
{
    public class Glyph
    {
        public Texture2D texture;
        public int x0, x1, y0, y1;
        public int Width => x1 - x0;
        public int Height => y1 - y0;
        public int WidthAlt => x1 + x0;
        public int HeightAlt => Math.Abs(y1) + Math.Abs(y0);
        public Glyph(Texture2D texture, int x0, int x1, int y0, int y1)
        {
            this.texture = texture;
            this.x0 = x0;
            this.x1 = x1;
            this.y0 = y0;
            this.y1 = y1;
        }
    }
    public interface IFont
    {
        public Glyph[] GetGlyphsFromCodepoint(float height, int[] codepoint, float scaleX, float scaleY);
    }
}
