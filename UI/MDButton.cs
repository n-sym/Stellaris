using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellaris.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stellaris.UI
{
    public class MDButton : BaseUIElement
    {
        public int roundCorner;
        public Color paddingColor;
        public Color borderColor;
        public Color rippleColor;
        public Color shadowColor;
        public Color highLightColor;
        public float quality;
        public float shadowDepth;
        public string text;
        public DynamicSpriteFont font;
        public Color textColor;
        public float textScale;
        private VertexDrawInfo paddingBorderInfo;
        private VertexDrawInfo borderInfo;
        private float rippleTimer;
        private float highLightTimer;
        private Vector2 ripplePos;
        public MDButton(Vector2 position, int width, int height, int roundCorner, Color padding, Color border = default, Color ripple = default, Color shadow = default, Color hightLight = default, float shadowDepth = 1f, float quality = 1f) : base(position, width, height)
        {
            paddingColor = padding;
            borderColor = border;
            rippleColor = ripple;
            shadowColor = shadow;
            highLightColor = hightLight;
            this.roundCorner = roundCorner;
            this.shadowDepth = shadowDepth;
            this.quality = quality;
        }
        public void SetText(string text, DynamicSpriteFont font, Color textColor, float textScale)
        {
            this.text = text;
            this.font = font;
            this.textColor = textColor;
            this.textScale = textScale;
        }
        protected override void OnHover()
        {
            highLightTimer = highLightTimer.LinearTo(1, 1, 10);
        }
        protected override void NotHover()
        {
            if (highLightTimer != 0)
            {
                highLightTimer = highLightTimer.LinearTo(0, 1, 10);
                if (highLightTimer < 0.01) highLightTimer = 0;
            }
        }
        protected override void LeftClick()
        {
            rippleTimer = 1;
            ripplePos = Ste.MousePos - position;
        }
        public void Refresh()
        {
            paddingBorderInfo = null;
            borderInfo = null;
        }
        public override void Draw(IDrawAPI drawAPI)
        {
            if (drawAPI is VertexBatch v)
            {
                if (paddingBorderInfo == null)
                {
                    paddingBorderInfo = Border.GetPaddingBorderDrawInfo(v.primitiveType, position, width, height, roundCorner, paddingColor, quality);
                }
                if (borderColor != default && borderInfo == null)
                {
                    borderInfo = Border.GetBorderDrawInfo(PrimitiveType.LineStrip, position, width, height, roundCorner, borderColor, quality);
                    borderInfo.primitiveType = PrimitiveType.LineStrip;
                }
                if (shadowColor != default)
                {
                    for (int i = 0; i < 6 / shadowDepth; i++)
                    {
                        v.Draw(paddingBorderInfo.Transform(delegate (int z, Vertex v)
                        {
                            return new Vertex(v.Position + new Vector2(0, i * 2), shadowColor * 0.05f);
                        }));
                    }
                }
                v.Draw(paddingBorderInfo);
                font?.DrawString(v, text, position + new Vector2(width, height) / 2, textColor, CenterType.MiddleCenter, textScale);
                if (highLightTimer != 0)
                {
                    v.Draw(paddingBorderInfo.Transform(delegate (int z, Vertex v)
                    {
                        return new Vertex(v.Position, highLightColor * 0.2f * highLightTimer);
                    }));
                }
                if (rippleTimer > 0)
                {
                    Ripple.DrawRound(v, (int)Helper.Linear(2 * width + height, 0, 3 + rippleTimer * 0.75f, 4), position, ripplePos, width, height, Color.Transparent.LinearTo(rippleColor, rippleTimer, 1), roundCorner, 1, quality * 3);
                }
                rippleTimer *= 0.95f;
                if (rippleTimer < 0.01f) rippleTimer = 0;
                if (borderInfo != null)
                {
                    v.Draw(borderInfo);
                }
            }
        }
    }
}
