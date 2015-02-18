using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondShiftMobile.Behaviours
{
    public class DirectionToRotation : Behaviour
    {
        float scale = 1;
        public bool UseCamera = true;
        public DirectionToRotation(float scale)
        {
            this.scale = scale;
        }
        public override void Update(Obj o)
        {
            base.Update(o);
            if (o.PreviousPos != Vector3.Zero)
            {
                Vector2 currPos, lastPos;
                if (UseCamera)
                {
                    currPos = Global.Camera.GetScreenPosition(o.Pos);
                    lastPos = Global.Camera.GetScreenPosition(o.PreviousPos);
                }
                else
                {
                    currPos = Global.Camera.GetScreenPosition(o.Pos);
                    lastPos = Global.Camera.GetScreenPosition(o.PreviousPos);
                }
                if (currPos != lastPos)
                {
                    o.Scale.X = MathHelper.Clamp(((currPos - lastPos).Length() / o.Texture.Width) * scale / o.PlaySpeed, o.Scale.Y, float.MaxValue);
                }
                o.Rotation.Z = MathHelper.ToDegrees(MyMath.Direction(lastPos, currPos));
                
            }
            
        }
    }
}
