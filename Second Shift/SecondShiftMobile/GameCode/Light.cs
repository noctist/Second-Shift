using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecondShiftMobile
{
    public class Light : Obj
    {
        public Light(Game1 Doc, float X, float Y, float Z)
            :base(Doc, Doc.Content.Load<Texture2D>("Light"), X, Y, Z)
        {
            Bloom = 1;
        }
    }
}
