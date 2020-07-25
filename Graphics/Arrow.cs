using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Stellaris.Graphics
{
    class ArrowTexture : DynamicTexture
    {
        public ArrowTexture(GraphicsDevice graphicsDevice, int size) : base(graphicsDevice, size, size)
        {

        }
        protected override List<Color[]> Generate()
        {
            var data = new List<Color[]>();
            data.Add(new Color[Width * Height]);
            for (int i = 0; i < data[0].Length; i++)
            {
                var p = IndexToPoint(i);
                if ((p.Y + (Height / Width * 2) * p.X >= Height && p.X <= Width / 2) || (p.Y - (Height / Width * 2) * p.X >= -Height && p.X >= Width / 2)) data[0][i] = Color.White;
            }
            return data;
        }
    }
    class ArrowBodyTexture : DynamicTexture
    {
        public ArrowBodyTexture(GraphicsDevice graphicsDevice, int size) : base(graphicsDevice, size * 2 / 3, 1)
        {

        }
        protected override List<Color[]> Generate()
        {
            var data = new List<Color[]>();
            data.Add(new Color[Width * Height]);
            for (int i = 1; i < Width + 1; i++)
            {
                if (i >= Math.Ceiling(Width / 4d) + 1 && i <= Math.Floor(Width * 3 / 4d)) data[0][i - 1] = Color.White;
            }
            return data;
        }
    }
    public class Arrow
    {
        static ArrowTexture arrowTexture;
        static ArrowBodyTexture arrowBodyTexture;
        public int size;
        public Color color;
        GraphicsDevice graphicsDevice;
        public Arrow(GraphicsDevice graphicsDevice, int size = 10)
        {
            this.graphicsDevice = graphicsDevice;
            this.color = Color.White;
            this.size = size;
        }
        public Arrow(GraphicsDevice graphicsDevice, Color color, int size = 10)
        {
            this.graphicsDevice = graphicsDevice;
            this.color = color;
            this.size = size;
        }
        void EnsureInstancedTexture()
        {
            if (arrowTexture == null || arrowBodyTexture == null)
            {
                arrowTexture = new ArrowTexture(graphicsDevice, size);
                arrowBodyTexture = new ArrowBodyTexture(graphicsDevice, size);
            }
        }
        public void Draw(SpriteBatch spriteBatch, Vector2 position, Vector2 vector, float scale)
        {
            EnsureInstancedTexture();
            arrowTexture.Draw(spriteBatch, position + vector, null, color, vector.Angle() + 1.571f, arrowTexture.Size / 2, scale, SpriteEffects.None, 1f);
            arrowBodyTexture.Draw(spriteBatch, position, null, color, vector.Angle() - 1.571f, new Vector2(arrowBodyTexture.Width / 2, 0), new Vector2(1, vector.Length()) * scale, SpriteEffects.None, 1f);
        }
    }

}
