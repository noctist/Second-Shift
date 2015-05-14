using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Audio;
using SecondShiftMobile.Networking;

namespace SecondShiftMobile
{
    public enum PlatformerState { Standing, Running, Jumping, Falling, Wall, DashingGround, DashingAir }
    public class PlatformerObj : GravityObj
    {
        protected float WallStickCenter = 64;
        float wallTimer = 0;
        float wallTimerMax = 25;
        public PlatformerState State = PlatformerState.Standing;
        protected float speed2 = 0, speed3 = 0;
        protected float acceleration = 0.8f, deceleration = 0.5f;
        protected int speedMul = 1;
        bool dashing = false;
        
        protected bool Dashing
        {
            get
            {
                return dashing;
            }
        }
        /// <summary>
        /// The top horizontal speed of the platformer object
        /// </summary>
        public float topSpeed = 15;

        public TextureFrame[] RunAnimation;

        public TextureFrame FallSprite;
        public TextureFrame[] FallAnimation;

        public TextureFrame JumpSprite;
        public TextureFrame[] JumpAnimation;

        public TextureFrame[] StandAnimation;
        public TextureFrame StandSprite;

        public TextureFrame WallSprite;
        public TextureFrame[] WallAnimation;

        public TextureFrame DashGroundSprite;
        public TextureFrame[] DashGroundAnimation;

        public TextureFrame DashAirSprite;
        public TextureFrame[] DashAirAnimation;

        public SoundEffect JumpSound;
        public float JumpSoundVolume = 1;

        public SoundEffect WallJumpSound;
        public float WallJumpSoundVolume = 1;
        /// <summary>
        /// Gets whether or not the platformer object is on the ground
        /// </summary>
        public bool OnTheGround = false;

        public bool OnAWall = false;

        protected int WallDirection = 1;

        protected Obj FloorObj = null;

        protected Obj WallObj = null;

        float wallJumpMul = 1;

        public PlatformerObj(Game1 Doc, TextureFrame Tex, float X, float Y, float Z)
            : base(Doc, Tex, X, Y, Z)
        {
            Snap = Vector2.One;
            Collidable = true;
            IsPlatformerObj = true;
        }
        float windTimer = 0;
        float maxWindTimer = 8;
        int speedMulPrev = 1;
        public override void EarlyUpdate()
        {
            if (!IsNetworkControlled && NetworkID != null)
            {
                if (speedMul != speedMulPrev)
                {
                    var sm = new SocketMessage();
                    sm.NetworkId = NetworkID;
                    sm.Info.BaseAddress = "MoveDir";
                    sm.Info["Val"] = speedMul.ToString();
                    sm.Send();
                }
                speedMulPrev = speedMul;
            }
            wallJumpMul += (1 - wallJumpMul) * 0.02f;
            speed2 -= deceleration * PlaySpeed * ((dashing) ? 2 : 1) * wallJumpMul;
            if (speed3 != 0)
            {
                speed3 -= (deceleration * PlaySpeed * ((dashing) ? 2 : 1) * wallJumpMul) * (speed3 > 0 ? 1 : -1);
                if (Math.Abs(speed3) < Math.Abs(deceleration))
                    speed3 = 0;
            }
            if (speed2 < 0) { speed2 = 0; dashing = false; }

            //if (!dashing)
            {
                spriteEffect = (speedMul == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            }

            if (OnAWall && Speed.Y > 5)
            {
                Speed.Y -= GetGravity() * PlaySpeed * 2;
            }

            base.EarlyUpdate();
            if (Collidable)
            {
                CheckCollisions();
            }
            if (OnTheGround)
            {
                //Speed.Y = 0;
            }
            
        }
        public override Vector3 GetMoveSpeedOverride()
        {
            return base.GetMoveSpeedOverride() + new Vector3((speed2 + speed3) * speedMul, 0 ,0);
        }
        public override void Update()
        {
            base.Update();
        }
        public override void LateUpdate()
        {
            if (!IsNetworkControlled)
            {
                if (SetState())
                {
                    SetAnimations();
                }
            }

            SetFramespeed();
            if (OnTheGround && FloorObj != null)
                {
                    Pos.X += FloorObj.Pos.X - FloorObj.PreviousPos.X;
                }
            base.LateUpdate();
        }
        public override void ParallelUpdate()
        {
            base.ParallelUpdate();
            Vector3 s = GetMoveSpeed();
            if (OnScreen && s.Length() > 3)
            {
                windTimer += PlaySpeed;
                while (windTimer > maxWindTimer)
                {
                    windTimer -= maxWindTimer;
                    var w = new Elements.Wind(doc, Pos.X, Pos.Y, Pos.Z);
                    w.Speed = s;
                    w.AddBehaviour(new SecondShiftMobile.Behaviours.DisappearBehaviour(60, 0.1f, 0.1f, 1));
                    w.AddBehaviour(new SecondShiftMobile.Behaviours.SpeedDissipation(1, 0.95f));
                    w.SetScaleByPixelSize(BoundingBox.Width, BoundingBox.Height);
                    w.Scale *= 1.5f;
                    w.ScaleSpeed = new Vector2(0.05f, 0.05f);
                }
            }
        }
        protected virtual void CheckCollisions()
        {
            if (!LevelBuilder.Active)
            {
                var speed = GetMoveSpeedOverride();
                Rectangle copyBox = BoundingBox;
                bool onTheGroundPrev = OnTheGround;
                bool onAWallPrev = OnAWall;
                OnTheGround = false;
                OnAWall = false;
                Rectangle testBox = BoundingBox;
                for (int i = 0; i < doc.NumberOfObjects; i++)
                {
                    Obj o = doc.objArray[i];
                    if (o == null)
                        continue;
                    if (o != this && Math.Abs(o.Pos.Z - Pos.Z) < 10)
                    {
                        if (o.Active && o.IsPlatformerObj && o.Collidable)
                        {
                            if (o.BoundingBox.Intersects(BoundingBox))
                            {
                                float move = (Math.Abs(Pos.X - PreviousPos.X) * 0.3f) + (2 * Global.Speed);
                                if (dashing)
                                    move = 0;
                                if (o.BoundingBox.Center.X > BoundingBox.Center.X)
                                {
                                    Pos.X -= move;
                                }
                                else
                                {
                                    Pos.X += move;
                                }
                            }
                        }
                        else if (o.Collidable && o.IsFloor)
                        {
                            //testBox = BoundingBox;
                            // Helper.Write("Object Left, Right: " + o.BoundingBox.Left + ", " + o.BoundingBox.Right + " -  Player Left, Right: " + BoundingBox.Left + ", " + BoundingBox.Right);
                            //if ( Math.Abs(Pos.X - o.Pos.X) < BoundingBox.Width + o.BoundingBox.Width)
                            if (o != WallObj &&
                                    (
                                        (BoundingBox.Left < o.BoundingBox.Right && BoundingBox.Right > o.BoundingBox.Right) ||
                                        (BoundingBox.Left < o.BoundingBox.Left && BoundingBox.Right > o.BoundingBox.Left) ||
                                        (BoundingBox.Left > o.BoundingBox.Left && BoundingBox.Right < o.BoundingBox.Right) ||
                                        BoundingBox.Left == o.BoundingBox.Left || BoundingBox.Right == o.BoundingBox.Right
                                    )
                                && (WallObj == null || ((WallDirection == 1 && WallObj.BoundingBox.Left != o.BoundingBox.Left) || (WallDirection == -1 && WallObj.BoundingBox.Right != o.BoundingBox.Right)))
                                )
                            {
                                //Helper.Write("Object: " + o.BoundingBox + ", Player: " + BoundingBox);
                                if ((o.FloorCollisionType & FloorCollisionType.Top) == SecondShiftMobile.FloorCollisionType.Top && Speed.Y >= 0 && BoundingBox.Y < o.BoundingBox.Y && o.BoundingBox.Y <= BoundingBox.Bottom + (Speed.Y * PlaySpeed))
                                {

                                    for (int y = BoundingBox.Top; y <= BoundingBox.Top + (Speed.Y * PlaySpeed); y += 10)
                                    {
                                        testBox.Y = y;
                                        //Helper.Write("Object: " + o.BoundingBox + ", Player: " + testBox);
                                        //Helper.Write("Object Top, Bottom: " + o.BoundingBox.Top + ", " + o.BoundingBox.Bottom + " -  Player Top, Bottom: " + testBox.Top + ", " + testBox.Bottom);
                                        if (o.BoundingBox.Y < testBox.Bottom && o.BoundingBox.Y > testBox.Top && (WallObj == null || ((WallDirection == 1 && WallObj.BoundingBox.Left != o.BoundingBox.Left) || (WallDirection == -1 && WallObj.BoundingBox.Right != o.BoundingBox.Right))))
                                        {
                                            o.CallCollision(new Vector3(0, Speed.Y, 0));
                                            Speed.Y = 0;
                                            Pos.Y = o.BoundingBox.Top - (BoundingBox.Bottom - Pos.Y) + 1;
                                            //Helper.Write("On the ground");
                                            OnTheGround = true;
                                            FloorObj = o;
                                        }
                                    }
                                }
                                else if ((o.FloorCollisionType & FloorCollisionType.Bottom) == SecondShiftMobile.FloorCollisionType.Bottom && Speed.Y <= 0 && BoundingBox.Bottom > o.BoundingBox.Bottom)
                                {
                                    for (int y = BoundingBox.Top; y >= BoundingBox.Top + Speed.Y; y -= 10)
                                    {
                                        testBox.Y = y;
                                        if (o.BoundingBox.Y < testBox.Y && o.BoundingBox.Bottom > testBox.Y)
                                        {
                                            o.CallCollision(new Vector3(0, Speed.Y, 0));
                                            Speed.Y = 0;
                                            FloorObj = null;
                                        }
                                    }
                                }
                                else
                                {
                                    //FloorObj = null;
                                }
                            }
                            if (o != FloorObj &&
                                    (
                                        (BoundingBox.Top < o.BoundingBox.Bottom && BoundingBox.Bottom > o.BoundingBox.Bottom) ||
                                        (BoundingBox.Top < o.BoundingBox.Top && BoundingBox.Bottom > o.BoundingBox.Top) ||
                                        (BoundingBox.Top > o.BoundingBox.Top && BoundingBox.Bottom < o.BoundingBox.Bottom) ||
                                        BoundingBox.Top == o.BoundingBox.Top || BoundingBox.Bottom == o.BoundingBox.Bottom
                                    )
                                && (FloorObj == null || ((Speed.Y >= 0 && FloorObj.BoundingBox.Top != o.BoundingBox.Top) || (Speed.Y < 0 && FloorObj.BoundingBox.Bottom != o.BoundingBox.Bottom)))
                                )
                            {
                                if ((o.FloorCollisionType & FloorCollisionType.Left) == SecondShiftMobile.FloorCollisionType.Left && GetMoveSpeedOverride().X >= 0 && BoundingBox.X < o.BoundingBox.X - 3 && BoundingBox.Right >= o.BoundingBox.X - 3)
                                {
                                    for (float x = BoundingBox.Right; x <= BoundingBox.Right + Speed.X; x += 10)
                                    {
                                        if (x > o.BoundingBox.Left - 3)
                                        {
                                            o.CallCollision(new Vector3(Speed.X, 0, 0));
                                            //Pos.X = o.BoundingBox.Left - (((BoundingBox.Width + BoundingRectangle.X) - (Origin.X * Scale.X))) + 1;
                                            //Pos.X = o.BoundingBox.Left;
                                            //Pos.X = o.BoundingBox.Left - BoundingBox.Width + 3;
                                            Pos.X = o.BoundingBox.Left - ((Texture.Width - Origin.X - BoundingRectangle.Width) * Scale.X) - 1;
                                            if (speedMul == 1)
                                            {

                                                speed2 = 0;
                                                WallDirection = 1;
                                                //if (!onTheGroundPrev)
                                                OnAWall = true;
                                                WallObj = o;
                                            }
                                            break;
                                        }
                                    }
                                }
                                if ((o.FloorCollisionType & FloorCollisionType.Right) == SecondShiftMobile.FloorCollisionType.Right && GetMoveSpeedOverride().X <= 0 && BoundingBox.Right > o.BoundingBox.Right + 3 && BoundingBox.X <= o.BoundingBox.Right + 3)
                                {
                                    for (float x = BoundingBox.X; x >= BoundingBox.X + Speed.X; x -= 10)
                                    {
                                        if (x < o.BoundingBox.Right + 3)
                                        {
                                            o.CallCollision(new Vector3(Speed.X, 0, 0));
                                            //Pos.X = o.BoundingBox.Right + 1;
                                            //Pos.X = o.BoundingBox.Right + (((BoundingBox.Width + BoundingRectangle.X) - (Origin.X * Scale.X))) + 1; ;
                                            //Pos.X = o.BoundingBox.Right + 1;
                                            Pos.X = o.BoundingBox.Right + ((Texture.Width - Origin.X - BoundingRectangle.Width) * Scale.X) + 1;
                                            if (speedMul == -1)
                                            {

                                                speed2 = 0;
                                                WallDirection = -1;
                                                //if (!onTheGroundPrev)
                                                OnAWall = true;
                                                WallObj = o;
                                            }
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    //WallObj = null;
                                }
                            }
                        }
                    }
                }
                BoundingBox = copyBox;
                AllowGravity = !OnTheGround;
                
                if (onTheGroundPrev != OnTheGround)
                    OnTheGroundChanged(speed);
                if (onAWallPrev != OnAWall)
                    OnAWallChanged(speed);
                if (!OnTheGround)
                    FloorObj = null;
                if (!OnAWall)
                    WallObj = null;
                if (!onAWallPrev && OnAWall)
                {
                    wallTimer = 0;
                }
            }
        }
        protected virtual void OnAWallChanged(Vector3 speed)
        {

        }
        protected virtual bool AllowChangeDir()
        {
            return true;
        }
        protected virtual void OnTheGroundChanged(Vector3 speed)
        {
            if (!OnTheGround && FloorObj != null)
            {
                Speed.Y += FloorObj.Pos.Y - FloorObj.PreviousPos.Y;
                float x = FloorObj.Pos.X - FloorObj.PreviousPos.X;
                if (Math.Abs(x) > 3)
                {
                    /*speed2 += Math.Abs(x);
                    if (x < 0)
                        speedMul = -1;
                    else speedMul = 1;*/
                }
            }
            if (OnTheGround)
            {

            }
        }
        public virtual void MoveRight(float runSpeed = 1)
        {
            if (wallTimer > wallTimerMax || (!OnAWall) || OnTheGround)
            {
                if (OnAWall)
                {
                    Pos.X += BoundingBox.Width;
                }
                if (speedMul == 1)
                {
                    if (!dashing)
                    {
                        if (speed2 < topSpeed && (!OnAWall || WallDirection != 1))
                            speed2 += acceleration * PlaySpeed * runSpeed;
                    }
                    else if (speed2 < 5)
                    {
                        dashing = false;
                    }
                }
                else
                {
                    if (speed2 > 0)
                    {
                        speed2 -= acceleration * 2 * PlaySpeed * runSpeed * wallJumpMul;
                    }
                    else
                    {
                        if (AllowChangeDir())
                        {
                            speedMul = 1;
                        }
                        else
                        {
                            speed2 = 0;
                        }
                    }
                }
            }
            else
            {
                if (WallDirection == -1)
                {
                    wallTimer += Global.Speed;
                    //if (wallTimer > wallTimerMax)
                        //Pos.X += BoundingBox.Width;
                }
            }
        }
        public virtual void MoveLeft(float runSpeed = 1)
        {
            if (wallTimer > wallTimerMax || (!OnAWall) || OnTheGround)
            {
                if (OnAWall)
                {
                    Pos.X -= BoundingBox.Width;
                }
                if (speedMul == -1)
                {
                    if (!dashing)
                    {
                        if (speed2 < topSpeed && (!OnAWall || WallDirection != -1))
                            speed2 += acceleration * PlaySpeed * runSpeed;
                    }
                    else if (speed2 < 5)
                    {
                        dashing = false;
                    }
                }
                else
                {
                    if (speed2 > 0)
                    {
                        speed2 -= acceleration * 2 * PlaySpeed * runSpeed * wallJumpMul;
                    }
                    else
                    {
                        if (AllowChangeDir())
                        {
                            speedMul = -1;
                        }
                        else
                        {
                            speed2 = 0;
                        }
                    }
                }
            }
            else
            {
                if (WallDirection == 1)
                {
                    wallTimer += Global.Speed;
                    //if (wallTimer > wallTimerMax)
                        //Pos.X -= BoundingBox.Width;
                }
            }
        }

        /// <summary>
        /// This will make the object dash to the right with a certain speed
        /// </summary>
        /// <param name="dashSpeed">The speed to dash at</param>
        public virtual void DashRight(float dashSpeed)
        {
            if (!dashing || speed2 < 5 || speedMul != 1)
            {
                speedMul = 1;
                dashing = true;
                speed2 = dashSpeed;
            }
        }

        /// <summary>
        /// This will make the object dash to the right with a certain speed
        /// </summary>
        /// <param name="dashSpeed">The speed to dash at</param>
        public virtual void DashLeft(float dashSpeed)
        {
            if (!dashing || speed2 < 5 || speedMul != -1)
            {
                speedMul = -1;
                dashing = true;
                speed2 = dashSpeed;
            }
        }
        /// <summary>
        /// This will make the object jump with a specified speed
        /// </summary>
        /// <param name="jumpSpeed">This is speed the object will jump with</param>
        public virtual void Jump(float jumpSpeed)
        {
            if (OnTheGround)
            {
                if (JumpSound != null)
                    JumpSound.Play(GetSoundVolume(JumpSoundVolume), GetSoundPitch(), GetSoundPan());
                Speed.Y -= jumpSpeed;
            }
        }

        /// <summary>
        /// Use this to jump while one a wall. This will reverse the direction of the character.
        /// </summary>
        /// <param name="jumpSpeed">The vertical speed</param>
        /// <param name="xSpeed">The horizontal speed</param>
        public virtual void WallJump(float jumpSpeed, float xSpeed)
        {
            if (OnAWall)
            {
                wallJumpMul = 0;
                if (WallJumpSound != null)
                    WallJumpSound.Play(GetSoundVolume(WallJumpSoundVolume), GetSoundPitch(), GetSoundPan());
                speedMul *= -1;
                Speed.Y = -jumpSpeed;
                speed2 = xSpeed;
            }
        }
        protected virtual bool SetState()
        {
            var s = State;
            if (OnTheGround)
            {
                if (dashing)
                {
                    State = PlatformerState.DashingGround;
                }
                else
                if (speed2 > 0.5f)
                {
                    State = PlatformerState.Running;
                }
                else
                {
                    State = PlatformerState.Standing;
                }
            }
            else
            {
                if (dashing)
                {
                    State = PlatformerState.DashingAir;
                }
                else
                if (Speed.Y > 0)
                {
                    State = PlatformerState.Falling;
                }
                else
                {
                    State = PlatformerState.Jumping;
                }
            }
            if (OnAWall && !OnTheGround)
            {
                State = PlatformerState.Wall;
            }
            if (s != State)
            {
                if (!IsNetworkControlled && IsNetworkCapable && !string.IsNullOrWhiteSpace(NetworkID))
                {
                    var sm = new SocketMessage();
                    sm.Info.BaseAddress = "State";
                    sm.Info["Value"] = State.ToString();
                    sm.NetworkId = NetworkID;
                    sm.Send();
                }
                StateChanged(s, State);
            }
            return s != State;
        }
        protected virtual void StateChanged(PlatformerState oldState, PlatformerState newState)
        {

        }
        protected virtual void SetFramespeed()
        {
            
        }
        protected virtual void SetAnimations()
        {
            switch (State)
            {
                case PlatformerState.Running:
                    if (RunAnimation != null)
                        Animation = RunAnimation;
                    break;
                case PlatformerState.Standing:
                    if (StandAnimation != null)
                        Animation = StandAnimation;
                    else if (StandSprite != null)
                    {
                        Animation = null;
                        Texture = StandSprite;
                    }
                    break;
                case PlatformerState.Jumping:
                    if (JumpAnimation != null)
                        Animation = JumpAnimation;
                    else if (JumpSprite != null)
                    {
                        Animation = null;
                        Texture = JumpSprite;
                    }
                    break;
                case PlatformerState.Falling:
                    if (FallAnimation != null)
                        Animation = FallAnimation;
                    else if (FallSprite != null)
                    {
                        Animation = null;
                        Texture = FallSprite;
                    }
                    break;
                case PlatformerState.Wall:
                    if (WallAnimation != null)
                        Animation = WallAnimation;
                    else if (WallSprite != null)
                    {
                        Animation = null;
                        Texture = WallSprite;
                    }
                    break;
                case PlatformerState.DashingGround:
                    if (DashGroundAnimation != null)
                        Animation = DashGroundAnimation;
                    else if (DashGroundSprite != null)
                    {
                        Animation = null;
                        Texture = DashGroundSprite;
                    }
                    break;
                case PlatformerState.DashingAir:
                    if (DashAirAnimation != null)
                        Animation = DashAirAnimation;
                    else if (DashAirSprite != null)
                    {
                        Animation = null;
                        Texture = DashAirSprite;
                    }
                    break;
            }
        }
        public override void ReceiveSocketMessage(SocketMessage sm)
        {
            base.ReceiveSocketMessage(sm);
            if (sm.Info.BaseAddress == "State")
            {
                try
                {
                    var s = State;
                    State = (PlatformerState)Enum.Parse(typeof(PlatformerState), sm.Info["Value"]);
                    if (s != State)
                    {
                        StateChanged(s, State);
                        SetAnimations();
                    }
                }
                catch
                {

                }
            }
            if (sm.Info.BaseAddress == "MoveDir")
            {
                speedMul = int.Parse(sm.Info["Val"]);
            }
        }
    }
}
