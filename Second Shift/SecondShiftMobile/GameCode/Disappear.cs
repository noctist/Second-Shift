using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SecondShiftMobile
{
    public class Disappear : Obj
    {
        public float MaxAlpha = 0.7f;
        public float FadeInSpeed = 0.05f;
        public float FadeOutSpeed = -0.005f;
        public float Lifetime = 90f;
        public float LifetimeWait = 0;
        bool waitOver = false;
        float timer = 0;
        public float LifetimePercent
        {
            get
            {
                return MathHelper.Clamp(timer / Lifetime, 0, 1);
            }
        }
        public Disappear(Game1 Doc, TextureFrame Tex, float X, float Y, float Z)
            : base(Doc, Tex, X, Y, Z)
        {
            Alpha = 0;
        }
        public override void ParallelUpdate()
        {
            timer += PlaySpeed;
            if (timer > LifetimeWait && !waitOver)
            {
                waitOver = true;
                timer = 0;

                AlphaSpeed = FadeInSpeed;
            }
            if (waitOver)
            {
                if (timer < Lifetime)
                {
                    AlphaSpeed = FadeInSpeed;
                    if (Alpha >= MaxAlpha)
                    {
                        AlphaSpeed = 0;
                        Alpha = MaxAlpha;
                    }
                }
                else
                {
                    AlphaSpeed = FadeOutSpeed;
                    if (Alpha < 0)
                    {
                        Remove();
                    }
                }
            }
            base.ParallelUpdate();
        }
    }
}
