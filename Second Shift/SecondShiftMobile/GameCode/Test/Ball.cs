using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondShiftMobile.Test
{
    public class Ball : PlatformerObj
    {
        Behaviours.Bounce bounce;
        Behaviours.GravityBehaviour gravity;
        public Ball(Game1 Doc, float X, float Y, float Z)
            : base(Doc, Doc.LoadTex("Objects/Ball"), X, Y, Z)
        {
            Gravity = 0.9f;
            Attackable = true;
        }
        public override bool Attacked(Attack attack, Obj obj, Rectangle attackBox, Rectangle interection)
        {

            //var move = MyMath.Rotate(((Pos - interection.Center.ToVector2().ToVector3(Pos.Z)) * attack.Power).ToVector2(), attack.Direction);
            if (attack.KnockBack)
            {
                int dir = 1;
                if (obj.BoundingBox.X > BoundingBox.Center.X)
                    dir = -1;
                float power = attack.KnockBackAmount * 0.5f * (obj.PlaySpeed / PlaySpeed);
                Speed += new Vector3(MyMath.LengthDirX(power * dir, attack.Direction), MyMath.LengthDirY(power, attack.Direction), 0);
            }
            return base.Attacked(attack, obj, attackBox, interection);
        }
        public override void Update()
        {
            if (OnTheGround)
            {
                Speed.X *= 0.95f;
                RotationSpeed = new Vector3(0, 0, GetMoveSpeed().X * 0.85f);
            }
            base.Update();
        }
        public override void LateUpdate()
        {
            /*var colls = GetCollisions();
            foreach (var obj in colls)
            {

            }*/
            base.LateUpdate();
        }
        protected override void OnTheGroundChanged(Vector3 speed)
        {
            if (OnTheGround && speed.Y > 5)
            {
                //Pos.Y -= -100;
                Speed.Y = (speed.Y) * -0.7F;
            }
            base.OnTheGroundChanged(speed);
        }
        protected override void OnAWallChanged(Vector3 speed)
        {
            if (OnAWall)
            {
                Speed.X = -speed.X * 0.7f;
            }
            base.OnAWallChanged(speed);
        }
    }
}
