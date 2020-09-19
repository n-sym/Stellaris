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
    public class UIBase : IUIElement
    {
        public Vector2 position;
        public float width;
        public float height;
        public MouseStatus mouseStatus;
        public Vector2 Size => new Vector2(width, height);
        public Rectangle HitBox => new Rectangle((int)position.X, (int)position.Y, (int)width, (int)height);
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
                }
                else if (mouseState.Right == ButtonState.Pressed)
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
            OnUpdate();
        }
        public void DrawBorder(VertexBatch vertexBatch)
        {
            DrawBorder(vertexBatch);
        }
        public static void DrawBorder(VertexBatch vertexBatch, Vector2 position, Vector2 size)
        {
            if (vertexBatch.primitiveType == PrimitiveType.TriangleList) vertexBatch.Draw(new Vertex[] { new Vertex(position), new Vertex(position + size.X_Vector()), new Vertex(position + size.Y_Vector()), new Vertex(position + size) }, 0, 1, 2, 1, 2, 3);
            if (vertexBatch.primitiveType == PrimitiveType.TriangleStrip) vertexBatch.Draw(new Vertex[] { new Vertex(position), new Vertex(position + size.X_Vector()), new Vertex(position + size.Y_Vector()), new Vertex(position + size) }, 0, 1, 2, 3);
            if (vertexBatch.primitiveType == PrimitiveType.LineList) vertexBatch.Draw(new Vertex[] { new Vertex(position), new Vertex(position + size.X_Vector()), new Vertex(position + size.Y_Vector()), new Vertex(position + size) }, 0, 1, 1, 3, 3, 2, 2, 0);
            if (vertexBatch.primitiveType == PrimitiveType.LineStrip) vertexBatch.Draw(new Vertex[] { new Vertex(position), new Vertex(position + size.X_Vector()), new Vertex(position + size.Y_Vector()), new Vertex(position + size) }, 0, 1, 3, 2, 0);
        }
        public virtual void Draw(IDrawAPI drawAPI)
        {

        }
        protected virtual void OnUpdate()
        {

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
