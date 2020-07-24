using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public Triangle Rotate(float radian, TriangleVertexType center)
        {
            if (center == TriangleVertexType.A)
            {
                Vector2 a = vertexA.Position;
                Vector2 b = vertexB.Position.Rotate(radian, a);
                Vector2 c = vertexC.Position.Rotate(radian, a);
                return new Triangle(vertexA, vertexB.ChangePosition(b), vertexC.ChangePosition(c));
            }
            if (center == TriangleVertexType.B)
            {
                Vector2 b = vertexB.Position;
                Vector2 a = vertexA.Position.Rotate(radian, b); ;
                Vector2 c = vertexC.Position.Rotate(radian, b);
                return new Triangle(vertexA.ChangePosition(a), vertexB, vertexC.ChangePosition(c));
            }
            Vector2 C = vertexC.Position;
            Vector2 A = vertexA.Position.Rotate(radian, C);
            Vector2 B = vertexB.Position.Rotate(radian, C);
            return new Triangle(vertexA.ChangePosition(A), vertexB.ChangePosition(B), vertexC);
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
        private static VertexInfo Rotation(Vertex vertexA, Vertex vertexB, Vertex vertexC, float totalRadian)
        {
            Vector2 side = vertexB.Position - vertexA.Position;
            Vector2 anoter = vertexC.Position - vertexA.Position;
            bool flag = side.Angle() < anoter.Angle();
            float theta = side.AngleBetween(anoter);
            bool flag2 = (side.Angle() + theta >= 6.283f) || (anoter.Angle() + theta >= 6.283f);
            flag = flag2 ? !flag : flag;
            theta = flag2 ? 6.283f - theta : theta;
            if (flag)
            {
                side = anoter;
                Vertex cache2 = vertexB;
                vertexB = vertexC;
                vertexC = cache2;
            }
            int count = (int)Math.Ceiling(totalRadian / theta);
            if (count < 2) return new VertexInfo(new Vertex[] { vertexA, vertexB, vertexC});
            Vertex[] vertices = new Vertex[count + 2];
            vertices[0] = vertexA;
            vertices[1] = vertexC;
            vertices[2] = vertexB;
            bool isEven = false;
            for (int i = 3; i < count + 2; i++)
            {
                side = side.Rotate(theta);
                vertices[i] = vertexB.ChangePosition(side + vertexA.Position);
                isEven = !isEven;
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
                return Rotation(vertexA, vertexB, vertexC, radian);
            }
            if (center == TriangleVertexType.B)
            {
                return Rotation(vertexB, vertexA, vertexC, radian);
            }
            return Rotation(vertexC, vertexA, vertexB, radian);
        }
    }
}
