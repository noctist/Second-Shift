using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondShiftMobile.Enemies
{
    public class Samurai : Enemy
    {
        const float TargetDistance = 1000;
        PlatformerObj target;
        Combo stabCombo, dashStabCombo;
        public Samurai(Game1 Doc, float X, float Y, float Z)
            : base(Doc, Doc.LoadTex("Characters/Enemies/Samurai/Stand"), X, Y, Z)
        {
            health = maxHealth = 100;
            Scale = new Vector2((200) / 506f);
            BoundingRectangle = new Rectangle(200, 170, 210, 0);
            DashAirAnimation = DashGroundAnimation = Doc.LoadStickAnimation("Characters/Enemies/Samurai/Dash", 5);
            StandSprite = Doc.LoadTex("Characters/Enemies/Samurai/Stand");
            RunAnimation = Doc.LoadAtlasAnimation("Characters/Enemies/Samurai/Run", Vector2.Zero, new Vector2(506, 415), 5, TextureDirection.Horizontal);
            Faction = SecondShiftMobile.Faction.Evil;
            topSpeed = 14;
            stabCombo = new Combo()
            {
                Animation = Doc.LoadAtlasAnimation("Characters/Enemies/Samurai/Stab", Vector2.Zero, new Vector2(506, 415), 12, TextureDirection.Horizontal),
                AllowSwitchOut = false,
                Framespeed = 0.5f, 
            };
            stabCombo.Attacks.Add(new Attack(0, 8, 4)
            {
                Power = 2,
                HitBox = new Rectangle(300, 250, 210, 30),
                WaitTime = 0,
                TimeOut = 4,
                MoveSpeed = new Vector3(-3, 0, 0),
                AttackSound = "meleemiss1",
                HitSound = "swordhit"
            });
            dashStabCombo = new Combo()
            {
                Animation = Doc.LoadStickAnimation("Characters/Enemies/Samurai/DashStab", 10),
                AllowSwitchOut = false,
                Framespeed = 0.5f,
            };
            dashStabCombo.Attacks.Add(new Attack(0, 10, 3)
            {
                Power = 20, KnockBackAmount = 20, HitPauseLength = 90, EffectType = AttackEffectType.Sharp,
                HitBox = new Rectangle(300, 250, 210, 30),
                AttackSound = "spin", HitSound = "strongpunch"
            });
            AddCombo(stabCombo, dashStabCombo);
        }
        protected override void CreateDecisions()
        {
            var dashDecision = new Decision(30, 0.1f)
            {
                DecisionAction = () =>
                    {
                        if (target != null)
                        {
                            if (target.Pos.X > Pos.X)
                                DashRight(35);
                            else DashLeft(35);
                        }
                    },
                NecessaryCondition = () =>
                    {
                        return !Dashing && target != null && target.Active;
                    }
            };
            dashDecision.AddDecisionHelper(0.25f, () =>
                {
                    return Math.Abs(target.Pos.Y - Pos.Y) < 200;
                });
            dashDecision.AddDecisionHelper(0.5f, () =>
                {
                    var yspeed = target.Speed.Y;
                    if (yspeed < 0)
                        return false;
                    var y = yspeed * 15 * target.PlaySpeed;
                    y += target.Pos.Y;
                    if (Math.Abs(y - Pos.Y) < 300)
                        return true;
                    return false;
                });
            dashDecision.AddDecisionHelper(1, () =>
                {
                    if (speed2 > 5)
                    {
                        var dist = Math.Abs(target.Pos.X - Pos.X);
                        if (dist < 500 && dist > 170)
                        {
                            if (target.Pos.X > Pos.X && speedMul == 1)
                                return true;
                            if (target.Pos.X < Pos.X && speedMul == -1)
                                return true;
                        }
                    }
                    return false;
                });
            AddDecision(dashDecision);
        }
        public override void Update()
        {
            if (target != null)
            {
                var xdiff = Math.Abs(Pos.X - target.Pos.X);
                if (!Attacking)
                {
                    if (xdiff > 150)
                    {
                        if (Pos.X > target.Pos.X)
                        {
                            MoveLeft(1);
                        }
                        else
                        {
                            MoveRight(1);
                        }
                    }
                    else if (xdiff < 125)
                    {
                        if (Pos.X < target.Pos.X)
                        {
                            MoveLeft(1);
                        }
                        else
                        {
                            MoveRight(1);
                        }
                    }
                }
                if (xdiff < 150)
                {
                    if (!Attacking && OnTheGround && !Dashing)
                    {
                        if (Pos.X > target.Pos.X && speedMul == 1)
                        {
                            MoveLeft(1);
                        }
                        else if (Pos.X < target.Pos.X && speedMul == -1)
                        {
                            MoveRight(1);
                        }
                        ChangeCurrentCombo(stabCombo);
                        NextAttack();
                    }
                }
                if (xdiff < 200)
                {
                    if (Dashing && !Attacking)
                    {
                        ChangeCurrentCombo(dashStabCombo);
                        NextAttack();
                    }
                }
            }
            base.Update();
        }
        public override void ParallelUpdate()
        {
            base.ParallelUpdate();
            var targets = doc.FindObjects<PlatformerObj>();
            if (target == null || !target.Active || (Pos - target.Pos).Length() > TargetDistance)
            {
                PlatformerObj potential = null;
                float dist = 1000;
                foreach (var t in targets)
                {
                    if (t.Faction != SecondShiftMobile.Faction.Evil && t.Active)
                    {
                        var currDist = (Pos - t.Pos).Length();
                        if (currDist < dist)
                        {
                            potential = t;
                        }
                    }
                }
                target = potential;
            }
        }
        protected override void AttackLandedOverride(Obj obj, Attack attack, Rectangle attackBoundingBox, Rectangle attackIntersection)
        {
            if (dashStabCombo.Attacks.Contains(attack))
            {
                speed2 = 0;
            }
            base.AttackLandedOverride(obj, attack, attackBoundingBox, attackIntersection);
        }
        protected override void SetFramespeed()
        {
            switch (State)
            {
                case PlatformerState.Running:
                    Framespeed = Math.Abs(GetMoveSpeed().X) * 0.05f;
                    break;
                default:

                    base.SetFramespeed();
                    break;
            }
        }
    }
}
