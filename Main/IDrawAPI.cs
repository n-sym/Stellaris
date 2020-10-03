using Stellaris.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stellaris
{
    /// <summary>
    /// 图形绘制接口
    /// </summary>
    public interface IDrawAPI
    {
        public void Draw(SpriteDrawInfo spriteDrawInfo);
        public void Draw(VertexDrawInfo vertexDrawInfo);
    }
}
