using Stellaris.Graphics;

namespace Stellaris
{
    /// <summary>
    /// 图形绘制接口
    /// </summary>
    public interface IDrawAPI
    {
        /// <summary>
        /// 通过SpriteDrawInfo进行绘制
        /// </summary>
        public void Draw(SpriteDrawInfo spriteDrawInfo);
        /// <summary>
        /// 通过VertexDrawInfo进行绘制
        /// </summary>
        public void Draw(VertexDrawInfo vertexDrawInfo);
    }
}
