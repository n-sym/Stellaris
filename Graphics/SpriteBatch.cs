using Microsoft.Xna.Framework.Graphics;

namespace Stellaris.Graphics
{
    public class SpriteBatchS : SpriteBatch, IDrawAPI
    {
        public SpriteBatchS(GraphicsDevice graphicsDevice) : base(graphicsDevice)
        {

        }

        public void Draw(IDrawInfo drawInfo)
        {
            if (drawInfo is SpriteDrawInfo info) Draw(info.texture, info.position, info.sourceRectangle, info.color, info.rotation, info.origin, info.scale, info.effects, info.layerDepth);
        }
        public SpriteBatch ToXNA()
        {
            return this;
        }
    }
}
