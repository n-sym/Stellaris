using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Stellaris.Graphics
{
    public static class Border
    {
        private static void GetBorder(List<Vertex> cache, Vector2 positon, int width, int height, int roundCorner, Color padding, float quality = 1f)
        {
            int length = (int)(roundCorner * 0.5f * quality);
            float theta = 1.571f / (length - 1);
            Vector2 rVecH = new Vector2(roundCorner, 0);
            Vector2 rVecV = new Vector2(0, roundCorner); for (int i = 0; i < length; i++)
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
            cache.Add(new Vertex(positon + new Vector2(roundCorner + 1, height), padding));
            cache.Add(new Vertex(positon + new Vector2(width - roundCorner, height), padding));
            for (int i = 0; i < length; i++)
            {
                cache.Add(new Vertex(positon + new Vector2(width - roundCorner, height - roundCorner) + rVecV.Rotate(-i * theta), padding));
            }
            cache.Add(new Vertex(positon + new Vector2(width, height - roundCorner), padding));
            cache.Add(new Vertex(positon + new Vector2(width, roundCorner), padding));
        }
        public static VertexDrawInfo GetPaddingBorderDrawInfo(PrimitiveType primitiveType, Vector2 positon, int width, int height, int roundCorner, Color padding, float quality = 1f)
        {
            if (primitiveType != PrimitiveType.TriangleList) return new VertexDrawInfo(new Vertex[] { new Vertex() });
            List<Vertex> cache = new List<Vertex>();
            GetBorder(cache, positon, width, height, roundCorner, padding, quality);
            cache.Add(new Vertex(positon + new Vector2(width, height) / 2, padding));
            short[] indices = new short[cache.Count * 3 - 3];
            for (int i = 0; i < cache.Count - 1; i++)
            {
                indices[i * 3] = (short)(cache.Count - 1);
                indices[i * 3 + 1] = (short)i;
                if (i == cache.Count - 2) indices[i * 3 + 2] = 0;
                else indices[i * 3 + 2] = (short)(i + 1);
            }
            return new VertexDrawInfo(cache.ToArray(), indices);
        }
        public static VertexDrawInfo GetBorderDrawInfo(PrimitiveType primitiveType, Vector2 positon, int width, int height, int roundCorner, Color color, float quality = 1f)
        {
            if (primitiveType != PrimitiveType.LineStrip && primitiveType != PrimitiveType.LineList) return new VertexDrawInfo(new Vertex[] { new Vertex() });
            List<Vertex> cache = new List<Vertex>();
            GetBorder(cache, positon, width, height, roundCorner, color, quality);
            short[] indices = new short[primitiveType == PrimitiveType.LineStrip ? cache.Count : cache.Count * 2];
            if (primitiveType == PrimitiveType.LineStrip)
            {
                indices = Helper.FromAToB(0, (short)(cache.Count - 1));
            }
            else
            {
                for (int i = 0; i < cache.Count; i++)
                {
                    indices[i * 2] = (short)i;
                    if (i + 1 < cache.Count) indices[i * 2 + 1] = (short)(i + 1);
                    else indices[1 * 2 + 1] = 0;
                }
            }
            return new VertexDrawInfo(cache.ToArray(), indices);
        }

        public static void DrawPaddingBorder(VertexBatch vertexBatch, Vector2 positon, int width, int height, int roundCorner, Color padding, float quality = 1f)
        {
            vertexBatch.Draw(GetPaddingBorderDrawInfo(vertexBatch.primitiveType, positon, width, height, roundCorner, padding, quality));
        }
    }
}
