using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Stellaris.Entities;
using Stellaris.Graphics;
using Stellaris.UI;
using System;
using System.Collections.Generic;

namespace Stellaris.Test
{
    public class Main : Game
    {
        private GraphicsDeviceManager graphics;
        private GraphicsDevice graphicsDevice => graphics.GraphicsDevice;
        private SpriteBatchS spriteBatch;
        private DynamicTexture a;
        private List<Bullet> bullets;
        public static Main instance;

        public Main()
        {
            instance = this;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }
        protected override void Initialize()
        {
            Ste.Initialize(this, graphics);
            Window.AllowUserResizing = true;
            bullets = new List<Bullet>();
            Ste.ChangeResolution(1920, 1080);
            a = new FlareFxAlt(graphicsDevice, 40, 40, "Test2");
            MeteorBullet.flarefx = new FlareFx(graphicsDevice, 40, 40, "Test");
            MeteorBullet.flarefxAlt = a as FlareFxAlt;
            mousePos = new Vector2[15];
            vertexBatch = new VertexBatch(graphicsDevice);
            //var n = new FontStb_Native(Ste.GetAsset("SourceHanSansCN-Regular.ttf"), graphicsDevice);
            //text = new DynamicTextureTextGDI(graphicsDevice, Environment.CurrentDirectory + Path.DirectorySeparatorChar + "SourceHanSansCN-Regular.ttf", 40, "***ABCD文字绘制测试\nStellaris\n增益免疫汉化组");
            dtt = new DynamicSpriteFont(graphicsDevice, Ste.GetAsset("SourceHanSansCN-Regular.ttf"), 80, useNative: false);
            var font = new FontStb_Native(Ste.GetAsset("Product-Sans-Regular.ttf"));
            font.ReverseFont = dtt.font;
            dtt2 = new DynamicSpriteFont(graphicsDevice, font, 80);
            tex3 = Texture2D.FromStream(graphicsDevice, Ste.GetAsset("trail3.png"));
            tex = Texture2D.FromStream(graphicsDevice, Ste.GetAsset("zzzz.png"));
            tex2 = Texture2D.FromStream(graphicsDevice, Ste.GetAsset("trail3.png"));
            z = new Effect(graphicsDevice, Ste.GetAsset("Blur.cfx").ToByteArray());
            base.Initialize();
        }
        DynamicSpriteFont dtt;
        DynamicSpriteFont dtt2;
        Vertex[] vertices;
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatchS(GraphicsDevice);
        }
        float timer = 0;
        int timer2 = 0;
        protected override void Update(GameTime gameTime)
        {
            Ste.UpdateInput();
            timer += 0.15f;
            if (timer2 > 0) timer2--;
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                if (timer2 == 0) timer2 += 15;
            }
            if (Ste.MouseState.Left == ButtonState.Pressed)
            {
                bullets.Add(new MeteorBullet(new Vector2(960, 540), (Ste.MouseState.Position - new Vector2(960, 540)).Angle(), 0));
            }
            if (Keyboard.GetState().IsKeyDown(Keys.B))
            {
                timer = 0;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Space) && !lastSpace)
            {
                refresh = !refresh;
            }
            lastSpace = Keyboard.GetState().IsKeyDown(Keys.Space);
            EntityManager.Update(bullets);
            for (int i = 14; i > 0; i--)
            {
                mousePos[i] = mousePos[i - 1];
            }
            mousePos[0] = Ste.MouseState.Position;
            //mousePos[0] = Mouse.GetState().Position.ToVector2() + ellipse.Get(timer);
            //mousePos[0] = new Vector2((float)Math.Cos(timer * 2.5f), (float)Math.Sin(timer * 2.5f)) * 200 + new Vector2(960, 540);
            //mousePos[0] = new Vector2(timer * 200, timer * timer * 10);
            base.Update(gameTime);
        }
        Effect z;
        Vector2[] mousePos;
        VertexBatch vertexBatch;
        BaseUIElement u;
        Texture2D tex;
        Texture2D tex2;
        Texture2D tex3;
        bool refresh = false;
        bool lastSpace = false;
        float timerz;
        MDButton button = new MDButton(new Vector2(100, 100), 500, 150, 75, Color.White, default, Color.Black * 0.4f, Color.Black, Color.Black * 0.7f);
        float zDis;
        VertexDrawInfo sphere;
        VertexDrawInfo cache;
        Vector2 angle = default;
        Vector2 deltaAngle = default;
        protected unsafe override void Draw(GameTime gameTime)
        {
            Ste.UpdateFPS(gameTime);
            GraphicsDevice.Clear(Color.CornflowerBlue);
            if (Keyboard.GetState().IsKeyDown(Keys.A)) zDis += 1;
            vertexBatch.Begin(PrimitiveType.TriangleStrip);
            deltaAngle *= 0.9f;
            if (deltaAngle.Length() < 0.01f) deltaAngle = Vector2.Zero;
            if (Ste.MouseState.LeftDowned)
            {
                if (Ste.MousePos.X != Ste.LastMouseState.X) deltaAngle.Y = -(Ste.MousePos.X - Ste.LastMouseState.X) / Ste.Resolution_X * 4;
                if (Ste.MousePos.Y != Ste.LastMouseState.Y) deltaAngle.X = (Ste.MousePos.Y - Ste.LastMouseState.Y) / Ste.Resolution_Y * 4;
            }
            angle += deltaAngle;
            if (sphere == null)
            {
                var vertex = new Vertex[10001];
                for (int y = 0; y < 100; y++)
                {
                    for (int x = 0; x < 100; x++)
                    {
                        float p = y / 100f * 3.14f;
                        //纬度
                        float cp = (float)Math.Cos(p);
                        float sp = (float)Math.Sin(p);
                        float t = x / 100f * 6.28f;
                        //经度
                        float ct = (float)Math.Cos(t);
                        float st = (float)Math.Sin(t);
                        vertex[100 * y + x] = new Vertex(new Vector3(sp * ct * 200, cp * 200, sp * st * 123 + zDis));
                    }
                }
                vertex[10000] = new Vertex(new Vector3(0, -200, zDis));
                var indice = new int[20000];
                for (int i = 0; i < 10000; i++)
                {
                    indice[i * 2] = i;
                    if (i + 100 < 10000) indice[i * 2 + 1] = i + 100;
                    else indice[i * 2 + 1] = 10000;
                }
                sphere = new VertexDrawInfo(vertex, indice);
                cache = new VertexDrawInfo(new Vertex[10001], indice);
            }
            var m = Matrix.CreateRotationY(angle.Y) * Matrix.CreateRotationX(angle.X);
            for (int i = 0; i < 10001; i++)
            {
                var v = sphere.vertices[i].Position;
                v.Z += zDis;
                v = Vector3.Transform(v, m);
                double z = 0;
                if (v.Z < 0) z = -(float)Math.Sqrt(-v.Z);
                else z = (float)Math.Sqrt(v.Z);
                float zz = 200 + zDis;
                float l = (v - new Vector3(0, 0, -zz)).Length();
                cache.vertices[i] = new Vertex(Helper.Vector3(v.X / (z + zz) * 200, v.Y / (z + zz) * 200, 0) + new Vector3(Ste.Resolution / 2, 0),
                    Color.White.LerpTo(Color.Gray, (float)Math.Sqrt(l), 17));
            }
            vertexBatch.Draw(cache);
            vertexBatch.End();
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
            /*RenderTarget2D renderTarget2D = new RenderTarget2D(graphicsDevice, Ste.Resolution_X, Ste.Resolution_Y);
            graphicsDevice.SetRenderTarget(renderTarget2D);
            GraphicsDevice.Clear(Color.CornflowerBlue);
            vertexBatch.Begin(PrimitiveType.TriangleList);
            vertexBatch.SetDrawImmediately(false);
            spriteBatch.Begin();
            var b = vertexBatch;
            var text = "var rotation = (Ste.MouseState.Position - Ste.Resolution / 2).Angle()";
            var rotation = (Ste.MouseState.Position - Ste.Resolution / 2).Angle();
            if (refresh)
            {
                dtt.ClearCache();
                dtt2.ClearCache();
            }
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            dtt.Cache(text.ToCharArray());
            stopwatch.Stop();
            var timer1 = stopwatch.ElapsedMilliseconds;
            stopwatch.Reset();
            stopwatch.Start();
            dtt2.Cache(text.ToCharArray());
            stopwatch.Stop();
            var timer2 = stopwatch.ElapsedMilliseconds;
            Window.Title = string.Format("C#:{0}, C++:{1}, C++ / C#:{2}, 不缓存字体:{3}, 空格键切换", timer1, timer2, timer2 * 1f / timer1, refresh);
            //UIBase.DrawBorder(vertexBatch, new Vector2(50, 50), dtt.MeasureString("Stellaris测试", 1) + new Vector2(50, 50));*/
            //vertexBatch.End();
            //EntityManager.Draw(bullets, spriteBatch);
            //vertexBatch.End();
            //spriteBatch.End();
            /*
            if (mousePos[0] != mousePos[1])
            {
                float t = (mousePos[0] - mousePos[1]).Angle() + 0.7853f;
                Color cc = Color.White.LerpTo(MeteorBullet.drawColor, 1, 2);
                MeteorBullet.flarefx.Draw(spriteBatch, mousePos[0], cc, CenterType.MiddleCenter, 2.2f, t);
                MeteorBullet.flarefxAlt.Draw(spriteBatch, mousePos[0], cc * 0.55f, CenterType.MiddleCenter, 5f, t);
                MeteorBullet.flarefxAlt.Draw(spriteBatch, (mousePos[0] + mousePos[1]) / 2, cc * 0.55f, CenterType.MiddleCenter, 5f, t);
                MeteorBullet.flarefxAlt.Draw(spriteBatch, mousePos[1], cc * 0.55f, CenterType.MiddleCenter, 5f, t);
                MeteorBullet.flarefxAlt.Draw(spriteBatch, mousePos[2], cc * 0.55f, CenterType.MiddleCenter, 5f, t);
                MeteorBullet.flarefxAlt.Draw(spriteBatch, mousePos[4], cc * 0.55f, CenterType.MiddleCenter, 5f, t);
                MeteorBullet.flarefxAlt.Draw(spriteBatch, mousePos[6], cc * 0.55f, CenterType.MiddleCenter, 5f, t);
            }
            else MeteorBullet.flarefx.Draw(spriteBatch, mousePos[0], Color.White, CenterType.MiddleCenter, 1.5f, 0.7853f);
            spriteBatch.End();*/
            if (Ste.MouseState.LeftDowned)
            {
                timerz = 1;
            }
            timerz *= 0.95f;
            /*
            z.Parameters["tex"].SetValue(renderTarget2D);
            z.Parameters["resolution"].SetValue(Ste.Resolution);
            z.Parameters["delta"].SetValue(Ste.MousePos.Y / Ste.Resolution_Y * 4);
            graphicsDevice.SetRenderTarget(null);
            z.Techniques[0].Passes[0].Apply();
            spriteBatch.Draw(renderTarget2D, Vector2.Zero, Color.White);
            z.Techniques[0].Passes[0].Apply();
            spriteBatch.Draw(tex, Vector2.Zero, Color.White);
            spriteBatch.Draw(renderTarget2D, Vector2.Zero, Color.White);
            //spriteBatch.Draw(renderTarget2D, new Vector2(0, 150), Color.White);
            spriteBatch.End();
            renderTarget2D.Dispose();*/
            //Draw Mouse
            /*foreach (var vvv in v)
            {
                MeteorBullet.flarefx.Draw(spriteBatch, vvv, Color.White, CenterType.MiddleCenter);
            }*/
            /*var vi = VertexTriangle.StripOnSide(v, 100, delegate (int index, Vertex vertex)
            {
                return vertex.ChangeCoord(index % 2 == 0 ? 0.25f : 0.75f, (float)(Math.Sin(timer) + Math.Sin(index * 8f / v.Length) + 2) / 4).ChangeColor(MeteorBullet.drawColor.LerpTo(MeteorBullet.drawColor2, index, v.Length * 0.8f) * (-index * 0.5f / v.Length + 1) * (-index * 0.5f / v.Length + 1));
            }
            );*/
            /*foreach (var vvv in vi.vertex)
            {
                MeteorBullet.flarefx.Draw(spriteBatch, vvv.Position, Color.White, CenterType.MiddleCenter);
            }*/
            /*vertexBatch.Begin(tex3, BlendState.Additive, PrimitiveType.TriangleStrip);
            vertexBatch.Draw(vi);
            vertexBatch.End();*/
            /*var timer3 = timer2;
            var angle = 3.4f * (1 - timer3 / 15f) + 0.02f;
            var begin = 1 - timer3 / 4f;
            vertexBatch.Begin(tex, BlendState.Additive, PrimitiveType.TriangleStrip);
            //196 94 61
            var c = Helper.HslToRgb(196, 94, 61);
            var v = Helper.GetEllipse(200, 200, begin, angle + begin, -0.01f, Ste.Resolution / 2);
            var vi = VertexStrip.StripOneSide(v, -150, delegate (int index, Vertex vertex)
            {
                return vertex.ChangeCoord(index % 2 == 0 ? 1f : 0f, index * -0.5f / v.Length + 1f).ChangeColor(c);
            }
            );
            //vertexBatch.Draw(vi); 
            v = Helper.GetEllipse(190, 190, begin, angle + begin, -0.01f, Ste.Resolution / 2);
            vi = VertexStrip.StripOneSide(v, -135, delegate (int index, Vertex vertex)
            {
                return vertex.ChangeCoord(index % 2 == 0 ? 1f : 0f, (index * -0.5f / v.Length + 0.9f) * 1.15f).ChangeColor(c);
            }
            );
            //vertexBatch.Draw(vi);
            v = Helper.GetEllipse(180, 180, begin, angle + begin, -0.01f, Ste.Resolution / 2);
            vi = VertexStrip.StripOneSide(v, -120, delegate (int index, Vertex vertex)
            {
                return vertex.ChangeCoord(index % 2 == 0 ? 1f : 0f, (index * -0.5f / v.Length + 0.8f) * 1.3f).ChangeColor(Color.White);
            }
            );
            //vertexBatch.Draw(vi);
            vertexBatch.End();
            v = Helper.CatmullRom(mousePos, 4);
            vi = VertexStrip.Strip(v, 30, delegate (int index, Vertex vertex)
            {
                return vertex.ChangeCoord(index % 2 == 0 ? 0.3f : 0.7f, 1f - (float)(Math.Sin(timer) + index * 4f / v.Length + 1) / 4).ChangeColor(MeteorBullet.drawColor.LerpTo(MeteorBullet.drawColor2, index, v.Length * 0.8f) * (-index * 0.5f / v.Length + 1) * (-index * 0.5f / v.Length + 1) * 0.9f);
            }
            );
            vertexBatch.Begin(tex2, BlendState.Additive, PrimitiveType.TriangleStrip);
            vertexBatch.Draw(vi);
            vertexBatch.ChangeTexture(tex3);
            vertexBatch.Draw(vi);
            vertexBatch.End();*/
            base.Draw(gameTime);
        }
    }
}