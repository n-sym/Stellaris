using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Stellaris.Entities;
using Stellaris.Graphics;
using Stellaris.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

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
            vertexBatch = new VertexBatch(graphicsDevice);
            swordFx = new SwordFx(GraphicsDevice, 1);
            u = new UIBase();
            text = new DynamicTextureTextGDI(graphicsDevice, Environment.CurrentDirectory + Path.DirectorySeparatorChar + "SourceHanSansCN-Regular.ttf", 40, "***ABCD文字绘制测试\nStellaris\n增益免疫汉化组");
            dtt = new DynamicTextureFont(graphicsDevice, Environment.CurrentDirectory + Path.DirectorySeparatorChar + "SourceHanSansCN-Regular.ttf", 80);
            base.Initialize();
        }
        DynamicTextureFont dtt;
        Vertex[] vertices;
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }
        float timer = 0;
        protected override void Update(GameTime gameTime)
        {
            Common.Update(this);
            var p = Common.MouseState.position;
            vertices = new Vertex[]
            {
                new Vertex(new Vector2(0 + p.X , 0f + p.Y), Color.White, new Vector2(0.5f, 0.5f)),
                new Vertex(new Vector2(5f + p.X, 200f + p.Y), Color.White, new Vector2(0.5f, 1f)),
                new Vertex(new Vector2(-5f + p.X, 200f + p.Y), Color.White, new Vector2(0.5f, 1f))
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
            EntityManager.Update(bullets);
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
        DynamicTextureTextGDI text;
        SwordFx swordFx;
        UIBase u;
        protected override void Draw(GameTime gameTime)
        {
            Common.UpdateFPS(gameTime);
            GraphicsDevice.Clear(Color.Black);
            u.width = 100;
            u.height = 100;
            u.postion = (Common.Resolution - u.Size) / 2;
            u.Update();
            Window.Title = u.mouseStatus.ToString() + mousePos[0].ToString();
            var z = Common.MouseState.position.Y / Common.Resolution.Y * 1.15f;
            vertexBatch.Begin(swordFx.Texture, PrimitiveType.TriangleList);
            var v = new VertexTriangle(vertices).Rotate(timer, TriangleVertexType.A).RotationList(TriangleVertexType.A, 3.1415f).
                TransformPosition(Matrix.CreateScale(1f, z, 1f), Common.MouseState.position);
            v = v.Transform(delegate (int index, Vertex vertex)
            {
                return vertex.ChangeColor(vertex.Color * (index * 1.4f / v.vertex.Length));
            });
            vertexBatch.Draw(v);
            vertexBatch.End();
            vertexBatch.Begin(PrimitiveType.LineStrip);
            u.DrawBorder(vertexBatch);
            vertexBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
            //Draw Texts
            dtt.DrawString(spriteBatch, "开始游戏", (Common.Resolution - dtt.MeasureString("Stellaris", 1)) / 2, Color.White, default, 1);
            text.Draw(spriteBatch, new Vector2(0, 0), null, Color.White, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
            //Draw Bullets
            EntityManager.Draw(bullets, spriteBatch);
            //Draw Mouse
            for (int k = 0; k < 1; k++)
            {
                for (int i = 0; i < 15; i++)
                {
                    if (mousePos[i] != Vector2.Zero)
                    {
                        float m = (mousePos[i] - mousePos.TryGetValue(i + 1)).Length() * 0.3f;
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
