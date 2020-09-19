using Microsoft.Xna.Framework.Graphics;
namespace Stellaris.Graphics
{
    public static class GraphicsExpansions
    {
        public static SpriteBatchS ToSte(this SpriteBatch spriteBatch)
        {
            return spriteBatch as SpriteBatchS;
        }
        public static void DrawUserIndexedPrimitives(this GraphicsDevice graphicsDevice, PrimitiveType primitiveType, VertexDrawInfo vertexInfo)
        {
            graphicsDevice.DrawUserIndexedPrimitives(primitiveType, vertexInfo.vertices, 0, vertexInfo.vertices.Length, vertexInfo.indices, 0, VertexBatch.LengthGusser(vertexInfo.vertices.Length, primitiveType));
        }
    }
}
