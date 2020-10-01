using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stellaris.Graphics
{
    public static class Ripple
    {
        public static void DrawRound(VertexBatch vertexBatch, int radius, int width, int height, Color color, Vector2 position, Vector2 center, float xScale = 1f, float quality = 1f)
        {
            if (vertexBatch.primitiveType == PrimitiveType.TriangleList)
            {
                Vector2[] pos = new Vector2[(int)(radius * 0.4f * quality) + 1];
                float theta = 6.283f / (pos.Length - 2);
                float extra = 200 / (float)Math.Sqrt(width + height);
                List<Vector2> cache = new List<Vector2>();
                for (int i = 0; i < pos.Length - 1; i++)
                {
                    Vector2 p = new Vector2(radius, 0).Rotate(theta * i);
                    p.X *= xScale;
                    p += center;
                    if (p.Y < height * (1 + extra) && p.Y > -height * extra && p.X < width * (1 + extra) && p.X > -width * extra)
                    {
                        cache.Add(p);
                    }
                }
                Vector2 po = center;
                if (po.Y > height) po.Y = height;
                else if (po.Y < 0) po.Y = 0;
                if (po.X > width) po.X = width;
                else if (po.X < 0) po.X = 0;
                po += position;
                cache.Add(po);
                pos = cache.ToArray();
                Color[] colors = new Color[pos.Length];
                short[] indices = new short[pos.Length * 3 - 3];
                for (int i = 0; i < pos.Length - 1; i++)
                {
                    if (pos[i].Y > height) pos[i].Y = height;
                    else if (pos[i].Y < 0) pos[i].Y = 0;
                    if (pos[i].X > width) pos[i].X = width;
                    else if (pos[i].X < 0) pos[i].X = 0;
                    pos[i] += position;
                    colors[i] = color;
                    indices[i * 3] = (short)(pos.Length - 1);
                    indices[i * 3 + 1] = (short)i;
                    if (i == pos.Length - 2) indices[i * 3 + 2] = 0;
                    else indices[i * 3 + 2] = (short)(i + 1);
                }
                colors[pos.Length - 1] = color;
                if (indices.Length == 0) return;
                //Ste.game.Window.GetType().GetMethod("set_Title").Invoke(Ste.game.Window, new object[] { pos.Length.ToString() });
                vertexBatch.Draw(new VertexDrawInfo(pos, colors, indices));
            }
        }
        public static void DrawRound(VertexBatch vertexBatch, int radius, Rectangle rectangle, Vector2 center, Color color, float xScale = 1f, float quality = 1f)
        {
            DrawRound(vertexBatch, radius, rectangle.Width, rectangle.Height, color, rectangle.Location.ToVector2(), center, xScale, quality);
        }
    }
}
