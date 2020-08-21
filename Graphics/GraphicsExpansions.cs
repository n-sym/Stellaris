using Stellaris;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Stellaris.Graphics
{
    public static class GraphicsExpansions
    {
        public static void DrawUserIndexedPrimitives(this GraphicsDevice graphicsDevice, PrimitiveType primitiveType, VertexInfo vertexInfo)
        {
            graphicsDevice.DrawUserIndexedPrimitives(primitiveType, vertexInfo.vertex, 0, vertexInfo.vertex.Length, vertexInfo.index, 0, VertexBatch.LengthGusser(vertexInfo.vertex.Length, primitiveType));
        }
    }
}
