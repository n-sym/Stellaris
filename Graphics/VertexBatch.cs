using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellaris.Main;
using System;

namespace Stellaris.Graphics
{
    /// <summary>
    /// 简单的绘制接口，对顶点的绘制或许简化
    /// </summary>
    public class VertexBatch : IDisposable, IDrawAPI
    {
        public GraphicsDevice graphicsDevice;
        public PrimitiveType primitiveType;
        private Vertex[] vertexData;
        private short[] indexData;
        private bool _begin;
        private Effect spriteEffect;
        private EffectParameter _matrixTransform;
        private Matrix _projection;
        public bool DrawImmediately { get; private set; }
        /// <summary>
        /// 构建一个VertexBatch
        /// </summary>
        public VertexBatch(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;
            vertexData = new Vertex[0];
            Type effectResource = ReflectionHelper.GetMGClass("Microsoft.Xna.Framework.Graphics.EffectResource");
            spriteEffect = new Effect(graphicsDevice, effectResource.GetPublicInstanceMethod("get_Bytecode").Invoke(effectResource.GetPublicStaticField("SpriteEffect").GetValue(null), null) as byte[]);
            _matrixTransform = spriteEffect.Parameters["MatrixTransform"];
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
            Viewport viewport = graphicsDevice.Viewport;
            Matrix.CreateOrthographicOffCenter(0f, viewport.Width, viewport.Height, 0f, 0f, -1f, out _projection);
            _matrixTransform.SetValue(_projection);
            spriteEffect.CurrentTechnique.Passes[0].Apply();
            _begin = true;
        }
        /// <summary>
        /// 开始准备绘制
        /// </summary>
        /// <param name="primitiveType">所用顶点类型</param>
        public void Begin(PrimitiveType primitiveType = PrimitiveType.TriangleStrip)
        {
            Begin(BlendState.AlphaBlend, primitiveType);
        }
        /// <summary>
        /// 开始准备绘制
        /// </summary>
        /// <param name="blendState">混合模式</param>
        /// <param name="primitiveType">所用顶点类型</param>
        public void Begin(BlendState blendState, PrimitiveType primitiveType = PrimitiveType.TriangleList)
        {
            OnBeginning(primitiveType, blendState);
        }
        /// <summary>
        /// 开始顶点绘制
        /// </summary>
        /// <param name="vertex">顶点</param>
        /// <param name="index">索引</param>
        /// <param name="texture2D">材质</param>
        public void Draw(Vertex[] vertex, short[] index, Texture2D texture2D = null)
        {
            if (!_begin) throw new Exception("Called Draw Before Begin");
            if (DrawImmediately)
            {
                DoDraw(vertex, index, texture2D);
                return;
            }
            if (indexData == null) indexData = new short[0];
            ResizeAndAdd(index);
            ResizeAndAdd(vertex);
            if (vertexData.Length > short.MaxValue) throw new Exception("Vertices Counts Over 32768");
        }
        /// <summary>
        /// 开始顶点绘制
        /// </summary>
        /// <param name="vertexInfo">顶点绘制信息</param>
        public void Draw(VertexDrawInfo vertexInfo)
        {
            if (DrawImmediately)
            {
                PrimitiveType cache = primitiveType;
                if (vertexInfo.primitiveType != null) primitiveType = vertexInfo.primitiveType.Value;
                Draw(vertexInfo.vertices, vertexInfo.indices, vertexInfo.texture);
                primitiveType = cache;
                return;
            }
            Draw(vertexInfo.vertices, vertexInfo.indices);
        }
        /// <summary>
        /// 开始贴图绘制
        /// </summary>
        /// <param name="spriteDrawInfo">贴图绘制信息</param>
        public void Draw(SpriteDrawInfo spriteDrawInfo)
        {
            Vertex[] v = new Vertex[4];
            Vector2 pos = spriteDrawInfo.position;
            if (spriteDrawInfo.rotation == 0)
            {
                pos -= spriteDrawInfo.origin;
                v[0] = new Vertex(pos, spriteDrawInfo.color, Vector2.Zero);
                v[1] = new Vertex(pos + new Vector2(spriteDrawInfo.texture.Width * spriteDrawInfo.scale.X, 0), spriteDrawInfo.color, new Vector2(1, 0));
                v[2] = new Vertex(pos + new Vector2(0, spriteDrawInfo.texture.Height * spriteDrawInfo.scale.Y), spriteDrawInfo.color, new Vector2(0, 1));
                v[3] = new Vertex(pos + new Vector2(spriteDrawInfo.texture.Width * spriteDrawInfo.scale.X, spriteDrawInfo.texture.Height * spriteDrawInfo.scale.Y), spriteDrawInfo.color, Vector2.One);
            }
            else
            {
                pos -= spriteDrawInfo.origin.Rotate(spriteDrawInfo.rotation);
                v[0] = new Vertex(pos, spriteDrawInfo.color, Vector2.Zero);
                v[1] = new Vertex(pos + new Vector2(spriteDrawInfo.texture.Width * spriteDrawInfo.scale.X, 0).Rotate(spriteDrawInfo.rotation), spriteDrawInfo.color, new Vector2(1, 0));
                v[2] = new Vertex(pos + new Vector2(0, spriteDrawInfo.texture.Height * spriteDrawInfo.scale.Y).Rotate(spriteDrawInfo.rotation), spriteDrawInfo.color, new Vector2(0, 1));
                v[3] = new Vertex(pos + new Vector2(spriteDrawInfo.texture.Width * spriteDrawInfo.scale.X, spriteDrawInfo.texture.Height * spriteDrawInfo.scale.Y).Rotate(spriteDrawInfo.rotation), spriteDrawInfo.color, Vector2.One);
            }
            DoDraw(v, primitiveType == PrimitiveType.TriangleStrip ? new short[] { 0, 1, 2, 3 } : new short[] { 0, 1, 2, 1, 3, 2 }, spriteDrawInfo.texture);
        }
        /// <summary>
        /// 设置是否立即绘制
        /// </summary>
        public void SetDrawImmediately(bool drawImmediately)
        {
            if (primitiveType == PrimitiveType.TriangleStrip || primitiveType == PrimitiveType.LineStrip) return;
            else DrawImmediately = drawImmediately;
        }
        private void ResizeAndAdd(Vertex[] v)
        {
            Vertex[] result = new Vertex[vertexData.Length + v.Length];
            Array.Copy(vertexData, result, vertexData.Length);
            Array.Copy(v, 0, result, vertexData.Length, v.Length);
            vertexData = result;
        }
        private void ResizeAndAdd(short[] i)
        {
            i.Plus((short)(vertexData.Length));
            short[] result = new short[indexData.Length + i.Length];
            Array.Copy(indexData, result, indexData.Length);
            Array.Copy(i, 0, result, indexData.Length, i.Length);
            indexData = result;
        }
        public static int LengthGusser(int length, PrimitiveType primitiveType)
        {
            return primitiveType == PrimitiveType.TriangleList ? length / 3 : (primitiveType == PrimitiveType.LineList ? length / 2 : (primitiveType == PrimitiveType.TriangleStrip ? length - 2 : length - 1));
        }
        private void DoDraw(Vertex[] vertices, short[] index, Texture2D texture2D = null)
        {
            if (!_begin) throw new Exception("Called Draw Before Begin");
            if (vertices.Length == 0) return;
            int length = index.Length == 0 ? vertices.Length : index.Length;
            length = LengthGusser(length, primitiveType);
            if (texture2D != null) graphicsDevice.Textures[0] = texture2D;
            else graphicsDevice.Textures[0] = Ste.Pixel;
            //var p = typeof(GraphicsMetrics).GetField("_spriteCount", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            //p.SetValue(graphicsDevice.Metrics, (long)p.GetValue(graphicsDevice.Metrics) + 1);
            graphicsDevice.DrawUserIndexedPrimitives(primitiveType, vertices, 0, vertices.Length, index, 0, length, Vertex.VertexDeclaration);
            graphicsDevice.Textures[0] = null;
        }
        /// <summary>
        /// 结束绘制，如果没有设置立即绘制此时将一次性绘制所有顶点
        /// </summary>
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
        }
    }
}
