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
        static VertexDeclaration VertexDeclaration = new VertexDeclaration(
            new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0), 
            new VertexElement(8, VertexElementFormat.Color, VertexElementUsage.Color, 0));
        VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;
        public VertexInfo(Vector2 position, Color color)
        {
            this.Position = position;
            this.Color = color;
        }
    }
}
