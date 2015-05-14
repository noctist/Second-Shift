using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using SecondShiftMobile.Networking;
using System.ComponentModel;
namespace SecondShiftMobile
{
    [Flags]
    public enum Faction { Unknown = 0x0, Good = 0x1, Evil = 0x2, AntiHero = 0x4, Civilian = 0x8 }
    public enum FloorCollisionType { Top = 0x0, Bottom = 0x1, Left = 0x2, Right = 0x4, None = 0x8 }
    public enum DepthSortingType { Bottom, ZDepth, Top }
    public class Obj
    {
        public List<Combo> Combos;
        public Color AttackColor = new Color(255, 100, 100);
        int updateCount = 0;
        [DefaultValue(false)]
        public bool IsNetworkCapable { get; set; }
        [DefaultValue(false)]
        public bool IsNetworkControlled { get; set; }
        public string NetworkID { get; private set; }
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

        bool allowDecisions = true;
        List<Decision> decisions;
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
        Vector3 networkSpeed = Vector3.Zero;
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
            Combos = new List<Combo>(16);
            Active = true;
            this.Texture = Tex;
            if (Tex != null)
            {
                Origin = new Vector2(Tex.Width / 2, Tex.Height / 2);
            }
            doc = Doc;
            Pos = new Vector3(x, y, z);
            doc.AddObj(this);
            callCreateDecisions();
        }
        public void AddCombo(params Combo[] combos)
        {
            foreach (var c in combos)
            {
                if (!Combos.Contains(c))
                {
                    Combos.Add(c);
                }
            }
        }
        public virtual StageObjectProperty[] GetStageProperties()
        {
            return null;
        }
        public virtual void SetStageProperties(params StageObjectProperty[] sop)
        {

        }
        public virtual void PeerConnected()
        {

        }
        public void SetNetworkId(string id)
        {
            NetworkID = id;
        }
        public void NewNetworkId()
        {
            NetworkID = Guid.NewGuid().ToString().Split('-')[0];
        }
        void callCreateDecisions()
        {
            allowDecisions = true;
            CreateDecisions();
            allowDecisions = false;
        }
        protected virtual void CreateDecisions()
        {

        }
        protected void AddDecision(params Decision[] decision)
        {
            foreach (var d in decision)
            {
                AddDecision(d);
            }
        }
        protected void AddDecision(Decision decision)
        {
            if (!allowDecisions)
                throw new InvalidOperationException("Decisions are no longer allowed to be added");
            if (decisions == null)
                decisions = new List<Decision>();
            decisions.Add(decision);
        }
        public void SendNetworkAttack(Attack a, Rectangle attackBox, Rectangle intersection, Obj o)
        {
            if (string.IsNullOrWhiteSpace(o.NetworkID))
                return;
            int comboIndex = -1;
            int attackIndex = -1;
            for (int ii = 0; ii < o.Combos.Count; ii++)
            {
                if (o.Combos[ii].Attacks.Contains(a))
                {
                    comboIndex = ii;
                    attackIndex = o.Combos[ii].Attacks.IndexOf(a);
                }
            }
            if (comboIndex == -1)
                return;
            SocketMessage sm = new SocketMessage();
            var i = sm.Info;
            sm.Info.BaseAddress = "A";
            i["AI"] = attackIndex.ToString();
            i["CI"] = comboIndex.ToString();
            i["R"] = attackBox.ToStageString();
            i["IN"] = intersection.ToStageString();
            i["ID"] = o.NetworkID;
            sm.NetworkId = this.NetworkID;
            sm.Send();
        }
        public void SendNetworkAttackLanded(Attack a, Rectangle attackBox, Rectangle intersection, Obj o)
        {
            if (string.IsNullOrWhiteSpace(o.NetworkID))
                return;
            int comboIndex = -1;
            int attackIndex = -1;
            for (int ii = 0; ii < Combos.Count; ii++)
            {
                if (Combos[ii].Attacks.Contains(a))
                {
                    comboIndex = ii;
                    attackIndex = Combos[ii].Attacks.IndexOf(a);
                }
            }
            if (comboIndex == -1)
                return;
            SocketMessage sm = new SocketMessage();
            var i = sm.Info;
            sm.Info.BaseAddress = "ALanded";
            i["AI"] = attackIndex.ToString();
            i["CI"] = comboIndex.ToString();
            i["R"] = attackBox.ToStageString();
            i["IN"] = intersection.ToStageString();
            i["ID"] = o.NetworkID;
            sm.NetworkId = this.NetworkID;
            sm.Send();
        }

        public void SendHostOrClientRequest(bool isHost)
        {
            if (string.IsNullOrWhiteSpace(NetworkID))
                return;
            var sm = new SocketMessage();
            sm.NetworkId = NetworkID;
            sm.Info.BaseAddress = isHost ? "Client" : "Host";
            sm.Send();
        }

        bool animatePos = false;
        Vector3 networkPosTarget = Vector3.Zero;
        public virtual void ReceiveSocketMessage(SocketMessage sm)
        {
            var info = sm.Info;
            if (info.BaseAddress == "M")
            {
                animatePos = false;
                if (info.ContainsKey("P"))
                {
                    animatePos = true;
                    networkPosTarget = StageObjectPropertyConverter.GetVector3(info["P"]);
                    if (!Active)
                        Pos = networkPosTarget;
                }
                if (info.ContainsKey("S"))
                {
                    networkSpeed = StageObjectPropertyConverter.GetVector3(info["S"]);
                }
            }
            else if (info.BaseAddress == "A")
            {
                Rectangle intersection = StageObjectPropertyConverter.GetRectangle(info["IN"]);
                Rectangle r = StageObjectPropertyConverter.GetRectangle(info["R"]);

                Obj o = doc.FindObjectByNetworkId(info["ID"]);
                Attack a = o.Combos[int.Parse(info["CI"])].Attacks[int.Parse(info["AI"])];
                AttackedOverride(a, o, r, intersection);
                if (o != null)
                {
                    o.AttackLanded(this, a, r, intersection);
                }
                if (a.HitPause)
                {
                    Pause(a.HitPauseLength, false);
                }
            }
            else if (info.BaseAddress == "ALanded")
            {
                Rectangle intersection = StageObjectPropertyConverter.GetRectangle(info["IN"]);
                Rectangle r = StageObjectPropertyConverter.GetRectangle(info["R"]);

                Obj o = doc.FindObjectByNetworkId(info["ID"]);
                Attack a = Combos[int.Parse(info["CI"])].Attacks[int.Parse(info["AI"])];
                o.AttackedOverride(a, this, r, intersection);
                if (o != null)
                {
                    AttackLanded(o, a, r, intersection);
                }
                if (a.HitPause)
                {
                    Pause(a.HitPauseLength, false);
                }
            }
            else if (info.BaseAddress == "Client")
            {
                IsNetworkControlled = true;
                var mess = new SocketMessage();
                mess.NetworkId = NetworkID;
                mess.Info.BaseAddress = "ClientReceived";
                mess.Send();
            }
            else if (info.BaseAddress == "ClientReceived")
            {
                IsNetworkControlled = false;
            }
            else if (info.BaseAddress == "Host")
            {
                IsNetworkControlled = false;
                var mess = new SocketMessage();
                mess.NetworkId = NetworkID;
                mess.Info.BaseAddress = "HostReceived";
                mess.Send();
            }
            else if (info.BaseAddress == "HostReceived")
            {
                IsNetworkControlled = true;
            }
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
        public Vector3 GetMoveSpeed()
        {
            if (IsNetworkControlled)
                return networkSpeed;
            else return GetMoveSpeedOverride();
        }
        public virtual Vector3 GetMoveSpeedOverride()
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
        public bool ShouldDoNetworkUpdate(int count)
        {
            return IsNetworkCapable && ((updateCount % count) == 0);
        }
        Vector3 lastNetworkPosUpdate = Vector3.Zero, lastNetworkSpeed = Vector3.Zero;
        SocketMessage moveSocket;
        public virtual void EarlyUpdate()
        {
            updateCount++;
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
            if (animatePos)
            {
                Pos += (networkPosTarget - Pos) * 0.2f;
            }
            Rotation += RotationSpeed * PlaySpeed;
            Scale += ScaleSpeed * PlaySpeed;
            Alpha += AlphaSpeed * PlaySpeed;
            if (!IsNetworkControlled)
            {
                bool send = false;
                if (moveSocket == null)
                {
                    moveSocket = new SocketMessage();
                    moveSocket.Info.BaseAddress = "M";
                }
                var sm = moveSocket;
                sm.Info.RemoveValue("S");
                sm.Info.RemoveValue("P");
                sm.NetworkId = this.NetworkID;
                if (ShouldDoNetworkUpdate(10) && lastNetworkPosUpdate != Pos)
                {
                    lastNetworkPosUpdate = Pos;
                    sm.Info["P"] = Pos.ToStageString();
                    
                    send = true;
                }
                if (ShouldDoNetworkUpdate(2) && lastNetworkSpeed != GetMoveSpeed())
                {
                    sm.Info["S"] = GetMoveSpeed().ToStageString();
                    lastNetworkSpeed = GetMoveSpeed();
                    send = true;
                }
                if (send)
                {
                    NetworkManager.Send(sm);
                }
                
            }
            
            //if (Collidable)
            {
                SetBoundingBox();
            }
            if (decisions != null)
            {
                foreach (var d in decisions)
                {
                    d.Update(PlaySpeed);
                }
            }
        }
        void sendNetworkMove()
        {
            var sm = new SocketMessage("Move");
            sm.Info.BaseAddress = "Move";
            sm.Info["Pos"] = Pos.ToStageString();
            sm.NetworkId = this.NetworkID;
            NetworkManager.Send(sm);
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

        public void CreateAttackSparks(Attack attack, Rectangle intersection, Color sparkColor, int sparkDir = 0 )
        {
            if (sparkColor == Color.Transparent)
                sparkColor = AttackColor;
            var diss = new Disappear(doc, doc.LoadTex("Light2"), intersection.Center.X, intersection.Center.Y, Pos.Z) { SortingType = DepthSortingType.Top, SlowDown = false, Color = AttackColor };
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
            //speed.X *= (sparkDir == 0) ? (intersection.Center.X < BoundingBox.Center.X ? 1 : - 1) : sparkDir;
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
                var d1 = new Disappear(doc, Textures.SmallLight, MyMath.RandomRange(intersection.Left, intersection.Right), MyMath.RandomRange(intersection.Top, intersection.Bottom), Pos.Z)
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
            var di = new Disappear(doc, Textures.SmallLight, intersection.Center.X, intersection.Center.Y, Pos.Z)
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
            var di2 = new Disappear(doc, Textures.SmallLight, intersection.Center.X, intersection.Center.Y, Pos.Z)
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
            if (attack.EffectType != AttackEffectType.None)
            {
                for (int i = 0; i < 1; i++)
                {
                    var effect = new Disappear(doc, doc.LoadTex("HitEffects/blunt"), intersection.Center.X, intersection.Center.Y, Pos.Z)
                    {
                        Lifetime = attack.HitPauseLength * 0.25f,
                        Bloom = 0.2f,
                        FadeInSpeed = 1,
                        FadeOutSpeed = -0.5f,
                        SortingType = DepthSortingType.Top,
                        MaxAlpha = 0.7f,
                        Alpha = 1,
                        Color = AttackColor,
                        BlendState = BlendState.Additive
                    };
                    bool flip = intersection.Center.X < Pos.X;
                    switch (attack.EffectType)
                    {
                        case AttackEffectType.Sharp:
                            effect.spriteEffect = SpriteEffects.FlipHorizontally;
                            break;
                    }
                    if (flip)
                    {
                        var dir = attack.Direction % 360f;
                        if (dir > 180)
                        {
                            if (dir < 270)
                                dir = 270 + (270 - dir);
                            else dir = 270 - (dir - 270);
                        }
                        else
                        {
                            if (dir < 90)
                                dir = 90 + (90 - dir);
                            else dir = 90 - (dir - 90);
                        }
                        effect.Rotation = new Vector3(0, 0, dir);
                    }
                    else
                    {
                        effect.Rotation = new Vector3(0, 0, attack.Direction);
                    }
                    var scale = MathHelper.Clamp(attack.Power / 20f, 1f, 3f);
                    effect.AddBehaviour(new Behaviours.HitEffects.HitEffect(scale, scale * 0.25f));
                }
            }
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
        }
        protected void AttackLanded(Obj obj, Attack attack, Rectangle attackBoundingBox, Rectangle attackIntersection)
        {
            if (!IsNetworkControlled)
            {
                AttackLandedOverride(obj, attack, attackBoundingBox, attackIntersection);
            }
            else
            {

            }
        }
        protected virtual void AttackLandedOverride(Obj obj, Attack attack, Rectangle attackBoundingBox, Rectangle attackIntersection)
        {
            
        }
        public bool Attacked(Attack attack, Obj obj, Rectangle attackBox, Rectangle interection)
        {
            if (Attackable)
            {
                if (!IsNetworkControlled)
                {
                    if (attack.HitPause)
                    {
                        float pause = attack.HitPauseLength;
                        if (attack.HitPause)
                        {
                            this.Pause(pause, false);
                        }
                    }
                }
                if (obj.IsNetworkControlled)
                {
                    SendNetworkAttack(attack, attackBox, interection, obj);
                }
                {
                    if (attack.HitPause)
                        Pause(attack.HitPauseLength, false);
                    return AttackedOverride(attack, obj, attackBox, interection);
                }
            }
            else
            {
                return false;
            }
        }

        public virtual bool IsValidAttack()
        {
            return true;
        }
        /// <summary>
        /// This method is called when the object is attacked
        /// </summary>
        /// <param name="attack"></param>
        protected virtual bool AttackedOverride(Attack attack, Obj obj, Rectangle attackBox, Rectangle interection)
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
                var screenRect2 = Global.NetworkScreenRect;
                screenRect2.Inflate(OnScreenExpansion, OnScreenExpansion);
                OnScreen = ScreenRect.Intersects(screenRect) || ScreenRect.Intersects(screenRect2);
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
            /*foreach (var pass in Global.Effects.CurrentTechnique.Passes)
            {
                pass.Apply();
                //doc.DrawSprite(Texture, zPos, Color, Rotation, Origin, Scale * zFactor, spriteEffect, 0);
                doc.DrawSprite(doc.LoadTex("TestWall"), ScreenRect, null, Color * 0.2f);
            }*/
        }
        public virtual void SelectedDraw()
        {
            float zfact = Camera.GetZFactor(Global.Camera.View.Z, Pos.Z, Global.Camera.DepthSize, Global.Camera.DepthPower);
            for (int i = -5; i < 5; i++)
            {
                for (int o = -5; o < 5; o++)
                {
                    //Global.Drawer.DrawRelativeToObj(this, Texture, new Vector3(i, o, 0) / zfact, Color.Black, new Vector3(Origin, 0));
                    //Global.Drawer.Draw(Texture, Techniques.Normal, Pos + new Vector3(i, o, 0) / zfact, Color.Black, Origin.ToVector3(), Rotation, Scale.ToVector3(), BlendState.AlphaBlend, SortingType);
                    Global.Drawer.Draw(Texture, Techniques.Normal, Pos + new Vector3(i, o, 0) / zfact, Color.Black, Origin.ToVector3(), Rotation, Scale.ToVector3(), TextureScale, 0, 0, spriteEffect, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, new BevelData(), ShaderValues, SortingType, PosType);
                }
            }
            /*Global.Effects.Technique = Techniques.Selected;
            //Global.Effects.SetObjValues(this);
            foreach (var pass in Global.Effects.CurrentTechnique.Passes)
            {
                pass.Apply();
                //doc.GraphicsDevice.BlendState = BlendState.Additive;
                doc.DrawSprite(Texture, Vector3.Zero, GetDrawColor() * GetDrawAlpha(), spriteEffect);
                doc.DrawSprite(Texture, Pos.ToVector2(), GetDrawColor() * GetDrawAlpha(), Rotation.Z, Origin, GetDrawScale(), spriteEffect, -Pos.Z);

            }*/
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
