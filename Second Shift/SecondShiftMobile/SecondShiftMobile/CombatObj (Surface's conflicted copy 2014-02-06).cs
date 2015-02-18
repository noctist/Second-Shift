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
        float hurt = 0;
        public bool CanAttack
        {
            get { return hurt < 0.1f; }
        }
        public bool IsHurt
        {
            get { return !CanAttack || IsKnockedBack; }
        }
        /// <summary>
        /// The weight of the object
        /// </summary>
        public float Weight = 10;
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

        public CombatObj(Game1 Doc, Texture2D Tex, float X, float Y, float Z)
            :base(Doc, Tex, X, Y, Z)
        {
            Combos = new List<Combo>();
            AttackTimer = 0;
            Attackable = true;
            IsKnockedBack = false;
        }
        protected virtual void ChangeCurrentCombo(int comboIndex)
        {
            CurrentCombo = Combos[comboIndex];
        }
        protected virtual void ChangeCurrentCombo(Combo combo)
        {
            CurrentCombo = combo;
            attackIndex = 0;
            hitID = -1;
        }

        protected virtual void Die(Attack attack, Obj obj, Rectangle intersection)
        {
            //Remove();
        }
        protected virtual void NextAttack()
        {
            //if (CanAttack)
            {
                if (!Attacking && CanAttack)
                {
                    BeginAttack(0);
                }
                /*else if ((int)Frame >= CurrentCombo.Attacks[AttackIndex].EndFrame && (AttackIndex < CurrentCombo.Attacks.Count - 1 || CurrentCombo.LoopAttack))
                {
                    if (AttackIndex < CurrentCombo.Attacks.Count - 1)
                    {
                        BeginAttack(AttackIndex + 1);
                    }
                    else if (CurrentCombo.LoopAttack)
                    {
                        BeginAttack(0);
                    }
                }*/
                else
                {
                    scheduleAttack = true;
                }
            }
        }

        void nextAttack()
        {
            //if (CanAttack)
            {
                if (!Attacking)
                {
                    BeginAttack(0);
                }
                else

                    if ((int)Frame >= CurrentCombo.Attacks[AttackIndex].EndFrame && AttackTimer > CurrentCombo.Attacks[AttackIndex].WaitTime && (AttackIndex < CurrentCombo.Attacks.Count - 1 || CurrentCombo.LoopAttack))
                    {
                        if (AttackIndex < CurrentCombo.Attacks.Count - 1)
                        {
                            BeginAttack(AttackIndex + 1);
                        }
                        else if (CurrentCombo.LoopAttack)
                        {
                            BeginAttack(0);
                        }
                    }
                    else
                    {
                        //scheduleAttack = true;
                    }
            }
        }

        protected virtual void BeginAttack(int attackIndex)
        {
            if (!Attacking || AttackTimer > 0)
            {
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
                    var c = CurrentCombo.Attacks[AttackIndex];
                    c.AttackSoundEffect.Play(GetSoundVolume(c.AttackVolume), GetSoundPitch(), GetSoundPan());
                }
            }
        }

        protected virtual void EndAttack()
        {
            scheduleAttack = false;
            Attacking = false;
            Frame = 0;
            SetAnimations();
            SetFramespeed();
            hitID = -1;
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
        protected override void OnAWallChanged()
        {
            if (IsKnockedBack && OnAWall)
            {
                Speed.X = 0;
                IsKnockedBack = false;
            }
            base.OnAWallChanged();
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
                        if (AttackHit(CurrentCombo.Attacks[attackIndex]) || !a.HitOnAllFrames)
                            hitID = attackIndex;
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
                            }
                            else
                            EndAttack();
                            return;
                        }
                        nextAttack();
                    }
                    if (ind == AttackIndex)
                    {
                        if (AttackTimer > a.TimeOut)
                        {
                            EndAttack();
                        }
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
                        (int)(Pos.X - (Origin.X * Scale.X)) + attack.HitBox.X,
                        (int)(Pos.Y - (Origin.Y * Scale.Y)) + attack.HitBox.Y,
                        attack.HitBox.Width,
                        attack.HitBox.Height
                    );
            }
            else
            {
                return new Rectangle
                    (
                        (int)(Pos.X - (Origin.X * Scale.X)) + ((Texture.Width - attack.HitBox.X - attack.HitBox.Width)),
                        (int)(Pos.Y - (Origin.Y * Scale.Y)) + attack.HitBox.Y ,
                        attack.HitBox.Width,
                        attack.HitBox.Height
                    );
            }
        }
        private bool AttackHit(Attack attack)
        {
            Obj o;
            bool hit = false;
            Rectangle rec = BoundingBox;
            if (attack.HitBox != Rectangle.Empty)
            {
                rec = createAttackRectangle(attack);
            }
            for (int i = 0; i < doc.NumberOfObjects; i++)
            {
                o = doc.objArray[i];
                if (o.Attackable && o != this && Math.Abs(Pos.Z - o.Pos.Z) < 10)
                {
                    if (rec.Intersects(o.BoundingBox) || o.BoundingBox.Contains(rec) || rec.Contains(o.BoundingBox))
                    {
                        hit = true;
                        Rectangle r = o.BoundingBox;
                        Rectangle.Intersect(ref rec, ref o.BoundingBox, out r);
                        o.Attacked(attack, this, rec, r);
                        var diss = new Disappear(doc, doc.LoadTex("Light2"), r.Center.X, r.Center.Y, Pos.Z) { AlwaysOnTop = true, SlowDown = false, Color = AttackColor };
                        diss.Scale = new Vector2(attack.Power * 0.01f);
                        diss.ScaleSpeed = new Vector2(MathHelper.Clamp(attack.Power, 5, 50) * 0.02f);
                        diss.FadeInSpeed = 1;
                        diss.Lifetime = 5;
                        diss.FadeOutSpeed = -0.1f;
                        diss.MaxAlpha = 0.8f;
                        diss.Distortion = 0.3f;
                        diss.BlendState = BlendState.Additive;
                        AttackLanded(o, attack);
                        /*if (attack.HitSoundEffect != null)
                        {
                            attack.HitSoundEffect.Play(GetSoundVolume(attack.HitVolume), GetSoundPitch(), GetSoundPan());
                        }*/
                        Global.Camera.AddShake(MathHelper.Clamp(attack.Power, 0, 50), MathHelper.Clamp(attack.Power, 0, 50) / 80, 0.05f);
                    }
                }
            }
            if (CurrentCombo.Attacks[attackIndex].HitSoundEffect != null && hit)
            {
                CurrentCombo.Attacks[attackIndex].HitSoundEffect.Play(GetSoundVolume(attack.HitVolume), GetSoundPitch(), GetSoundPan());
            }
            return hit;
        }

        protected virtual void AttackLanded(Obj obj, Attack attack)
        {
            
        }
        public override void Attacked(Attack attack, Obj obj, Rectangle AttackBox, Rectangle intersection)
        {
            if (!Dashing)
            {
                health -= (attack.Power) * (obj.PlaySpeed / PlaySpeed);
                hurt = (attack.Power / Weight) * (obj.PlaySpeed / PlaySpeed);
                if (health <= 0)
                    Die(attack, obj, intersection);
            }
            speed2 /= MathHelper.Clamp(attack.Power / 3f, 1, 5); ;
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
        }
        public virtual void KnockedBack(Obj obj, Attack attack, Rectangle attackBox)
        {
            float direction = attack.Direction;
            float knockWeight = MathHelper.Clamp((attack.Power / Weight), 0, 1);
            float dir = 1;
            if (attackBox.Center.X > BoundingBox.Center.X)
            {
                dir = -1;
            }
            Speed += new Vector3(MyMath.LengthDirX(attack.Power * dir * knockWeight, attack.Direction), MyMath.LengthDirY(attack.Power * knockWeight, attack.Direction), 0) * (obj.PlaySpeed / PlaySpeed);
            
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
        public override void Draw()
        {
            base.Draw();
            if (false)
            {
                if (Attacking)
                    doc.spriteBatch.Draw(doc.LoadTex("TestWall"), ConvertToScreenRec(createAttackRectangle(CurrentCombo.Attacks[attackIndex])), Color.White * 0.75f);
                doc.spriteBatch.Draw(doc.LoadTex("TestWall"), ConvertToScreenRec(BoundingBox), Color.White * 0.2f);
            }
            if (false)
            {
                string drawString = AttackTimer.ToString();
                var vec2 = doc.font1.MeasureString(drawString);
                doc.spriteBatch.DrawString(doc.font1, drawString, zPos + new Vector2(0, -100), Color.Black, 0, vec2 / 2, 1, SpriteEffects.None, 0);
            }
        }
        public override Color GetDrawColor()
        {
            return MyMath.Between(Color, hurtColor, hurt);
        }
    }
    public class Combo
    {
        public Texture2D[] Animation;
        public List<Attack> Attacks;
        public bool LoopAttack = false;
        public float Framespeed = 0.5f;
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
        public float WaitTime = 5;
        public float TimeOut = 15;
        public Rectangle HitBox = Rectangle.Empty;
        public bool HitOnAllFrames = false;
        public bool KnockBack = false;
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
}
