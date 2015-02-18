using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondShiftMobile.Elements.Winds
{
    public class SphereWind : Wind
    {
        public float PowerSpeed = 0;
        public float Power = 10;
        public float Size = 100;
        public SphereWind(Game1 Doc, float X, float Y, float Z)
            : base(Doc, X, Y, Z)
        {

        }
        public override object GetElementValue(Obj o)
        {
            Vector3 difference = o.Pos - Pos;
            float dist = difference.Length();
            difference.Normalize();
            return (difference * Power) * MathHelper.Clamp((Size - dist) / Size, 0, 1);
        }
        public override void Update()
        {
            base.Update();
            Power += PowerSpeed * PlaySpeed;
            Scale.X = Scale.Y = Size / Texture.Width;
            ZWidth = Size;
        }
        
    }
}
