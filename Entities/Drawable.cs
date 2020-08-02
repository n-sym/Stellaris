using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellaris.Graphics;
using System;

namespace Stellaris.Entities
{
    public abstract class Drawable : Entity
    {
        public DynamicTexture dynamicTexture;
        public Texture2D texture;
        public Color color;
        public float rotation;
        public bool flipped;
        public void Draw(SpriteBatch spriteBatch)
        {
            if (!active || CustomDraw(spriteBatch)) return;
            if (dynamicTexture == null)
            {
                spriteBatch.Draw(texture, position, null, color, rotation, Vector2.Zero, scale, flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            }
            else
            {
                dynamicTexture.Draw(spriteBatch, position, null, color, rotation, Vector2.Zero, scale, flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            }
        }
        public virtual bool CustomDraw(SpriteBatch spriteBatch)
        {
            return false;
        }
    }
}
