using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellaris.Graphics;

namespace Stellaris.Entities
{
    public class Character : Drawable
    {
        public int life;
        public int lifeMax;
        private void Initialize(int life, int lifeMax, Vector2 position)
        {
            this.life = life;
            this.lifeMax = lifeMax;
            this.position = position;
        }
        public Character(Texture2D texture2D, int life, int lifeMax, Vector2 position)
        {
            texture = texture2D;
            Initialize(life, lifeMax, position);
        }
        public Character(DynamicTexture dynamicTexture, int life, int lifeMax, Vector2 position)
        {
            this.dynamicTexture = dynamicTexture;
            Initialize(life, lifeMax, position);
        }
        public override void BasicBehavior()
        {
            if (life <= 0 && PreKill())
            {
                active = false;
                OnKill();
            }
            base.BasicBehavior();
        }
        public virtual bool PreKill()
        {
            return true;
        }
        public virtual void OnKill()
        {
        }
    }
}
