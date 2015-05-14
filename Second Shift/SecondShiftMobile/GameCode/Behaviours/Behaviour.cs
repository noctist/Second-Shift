using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondShiftMobile.Behaviours
{
    public abstract class Behaviour
    {
        public Behaviour()
        {

        }
        public abstract void Update(Obj o);
        public virtual Vector3 Speed(Obj o, Vector3 speed)
        {
            return speed;
        }
    }
}
