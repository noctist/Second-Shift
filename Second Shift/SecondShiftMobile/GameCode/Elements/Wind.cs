using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace SecondShiftMobile.Elements
{
    public class Wind : Element
    {
        public Wind(Game1 Doc, float X, float Y, float Z)
            : base(Doc, Textures.Light, X, Y, Z)
        {
            ElementType = Elements.ElementType.Wind;
            Visible = false;
            Alpha = 1;
            ZWidth = 500;
        }
        public override object GetElementValue(Obj o)
        {
            var moveVec = (Pos - PreviousPos);
            float moveL = moveVec.Length();
            Vector3 vec = ((o.Pos - Pos));
            float l = vec.Length();
            vec.Normalize();
            moveVec.Normalize();
            return ((moveVec + vec) * (moveL - MathHelper.Clamp(l / 20, 0, moveL))) / (PlaySpeed);
        }
        public override void Draw()
        {
            base.Draw();
            //doc.DrawSprite(Textures.WhiteBlock, ConvertToScreenRec(BoundingBox), null, Color.White);
        }
    }
}
