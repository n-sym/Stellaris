using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;

namespace Stellaris.Graphics
{
    /// <summary>
    /// 简单的绘制接口，对顶点的绘制或许简化
    /// </summary>
    public class VertexBatch : IDisposable, IDrawAPI
    {
        public GraphicsDevice graphicsDevice;
        public PrimitiveType primitiveType;
        private VertexDrawInfo[] infos;
        private int drawCalls;
        private int drawCount;
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
            Type effectResource = ReflectionHelper.GetMGClass("Microsoft.Xna.Framework.Graphics.EffectResource");
            spriteEffect = new Effect(graphicsDevice, effectResource.GetPublicInstanceMethod("get_Bytecode").Invoke(effectResource.GetPublicStaticField("SpriteEffect").GetValue(null), null) as byte[]);
            _matrixTransform = spriteEffect.Parameters["MatrixTransform"];
            infos = new VertexDrawInfo[1024];
        }
        private void OnBeginning(PrimitiveType primitiveType, BlendState blendState)
        {
            if (_begin) throw new Exception("Called Begin Twice");
            DrawImmediately = true;
            this.primitiveType = primitiveType;
            graphicsDevice.BlendState = blendState;
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            //graphicsDevice.RasterizerState = rasterizerState;
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
        public void Draw(Vertex[] vertex, int[] index, Texture2D texture2D = null)
        {
            if (!_begin) throw new Exception("Called Draw Before Begin");
            if (texture2D == null) texture2D = Ste.Pixel;
            if (DrawImmediately)
            {
                DoDraw(vertex, index, texture2D);
                return;
            }
            if (infos.Length < drawCount) Array.Resize(ref infos, drawCount + 512);
            infos[drawCount] = new VertexDrawInfo(vertex, index, texture2D);
            drawCount++;
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
            Draw(spriteDrawInfo.texture, spriteDrawInfo.position, spriteDrawInfo.sourceRectangle, spriteDrawInfo.color, spriteDrawInfo.rotation, spriteDrawInfo.origin, spriteDrawInfo.scale, spriteDrawInfo.effects, spriteDrawInfo.layerDepth);
        }
        public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
        {
            Vertex[] v = new Vertex[4];
            Vector2 pos = position;
            Vector2 size = sourceRectangle.HasValue ? new Vector2(sourceRectangle.Value.Width, sourceRectangle.Value.Height) :
                new Vector2(texture.Width, texture.Height);
            if (rotation == 0)
            {
                pos -= origin;
                v[0] = new Vertex(pos, color, Vector2.Zero);
                v[1] = new Vertex(pos + new Vector2((int)(size.X * scale.X), 0), color, new Vector2(1, 0));
                v[2] = new Vertex(pos + new Vector2(0, (int)(size.Y * scale.Y)), color, new Vector2(0, 1));
                v[3] = new Vertex(pos + new Vector2((int)(size.X * scale.X), (int)(size.Y * scale.Y)), color, Vector2.One);
            }
            else
            {
                pos -= origin.Rotate(rotation);
                v[0] = new Vertex(pos, color, Vector2.Zero);
                v[1] = new Vertex(pos + new Vector2(size.X * scale.X, 0).Rotate(rotation), color, new Vector2(1, 0));
                v[2] = new Vertex(pos + new Vector2(0, size.Y * scale.Y).Rotate(rotation), color, new Vector2(0, 1));
                v[3] = new Vertex(pos + new Vector2(size.X * scale.X, size.Y * scale.Y).Rotate(rotation), color, Vector2.One);
            }
            if (sourceRectangle.HasValue)
            {
                v[0].TextureCoordinate = new Vector2(sourceRectangle.Value.X / (float)texture.Width,
                    sourceRectangle.Value.Y / (float)texture.Height);
                v[1].TextureCoordinate = v[0].TextureCoordinate + new Vector2(size.X / texture.Width, 0);
                v[2].TextureCoordinate = v[0].TextureCoordinate + new Vector2(0, size.Y / texture.Height);
                v[3].TextureCoordinate = new Vector2(v[1].TextureCoordinate.X, v[2].TextureCoordinate.Y);
            }
            if (effects != SpriteEffects.None)
            {
                if (effects == SpriteEffects.FlipHorizontally)
                {
                    v[0].SwapCoord(ref v[1]);
                    v[2].SwapCoord(ref v[3]);
                }
                else
                {
                    v[0].SwapCoord(ref v[3]);
                    v[2].SwapCoord(ref v[1]);
                }
            }
            Draw(v, primitiveType == PrimitiveType.TriangleStrip ? new int[] { 0, 1, 2, 3 } : new int[] { 0, 1, 2, 1, 3, 2 }, texture);
        }
        /// <summary>
        /// 设置是否立即绘制
        /// </summary>
        public void SetDrawImmediately(bool drawImmediately)
        {
            DrawImmediately = true;
            //primitiveType = PrimitiveType.TriangleList;
        }
        private void ResizeAndAdd(int id, Vertex[] v)
        {
        }
        private void ResizeAndAdd(int id, int[] i)
        {
        }
        public static int LengthGusser(int length, PrimitiveType primitiveType)
        {
            return primitiveType == PrimitiveType.TriangleList ? length / 3 : (primitiveType == PrimitiveType.LineList ? length / 2 : (primitiveType == PrimitiveType.TriangleStrip ? length - 2 : length - 1));
        }
        private void DoDraw(Vertex[] vertices, int[] index, Texture2D texture2D = null)
        {
            if (!_begin) throw new Exception("Called Draw Before Begin");
            if (vertices.Length == 0) return;
            int length = index.Length == 0 ? vertices.Length : index.Length;
            length = LengthGusser(length, primitiveType);
            graphicsDevice.Textures[0] = texture2D;
            drawCalls++;
            graphicsDevice.DrawUserIndexedPrimitives(primitiveType, vertices, 0, vertices.Length, index, 0, length, Vertex.VertexDeclaration);
        }
        ///<summary>
        /// 结束绘制，如果没有设置立即绘制此时将一次性绘制所有顶点
        /// </summary>
        public void End()
        {
            if (!DrawImmediately)
            {
                for (int i = 0; i < drawCount; i++)
                {
                    DoDraw(infos[i].vertices, infos[i].indices, infos[i].texture);
                }
            }
            Debug.WriteLine("DrawCalls:" + drawCalls.ToString());
            drawCalls = 0;
            drawCount = 0;
            _begin = false;
        }
        public void Dispose()
        {
        }
    }
}
