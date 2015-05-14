using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace SecondShiftMobile.Enemies
{
    public class Guard : Enemy
    {
        CombatObj player;
        protected float RunFramespeedMultiplier = 0.03f;
        Combo normalCombo;
        Random rand = new Random();
        float attackWaitTimer = 0;
        float maxAttackWaitTimer = 30;
        public Guard(Game1 Doc, float x, float y, float z)
            : base(Doc, Doc.LoadTex("Characters/Enemies/Guard/Stand"), x, y, z)
        {
            Faction = SecondShiftMobile.Faction.Evil;
            health = maxHealth = 100;
            StandSprite = Texture;
            RunAnimation = Doc.LoadAnimation("Characters/Enemies/Guard/Run", "run", 0, 3);
            topSpeed = 9;
            BoundingRectangle = new Rectangle(30, 0, 30, 0);
            normalCombo = new Combo() { Animation = doc.LoadAnimation("Characters/Enemies/Guard/Attack", "hit", 0, 3), Framespeed = 0.2f, LoopAttack = false };
            normalCombo.Attacks.Add(new Attack(0, 3, 2)
            {
                Power = 2,
                AttackSound = "meleemiss1", HitSound = "weakkick",
                WaitTime = 60,
                TimeOut = 60,
                HitBox = new Rectangle(80, 52, 47, 70),
            });
            AddCombo(normalCombo);
            ChangeCurrentCombo(normalCombo);
            Active = true;
            DashGroundSprite = Doc.LoadTex("Characters/Enemies/Guard/Dash");
            //Parallel = true;
            DeactivateOffscreen = true;
            OnScreenExpansion = 400;
            Bevel = true;
        }
        Rectangle recIntersection = Rectangle.Empty;
        protected override bool AttackedOverride(Attack attack, Obj obj, Rectangle AttackBox, Rectangle intersection)
        {
            recIntersection = intersection;

            attackWaitTimer += attack.Power;
            return base.AttackedOverride(attack, obj, AttackBox, intersection);
        }
        protected override void Die(Attack attack, Obj obj, Rectangle intersection)
        {
            var dat = Textures.GetTextureData(Texture);
            Vector3 cent = intersection.Center.ToVector2().ToVector3();
            Vector3 mycent = BoundingBox.Center.ToVector2().ToVector3();
            cent.Z = mycent.Z = Pos.Z;
            Vector3 baseSpeed = mycent - cent;
            for (int i = 0; i < dat.Length; i+= 60)
            {
                float ix = i % Texture.Width;
                float iy = i / Texture.Height;
                var c = dat[i % Texture.Width, i / Texture.Height];
                if (c.A > 100)
                {
                    var d = new Disappear(doc, Textures.SmallLight, Pos.X, Pos.Y, Pos.Z) {  Color = c, Scale = new Vector2(0.5f) };
                    d.AddBehaviour(new SecondShiftMobile.Behaviours.DirectionToRotation(2) { UseCamera = true });
                    Vector3 pos = Pos + ((new Vector3(ix, iy, 0) - Origin.ToVector3()) * Scale.ToVector3());
                    d.Pos = Pos + ((new Vector3(ix, iy, 0) - Origin.ToVector3()) * Scale.ToVector3());
                    d.Speed = ((baseSpeed * 0.5f) + (new Vector3(MyMath.RandomRange(-1, 1), MyMath.RandomRange(-1, 1), MyMath.RandomRange(-1, 1)) * 10));
                    //d.Speed = baseSpeed;
                    d.UpscaledSecondShiftDraw = true;
                }
            }
            base.Die(attack, obj, intersection);
        }
        public override void Update()
        {
            if (!IsNetworkControlled)
            {
                if (player == null || !player.Active || player.RemoveCalled || (player.Pos - Pos).Length() > 200)
                {
                    var objs = doc.FindObjects<CombatObj>();
                    var dist = float.MaxValue;
                    foreach (var o in objs)
                    {
                        if (o != this && o.Faction != SecondShiftMobile.Faction.Evil)
                        {
                            var tDist = (o.Pos - Pos).Length();
                            if (tDist < dist)
                            {
                                dist = tDist;
                                player = o;
                            }
                        }
                    }
                }
                if (player != null)
                {
                    if (Math.Abs(player.Pos.X - Pos.X) < 1500 && Math.Abs(player.Pos.Y - Pos.Y) < 400 && OnTheGround)
                    {
                        if (Math.Abs(player.Pos.X - Pos.X) < 140 && Math.Abs(player.Pos.X - Pos.X) > 30 && player.AttackState == SecondShiftMobile.AttackState.Starting && AttackState < AttackState.Hitting && !Dashing && !IsHurt)
                        {
                            int r = rand.Next(6);
                            if (r == 0)
                            {
                                r = rand.Next(2);
                                if (player.Pos.X > Pos.X)
                                {
                                    if (r == 0)
                                    {
                                        DashLeft(20);
                                    }
                                    else
                                    {
                                        DashRight(20);
                                    }
                                }
                                else
                                {
                                    if (r == 0)
                                    {
                                        DashRight(20);
                                    }
                                    else
                                    {
                                        DashLeft(20);
                                    }
                                }
                                EndAttack();
                            }
                            else if (r == 1)
                            {
                                //Jump(15);
                            }
                        }
                        if (!Dashing)
                        {
                            if (Math.Min(Math.Abs(player.BoundingBox.X - BoundingBox.Right), Math.Abs(player.BoundingBox.Right - BoundingBox.Left)) > 65)
                            {
                                if (player.Pos.X > Pos.X)
                                    MoveRight();
                                else if (player.Pos.X < Pos.X)
                                    MoveLeft();
                            }
                            /*if (Math.Abs(player.Pos.X - Pos.X) < 50)
                            {
                                if (player.Pos.X > Pos.X)
                                    MoveLeft();
                                else if (player.Pos.X < Pos.X)
                                    MoveRight();
                            }*/
                        }
                        if (player.Pos.Y < Pos.Y - 100 && !Attacking)
                            Jump(20);
                        if (Math.Min(Math.Abs(player.BoundingBox.X - BoundingBox.Right), Math.Abs(player.BoundingBox.Right - BoundingBox.Left)) < 65 && OnTheGround && !Dashing)
                        {
                            attackWaitTimer += PlaySpeed;
                            if (attackWaitTimer > maxAttackWaitTimer)
                            {
                                if (Speed.X < 1)
                                {
                                    if (player.Pos.X > Pos.X)
                                        speedMul = 1;
                                    else if (player.Pos.X < Pos.X)
                                        speedMul = -1;
                                }
                                NextAttack();
                                attackWaitTimer = 0;
                                maxAttackWaitTimer = MyMath.RandomRange(15, 60, rand);
                            }
                        }
                    }
                }
            }
            base.Update();
        }
        protected override void SetFramespeed()
        {
            if (!Attacking)
            {
                switch (State)
                {
                    case PlatformerState.Running:
                        Framespeed = Math.Abs(GetMoveSpeed().X) * RunFramespeedMultiplier;
                        break;
                    default:
                        Framespeed = 1;
                        break;
                }
                base.SetFramespeed();
            }
        }
        public override void Draw()
        {
            base.Draw();
            //var rec = ConvertToScreenRec(BoundingBox);
            //doc.DrawSprite(doc.LoadTex("TestWall"), new Vector2(rec.Center.X, rec.Top - 20), Color.Black * 0.4f, 0, new Vector2(64, 64), new Vector2(1, 0.3f), Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0);
            //doc.DrawSprite(doc.LoadTex("TestWall"), new Vector2(rec.Center.X, rec.Top - 20), Color.White, 0, new Vector2(64, 64), new Vector2(health / maxHealth, 0.3f), Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0);
        }
    }
}
