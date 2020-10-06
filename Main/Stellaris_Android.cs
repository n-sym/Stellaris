using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.IO;
using System.Linq;

namespace Stellaris
{
    /// <summary>
    /// Stellaris的管理器
    /// </summary>
    public static class Ste
    {
        private static TouchCollection touchCollection;
        private static TouchLocation[] touchLocations;
        private static MouseState mouseState;
        private static float lastScrollWheel;
        public static readonly Platform Platform = Platform.Windows;
        public static string CurrentDirectory => Game.Activity.ExternalCacheDir.AbsolutePath;
        public static CommonMouseState MouseState { get; private set; }
        public static CommonMouseState LastMouseState { get; private set; }
        public static Vector2 MousePos => MouseState.Position;
        public static Vector2 Resolution { get => new Vector2(Resolution_X, Resolution_Y); }
        public static int Resolution_X;
        public static int Resolution_Y;
        public static int FPS { get; private set; }
        public static int Quality { get; private set; }
        public static Texture2D Pixel;
        public static Game Game;
        public static GraphicsDeviceManager Graphics;
        internal static NativeMethods Native;
        /// <summary>
        /// 在使用Stellaris的各项功能前，强烈建议调用的初始化
        /// </summary>
        /// <param name="game"></param>
        /// <param name="graphics"></param>
        public static void Initialize(Game game, GraphicsDeviceManager graphics)
        {
            Ste.Game = game;
            Ste.Graphics = graphics;
            Native = new NativeMethods();
            Pixel = new Texture2D(game.GraphicsDevice, 1, 1);
            Pixel.SetData(new Color[] { Color.White });
        }
        /// <summary>
        /// 切换分辨率
        /// </summary>
        public static void ChangeResolution(int x, int y)
        {
            Graphics.PreferredBackBufferWidth = x;
            Graphics.PreferredBackBufferHeight = y;
            Resolution_X = x;
            Resolution_Y = y;
            Graphics.ApplyChanges();
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
        /// <summary>
        /// 强烈建议在每次Update时第一个执行的输入更新
        /// </summary>
        public static void UpdateInput()
        {
            Resolution_X = Game.GraphicsDevice.Viewport.Width;
            Resolution_Y = Game.GraphicsDevice.Viewport.Height;
            touchCollection = TouchPanel.GetState(Game.Window).GetState();
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
        /// <summary>
        /// 获取Assets文件夹下文件。对于Android，Assets文件夹就是安装包的Assets文件夹
        /// </summary>
        public static Stream GetAsset(string path)
        {
            using (Stream stream = Game.Activity.Assets.Open(path))
            {
                MemoryStream memoryStream = new MemoryStream();
                stream.CopyTo(memoryStream);
                return memoryStream;
            }
        }
    }
}
