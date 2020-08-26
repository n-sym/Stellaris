﻿using Microsoft.Xna.Framework;
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
            //text = new DynamicTextureTextGDI(graphicsDevice, Environment.CurrentDirectory + Path.DirectorySeparatorChar + "SourceHanSansCN-Regular.ttf", 40, "***ABCD文字绘制测试\nStellaris\n增益免疫汉化组");
            dtt = new DynamicSpriteFont(graphicsDevice, Common.GetAsset("SourceHanSansCN-Regular.ttf"), 80);
            tex3 = Texture2D.FromStream(graphicsDevice, Common.GetAsset("trail1.png"));
            tex = Texture2D.FromStream(graphicsDevice, Common.GetAsset("aaa28.png"));
            tex2 = Texture2D.FromStream(graphicsDevice, Common.GetAsset("trail2.png"));
            tt = new TestTex(graphicsDevice, 70, 70);
            base.Initialize();
        }
        DynamicSpriteFont dtt;
        Vertex[] vertices;
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }
        float timer = 0;
        int timer2 = 0;
        protected override void Update(GameTime gameTime)
        {
            Common.Update();
            timer += 0.15f;
            if (timer2 > 0) timer2--;
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                if (timer2 == 0) timer2 += 15;
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
            //mousePos[0] = new Vector2((float)Math.Cos(timer * 2.5f), (float)Math.Sin(timer * 2.5f)) * 200 + new Vector2(960, 540);
            //mousePos[0] = new Vector2(timer * 200, timer * timer * 10);
            base.Update(gameTime);
        }
        Vector2[] mousePos;
        VertexBatch vertexBatch;
        SwordFx swordFx;
        UIBase u;
        TestTex tt;
        Texture2D tex;
        Texture2D tex2;
        Texture2D tex3;
        protected override void Draw(GameTime gameTime)
        {
            Common.UpdateFPS(gameTime);
            GraphicsDevice.Clear(Color.Black);
            u.width = 100;
            u.height = 100;
            u.position = (Common.Resolution - u.Size) / 2;
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
            var vv = new VertexTriangle(vertices).Rotate(timer, TriangleVertexType.A).RotationList(TriangleVertexType.A, 3.1415f).
                TransformPosition(Matrix.CreateScale(1f, z, 1f), Common.MouseState.position);
            //声明新的三角形，将三角形绕点A（第一个点）旋转timer rad，从这个三角形生成RotationList，RotationList包含有序的经过旋转的三角形，索引增加三角形旋转的度数也增加，这里最多转3.1415 rad（也就是半圈）
            //对Vertex进行变换，每个顶点x不变，y乘以z，这里z是小于1的
            vv = vv.Transform(delegate (int index, Vertex vertex)
            {
                return vertex.ChangeColor(vertex.Color * (index * 1f / vv.vertex.Length));
            });
            //对Vertex进行变换，索引数值小的透明度低
            vertexBatch.Draw(vv);
            //进行绘制
            vertexBatch.End();
            //真正地将它绘制到屏幕上，释放资源
            vertexBatch.Begin(PrimitiveType.LineStrip);
            //u.DrawBorder(vertexBatch);
            vertexBatch.End();
            */
            //Draw Texts
            //dtt.DrawString(spriteBatch, "开始游戏\n啥也开始不了", (Common.Resolution - dtt.MeasureString("开始游戏\n啥也开始不了", 1)) / 2, Color.White, default, 1);
            //Draw Bullets
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
            dtt.DrawString(spriteBatch, "Stellaris测试", new Vector2(50, 50), Color.White, Vector2.Zero, 1.2f);
            dtt.DrawString(spriteBatch, "Stellaris测试", new Vector2(50, 50) + dtt.MeasureString("Stellaris测试", 1.2f).Y_Vector());
            vertexBatch.Begin(PrimitiveType.LineStrip);
            UIBase.DrawBorder(vertexBatch, new Vector2(50, 50), dtt.MeasureString("Stellaris测试", 1.2f));//vertexBatch.Begin(PrimitiveType.LineStrip);
            vertexBatch.End();
            //UIBase.DrawBorder(vertexBatch, new Vector2(50, 50), dtt.MeasureString("Stellaris测试", 1) + new Vector2(50, 50));
            //vertexBatch.End();
            EntityManager.Draw(bullets, spriteBatch);
            if (mousePos[0] != mousePos[1])
            {
                float t = (mousePos[0] - mousePos[1]).Angle() + 0.7853f;
                Color cc = Color.White.LinearTo(MeteorBullet.drawColor, 1, 2);
                MeteorBullet.flarefx.Draw(spriteBatch, mousePos[0], cc, CenterType.MiddleCenter, 2.2f, t);
                MeteorBullet.flarefxAlt.Draw(spriteBatch, mousePos[0], cc * 0.55f, CenterType.MiddleCenter, 5f, t);
                MeteorBullet.flarefxAlt.Draw(spriteBatch, (mousePos[0] + mousePos[1]) / 2, cc * 0.55f, CenterType.MiddleCenter, 5f, t);
                MeteorBullet.flarefxAlt.Draw(spriteBatch, mousePos[1], cc * 0.55f, CenterType.MiddleCenter, 5f, t);
                MeteorBullet.flarefxAlt.Draw(spriteBatch, mousePos[2], cc * 0.55f, CenterType.MiddleCenter, 5f, t);
                MeteorBullet.flarefxAlt.Draw(spriteBatch, mousePos[4], cc * 0.55f, CenterType.MiddleCenter, 5f, t);
                MeteorBullet.flarefxAlt.Draw(spriteBatch, mousePos[6], cc * 0.55f, CenterType.MiddleCenter, 5f, t);
            }
            else MeteorBullet.flarefx.Draw(spriteBatch, mousePos[0], Color.White, CenterType.MiddleCenter, 1.5f, 0.7853f);
            //Draw Mouse
            /*foreach (var vvv in v)
            {
                MeteorBullet.flarefx.Draw(spriteBatch, vvv, Color.White, CenterType.MiddleCenter);
            }*/
            /*var vi = VertexTriangle.StripOnSide(v, 100, delegate (int index, Vertex vertex)
            {
                return vertex.ChangeCoord(index % 2 == 0 ? 0.25f : 0.75f, (float)(Math.Sin(timer) + Math.Sin(index * 8f / v.Length) + 2) / 4).ChangeColor(MeteorBullet.drawColor.LinearTo(MeteorBullet.drawColor2, index, v.Length * 0.8f) * (-index * 0.5f / v.Length + 1) * (-index * 0.5f / v.Length + 1));
            }
            );*/
            /*foreach (var vvv in vi.vertex)
            {
                MeteorBullet.flarefx.Draw(spriteBatch, vvv.Position, Color.White, CenterType.MiddleCenter);
            }*/
            spriteBatch.End();
            /*vertexBatch.Begin(tex3, BlendState.Additive, PrimitiveType.TriangleStrip);
            vertexBatch.Draw(vi);
            vertexBatch.End();*/
            var timer3 = timer2;
            var angle = 3.4f * (1 - timer3 / 15f) + 0.02f;
            var begin = 1 - timer3 / 4f;
            vertexBatch.Begin(tex, BlendState.Additive, PrimitiveType.TriangleStrip);
            //196 94 61
            var c = Helper.HslToRgb(196, 94, 61);
            var v = Helper.GetEllipse(200, 200, begin, angle + begin, -0.01f, Common.Resolution / 2);
            var vi = VertexTriangle.StripOneSide(v, -150, delegate (int index, Vertex vertex)
            {
                return vertex.ChangeCoord(index % 2 == 0 ? 1f : 0f, index * -0.5f / v.Length + 1f).ChangeColor(c);
            }
            );
            //vertexBatch.Draw(vi); 
            v = Helper.GetEllipse(190, 190, begin, angle + begin, -0.01f, Common.Resolution / 2);
            vi = VertexTriangle.StripOneSide(v, -135, delegate (int index, Vertex vertex)
            {
                return vertex.ChangeCoord(index % 2 == 0 ? 1f : 0f, (index * -0.5f / v.Length + 0.9f) * 1.15f).ChangeColor(c);
            }
            );
            //vertexBatch.Draw(vi);
            v = Helper.GetEllipse(180, 180, begin, angle + begin, -0.01f, Common.Resolution / 2);
            vi = VertexTriangle.StripOneSide(v, -120, delegate (int index, Vertex vertex)
            {
                return vertex.ChangeCoord(index % 2 == 0 ? 1f : 0f, (index * -0.5f / v.Length + 0.8f) * 1.3f).ChangeColor(Color.White);
            }
            );
            vertexBatch.Draw(vi);
            vertexBatch.End();
            v = Helper.CatmullRom(mousePos, 4);
            vi = VertexTriangle.Strip(v, 30, delegate (int index, Vertex vertex)
            {
                return vertex.ChangeCoord(index % 2 == 0 ? 0.3f : 0.7f, 1f - (float)(Math.Sin(timer) + Math.Sin(index * 8f / v.Length) + 2) / 4).ChangeColor(MeteorBullet.drawColor.LinearTo(MeteorBullet.drawColor2, index, v.Length * 0.8f) * (-index * 0.5f / v.Length + 1) * (-index * 0.5f / v.Length + 1) * 0.9f);
            }
            );
            vertexBatch.Begin(tex2, BlendState.Additive, PrimitiveType.TriangleStrip);
            vertexBatch.Draw(vi);
            vertexBatch.ChangeTexture(tex3);
            vertexBatch.Draw(vi);
            vertexBatch.End();
            base.Draw(gameTime);
        }
    }
}
