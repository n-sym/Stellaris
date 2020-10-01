using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stellaris.Graphics
{
    public static class Border
    {
        public static void DrawRoundedCornerBorder(VertexBatch vertexBatch, Vector2 positon, int width, int height, int roundCorner, Color padding, float quality = 1f)
        {
            if (vertexBatch.primitiveType != PrimitiveType.TriangleList) return;
            int length = (int)(roundCorner * 0.5f * quality);
            float theta = 1.571f / (length - 1);
            Vector2 rVecH = new Vector2(roundCorner, 0);
            Vector2 rVecV = new Vector2(0, roundCorner);
            List<Vertex> cache = new List<Vertex>();
            for (int i = 0; i < length; i++)
            {
                cache.Add(new Vertex(positon + new Vector2(width - roundCorner, roundCorner) + rVecH.Rotate(-i * theta), padding));
            }
            cache.Add(new Vertex(positon + new Vector2(width - roundCorner, 0), padding));
            cache.Add(new Vertex(positon + new Vector2(roundCorner, 0), padding));
            for (int i = 0; i < length; i++)
            {
                cache.Add(new Vertex(positon + new Vector2(roundCorner, roundCorner) - rVecV.Rotate(-i * theta), padding));
            }
            cache.Add(new Vertex(positon + new Vector2(0, roundCorner), padding));
            cache.Add(new Vertex(positon + new Vector2(0, height - roundCorner), padding));
            for (int i = 0; i < length; i++)
            {
                cache.Add(new Vertex(positon + new Vector2(roundCorner, height - roundCorner) - rVecH.Rotate(-i * theta), padding));
            }
            cache.Add(new Vertex(positon + new Vector2(roundCorner, height), padding));
            cache.Add(new Vertex(positon + new Vector2(width - roundCorner, height), padding));
            for (int i = 0; i < length; i++)
            {
                cache.Add(new Vertex(positon + new Vector2(width - roundCorner, height - roundCorner) + rVecV.Rotate(-i * theta), padding));
            }
            cache.Add(new Vertex(positon + new Vector2(width, height - roundCorner), padding));
            cache.Add(new Vertex(positon + new Vector2(width, roundCorner), padding));
            cache.Add(new Vertex(positon + new Vector2(width, height) / 2, padding));
            short[] indices = new short[cache.Count * 3 - 3];
            for (int i = 0; i < cache.Count - 1; i++)
            {
                indices[i * 3] = (short)(cache.Count - 1);
                indices[i * 3 + 1] = (short)i;
                if (i == cache.Count - 2) indices[i * 3 + 2] = 0;
                else indices[i * 3 + 2] = (short)(i + 1);
            }
            vertexBatch.Draw(cache.ToArray(), indices);
        }
    }
}
