 using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if PC
using Microsoft.Xna.Framework.Input;
#endif
namespace SecondShiftMobile
{
    public enum ControlDirection { None, Up, Down, Forward, Back }
    public class Player : CombatObj
    {
        bool checkDashLeft = false, checkDashRight = false;
        protected float RunFramespeedMultiplier = 0.03f;
        protected float JumpSpeed = 20;
        protected float WallJumpSpeed = 20;
        protected float WallJumpXSpeed = 15;
        protected float DashSpeed = 25;
        bool specialAttack;
        public bool Controllable = true;
        bool showHUD = false;
        protected float HUDAlpha = 0;
        float hudTimer = 0;
        /*public bool UpPressed
        {
            get
            {
#if PC
                return (VirtualJoystick.LeftStick.Y > 0.6f || Controls.GetButton(Buttons.DPadUp) == ControlState.Held || Controls.GetKey(Keys.Up) == ControlState.Held);
                
#else
                if (VirtualJoystick.LeftStick.Y < -0.6f)
                {
                    return true;
                }
                else return false;
#endif
            }
        }*/
        protected bool SpecialAttack
        {
            get
            {
                return specialAttack;
            }
            private set
            {
                if (specialAttack != value)
                {
                    specialAttack = value;
                    SpecialAttackChanged();
                }
            }
        }
        public Player(Game1 Doc, TextureFrame Tex, float x, float y, float z)
            : base(Doc, Tex, x, y, z)
        {
            Attackable = true;
            Gravity = 0.8f;
            Depth = -1;
            Global.HUDObj = this;
            AttackColor = new Color(255, 150, 0);
            Bevel = true;
        }
        protected virtual void SpecialAttackChanged()
        {
            if (specialAttack)
            {
                showHUD = true;
                hudTimer = 0;
            }
        }
        public virtual void EndSpecialAttack()
        {
            SpecialAttack = false;
        }
        public virtual void AttackButtonHit(ControlDirection direction)
        {
            NextAttack();
        }
        public virtual void JumpButtonHit()
        {
            if (OnAWall && !OnTheGround)
            {
                WallJump(WallJumpSpeed, WallJumpXSpeed);
            }
            else Jump(JumpSpeed);
        }
        public override void EarlyUpdate()
        {
            if (Controllable)
            {
                #region Controls
#if MONO
            for (int i = 0; i < 4; i++)
            {
                if (Touch.States[i] == TouchScreen.Released)
                {
                    
                    if (Touch.TouchTimers[i] < 15 && Touch.PressLocation[i].X < Global.ScreenSize.X / 2 && (Touch.PressLocation[i] - Touch.MoveLocation2[i]).Length() > 100)
                    {
                        
                    }
                    if (Touch.TouchTimers[i] < 10 && Touch.PressLocation[i].X < Global.ScreenSize.X / 2 && (Touch.MoveLocation2[i] - Touch.PressLocation[i]).Length() > 100)
                    {
                        float dist = MyMath.AngleDistance(Touch.PressLocation[i], Touch.MoveLocation2[i], 270);
                        if (MyMath.AngleDistance(Touch.PressLocation[i], Touch.MoveLocation2[i], 0) < 45 / 2)
                        {
                            DashRight(DashSpeed);
                        }
                        else if (MyMath.AngleDistance(Touch.PressLocation[i], Touch.MoveLocation2[i], 180) < 45 / 2)
                        {
                            DashLeft(DashSpeed);
                        }
                        else if (MyMath.AngleDistance(Touch.PressLocation[i], Touch.MoveLocation2[i], 270) < 45 / 2)
                            Explode();
                    }
                }
                if (Touch.States[i] == TouchScreen.Moved)
                {
                    if (Touch.MoveLocation[i].Y < 100)
                    {
                        Speed.Y -= PlaySpeed;
                    }
                    
                    /*if (Controls.TouchTimers[i] > 2 && Controls.PressLocation[i].X > 640 && (Controls.PressLocation[i] - Controls.MoveLocation[i]).Length() < 2)
                    {
                        if (OnAWall)
                        {
                            WallJump(20, 15);
                        }
                        //else Jump(20);
                    }*/
                }
                if (Touch.States[i] == TouchScreen.Pressed)
                {

                }
                /*if (Controls.States[i] == TouchScreen.Released)
                {
                    if (Controls.TouchTimers[i] <= 5 && (Controls.MoveLocation[i] - Controls.PressLocation[i]).Length() > 20 && Controls.PressLocation[i].X > 640)
                    {
                        if (MyMath.AngleDistance(Controls.PressLocation[i], Controls.MoveLocation2[i], 270) < 45 / 2)
                        {
                            SpecialAttack = !specialAttack;
                        }
                        else JumpButtonHit();
                    }
                }*/
                if (((Touch.TouchTimers[i] == 3 && Touch.States[i] == TouchScreen.Moved) || (Touch.TouchTimers[i] <= 3 && Touch.States[i] == TouchScreen.Released) )
                    && (Touch.MoveLocation2[i] - Touch.PressLocation[i]).Length() < 10 && Touch.PressLocation[i].X > Global.ScreenSize.X / 2)
                {
                    JumpButtonHit();
                }
                if (((Touch.States[i] == TouchScreen.Moved && Touch.TouchTimers[i] == 3) || (Touch.States[i] == TouchScreen.Released && Touch.TouchTimers[i] <= 3))
                    && (Touch.MoveLocation2[i] - Touch.PressLocation[i]).Length() > 10 && Touch.PressLocation[i].X > Global.ScreenSize.X / 2)
                {
                    /*if (MyMath.AngleDistance(Touch.PressLocation[i], Touch.MoveLocation2[i], 90) < 90 / 2)
                    {
                        SpecialAttack = !specialAttack;
                    }
                    else*/ //if (MyMath.AngleDistance(Controls.PressLocation[i], Controls.MoveLocation2[i], 90) < 90 / 2)
                    {
                        ControlDirection dir = ControlDirection.None;
                        if (MyMath.AngleDistance(Touch.PressLocation[i], Touch.MoveLocation2[i], 270) < 45)
                            dir = ControlDirection.Up;
                        else if (MyMath.AngleDistance(Touch.PressLocation[i], Touch.MoveLocation2[i], 0) < 45)
                            dir = speedMul == 1 ? ControlDirection.Forward : ControlDirection.Back;
                        else if (MyMath.AngleDistance(Touch.PressLocation[i], Touch.MoveLocation2[i], 180) < 45)
                            dir = speedMul == -1 ? ControlDirection.Forward : ControlDirection.Back;
                        else if (MyMath.AngleDistance(Touch.PressLocation[i], Touch.MoveLocation2[i], 90) < 45)
                            dir = ControlDirection.Down;
                        AttackButtonHit(dir);
                    }
                }
            }
            if (VirtualJoystick.LeftStick.X < -0.1f)
            {
                MoveLeft(-VirtualJoystick.LeftStick.X);
            }
            if (VirtualJoystick.LeftStick.X > 0.1f)
            {
                MoveRight(VirtualJoystick.LeftStick.X);
            }
#elif PC

                if (Controls.GetKey(Keys.Left) == ControlState.Held || Controls.GetButton(Buttons.DPadLeft) == ControlState.Held)
                {
                    MoveLeft();
                }
                else if (Controls.LeftStick.X < -0.1f)
                {
                    MoveLeft(-Controls.LeftStick.X);
                }
                if (Controls.GetKey(Keys.Right) == ControlState.Held || Controls.GetButton(Buttons.DPadRight) == ControlState.Held)
                {
                    MoveRight();
                }
                else if (Controls.LeftStick.X > 0.1)
                {
                    MoveRight(Controls.LeftStick.X);
                }
                if (Controls.GetKey(Keys.Z) == ControlState.Pressed || Controls.GetButton(Buttons.A) == ControlState.Pressed)
                {
                    JumpButtonHit();
                }
                if (Controls.GetKey(Keys.A) == ControlState.Held || Controls.GetButton(Buttons.Y) == ControlState.Held)
                {
                    Speed.Y -= PlaySpeed;
                }
                #region Dashing
                if (Controls.GetButton(Buttons.LeftThumbstickLeft) == ControlState.Released)
                {
                    if (Controls.GetButtonTime(Buttons.LeftThumbstickLeft) <= 8)
                    {
                        checkDashLeft = true;
                    }
                    else
                    {
                        checkDashLeft = false;
                    }
                }
                if (Controls.GetButton(Buttons.LeftThumbstickLeft) == ControlState.Pressed)
                {
                    if (Controls.GetButtonReleaseTime(Buttons.LeftThumbstickLeft) <= 8 && checkDashLeft)
                    {
                        DashLeft(DashSpeed);
                    }
                    checkDashLeft = false;
                }

                if (Controls.GetButton(Buttons.LeftThumbstickRight) == ControlState.Released)
                {
                    if (Controls.GetButtonTime(Buttons.LeftThumbstickRight) <= 8)
                    {
                        checkDashRight = true;
                    }
                    else
                    {
                        checkDashRight = false;
                    }
                }
                if (Controls.GetButton(Buttons.LeftThumbstickRight) == ControlState.Pressed)
                {
                    if (Controls.GetButtonReleaseTime(Buttons.LeftThumbstickRight) <= 8 && checkDashRight)
                    {
                        DashRight(DashSpeed);
                    }
                    checkDashRight = false;
                }

                if (Controls.GetButton(Buttons.DPadLeft) == ControlState.Released)
                {
                    if (Controls.GetButtonTime(Buttons.DPadLeft) <= 8)
                    {
                        checkDashLeft = true;
                    }
                    else
                    {
                        checkDashLeft = false;
                    }
                }
                if (Controls.GetButton(Buttons.DPadLeft) == ControlState.Pressed)
                {
                    if (Controls.GetButtonReleaseTime(Buttons.DPadLeft) <= 8 && checkDashLeft)
                    {
                        DashLeft(DashSpeed);
                    }
                    checkDashLeft = false;
                }

                if (Controls.GetButton(Buttons.DPadRight) == ControlState.Released)
                {
                    if (Controls.GetButtonTime(Buttons.DPadRight) <= 8)
                    {
                        checkDashRight = true;
                    }
                    else
                    {
                        checkDashRight = false;
                    }
                }
                if (Controls.GetButton(Buttons.DPadRight) == ControlState.Pressed)
                {
                    if (Controls.GetButtonReleaseTime(Buttons.DPadRight) <= 8 && checkDashRight)
                    {
                        DashRight(DashSpeed);
                    }
                    checkDashRight = false;
                }

                if (Controls.GetKey(Keys.Left) == ControlState.Released)
                {
                    if (Controls.GetKeyTime(Keys.Left) <= 8)
                    {
                        checkDashLeft = true;
                    }
                    else
                    {
                        checkDashLeft = false;
                    }
                }
                if (Controls.GetKey(Keys.Left) == ControlState.Pressed)
                {
                    if (Controls.GetKeyReleaseTime(Keys.Left) <= 8 && checkDashLeft)
                    {
                        DashLeft(DashSpeed);
                    }
                    checkDashLeft = false;
                }

                if (Controls.GetKey(Keys.Right) == ControlState.Released)
                {
                    if (Controls.GetKeyTime(Keys.Right) <= 8)
                    {
                        checkDashRight = true;
                    }
                    else
                    {
                        checkDashRight = false;
                    }
                }
                if (Controls.GetKey(Keys.Right) == ControlState.Pressed)
                {
                    if (Controls.GetKeyReleaseTime(Keys.Right) <= 8 && checkDashRight)
                    {
                        DashRight(DashSpeed);
                    }
                    checkDashRight = false;
                }

                #endregion
                if (Controls.GetKey(Keys.V) == ControlState.Pressed || Controls.GetButton(Buttons.LeftTrigger) == ControlState.Pressed)
                {
                    Explode();
                }
                if (Controls.GetKey(Keys.X) == ControlState.Pressed || Controls.GetButton(Buttons.X) == ControlState.Pressed)
                {
                    ControlDirection dir = ControlDirection.None;
                    if (VirtualJoystick.LeftStick.Y > 0.6f || Controls.GetButton(Buttons.DPadUp) == ControlState.Held || Controls.GetKey(Keys.Up) == ControlState.Held)
                        dir = ControlDirection.Up;
                    else if (VirtualJoystick.LeftStick.Y < -0.6f || Controls.GetButton(Buttons.DPadDown) == ControlState.Held || Controls.GetKey(Keys.Down) == ControlState.Held)
                        dir = ControlDirection.Down;
                    else if (VirtualJoystick.LeftStick.X < -0.6f || Controls.GetButton(Buttons.DPadLeft) == ControlState.Held || Controls.GetKey(Keys.Left) == ControlState.Held)
                        dir = speedMul == -1 ? ControlDirection.Forward : ControlDirection.Back;
                    else if (VirtualJoystick.LeftStick.X > 0.6f || Controls.GetButton(Buttons.DPadRight) == ControlState.Held || Controls.GetKey(Keys.Right) == ControlState.Held)
                        dir = speedMul == 1 ? ControlDirection.Forward : ControlDirection.Back;
                    AttackButtonHit(dir);
                }
                if (Controls.GetKey(Keys.Space) == ControlState.Pressed || Controls.GetButton(Buttons.RightTrigger) == ControlState.Pressed)
                {
                    SpecialAttack = true;
                }
                if (Controls.GetKey(Keys.Space) == ControlState.Released || Controls.GetButton(Buttons.RightTrigger) == ControlState.Released)
                {
                    SpecialAttack = false;
                }
#endif
                #endregion
            }
            if (showHUD)
            {
                if (!specialAttack)
                {
                    hudTimer += (Global.FrameSpeed);
                }
                if (hudTimer > 600)
                {
                    showHUD = false;
                    hudTimer = 0;
                }
            }
            if (showHUD)
            {
                HUDAlpha += (0.1f * Global.FrameSpeed);
            }
            else
            {
                HUDAlpha -= (0.016f * Global.FrameSpeed);
            }

            HUDAlpha = MathHelper.Clamp(HUDAlpha, 0, 1);
            
            base.EarlyUpdate();
        }
        public override bool Attacked(Attack attack, Obj obj, Rectangle AttackBox, Rectangle intersection)
        {
            showHUD = true;
            hudTimer = 0;
            return base.Attacked(attack, obj, AttackBox, intersection);
        }
        protected override void AttackLanded(Obj obj, Attack attack, Rectangle attackBoundingBox, Rectangle attackIntersection)
        {
            showHUD = true;
            hudTimer = 0;
            base.AttackLanded(obj, attack, attackBoundingBox, attackIntersection);
        }
        void Explode()
        {
            new Explosion(doc, Pos.X, Pos.Y - 300, 2000, 25, 0.01f, 300, 0.5f, 300, 20);
            //var o = new Exploder(doc, Pos.X, Pos.Y - 300, Pos.Z) { Speed = new Vector3(0, 0, 9) };
            //SmokeEmitter s = new SmokeEmitter(doc, Pos.X, Pos.Y, Pos.Z) { Target = o };

        }
        protected override void SetFramespeed()
        {
            if (!Attacking)
            {
                switch (State)
                {
                    case PlatformerState.Running:
                        Framespeed = speed2 * RunFramespeedMultiplier;
                        break;
                    default:
                        Framespeed = 1;
                        break;
                }
                base.SetFramespeed();
            }
        }
        private class Exploder : Obj
        {
            SmokeEmitter smoke;
            public Exploder(Game1 Doc, float X, float Y, float Z)
                : base(Doc, Doc.LoadTex("Light"), X, Y, Z)
            {
                smoke = new SmokeEmitter(doc, Pos.X, Pos.Y, Pos.Z) { Target = this };
            }
            public override void Update()
            {
                if (Pos.Z > 4000)
                {
                    new Explosion(doc, Pos.X, Pos.Y, Pos.Z, 15, 0.01f, 300, 0.5f, 300, 20);
                    Remove();
                }
                base.Update();
            }
            public override void Remove()
            {
                smoke.Remove();
                base.Remove();
            }
        }
        int statusIndex = 0;
        public override void DrawHUD(Vector2 Pos, float alpha)
        {
            if (HUDAlpha >= 0.01)
            {
                statusIndex = 0;
                //Global.Effects.Technique = Techniques.Normal;
                //foreach (var pass in Global.Effects.CurrentTechnique.Passes)
                {
                    //pass.Apply();
                    DrawStatusIcon(Textures.HealthIcon, Pos, health / maxHealth, Color.White, alpha * HUDAlpha);
                    DrawHUDExtra(Pos, alpha * HUDAlpha);
                }
            }
        }
        protected virtual void DrawHUDExtra(Vector2 pos, float alpha)
        {

        }
        public virtual void DrawStatusIcon(Texture2D tex, Vector2 pos, float width, Color col, float alpha)
        {
            width = MathHelper.Clamp(width, 0, 1);
            Rectangle destRec = new Rectangle(24 + (250 * statusIndex) + (int)pos.X, 24 + (int)pos.Y, 250, 250);
            doc.DrawSprite(tex, new Rectangle(destRec.X + (int)(destRec.Width * width), destRec.Y, destRec.Width - ((int)(destRec.Width * width)), destRec.Height),
                new Rectangle(tex.Width - ((int)(tex.Width * (1 - width))), 0, tex.Width - ((int)(tex.Width * width)), tex.Height),
                Color.Black * alpha);
            doc.DrawSprite(tex, new Rectangle(destRec.X, destRec.Y, (int)(destRec.Width * width), destRec.Height),
                new Rectangle(0, 0, (int)(tex.Width * width), tex.Height),
                col * alpha);
            statusIndex++;
            //doc.spriteBatch.Draw(Textures.HealthIcon, new Rectangle(12, 24, (int)MyMath.Between(0, 75, health / maxHealth), 75), new Rectangle(0, 0, (int)MyMath.Between(0, 100, health / maxHealth), 100), Color.White);
        }

    }
}
