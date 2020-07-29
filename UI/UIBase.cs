using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Stellaris.Graphics;

namespace Stellaris.UI
{

    public enum MouseStatus
    {
        NotHover = 0,
        Hover = 1,
        LeftPressed = 2,
        RightPressed = 3
    }
    public class UIBase
    {
        public Vector2 postion;
        public float width;
        public float height;
        public MouseStatus mouseStatus;
        public Vector2 Size => new Vector2(width, height);
        public Rectangle Box => new Rectangle((int)postion.X, (int)postion.Y, (int)width, (int)height);
        protected void ChekCommonMouse()
        {
            CommonMouseState mouseState = Common.MouseState;
            CommonMouseState lastMouseState = Common.LastMouseState;
            Rectangle box = Box;
            if (mouseState.left == ButtonState.Released && lastMouseState.left == ButtonState.Pressed)
            {
                mouseStatus = MouseStatus.LeftPressed;
                LeftClick();
            }
            else if (mouseState.right == ButtonState.Released && lastMouseState.right == ButtonState.Pressed)
            {
                mouseStatus = MouseStatus.RightPressed;
                LeftClick();
            }
            else if (box.Contains(mouseState.position.ToPoint()))
            {
                if (mouseState.left == ButtonState.Pressed)
                {
                    mouseStatus = MouseStatus.LeftPressed;
                    OnLeftClick();
                }
                else if (mouseState.right == ButtonState.Pressed)
                {
                    mouseStatus = MouseStatus.RightPressed;
                    OnRightClick();
                }
                else
                {
                    mouseStatus = MouseStatus.Hover;
                    OnHover();
                }
            }
            else
            {
                mouseStatus = MouseStatus.NotHover;
                NotHover();
            }
        }
        public void Update()
        {
            ChekCommonMouse();
        }
        public void DrawBorder(VertexBatch vertexBatch)
        {
            Vector2 size = Size;
            if (vertexBatch.primitiveType == PrimitiveType.TriangleList) vertexBatch.Draw(new Vertex[] { new Vertex(postion), new Vertex(postion + size.X_Vector()), new Vertex(postion + size.Y_Vector()), new Vertex(postion + size) }, 0, 1, 2, 1, 2, 3);
            if (vertexBatch.primitiveType == PrimitiveType.TriangleStrip) vertexBatch.Draw(new Vertex[] { new Vertex(postion), new Vertex(postion + size.X_Vector()), new Vertex(postion + size.Y_Vector()), new Vertex(postion + size) }, 0, 1, 2, 3);
            if (vertexBatch.primitiveType == PrimitiveType.LineList) vertexBatch.Draw(new Vertex[] { new Vertex(postion), new Vertex(postion + size.X_Vector()), new Vertex(postion + size.Y_Vector()), new Vertex(postion + size) }, 0, 1, 1, 3, 3, 2, 2, 0);
            if (vertexBatch.primitiveType == PrimitiveType.LineStrip) vertexBatch.Draw(new Vertex[] { new Vertex(postion), new Vertex(postion + size.X_Vector()), new Vertex(postion + size.Y_Vector()), new Vertex(postion + size) }, 0, 1, 3, 2, 0);
        }
        protected virtual void LeftClick()
        {

        }
        protected virtual void OnLeftClick()
        {

        }
        protected virtual void RightClick()
        {

        }
        protected virtual void OnRightClick()
        {

        }
        protected virtual void OnHover()
        {

        }
        protected virtual void NotHover()
        {

        }
    }
}
