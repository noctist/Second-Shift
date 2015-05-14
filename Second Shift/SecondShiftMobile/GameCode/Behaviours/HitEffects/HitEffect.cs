using System;
using System.Collections.Generic;
using System.Text;

namespace SecondShiftMobile.Behaviours.HitEffects
{
    public class HitEffect : Behaviour
    {
        float scale, range;
        public HitEffect(float scale, float range)
        {
            this.scale = scale;
            this.range = range;
        }

        public override void Update(Obj o)
        {
            float rval = MyMath.RandomRange(-range, range);
            o.Scale.X = o.Scale.Y = scale + rval;
        }
    }
}
