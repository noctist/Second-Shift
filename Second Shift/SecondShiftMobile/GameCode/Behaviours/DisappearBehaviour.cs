using System;
using System.Collections.Generic;
using System.Text;

namespace SecondShiftMobile.Behaviours
{
    public class DisappearBehaviour : Behaviour
    {
        float totalFrames, fadeInSpeed, fadeOutSpeed, maxAlpha;
        float frames = 0;
        float alpha = 0;
        public DisappearBehaviour(float totalFrames, float fadeInSpeed, float fadeOutSpeed, float maxAlpha)
        {
            this.totalFrames = totalFrames;
            this.fadeInSpeed = fadeInSpeed;
            this.fadeOutSpeed = fadeOutSpeed;
            this.maxAlpha = maxAlpha;
        }
        public override void Update(Obj o)
        {
            if (frames < totalFrames)
            {
                frames += o.PlaySpeed;
                if (alpha < maxAlpha)
                {
                    alpha += fadeInSpeed * o.PlaySpeed;
                }
                else
                {
                    alpha = maxAlpha;
                }
                
            }
            else if (alpha > 0)
            {
                alpha -= fadeOutSpeed * o.PlaySpeed;
            }
            else
            {
                o.Remove();
            }
            o.Alpha = alpha;
            //base.Update(o);
        }
    }
}
