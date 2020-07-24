using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Stellaris.Entities;
using Stellaris.Graphics;
using Stellaris.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stellaris.Test
{
    class MeteorBullet : Bullet
    {
        public static FlareFx flarefx;
        public static FlareFxAlt flarefxAlt;
        Vector2 v;
        Vector2 end;
        public MeteorBullet(Vector2 position, float radian, int damage) : base(flarefx, position, Vector2.Zero, damage, new Color(86, 111, 193), 40)
        {
            timeLeft = 150;
            size = new Vector2(12, 12);
            velocity = Helper.RandomAngleVec(10, radian - 0.85f, radian + 0.85f);
            v = velocity;
            end = Common.MouseState.position;
        }
        public override void CustomBehavior()
        {
            velocity = velocity.LinearInterpolationTo(velocity * 0.9f, 1.3f - Math.Abs(v.Angle()), 30f);
            velocity = velocity.LinearInterpolationTo(((end - new Vector2(960, 540)).NormalizeAlt() * 350 + new Vector2(960, 540) - position).NormalizeAlt() * velocity.Length(), 1, 35);
        }
        public override bool CustomDraw(SpriteBatch spriteBatch)
        {
            float t = 1;
            if (timeLeft < 100) t = timeLeft / 100f;
            if (timeLeft > 146) t = 0;
            if (Common.Quality > 2)
            {
                for (int i = 0; i < oldPosLength; i++)
                {
                    if (oldPositon[i] == Vector2.Zero) break;
                    if (oldPositon.TryGetValue(i + 1) == Vector2.Zero) continue;
                    float m = (oldPositon[i] - oldPositon.TryGetValue(i + 1)).Length() * 0.5f;
                    for (int j = 0; j < m; j++)
                    {
                        if (i == 0 && j == 0) flarefx.Draw(spriteBatch, oldPositon[i].LinearInterpolationTo(oldPositon.TryGetValue(i + 1), j, m), null, colorOver.LinearInterpolationTo(new Color(166, 201, 255), oldPosLength - i, oldPosLength) * (1.4f - (float)(Math.Sqrt(i) / Math.Sqrt(oldPosLength))) * t, 0.7853f, new Vector2(20, 20), 1.4f * (1f - (float)(Math.Sqrt(i) / Math.Sqrt(oldPosLength))), SpriteEffects.None, 0f);
                        else flarefxAlt.Draw(spriteBatch, oldPositon[i].LinearInterpolationTo(oldPositon.TryGetValue(i + 1), j, m), null, colorOver.LinearInterpolationTo(new Color(136, 171, 225), (oldPosLength - i) * (oldPosLength - i), oldPosLength * oldPosLength) * (1f - (float)(Math.Sqrt(i) / Math.Sqrt(oldPosLength))) * t * 0.9f, 0.7853f, new Vector2(20, 20), 1.1f * (1f - (float)(Math.Sqrt(i) / Math.Sqrt(oldPosLength))), SpriteEffects.None, 0f);
                    }
                }
            }
            else if (Common.Quality != 0)
            {
                for (int i = 0; i < oldPosLength / 2; i++)
                {
                    if (oldPositon[i] == Vector2.Zero) break;
                    if (oldPositon.TryGetValue(i + 1) == Vector2.Zero) continue;
                    float m = (oldPositon[i] - oldPositon.TryGetValue(i + 1)).Length() * 0.05f;
                    for (int j = 0; j < m; j++)
                    {
                        if (i == 0 && j == 0) flarefx.Draw(spriteBatch, oldPositon[i].LinearInterpolationTo(oldPositon.TryGetValue(i + 1), j, m), null, colorOver.LinearInterpolationTo(new Color(166, 201, 255), oldPosLength - i, oldPosLength) * (1.4f - (float)(Math.Sqrt(i) / Math.Sqrt(oldPosLength))) * t, 0.7853f, new Vector2(20, 20), 1.3f * (1f - (float)(Math.Sqrt(i) / Math.Sqrt(oldPosLength))), SpriteEffects.None, 0f);
                        else flarefxAlt.Draw(spriteBatch, oldPositon[i].LinearInterpolationTo(oldPositon.TryGetValue(i + 1), j, m), null, colorOver.LinearInterpolationTo(new Color(136, 171, 225), (oldPosLength - i) * (oldPosLength - i), oldPosLength * oldPosLength) * (1.55f - (float)(Math.Sqrt(i) / Math.Sqrt(oldPosLength))) * t, 0.7853f, new Vector2(20, 20), 1.8f * (1f - (float)(Math.Sqrt(i) / Math.Sqrt(oldPosLength))), SpriteEffects.None, 0f);
                    }
                }
            }
            else
            {
                for (int i = 0; i < oldPosLength / 4; i++)
                {
                    if (oldPositon[i] == Vector2.Zero) break;
                    if (oldPositon.TryGetValue(i + 1) == Vector2.Zero) continue;
                    if (i == 0) flarefx.Draw(spriteBatch, oldPositon[i], null, colorOver.LinearInterpolationTo(new Color(166, 201, 255), oldPosLength - i, oldPosLength) * (1.4f - (float)(Math.Sqrt(i) / Math.Sqrt(oldPosLength))) * t, 0.7853f, new Vector2(20, 20), 1.3f * (1f - (float)(Math.Sqrt(i) / Math.Sqrt(oldPosLength))), SpriteEffects.None, 0f);
                    else flarefxAlt.Draw(spriteBatch, oldPositon[i], null, colorOver.LinearInterpolationTo(new Color(136, 171, 225), (oldPosLength - i) * (oldPosLength - i), oldPosLength * oldPosLength) * (1.55f - (float)(Math.Sqrt(i) / Math.Sqrt(oldPosLength))) * t, 0.7853f, new Vector2(20, 20), 1.8f * (1f - (float)(Math.Sqrt(i) / Math.Sqrt(oldPosLength))), SpriteEffects.None, 0f);
                }
            }
            return true;
        }
    }
}
