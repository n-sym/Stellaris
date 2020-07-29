using Microsoft.Xna.Framework;
using System;

namespace Stellaris.Graphics
{
    public enum TriangleVertexType
    {
        A = 1,
        B = 2,
        C = 3
    }
    public struct Triangle
    {
        Vertex vertexA;
        Vertex vertexB;
        Vertex vertexC;
        public Triangle(Vertex vertexA, Vertex vertexB, Vertex vertexC)
        {
            this.vertexA = vertexA;
            this.vertexB = vertexB;
            this.vertexC = vertexC;
        }
        public Triangle(Vertex[] vertices)
        {
            vertexA = vertices.TryGetValue(0);
            vertexB = vertices.TryGetValue(1);
            vertexC = vertices.TryGetValue(2);
        }
        public Vertex[] ToVertex()
        {
            return new Vertex[] { vertexA, vertexB, vertexC };
        }
        public static Triangle Rotate(Vertex vertexA, Vertex vertexB, Vertex vertexC, float radian)
        {
            Vector2 a = vertexA.Position;
            Vector2 b = vertexB.Position.Rotate(radian, a);
            Vector2 c = vertexC.Position.Rotate(radian, a);
            return new Triangle(vertexA, vertexB.ChangePosition(b), vertexC.ChangePosition(c));
        }
        public Triangle Rotate(float radian, TriangleVertexType center)
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
        public Triangle Rotate(float radian, Vector2 center)
        {
            Vector2 a = vertexA.Position.Rotate(radian, center);
            Vector2 b = vertexB.Position.Rotate(radian, center);
            Vector2 c = vertexC.Position.Rotate(radian, center);
            return new Triangle(vertexA.ChangePosition(a), vertexB.ChangePosition(b), vertexC.ChangePosition(c));
        }
        public Triangle Rotate(float radian)
        {
            return Rotate(radian, (vertexA.Position + vertexB.Position + vertexC.Position) / 3);
        }
        private static VertexInfo RotationList(Vertex vertexA, Vertex vertexB, Vertex vertexC, float totalRadian)
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
            vertices[0] = vertexA;
            for (int i = 1; i < count + 2; i++)
            {
                vertices[i] = vertexC.ChangePosition(side + vertexA.Position);
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
        public static Triangle operator +(Triangle left, Triangle right)
        {
            return new Triangle(left.vertexA.AddPosition(right.vertexA.Position), left.vertexB.AddPosition(right.vertexB.Position), left.vertexC.AddPosition(right.vertexC.Position));
        }
        public static Triangle operator -(Triangle left, Triangle right)
        {
            return new Triangle(left.vertexA.AddPosition(-right.vertexA.Position), left.vertexB.AddPosition(-right.vertexB.Position), left.vertexC.AddPosition(-right.vertexC.Position));
        }
        public static Triangle operator *(Triangle left, float right)
        {
            return new Triangle(left.vertexA.ChangePosition(left.vertexA.Position * right), left.vertexB.ChangePosition(left.vertexB.Position * right), left.vertexC.ChangePosition(left.vertexC.Position * right));
        }
    }
}
