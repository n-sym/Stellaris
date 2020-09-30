using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Stellaris.Graphics
{
    public class VertexBatch : IDisposable, IDrawAPI
    {
        public GraphicsDevice graphicsDevice;
        public PrimitiveType primitiveType;
        private bool _begin;
        Vertex[] vertexData;
        short[] indexData;
        BasicEffect basicEffect;
        public bool DrawImmediately { get; private set; }
        public VertexBatch(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;
            basicEffect = new BasicEffect(graphicsDevice);
            basicEffect.VertexColorEnabled = true;
            basicEffect.World = Matrix.Identity;
            vertexData = new Vertex[0];
        }
        private void OnBeginning(PrimitiveType primitiveType, BlendState blendState)
        {
            if (_begin) throw new Exception("Called Begin Twice");
            DrawImmediately = true;
            this.primitiveType = primitiveType;
            graphicsDevice.BlendState = blendState;
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            graphicsDevice.RasterizerState = rasterizerState;
            basicEffect.View = Matrix.CreateTranslation(-Ste.Resolution.X / 2, -Ste.Resolution.Y / 2, 0) * Matrix.CreateRotationX(3.141592f);
            basicEffect.Projection = Matrix.CreateOrthographic(Ste.Resolution.X, Ste.Resolution.Y, -100, 100);
            _begin = true;
        }
        public void Begin(PrimitiveType primitiveType = PrimitiveType.TriangleStrip)
        {
            Begin(BlendState.AlphaBlend, primitiveType);
        }
        public void Begin(BlendState blendState, PrimitiveType primitiveType = PrimitiveType.TriangleList)
        {
            OnBeginning(primitiveType, blendState);
            basicEffect.TextureEnabled = false;
        }
        public void Begin(Texture2D texture2D, PrimitiveType primitiveType = PrimitiveType.TriangleList)
        {
            Begin(texture2D, BlendState.AlphaBlend, primitiveType);
        }
        public void Begin(Texture2D texture2D, BlendState blendState, PrimitiveType primitiveType = PrimitiveType.TriangleList)
        {
            OnBeginning(primitiveType, blendState);
            basicEffect.TextureEnabled = true;
            basicEffect.Texture = texture2D;
        }
        public void Draw(Vertex[] vertex, params short[] index)
        {
            if (!_begin) throw new Exception("Called Draw Before Begin");
            if (DrawImmediately)
            {
                DoDraw(vertex, index);
                return;
            }
            if (indexData == null) indexData = new short[0];
            ResizeAndAdd(index);
            ResizeAndAdd(vertex);
            if (vertexData.Length > short.MaxValue) throw new Exception("Vertices Counts Over 32768");
        }
        public void Draw(VertexDrawInfo vertexInfo)
        {
            if (DrawImmediately && vertexInfo.texture != null) ChangeTexture(vertexInfo.texture);
            Draw(vertexInfo.vertices, vertexInfo.indices);
        }
        public void Draw(SpriteDrawInfo spriteDrawInfo)
        {
            Vertex[] v = new Vertex[4];
            Vector2 pos = spriteDrawInfo.position + spriteDrawInfo.origin;
            v[0] = new Vertex(pos, spriteDrawInfo.color, Vector2.Zero);
            if (spriteDrawInfo.rotation == 0)
            {
                v[1] = new Vertex(pos + new Vector2(spriteDrawInfo.texture.Width * spriteDrawInfo.scale.X, 0), spriteDrawInfo.color, new Vector2(1, 0));
                v[2] = new Vertex(pos + new Vector2(0, spriteDrawInfo.texture.Height * spriteDrawInfo.scale.Y), spriteDrawInfo.color, new Vector2(0, 1));
                v[3] = new Vertex(pos + new Vector2(spriteDrawInfo.texture.Width * spriteDrawInfo.scale.X, spriteDrawInfo.texture.Height * spriteDrawInfo.scale.Y), spriteDrawInfo.color, Vector2.One);
            }
            else
            {
                v[1] = new Vertex(pos + new Vector2(spriteDrawInfo.texture.Width * spriteDrawInfo.scale.X, 0).Rotate(spriteDrawInfo.rotation), spriteDrawInfo.color, new Vector2(1, 0));
                v[2] = new Vertex(pos + new Vector2(0, spriteDrawInfo.texture.Height * spriteDrawInfo.scale.Y).Rotate(spriteDrawInfo.rotation), spriteDrawInfo.color, new Vector2(0, 1));
                v[3] = new Vertex(pos + new Vector2(spriteDrawInfo.texture.Width * spriteDrawInfo.scale.X, spriteDrawInfo.texture.Height * spriteDrawInfo.scale.Y).Rotate(spriteDrawInfo.rotation), spriteDrawInfo.color, Vector2.One);
            }
            ChangeTexture(spriteDrawInfo.texture);
            Draw(v, primitiveType == PrimitiveType.TriangleStrip ? new short[] { 0, 1, 2, 3} : new short[] { 0, 1, 2, 1, 3, 2});
        }
        public void ChangeTexture(Texture2D texture2D)
        {
            basicEffect.TextureEnabled = true;
            basicEffect.Texture = texture2D;
        }
        public void SetDrawImmediately(bool drawImmediately)
        {
            if (primitiveType == PrimitiveType.TriangleStrip || primitiveType == PrimitiveType.LineStrip) return;
            else DrawImmediately = drawImmediately;
        }
        public void ResizeAndAdd(Vertex[] v)
        {
            Vertex[] result = new Vertex[vertexData.Length + v.Length];
            Array.Copy(vertexData, result, vertexData.Length);
            Array.Copy(v, 0, result, vertexData.Length, v.Length);
            vertexData = result;
        }
        public void ResizeAndAdd(short[] i)
        {
            i.PlusAll((short)(vertexData.Length));
            short[] result = new short[indexData.Length + i.Length];
            Array.Copy(indexData, result, indexData.Length);
            Array.Copy(i, 0, result, indexData.Length, i.Length);
            indexData = result;
        }
        public static int LengthGusser(int length, PrimitiveType primitiveType)
        {
            return primitiveType == PrimitiveType.TriangleList ? length / 3 : (primitiveType == PrimitiveType.LineList ? length / 2 : (primitiveType == PrimitiveType.TriangleStrip ? length - 2 : length - 1));
        }
        public void DoDraw(Vertex[] vertices, short[] index)
        {
            if (!_begin) throw new Exception("Called Draw Before Begin");
            if (vertices.Length == 0) return;
            int length = index.Length == 0 ? vertices.Length : index.Length;
            length = LengthGusser(length, primitiveType);
            basicEffect.CurrentTechnique.Passes[0].Apply();
            graphicsDevice.DrawUserIndexedPrimitives(primitiveType, vertices, 0, vertices.Length, index, 0, length);
        }
        public void End()
        {
            if (!DrawImmediately) DoDraw(vertexData, indexData);
            vertexData = new Vertex[0];
            indexData = new short[0];
            _begin = false;
        }
        public void Dispose()
        {
            vertexData = new Vertex[0];
            basicEffect.Dispose();
            basicEffect = null;
        }
    }
}
