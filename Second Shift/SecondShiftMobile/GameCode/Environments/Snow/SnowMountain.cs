using System;
using System.Collections.Generic;
using System.Text;

namespace SecondShiftMobile.Environments.Snow
{
    public class SnowMountain : Test.Mountain
    {
        public SnowMountain(Game1 Doc, float X, float Y, float Z)
            : base(Doc, Doc.LoadTex("Environment/Snow/Mountain"), X, Y, Z)
        {
            DeactivateOffscreen = true;
        }
    }
}
