using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
namespace SecondShiftMobile
{
    public class CombatObj: PlatformerObj
    {
        AttackState prevAttackState = AttackState.None;
        AttackState attackState = AttackState.None;
        public AttackState AttackState
        {
            get
            {
                return attackState;
            }
        }
        public bool IsKnockedBack { private set; get; }
        public Color AttackColor = Color.Red;
        Color hurtColor = new Color(200, 0, 0);
        protected float health = 100;
        protected float maxHealth = 100;
        public List<Combo> Combos;
        public Combo CurrentCombo { get; private set; }
        protected bool Attacking = false;
        int attackIndex = 0;
        int hitID = -1;
        protected float AttackTimer { get; private set; }
        protected float MaxAttackTimer = 30;
        bool scheduleAttack = false;
        int scheduledAttackIndex = -1;
        float hurt = 0;
        public bool CanAttack
        {
            get { return hurt < 0.1f; }
        }
        public bool IsHurt
        {
            get { return !CanAttack || IsKnockedBack; }
        }
        public bool AllowHurtWhileDashing = false;
        /// <summary>
        /// The weight of the object
        /// </summary>
        public float Weight = 10;
        Vector3 combatSpeed = new Vector3();
        int attacksInCurrentCombo = 0;
        public int AttacksInCurrentCombo { get { return attacksInCurrentCombo; } }
        protected int AttackIndex
        {
            get { return attackIndex; }
            set
            {
                if (CurrentCombo != null)
                {
                    if (value >= CurrentCombo.Attacks.Count)
                    {
                        attackIndex = CurrentCombo.Attacks.Count - 1;
                    }
                    else if (value < 0)
                    {
                        attackIndex = 0;
                    }
                    else
                    {
                        attackIndex = value;
                    }
                }
            }
        }

        public CombatObj(Game1 Doc, TextureFrame Tex, float X, float Y, float Z)
            :base(Doc, Tex, X, Y, Z)
        {
            Combos = new List<Combo>();
            AttackTimer = 0;
            Attackable = true;
            IsKnockedBack = false;
        }
        protected virtual void ChangeCurrentCombo(int comboIndex)
        {
            ChangeCurrentCombo(Combos[comboIndex]);
        }
        protected virtual bool ChangeCurrentCombo(Combo combo, bool resetIfCurrent = true)
        {
            if (CurrentCombo == combo && !resetIfCurrent)
                return true;
            if ((CurrentCombo == null) || ((CurrentCombo != combo || resetIfCurrent) && (CurrentCombo.AllowSwitchOut || !Attacking)))
            {
                if (Attacking)
                    EndAttack();
                CurrentCombo = combo;
                attackIndex = 0;
                hitID = -1;
                attacksInCurrentCombo = 0;
                return true;
            }
            return false;
        }

        protected virtual void Die(Attack attack, Obj obj, Rectangle intersection)
        {
            //Remove();
        }

        protected virtual void NextAttack(int attackIndex)
        {
            if (!Attacking)
            {
                if (CanAttack)
                {
                    BeginAttack(attackIndex == -1 ? 0 : attackIndex);
                }
            }
            else
            {
                scheduleAttack = true;
                scheduledAttackIndex = attackIndex;
            }
        }
        protected void NextAttack()
        {
            NextAttack(-1);
            /*if (!Attacking)
            {
                if (CanAttack)
                {
                    BeginAttack(0);
                }
            }
            else
            {
                scheduleAttack = true;
            }*/
        }

        void nextAttack(int attackIndex = -1)
        {
            //if (CanAttack)
            {
                if (!Attacking)
                {
                    if (CanAttack)
                        BeginAttack(0);
                }
                else

                    if ((int)Frame >= CurrentCombo.Attacks[AttackIndex].EndFrame && AttackTimer > CurrentCombo.Attacks[AttackIndex].WaitTime && (AttackIndex < CurrentCombo.Attacks.Count - 1 || CurrentCombo.LoopAttack))
                    {
                        if (AttackIndex < CurrentCombo.Attacks.Count - 1)
                        {
                            BeginAttack(attackIndex == -1 ? (AttackIndex + 1) : attackIndex);
                        }
                        else if (CurrentCombo.LoopAttack)
                        {
                            if (CanAttack)
                                BeginAttack(0);
                        }
                    }
                    else
                    {
                        //scheduleAttack = true;
                    }
            }
        }

        protected virtual void NextAttackOverride()
        {

        }
        protected virtual void BeginAttack(int attackIndex)
        {
            if (CurrentCombo != null && CurrentCombo.Attacks.Count > 0)
            {
                if ((!Attacking || AttackTimer > 0))
                {
                    lastMovedAttack = null;
                    hitID = -1;
                    scheduleAttack = false;
                    Attacking = true;
                    AttackTimer = 0;
                    AttackIndex = attackIndex;
                    Frame = CurrentCombo.Attacks[AttackIndex].StartFrame;
                    Animation = CurrentCombo.Animation;
                    Framespeed = CurrentCombo.Framespeed;
                    
                    if (CurrentCombo.Attacks[AttackIndex].AttackSoundEffect != null)
                    {
                        attackState = SecondShiftMobile.AttackState.Starting;
                        var c = CurrentCombo.Attacks[AttackIndex];
                        c.AttackSoundEffect.Play(GetSoundVolume(c.AttackVolume), GetSoundPitch(), GetSoundPan());
                    }
                    ApplyAttackMovement(CurrentCombo.Attacks[AttackIndex].MoveSpeed);
                    NextAttackOverride();
                    attacksInCurrentCombo++;
                }
            }
        }
        public override Vector3 GetMoveSpeed()
        {
            return base.GetMoveSpeed() + combatSpeed;
        }
        protected virtual void EndAttack()
        {
            scheduleAttack = false;
            Attacking = false;
            Frame = 0;
            SetAnimations();
            SetFramespeed();
            hitID = -1;
            attackState = SecondShiftMobile.AttackState.Ending;
            scheduledAttackIndex = -1;
        }
        
        public override void MoveLeft(float runSpeed = 1)
        {
            if (!Attacking && !IsKnockedBack)
            base.MoveLeft(runSpeed);
        }

        public override void MoveRight(float runSpeed = 1)
        {
            if (!Attacking && !IsKnockedBack)
            base.MoveRight(runSpeed);
        }
        public override void DashLeft(float dashSpeed)
        {
            if (!Attacking || Frame >= CurrentCombo.Attacks[attackIndex].HitFrame && hitID == attackIndex)
            {
                EndAttack();
                base.DashLeft(dashSpeed);
            }
        }
        public override void DashRight(float dashSpeed)
        {
            if (!Attacking || Frame >= CurrentCombo.Attacks[attackIndex].HitFrame && hitID == attackIndex)
            {
                EndAttack();
                base.DashRight(dashSpeed);
            }
        }
        public override void Jump(float jumpSpeed)
        {
            if ((!Attacking || (Frame >= CurrentCombo.Attacks[attackIndex].HitFrame && hitID == attackIndex)) && (!IsHurt))
            {
                EndAttack();
                base.Jump(jumpSpeed);
            }
        }
        protected override void OnAWallChanged(Vector3 speed)
        {
            if (IsKnockedBack && OnAWall)
            {
                Speed.X = 0;
                IsKnockedBack = false;
            }
            base.OnAWallChanged(speed);
        }
        public override void EarlyUpdate()
        {
            base.EarlyUpdate();
            combatSpeed *= 0.9f;
            if (combatSpeed.Length() < 1)
                combatSpeed = Vector3.Zero;
            if (prevAttackState == SecondShiftMobile.AttackState.Ending && attackState == SecondShiftMobile.AttackState.Ending)
            {
                attackState = SecondShiftMobile.AttackState.None;
            }
            else if ((prevAttackState == SecondShiftMobile.AttackState.Starting && attackState == SecondShiftMobile.AttackState.Starting))
            {
                attackState = SecondShiftMobile.AttackState.Animating;
            }
            prevAttackState = attackState;
        }
        public override void Update()
        {
            if (IsKnockedBack)
            {
                float xsub = 1f;
                if (!OnTheGround)
                    xsub *= 0.5f;
                if (Speed.X > 0)
                {
                    Speed.X -= xsub * PlaySpeed;
                }
                else if (Speed.X < 0)
                {
                    Speed.X += xsub * PlaySpeed;
                }
                if (Math.Abs(Speed.X) < xsub || Math.Abs(Speed.X) < 1)
                {
                    Speed.X = 0;
                    IsKnockedBack = false;
                }
            }
            hurt += -hurt * 0.1f * PlaySpeed;
            base.Update();
            if (Attacking && OnAWall && !OnTheGround)
                EndAttack();
            if (Attacking)
            {
                int ind = AttackIndex;
                var a = CurrentCombo.Attacks[attackIndex];
                float frame = Frame;
                if (frame >= CurrentCombo.Attacks[attackIndex].HitFrame || a.HitOnAllFrames)
                {
                    if (hitID != attackIndex)
                    {
                        attackState = SecondShiftMobile.AttackState.Hitting;
                        if (AttackHit(CurrentCombo.Attacks[attackIndex]) || !a.HitOnAllFrames)
                        {
                            hitID = attackIndex;
                        }
                    }
                    else if (attackState != SecondShiftMobile.AttackState.Starting)
                    {
                        attackState = SecondShiftMobile.AttackState.Animating;
                    }
                }
                if (frame >= a.EndFrame )
                {
                    AttackTimer += PlaySpeed;
                    if (Attacking && scheduleAttack && AttackTimer >= a.WaitTime)
                    {
                        if (attackIndex == CurrentCombo.Attacks.Count - 1)
                        {
                            if (CurrentCombo.LoopAttack)
                            {
                                BeginAttack(0);
                                return;
                            }
                            else if (AttackTimer >= a.TimeOut)
                            {
                                EndAttack();
                                return;
                            }
                        }
                        nextAttack(scheduledAttackIndex);
                        
                    }
                    if (ind == AttackIndex)
                    {
                        if (AttackTimer > a.TimeOut)
                        {
                            EndAttack();
                        }
                        if (CurrentCombo.LoopAnimation)
                        {
                            Framespeed = CurrentCombo.Framespeed;
                        }
                        else
                        Framespeed = 0;
                        /*if (attackIndex == CurrentCombo.Attacks.Count - 1 && AttackTimer > a.TimeOut)
                        {
                            EndAttack();
                        }*/
                    }
                }
                else
                {
                    Framespeed = CurrentCombo.Framespeed;
                }
            }
        }
        Rectangle createAttackRectangle(Attack attack)
        {
            if (spriteEffect == SpriteEffects.None)
            {
                return new Rectangle
                    (
                        (int)((Pos.X - (Origin.X * Scale.X)) + (attack.HitBox.X * Scale.X)),
                        (int)((Pos.Y - (Origin.Y * Scale.Y)) + (attack.HitBox.Y * Scale.Y)),
                        (int)(attack.HitBox.Width * Scale.X),
                        (int)(attack.HitBox.Height * Scale.Y)
                    );
            }
            else
            {
                return new Rectangle
                    (
                        (int)((Pos.X - (Origin.X * Scale.X)) + ((Texture.Width - attack.HitBox.X - attack.HitBox.Width) * Scale.X)),
                        (int)((Pos.Y - (Origin.Y * Scale.Y)) + (attack.HitBox.Y * Scale.Y)) ,
                        (int)(attack.HitBox.Width * Scale.X),
                        (int)(attack.HitBox.Height * Scale.Y)
                    );
            }
        }
        public virtual void ApplyAttackMovement(Vector3 move)
        {
            var x = Math.Abs(move.X);
            speed3 += Math.Max(Math.Min(x, x - (speed2 + speed3)), 0) * (move.X < 0 ? -1 : 1);
            Speed.Y += move.Y;
        }
        Attack lastMovedAttack = null;
        private bool AttackHit(Attack attack)
        {
            Obj o;
            bool hit = false;
            Rectangle rec = BoundingBox;
            if (attack.HitBox != Rectangle.Empty)
            {
                rec = createAttackRectangle(attack);
            }
            AttackLaunched(attack, rec);
            var s = CurrentCombo.Attacks[AttackIndex].MoveSpeed;
            s.X *= speedMul;
            //combatSpeed = s;
            /*if (lastMovedAttack != attack)
            {
                speed2 += attack.MoveSpeed.X;
                Speed.Y += attack.MoveSpeed.Y;
                lastMovedAttack = attack;
            }*/
            for (int i = 0; i < doc.NumberOfObjects; i++)
            {
                o = doc.objArray[i];
                if (o.Attackable && o != this && Math.Abs(Pos.Z - o.Pos.Z) < 10)
                {
                    if (rec.Intersects(o.BoundingBox) || o.BoundingBox.Contains(rec) || rec.Contains(o.BoundingBox))
                    {
                        Rectangle r = o.BoundingBox;
                        Rectangle.Intersect(ref rec, ref o.BoundingBox, out r);
                        bool landed = o.Attacked(attack, this, rec, r);
                        if (landed)
                        {
                            hit = true;
                            var diss = new Disappear(doc, doc.LoadTex("Light2"), r.Center.X, r.Center.Y, Pos.Z) { SortingType = DepthSortingType.Top, SlowDown = false, Color = AttackColor };
                            diss.Scale = new Vector2(attack.Power * 0.01f);
                            diss.ScaleSpeed = new Vector2(MathHelper.Clamp(attack.Power, 5, 50) * 0.02f);
                            diss.FadeInSpeed = 1;
                            diss.Lifetime = 5;
                            diss.FadeOutSpeed = -0.1f;
                            diss.MaxAlpha = 0.8f;
                            diss.Distortion = 0.3f;
                            diss.BlendState = BlendState.Additive;
                            //diss.IsLightSource = true;
                            diss.LightColor = diss.Color;
                            Vector2 speed = MyMath.FromLengthDir(attack.Power, attack.Direction);
                            speed.X *= speedMul;
                            for (int j = 0; j < attack.Power * 2; j++)
                            {
                                /*var d = new Disappear(doc, Textures.SmallLight, r.Center.X, r.Center.Y, Pos.Z)
                                {
                                    Lifetime = 10f,
                                    FadeInSpeed = 1f,
                                    MaxAlpha = 0,
                                    FadeOutSpeed = -0.1f,
                                    Color = AttackColor,
                                    Speed = MyMath.RandomRange(new Vector3(-attack.Power), new Vector3(attack.Power)),
                                    BlendState = BlendState.Additive,
                                    SlowDown = false,
                                    Scale = new Vector2(0.25f)
                                };
                                d.AddBehaviour(new Behaviours.DirectionToRotation(1));*/
                                var d1 = new Disappear(doc, Textures.SmallLight, MyMath.RandomRange(r.Left, r.Right), MyMath.RandomRange(r.Top, r.Bottom), Pos.Z)
                                {
                                    Lifetime = 10f,
                                    FadeInSpeed = 1f,
                                    MaxAlpha = 1,
                                    FadeOutSpeed = -0.1f,
                                    Color = AttackColor,
                                    Speed = new Vector3(speed, 0) + (MyMath.RandomRange(new Vector3(-attack.Power), new Vector3(attack.Power)) / 4),
                                    BlendState = BlendState.Additive,
                                    SlowDown = false,
                                    Scale = new Vector2(0.25f)
                                };
                                d1.AddBehaviour(new Behaviours.DirectionToRotation(1));

                            }
                            var di = new Disappear(doc, Textures.SmallLight, r.Center.X, r.Center.Y, Pos.Z)
                            {
                                Depth = -1000,
                                Scale = new Vector2(attack.Power * 6, 3),
                                Lifetime = 2,
                                FadeInSpeed = 1,
                                FadeOutSpeed = -0.25f,
                                Color = AttackColor,
                                MaxAlpha = 0.75f,
                                BlendState = BlendState.Additive,
                                SlowDown = false,
                                Rotation = new Vector3(0, 0, MyMath.RandomRange(0, 360)),
                                //Bloom = 0.5f,
                                SortingType = DepthSortingType.Top,
                                SunBlockAlpha = 0
                            };
                            var di2 = new Disappear(doc, Textures.SmallLight, r.Center.X, r.Center.Y, Pos.Z)
                            {
                                Depth = -1000,
                                Scale = new Vector2(attack.Power * 6, 4),
                                Lifetime = 2,
                                FadeInSpeed = 1,
                                FadeOutSpeed = -0.125f,
                                Color = AttackColor,
                                MaxAlpha = 0.75f,
                                BlendState = BlendState.Additive,
                                SlowDown = false,
                                Rotation = new Vector3(di.Rotation.X, di.Rotation.Y, di.Rotation.Z + MyMath.RandomRange(30, 150)),
                                SortingType = DepthSortingType.Top,
                                SunBlockAlpha = 0,
                                //Bloom = di.Bloom
                            };
                            /*if (attack.Power > 20)
                            {
                                var di3 = new Disappear(doc, Textures.SmallLight, r.Center.X, r.Center.Y, Pos.Z)
                                {
                                    Depth = -1000,
                                    Scale = new Vector2(100),
                                    Lifetime = 2,
                                    FadeInSpeed = 1,
                                    FadeOutSpeed = -0.25f,
                                    Color = AttackColor,
                                    MaxAlpha = 1,
                                    BlendState = BlendState.Additive,
                                    SlowDown = false,
                                    SortingType = DepthSortingType.Top,
                                    SunBlockAlpha = 0
                                };
                            }*/
                            AttackLanded(o, attack, rec, r);
                            Global.Camera.AddShake(MathHelper.Clamp((float)Math.Pow(attack.Power, 1), 0, 50), MathHelper.Clamp((float)Math.Pow(attack.Power, 1), 0, 40) / 40, 0.05f);
                        }
                        else
                        {
                            AttackMissed(o, attack, rec, r);
                        }
                        
                        /*if (attack.HitSoundEffect != null)
                        {
                            attack.HitSoundEffect.Play(GetSoundVolume(attack.HitVolume), GetSoundPitch(), GetSoundPan());
                        }*/
                        
                    }
                }
            }
            
            if (CurrentCombo.Attacks[attackIndex].HitSoundEffect != null && hit)
            {
                CurrentCombo.Attacks[attackIndex].HitSoundEffect.Play(GetSoundVolume(attack.HitVolume), GetSoundPitch(), GetSoundPan());
            }
            return hit;
        }
        protected virtual void AttackLaunched(Attack attack, Rectangle attackBoundingBox)
        {
            Vector2 vec = MyMath.FromLengthDir(attack.Power, attack.Direction);
            vec.X *= speedMul;
            Elements.Wind w = new Elements.Wind(doc, Pos.X, Pos.Y, Pos.Z);
            w.Speed = new Vector3(vec, 0);
            w.AddBehaviour(new Behaviours.SpeedDissipation(0, 0.9f));
            w.AddBehaviour(new Behaviours.DisappearBehaviour(30, 0.1f, 0.1f, 1));
            w.SetScaleByPixelSize(attackBoundingBox.Width, attackBoundingBox.Height);
            w.Scale *= 2;
            w.ScaleSpeed = new Vector2(0.05f, 0.05f);
        }
        protected virtual void AttackLanded(Obj obj, Attack attack, Rectangle attackBoundingBox, Rectangle attackIntersection)
        {
            if (attack.HitPause)
            {
                float pause = attack.PauseLength;
                if (attack.HitPause)
                {
                    this.Pause(pause, false);
                }
                obj.Pause(pause, false);
                this.Frame = attack.HitFrame;
            }
        }
        protected virtual void AttackMissed(Obj obj, Attack attack, Rectangle attackBoundingBox, Rectangle attackIntersection)
        {

        }
        public override bool Attacked(Attack attack, Obj obj, Rectangle AttackBox, Rectangle intersection)
        {
            if (!Dashing || AllowHurtWhileDashing || (Dashing && obj.PlaySpeed > PlaySpeed * 2))
            {
                
                health -= (attack.Power) * (obj.PlaySpeed / PlaySpeed);
                hurt = (attack.Power / Weight) * (obj.PlaySpeed / PlaySpeed);
                if (health <= 0)
                    Die(attack, obj, intersection);
                speed2 /= MathHelper.Clamp(attack.Power / 3f, 1, 5);
                if (Attacking && attack.KnockBack)
                {
                    if (Weight <= CurrentCombo.Attacks[AttackIndex].Power * (obj.PlaySpeed / PlaySpeed))
                    {
                        EndAttack();
                    }
                }
                if (attack.KnockBack)
                {
                    KnockedBack(obj, attack, AttackBox);
                }
                base.Attacked(attack, obj, AttackBox, intersection);
                return true;
            }
            return false;
        }
        public virtual void KnockedBack(Obj obj, Attack attack, Rectangle attackBox)
        {
            if (Attacking)
                EndAttack();
            float direction = attack.Direction;
            float knockWeight = MathHelper.Clamp(((float)Math.Pow(attack.KnockBackAmount, 0.2) * 5f / Weight) * (obj.PlaySpeed / PlaySpeed), 0, 2);
            float dir = 1;
            if (obj.BoundingBox.Center.X > BoundingBox.Center.X)
            {
                dir = -1;
            }
            Speed += new Vector3(MyMath.LengthDirX(attack.KnockBackAmount * dir * knockWeight, attack.Direction), MyMath.LengthDirY(attack.KnockBackAmount * knockWeight, attack.Direction), 0);
            
            IsKnockedBack = true;
        }

        protected override void SetAnimations()
        {
            if (!Attacking || (OnAWall && !OnTheGround))
            {
                base.SetAnimations();
            }
            else
            {
                if (CurrentCombo != null && CurrentCombo.Animation != null)
                {
                    Animation = CurrentCombo.Animation;
                }
            }
        }
        protected override void SetFramespeed()
        {
            if (!Attacking)
            {
                base.SetFramespeed();
            }
        }
        protected override void OnTheGroundChanged(Vector3 speed)
        {
            if (OnTheGround)
            {
                combatSpeed.Y = 0;
            }
            base.OnTheGroundChanged(speed);
        }
        public override void Draw()
        {
            base.Draw();
            if (false)
            {
                if (Attacking)
                    doc.DrawSprite(doc.LoadTex("TestWall"), ConvertToScreenRec(createAttackRectangle(CurrentCombo.Attacks[attackIndex])), null, Color.White * 0.75f);
                doc.DrawSprite(doc.LoadTex("TestWall"), ConvertToScreenRec(BoundingBox), null, Color.White * 0.20f);
            }
            if (false)
            {
                string drawString = AttackTimer.ToString();
                var vec2 = doc.font1.MeasureString(drawString);
                doc.DrawString(doc.font1, drawString, Pos.ToVector2() + new Vector2(0, -100), Color.Black, 0, vec2 / 2, 1, SpriteEffects.None);
            }
        }
        public override Color GetDrawColor()
        {
            return MyMath.Between(Color, hurtColor, hurt);
        }
    }
    public class Combo
    {
        public TextureFrame[] Animation;
        public List<Attack> Attacks;
        public bool LoopAttack = false;
        public bool LoopAnimation = false;
        public float Framespeed = 0.5f;
        public bool AllowSwitchOut = true;
        public Combo()
        {
            Attacks = new List<Attack>();
        }
    }
    public class Attack
    {
        public float Direction = 0;
        public int StartFrame;
        public int EndFrame;
        public int HitFrame = 1;
        public float Power = 10;
        SoundEffect attS = null, hitS = null;
        public SoundEffect AttackSoundEffect { get { return attS; } }
        public SoundEffect HitSoundEffect { get { return hitS; } }
        public float AttackVolume = 0.5f;
        public float HitVolume = 0.5f;
        public float WaitTime = 0;
        public float TimeOut = 15;
        public Rectangle HitBox = Rectangle.Empty;
        public bool HitOnAllFrames = false;
        public bool KnockBack = false;
        float knockBack = float.NaN;
        public float KnockBackAmount
        {
            get
            {
                if (float.IsNaN(knockBack))
                    return Power;
                else return knockBack;
            }
            set
            {
                knockBack = value;
            }
        }
        float pauseLength = float.NaN;
        public float PauseLength
        {
            get
            {
                if (float.IsNaN(pauseLength))
                    return Power * 1.5f;
                else return pauseLength;
            }
            set
            {
                pauseLength = value;
            }
        }
        public Vector3 MoveSpeed = Vector3.Zero;
        public bool HitPause = true;
        public string AttackSound
        {
            set { attS = Global.Game.LoadSoundEffect(value); }
        }
        public string HitSound
        {
            set { hitS = Global.Game.LoadSoundEffect(value); }
        }
        private string val;
        public string Box
        {
            private set
            {
                val = value;
            }
            get
            {
                return val;
            }
        }
        public Attack(int startFrame, int endFrame, int hitFrame)
        {
            StartFrame = startFrame;
            EndFrame = endFrame;
            HitFrame = hitFrame;
        }
    }
    public enum AttackState { None, Starting, Animating, Hitting, Ending }
}
