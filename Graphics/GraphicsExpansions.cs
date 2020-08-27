
/* 项目“StellarisMobile”的未合并的更改
在此之前:
using Stellaris;
using Microsoft.Xna.Framework;
在此之后:
using Microsoft.Xna.Framework;
*/
using Microsoft.Xna.Framework.Graphics;
using Stellaris;

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
