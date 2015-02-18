using System;
using System.Collections.Generic;
using System.Text;

namespace SecondShiftMobile.UI.Animations
{
    public class ThicknessAnimation : Animation<Thickness>
    {
        public ThicknessAnimation(object obj, string path, Thickness targetValue)
            : base(obj, path, targetValue)
        {
            
        }
        protected override void SetValue(Thickness startValue, Thickness endValue, float progress, FieldAndObjectInfo info)
        {
            Thickness th = new Thickness(MyMath.Between(startValue.Left, endValue.Left, progress),
                MyMath.Between(startValue.Top, endValue.Top, progress),
                MyMath.Between(startValue.Right, endValue.Right, progress),
                MyMath.Between(startValue.Bottom, endValue.Bottom, progress));
            info.FieldInfo.SetValue(info.Object, th);
            info.BubbleBackValues();
        }
    }
}
