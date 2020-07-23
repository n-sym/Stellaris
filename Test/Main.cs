using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Stellaris.Curves;
using Stellaris.Entities;
using Stellaris.Graphics;
using Stellaris.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Stellaris.Test
{
    public class Main : Game
    {
        private GraphicsDeviceManager graphics;
        private GraphicsDevice graphicsDevice => graphics.GraphicsDevice;
        private SpriteBatch spriteBatch;
        private DynamicTexture a;
        private List<Bullet> bullets;
        public static Main instance;

        public Main()
        {
            instance = this;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
        }
        protected override void Initialize()
        {
            Common.Initialize(this);
            Window.AllowUserResizing = true;
            bullets = new List<Bullet>();
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            graphics.ApplyChanges();
            a = new FlareFxAlt(graphicsDevice, 40, 40, "Test2");
            MeteorBullet.flarefx = new FlareFx(graphicsDevice, 40, 40, "Test");
            MeteorBullet.flarefxAlt = a as FlareFxAlt;
            mousePos = new Vector2[15];
            text = new DynamicTextureText(graphicsDevice, new System.Drawing.Font("华文中宋", 100), "Stellaris");
            vertexBatch = new VertexBatch(graphicsDevice);
            base.Initialize();
            
        }
        VertexInfo[] vertices;
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }
        Ellipse ellipse = new Ellipse(200, 200);
        float timer = 0;
        protected override void Update(GameTime gameTime)
        {
            Common.Update(this);
            var p = Common.MouseState.position;
            vertices = new VertexInfo[]
            {
                new VertexInfo(new Vector2(0 + p.X , 71f - p.Y), Color.Red),
                new VertexInfo(new Vector2(100 + p.X, -100f - p.Y), Color.Blue),
                new VertexInfo(new Vector2(-100f + p.X, -100f - p.Y), Color.Green),
                new VertexInfo(new Vector2(200f + p.X, 71f - p.Y), Color.Purple)
            };
            timer += 0.15f;
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                for (int i = 54; i > 0; i--)
                {
                    bullets.Add(new MeteorBullet(new Vector2(960, 540), 0, 0));
                }
            }
            if (Common.MouseState.left == ButtonState.Pressed)
            {
                bullets.Add(new MeteorBullet(new Vector2(960, 540), (Common.MouseState.position - new Vector2(960, 540)).Angle(), 0));
            }
            if (Keyboard.GetState().IsKeyDown(Keys.B))
            {
                timer = 0;
            }
            for (int i = 0; i < bullets.Count; i++)
            {
                if (bullets[i] == null) break;
                if (!bullets[i].active)
                {
                    bullets.RemoveAt(i);
                    continue;
                }
                bullets[i].Update();
            }
            for (int i = 14; i > 0; i--)
            {
                mousePos[i] = mousePos[i - 1];
            }
            mousePos[0] = Common.MouseState.position;
            //mousePos[0] = Mouse.GetState().Position.ToVector2() + ellipse.Get(timer);
            //mousePos[0] = ellipse.Get(timer) + new Vector2(960, 540);
            //mousePos[0] = new Vector2(timer * 200, timer * timer * 10);
            base.Update(gameTime);
        }
        Vector2[] mousePos;
        VertexBatch vertexBatch;
        DynamicTextureText text;
        protected override void Draw(GameTime gameTime)
        {
            Common.UpdateFPS(gameTime);
            //Window.Title = Common.Quality.ToString() + "," + Common.FPS.ToString();
            GraphicsDevice.Clear(Color.Black);
            vertexBatch.Begin();
            vertexBatch.Draw(vertices[0], vertices[1], vertices[2], vertices[3], vertices[1], vertices[0]);
            vertexBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
            text.Draw(spriteBatch, new Vector2(100, 100), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            for (int i = 0; i < bullets.Count; i++)
            {
                if (bullets[i] == null) break;
                bullets[i].Draw(spriteBatch);
            }
            for (int k = 0; k < 1; k++)
            {
                for (int i = 0; i < 15; i++)
                {
                    if (mousePos[i] != Vector2.Zero)
                    {
                        float m = (mousePos[i] - mousePos.TryGetValue(i + 1)).Length() * 0.3f;
                        /*try
                        {
                            Vector2[] v = Helper.LagrangeInterpolation(mousePos.CutOut(i - 1, i + 1), (int)(m));
                            for (int j = 0; j < v.Length; j++)
                            {
                                a.Draw(spriteBatch, v[j] + k * new Vector2(10, 10), null, Color.White * (1.5f - (float)(Math.Sqrt(i) / Math.Sqrt(14))), 0.7853f, new Vector2(20, 20), 1.3f * (1 - (float)(Math.Sqrt(i) / Math.Sqrt(14))), SpriteEffects.None, 0f);
                            }
                        }
                        catch
                        {

                        }*/
                        for (int j = 0; j < m; j++)
                        {
                            a.Draw(spriteBatch, mousePos[i].LinearInterpolationTo(mousePos.TryGetValue(i + 1), j, m) + k * new Vector2(10, 10), null, Color.White * (1.1f - (float)(Math.Sqrt(i) / Math.Sqrt(14))), 0.7853f, new Vector2(20, 20), 0.9f * (1 - (float)(Math.Sqrt(i) / Math.Sqrt(14))), SpriteEffects.None, 0f);

                        }
                    }
                }
                MeteorBullet.flarefx.Draw(spriteBatch, mousePos[0], null, Color.White * 0.9f, 0.7853f, new Vector2(20, 20), 1.4f, SpriteEffects.None, 0f);
                spriteBatch.End();
                base.Draw(gameTime);
            }
        }
    }
}
