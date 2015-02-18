using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondShiftMobile.Behaviours
{
    public class Behaviour
    {
        public Behaviour()
        {

        }
        public virtual void Update(Obj o)
        {

        }

        public virtual Vector3 Speed(Obj o, Vector3 speed)
        {
            return speed;
        }
    }
}
