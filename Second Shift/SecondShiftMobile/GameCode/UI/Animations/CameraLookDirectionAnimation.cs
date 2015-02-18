using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondShiftMobile.UI.Animations
{
    public class CameraLookDirectionAnimation : Animation<Vector2>
    {
        public CameraLookDirectionAnimation(Vector2 targetValue)
            :base(Global.Camera, "LookDirection", targetValue)
        {

        }
        protected override Vector2 GetStartingValue(object obj, string valuePath, Vector2 fallbackValue)
        {
            return Global.Camera.BaseLookDirection;
        }
        protected override void SetValue(Vector2 startValue, Vector2 endValue, float progress, FieldAndObjectInfo info)
        {
            Vector2 targ = MyMath.Between(startValue, endValue, progress);
            Global.Camera.SetLookDirection(targ, 0.5f);
        }
    }
}
