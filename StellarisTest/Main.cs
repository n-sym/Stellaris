using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Stellaris.Curves;
using Stellaris.Entities;
using Stellaris.Graphics;
using Stellaris.UI;
using System;
using System.Collections.Generic;
using System.IO;
using StbTrueTypeSharp;
using System.Runtime.InteropServices;

namespace Stellaris.Test
{
    public unsafe class Main : Game
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
            using (FileStream fileStream = File.OpenRead(@"C:\Windows\Fonts\arial.ttf"))
            {
                var a = ToByteArray(fileStream);
                byte* aa = (byte*)GCHandle.Alloc(a, GCHandleType.Pinned).AddrOfPinnedObject();
                x = new StbTrueType.stbtt_fontinfo();
                StbTrueType.stbtt_InitFont(x, aa, 0);
                a = new byte[100];
                int x1, x2, y1, y2;
                StbTrueType.stbtt_GetGlyphBitmapBox(x, 55, 0.007550335f, 0.007550335f, &x1, &y1, &x2, &y2);
                RenderGlyphBitmap(a, 9, 10, 9, 55);
                bool flag = a[10] == 0;
                sth = y2.ToString();
            }
            text = new DynamicTextureTextGDI(graphicsDevice, new System.Drawing.Font("华文中宋", 25), "Stellaris" + sth);
            base.Initialize();
        }
        string sth;
        Vertex[] vertices;
        StbTrueType.stbtt_fontinfo x; 
        public byte[] ToByteArray(Stream stream)
        {
            byte[] bytes;

            // Rewind stream if it is at end
            if (stream.CanSeek && stream.Length == stream.Position)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }

            // Copy it's data to memory
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                bytes = ms.ToArray();
            }

            return bytes;
        }
        public unsafe void RenderGlyphBitmap(byte[] output, int outWidth, int outHeight, int outStride, int glyph)
        {
            fixed (byte* outputPtr = output)
            {
                StbTrueType.stbtt_MakeGlyphBitmap(x , outputPtr, outWidth, outHeight, outStride, 1, 1, glyph);
            }
        }
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
            vertices = new Vertex[]
            {
                new Vertex(new Vector2(0 + p.X , 0f + p.Y), Color.White, new Vector2(0.5f, 0f)),
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
        DynamicTextureTextGDI text;
        SwordFx swordFx;
        UIBase u;
        protected override void Draw(GameTime gameTime)
        {
            Common.UpdateFPS(gameTime);
            GraphicsDevice.Clear(Color.Black);
            u.width = 100;
            u.height = 100;
            u.postion = new Vector2(100, 100);
            u.Update();
            Window.Title = u.mouseStatus.ToString();
            var z = Common.MouseState.position.Y / Common.Resolution.Y * 1.15f;
            vertexBatch.Begin(swordFx.Texture, PrimitiveType.TriangleList);
            var v = new Triangle(vertices).Rotate(timer, TriangleVertexType.A).RotationList(TriangleVertexType.A, 3.1415f).
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
            text.Draw(spriteBatch, new Vector2(100, 100), null, Color.White, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
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
