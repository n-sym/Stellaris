using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellaris.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stellaris.Graphics
{
    public class VertexBatch : IDisposable
    {
        public GraphicsDevice graphicsDevice;
        List<VertexInfo> vertexData;
        List<short> indexData;
        BasicEffect basicEffect;
        PrimitiveType primitiveType;
        bool begin;
        public VertexBatch(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;
            basicEffect = new BasicEffect(graphicsDevice);
            basicEffect.VertexColorEnabled = true;
            basicEffect.World = Matrix.Identity;
            vertexData = new List<VertexInfo>();
            indexData = new List<short>();
        }
        public void Begin(PrimitiveType primitiveType = PrimitiveType.TriangleList)
        {
            this.primitiveType = primitiveType;
            basicEffect.TextureEnabled = false;
            basicEffect.View = Matrix.CreateTranslation(-Common.Resolution.X / 2, Common.Resolution.Y / 2, 0);
            basicEffect.Projection = Matrix.CreateOrthographic(Common.Resolution.X, Common.Resolution.Y, 0, 100);
            begin = true;
        }
        public void Begin(Texture2D texture2D, PrimitiveType primitiveType = PrimitiveType.TriangleList)
        {
            if (begin) throw new Exception("Called Begin Twice");
            this.primitiveType = primitiveType;
            basicEffect.TextureEnabled = true;
            basicEffect.Texture = texture2D;
            basicEffect.View = Matrix.CreateTranslation(-Common.Resolution.X / 2, Common.Resolution.Y / 2, 0);
            basicEffect.Projection = Matrix.CreateOrthographic(Common.Resolution.X, Common.Resolution.Y, 0, 100);
            begin = true;
        }
        public void Draw(params VertexInfo[] vertex)
        {
            if (!begin) throw new Exception("Called Draw Before Begin");
            vertexData.AddRange(vertex);
        }
        private void FixIndex()
        {
            if (vertexData.Count > short.MaxValue) throw new Exception("Vertices Counts Over 32768");
            if (indexData.Count == 0)
            {
                for (short i = 0; i < vertexData.Count; i++)
                {
                    indexData.Add(i);
                }
            }
            else
            {
                throw new Exception("Try to use unindexed mode after using indexed mode");
            }
        }
        public void Draw(VertexInfo[] vertex, params short[] index)
        {
            if (!begin) throw new Exception("Called Draw Before Begin");
            if (indexData.Count == 0)
            {
                indexData = new List<short>();
                FixIndex();
            }
            vertexData.AddRange(vertex);
            indexData.AddRange(index);
            if (vertexData.Count > short.MaxValue) throw new Exception("Vertices Counts Over 32768");
        }
        public void End()
        {
            if (!begin) return;
            if (vertexData.Count == 0) return;
            VertexInfo[] array = vertexData.ToArray();
            int length = indexData.Count == 0 ? vertexData.Count : indexData.Count;
            length = primitiveType == PrimitiveType.TriangleList ? length / 3 : (primitiveType == PrimitiveType.LineList ? length / 2 : (primitiveType == PrimitiveType.TriangleStrip ? length - 2 : length - 1));
            if(indexData.Count == 0)
            {
                foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    graphicsDevice.DrawUserPrimitives(primitiveType, array, 0, length);
                }
            }
            else
            {
                foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    graphicsDevice.DrawUserIndexedPrimitives(primitiveType, array, 0, array.Length, indexData.ToArray(), 0, length);
                }
            }
            vertexData.Clear();
            indexData.Clear();
            begin = false;
        }
        public void Dispose()
        {
            vertexData.Clear();
            indexData.Clear();
        }
    }
}
