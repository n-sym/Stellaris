using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stellaris.Graphics
{
    public class SpriteDrawInfo : IDrawInfo
    {
        public Texture2D texture;
        public Vector2 position;
        public Rectangle? sourceRectangle;
        public Color color;
        public float rotation;
        public Vector2 origin;
        public Vector2 scale;
        public SpriteEffects effects;
        public float layerDepth;
        public SpriteDrawInfo(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, Vector2 origin, Vector2 scale, float rotation = 0f, SpriteEffects effects = SpriteEffects.None, float layerDepth = 0f)
        {
            Initialize(texture, position, sourceRectangle, color, origin, scale, rotation, effects, layerDepth);
        }
        public SpriteDrawInfo(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, Vector2 origin, float scale, float rotation = 0f, SpriteEffects effects = SpriteEffects.None, float layerDepth = 0f)
        {
            Initialize(texture, position, sourceRectangle, color, origin, new Vector2(scale, scale), rotation, effects, layerDepth);
        }
        public void Initialize(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, Vector2 origin, Vector2 scale, float rotation, SpriteEffects effects, float layerDepth)
        {
            this.texture = texture;
            this.position = position;
            this.sourceRectangle = sourceRectangle;
            this.color = color;
            this.rotation = rotation;
            this.origin = origin;
            this.scale = scale;
            this.effects = effects;
            this.layerDepth = layerDepth;
        }
    }
}
