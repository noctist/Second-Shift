using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondShiftMobile.Behaviours
{
    public class Bounce : Behaviour
    {
        struct CollisionInfo
        {
            public FloorCollisionType Type;
            public Vector3 ObjSpeed;
            public CollisionInfo(FloorCollisionType type, Vector3 objSpeed)
            {
                Type = type;
                ObjSpeed = objSpeed;
            }
        }
        float bounce;
        Vector3 bounceSpeed;
        List<CollisionInfo> colls = new List<CollisionInfo>();
        public Bounce(float bounce)
        {
            this.bounce = bounce;
        }
        public override void Update(Obj o)
        {
            base.Update(o);
            var collisions = o.GetCollisions();
            foreach (var obj in collisions)
            {
                float left, right, top, bottom;
                left = Math.Abs(o.BoundingBox.Left - obj.BoundingBox.Left);
                right = Math.Abs(o.BoundingBox.Right - obj.BoundingBox.Right);
                top = Math.Abs(o.BoundingBox.Top - obj.BoundingBox.Top);
                bottom = Math.Abs(o.BoundingBox.Bottom - obj.BoundingBox.Bottom);
                bool inside = obj.BoundingBox.Contains(o.BoundingBox);
                inside = true;
                
                float min = Math.Min(left, Math.Min(right, Math.Min(top, bottom)));
                if (min == left)
                {
                    colls.Add(new CollisionInfo(FloorCollisionType.Left, obj.GetMoveSpeed()));
                    if (inside)
                        o.Pos.X += (obj.BoundingBox.Left - o.BoundingBox.Right) * 0.5f;
                }
                else if (min == right)
                {
                    colls.Add(new CollisionInfo(FloorCollisionType.Right, obj.GetMoveSpeed()));
                    if (inside)
                        o.Pos.X += (obj.BoundingBox.Right - o.BoundingBox.Left) * 0.5f;
                }
                else if (min == top)
                {
                    colls.Add(new CollisionInfo(FloorCollisionType.Top, obj.GetMoveSpeed()));
                    if (inside)
                    {
                        o.Speed.Y = 0;
                        o.Pos.Y += (obj.BoundingBox.Top - o.BoundingBox.Bottom) * 0.5f;
                    }
                }
                else
                {
                    colls.Add(new CollisionInfo(FloorCollisionType.Bottom, obj.GetMoveSpeed()));
                    if (inside)
                        o.Pos.Y += (obj.BoundingBox.Bottom - o.BoundingBox.Top)  * 0.5f;
                }
            }
        }
        public override Vector3 Speed(Obj o, Vector3 speed)
        {
            foreach (var c in colls)
            {
                if (c.Type == FloorCollisionType.Left)
                {
                    if (speed.X > 0)
                        speed.X *= -bounce;
                    speed += c.ObjSpeed;
                }
                else if (c.Type == FloorCollisionType.Right)
                {
                    if (speed.X < 0)
                        speed.X *= -bounce;
                    speed += c.ObjSpeed;
                }
                else if (c.Type == FloorCollisionType.Top)
                {
                    if (speed.Y > 0)
                        speed.Y *= -bounce;
                    speed += c.ObjSpeed;
                }
                else if (c.Type == FloorCollisionType.Bottom)
                {
                    if (speed.Y < 0)
                        speed.Y *= -bounce;
                    speed += c.ObjSpeed;
                }
                speed *= 0.7f;
            }
            colls.Clear();
            return speed;
        }
    }
}
