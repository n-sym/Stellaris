using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellaris.Graphics;
using System;

namespace Stellaris.Entities
{
    public class Bullet : Drawable
    {
        public int damage;
        public int timeLeft;
        public Vector2[] oldPositon;
        public int oldPosLength;
        public Character owner;
        private void Initialize(Vector2 position, Vector2 velocity, int damage, Color color, Character owner, int oldPosLength, float scale, bool flipped)
        {
            if (texture != null) this.size = new Vector2(texture.Width, texture.Height) * scale;
            else this.size = new Vector2(dynamicTexture.Width, dynamicTexture.Height) * scale;
            this.position = position;
            this.velocity = velocity;
            this.damage = damage;
            this.scale = scale;
            this.flipped = flipped;
            this.color = color;
            this.owner = owner;
            this.oldPositon = new Vector2[oldPosLength];
            this.oldPosLength = oldPosLength;
            OnCreate();
        }
        public Bullet(Texture2D texture, Vector2 position, Vector2 velocity, int damage, Color color, Character owner, int oldPosLength = 0, float scale = 1, bool flipped = false)
        {
            this.texture = texture;
            Initialize(position, velocity, damage, color, owner, oldPosLength, scale, flipped);
        }
        public Bullet(DynamicTexture dynamicTexture, Vector2 position, Vector2 velocity, int damage, Color color, Character owner, int oldPosLength = 0, float scale = 1, bool flipped = false)
        {
            this.dynamicTexture = dynamicTexture;
            Initialize(position, velocity, damage, color, owner, oldPosLength, scale, flipped);
        }
        public override void BasicBehavior()
        {
            if (timeLeft == 0 && PreInactive())
            {
                active = false;
                OnInactive();
                return;
            }
            timeLeft--;
            for (int i = oldPosLength - 1; i > 0; i--)
            {
                oldPositon[i] = oldPositon[i - 1];
            }
            oldPositon[0] = position;
            CharacterCollision();
            base.BasicBehavior();
        }
        public virtual void CharacterCollision()
        {
        }
        public virtual bool PreInactive()
        {
            return true;
        }
        public virtual void OnCreate()
        {
        }
        public virtual void OnInactive()
        {

        }
    }
}
