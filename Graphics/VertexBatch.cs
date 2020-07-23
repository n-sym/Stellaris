using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellaris.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stellaris.Graphics
{
    public class VertexBatch
    {
        public GraphicsDevice graphicsDevice;
        List<VertexInfo> vertexData;
        List<short> indexData;
        BasicEffect basicEffect;
        bool begin;
        public VertexBatch(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;
            basicEffect = new BasicEffect(graphicsDevice);
            basicEffect.VertexColorEnabled = true;
            basicEffect.World = Matrix.Identity;
        }
        public void Begin()
        {
            vertexData = new List<VertexInfo>();
            indexData = null;
            basicEffect.View = Matrix.CreateTranslation(-Common.Resolution.X / 2, Common.Resolution.Y / 2, 0);
            basicEffect.Projection = Matrix.CreateOrthographic(Common.Resolution.X, Common.Resolution.Y, 0, 100);
            begin = true;
        }
        public void Draw(params VertexInfo[] vertex)
        {
            if (!begin) throw new Exception("Awoke Draw Before Begin");
            if (vertex.Length < 3) return;
            if (vertex.Length % 3 != 0) Array.Resize(ref vertex, vertex.Length - (vertex.Length % 3));
            vertexData.AddRange(vertex);
        }
        private void FixIndex()
        {
            if (indexData.Count != 0) return;
            if (vertexData.Count > short.MaxValue) throw new Exception("Vertices Counts Over 32768");
            for (short i = 0; i < vertexData.Count; i++)
            {
                indexData.Add(i);
            }
        }
        public void Draw(VertexInfo[] vertex, params short[] index)
        {
            if (!begin) throw new Exception("Awoke Draw Before Begin");
            if (indexData == null)
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
            VertexBuffer vertexBuffer = new VertexBuffer(graphicsDevice,
                typeof(VertexInfo),
                array.Length,
                BufferUsage.None);
                vertexBuffer.SetData(array);
            graphicsDevice.SetVertexBuffer(vertexBuffer);
            if(indexData == null)
            {
                foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, array, 0, array.Length / 3);
                }
            }
            else
            {
                foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, array, 0, array.Length, indexData.ToArray(), 0, array.Length / 3);
                }
            }
           
        }
    }
}
