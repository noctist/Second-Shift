using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using SecondShiftMobile.Networking;
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
        Color hurtColor = new Color(200, 0, 0);
        protected float health = 100;
        protected float maxHealth = 100;
        
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
        float gravityMult = 1;
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
            AttackTimer = 0;
            Attackable = true;
            IsKnockedBack = false;
        }
        
        public override void ReceiveSocketMessage(SocketMessage sm)
        {
            base.ReceiveSocketMessage(sm);
            if (sm.Info.BaseAddress == "ChangeCombo")
            {
                ChangeCurrentCombo(Combos[int.Parse(sm.Info["Index"])], true);
            }
            else if (sm.Info.BaseAddress == "NextAttack")
            {
                NextAttack(int.Parse(sm.Info["Index"]));
            }
            else if (sm.Info.BaseAddress == "Die")
            {
                NetworkManager.Log("Received Die message");
                int cIndex = 0, aIndex = 0;
                int.TryParse(sm.Info["cIndex"], out cIndex);
                int.TryParse(sm.Info["aIndex"], out aIndex);
                Rectangle rect = StageObjectPropertyConverter.GetRectangle(sm.Info["rect"]);
                var obj = doc.FindObjectByNetworkId(sm.Info["obj"]);
                Die(obj.Combos[cIndex].Attacks[aIndex], obj, rect);
                NetworkManager.Log("Called Die in CombatObj");
            }
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
                if (Combos.Contains(combo) && !IsNetworkControlled && NetworkID != null)
                {
                    var sm = new SocketMessage();
                    sm.NetworkId = NetworkID;
                    sm.Info.BaseAddress = "ChangeCombo";
                    sm.Info["Index"] = Combos.IndexOf(combo).ToString();
                    sm.Send();
                }
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
            if (!IsNetworkControlled)
            {
                var sm = new SocketMessage();
                sm.NetworkId = NetworkID;
                sm.Info.BaseAddress = "Die";
                sm.Info["rect"] = intersection.ToStageString();
                int comboIndex = 0, attackIndex = 0;
                for (int i = 0; i < obj.Combos.Count; i++)
                {
                    var attIndex = obj.Combos[i].Attacks.IndexOf(attack);
                    if (attIndex != -1)
                    {
                        comboIndex = i;
                        attackIndex = attIndex;
                    }
                }
                sm.Info["cIndex"] = comboIndex.ToString();
                sm.Info["aIndex"] = attackIndex.ToString();
                sm.Info["obj"] = obj.NetworkID;
                sm.Send();
            }
        }

        protected virtual void NextAttack(int attackIndex)
        {
            if (!IsNetworkControlled)
            {
                var sm = new SocketMessage();
                sm.NetworkId = NetworkID;
                sm.Info.BaseAddress = "NextAttack";
                sm.Info["Index"] = attackIndex.ToString();
                sm.Send();
            }
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
                    gravityMult = CurrentCombo.Attacks[AttackIndex].GravityMult;
                    ApplyAttackMovement(CurrentCombo.Attacks[AttackIndex].MoveSpeed);
                    NextAttackOverride();
                    attacksInCurrentCombo++;
                }
            }
        }
        public override Vector3 GetMoveSpeedOverride()
        {
            return base.GetMoveSpeedOverride() + combatSpeed;
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
        protected override bool AllowChangeDir()
        {
            return !Attacking && !IsKnockedBack;
        }
        public override void MoveLeft(float runSpeed = 1)
        {
            base.MoveLeft(calculateRunSpeed(runSpeed));
        }
        float calculateRunSpeed(float runSpeed)
        {
            if (Attacking && CurrentCombo != null)
                runSpeed *= CurrentCombo.RunSpeedMultiplier;
            if (IsKnockedBack)
                runSpeed *= 0.02f;
            return runSpeed;
        }
        public override void MoveRight(float runSpeed = 1)
        {
            base.MoveRight(calculateRunSpeed(runSpeed));
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
            gravityMult += (1 - gravityMult) * 0.1f;
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
            Speed.Y = move.Y;
        }
        public override float GetGravity()
        {
            return base.GetGravity() * gravityMult;
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
                            CreateAttackSparks(attack, r, AttackColor);
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
        protected override void AttackLandedOverride(Obj obj, Attack attack, Rectangle attackBoundingBox, Rectangle attackIntersection)
        {
            if (attack.HitPause)
            {
                this.Frame = attack.HitFrame;
                this.Pause(attack.HitPauseLength, true);
            }
        }
        protected virtual void AttackMissed(Obj obj, Attack attack, Rectangle attackBoundingBox, Rectangle attackIntersection)
        {

        }
        protected override bool AttackedOverride(Attack attack, Obj obj, Rectangle AttackBox, Rectangle intersection)
        {
            if (!Dashing || AllowHurtWhileDashing || (Dashing && obj.PlaySpeed > PlaySpeed * 2))
            {
                if (!IsNetworkControlled)
                {
                    health -= (attack.Power) * (obj.PlaySpeed / PlaySpeed);
                }
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
                base.AttackedOverride(attack, obj, AttackBox, intersection);
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
                doc.DrawSprite(doc.LoadTex("TestWall"), ConvertToScreenRec(new Rectangle((int)Pos.X - 4, (int)Pos.Y - 4, 8, 8)), null, Color.White);
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
        public string Name = null;
        public TextureFrame[] Animation;
        public List<Attack> Attacks;
        public bool LoopAttack = false;
        public bool LoopAnimation = false;
        public float Framespeed = 0.5f;
        public float RunSpeedMultiplier = 0f;
        public bool AllowSwitchOut = true;
        public Combo()
        {
            Attacks = new List<Attack>();
        }
    }
    public enum AttackEffectType { None, Blunt, Sharp }
    public class Attack
    {
        public AttackEffectType EffectType = AttackEffectType.None;
        public float GravityMult = 0.5f;
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
        public bool KnockBack
        {
            get
            {
                return knockBack > 0;
            }
        }
        float knockBack = 0;
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
        public float HitPauseLength
        {
            get
            {
                if (float.IsNaN(pauseLength))
                    return Power * 1.5f;
                else return pauseLength;
            }
            set
            {
                hitPause = true;
                pauseLength = value;
            }
        }
        public Vector3 MoveSpeed = Vector3.Zero;
        bool hitPause = false;
        public bool HitPause
        {
            get
            {
                return hitPause;
            }
        }
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
