using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System;
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
        PC = 1,
        Android = 2
    }
    public static class Common
    {
        public static Platform platform = Platform.PC;
        private static TouchCollection touchCollection;
        private static TouchLocation[] touchLocations;
        private static MouseState mouseState;
        private static float lastScrollWheel;
        private static GraphicsDeviceManager graphics;
        public static CommonMouseState MouseState { get; private set; }
        public static CommonMouseState LastMouseState { get; private set; }
        public static Vector2 Resolution { get; private set; }
        public static int FPS { get; private set; }
        public static int Quality { get; private set; }
        public static void Initialize(Game game)
        {
            graphics = game.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).First(m => m.FieldType == typeof(GraphicsDeviceManager)).GetValue(game) as GraphicsDeviceManager;

        }
        private static DateTime lastTime;
        public static void UpdateFPS(GameTime gameTime)
        {
            DateTime nowTime = DateTime.Now;
            if (lastTime == null) lastTime = nowTime;
            FPS = (int)(200d / (nowTime.TimeOfDay.TotalMilliseconds - lastTime.TimeOfDay.TotalMilliseconds) + (FPS / 5d) + (600d / gameTime.ElapsedGameTime.TotalMilliseconds));
            if (Quality > 2) Quality = (int)Math.Ceiling(FPS / 30f + (Quality / 2f));
            else Quality = (int)(FPS / 30f + (Quality / 2f));
            lastTime = nowTime;
        }
        public static void UpdateFPS()
        {
            DateTime nowTime = DateTime.Now;
            if (lastTime == null) lastTime = nowTime;
            FPS = (int)(500d / (nowTime.TimeOfDay.TotalMilliseconds - lastTime.TimeOfDay.TotalMilliseconds) + (FPS / 2d));
            if (Quality > 2) Quality = (int)Math.Ceiling(FPS / 30f + (Quality / 2f));
            else Quality = (int)(FPS / 30f + (Quality / 2f));
            lastTime = nowTime;
        }
        public static void Update(Game game)
        {
            Resolution = new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            touchCollection = TouchPanel.GetState(game.Window).GetState();
            touchLocations = touchCollection.ToArray();
            LastMouseState = MouseState;
            mouseState = Mouse.GetState();
            if (mouseState.Position != default)
            {
                MouseState = new CommonMouseState(mouseState.Position.ToVector2(), mouseState.LeftButton, mouseState.RightButton, mouseState.ScrollWheelValue - lastScrollWheel);
            }
            else
            {
                if (touchLocations.Length == 0)
                {
                    MouseState = new CommonMouseState(MouseState.position, ButtonState.Released, ButtonState.Released, 0);
                }
                else
                {
                    MouseState = new CommonMouseState(touchLocations[0].Position, touchLocations.Length == 1 ? ButtonState.Pressed : ButtonState.Released, touchLocations.Length == 1 ? ButtonState.Released : ButtonState.Pressed, 0);
                }
            }
            lastScrollWheel = mouseState.ScrollWheelValue;
        }
    }
}
