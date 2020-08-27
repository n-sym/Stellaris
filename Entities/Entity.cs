using Microsoft.Xna.Framework;

namespace Stellaris.Entities
{
    public abstract class Entity
    {
        public Vector2 position;
        public Vector2 velocity;
        public Vector2 size;
        public float scale;
        public Rectangle box;
        public bool active;
        public Vector2 Center => position + size * scale / 2;
        public Point ActualPos => position.ToPoint();
        public Point ActualSize => (size * scale).ToPoint();
        public Entity()
        {
            active = true;
        }
        public void Update()
        {
            if (!active) return;
            BasicBehavior();
            CustomBehavior();
        }
        public virtual void BasicBehavior()
        {
            position += velocity;
            box = new Rectangle(ActualPos, ActualSize);
        }
        public virtual void CustomBehavior()
        {
        }

    }
}
