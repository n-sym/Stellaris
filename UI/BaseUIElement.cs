using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Stellaris.Graphics;
using System;

namespace Stellaris.UI
{
    /// <summary>
    /// 鼠标状态。BaseUIElement中专指鼠标对于自己的状态。
    /// </summary>
    public enum MouseStatus
    {
        NotHover = 0,
        Hover = 1,
        LeftPressed = 2,
        RightPressed = 3
    }
    /// <summary>
    /// 可选的UI基类
    /// </summary>
    public class BaseUIElement : IUIElement
    {
        public Vector2 position;
        public int width;
        public int height;
        public MouseStatus mouseStatus;
        public Vector2 Size => new Vector2(width, height);
        public Rectangle HitBox => new Rectangle((int)position.X, (int)position.Y, width, height);
        public Action leftClick;
        public Action rightClick;
        public Action hover;
        public BaseUIElement(Vector2 position, int width, int height)
        {
            this.position = position;
            this.width = width;
            this.height = height;
        }
        /// <summary>
        /// 获取鼠标信息，以后也会对触摸进行支持，一般不需要手动调用
        /// </summary>
        protected void ChekCommonMouse()
        {
            CommonMouseState mouseState = Ste.MouseState;
            CommonMouseState lastMouseState = Ste.LastMouseState;
            Rectangle box = HitBox;
            if (mouseState.Left == ButtonState.Released && lastMouseState.Left == ButtonState.Pressed)
            {
                mouseStatus = MouseStatus.LeftPressed;
                LeftClick();
            }
            else if (mouseState.Right == ButtonState.Released && lastMouseState.Right == ButtonState.Pressed)
            {
                mouseStatus = MouseStatus.RightPressed;
                LeftClick();
            }
            else if (box.Contains(mouseState.Position.ToPoint()))
            {
                if (mouseState.Left == ButtonState.Pressed)
                {
                    mouseStatus = MouseStatus.LeftPressed;
                    OnLeftClick();
                    leftClick?.Invoke();
                }
                else if (mouseState.Right == ButtonState.Pressed)
                {
                    mouseStatus = MouseStatus.RightPressed;
                    OnRightClick();
                    rightClick?.Invoke();
                }
                else
                {
                    mouseStatus = MouseStatus.Hover;
                    OnHover();
                    hover?.Invoke();
                }
            }
            else
            {
                mouseStatus = MouseStatus.NotHover;
                NotHover();
            }
        }
        /// <summary>
        /// 需要手动调用的更新
        /// </summary>
        public void Update()
        {
            ChekCommonMouse();
            OnUpdate();
        }
        [Obsolete]
        public void DrawBorder(VertexBatch vertexBatch)
        {
            DrawBorder(vertexBatch);
        }
        [Obsolete]
        public static void DrawBorder(VertexBatch vertexBatch, Vector2 position, Vector2 size)
        {
            //if (vertexBatch.primitiveType == PrimitiveType.TriangleList) vertexBatch.Draw(new Vertex[] { new Vertex(position), new Vertex(position + size.X_Vector()), new Vertex(position + size.Y_Vector()), new Vertex(position + size) }, 0, 1, 2, 1, 2, 3);
            //if (vertexBatch.primitiveType == PrimitiveType.TriangleStrip) vertexBatch.Draw(new Vertex[] { new Vertex(position), new Vertex(position + size.X_Vector()), new Vertex(position + size.Y_Vector()), new Vertex(position + size) }, 0, 1, 2, 3);
            //if (vertexBatch.primitiveType == PrimitiveType.LineList) vertexBatch.Draw(new Vertex[] { new Vertex(position), new Vertex(position + size.X_Vector()), new Vertex(position + size.Y_Vector()), new Vertex(position + size) }, 0, 1, 1, 3, 3, 2, 2, 0);
            //if (vertexBatch.primitiveType == PrimitiveType.LineStrip) vertexBatch.Draw(new Vertex[] { new Vertex(position), new Vertex(position + size.X_Vector()), new Vertex(position + size.Y_Vector()), new Vertex(position + size) }, 0, 1, 3, 2, 0);
        }
        /// <summary>
        /// 需要手动调用的绘制
        /// </summary>
        /// <param name="drawAPI">绘制接口</param>
        public virtual void Draw(IDrawAPI drawAPI)
        {

        }
        /// <summary>
        /// 自动调用的更新，默认为空，用于自定义行为
        /// </summary>
        protected virtual void OnUpdate()
        {

        }
        /// <summary>
        /// 左键单击后调用，默认为空，用于自定义行为
        /// </summary>
        protected virtual void LeftClick()
        {

        }
        /// <summary>
        /// 左键单击时调用，默认为空，用于自定义行为
        /// </summary>
        protected virtual void OnLeftClick()
        {

        }
        /// <summary>
        /// 右键单击后调用，默认为空，用于自定义行为
        /// </summary>
        protected virtual void RightClick()
        {

        }
        /// <summary>
        /// 左键单击时调用，默认为空，用于自定义行为
        /// </summary>
        protected virtual void OnRightClick()
        {

        }
        /// <summary>
        /// 鼠标仅仅悬浮时调用，默认为空，用于自定义行为
        /// </summary>
        protected virtual void OnHover()
        {

        }
        /// <summary>
        /// 鼠标没有悬浮时调用，默认为空，用于自定义行为
        /// </summary>
        protected virtual void NotHover()
        {

        }
    }
}
