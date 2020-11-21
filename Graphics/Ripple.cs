using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Stellaris.Graphics
{
    public static class Ripple
    {
        private static void IfBiggerThenSet(ref float value1, float value2)
        {
            if (value2 >= value1) value1 = value2;
        }
        private static void IfSmallerThenSet(ref float value1, float value2)
        {
            if (value2 <= value1) value1 = value2;
        }
        private static void CheckAndApplyCorner(ref Vector2 v, int w, int h, int r)
        {
            if (v.X < r)
            {
                if (v.Y < r) IfBiggerThenSet(ref v.X, r - (float)Math.Cos(Math.Asin(1 - v.Y / r)) * r);
                else if (v.Y > h - r) IfBiggerThenSet(ref v.X, r - (float)Math.Cos(Math.Asin(1 + (v.Y - h) / r)) * r);
            }
            else if (v.X > w - r)
            {
                if (v.Y < r) IfSmallerThenSet(ref v.X, w - r + (float)Math.Cos(Math.Asin(1 - v.Y / r)) * r);
                else if (v.Y > h - r) IfSmallerThenSet(ref v.X, w - r + (float)Math.Cos(Math.Asin(1 + (v.Y - h) / r)) * r);
            }
        }
        public static void DrawRound(VertexBatch vertexBatch, int radius, Vector2 position, Vector2 center, int width, int height, Color color, int roundCorner = 0, float xScale = 1f, float quality = 1f)
        {
            if (vertexBatch.primitiveType == PrimitiveType.TriangleList)
            {
                if (center.Y > height) return;
                else if (center.Y < 0) return;
                if (center.X > width) return;
                else if (center.X < 0) return;
                if (roundCorner > height / 3)
                {
                    if (center.X < roundCorner * 2 / 3) center.X = roundCorner * 2 / 3;
                    else if (center.X > width - roundCorner * 2 / 3) center.X = width - roundCorner * 2 / 3;
                }
                Vector2[] pos = new Vector2[(int)(radius * 0.5f * quality) + 1];
                float theta = 6.283f / (pos.Length - 2);
                float extra = radius * 1f / width;
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
                if (roundCorner != 0) CheckAndApplyCorner(ref po, width, height, roundCorner);
                po += position;
                cache.Add(po);
                pos = cache.ToArray();
                Color[] colors = new Color[pos.Length];
                int[] indices = new int[pos.Length * 3 - 3];
                for (int i = 0; i < pos.Length - 1; i++)
                {
                    if (pos[i].Y > height) pos[i].Y = height;
                    else if (pos[i].Y < 0) pos[i].Y = 0;
                    if (pos[i].X > width) pos[i].X = width;
                    else if (pos[i].X < 0) pos[i].X = 0;
                    if (roundCorner != 0) CheckAndApplyCorner(ref pos[i], width, height, roundCorner);
                    pos[i] += position;
                    colors[i] = color;
                    indices[i * 3] = (int)(pos.Length - 1);
                    indices[i * 3 + 1] = (int)i;
                    if (i == pos.Length - 2) indices[i * 3 + 2] = 0;
                    else indices[i * 3 + 2] = (int)(i + 1);
                }
                colors[pos.Length - 1] = color;
                if (indices.Length == 0) return;
                //Ste.game.Window.GetType().GetMethod("set_Title").Invoke(Ste.game.Window, new object[] { pos.Length.ToString() });
                vertexBatch.Draw(new VertexDrawInfo(pos, colors, indices));
            }
        }
        public static void DrawRound(VertexBatch vertexBatch, int radius, Rectangle rectangle, Vector2 center, Color color, int roundCorner = 0, float xScale = 1f, float quality = 1f)
        {
            DrawRound(vertexBatch, radius, rectangle.Location.ToVector2(), center, rectangle.Width, rectangle.Height, color, roundCorner, xScale, quality);
        }
    }
}
