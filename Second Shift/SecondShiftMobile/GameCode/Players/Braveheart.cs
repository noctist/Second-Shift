using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SecondShiftMobile.Players
{
    class Braveheart : Player
    {
        public Braveheart(Game1 Doc, float X, float Y, float Z)
            :base(Doc, Doc.LoadTex("Characters/Braveheart/Stand"), X, Y, Z)
        {
            StandSprite = Doc.LoadTex("Characters/Braveheart/Stand");
            RunAnimation = Doc.LoadAnimation("Characters/Braveheart/Run", "Run", 0, 5);
            Scale = new Vector2(0.4f);
            BoundingRectangle = new Rectangle(80, 80, 120, 0);
            JumpSpeed = 25;
            WallStickCenter = 128;
            RunFramespeedMultiplier = 0.02f;
            Weight = 6;
        }
    }
}
