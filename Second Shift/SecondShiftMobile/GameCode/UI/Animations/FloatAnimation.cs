using System;
using System.Collections.Generic;
using System.Text;

namespace SecondShiftMobile.UI.Animations
{
    public class FloatAnimation : Animation<float>
    {
        public FloatAnimation(object obj, string valuePath, float targetValue)
            : base(obj, valuePath, targetValue)
        {

        }

        protected override void SetValue(float startValue, float endValue, float progress, FieldAndObjectInfo info)
        {
            info.FieldInfo.SetValue(info.Object, MyMath.Between(startValue, endValue, progress));
            info.BubbleBackValues();
        }
    }
}
