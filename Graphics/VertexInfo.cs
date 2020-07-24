using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stellaris.Graphics
{
    public struct VertexInfo : IVertexType
    {
        public Vector2 Position;
        public Color Color;
        public Vector2 TextureCoordinate;
        static VertexDeclaration VertexDeclaration = new VertexDeclaration(
            new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0), 
            new VertexElement(8, VertexElementFormat.Color, VertexElementUsage.Color, 0),
            new VertexElement(12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0));
        VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;
        public VertexInfo(Vector2 position, Color color)
        {
            Position = position;
            Color = color;
            TextureCoordinate = Vector2.Zero;
        }
        public VertexInfo(Vector2 position, Color color, Vector2 textureCoord)
        {
            Position = position;
            Color = color;
            TextureCoordinate = textureCoord;
        }
        public static VertexInfo operator +(VertexInfo left, VertexInfo right)
        {
            return new VertexInfo(left.Position + right.Position, (left.Color.ToVector4() + right.Color.ToVector4()).ToColor(), left.TextureCoordinate + right.TextureCoordinate);
        }
        public static VertexInfo operator *(VertexInfo left, float right)
        {
            return new VertexInfo(left.Position * right, (left.Color.ToVector4() * right).ToColor(), left.TextureCoordinate * right);
        }
        public static VertexInfo operator /(VertexInfo left, float right)
        {
            return new VertexInfo(left.Position / right, (left.Color.ToVector4() / right).ToColor(), left.TextureCoordinate / right);
        }
    }
}
