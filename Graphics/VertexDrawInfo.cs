using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Stellaris.Graphics
{
    /// <summary>
    /// 顶点绘制信息
    /// </summary>
    public class VertexDrawInfo
    {
        public PrimitiveType? primitiveType;
        public Texture2D? texture;
        public Vertex[] vertices;
        public int[] indices;
        public VertexDrawInfo(Vertex[] vertex, int[] index)
        {
            vertices = vertex;
            indices = index;
        }
        public VertexDrawInfo(Vertex[] vertex, int[] index, Texture2D texture2D)
        {
            vertices = vertex;
            indices = index;
            texture = texture2D;
        }
        public VertexDrawInfo(Vertex[] vertex, int[] index, PrimitiveType primitiveType)
        {
            vertices = vertex;
            indices = index;
            this.primitiveType = primitiveType;
        }
        public VertexDrawInfo(Vertex[] vertex)
        {
            vertices = vertex;
            indices = new int[vertex.Length];
            for (int i = 0; i < vertex.Length; i++)
            {
                indices[i] = i;
            }
        }
        public VertexDrawInfo(Vector2[] position, Color[] color, int[] index)
        {
            vertices = new Vertex[position.Length];
            for (int i = 0; i < position.Length; i++)
            {
                vertices[i] = new Vertex(position[i], color[i]);
            }
            indices = index;
        }
        public VertexDrawInfo TransformPosition(Matrix matrix, Vector2 center)
        {
            Vertex[] _vertices = new Vertex[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                _vertices[i] = vertices[i].ChangePosition(Vector2.Transform(vertices[i].Position.XY() - center, matrix) + center);
            }
            return new VertexDrawInfo(_vertices, indices);
        }
        public VertexDrawInfo TransformPosition(Func<int, Vector2, Vector2> positionFunction)
        {
            Vertex[] _vertices = new Vertex[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                _vertices[i] = vertices[i].ChangePosition(positionFunction(i, vertices[i].Position.XY()));
            }
            return new VertexDrawInfo(_vertices, indices);
        }
        public VertexDrawInfo TransformPosition(Matrix matrix, Vector3 center = default)
        {
            Vertex[] _vertices = new Vertex[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                _vertices[i] = vertices[i].ChangePosition(Vector3.Transform(vertices[i].Position - center, matrix) + center);
            }
            return new VertexDrawInfo(_vertices, indices);
        }
        public VertexDrawInfo TransformPosition(Func<int, Vector3, Vector3> positionFunction)
        {
            Vertex[] _vertices = new Vertex[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                _vertices[i] = vertices[i].ChangePosition(positionFunction(i, vertices[i].Position));
            }
            return new VertexDrawInfo(_vertices, indices);
        }
        public VertexDrawInfo TransformColor(Func<int, Color, Color> colorFunction)
        {
            Vertex[] _vertices = new Vertex[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                _vertices[i] = vertices[i].ChangeColor(colorFunction(i, vertices[i].Color));
            }
            return new VertexDrawInfo(_vertices, indices);
        }
        public VertexDrawInfo TransformCoord(Func<int, Vector2, Vector2> coordFunction)
        {
            Vertex[] _vertices = new Vertex[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                _vertices[i] = vertices[i].ChangeCoord(coordFunction(i, vertices[i].TextureCoordinate));
            }
            return new VertexDrawInfo(_vertices, indices);
        }
        public VertexDrawInfo Transform(Func<int, Vertex, Vertex> vertexFunction)
        {
            Vertex[] _vertices = new Vertex[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                _vertices[i] = vertexFunction(i, vertices[i]);
            }
            return new VertexDrawInfo(_vertices, indices);
        }
        public void Append(VertexDrawInfo vertexDrawInfo)
        {
            int v = vertices.Length;
            int i = indices.Length;
            Array.Resize(ref vertices, vertices.Length + vertexDrawInfo.vertices.Length);
            Array.Resize(ref indices, indices.Length + vertexDrawInfo.indices.Length);
            Array.Copy(vertexDrawInfo.vertices, 0, vertices, v, vertexDrawInfo.vertices.Length);
            Array.Copy(vertexDrawInfo.indices, 0, indices, i, vertexDrawInfo.indices.Length);
            for (int j = i; j < indices.Length; j++)
            {
                indices[j] += i;
            }
        }
        public static Vertex[] NewVertices(int length, Func<int, Vertex> vertexFunction)
        {
            Vertex[] result = new Vertex[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = vertexFunction(i);
            }
            return result;
        }
        public static int[] NewIndices(int length, Func<int, int> indiceFunction)
        {
            int[] result = new int[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = indiceFunction(i);
            }
            return result;
        }
    }
    public struct Vertex : IVertexType
    {
        public Vector3 Position;
        public Color Color;
        public Vector2 TextureCoordinate;
        public static VertexDeclaration VertexDeclaration = new VertexDeclaration(
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(12, VertexElementFormat.Color, VertexElementUsage.Color, 0),
            new VertexElement(16, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0));
        VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;
        public Vertex(Vector3 position, Color color, Vector2 textureCoord)
        {
            Position = position;
            Color = color;
            TextureCoordinate = textureCoord;
        }
        public Vertex(Vector3 position, Vector2 textureCoord) : this(position, Color.White, textureCoord)
        {
        }
        public Vertex(Vector3 position, Color color) : this(position, color, Vector2.Zero)
        {
        }
        public Vertex(Vector3 position) : this(position, Color.White, Vector2.Zero)
        {
        }
        public Vertex(Vector2 position, Color color, Vector2 textureCoord) : this(new Vector3(position, 0), color, textureCoord)
        {

        }
        public Vertex(Vector2 position, Vector2 textureCoord) : this(new Vector3(position, 0), Color.White, textureCoord)
        {
        }
        public Vertex(Vector2 position, Color color) : this(new Vector3(position, 0), color, Vector2.Zero)
        {
        }
        public Vertex(Vector2 position) : this(new Vector3(position, 0), Color.White, Vector2.Zero)
        {
        }
        public Vertex AddPosition(Vector3 position)
        {
            return new Vertex(Position + position, Color, TextureCoordinate);
        }
        public Vertex ChangePosition(Vector3 newPosition)
        {
            return new Vertex(newPosition, Color, TextureCoordinate);
        }
        public Vertex AddPosition(Vector2 position)
        {
            return new Vertex(Position.XY() + position, Color, TextureCoordinate);
        }
        public Vertex ChangePosition(Vector2 newPosition)
        {
            return new Vertex(newPosition, Color, TextureCoordinate);
        }
        public Vertex ChangeColor(Color newColor)
        {
            return new Vertex(Position, newColor, TextureCoordinate);
        }
        public Vertex AddCrood(Vector2 coord)
        {
            return new Vertex(Position, Color, TextureCoordinate + coord);
        }
        public Vertex ChangeCoord(Vector2 newCoord)
        {
            return new Vertex(Position, Color, newCoord);
        }
        public Vertex ChangeCoord(float newCoordX, float newCoordY)
        {
            return new Vertex(Position, Color, new Vector2(newCoordX, newCoordY));
        }

        public void SwapCoord(ref Vertex target)
        {
            Vector2 cache = TextureCoordinate;
            TextureCoordinate = target.TextureCoordinate;
            target.TextureCoordinate = cache;
        }
        public override string ToString()
        {
            return string.Format("Position:{0},Color:{1},Coord:{2}", Position, Color, TextureCoordinate);
        }
    }
}
