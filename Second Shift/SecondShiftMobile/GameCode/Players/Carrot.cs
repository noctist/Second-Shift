using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondShiftMobile.Players
{
    public class Carrot : Player
    {
        Combo normalCombo, upCombo, downAirCombo, dashCombo, upAttack, backGroundCombo;
        TextureFrame standSprite, stanceSprite;
        float fighting = 0;
        public Carrot(Game1 Doc, float X, float Y, float Z)
            :base(Doc, Doc.LoadTex("Characters/Carrot/Standing"),
                X, Y, Z)
        {
            DashSpeed = 27;
            Faction = SecondShiftMobile.Faction.Good;
            // Default carrot sprite is 506, 415 large
            Scale = new Vector2((200) / 506f);
            StandSprite = standSprite = Texture;
            stanceSprite = Doc.LoadTex("Characters/Carrot/FightingStance");
            RunAnimation = Doc.LoadAtlasAnimation("Characters/Carrot/RunSprite_Trans", Vector2.Zero, new Vector2(506, 415), 4, TextureDirection.Horizontal);
            JumpAnimation = FallAnimation = Doc.LoadAtlasAnimation("Characters/Carrot/flip", Vector2.Zero, new Vector2(100, 82), 7, TextureDirection.Horizontal);
            foreach (var t in JumpAnimation)
            {
                t.Height = 415;
                t.Width = 506;
            }
            DashGroundAnimation = Doc.LoadAtlasAnimation("Characters/Carrot/Dash", Vector2.Zero, new Vector2(506, 415), 4, TextureDirection.Horizontal);
            BoundingRectangle = new Rectangle(179, 60, 230, 0);
            WallStickCenter = 50;
            normalCombo = new Combo()
            {
                Animation = doc.LoadAtlasAnimation("Characters/Carrot/Combo", Vector2.Zero, new Vector2(506, 415), 39, TextureDirection.Horizontal),
                Framespeed = 0.6f
            };
            var punchRec = new Rectangle(250, 230, 120, 100);
            normalCombo.Attacks.Add(
                new Attack(0, 8, 4) { Power = 10, HitBox = punchRec, HitPause = false, AttackSound = "meleemiss1", HitSound = "weakpunch",
                MoveSpeed = new Vector3(6, 0, 0)}
                );
            normalCombo.Attacks.Add(
                new Attack(9, 17, 14) { Power = 15, HitBox = punchRec, HitPause = false, AttackSound = "meleemiss2", HitSound = "weakkick",
                MoveSpeed = new Vector3(6, 0, 0)}
                );
            normalCombo.Attacks.Add(
                new Attack(18, 39, 22) { Power = 25, HitBox = new Rectangle(250, 280, 170, 100), AttackSound = "meleemiss3", HitSound = "mediumpunch", KnockBack = true, Direction = -20, 
                    MoveSpeed = new Vector3(7, 0, 0) }
                );
            upCombo = new Combo()
            {
                Animation = doc.LoadAtlasAnimation("Characters/Carrot/UpAttack", Vector2.Zero, new Vector2(506, 415), 4, TextureDirection.Horizontal),
                Framespeed = 0.75f,
                LoopAnimation = true,
                AllowSwitchOut = false
            };
            upCombo.Attacks.Add(new Attack(0, 3, 0)
            {
                HitOnAllFrames = true,
                HitBox = new Rectangle(250, 150, 120, 150),
                //MoveSpeed = new Vector3(5, -15, 0),
                Power = 27,
                KnockBack = true,
                Direction = -75,
                HitSound = "strongpunch",
                AttackSound = "meleemiss1",
                WaitTime = 120,
                TimeOut = 120
            }
            );

            downAirCombo = new Combo()
            {
                Animation = doc.LoadAtlasAnimation("Characters/Carrot/kickdown", Vector2.Zero, new Vector2(100, 82), 2, TextureDirection.Horizontal, 20),
                Framespeed = 0.5f,AllowSwitchOut = false
            };
            downAirCombo.Attacks.Add(new Attack(0, downAirCombo.Animation.Length, downAirCombo.Animation.Length)
            {
                HitOnAllFrames = true,
                HitBox = new Rectangle(250, 250, 120, 150),
                MoveSpeed = new Vector3(15, 10, 0),
                Power = 30,
                KnockBack = true,
                Direction = 45,
                HitSound = "strongkick",
                AttackSound = "meleemiss2",
                WaitTime = 60,
                
            }
            );
            foreach (var t in downAirCombo.Animation)
            {
                t.Height = 415;
                t.Width = 506;
            }
            dashCombo = new Combo()
            {
                Animation = doc.LoadAtlasAnimation("Characters/Carrot/DashPunch", Vector2.Zero, new Vector2(506, 415), 4, TextureDirection.Horizontal, 1),
                Framespeed = 0.5f,
                LoopAnimation = true, AllowSwitchOut = false
            };
            dashCombo.Attacks.Add(new Attack(0, dashCombo.Animation.Length, dashCombo.Animation.Length)
            {
                HitOnAllFrames = true,
                HitBox = punchRec,
                Power = 20,
                HitPause = true,
                KnockBackAmount = 30,
                KnockBack = true,
                Direction = 10,
                HitSound = "strongpunch",
                AttackSound = "meleemiss2",
                WaitTime = 60,
                TimeOut = 90
            }
            );
            backGroundCombo = new Combo()
            {
                Animation = doc.LoadAtlasAnimation("Characters/Carrot/BackAttack", Vector2.Zero, new Vector2(506, 415), 31, TextureDirection.Horizontal),
                Framespeed = 0.7f,
                AllowSwitchOut = true
            };
            backGroundCombo.Attacks.Add(new Attack(0, 13, 6)
            {
                AttackSound = "meleemiss2",
                HitSound = "weakkick",
                Power = 20, HitPause = true, PauseLength = 10,
                KnockBack = true, KnockBackAmount = 10,
                MoveSpeed = new Vector3(-8, 0, 0),
                HitBox = new Rectangle(60, 240, 125, 50)
            });
            backGroundCombo.Attacks.Add(new Attack(14, 31, 17)
            {
                Power = 25, HitPause = true, PauseLength = 30,
                KnockBack = true, KnockBackAmount = 35,
                AttackSound = "meleemiss3",
                HitSound = "strongkick",
                HitBox = new Rectangle(270, 230, 120, 100),
                MoveSpeed = new Vector3(10, 0, 0)
            });
        }
        protected override void NextAttackOverride()
        {
            /*if (CurrentCombo == normalCombo)
            {
                float speed = 6;
                speed2 += Math.Max(Math.Min(speed, speed - speed2), 0);
            }*/
        }
        protected override void AttackLanded(Obj obj, Attack attack, Rectangle attackBoundingBox, Rectangle attackIntersection)
        {
            fighting = 200;
            base.AttackLanded(obj, attack, attackBoundingBox, attackIntersection);
        }
        public override bool Attacked(Attack attack, Obj obj, Rectangle AttackBox, Rectangle intersection)
        {
            fighting = 200;
            return base.Attacked(attack, obj, AttackBox, intersection);
        }
        public override void Update()
        {

            Global.Output = AttackTimer;
            if (fighting > 0)
            {
                fighting -= PlaySpeed;
                if (StandSprite != stanceSprite)
                {
                    StandSprite = stanceSprite;
                    SetAnimations();
                }
            }
            else
            {
                if (StandSprite != standSprite)
                {
                    StandSprite = standSprite;
                    SetAnimations();
                }
            }
            if (Attacking)
            {
                if (CurrentCombo == dashCombo)
                {
                    if (!Dashing && AttackTimer > 30)
                    {
                        EndAttack();
                    }
                }
            }
            base.Update();
        }
        protected override void OnTheGroundChanged(Vector3 speed)
        {
            if (OnTheGround)
            {
                if (CurrentCombo == downAirCombo || CurrentCombo == upCombo)
                {
                    EndAttack();
                }
            }
            base.OnTheGroundChanged(speed);
        }
        protected override void SetFramespeed()
        {
            if (State == PlatformerState.DashingGround)
            {
                Framespeed = 0.5f;
            }
            else
            {
                base.SetFramespeed();
            }
        }
        public override void AttackButtonHit(ControlDirection direction)
        {
            switch (direction)
            {
                default:
                    if (Dashing && !Attacking)
                    {
                        ChangeCurrentCombo(dashCombo, false);
                        NextAttack();
                    }
                    else
                    {
                        if (!Attacking)
                        {
                            if (direction == ControlDirection.Back)
                                speedMul *= -1;
                        }
                        else if (Attacking && (CurrentCombo == normalCombo || CurrentCombo == dashCombo || CurrentCombo == backGroundCombo))
                        {
                            if (direction == ControlDirection.Back)
                            {
                                if (CurrentCombo == backGroundCombo && AttacksInCurrentCombo < 3)
                                {
                                    NextAttack(0);
                                    return;
                                }
                                else if (CurrentCombo != backGroundCombo && ChangeCurrentCombo(backGroundCombo, false))
                                {
                                    NextAttack(0);
                                    return;
                                }
                                else if (ChangeCurrentCombo(normalCombo, false))
                                {
                                    speedMul *= -1;
                                    NextAttack(0);
                                    return;
                                }
                            }
                            else if (direction == ControlDirection.Forward && CurrentCombo == backGroundCombo)
                            {
                                NextAttack();
                                return;
                            }
                        }
                        if (ChangeCurrentCombo(normalCombo, false))
                        {
                            NextAttack();
                        }
                    }
                    break;
                case ControlDirection.Up:
                    if (OnTheGround)
                    {
                        if (ChangeCurrentCombo(upCombo, false))
                        {
                            NextAttack();
                            Speed.Y -= 14;
                        }
                    }
                    else
                    {
                        goto default;
                    }
                    break;
                case ControlDirection.Down:
                    if (!OnTheGround)
                    {
                        ChangeCurrentCombo(downAirCombo, false);
                        NextAttack();
                    }
                    else goto default;
                    break;
            }
            //base.AttackButtonHit(direction);
        }

        
    }
}
        