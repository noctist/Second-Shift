using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecondShiftMobile
{
    public class VirtualJoystick
    {
        int moveIndex = 0;
        bool movingJoystick = false;
        public float Radius = 200;
        static Vector2 joystickCenter = Vector2.Zero, leftStick = Vector2.Zero;
        static float alpha = 0;
        public bool SnapY = false;
        public bool SnapX = false;
        public static float Alpha
        {
            get { return alpha; }
        }
        public static Vector2 LeftStick
        {
#if MONO
            get { return leftStick; }
#elif PC
            get { return Controls.LeftStick; }
#endif
        }
        public static Vector2 Center
        {
            get { return joystickCenter; }
        }
        public VirtualJoystick()
        {
            
        }
        public void Update()
        {
#if MONO
            for (int i = 0; i < 4; i++)
            {
                if (!movingJoystick)
                {
                    if (Touch.States[i] == TouchScreen.Moved && Touch.PressLocation[i].X < Global.ScreenSize.X / 2 && Touch.TouchTimers[i] > 5)
                    {
                        movingJoystick = true;
                        moveIndex = i;
                        joystickCenter = Touch.MoveLocation[i];
                    }
                }
            }
            if (movingJoystick)
            {
                alpha += Global.FrameSpeed * 0.1f;
                if (SnapY)
                {
                    joystickCenter.Y = Touch.MoveLocation[moveIndex].Y;
                }
                if (SnapX)
                {
                    joystickCenter.X = Touch.MoveLocation[moveIndex].X;
                }
                float dist = Vector2.Distance(Touch.MoveLocation[moveIndex], joystickCenter);
                if (Vector2.Distance(Touch.MoveLocation[moveIndex], joystickCenter) > Radius)
                {
                    Vector2 pos = joystickCenter + ((Touch.MoveLocation[moveIndex] - joystickCenter) * ((dist - Radius) / dist));
                    //joystickCenter += (pos - joystickCenter) * 0.1f;
                    joystickCenter = pos;
                }
                leftStick = (Touch.MoveLocation[moveIndex] - joystickCenter) / Radius;
                leftStick.Y *= -1;
                if (Touch.States[moveIndex] == TouchScreen.None)
                {
                    movingJoystick = false;
                    moveIndex = 0;
                    leftStick = Vector2.Zero;
                }
            }
            else
            {
                alpha -= Global.FrameSpeed * 0.1f;
            }
            alpha = MathHelper.Clamp(alpha, 0, 1);
#endif
        }
    }
}
