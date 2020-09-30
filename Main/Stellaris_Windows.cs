using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Stellaris
{
    public static class Ste
    {
        public static readonly Platform Platform = Platform.Windows;
        public static string CurrentDirectory => Environment.CurrentDirectory;
        private static TouchCollection touchCollection;
        private static TouchLocation[] touchLocations;
        private static MouseState mouseState;
        private static float lastScrollWheel;
        public static CommonMouseState MouseState { get; private set; }
        public static CommonMouseState LastMouseState { get; private set; }
        public static Vector2 MousePos => MouseState.Position;
        public static Vector2 Resolution { get; private set; }
        public static float Resolution_X => Resolution.X;
        public static float Resolution_Y => Resolution.Y;
        public static int FPS { get; private set; }
        public static int Quality { get; private set; }
        public static Game game;
        public static GraphicsDeviceManager graphics;
        public static void Initialize(Game game, GraphicsDeviceManager graphics)
        {
            Ste.game = game;
            Ste.graphics = graphics;
            NativeMethods.Initialize();
        }
        public static void ChangeResolution(int x, int y)
        {
            graphics.PreferredBackBufferWidth = x;
            graphics.PreferredBackBufferHeight = y;
            Resolution = new Vector2(x, y);
            graphics.ApplyChanges();
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
        public static void UpdateInput()
        {
            Resolution = game.Window.ClientBounds.Size.ToVector2();
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
                    MouseState = new CommonMouseState(MouseState.Position, ButtonState.Released, ButtonState.Released, 0);
                }
                else
                {
                    MouseState = new CommonMouseState(touchLocations[0].Position, touchLocations.Length == 1 ? ButtonState.Pressed : ButtonState.Released, touchLocations.Length == 1 ? ButtonState.Released : ButtonState.Pressed, 0);
                }
            }
            lastScrollWheel = mouseState.ScrollWheelValue;
        }
        public static Stream GetAsset(string path)
        {
            return File.OpenRead(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "Assets" + Path.DirectorySeparatorChar + path);
        }
    }
}
