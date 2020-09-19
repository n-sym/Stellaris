using Microsoft.Xna.Framework;
using System;

namespace Stellaris.Graphics
{
    public static class VertexStrip
    {
        public static VertexDrawInfo Strip(Vector2[] pos, float size, Func<int, Vertex, Vertex> vertexFunc = null)
        {
            Vertex[] result = new Vertex[pos.Length * 2];
            for (int i = 0; i < pos.Length; i++)
            {
                float theta = (pos.TryGetValue(i + 1) - pos[i]).Angle() + 1.571f;
                Vector2 vec = new Vector2(size, 0).RotateTo(theta);
                if (vertexFunc == null)
                {
                    result[i * 2] = new Vertex(pos[i] + vec);
                    result[i * 2 + 1] = new Vertex(pos[i] - vec);
                }
                else
                {
                    result[i * 2] = vertexFunc(i * 2, new Vertex(pos[i] + vec));
                    result[i * 2 + 1] = vertexFunc(i * 2 + 1, new Vertex(pos[i] - vec));
                }
            }
            if (vertexFunc == null)
            {
                result[result.Length - 2] = result[result.Length - 4].AddPosition(pos[pos.Length - 1] - pos[pos.Length - 2]);
                result[result.Length - 1] = result[result.Length - 3].AddPosition(pos[pos.Length - 1] - pos[pos.Length - 2]);
            }
            else
            {
                result[result.Length - 2] = vertexFunc(result.Length - 2, result[result.Length - 4].AddPosition(pos[pos.Length - 1] - pos[pos.Length - 2]));
                result[result.Length - 1] = vertexFunc(result.Length - 1, result[result.Length - 3].AddPosition(pos[pos.Length - 1] - pos[pos.Length - 2]));
            }
            return new VertexDrawInfo(result, Helper.FromAToB(0, (short)result.Length));
        }
        public static VertexDrawInfo StripOneSide(Vector2[] pos, float size, Func<int, Vertex, Vertex> vertexFunc = null)
        {
            Vertex[] result = new Vertex[pos.Length * 2];
            for (int i = 0; i < pos.Length; i++)
            {
                float theta = (pos.TryGetValue(i + 1) - pos[i]).Angle() + 1.571f;
                Vector2 vec = new Vector2(size, 0).RotateTo(theta) * (size > 0 ? 1 : -1);
                if (vertexFunc == null)
                {
                    result[i * 2] = new Vertex(pos[i] + vec);
                    result[i * 2 + 1] = new Vertex(pos[i]);
                }
                else
                {
                    result[i * 2] = vertexFunc(i * 2, new Vertex(pos[i] + vec));
                    result[i * 2 + 1] = vertexFunc(i * 2 + 1, new Vertex(pos[i]));
                }
            }
            if (vertexFunc == null)
            {
                result[result.Length - 2] = result[result.Length - 4].AddPosition(pos[pos.Length - 1] - pos[pos.Length - 2]);
                result[result.Length - 1] = result[result.Length - 3].AddPosition(pos[pos.Length - 1] - pos[pos.Length - 2]);
            }
            else
            {
                result[result.Length - 2] = vertexFunc(result.Length - 2, result[result.Length - 4].AddPosition(pos[pos.Length - 1] - pos[pos.Length - 2]));
                result[result.Length - 1] = vertexFunc(result.Length - 1, result[result.Length - 3].AddPosition(pos[pos.Length - 1] - pos[pos.Length - 2]));
            }
            return new VertexDrawInfo(result, Helper.FromAToB(0, (short)result.Length));
        }
        public static VertexDrawInfo Strip(Vector2[] pos, float[] size, Func<int, Vertex, Vertex> vertexFunc = null)
        {
            Vertex[] result = new Vertex[pos.Length * 2];
            for (int i = 0; i < pos.Length; i++)
            {
                float theta = (pos.TryGetValue(i + 1) - pos[i]).Angle() + 1.571f;
                Vector2 vec = new Vector2(size[i], 0).RotateTo(theta);
                if (vertexFunc == null)
                {
                    result[i * 2] = new Vertex(pos[i] + vec);
                    result[i * 2 + 1] = new Vertex(pos[i] - vec);
                }
                else
                {
                    result[i * 2] = vertexFunc(i * 2, new Vertex(pos[i] + vec));
                    result[i * 2 + 1] = vertexFunc(i * 2 + 1, new Vertex(pos[i] - vec));
                }
            }
            if (vertexFunc == null)
            {
                float theta = (pos[pos.Length - 1] - pos[pos.Length - 2]).Angle() + 1.571f;
                Vector2 vec = new Vector2(size[size.Length - 1], 0).RotateTo(theta);
                result[result.Length - 2] = new Vertex(pos[pos.Length - 1] + vec);
                result[result.Length - 1] = new Vertex(pos[pos.Length - 1] - vec);
            }
            else
            {
                float theta = (pos[pos.Length - 1] - pos[pos.Length - 2]).Angle() + 1.571f;
                Vector2 vec = new Vector2(size[size.Length - 1], 0).RotateTo(theta);
                result[result.Length - 2] = vertexFunc(result.Length - 2, new Vertex(pos[pos.Length - 1] + vec));
                result[result.Length - 1] = vertexFunc(result.Length - 1, new Vertex(pos[pos.Length - 1] - vec));
            }
            return new VertexDrawInfo(result, Helper.FromAToB(0, (short)result.Length));
        }
    }
    /*public enum TriangleVertexType
    {
        A = 1,
        B = 2,
        C = 3
    }
    public struct VertexTriangle
    {
        Vertex vertexA;
        Vertex vertexB;
        Vertex vertexC;
        public VertexTriangle(Vertex vertexA, Vertex vertexB, Vertex vertexC)
        {
            this.vertexA = vertexA;
            this.vertexB = vertexB;
            this.vertexC = vertexC;
        }
        public VertexTriangle(Vertex[] vertices)
        {
            vertexA = vertices.TryGetValue(0);
            vertexB = vertices.TryGetValue(1);
            vertexC = vertices.TryGetValue(2);
        }
        public Vertex[] ToVertex()
        {
            return new Vertex[] { vertexA, vertexB, vertexC };
        }
        public static VertexTriangle Rotate(Vertex vertexA, Vertex vertexB, Vertex vertexC, float radian)
        {
            Vector2 a = vertexA.Position;
            Vector2 b = vertexB.Position.Rotate(radian, a);
            Vector2 c = vertexC.Position.Rotate(radian, a);
            return new VertexTriangle(vertexA, vertexB.ChangePosition(b), vertexC.ChangePosition(c));
        }
        public VertexTriangle Rotate(float radian, TriangleVertexType center)
        {
            if (center == TriangleVertexType.A)
            {
                return Rotate(vertexA, vertexB, vertexC, radian);
            }
            if (center == TriangleVertexType.B)
            {
                return Rotate(vertexB, vertexA, vertexC, radian);
            }
            return Rotate(vertexC, vertexA, vertexB, radian);
        }
        public VertexTriangle Rotate(float radian, Vector2 center)
        {
            Vector2 a = vertexA.Position.Rotate(radian, center);
            Vector2 b = vertexB.Position.Rotate(radian, center);
            Vector2 c = vertexC.Position.Rotate(radian, center);
            return new VertexTriangle(vertexA.ChangePosition(a), vertexB.ChangePosition(b), vertexC.ChangePosition(c));
        }
        public VertexTriangle Rotate(float radian)
        {
            return Rotate(radian, (vertexA.Position + vertexB.Position + vertexC.Position) / 3);
        }
        private static VertexInfo RotationList(Vertex vertexA, Vertex vertexB, Vertex vertexC, float totalRadian, Func<int, Vertex, Vertex> vertexFunc = null)
        {
            Vector2 side = vertexB.Position - vertexA.Position;
            Vector2 anoter = vertexC.Position - vertexA.Position;
            bool flag = side.Angle() < anoter.Angle();
            float theta = side.AngleBetween(anoter);
            if ((side.Angle() + theta >= 6.283f) || (anoter.Angle() + theta >= 6.283f)) theta = 6.283f - theta;
            if (flag)
            {
                side = anoter;
                Vertex cache2 = vertexB;
                vertexB = vertexC;
                vertexC = cache2;
            }
            int count = (int)Math.Ceiling(totalRadian / theta);
            if (count < 2) return new VertexInfo(new Vertex[] { vertexA, vertexB, vertexC });
            Vertex[] vertices = new Vertex[count + 2];
            if (vertexFunc == null) vertices[0] = vertexA;
            else vertices[0] = vertexFunc(0, vertexA);
            for (int i = 1; i < count + 2; i++)
            {
                if (vertexFunc == null) vertices[i] = vertexC.ChangePosition(side + vertexA.Position);
                else vertices[i] = vertexFunc(i, vertexC.ChangePosition(side + vertexA.Position));
                side = side.Rotate(theta);
            }
            short[] index = new short[count * 3];
            for (int i = 0; i < count * 3; i += 3)
            {
                index[i + 2] = 0;
                index[i + 1] = (short)(i / 3 + 1);
                index[i] = (short)(i / 3 + 2);
            }
            return new VertexInfo(vertices, index);
        }
        public static VertexInfo RotationList(VertexTriangle triangle, float totalRadian, Func<int, Vertex, Vertex> vertexFunc = null)
        {
            return RotationList(triangle.vertexA, triangle.vertexB, triangle.vertexC, totalRadian, vertexFunc);
        }
        public VertexInfo RotationList(TriangleVertexType center, float radian = 6.283f)
        {
            if (center == TriangleVertexType.A)
            {
                return RotationList(vertexA, vertexB, vertexC, radian);
            }
            if (center == TriangleVertexType.B)
            {
                return RotationList(vertexB, vertexA, vertexC, radian);
            }
            return RotationList(vertexC, vertexA, vertexB, radian);
        }
        public static VertexTriangle operator +(VertexTriangle left, VertexTriangle right)
        {
            return new VertexTriangle(left.vertexA.AddPosition(right.vertexA.Position), left.vertexB.AddPosition(right.vertexB.Position), left.vertexC.AddPosition(right.vertexC.Position));
        }
        public static VertexTriangle operator -(VertexTriangle left, VertexTriangle right)
        {
            return new VertexTriangle(left.vertexA.AddPosition(-right.vertexA.Position), left.vertexB.AddPosition(-right.vertexB.Position), left.vertexC.AddPosition(-right.vertexC.Position));
        }
        public static VertexTriangle operator *(VertexTriangle left, float right)
        {
            return new VertexTriangle(left.vertexA.ChangePosition(left.vertexA.Position * right), left.vertexB.ChangePosition(left.vertexB.Position * right), left.vertexC.ChangePosition(left.vertexC.Position * right));
        }
    }*/
}
