using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace SecondShiftMobile.Enemies
{
    public class HomingMissle : Obj
    {
        public Player player;
        float speed = 0;
        SmokeEmitter sm;
        float dir;
        Behaviours.Behaviour dirToRot;
        public HomingMissle(Game1 Doc, float X, float Y, float Z)
            : base(Doc, Doc.LoadTex("Rytard Missle"), X, Y, Z)
        {
            Texture.Width = Texture.Height = 100;
            Origin = new Vector2(13, 51);
            sm = new SmokeEmitter(Doc, X, Y, Z)
            {
                Target = this,
                StartScale = 0.25f,
                MaxTimer = 2f,
                Lifetime = 30,
                TargetSpeedMultiplier = 0.5f,
                DeactivateOffscreen = false
            };
            sm.Emitted += sm_Emitted;
            sm.EmitterSize.Z = 0;
            sm.SmokeSpeed = Vector3.Zero;
            sm.SmokeSpeedRange = new Vector3(2);
            //AddBehaviour(new Behaviours.GravityBehaviour(0.09f));
            dirToRot = new Behaviours.DirectionToRotation(20);
            Collidable = true;
        }

        void sm_Emitted(object sender, SmokeEmittedEventArgs e)
        {
            e.Fire.AddBehaviour(dirToRot);
            e.Fire.RotationSpeed = Vector3.Zero;
            e.Fire.FadeInSpeed = 1;
            e.Smoke.FadeInSpeed = 0.1f;
            e.Smoke.AddBehaviour(dirToRot);
            e.Smoke.RotationSpeed = Vector3.Zero;
            e.Fire.Speed = new Vector3(MyMath.Rotate(e.Fire.Speed.ToVector2(), Rotation.Z ), e.Fire.Speed.Z);
            e.Smoke.Speed = new Vector3(MyMath.Rotate(e.Smoke.Speed.ToVector2(), Rotation.Z ), e.Smoke.Speed.Z);
            e.Smoke.AffectedByWind = e.Fire.AffectedByWind = false;
            //e.Smoke.ScaleSpeed *= 2;
        }

        public override void ParallelUpdate()
        {
            //Global.Camera.Target = this;
            if (player == null)
                player = doc.FindObject<Player>();
            base.ParallelUpdate();
        }
        public override void Remove()
        {
            sm.Remove();
            base.Remove();
        }
        public override void Update()
        {
            
            sm.SmokeSpeed = -Speed;
            if (player != null)
            {
                float dist = (Pos - player.Pos).Length();
                if ((Pos - player.Pos).Length() < 100000)
                {
                    float dir2 = MathHelper.ToDegrees(MyMath.Direction(Pos.ToVector2(), player.Pos.ToVector2()));

                    Rotation.Z += MathHelper.Clamp(MyMath.ClosestAngleDifference(Rotation.Z, dir2) * 0.05f * PlaySpeed, -10, 10);

                    dir += MyMath.ClosestAngleDifference(dir, Rotation.Z) * 0.01f * PlaySpeed;
                    //
                    float speedTarget = MathHelper.Clamp(player.Speed.Length() + (dist / 10), 5, 40);
                    speed += 0.05f;
                    speed -= Math.Abs(MyMath.ClosestAngleDifference(dir, dir2) / 6000) * speed;
                    if (speed > speedTarget)
                        speed -= 0.1f;
                    if (speed < 0)
                        speed = 0;
                    if (dist < 100)
                        speed -= 0.05f;
                }
                if (IsCollidingWith(player))
                {
                    new Explosion(doc, Pos.X, Pos.Y, Pos.Z, 5, 0.1f, 100f, 1f, 90f, 30f);
                    Remove();
                    var att = new Attack(0, 0, 0)
                        {
                            Power = 50,
                        };
                    Rectangle r = BoundingBox;
                    player.Attacked(att, this, BoundingBox, Rectangle.Intersect(BoundingBox, player.BoundingBox));
                }
            }
            Speed = new Vector3(MyMath.LengthDir(speed, dir), 0);
            base.Update();
        }

    }
}
