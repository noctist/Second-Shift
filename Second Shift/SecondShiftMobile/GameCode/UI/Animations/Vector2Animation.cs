using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondShiftMobile.UI.Animations
{
    public class Vector2Animation : Animation<Vector2>
    {
        public Vector2Animation(object obj, string valuePath, Vector2 targetValue)
            : base(obj, valuePath, targetValue)
        {

        }

        protected override void SetValue(Vector2 startValue, Vector2 endValue, float progress, FieldAndObjectInfo info)
        {
            info.FieldInfo.SetValue(info.Object, MyMath.Between(startValue, endValue, progress));
            info.BubbleBackValues();
        }
    }
}
