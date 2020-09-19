using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Stellaris
{
    public struct CommonMouseState
    {
        public Vector2 position;
        public ButtonState left;
        public ButtonState right;
        public float scrollWheel;
        public float X => position.X;
        public float Y => position.Y;
        public CommonMouseState(Vector2 position, ButtonState left, ButtonState right, float scrollWheel)
        {
            this.position = position;
            this.left = left;
            this.right = right;
            this.scrollWheel = scrollWheel;
        }
    }
    public enum Platform
    {
        Windows = 1,
        Linux = 2,
        Android = 3
    }
}
