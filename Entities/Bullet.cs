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
        public Bullet(Texture2D texture, Vector2 position, Vector2 velocity, int damage, Color? colorOver, int oldPosLength = 0, int scale = 1, bool flipped = false)
        {
            this.texture = texture;
            this.size = new Vector2(texture.Width, texture.Height) * scale;
            this.position = position;
            this.velocity = velocity;
            this.damage = damage;
            this.scale = scale;
            this.flipped = flipped;
            if (colorOver.HasValue) this.colorOver = colorOver.Value;
            else this.colorOver = Color.White;
            this.oldPositon = new Vector2[oldPosLength];
            this.oldPosLength = oldPosLength;
        }
        public Bullet(DynamicTexture dynamicTexture, Vector2 position, Vector2 velocity, int damage, Color? colorOver, int oldPosLength = 0, int scale = 1, bool flipped = false)
        {
            this.dynamicTexture = dynamicTexture;
            this.size = new Vector2(dynamicTexture.Width, dynamicTexture.Height) * scale;
            this.position = position;
            this.velocity = velocity;
            this.damage = damage;
            this.scale = scale;
            this.flipped = flipped;
            if (colorOver.HasValue) this.colorOver = colorOver.Value;
            else this.colorOver = Color.White;
            this.oldPositon = new Vector2[oldPosLength];
            this.oldPosLength = oldPosLength;
        }
        public override void BasicBehavior()
        {
            if (timeLeft == 0) active = false;
            if (!active) return;
            timeLeft--;
            for (int i = oldPosLength - 1; i > 0; i--)
            {
                oldPositon[i] = oldPositon[i - 1];
            }
            oldPositon[0] = position;
            base.BasicBehavior();
        }
        public static Bullet NewBullet(Type type, Texture2D texture, Vector2 position, Vector2 velocity, int damage, Color? colorOver, int scale = 1, bool flipped = false)
        {
            Bullet bullet = Activator.CreateInstance(type) as Bullet;
            bullet.size = new Vector2(texture.Width, texture.Height) * scale;
            bullet.texture = texture;
            bullet.position = position;
            bullet.velocity = velocity;
            bullet.damage = damage;
            bullet.scale = scale;
            bullet.flipped = flipped;
            if (colorOver.HasValue) bullet.colorOver = colorOver.Value;
            else bullet.colorOver = Color.White;
            return bullet;
        }
        public static Bullet NewBullet(Type type, DynamicTexture dynamicTexture, Vector2 position, Vector2 velocity, int damage, Color? colorOver, int scale = 1, bool flipped = false)
        {
            Bullet bullet = Activator.CreateInstance(type) as Bullet;
            bullet.dynamicTexture = dynamicTexture;
            bullet.size = new Vector2(dynamicTexture.Width, dynamicTexture.Height) * scale;
            bullet.position = position;
            bullet.velocity = velocity;
            bullet.damage = damage;
            bullet.scale = scale;
            bullet.flipped = flipped;
            if (colorOver.HasValue) bullet.colorOver = colorOver.Value;
            else bullet.colorOver = Color.White;
            return bullet;
        }
    }
}
