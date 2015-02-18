using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondShiftMobile.Test
{
    public class TestGrass : Obj
    {
        public TestGrass(Game1 Doc, float X, float Y, float Z)
            : base(Doc, Doc.LoadTex("Shard"), X, Y, Z)
        {
            Color = Color.Green;
        }
        protected override void DrawObj()
        {
            float div = 1f / 10f;
            float mul = 400;
            for (float i = -1; i <= 1; i+=div)
            {
                for (float o = -1; o <= 1; o+=div)
                {
                    Global.Drawer.DrawRelativeToObj(this, Texture, new Vector3(i * mul, 0, o * mul), Color, Origin.ToVector3());
                }
            }
            base.DrawObj();
        }
    }
}
