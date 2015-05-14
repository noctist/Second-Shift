using System;
using System.Collections.Generic;
using System.Text;

namespace SecondShiftMobile.Behaviours
{
    public class SpeedDissipation : Behaviour
    {
        float maxSpeed, speedMult;
        public SpeedDissipation(float maxSpeed, float speedMultiplier)
        {
            this.maxSpeed = maxSpeed;
            speedMult = speedMultiplier;
        }
        public override void Update(Obj o)
        {
            if (o.Speed.Length() > maxSpeed)
            {
                o.Speed *= speedMult;
            }
            //base.Update(o);
        }
    }
}
