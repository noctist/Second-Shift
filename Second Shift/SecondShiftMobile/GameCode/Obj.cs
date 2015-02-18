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
    [Flags]
    public enum Faction { Unknown = 0x0, Good = 0x1, Evil = 0x2, AntiHero = 0x4, Civilian = 0x8 }
    public enum FloorCollisionType { Top = 0x0, Bottom = 0x1, Left = 0x2, Right = 0x4, None = 0x8 }
    public enum DepthSortingType { Bottom, ZDepth, Top }
    public class Obj
    {
        public Faction Faction = Faction.Unknown;
        public PosType PosType = PosType.Normal;
        public Dictionary<string, object> ShaderValues = new Dictionary<string, object>();
        public int OnScreenExpansion = 0;
        public bool UpscaledSecondShiftDraw = false;
        /// <summary>
        /// The amount of distortion the object will cause to the view around it
        /// </summary>
        public float Distortion = 0;
        /// <summary>
        /// Gets or sets whether or not the obect can be slowed down by the Second Shift or some other object
        /// </summary>
        public bool SlowDown = true;
        /// <summary>
        /// A name to identify the object by
        /// </summary>
        public string Name = "";
        /// <summary>
        /// This sets what values the object should snap to in build mode
        /// </summary>
        public Vector2 Snap = new Vector2(128, 128);
        /// <summary>
        /// Sets whether or not the object is run on other threads
        /// </summary>
        public bool Parallel = false;

        /// <summary>
        /// Sets whether or not the object can be attacked
        /// </summary>
        public bool Attackable = false;

        /// <summary>
        /// Gets whether or not remove has been called on this object
        /// </summary>
        public bool RemoveCalled = false;
        public float SunBlockAlpha = 1;
        /// <summary>
        /// Sets whether or not the object can be collided with
        /// </summary>
        public bool Collidable = false;

        /// <summary>
        /// Sets whether or not the object is a floor that will be used for platforming
        /// </summary>
        public bool IsFloor = false;

        /// <summary>
        /// Sets whether or not the object is a platformer object, such as a player or enemy
        /// </summary>
        public bool IsPlatformerObj = false;

        /// <summary>
        /// Sets whether or not the object is a light, and will affect the draw order of the game. Only set this on LightBloom objects, or the game will crash.
        /// </summary>
        public bool IsLight = false;

        /// <summary>
        /// Sets the collision type of the floor
        /// </summary>
        public FloorCollisionType FloorCollisionType = FloorCollisionType.Top | FloorCollisionType.Left | FloorCollisionType.Right;

        /// <summary>
        /// The object's position
        /// </summary>
        public Vector3 Pos = Vector3.Zero;

        /// <summary>
        /// The object's position in the previous frame
        /// </summary>
        public Vector3 PreviousPos = Vector3.Zero;
        /// <summary>
        /// The object's screen position
        /// </summary>
        //protected Vector2 zPos = Vector2.Zero;

        //protected Vector2 previousZPos = Vector2.Zero;


        //protected float zFactor = 1;
        protected Game1 doc;

        /// <summary>
        /// The object's current sprite
        /// </summary>
        public TextureFrame Texture = null;

        /// <summary>
        /// An array of textures that will serve as the object's animation
        /// </summary>
        public TextureFrame[] Animation = null;

        /// <summary>
        /// The speed the object will animate it.
        /// </summary>
        public float Framespeed = 1;

        /// <summary>
        /// The index of the current frame in the animation
        /// </summary>
        protected float Frame = 0;

        /// <summary>
        /// The color the object is drawn with
        /// </summary>
        public Color Color = Color.White;

        /// <summary>
        /// The object's rotation
        /// </summary>
        public Vector3 Rotation = Vector3.Zero;

        /// <summary>
        /// The origin of the object's sprite
        /// </summary>
        public Vector2 Origin = Vector2.Zero;

        /// <summary>
        /// The scale the object will be drawn at
        /// </summary>
        public Vector2 Scale = Vector2.One;

        /// <summary>
        /// The speed the object will move at
        /// </summary>
        public Vector3 Speed = Vector3.Zero;

        /// <summary>
        /// The depth the object will be drawn at against objects at the same Z level
        /// </summary>
        public float Depth = 0;

        /// <summary>
        /// The opacity the object will be drawn with, with 1 being opaque and 0 being transparent
        /// </summary>
        public float Alpha = 1;

        /// <summary>
        /// The speed the object's opacity will change at
        /// </summary>
        public float AlphaSpeed = 0;

        /// <summary>
        /// The speed the object rotates at
        /// </summary>
        public Vector3 RotationSpeed = Vector3.Zero;

        /// <summary>
        /// The speed at which the object's size changes
        /// </summary>
        public Vector2 ScaleSpeed = Vector2.Zero;

        /// <summary>
        /// This tells you whether or not the object is visible on screen
        /// </summary>
        public bool OnScreen = false;

        /// <summary>
        /// The current game play speed of the object (ex. slow motion)
        /// </summary>
        public float PlaySpeed = 1;

        /// <summary>
        /// The level of light bloom (glow) the object will be drawn with
        /// </summary>
        public float Bloom = 0;

        /// <summary>
        /// Gets whether or not the object is active
        /// </summary>
        public bool Active = false;

        /// <summary>
        /// Setting whether or not the object should be deactivated when off screen to save performance
        /// </summary>
        public bool DeactivateOffscreen = false;

        /// <summary>
        /// Gets or sets whether or not the object is drawn
        /// </summary>
        public bool Visible = true;

        public BlendState BlendState = BlendState.AlphaBlend;
        public SamplerState SamplerState = SamplerState.LinearClamp;

        public Rectangle ScreenRect = new Rectangle();
        public float ScreenAlpha = 1;

        public bool AlwaysOnTop { get { return SortingType == DepthSortingType.Top; } }
        public bool AlwaysOnBottom { get { return SortingType == DepthSortingType.Bottom; } }
        public DepthSortingType SortingType = DepthSortingType.ZDepth;

        public SpriteEffects spriteEffect = SpriteEffects.None;

        public Rectangle BoundingBox = new Rectangle();

        public Rectangle BoundingRectangle = new Rectangle();

        public Point Center = new Point();

        float slowSpeed = 1, secondShiftAlpha = 0, secondShiftAlphaTarget = 0;

        bool FirstUpdate = false;

        public bool Bevel = false;
        public float BevelGlow = 0;
        public Vector2 BevelDeltaMultiplier = Vector2.One;

        public bool IsLightSource = false;
        public float LightIntensity = 1;
        public float LightFallOff = 500;
        public float LightFallOffRange = 100;
        public Color LightColor = Color.White;

        public Vector2 TextureScale = Vector2.One;

        public float ZWidth = 10;

        public List<Behaviours.Behaviour> Behaviours;

        public bool AlwaysChangeSpriteBatchAfter = false;

        public bool IsBeveling
        {
            get
            {
                return isBeveling;
            }
        }

        float pauseFrames = 0, maxPauseFrames = 0;
        bool multiplyPauseSpeed = false;
        bool pauseRequested = false;
        public bool IsPaused { get; private set; }

        protected bool isBeveling = false; protected Vector2 bevelDelta = Vector2.Zero; protected float bevelIntensity = 0; protected Color bevelColor = Color.White;

        public Obj(Game1 Doc, Texture2D Tex, float x, float y, float z)
            :this(Doc, TextureFrame.FromTexture(Tex), x, y, z)
        {

        }
        public Obj(Game1 Doc, TextureFrame Tex, float x, float y, float z)
        {
            Active = true;
            this.Texture = Tex;
            if (Tex != null)
            {
                Origin = new Vector2(Tex.Width / 2, Tex.Height / 2);
            }
            doc = Doc;
            Pos = new Vector3(x, y, z);
            doc.AddObj(this);
        }
        public virtual StageObjectProperty[] GetStageProperties()
        {
            return null;
        }
        public virtual void SetStageProperties(params StageObjectProperty[] sop)
        {

        }
        protected float GetSlowSpeed()
        {
            secondShiftAlphaTarget = 0;
            if (SlowDown)
            {
                Obj o = doc.FindObjectByName("Second Shift");
                
                if (o != null)
                {
                    SecondShift s = o as SecondShift;
                    if (s.Intensity > 0.1 && Math.Abs(Pos.Z - s.Pos.Z) < s.Radius && Math.Abs(Pos.X - s.Pos.X) < s.Radius & Math.Abs(Pos.Y - s.Pos.Y) < s.Radius)
                    {
                        //float dist = BoundingBox.MinDistance(Pos.Z, s.Pos);
                        float dist = (Pos - s.Pos).Length();
                        //Helper.Write(dist);
                        /*float d = (new Vector2(BoundingBox.Center.X, BoundingBox.Center.Y) - s.Pos.ToVector2()).Length();
                        if (d < s.Radius && d < )
                        {

                        }*/
                        if (dist < s.Radius)
                        {
                            if (dist > s.SlowDownRadius)
                            {
                                float f = MyMath.BetweenValue(s.SlowDownRadius, s.Radius, dist);
                                secondShiftAlphaTarget = 1;
                                float r = MyMath.BetweenValue(s.BaseSlowDown, 1, f);
                                if (r < s.BaseSlowDown)
                                    r = s.BaseSlowDown;
                                return r;
                            }
                            else
                            {
                                secondShiftAlphaTarget = 1;
                                return s.BaseSlowDown;
                            }
                        }
                        else
                        {
                            return 1;
                        }
                    }
                    else
                    {
                        return 1;
                    }
                }
                else
                {
                    return 1;
                }
            }
            else
            {
                return 1;
            }
        }
        public virtual Vector3 GetMoveSpeed()
        {
            return Speed;
            
        }
        public void Pause(float howManyFrames, bool multiplyByGameSpeed)
        {
            pauseFrames = 0;
            maxPauseFrames = howManyFrames;
            multiplyPauseSpeed = multiplyByGameSpeed;
            pauseRequested = true;
        }
        public void IncrementPause()
        {
            pauseFrames += 1 * ((multiplyPauseSpeed) ? PlaySpeed : 1);
            if (pauseFrames >= maxPauseFrames)
            {
                IsPaused = false;
            }
        }
        public virtual void EarlyUpdate()
        {
            if (!FirstUpdate)
            {
                slowSpeed = GetSlowSpeed();
                FirstUpdate = true;
            }
            else
            {
                slowSpeed += (GetSlowSpeed() - slowSpeed) * 0.1f;
            }
            secondShiftAlpha += (secondShiftAlphaTarget - secondShiftAlpha) * 0.1f;
            PlaySpeed = Global.Speed * slowSpeed;
            PreviousPos = Pos;
            Pos += GetMoveSpeed() * PlaySpeed;
            Rotation += RotationSpeed * PlaySpeed;
            Scale += ScaleSpeed * PlaySpeed;
            Alpha += AlphaSpeed * PlaySpeed;
            //if (Collidable)
            {
                SetBoundingBox();
            }
        }
        public virtual void Update()
        {

            
        }
        public virtual void LateUpdate()
        {
            SetPosition();
            if (Animation != null)
            {
                if (!pauseRequested && !IsPaused)
                {
                    Frame += Framespeed * PlaySpeed;
                }
                /*if ((int)Frame >= Animation.Length)
                {
                    Frame = 0;
                }*/
                Texture = Animation[(int)Frame % Animation.Length];
            }
            if (pauseRequested)
            {
                pauseRequested = false;
                IsPaused = true;
            }
        }
        public virtual void ParallelUpdate()
        {
            if (Behaviours != null)
            {
                
                for (int i = 0; i < Behaviours.Count; i++)
                {
                    Behaviours[i].Update(this);
                    Speed = Behaviours[i].Speed(this, Speed);
                }
            }
            SetScreenAlpha();
        }

        /// <summary>
        /// This update will run in the background for operations that don't need to be performed each frame
        /// </summary>
        public virtual void NonCriticalUpdate()
        {
            if (Bevel)
                SetBevelData();
            else
            {
                isBeveling = false;
            }
        }
        public void RemoveBehaviour(Behaviours.Behaviour behaviour)
        {
            if (Behaviours != null)
            {
                Behaviours.Remove(behaviour);
            }
        }
        public void AddBehaviour(Behaviours.Behaviour behaviour)
        {
            if (Behaviours == null)
            {
                Behaviours = new List<Behaviours.Behaviour>(10);
            }
            Behaviours.Add(behaviour);
        }

        

        protected virtual void SetBevelData()
        {
            Obj light = null;
            float dist = float.MaxValue;
            for (int i = 0; i < doc.NumberOfObjects; i++)
            {
                float d = Vector3.Distance(doc.objArray[i].Pos, Pos);
                if (doc.objArray[i] != null && doc.objArray[i].IsLightSource && d < dist)
                {
                    light = doc.objArray[i];
                    dist = d;
                }
            }
            if (light != null && dist < light.LightFallOff + light.LightFallOffRange)
            {
                
                isBeveling = true;
                bevelIntensity = MathHelper.Clamp(MyMath.BetweenValue(light.LightFallOffRange + light.LightFallOff, light.LightFallOff, dist), 0, 1) * light.LightIntensity;
                bevelDelta = ((light.GetScreenPosition(light.Pos) - GetScreenPosition(Pos)) / 2000) * new Vector2((this.spriteEffect == SpriteEffects.FlipHorizontally) ? -1 : 1, 1) * BevelDeltaMultiplier;
                bevelColor = light.LightColor;
            }
            else
            {
                isBeveling = false;
            }
        }

        /// <summary>
        /// Sets the object's screen position. This is different from it's world position.
        /// </summary>
        public virtual void SetPosition()
        {
            /*previousZPos = zPos;
            float zdiff = Camera.GetZDiff(Global.Camera.View.Z, Pos.Z, Global.Camera.DepthSize);
            zFactor = Camera.GetZFactor(zdiff, Global.Camera.DepthSize, Global.Camera.DepthPower);
            if (zdiff < 50)
            {
                ScreenAlpha = (zdiff) / 50;
            }
            else ScreenAlpha = 1;
            zPos = Camera.GetScreenPosition(Pos, Global.Camera.View, Global.Camera.DepthSize, Global.Camera.DepthPower);*/
            //zPos.X = Global.Camera.View.X + ((Pos.X - Global.Camera.View.X) * zFactor);
            //zPos.Y = Global.Camera.View.Y + ((Pos.Y - Global.Camera.View.Y) * zFactor);
            //zPos.X = Pos.X;
            //zPos.Y = Pos.Y;
            SetOnScreen();
        }

        protected virtual void SetScreenAlpha()
        {
            ScreenAlpha = MathHelper.Clamp(MyMath.BetweenValue(Global.Camera.View.Z - 600, Global.Camera.View.Z - 300, Pos.Z), 0, 1);
        }
        public virtual Vector2 GetScreenPosition(Vector3 pos)
        {
            return Global.Camera.GetScreenPosition(pos);
            /*var os = new Vector2();
            os.X = Global.Camera.View.X + ((pos.X - Global.Camera.View.X) * zFactor);
            os.Y = Global.Camera.View.Y + ((pos.Y - Global.Camera.View.Y) * zFactor);
            return os;*/
        }
        public virtual void SetScreenPosition(Vector2 vec)
        {
            float zFactor = Camera.GetZFactor(Global.Camera.View.Z, Pos.Z, Global.Camera.DepthSize, Global.Camera.DepthPower);
            Vector2 v = (Global.Camera.View.ToVector2()) + (((vec * (Global.Camera.CameraSize / Global.ScreenSize)) - (Global.Camera.CameraSize / 2)) / zFactor);
            //v = vec;
            Pos.X = v.X;
            Pos.Y = v.Y;
        }
        protected Rectangle ConvertToScreenRec(Rectangle rec)
        {
            float zFactor = Camera.GetZFactor(Global.Camera.View.Z, Pos.Z, Global.Camera.DepthSize, Global.Camera.DepthPower);
            return new Rectangle((int)(Global.Camera.View.X + ((rec.X - Global.Camera.View.X) * zFactor)), (int)(Global.Camera.View.Y + ((rec.Y - Global.Camera.View.Y) * zFactor)), (int)(rec.Width * zFactor), (int)(rec.Height * zFactor));
        }
        /// <summary>
        /// Overridable method used on Floor objects, where you can make it do something when an object collides with it
        /// </summary>
        /// <param name="speed"></param>
        public virtual void CallCollision(Vector3 speed)
        {

        }

        public float GetSoundPitchDoppler()
        {
            var currentDist = (Pos - Global.Camera.View).Length();
            var prevDist = (PreviousPos - Global.Camera.PrevView).Length();
            var doppler = (prevDist - currentDist) / 200;
            return MathHelper.Clamp(((PlaySpeed * 2 / Global.FrameSpeed) - 2) + doppler, -1, 1);
        }

        public float GetSoundPitch()
        {
            return MathHelper.Clamp(((PlaySpeed * 2 / Global.FrameSpeed) - 2), -1, 1);
        }
        public float GetSoundVolume(float Multiplier = 1, float maxDist = 3000)
        {
            maxDist *= Multiplier;
            //return MathHelper.Clamp((Global.ScreenSize.X / 2) * Multiplier / (Vector3.Distance(Pos, Global.Camera.View) + (Global.ScreenSize.X / 2)), 0, 1);
            return MathHelper.Clamp((float)Math.Pow((maxDist - Vector3.Distance(Pos, Global.Camera.View)) / maxDist, 1) * Multiplier, 0, 1);
        }
        public float GetSoundPan()
        {
            
            Vector2 zPos = Global.Camera.GetScreenPosition(Pos);
            return MathHelper.Clamp((zPos.X - Global.Camera.View.X) / (Global.Camera.CameraSize.X / 2), -1, 1);
        }

        /// <summary>
        /// This method is called when the object is attacked
        /// </summary>
        /// <param name="attack"></param>
        public virtual bool Attacked(Attack attack, Obj obj, Rectangle attackBox, Rectangle interection)
        {
            return true;
        }

        public Obj[] GetCollisions()
        {
            List<Obj> obj = new List<Obj>();
            for (int i = 0; i < doc.NumberOfObjects; i++)
            {
                if (doc.objArray[i] != this && IsCollidingWith(doc.objArray[i]))
                {
                    obj.Add(doc.objArray[i]);
                }
            }
            return obj.ToArray();
        }

        public bool IsCollidingWith(Obj o)
        {
            return o != this && o.Active && o.Collidable 
                && ((o.Pos.Z + (o.ZWidth / 2) <= Pos.Z + (ZWidth / 2) && o.Pos.Z + (o.ZWidth / 2) >= Pos.Z - (ZWidth / 2))
                || (Pos.Z + (ZWidth / 2) <= o.Pos.Z + (o.ZWidth / 2) && Pos.Z + (ZWidth / 2) >= o.Pos.Z - (o.ZWidth / 2)))
                && (BoundingBox.Intersects(o.BoundingBox) || o.BoundingBox.Contains(BoundingBox) || BoundingBox.Contains(o.BoundingBox));
        }

        /// <summary>
        /// This method sets whehter or not the object is considered to be on screen
        /// </summary>
        protected virtual void SetOnScreen()
        {
            if (Texture != null)
            {
                var zFactor = Camera.GetZFactor(Global.Camera.View.Z, Pos.Z, Global.Camera.DepthSize, Global.Camera.DepthPower);
                var zPos = Global.Camera.GetScreenPosition(Pos);
                float maxWid = (Texture.Width > Texture.Height) ? Texture.Width : Texture.Height;
                ScreenRect = new Rectangle((int)(zPos.X - (Origin.X * Scale.X * zFactor)), (int)(zPos.Y - (Origin.Y * Scale.Y * zFactor)), (int)(Texture.Width * zFactor * Scale.X), (int)(Texture.Height * zFactor * Scale.Y));
                //Vector2 Offset = 
                var screenRect = Global.ScreenRect;
                screenRect.Inflate(OnScreenExpansion, OnScreenExpansion);
                OnScreen = ScreenRect.Intersects(screenRect);
                if (DeactivateOffscreen)
                {
                    Active = OnScreen;
                }
            }
            else
            {
                //Pos.Length();
                OnScreen = false;
            }
        }
        /// <summary>
        /// Set the properies of the object using a semicolon divided string
        /// </summary>
        /// <param name="prop">The string of properties to be changed. Separate each propery with ';' and separate the property name and property value with '=' with the name coming first</param>
        public void SetPropertiesFromString(string prop)
        {
            Global.SetPropertiesFromString(this, prop);
            
        }
        public virtual bool IsInsideScreenRect(Vector2 vec)
        {
            return vec.X > ScreenRect.Left && vec.Y > ScreenRect.Top && vec.X < ScreenRect.Right && vec.Y < ScreenRect.Bottom;
        }
        /// <summary>
        /// This will remove the object from the game
        /// </summary>
        public virtual void Remove()
        {
            RemoveCalled = true;
            Active = false;
            doc.RemoveObj(this);
        }

        public virtual void SetScaleByPixelSize(float X, float Y)
        {
            Scale = new Vector2(X / Texture.Width, Y / Texture.Height);
        }

        public virtual void QualityChanged(Quality quality)
        {

        }
        /// <summary>
        /// An overridable method that will set the object's bounding box
        /// </summary>
        public virtual void SetBoundingBox()
        {
            if (spriteEffect == SpriteEffects.None)
            {
                BoundingBox = new Rectangle((int)(Pos.X - ((Origin.X - BoundingRectangle.X) * Scale.X)), 
                    (int)(Pos.Y - ((Origin.Y - BoundingRectangle.Y) * Scale.Y)), 
                    (int)((Texture.Width - BoundingRectangle.Width - BoundingRectangle.X) * Scale.X), 
                    (int)((Texture.Height - BoundingRectangle.Height - BoundingRectangle.Y) * Scale.Y));
            }
            else if (spriteEffect == SpriteEffects.FlipHorizontally)
            {
                BoundingBox = new Rectangle((int)(Pos.X - ((Origin.X) * Scale.X)) + (int)((BoundingRectangle.Width) * Scale.X), 
                    (int)(Pos.Y - ((Origin.Y - BoundingRectangle.Y) * Scale.Y)), 
                    (int)((Texture.Width - BoundingRectangle.Width - BoundingRectangle.X) * Scale.X), 
                    (int)((Texture.Height - BoundingRectangle.Height - BoundingRectangle.Y) * Scale.Y));
            }
            Center = BoundingBox.Center;
        }
        protected virtual void SetDepth()
        {
            Global.Effects.DrawDepth = GetDepth();
        }
        public virtual float GetDepth()
        {
            return MathHelper.Clamp(MyMath.BetweenValue(-Global.Effects.NearZPlane, -Global.Effects.FarZPlane, Pos.Z - Global.Camera.View.Z), 0, 1);
        }
        public virtual float GetDistortion()
        {
            return Distortion;
        }
        protected virtual void SetBloom()
        {
            Global.Effects.ObjBloom = Bloom;
        }
        protected virtual void SetDistortion()
        {
            Global.Effects.ObjDistort = GetDistortion(); ;
        }
        protected virtual void SetBevel()
        {
            //if (Bevel && bevelIntensity > 0)
            {
                //Global.Effects.BevelDelta = bevelDelta.Rotate(-Rotation.Z);
                //Global.Effects.BevelColor = bevelColor.ToVector3() * bevelIntensity;
                //Global.Effects.BevelGlow = BevelGlow;
            }
            //Global.Effects.BevelDelta = new Vector2(0.1f);
        }
        public virtual BevelData GetBevel()
        {
            BevelData b;
            if (isBeveling)
            {
                b.BevelDelta = bevelDelta.Rotate(-Rotation.Z);
                b.BevelColor = bevelColor.ToVector3() * bevelIntensity;
                b.BevelGlow = BevelGlow;
                b.IsBeveling = true;
            }
            else
            {
                b.BevelDelta = new Vector2();
                b.BevelColor = Vector3.Zero;
                b.BevelGlow = 0;
                b.IsBeveling = false;
            }
            return b;
        }
        public virtual Techniques GetDrawTechnique()
        {
            if (!isBeveling)
            {
                return Techniques.NormalDepth;
                if (Global.Effects.Blurring || doc.objBlooming || doc.Distort)
                {
                    return Techniques.NormalDepth;
                }
                else
                {
                    return Techniques.Normal;
                }
            }
            else if (Global.Effects.Quality >= Quality.Medium)
            {
                return Techniques.Bevel;
            }
            return Techniques.Normal;
        }
        protected virtual void SetDrawTechniques()
        {
            //Global.Effects.Technique = GetDrawTechnique();
            //if (Global.Effects.Blurring || doc.objBlooming || doc.Distort)
            {
                //SetDepth();
                //SetBloom();
                //SetDistortion();
            }
            if (isBeveling)
            {
                //SetBevel();
            }
        }
        public virtual bool CheckIfSpriteBatchChangeNeeded(Obj o)
        {
            //return true;
            return (o.BlendState != BlendState)
                || o.Rotation != Rotation
                || o.AlwaysChangeSpriteBatchAfter
                || (Global.Effects.Blurring && (Math.Abs(GetDepth() - o.GetDepth()) * Global.Effects.Blur > 0.0001f))
                || (doc.Distort && (GetDistortion() != o.GetDistortion())
                || ((doc.objBlooming || Global.Effects.Blooming) && Math.Abs(Bloom - o.Bloom) > 0.01f)
                || (isBeveling && (Global.Effects.BevelDelta != bevelDelta))
                || (GetDrawTechnique() != o.GetDrawTechnique())
                );
        }
        public virtual void Draw()
        {
            //Global.Effects.SetObjValues(this);
            //SetPosition();
            //if (tex != null && !tex.IsDisposed && screenAlpha > 0 && onScreen)
            {
                //SetDrawTechniques();
                //doc.GraphicsDevice.SamplerStates[0] = SamplerState;
                //doc.GraphicsDevice.BlendState = BlendState;
                //foreach (var pass in Global.Effects.CurrentTechnique.Passes)
                {
                    //pass.Apply();
                    DrawObj();

                    //doc.spriteBatch.Draw(doc.LoadTex("TestWall"), ConvertToScreenRec(BoundingBox), Color.White * 0.5f);
                }
            }

            //previousZPos = zPos;
        }

        protected virtual void DrawObj()
        {
            Global.Drawer.DrawObj(this);
            //doc.DrawSprite(Texture, Vector3.Zero, GetDrawColor() * GetDrawAlpha(), spriteEffect);
            //doc.DrawSprite(Textures.WhiteBlock, Vector3.Zero, Color.White, new Vector2(0.1f), spriteEffect);
        }
        public virtual void DrawSecondShift()
        {
            if (ShouldDrawSecondShift())
            {
                if (UpscaledSecondShiftDraw)
                    SetDrawTechniques();
                else SetSecondShiftTechnique();
                doc.GraphicsDevice.BlendState = BlendState.Additive;
                foreach (var pass in Global.Effects.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    if (UpscaledSecondShiftDraw)
                    {
                        doc.DrawSprite(Texture, Pos, new Color(0, 255, 255) * GetDrawAlpha() * secondShiftAlpha * 8, Rotation.Z, Origin, GetDrawScale(), spriteEffect);
                    }
                    else
                    {
                        doc.DrawSprite(Texture, Pos, new Color(0, 255, 255) * GetDrawAlpha() * secondShiftAlpha * 8, Rotation.Z, Origin, GetDrawScale(), spriteEffect);
                    }
                }
            }
        }
        public virtual bool ShouldDrawSecondShift()
        {
            return secondShiftAlpha > 0.01f;
        }
        public virtual void SetSecondShiftTechnique()
        {
            Global.Effects.Technique = Techniques.SlowDown;
        }
        public virtual void DrawScreenRect()
        {
            Global.Effects.Technique = Techniques.Normal;
            //doc.GraphicsDevice.BlendState = BlendState;
            foreach (var pass in Global.Effects.CurrentTechnique.Passes)
            {
                pass.Apply();
                //doc.DrawSprite(Texture, zPos, Color, Rotation, Origin, Scale * zFactor, spriteEffect, 0);
                doc.DrawSprite(doc.LoadTex("TestWall"), ScreenRect, null, Color * 0.2f);
            }
        }
        public virtual void SelectedDraw()
        {
            Global.Effects.Technique = Techniques.Selected;
            //Global.Effects.SetObjValues(this);
            foreach (var pass in Global.Effects.CurrentTechnique.Passes)
            {
                pass.Apply();
                //doc.GraphicsDevice.BlendState = BlendState.Additive;
                doc.DrawSprite(Texture, Vector3.Zero, GetDrawColor() * GetDrawAlpha(), spriteEffect);
                doc.DrawSprite(Texture, Pos.ToVector2(), GetDrawColor() * GetDrawAlpha(), Rotation.Z, Origin, GetDrawScale(), spriteEffect, -Pos.Z);

            }
        }
        public virtual Vector2 GetDrawScale()
        {
            //return Scale;
            return Scale;
        }
        public virtual void SetBloomTechnique()
        {
            Global.Effects.Technique = Techniques.Normal;
        }
        public virtual void BloomDraw(float alpha)
        {
            SetBloomTechnique();
            Global.Effects.SetObjValues(this);
            foreach (var pass in Global.Effects.CurrentTechnique.Passes)
            {
                pass.Apply();
                doc.DrawSprite(Texture, Vector3.Zero, Color.Black * GetDrawAlpha() * SunBlockAlpha, spriteEffect);
                //doc.DrawSprite(Texture, Pos, Color.Black * GetDrawAlpha() * SunBlockAlpha, Rotation.Z, Origin, GetDrawScale(), spriteEffect);
            }
        }

        public virtual float GetDrawAlpha()
        {
            return  Alpha * ScreenAlpha;
        }

        public virtual Color GetDrawColor()
        {
            return MyMath.Between(Color, Global.Effects.FogColor, MathHelper.Clamp(MyMath.BetweenValue(Global.Effects.FogStartDistance, Global.Effects.FogStartDistance + Global.Effects.FogDistance, Pos.Z - Global.Camera.View.Z), 0, 1) * Global.Effects.FogIntensity);
        }
        public virtual void DrawHUD(Vector2 pos, float alpha)
        {

        }
    }
}
