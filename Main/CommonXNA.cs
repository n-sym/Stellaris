using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
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
        Android = 2
    }
    public static class Common
    {
        public static Platform platform = Platform.Windows;
        private static MouseState mouseState;
        private static float lastScrollWheel;
        private static GraphicsDeviceManager graphics;
        public static CommonMouseState MouseState { get; private set; }
        public static CommonMouseState LastMouseState { get; private set; }
        public static Vector2 Resolution { get; private set; }
        public static int FPS { get; private set; }
        public static int Quality { get; private set; }
        public static Game game;
        public static void Initialize(Game game)
        {
            Common.game = game;
            graphics = game.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static).First(m => m.FieldType == typeof(GraphicsDeviceManager)).GetValue(game) as GraphicsDeviceManager;
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
        public static void Update()
        {
            Resolution = new Vector2(game.Window.ClientBounds.Width, game.Window.ClientBounds.Height);
            LastMouseState = MouseState;
            mouseState = Mouse.GetState();
            MouseState = new CommonMouseState(new Vector2(mouseState.X, mouseState.Y), mouseState.LeftButton, mouseState.RightButton, mouseState.ScrollWheelValue - lastScrollWheel);
        }
        public static Stream GetAsset(string path)
        {
            return File.OpenRead(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "Assets" + Path.DirectorySeparatorChar + path);
        }
    }
}
