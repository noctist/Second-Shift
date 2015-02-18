using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondShiftMobile.UI
{
    public enum MouseType { Mouse, Touch }
    public delegate void MouseEventHandler(Object sender, MouseEventArgs e);
    public class MouseEventArgs : EventArgs
    {
        public Vector2 MousePosition;
        public Vector2 PositionOffset;
        public Vector2 RelativePosition;
        public MouseType MouseType;
        public bool IsDown;
    }
}
