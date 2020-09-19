﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Stellaris.Graphics
{
    public class VertexDrawInfo : IDrawInfo
    {
        public Vertex[] vertices;
        public short[] indices;
        public VertexDrawInfo(Vertex[] vertex, short[] index)
        {
            this.vertices = vertex;
            this.indices = index;
        }
        public VertexDrawInfo(Vertex[] vertex)
        {
            this.vertices = vertex;
            indices = new short[vertex.Length];
            for (short i = 0; i < vertex.Length; i++)
            {
                indices[i] = i;
            }
        }
        public VertexDrawInfo TransformPosition(Matrix matrix, Vector2 center = default)
        {
            Vertex[] _vertices = new Vertex[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                _vertices[i] = vertices[i].ChangePosition(Vector2.Transform(vertices[i].Position - center, matrix) + center);
            }
            return new VertexDrawInfo(_vertices, indices);
        }
        public VertexDrawInfo TransformPosition(Func<int, Vector2, Vector2> positionFunction)
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
        public Vertex(Vector2 position)
        {
            Position = position;
            Color = Color.White;
            TextureCoordinate = Vector2.Zero;
        }
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
        public Vertex(Vector2 position, Vector2 textureCoord)
        {
            Position = position;
            Color = Color.White;
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
        public static Vertex[] GetVertices(int length, Func<int, Vertex> vertexFunction)
        {
            Vertex[] result = new Vertex[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = vertexFunction(i);
            }
            return result;
        }
        public override string ToString()
        {
            return string.Format("Position:{0},Color:{1},Coord:{2}", Position, Color, TextureCoordinate);
        }
    }
}