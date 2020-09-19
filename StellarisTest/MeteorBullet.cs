using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellaris.Entities;
using System;

namespace Stellaris.Test
{
    class MeteorBullet : Bullet
    {
        public static FlareFx flarefx;
        public static FlareFxAlt flarefxAlt;
        Vector2 v;
        Vector2 end;
        public MeteorBullet(Vector2 position, float radian, int damage) : base(flarefx, position, Vector2.Zero, damage, new Color(68, 112, 223), null, 40)
        {
            timeLeft = 150;
            size = new Vector2(12, 12);
            velocity = Helper.RandomAngleVec(10, Vector2.Zero, radian - 0.85f, radian + 0.85f);
            v = velocity;
            end = Stellaris.MouseState.position;
        }
        public override void CustomBehavior()
        {
            velocity = velocity.LinearTo(velocity * 0.9f, 1.3f - Math.Abs(v.Angle()), 30f);
            velocity = velocity.LinearTo(((end - new Vector2(960, 540)).NormalizeAlt() * 350 + new Vector2(960, 540) - position).NormalizeAlt() * velocity.Length(), 1, 35);
        }
        //public static Color drawColor = new Color(186, 221, 255);
        public static Color drawColor2 = new Color(128, 162, 233);
        public static Color drawColor = new Color(255, 250, 206);
        public override bool CustomDraw(SpriteBatch spriteBatch)
        {
            float t = 1;
            if (timeLeft < 100) t = timeLeft / 100f;
            if (timeLeft > 146) t = 0;
            if (Stellaris.Quality > 2)
            {
                for (int i = 0; i < oldPosLength; i++)
                {
                    if (oldPositon[i] == Vector2.Zero) break;
                    if (oldPositon.TryGetValue(i + 1) == Vector2.Zero) continue;
                    float m = (oldPositon[i] - oldPositon.TryGetValue(i + 1)).Length() * 0.4f;
                    for (int j = 0; j < m; j++)
                    {
                        if (i == 0 && j == 0) flarefx.Draw(spriteBatch, oldPositon[i].LinearTo(oldPositon.TryGetValue(i + 1), j, m), null, drawColor * 1.6f * t, 0.7853f, new Vector2(20, 20), 1.4f * (1f - (float)(Math.Sqrt(i) / Math.Sqrt(oldPosLength))), SpriteEffects.None, 0f);
                        else flarefxAlt.Draw(spriteBatch, oldPositon[i].LinearTo(oldPositon.TryGetValue(i + 1), j, m), null, color.LinearTo(drawColor, (oldPosLength - i) * (oldPosLength - i), oldPosLength * oldPosLength * 1.6f) * (1f - (float)(Math.Sqrt(i) / Math.Sqrt(oldPosLength))) * t, 0.7853f, new Vector2(20, 20), 1.1f * (1f - (float)(Math.Sqrt(i) / Math.Sqrt(oldPosLength))), SpriteEffects.None, 0f);
                    }
                }
            }
            else if (Stellaris.Quality != 0)
            {
                for (int i = 0; i < oldPosLength / 2; i++)
                {
                    if (oldPositon[i] == Vector2.Zero) break;
                    if (oldPositon.TryGetValue(i + 1) == Vector2.Zero) continue;
                    float m = (oldPositon[i] - oldPositon.TryGetValue(i + 1)).Length() * 0.05f;
                    for (int j = 0; j < m; j++)
                    {
                        if (i == 0 && j == 0) flarefx.Draw(spriteBatch, oldPositon[i].LinearTo(oldPositon.TryGetValue(i + 1), j, m), null, drawColor * 1.4f * t, 0.7853f, new Vector2(20, 20), 1.3f * (1f - (float)(Math.Sqrt(i) / Math.Sqrt(oldPosLength))), SpriteEffects.None, 0f);
                        else flarefxAlt.Draw(spriteBatch, oldPositon[i].LinearTo(oldPositon.TryGetValue(i + 1), j, m), null, color.LinearTo(drawColor, (oldPosLength - i) * (oldPosLength - i), oldPosLength * oldPosLength) * (1.55f - (float)(Math.Sqrt(i) / Math.Sqrt(oldPosLength))) * t, 0.7853f, new Vector2(20, 20), 1.8f * (1f - (float)(Math.Sqrt(i) / Math.Sqrt(oldPosLength))), SpriteEffects.None, 0f);
                    }
                }
            }
            else
            {
                for (int i = 0; i < oldPosLength / 4; i++)
                {
                    if (oldPositon[i] == Vector2.Zero) break;
                    if (oldPositon.TryGetValue(i + 1) == Vector2.Zero) continue;
                    if (i == 0) flarefx.Draw(spriteBatch, oldPositon[i], null, drawColor * 1.4f * t, 0.7853f, new Vector2(20, 20), 1.3f * (1f - (float)(Math.Sqrt(i) / Math.Sqrt(oldPosLength))), SpriteEffects.None, 0f);
                    else flarefxAlt.Draw(spriteBatch, oldPositon[i], null, color.LinearTo(drawColor, (oldPosLength - i) * (oldPosLength - i), oldPosLength * oldPosLength) * (1.55f - (float)(Math.Sqrt(i) / Math.Sqrt(oldPosLength))) * t, 0.7853f, new Vector2(20, 20), 1.8f * (1f - (float)(Math.Sqrt(i) / Math.Sqrt(oldPosLength))), SpriteEffects.None, 0f);
                }
            }
            return true;
        }
    }
}
