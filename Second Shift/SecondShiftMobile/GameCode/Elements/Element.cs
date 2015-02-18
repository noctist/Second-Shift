using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondShiftMobile.Elements
{
    public enum ElementType { Neutral, Wind, Fire, Water, Lightning, Stone, Time }
    public class Element : Obj
    {
        public ElementType ElementType = ElementType.Neutral;
        public Element(Game1 Doc, Texture2D Tex, float X, float Y, float Z)
            :base(Doc, Tex, X, Y, Z)
        {
            Collidable = true;
        }
        public virtual object GetElementValue(Obj o)
        {
            return null;
        }
    }
}
