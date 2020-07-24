using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stellaris.UI
{
    public class UIBase
    {
        public Vector2 postion;
        public float width;
        public float height;
        public Vector2 Size => new Vector2(width, height);
        public Rectangle Box => new Rectangle((int)postion.X, (int)postion.Y, (int)width, (int)height);
    }
}
