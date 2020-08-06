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
            tex = Texture2D.FromStream(graphicsDevice, File.OpenRead(Environment.CurrentDirectory + @"\Extra_194.png"));
            tt = new TestTex(graphicsDevice, 70, 70);
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
                //bullets.Add(new MeteorBullet(new Vector2(960, 540), (Common.MouseState.position - new Vector2(960, 540)).Angle(), 0));
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
        TestTex tt;
        Texture2D tex;
        protected override void Draw(GameTime gameTime)
        {
            Common.UpdateFPS(gameTime);
            GraphicsDevice.Clear(Color.Black);
            u.width = 100;
            u.height = 100;
            u.postion = (Common.Resolution - u.Size) / 2;
            u.Update();
            //Window.Title = u.mouseStatus.ToString() + mousePos[0].ToString();
            /*
            var p = Common.MouseState.position;
            vertices = new Vertex[]
            {
                new Vertex(new Vector2(0 + p.X , 0f + p.Y), Color.White, new Vector2(0.5f, 0f)),
                new Vertex(new Vector2(5f + p.X, 200f + p.Y), Color.White, new Vector2(0.5f, 1f)),
                new Vertex(new Vector2(-5f + p.X, 200f + p.Y), Color.White, new Vector2(0.5f, 1f))
            };
            var z = Common.MouseState.position.Y / Common.Resolution.Y * 1.15f;
            vertexBatch.Begin(swordFx.Texture, PrimitiveType.TriangleList);
            //准备绘制，用到一个材质，顶点的类型是TriangleList
            var v = new VertexTriangle(vertices).Rotate(timer, TriangleVertexType.A).RotationList(TriangleVertexType.A, 3.1415f).
                TransformPosition(Matrix.CreateScale(1f, z, 1f), Common.MouseState.position);
            //声明新的三角形，将三角形绕点A（第一个点）旋转timer rad，从这个三角形生成RotationList，RotationList包含有序的经过旋转的三角形，索引增加三角形旋转的度数也增加，这里最多转3.1415 rad（也就是半圈）
            //对Vertex进行变换，每个顶点x不变，y乘以z，这里z是小于1的
            v = v.Transform(delegate (int index, Vertex vertex)
            {
                return vertex.ChangeColor(vertex.Color * (index * 1f / v.vertex.Length));
            });
            //对Vertex进行变换，索引数值小的透明度低
            vertexBatch.Draw(v);
            //进行绘制
            vertexBatch.End();
            //真正地将它绘制到屏幕上，释放资源
            vertexBatch.Begin(PrimitiveType.LineStrip);
            //u.DrawBorder(vertexBatch);
            vertexBatch.End();
            */
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
            //Draw Texts
            //dtt.DrawString(spriteBatch, "开始游戏\n啥也开始不了", (Common.Resolution - dtt.MeasureString("开始游戏\n啥也开始不了", 1)) / 2, Color.White, default, 1);
            //Draw Bullets
            EntityManager.Draw(bullets, spriteBatch);
            MeteorBullet.flarefx.Draw(spriteBatch, mousePos[0], Color.White, CenterType.MiddleCenter, 1.5f, 1.571f / 2);
            spriteBatch.End();
            //Draw Mouse
            vertexBatch.Begin(tex, PrimitiveType.TriangleStrip);
            var v = Helper.CatmullRom(mousePos, 3);
            if (Keyboard.GetState().IsKeyDown(Keys.Z)) throw new Exception(v.ToStringAlt());
            vertexBatch.Draw(VertexTriangle.TriangleStrip(v, 30, delegate(int index, Vertex vertex)
            {
                return vertex.ChangeCoord(index * 0.25f / v.Length + (float)(Math.Sin(timer) + 1) / 4, index % 2 == 0 ? 0.3f : 0.7f).ChangeColor(Color.White * (-index * 0.5f / v.Length + 1) * (-index * 0.5f / v.Length + 1) * 1.3f);
            }
            ));
            vertexBatch.End(); 
            base.Draw(gameTime);
        }
    }
}
