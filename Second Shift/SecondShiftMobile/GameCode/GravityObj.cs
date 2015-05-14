using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecondShiftMobile
{
    public class GravityObj : Obj
    {
        public const float NormalGravity = 0.7f;
        /// <summary>
        /// The acceleration the object will fall at
        /// </summary>
        public float Gravity = 1;

        /// <summary>
        /// This sets whether or not to use gravity
        /// </summary>
        public bool AllowGravity = false;
        public GravityObj(Game1 Doc, TextureFrame Tex, float X, float Y, float Z)
            : base(Doc, Tex, X, Y, Z)
        {

        }

        public override void EarlyUpdate()
        {
            if (AllowGravity)
            {
                Speed.Y += NormalGravity * GetGravity() * PlaySpeed;
            }

            //Helper.Write(gravity);
            base.EarlyUpdate();
        }

        public virtual float GetGravity()
        {
            return Gravity;
        }
    }
}
