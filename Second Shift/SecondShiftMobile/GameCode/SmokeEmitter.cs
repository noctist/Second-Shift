using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecondShiftMobile
{
    public class SmokeEmittedEventArgs : EventArgs
    {
        public Smoke Fire;
        public Smoke Smoke;
    }
    public class SmokeEmitter : Obj
    {
        float timer = 0;
        public float MaxTimer = 6;
        public bool SetSpeedToTargetSpeed = true;
        public float TargetSpeedMultiplier = 0.1f;
        public float TargetSpeedRangeMultiplier = 0.2f;
        public Vector3 SmokeSpeed = new Vector3(0, -4, 0);
        public Vector3 SmokeSpeedRange = new Vector3(2, 1, 0);
        public float StartScale = 1;
        public float ScaleSpeed = 0.02f;
        public float ScaleSpeedRange = 0.005f;
        public float Lifetime = 180;
        public Obj Target;
        public Vector3 EmitterSize = new Vector3(20);
        public event EventHandler<SmokeEmittedEventArgs> Emitted;
        public bool Emit = true;
        public SmokeEmitter(Game1 Doc, float X, float Y, float Z)
            : base(Doc, Doc.Content.Load<Texture2D>("Light"), X, Y, Z)
        {
            SunBlockAlpha = 0.5f;
            Color = Color.Black;
            OnScreenExpansion = 400;
            DeactivateOffscreen = true;
        }
        public override StageObjectProperty[] GetStageProperties()
        {
            //return base.GetStageProperties();
            return new StageObjectProperty[] 
            {
                new StageObjectProperty() { Name = "Emitter Size", Value = EmitterSize, Info = "The size of the emitter"},
                new StageObjectProperty() { Name = "Max Timer", Value = MaxTimer, Info = "How many frames before a new smoke is created"},
                new StageObjectProperty() { Name = "Sun Block Alpha", Value = SunBlockAlpha},
                new StageObjectProperty() { Name = "Smoke Lifetime", Value = Lifetime },
                new StageObjectProperty() { Name = "Start Scale", Value = StartScale }
            };
        }
        public override void SetStageProperties(params StageObjectProperty[] sop)
        {
            foreach (var s in sop)
            {
                if (s.Name == "Emitter Size")
                    EmitterSize = s.GetVector3();
                else if (s.Name == "Max Timer")
                    MaxTimer = s.GetFloat();
                else if (s.Name == "Sun Block Alpha")
                    SunBlockAlpha = s.GetFloat();
                else if (s.Name == "Smoke Lifetime")
                    Lifetime = s.GetFloat();
                else if (s.Name == "Start Scale")
                    StartScale = s.GetFloat();
            }
        }
        public override void EarlyUpdate()
        {
            if (Emit)
            {
                timer += PlaySpeed;
                Vector3 addSpeed = Vector3.Zero;
                if (Target != null)
                {
                    Pos = Target.Pos;
                    if (SetSpeedToTargetSpeed)
                    {
                        addSpeed = Target.Speed * TargetSpeedMultiplier;
                        //SmokeSpeedRange = SmokeSpeed * TargetSpeedRangeMultiplier;
                    }
                }
                while (timer > MaxTimer)
                {
                    timer -= MaxTimer;
                    Scale.X = MathHelper.Clamp(EmitterSize.X / 100f, 1, 100);
                    Scale.Y = MathHelper.Clamp(EmitterSize.Y / 100f, 1, 100);
                    Smoke sm = new Smoke(doc, Pos.X + (MyMath.RandomRange(-EmitterSize.X, EmitterSize.X) / 2), Pos.Y + (MyMath.RandomRange(-EmitterSize.Y, EmitterSize.Y) / 2), Pos.Z + (MyMath.RandomRange(-EmitterSize.Z, EmitterSize.Z) / 2)) { FadeInSpeed = 0.075f, Lifetime = Lifetime * 0.3f };
                    //sm.StartColor = new Color(170, 100, 40, 255) * MyMath.RandomRange(0.65f, 1.25f);
                    //sm.StartColor.A = 255;
                    //sm.ChangeRange = 0.4f;
                    sm.Color = new Color(170, 100, 40, 255);
                    sm.Color.A = 255;
                    sm.Depth = 2;
                    //sm.IsLightSource = true;
                    sm.LightColor = sm.Color;
                    sm.LightIntensity = 2;
                    //sm.Color = new Color(70, 70, 70, 255);
                    sm.Speed = MyMath.RandomRange(SmokeSpeed - SmokeSpeedRange, SmokeSpeed + SmokeSpeedRange) + addSpeed;
                    //sm.Speed.Z = 0;
                    sm.Rotation.Z = MyMath.RandomRange(0, 360);
                    sm.RotationSpeed.Z = (float)Math.Sqrt(-sm.Speed.X * 0.1f);
                    sm.ScaleSpeed = new Microsoft.Xna.Framework.Vector2(MyMath.RandomRange(ScaleSpeed - ScaleSpeedRange, ScaleSpeed + ScaleSpeedRange));
                    sm.SunBlockAlpha = SunBlockAlpha;
                    sm.Bloom = 0.5f;
                    sm.Scale = new Vector2(StartScale * MyMath.RandomRange(0.75f, 1.25f));

                    Scale.X = MathHelper.Clamp(EmitterSize.X / 100f, 1, 100) * 1.5f;
                    Scale.Y = MathHelper.Clamp(EmitterSize.Y / 100f, 1, 100) * 1.5f;
                    Smoke sm1 = new Smoke(doc, Pos.X + (MyMath.RandomRange(-EmitterSize.X, EmitterSize.X) / 2), Pos.Y + (MyMath.RandomRange(-EmitterSize.Y, EmitterSize.Y) / 2), Pos.Z + (MyMath.RandomRange(-EmitterSize.Z, EmitterSize.Z) / 2)) { FadeInSpeed = 0.0075f, Lifetime = Lifetime * 0.7f, LifetimeWait = (Lifetime * 0.3f) };
                    
                    //sm.StartColor = new Color(170, 100, 40, 255) * MyMath.RandomRange(0.65f, 1.25f);
                    //sm.StartColor.A = 255;
                    //sm.ChangeRange = 0.4f;
                    //sm1.DrawNormal = true;

                    sm1.Depth = 1;
                    sm1.Color = new Color(35, 30, 30);
                    //sm.Color = new Color(70, 70, 70, 255);
                    sm1.Speed = MyMath.RandomRange(SmokeSpeed - SmokeSpeedRange, SmokeSpeed + SmokeSpeedRange) + addSpeed;
                    sm1.RotationSpeed.Z = (float)Math.Sqrt(-sm.Speed.X * 0.1f);
                    sm1.ScaleSpeed = new Microsoft.Xna.Framework.Vector2(MyMath.RandomRange(ScaleSpeed - ScaleSpeedRange, ScaleSpeed + ScaleSpeedRange));
                    sm1.SunBlockAlpha = SunBlockAlpha;
                    sm1.Scale = new Vector2(StartScale * MyMath.RandomRange(0.75f, 1.25f));
                    if (Emitted != null)
                    {
                        Emitted(this, new SmokeEmittedEventArgs() { Fire = sm, Smoke = sm1 });
                    }
                    //sm1.Bloom = 0.5f;
                    //sm.Distortion = 0.05f;

                    /*sm = new Smoke(doc, Pos.X + MyMath.RandomRange(-20, 20), Pos.Y, Pos.Z) { FadeInSpeed = 0.05f, TimerToFadeOut = Lifetime / 2 }; ;
                    sm.Depth = 0;
                    sm.Distortion = MyMath.RandomRange(0.05f, 0.15f);
                    sm.Color = new Color(170, 100, 40, 255) * MyMath.RandomRange(0.65f, 1.25f);
                    sm.Color.A = 255;
                    sm.Bloom = 0.4f;
                    sm.Speed = MyMath.RandomRange(SmokeSpeed - SmokeSpeedRange, SmokeSpeed + SmokeSpeedRange);
                    sm.RotationSpeed = sm.Speed.X * 0.1f;
                    sm.Rotation = MyMath.RandomRange(0, 360);
                    //sm.Scale = new Vector2(1.3f);
                    sm.ScaleSpeed = new Microsoft.Xna.Framework.Vector2(MyMath.RandomRange(ScaleSpeed - ScaleSpeedRange, ScaleSpeed + ScaleSpeedRange));*/
                }
            }
            base.EarlyUpdate();
        }
        public override void Draw()
        {
            if (LevelBuilder.Active)
            {
                Alpha = 1;
                Color = Color.Black;
                Scale.X = MathHelper.Clamp(EmitterSize.X / 100f, 1, 100) * 1.5f;
                Scale.Y = MathHelper.Clamp(EmitterSize.Y / 100f, 1, 100) * 1.5f;
                BlendState = BlendState.AlphaBlend;
            }
            else
            {
                Alpha = 0;
                Scale = new Vector2(Math.Max(EmitterSize.X, EmitterSize.Y) / 30f);
                Color = new Color(255, 200, 100);
                BlendState = BlendState.Additive;
            }
            base.Draw();
        }
    }
    public class FireSmoke : Smoke
    {
        public Color StartColor = new Color(170, 100, 40, 255);
        public Color EndColor = new Color(50, 40, 30, 255);
        public float ChangeRange = 0.5f;
        float life = 0;
        public FireSmoke(Game1 Doc, float X, float Y, float Z)
            :base(Doc, X, Y, Z)
        {
            Color = StartColor;
        }
        public override void Update()
        {
            life = MathHelper.Clamp(0.5f + ((LifetimePercent - 0.5f) / ChangeRange), 0, 1);
            Color = MyMath.Between(StartColor, EndColor, life);
            //Bloom = (1 - life) * 0.4f;
            base.Update();
        }
        protected override void SetBloom()
        {
            Global.Effects.ObjBloom = (1 - life) * Bloom;
        }
        public override float GetDistortion()
        {
            return (1 - life) * Distortion;
        }
    }
}
