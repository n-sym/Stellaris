using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Stellaris.Graphics;

namespace Stellaris.UI
{
    public class UIText : UIBase
    {
        public string text;
        public DynamicTextureFont font;
        public float scale;
        public Color color;
        public CenterType centerType;
        public UIText(string text, DynamicTextureFont font)
        {
            this.text = text;
            this.font = font;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            font.DrawString(spriteBatch, text, position, color, centerType, scale);
        }
    }
}
