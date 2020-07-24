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
        List<Vertex> vertexData;
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
            vertexData = new List<Vertex>();
            indexData = new List<short>();
        }
        public void Begin(PrimitiveType primitiveType = PrimitiveType.TriangleList)
        {
            if (begin) throw new Exception("Called Begin Twice");
            this.primitiveType = primitiveType;
            RasterizerState rasterizerState = new RasterizerState();
            this.primitiveType = primitiveType;
            basicEffect.TextureEnabled = false;
            basicEffect.View = Matrix.CreateTranslation(-Common.Resolution.X / 2, -Common.Resolution.Y / 2, 0) * Matrix.CreateRotationX(3.141592f);
            basicEffect.Projection = Matrix.CreateOrthographic(Common.Resolution.X, Common.Resolution.Y, -100, 100);
            begin = true;
        }
        public void Begin(Texture2D texture2D, PrimitiveType primitiveType = PrimitiveType.TriangleList)
        {
            if (begin) throw new Exception("Called Begin Twice");
            this.primitiveType = primitiveType;
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            graphicsDevice.RasterizerState = rasterizerState;
            basicEffect.TextureEnabled = true;
            basicEffect.Texture = texture2D;
            basicEffect.View = Matrix.CreateTranslation(-Common.Resolution.X / 2, -Common.Resolution.Y / 2, 0) * Matrix.CreateRotationX(3.141592f);
            basicEffect.Projection = Matrix.CreateOrthographic(Common.Resolution.X, Common.Resolution.Y, -100, 100);
            begin = true;
        }
        public void Draw(params Vertex[] vertex)
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
        public void Draw(Vertex[] vertex, params short[] index)
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
        public void Draw(VertexInfo vertexInfo)
        {
            if (!begin) throw new Exception("Called Draw Before Begin");
            if (indexData.Count == 0)
            {
                indexData = new List<short>();
                FixIndex();
            }
            vertexData.AddRange(vertexInfo.vertex);
            indexData.AddRange(vertexInfo.index);
            if (vertexData.Count > short.MaxValue) throw new Exception("Vertices Counts Over 32768");
        }
        private int TriangleTripLengthGusser(int length)
        {
            bool flag = false;
            if (indexData.TryGetValue(0) == indexData.TryGetValue(3) && indexData.TryGetValue(3) == indexData.TryGetValue(6)) flag = true;
            if (indexData.TryGetValue(1) == indexData.TryGetValue(4) && indexData.TryGetValue(4) == indexData.TryGetValue(7)) flag = true;
            if (indexData.TryGetValue(2) == indexData.TryGetValue(5) && indexData.TryGetValue(5) == indexData.TryGetValue(8)) flag = true;
            if (flag) return (length - 1) / 2;
            else return length - 2;
        }
        public void End()
        {
            if (!begin) return;
            if (vertexData.Count == 0) return;
            Vertex[] array = vertexData.ToArray();
            int length = indexData.Count == 0 ? vertexData.Count : indexData.Count;
            length = primitiveType == PrimitiveType.TriangleList ? length / 3 : (primitiveType == PrimitiveType.LineList ? length / 2 : (primitiveType == PrimitiveType.TriangleStrip ? TriangleTripLengthGusser(length) : length - 1));
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
