using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Stellaris.Graphics
{
    public class VertexBatch : IDisposable
    {
        public GraphicsDevice graphicsDevice;
        public PrimitiveType primitiveType;
        public bool begin;
        List<Vertex> vertexData;
        List<short> indexData;
        BasicEffect basicEffect;
        public VertexBatch(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;
            basicEffect = new BasicEffect(graphicsDevice);
            basicEffect.VertexColorEnabled = true;
            basicEffect.World = Matrix.Identity;
            vertexData = new List<Vertex>();
            indexData = new List<short>();
        }
        private void OnBeginning(PrimitiveType primitiveType)
        {
            if (begin) throw new Exception("Called Begin Twice");
            this.primitiveType = primitiveType;
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            graphicsDevice.RasterizerState = rasterizerState;
            basicEffect.View = Matrix.CreateTranslation(-Common.Resolution.X / 2, -Common.Resolution.Y / 2, 0) * Matrix.CreateRotationX(3.141592f);
            basicEffect.Projection = Matrix.CreateOrthographic(Common.Resolution.X, Common.Resolution.Y, -100, 100);
            begin = true;
        }
        public void Begin(PrimitiveType primitiveType = PrimitiveType.TriangleList)
        {
            OnBeginning(primitiveType);
            basicEffect.TextureEnabled = false;
        }
        public void Begin(Texture2D texture2D, PrimitiveType primitiveType = PrimitiveType.TriangleList)
        {
            OnBeginning(primitiveType);
            basicEffect.TextureEnabled = true;
            basicEffect.Texture = texture2D;
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
            if (vertexData.Count != 0) index.PlusAll((short)vertexData.Count);
            vertexData.AddRange(vertex);
            indexData.AddRange(index);
            if (vertexData.Count > short.MaxValue) throw new Exception("Vertices Counts Over 32768");
        }
        public void Draw(VertexInfo vertexInfo)
        {
            Draw(vertexInfo.vertex, vertexInfo.index);
        }
        public static int LengthGusser(int length, PrimitiveType primitiveType)
        {
            return primitiveType == PrimitiveType.TriangleList ? length / 3 : (primitiveType == PrimitiveType.LineList ? length / 2 : (primitiveType == PrimitiveType.TriangleStrip ? length - 2 : length - 1));
        }
        public void DoDraw()
        {
            if (!begin) throw new Exception("Called Draw Before Begin");
            Vertex[] array = vertexData.ToArray();
            int length = indexData.Count == 0 ? vertexData.Count : indexData.Count;
            length = LengthGusser(length, primitiveType);
            basicEffect.CurrentTechnique.Passes[0].Apply(); 
            if (indexData.Count == 0)
            {
                graphicsDevice.DrawUserPrimitives(primitiveType, array, 0, length);
            }
            else
            {
                graphicsDevice.DrawUserIndexedPrimitives(primitiveType, array, 0, array.Length, indexData.ToArray(), 0, length);
            }
            vertexData.Clear();
            indexData.Clear();
        }
        public void End()
        {
            DoDraw();
            begin = false;
        }
        public void Dispose()
        {
            vertexData.Clear();
            indexData.Clear();
            basicEffect = null;
        }
    }
}
