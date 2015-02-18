using System;
using System.Collections.Generic;
using System.Text;

namespace SecondShiftMobile.UI
{
    public struct Thickness
    {
        public float Left, Top, Right, Bottom;

        public Thickness(float left, float top, float right, float bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }
        public Thickness(float thickness)
            : this(thickness, thickness, thickness, thickness)
        {

        }

        public static bool operator ==(Thickness t, Thickness t2)
        {
            return t.Left == t2.Left && t.Right == t2.Right && t.Top == t2.Top && t.Bottom == t2.Bottom;
        }
        public static bool operator !=(Thickness t, Thickness t2)
        {
            return t.Left != t2.Left || t.Right != t2.Right && t.Top != t2.Top || t.Bottom != t2.Bottom;
        }
    }
}
