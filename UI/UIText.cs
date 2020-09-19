using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellaris.Graphics;

namespace Stellaris.UI
{
    public class UIText : UIBase
    {
        public string text;
        public DynamicSpriteFont font;
        public float scale;
        public Color color;
        public CenterType centerType;
        public UIText(string text, DynamicSpriteFont font)
        {
            this.text = text;
            this.font = font;
        }
        public override void Draw(IDrawAPI drawAPI)
        {
            if(drawAPI is SpriteBatchS spriteBatch) font.DrawString(spriteBatch, text, position, color, centerType, scale);
        }
    }
}
