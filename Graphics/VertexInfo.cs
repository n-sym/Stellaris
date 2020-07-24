using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stellaris.Graphics
{
    public class VertexInfo
    {
        public Vertex[] vertex;
        public short[] index;
        public VertexInfo(Vertex[] vertex, short[] index)
        {
            this.vertex = vertex;
            this.index = index;
        }
        public VertexInfo(Vertex[] vertex)
        {
            this.vertex = vertex;
            index = new short[vertex.Length];
            for (short i = 0; i < vertex.Length; i++)
            {
                index[i] = i;
            }
        }
        public VertexInfo TransformPosition(Matrix matrix, Vector2 center = default)
        {
            Vertex[] vertices = new Vertex[vertex.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                 vertices[i] = vertex[i].ChangePosition(Vector2.Transform(vertex[i].Position - center, matrix) + center);
            }
            return new VertexInfo(vertices, index);
        }
        public VertexInfo TransformColor(Func<int, Color, Color> colorFunction)
        {
            Vertex[] vertices = new Vertex[vertex.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = vertex[i].ChangeColor(colorFunction(i, vertex[i].Color));
            }
            return new VertexInfo(vertices, index);
        }
    }
    public struct Vertex : IVertexType
    {
        public Vector2 Position;
        public Color Color;
        public Vector2 TextureCoordinate;
        static VertexDeclaration VertexDeclaration = new VertexDeclaration(
            new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0), 
            new VertexElement(8, VertexElementFormat.Color, VertexElementUsage.Color, 0),
            new VertexElement(12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0));
        VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;
        public Vertex(Vector2 position, Color color)
        {
            Position = position;
            Color = color;
            TextureCoordinate = Vector2.Zero;
        }
        public Vertex(Vector2 position, Color color, Vector2 textureCoord)
        {
            Position = position;
            Color = color;
            TextureCoordinate = textureCoord;
        }
        public static Vertex operator +(Vertex left, Vertex right)
        {
            return new Vertex(left.Position + right.Position, (left.Color.ToVector4() + right.Color.ToVector4()).ToColor(), left.TextureCoordinate + right.TextureCoordinate);
        }
        public static Vertex operator *(Vertex left, float right)
        {
            return new Vertex(left.Position * right, (left.Color.ToVector4() * right).ToColor(), left.TextureCoordinate * right);
        }
        public static Vertex operator /(Vertex left, float right)
        {
            return new Vertex(left.Position / right, (left.Color.ToVector4() / right).ToColor(), left.TextureCoordinate / right);
        }
        public Vertex AddPosition(Vector2 position)
        {
            return new Vertex(Position + position, Color, TextureCoordinate);
        }
        public Vertex ChangePosition(Vector2 newPosition)
        {
            return new Vertex(newPosition, Color, TextureCoordinate);
        }
        public Vertex ChangeColor(Color newColor)
        {
            return new Vertex(Position, newColor, TextureCoordinate);
        }
    }
}
