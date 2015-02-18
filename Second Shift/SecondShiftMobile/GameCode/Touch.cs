using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
namespace SecondShiftMobile
{
    public class Touch
    {
        public static TouchCollection Touches = new TouchCollection();
        public static int[] TouchID = new int[4] { -1, -1, -1, -1 };
        static Vector2[] touchPos = new Vector2[4] { -Vector2.One, -Vector2.One, -Vector2.One, -Vector2.One };
        public static TouchScreen[] States = new TouchScreen[4] { 0, 0, 0, 0 };
        TouchScreen[] _states = new TouchScreen[4] { 0, 0, 0, 0 };
        TouchScreen[] _states2 = new TouchScreen[4] { 0, 0, 0, 0 };
        public static float[] TouchTimers = new float[4] { 0, 0, 0, 0 };
        public static Vector2[] PressLocation = new Vector2[4] { Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero };
        public static Vector2[] MoveLocation = new Vector2[4] { Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero };
        public static Vector2[] MoveLocation2 = new Vector2[4] { Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero };
        public static GameOrient Orient = GameOrient.Left;
        public static Vector2 ScreenSize = new Vector2(1280, 720);
        public static Vector2 WindowSize = new Vector2(1280, 800);
        public Touch()
        {
            Helper.Write(TouchPanel.DisplayHeight);
        }
        public void update()
        {
            
            Touches = TouchPanel.GetState();
            Vector2 touchVec; 
            touchVec = new Vector2(TouchPanel.DisplayHeight, TouchPanel.DisplayWidth);

            //Helper.Write(Touches.Count);
            for (int i = 0; i < 4; i++)
            {
                _states2[i] = _states[i];
                MoveLocation[i] = MoveLocation2[i];
                if (i < Touches.Count)
                {
                    //Helper.Write("Touch ID " + i + ": " + Touches[i].Id);
                    int index = i;
                    bool hasIndex = false;
                    for (int ii = 0; ii < 4; ii++)
                    {
                        if (TouchID[ii] == Touches[i].Id)
                        {
                            index = ii;
                            hasIndex = true;
                            break;
                        }
                    }
                    if (!hasIndex && TouchID[i] != -1)
                    {
                        for (int ii = 0; ii < 4; ii++)
                        {
                            if (TouchID[ii] == -1)
                            {
                                index = ii;
                                break;
                            }
                        }
                    }

                    touchPos[index] = Touches[i].Position;
                    if (Touches[i].State == TouchLocationState.Moved)
                    {
                        TouchID[index] = Touches[i].Id;
                        TouchTimers[index] += Global.FrameSpeed;
                        _states[index] = TouchScreen.Pressed;
                    }
                    if (Touches[i].State == TouchLocationState.Pressed)
                    {
                        TouchID[index] = Touches[i].Id;
                        PressLocation[index] = Touches[i].Position.Flip(Orient, ScreenSize, touchVec);
                        MoveLocation2[index] = Touches[i].Position.Flip(Orient, ScreenSize, touchVec);
                        _states[index] = TouchScreen.Pressed;
                    }
                    if (Touches[i].State == TouchLocationState.Released)
                    {
                        _states[index] = TouchScreen.None;
                        TouchTimers[index] += Global.FrameSpeed;
                        //TouchID[index] = -1;
                    }
                    //Helper.Write("" + i + ": " + Touches[i].Position);
                }
                else
                {

                }
            }
            for (int i = 0; i < 4; i++)
            {
                if (TouchID[i] == -1)
                {
                    _states[i] = TouchScreen.None;
                }
            }

            for (int i = 0; i < 4; i++)
            {
                
                if (_states[i] == TouchScreen.Pressed && _states2[i] == TouchScreen.Pressed)
                {
                    States[i] = TouchScreen.Moved;
                    MoveLocation2[i] = touchPos[i].Flip(Orient, ScreenSize, touchVec);
                }
                if (_states[i] == TouchScreen.None && _states2[i] == TouchScreen.None)
                {
                    TouchID[i] = -1;
                    TouchTimers[i] = 0;
                    PressLocation[i] = -Vector2.One;
                    MoveLocation2[i] = -Vector2.One;
                    States[i] = TouchScreen.None;
                }
                if (_states[i] == TouchScreen.None && _states2[i] == TouchScreen.Pressed)
                {
                    States[i] = TouchScreen.Released;
                }
                if (_states[i] == TouchScreen.Pressed && _states2[i] == TouchScreen.None)
                {
                    States[i] = TouchScreen.Pressed;
                    PressLocation[i] = touchPos[i].Flip(Orient, ScreenSize, touchVec);
                    MoveLocation2[i] = touchPos[i].Flip(Orient, ScreenSize, touchVec);
                }
            }
        }
    }
}
