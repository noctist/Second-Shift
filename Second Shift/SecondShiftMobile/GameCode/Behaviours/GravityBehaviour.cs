using System;
using System.Collections.Generic;
using System.Text;

namespace SecondShiftMobile.Behaviours
{
    public class GravityBehaviour : Behaviour
    {
        public float Gravity = 0;
        public GravityBehaviour(float gravity)
        {
            this.Gravity = gravity;
        }
        public override void Update(Obj o)
        {
            base.Update(o);
        }
        public override Microsoft.Xna.Framework.Vector3 Speed(Obj o, Microsoft.Xna.Framework.Vector3 speed)
        {
            speed.Y += Gravity * o.PlaySpeed;
            return speed;
        }
    }
}
