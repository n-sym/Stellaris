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
        public Vector2 Position;
        public ButtonState Left;
        public ButtonState Right;
        public float ScrollWheel;
        public float X => Position.X;
        public float Y => Position.Y;
        public CommonMouseState(Vector2 position, ButtonState left, ButtonState right, float scrollWheel)
        {
            this.Position = position;
            this.Left = left;
            this.Right = right;
            this.ScrollWheel = scrollWheel;
        }
    }
    public enum Platform
    {
        Windows = 1,
        Linux = 2,
        Android = 3
    }
}
