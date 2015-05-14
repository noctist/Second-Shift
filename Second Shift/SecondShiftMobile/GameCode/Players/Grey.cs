using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace SecondShiftMobile
{
    public class Grey : Player
    {
        Combo normalCombo, airCombo, dashCombo, punchCombo, upCombo, dashUpCombo;
        SecondShift SecondShift;
        public Grey(Game1 Doc, float X, float Y, float Z)
            :base(Doc, Doc.LoadTex("Characters/Grey/Stand"), X, Y, Z)
        {
            //RotationSpeed.Z = 1;
            //Distortion = 0;
            //Bloom = 1;
            
            topSpeed = 10;
            health = 100;
            SecondShift = new SecondShiftMobile.SecondShift(Doc, this);
            StandSprite = Doc.LoadTex("Characters/Grey/Stand");
            RunAnimation = doc.LoadAnimation("Characters/Grey/Run", "run", 0, 6);
            JumpSprite = doc.LoadTex("Characters/Grey/Jump");
            FallSprite = doc.LoadTex("Characters/Grey/Fall");
            WallSprite = doc.LoadTex("Characters/Grey/Wall");
            JumpSound = doc.LoadSoundEffect("Jump");
            WallJumpSound = doc.LoadSoundEffect("WallJump");
            DashGroundSprite = DashAirSprite = doc.LoadTex("Characters/Grey/Jump");
            JumpSoundVolume = 0.6f;
            WallJumpSoundVolume = 0.4f;
            //Origin.Y = 128;
            BoundingRectangle = new Rectangle(49, 30, 40, 0);
            SlowDown = false;
            normalCombo = new Combo() { Animation = doc.LoadAnimation("Characters/Grey/Combo", "Combos", 0, 12), Framespeed = 0.3f };
            normalCombo.Attacks.Add(new Attack(0, 2, 1) { Power = 10, MoveSpeed = new Vector3(1, 0, 0), AttackSound = "meleemiss1", HitSound = "weakpunch", AttackVolume = 0.5f, HitVolume = 0.5f, HitBox = new Rectangle(64, 45, 50, 40) });
            normalCombo.Attacks.Add(new Attack(3, 4, 4) { Power = 13, MoveSpeed = new Vector3(1, 0, 0), AttackSound = "meleemiss2", HitSound = "mediumpunch", AttackVolume = 0.5f, HitVolume = 0.5f, HitBox = new Rectangle(64, 40, 50, 45) });
            normalCombo.Attacks.Add(new Attack(5, 12, 10) { Power = 20, MoveSpeed = new Vector3(3, 0, 0), Direction = -30, EffectType = AttackEffectType.Blunt, HitPauseLength = 10, AttackSound = "meleemiss3", HitSound = "strongkick", AttackVolume = 0.5f, HitVolume = 0.5f, HitBox = new Rectangle(60, 70, 65, 45), });

            punchCombo = new Combo() { Animation = doc.LoadAnimation("Characters/Grey/Combo", "Combos", 0, 12), Framespeed = 0.3f, LoopAttack = true };
            punchCombo.Attacks.Add(new Attack(0, 2, 1) { Power = 10, AttackSound = "meleemiss1", HitSound = "weakpunch", AttackVolume = 0.5f, HitVolume = 0.5f, HitBox = new Rectangle(64, 45, 50, 40) });
            punchCombo.Attacks.Add(new Attack(3, 4, 4) { Power = 10,  AttackSound = "meleemiss2", HitSound = "mediumpunch", AttackVolume = 0.5f, HitVolume = 0.5f, HitBox = new Rectangle(64, 40, 50, 45) });
            upCombo = new Combo() { Animation = doc.LoadAnimation("Characters/Grey/Combo", "Combos", 0, 12), Framespeed = 0.3f };
            upCombo.Attacks.Add(new Attack(5, 12, 10) { MoveSpeed = new Vector3(10, -10, 0), AttackSound = "meleemiss3", HitSound = "strongkick", HitBox = new Rectangle(70, 70, 55, 45), Power = 25, Direction = -70 });
            airCombo = new Combo() { Animation = doc.LoadAnimation("Characters/Grey/Combo", "Combos", 0, 12), Framespeed = 0.3f };
            airCombo.Attacks.Add(new Attack(5, 12, 10) { Power = 25, EffectType = AttackEffectType.Blunt, MoveSpeed = new Vector3(3, -10, 0), AttackSound = "meleemiss3", HitSound = "strongpunch", AttackVolume = 0.5f, HitVolume = 0.5f, HitBox = new Rectangle(40, 40, 85, 75), HitOnAllFrames = true, TimeOut = 0 });

            dashCombo = new Combo() { Animation = doc.LoadAnimation("Characters/Grey/Combo", "Combos", 0, 12), Framespeed = 0.3f };
            dashCombo.Attacks.Add(new Attack(5, 12, 10) { Power = 30, EffectType = AttackEffectType.Blunt, Direction = -40, AttackSound = "meleemiss3", HitSound = "strongpunch", AttackVolume = 0.5f, HitVolume = 0.5f, HitBox = new Rectangle(30, 40, 95, 75), HitOnAllFrames = true, TimeOut = 0 });

            dashUpCombo = new Combo() { Animation = doc.LoadAnimation("Characters/Grey/Combo", "Combos", 0, 12), Framespeed = 0.3f };
            dashUpCombo.Attacks.Add(new Attack(5, 12, 10) { MoveSpeed = new Vector3(5, -10, 0), Power = 30, Direction = -60, AttackSound = "meleemiss3", HitSound = "strongpunch", AttackVolume = 0.6f, HitVolume = 0.6f, HitBox = new Rectangle(30, 40, 95, 75), HitOnAllFrames = true, TimeOut = 0 });
            //normalCombo.Attacks.Add(new Attack(0, 2, 1) { Power = 10, AttackSound = "meleemiss1", AttackVolume = 0.5f });
            ChangeCurrentCombo(normalCombo);
            AddCombo(normalCombo, dashCombo, airCombo, upCombo, punchCombo);
            
        }
        public override void AttackButtonHit(ControlDirection direction)
        {
            //base.AttackButtonHit();
            if (OnTheGround)
            {
                if (Dashing)
                {
                    if (direction == ControlDirection.Up)
                    {
                        if (CurrentCombo != dashUpCombo)
                            ChangeCurrentCombo(dashUpCombo);
                    }
                    else if (CurrentCombo != dashCombo)
                    {
                        ChangeCurrentCombo(dashCombo);
                    }
                    NextAttack();
                }
                else
                {
                    if (direction == ControlDirection.Up)
                    {
                        if (CurrentCombo != upCombo)
                            ChangeCurrentCombo(upCombo);
                        NextAttack();
                    }
                    else
                    {
                        if (CurrentCombo != normalCombo)
                            ChangeCurrentCombo(normalCombo);
                        NextAttack();
                    }
                }
            }
            else if (!OnAWall)
            {
                if (CurrentCombo != airCombo)
                    ChangeCurrentCombo(airCombo);
                NextAttack();
            }
            
        }
        protected override void SpecialAttackChanged()
        {
            if (SpecialAttack)
            {
                SecondShift.Begin();
            }
            else
            {
                SecondShift.End();
            }
            base.SpecialAttackChanged();
        }
        protected override void AttackLaunched(Attack attack, Rectangle attackBoundingBox)
        {
            
            base.AttackLaunched(attack, attackBoundingBox);
        }
        public override void LateUpdate()
        {
            if (Controls.GetKey(Keys.B) == ControlState.Pressed)
            {
                Pause(60, false);
            }
            //Global.Output = GetScreenPosition(Pos);
            //Remove();
            //Global.Camera.Target = new Players.Carrot(doc, Pos.X, Pos.Y, Pos.Z);
            base.LateUpdate();
        }
        public override void Remove()
        {
            SecondShift.Remove();
            base.Remove();
        }
        protected override void DrawHUDExtra(Vector2 pos, float alpha)
        {
            Color c = SecondShift.GetDrawColor();
            c.A = 255;
            DrawStatusIcon(Textures.TimeIcon, pos, SecondShift.Usability, c, alpha);
            base.DrawHUDExtra(pos, alpha);
        }
        
    }
    public class SecondShift : Obj
    {
        Player player;
        Color c = Color.White;
        Color targetColor;
        float intensity = 0;
        public float Intensity
        {
            get
            {
                return intensity;
            }
        }
        float radius = 0;
        public float Radius
        {
            get
            {
                return radius;
            }
        }
        public float IntensityTarget = 0;
        public float BaseScale = 1.25f;
        public float BaseSlowDown = 0.1f;
        public float FullSlowDownRange = 0.8f;
        float slowDownRadius = 0;
        float restartTimer = 0;
        bool started = false;
        public float MaxAliveTime = 210;
        public float RefillTime = 240;
        float timer = 0, refillTimer = 0;
        bool allowStart = true;
        public float SlowDownRadius
        {
            get
            {
                return slowDownRadius;
            }
        }
        public float Usability
        {
            get
            {
                if (started)
                {
                    return 1 - (timer / MaxAliveTime);
                }
                else
                {
                    return refillTimer / RefillTime;
                }
                return 0;
            }
        }
        SoundEffect speedUpSound, slowDownSound;
        public SecondShift(Game1 Doc, Player p)
            :base(Doc, Doc.LoadTex("Characters/Grey/TimeBall"), p.Pos.X, p.Pos.Y, p.Pos.Z)
        {
            Alpha = 0;
            Distortion = 2f;
            SlowDown = false;
            Name = "Second Shift";
            Depth = p.Depth - 10;
            player = p;
            Color = new Color(100, 255, 255, 50);
            targetColor = Color;
            speedUpSound = doc.LoadSoundEffect("SpeedUp");
            slowDownSound = doc.LoadSoundEffect("SlowDown");
            SortingType = DepthSortingType.Top;
        }
        public void Begin()
        {
            if (!started && allowStart)
            {
                started = true;
                if (Intensity < 0.5)
                    speedUpSound.Play(GetSoundVolume(1), GetSoundPitch(), GetSoundPan());
                IntensityTarget = 1;
            }
        }
        public void End()
        {
            if (started)
            {
                started = false;
                if (Intensity > 0.5)
                    slowDownSound.Play(GetSoundVolume(1), GetSoundPitch(), GetSoundPan());
                IntensityTarget = 0;
                refillTimer = MyMath.Between(RefillTime, 0, MyMath.BetweenValue(0, 1, timer / MaxAliveTime));
                player.EndSpecialAttack();
                if (refillTimer < RefillTime)
                    allowStart = false;
                restartTimer = 0;
            }
        }
        public override void Update()
        {
            //targetColor = Color;
            
            if (started)
            {
                timer += PlaySpeed;
                restartTimer = 0;
                if (timer > MaxAliveTime * 0.5)
                    targetColor = new Color(255, 0, 0, 100);
                if (timer > MaxAliveTime)
                {
                    End();
                }
            }
            else
            {
                restartTimer += PlaySpeed;
                if (refillTimer < RefillTime && restartTimer > 60)
                {
                    refillTimer += PlaySpeed;
                    if (refillTimer > RefillTime * 0.5)
                    {
                        targetColor = Color;
                        allowStart = true;
                    }
                }
                timer = (1 - (refillTimer / RefillTime)) * MaxAliveTime;
            }
            c = MyMath.Between(c, targetColor, 0.05f);
            intensity += (IntensityTarget - intensity) * 0.075f * PlaySpeed;
            Scale.X = Scale.Y = MyMath.Between(0.1f, BaseScale, intensity);
            Alpha = MathHelper.Clamp(intensity * 4, 0, 1); //Alpha = 1;
            radius = Texture.Width * Scale.X * 0.5f;
            Distortion = MyMath.Between(4, 2f, intensity);
            //Distortion = 0;
            slowDownRadius = radius * FullSlowDownRange;
            base.Update();
        }
        public override void LateUpdate()
        {
            base.LateUpdate();
        }
        public override void SetPosition()
        {
            Pos = player.BoundingBox.Center.ToVector2().ToVector3();
            Pos.Z = player.Pos.Z;
            base.SetPosition();
        }
        public override Color GetDrawColor()
        {
            return c;
        }

    }
}
