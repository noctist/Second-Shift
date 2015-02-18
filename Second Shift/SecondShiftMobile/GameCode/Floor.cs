using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecondShiftMobile
{
    public class Floor : Obj
    {
        public Floor(Game1 Doc, TextureFrame Tex, float X, float Y, float Z)
            : base(Doc, Tex, X, Y, Z)
        {
            FloorCollisionType = SecondShiftMobile.FloorCollisionType.Bottom | SecondShiftMobile.FloorCollisionType.Top | SecondShiftMobile.FloorCollisionType.Left | SecondShiftMobile.FloorCollisionType.Right;
            Collidable = true;
            IsFloor = true;
            DeactivateOffscreen = true;
            SlowDown = false;
        }
        public override void CallCollision(Vector3 speed)
        {
            //Speed = speed;
            base.CallCollision(speed);
        }
        public override void EarlyUpdate()
        {
            //Speed += (Vector3.Zero - Speed) * 0.1f;
            base.EarlyUpdate();
        }
    }
}
