using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using System.Windows;
using SecondShiftMobile.Elements;

namespace SecondShiftMobile.Test
{
    public class Moon : Obj
    {
        public Moon(Game1 Doc, float X, float Y, float Z)
            :base(Doc, Doc.LoadTex("Moon"), X, Y, Z)
        {
            Bloom = 0.2f;
        }
        public override StageObjectProperty[] GetStageProperties()
        {
            return new StageObjectProperty[]
            {
                new StageObjectProperty() { Name = "Scale", Value = Scale, Info = "The scale of the moon" }
            };
        }
        public override void SetStageProperties(params StageObjectProperty[] sop)
        {
            foreach (var s in sop)
            {
                if (s.Name == "Scale")
                {
                    Scale = s.GetVector2();
                }
            }
        }
    }
    public class LightPole : Obj
    {
        LensFlare flare;
        Obj l;
        public LightPole(Game1 Doc, float X, float Y, float Z)
            : base(Doc, Doc.LoadTex("TestLightPole"), X, Y, Z)
        {
            Scale = new Vector2(0.3f);
            flare = new LensFlare(Doc) { CloseDist = 150, FarDist = 300 };
            flare.Target = this;
            flare.Scale = new Vector2(0.5f);
            Origin.Y = 120;
            SlowDown = false;
            IsLightSource = true;
            l = new Obj(Doc, doc.LoadTex("Light2"), X, Y, Z) { Depth = -10, Alpha = 0.9f, BlendState = BlendState.Additive, Color = new Color(255, 255, 150), SlowDown = false };
        }
        public override void ParallelUpdate()
        {
            l.Pos = Pos;
            base.ParallelUpdate();
        }
        public override void Draw()
        {
            base.Draw();
            //doc.GraphicsDevice.BlendState = BlendState.Additive;
           // var dist = (Global.Camera.View.ToVector2() - zPos).Length();
            //l.Scale = Vector2.One * zFactor * (float)Math.Pow((1 - (dist / 640)) + 1, 2);
            //doc.DrawSprite(doc.LoadTex("Light2"), GetScreenPosition(Pos), new Color(255, 255, 150) * 0.9f, 0, new Vector2(50), Vector2.One * zFactor * (float)Math.Pow((1 - (dist / 640)) + 1, 2), SpriteEffects.None, 0);
        }
        public override void Remove()
        {
            base.Remove();
            l.Remove();
            flare.Remove();
        }
        public override bool CheckIfSpriteBatchChangeNeeded(Obj o)
        {
            return base.CheckIfSpriteBatchChangeNeeded(o)
                || (o.BlendState != BlendState.Additive)
                ;
        }

    }
    public class TestFloor : Floor
    {
        public TestFloor(Game1 Doc, float X, float Y, float Z)
            :base(Doc, Doc.LoadTex("TestWall"), X, Y, Z)
        {
            Snap = new Microsoft.Xna.Framework.Vector2(64);
            Attackable = true;
        }
        
    }
    public class InvisibleFloor : Floor
    {
        public InvisibleFloor(Game1 Doc, float X, float Y, float Z)
            : base(Doc, Doc.LoadTex("TestWall"), X, Y, Z)
        {
            Color = Color.Black;
            Origin = Vector2.Zero;
            Alpha = 0.75f;
            SunBlockAlpha = 0;
        }
        public override void Draw()
        {
            if (LevelBuilder.Active)
            {
                base.Draw();
            }
        }
        public override StageObjectProperty[] GetStageProperties()
        {
            return new StageObjectProperty[]
            {
                new StageObjectProperty() { Info = "The size of the floor", Name = "Size", Value = Scale * new Vector2(Texture.Width, Texture.Height) }
            };
        }
        public override void SetStageProperties(params StageObjectProperty[] sop)
        {
            base.SetStageProperties(sop);
            foreach (var s in sop)
            {
                if (s.Name == "Size")
                {
                    Scale = s.GetVector2() / new Vector2(Texture.Width, Texture.Height);
                }
            }
        }
    }
    public class MovingFloor : Floor
    {
        Vector3 startPos;
        Vector3 targetSpeed = new Vector3(4, 0, 0);
        Vector3 ts;
        public MovingFloor(Game1 Doc, float X, float Y, float Z)
            :base(Doc, Doc.LoadTex("TestWall"), X, Y, Z)
        {
            startPos = Pos;
            ts = Vector3.One;
            Snap = new Microsoft.Xna.Framework.Vector2(64);
            DeactivateOffscreen = false;
            SlowDown = true;
        }
        public override StageObjectProperty[] GetStageProperties()
        {
            return new StageObjectProperty[]
            {
                new StageObjectProperty() { Name = "Scale", Value = Scale, Info = "Sets the scale of the floor object"},
                new StageObjectProperty() { Name = "Target Speed", Value = targetSpeed, Info = "Sets the speed of the moving floor when it reaches its boundary"}
            };
        }
        public override void SetStageProperties(params StageObjectProperty[] sop)
        {
            foreach (var s in sop)
            {
                if (s.Name == "Scale")
                {
                    Scale = s.GetVector2();
                }
                else if (s.Name == "Target Speed")
                {
                    targetSpeed = s.GetVector3();
                }
            }
            base.SetStageProperties(sop);
        }
        public override void EarlyUpdate()
        {
            if (Pos.X < startPos.X - 200)
                ts.X = 1;
            else if (Pos.X > startPos.X + 200)
                ts.X = -1;
            Speed += ((targetSpeed * ts) - Speed) * 0.05f * PlaySpeed;
            base.EarlyUpdate();
        }
        public override void Draw()
        {
            base.Draw();
            if (LevelBuilder.Active)
            {
                Global.Effects.Technique = Techniques.NormalDepth;
                foreach (var pass in Global.Effects.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    doc.DrawSprite(Texture.Texture, startPos, Color * Alpha * ScreenAlpha * 0.5f, Rotation.Z, Origin, Scale, spriteEffect);
                }
            }
        }
    }
    public class LargeGrass : Floor
    {
        public LargeGrass(Game1 Doc, float X, float Y, float Z)
            : base(Doc, Doc.LoadTex("TestGrass"), X, Y, Z)
        {
            Snap.X = 1024;
            BoundingRectangle.Y = 135;
            Depth = -5;
        }
    }
    public class Mountain : Obj
    {
        public Mountain(Game1 Doc, float X, float Y, float Z)
            : base(Doc, Doc.LoadTex("Mountain2"), X, Y, Z)
        {
            Scale = new Vector2(20);
        }
        public Mountain(Game1 Doc, TextureFrame Tex, float X, float Y, float Z)
            : base(Doc, Tex, X, Y, Z)
        {
            Scale = new Vector2(20);
        }

        public override StageObjectProperty[] GetStageProperties()
        {
            return new StageObjectProperty[]
            {
                new StageObjectProperty() { Name = "Scale", Info = "A single float representing scale", Value = Scale.X }
            };
        }
        public override void SetStageProperties(params StageObjectProperty[] sop)
        {
            foreach (var s in sop)
            {
                if (s.Name == "Scale")
                {
                    Scale = new Vector2(s.GetFloat());
                }
            }
        }
    }
    public class TestPropellors : Obj
    {
        ClockHand[] props = new ClockHand[80];
        float iAdd = 200;
        public TestPropellors(Game1 Doc, float X, float Y, float Z)
            :base(Doc, Doc.LoadTex("Light"), X, Y, Z)
        {
            //Alpha = 0;
            for (int i = 0; i < props.Length; i++)
            {
                props[i] = new ClockHand(Doc, X, Y, Z + (i * iAdd));
                props[i].Rotation.Z = i * 20;
            }
            
        }
        public override void Update()
        {
            for (int i = 0; i < props.Length; i++)
            {
                props[i].Pos = Pos + new Vector3(0, 0, Pos.Z + (i * iAdd));
            }
        }
        public override void Draw()
        {
            for (int i = 0; i < props.Length; i++)
            {
                props[i].Pos = Pos + new Vector3(0, 0, Pos.Z + (i * iAdd));
            }
            if (LevelBuilder.Active)
            {
                base.Draw();
            }
        }
        public override void Remove()
        {
            foreach (var p in props)
            {
                p.Remove();
            }
            base.Remove();
        }
        public override void BloomDraw(float alpha)
        {
            if (LevelBuilder.Active)
            {
                base.BloomDraw(alpha);
            }
        }
    }
    public class ClockHand : Obj
    {
        Sun sun;
        public ClockHand(Game1 Doc, float X, float Y, float Z)
            : base(Doc, Doc.LoadTex("ClockHand"), X, Y, Z)
        {
            Scale = new Vector2(5);
            RotationSpeed.Z = 1;
            Attackable = true;
            Bevel = true;
            BevelGlow = 2f;
            bevelColor = new Color(255, 180, 100);
            Origin = new Vector2(128, 430);
        }
        protected override void SetBevelData()
        {
            if (sun == null)
            {
                sun = doc.FindObject<Sun>();
            }
            if (sun != null)
            {
                isBeveling = true;
                float ang = MathHelper.ToDegrees(MyMath.Direction(GetScreenPosition(Pos), sun.GetScreenPosition(sun.Pos) + new Vector2(0, -200)));
                //Global.Output = ang;

                bevelDelta = new Vector2(0.05f, 0).Rotate(ang);
                bevelDelta = bevelDelta;
                bevelIntensity = 2;
                //bevelDelta = new Vector2(0.1f);
            }
        }
        public override StageObjectProperty[] GetStageProperties()
        {
            return new StageObjectProperty[]
            {
                new StageObjectProperty() { Name = "Scale", Info = "A single float representing scale", Value = Scale.X },
                new StageObjectProperty() { Name = "Rotation Speed", Info = "The speed of rotation", Value = RotationSpeed.Z }
            };
        }
        public override void SetStageProperties(params StageObjectProperty[] sop)
        {
            foreach (var s in sop)
            {
                if (s.Name == "Scale")
                {
                    Scale = new Vector2(s.GetFloat());
                }
                else if (s.Name == "Rotation Speed")
                {
                    RotationSpeed.Z = s.GetFloat();
                }
            }
        }
    }
    public class TestPropellor : Obj
    {
        Sun sun;
        public TestPropellor(Game1 Doc, float X, float Y, float Z)
            : base(Doc, Doc.LoadTex("TestPropellor"), X, Y, Z)
        {
            Scale = new Vector2(5);
            RotationSpeed.Z = 1;
            Attackable = true;
            Bevel = true;
            BevelGlow = 1.5f;
        }
        /*protected override void SetBevelData()
        {
            if (sun == null)
            {
                sun = doc.FindObject<Sun>();
            }
            if (sun != null)
            {
                isBeveling = true;
                float ang = ((sun.GetScreenPosition(sun.Pos) - GetScreenPosition(Pos)) + new Vector2(0, -200)).Angle();

                bevelDelta = new Vector2(0.1f, 0).Rotate(ang);
                bevelDelta = bevelDelta;
                bevelIntensity = 1;
                //bevelDelta = new Vector2(0.1f);
            }
        }*/
        public override StageObjectProperty[] GetStageProperties()
        {
            return new StageObjectProperty[]
            {
                new StageObjectProperty() { Name = "Scale", Info = "A single float representing scale", Value = Scale.X },
                new StageObjectProperty() { Name = "Rotation Speed", Info = "The speed of rotation", Value = RotationSpeed.Z }
            };
        }
        public override void SetStageProperties(params StageObjectProperty[] sop)
        {
            foreach (var s in sop)
            {
                if (s.Name == "Scale")
                {
                    Scale = new Vector2(s.GetFloat());
                }
                else if (s.Name == "Rotation Speed")
                {
                    RotationSpeed.Z = s.GetFloat();
                }
            }
        }
    }
    public class TestVortex : Obj
    {
        public TestVortex(Game1 Doc, float X, float Y, float Z)
            : base(Doc, Doc.LoadTex("Light"), X, Y, Z)
        {
            for (int i = 0; i < 50; i++)
            {
                float d = MyMath.RandomRange(20, 300);
                new Vortex(doc, X, Y, Z) { Distance = d, TestVortex = this, Color = GameExtensions.GetColorFromHue(MyMath.Between(0, 3, MyMath.BetweenValue(20, 300, d))), Scale = new Vector2(0.3f) };
                
            }
        }
        private class Vortex : Obj
        {
            public float Distance = 100;
            public TestVortex TestVortex;

            float dir = 0;
            public Vortex(Game1 Doc, float X, float Y, float Z)
                : base(Doc, Doc.LoadTex("Light"), X, Y, Z)
            {
                UpscaledSecondShiftDraw = true;
                Depth = -6;
                Attackable = true;
                Bevel = true;
            }
            public override void EarlyUpdate()
            {
                base.EarlyUpdate();
                if (TestVortex != null)
                {
                    float dirAdd = 500 / (Pos - TestVortex.Pos).Length();
                    if (float.IsInfinity(dirAdd))
                        dirAdd = 0;
                    dir += dirAdd * PlaySpeed;
                    Pos = TestVortex.Pos + new Vector3(MyMath.LengthDirX(Distance, dir), -MyMath.LengthDirY(Distance, dir), 0);
                    Rotation.Z = dir + 90;
                    //Rotation = dir;
                    if (TestVortex.RemoveCalled)
                        Remove();
                }
            }
            protected override bool AttackedOverride(Attack attack, Obj obj, Rectangle AttackBox, Rectangle intersection)
            {
                Remove();
                return base.AttackedOverride(attack, obj, AttackBox, intersection);
            }
            public override void Draw()
            {
                Scale.X = MathHelper.Clamp((Pos - PreviousPos).Length() / 10, Scale.Y, 1000);      
                //Rotation = MathHelper.ToDegrees(MyMath.Direction(PreviousPos.ToVector2(), Pos.ToVector2()));
                base.Draw();
            }
        }
    }
    public enum VolumeType { MediaPlayer, SoundEffect }
    public class VolumeChanger : Obj
    {
        float timer = 0;
        int maxTimer = 0;
        float volume = 0;
        float origVolume = 0;
        VolumeType type;
        public VolumeChanger(Game1 Doc, float vol, int frames, VolumeType type)
            :base(Doc, Textures.SmallLight, 0, 0, 0)
        {
            if (frames <= 1)
            {
                if (type == VolumeType.MediaPlayer)
                    SoundManager.MediaPlayerVolume = vol;
                else if (type == VolumeType.SoundEffect)
                    SoundManager.SoundEffectVolume = vol;
                Remove();
            }
            else
            {
                this.type = type;
                if (type == VolumeType.MediaPlayer)
                {
                    origVolume = SoundManager.MediaPlayerVolume;
                }
                else
                {
                    origVolume = SoundManager.SoundEffectVolume;
                }
                maxTimer = frames;
                volume = vol;
                Alpha = 0;
            }
        }

        

        public override void Update()
        {
#if DEBUG
            timer += 1;
#else
            timer += Global.FrameSpeed;
#endif
            if (maxTimer > 0)
            {
                float val = MyMath.Between(origVolume, volume, timer / maxTimer);

                if (type == VolumeType.MediaPlayer)
                {
                    SoundManager.MediaPlayerVolume = MathHelper.Clamp(val, 0, 1);
                }
                else SoundManager.SoundEffectVolume = MathHelper.Clamp(val, 0, 1);
                if (timer >= maxTimer)
                {
                    Active = false;
                    Remove();
                }
            }
            else
            {
                if (type == VolumeType.MediaPlayer)
                {
                    SoundManager.MediaPlayerVolume = MathHelper.Clamp(volume, 0, 1);
                }
                else SoundManager.SoundEffectVolume = MathHelper.Clamp(volume, 0, 1);
                Active = false;
                Remove();
            }
            base.Update();
        }
        public override void Remove()
        {
            base.Remove();
        }
        public override void ParallelUpdate()
        {
            
            base.ParallelUpdate();
        }
    }
    public class LogoBloom : LightBloom
    {
        public LogoBloom(Game1 Doc, float X, float Y, float Z)
            : base(Doc, X, Y, Z, 100, "Logo")
        {
            AverageAlpha = 62.8f;
            OverlayAlpha = 0;
            BloomIntensity = 1.0f;
            BloomWidth = 20;
            ZoomIntensity = 0.3f;
            ZoomScale = 6;
            BlackCircleAlpha = 0;
            BlockTargetAlpha = 0.5f;
            Scale = new Vector2(2f);
            //BlendState = BlendState.AlphaBlend;
            
            //FullZoom = false;
            Rays3D = false;
        }
    }
    public class CameraDeltaChanger : Obj
    {
        Vector3 target;
        Vector3 start;
        float frames;
        float frame = 0;
        float val;
        float startPower, endPower;
        public CameraDeltaChanger(Vector3 Delta, float Frames, float StartPower, float EndPower)
            :base(Global.Game, Textures.SmallLight, 0, 0, 0)
        {
            start = Global.Camera.DeltaPos;
            target = Delta;
            frames = Frames;
            startPower = StartPower;
            endPower = EndPower;
        }
        public override void ParallelUpdate()
        {
            frame += Global.FrameSpeed;
            val = MyMath.BetweenValue(0, frames, frame);
            val = (float)Math.Pow(val, endPower);
            val = 1 - (float)Math.Pow(1 -val, startPower);
            Global.Camera.DeltaPos = MyMath.Between(start, target, val);
            if (frame >= frames)
            {
                Global.Camera.DeltaPos = target;
                Remove();
            }
            base.ParallelUpdate();
        }
        public override bool CheckIfSpriteBatchChangeNeeded(Obj o)
        {
            return false;
        }
        public override void Draw()
        {
            //base.Draw();
        }
    }
    public enum FadeType { FadeIn, FadeOut }
    public class ScreenFadeChanger : Obj
    {
        float frames, start, target, timer = 0;
        SecondShiftMobile.Cutscenes.Cutscene c;
        public ScreenFadeChanger(SecondShiftMobile.Cutscenes.Cutscene Cutscene, FadeType Fade, float Frames)
            : base(Global.Game, Textures.SmallLight, 0, 0, 0)
        {
            c = Cutscene;
            frames = Frames;
            if (Fade == FadeType.FadeIn)
            {
                c.BlockAlpha = start = 1;
                target = 0;
            }
            else
            {
                c.BlockAlpha = start = 0;
                target = 1;
            }
            if (Frames < 1)
            {
                c.BlockAlpha = target;
                Remove();
                Active = false;
            }
        }
        public override void Update()
        {
            timer += Global.FrameSpeed;
            //Global.Output = "update: " + timer;
            if (timer < frames)
            {
                c.BlockAlpha = MyMath.Between(start, target, timer / frames);
            }
            else
            {
                c.BlockAlpha = target;
                Remove();
            }
            base.Update();
        }
        public override void Draw()
        {
            base.Draw();
        }
    }
    public class Thing : Obj
    {
        public Thing(Game1 Doc, float X, float Y, float Z)
            : base(Doc, Doc.LoadTex("thing"), X, Y, Z)
        {
            Scale = new Vector2(5);
            Bevel = true;
            BevelGlow = 3;
            BevelDeltaMultiplier = new Vector2(2);
        }
        public override void LateUpdate()
        {
            base.LateUpdate();
        }
        public override void ParallelUpdate()
        {
            var winds = doc.FindObjects<Wind>();
            foreach (var wind in winds)
            {
                if (IsCollidingWith(wind))
                {
                    Speed += (Vector3)wind.GetElementValue(this) * 0.1f;
                }
            }
            Speed += -Speed * 0.1f;
            base.ParallelUpdate();
        }
    }
    public class LaunchRocket : Obj
    {
        bool launching = false;
        float accel = 0;
        SmokeEmitter emit;
        LightBloom light;
        Obj circ;
        float alpha = 0;
        SoundEffectInstance launchSound, movingSound;
        public LaunchRocket(Game1 Doc, float X, float Y, float Z)
            : base(Doc, Doc.LoadTex("Rytard Missle"), X, Y, Z)
        {
            Texture.Height = Texture.Width = 100;
            Origin = new Vector2(13, 51);
            Scale = new Vector2(40);
            Rotation.Z = -90;
            IsLightSource = true;
            LightFallOff = 5000;
            LightFallOffRange = 3000;
            LightIntensity = 3;
            emit = new SmokeEmitter(Doc, X, Y, Z)
            {
                Target = this,
                TargetSpeedMultiplier = 0.1f,
                StartScale = 10,
                MaxTimer = 18,
                Lifetime = 450,
                Emit = false
            };
            /*light = new LightBloom(Doc, X, Y, Z, 100)
            {
                Scale = new Vector2(15),
                ZoomIntensity = 0.1f,
                Color = new Color(255, 200, 100),
                ZoomScale = 5,
                BlackCircleAlpha = 0,
                OverlayAlpha = 1,
                FlareAlpha = 3
            };*/
            circ = new Obj(Doc, Doc.LoadTex("Light"), X, Y, Z)
            {
                Depth = -10,
                Scale = new Vector2(20),
                Alpha = 1,
                Color = new Color(255, 200, 150),
                Bloom = 1
            };
            emit.Emitted += emit_Emitted;
#if DEBUG
            Helper.Write("Loading files for rocket");
#endif
            launchSound = doc.LoadSoundEffect("charge").CreateInstance();
            movingSound = doc.LoadSoundEffect("beamfire").CreateInstance();
#if DEBUG
            Helper.Write("Rocket files loaded");
#endif
            movingSound.IsLooped = true;
            movingSound.Volume = 0;

        }

        void emit_Emitted(object sender, SmokeEmittedEventArgs e)
        {
            if (Speed.Y > -30)
            {
                var d = new Behaviours.SpeedDissipation(10, 0.9f);
                var d2 = new Behaviours.SpeedDissipation(10, 0.9f);
                e.Smoke.AddBehaviour(d); e.Fire.AddBehaviour(d2);
            }
            //e.Smoke.DrawNormal = true;
        }
        public override void Remove()
        {
            circ.Remove();
            //light.Remove();
            emit.Remove();
            launchSound.Stop();
            movingSound.Stop();
            base.Remove();
        }
        public override void ParallelUpdate()
        {
            //light.Pos = Pos;
            circ.Pos = Pos;
            //light.Alpha = alpha; 
            circ.Alpha = alpha * 0.5f;
            movingSound.Pitch = GetSoundPitchDoppler();
            if (launching)
            {
                alpha = MathHelper.Clamp(alpha + (0.01f * PlaySpeed), 0, 1);
                if (Speed.Y > -30)
                {
                    accel += 0.0001f * PlaySpeed;
                    Speed.Y -= accel * PlaySpeed;
                }
                if (Speed.Y > -5)
                {
                    emit.SmokeSpeedRange = new Vector3(40, 0, 40);
                }
                else
                {
                    emit.SmokeSpeedRange = new Vector3(5, 0, 0);
                }
                
            }
            base.ParallelUpdate();
        }
        public override void NonCriticalUpdate()
        {
            launchSound.Volume = this.GetSoundVolume(10, 30000);
            movingSound.Volume = this.GetSoundVolume(Speed.Length() * 0.1f, 15000);
            launchSound.Pan = movingSound.Pan = this.GetSoundPan();
            launchSound.Pitch = movingSound.Pitch = this.GetSoundPitch();
            base.NonCriticalUpdate();
        }
        public void Launch()
        {
            emit.Emit = true;
            launching = true;
            new Disappear(doc, doc.LoadTex("Light"), Pos.X, Pos.Y, Pos.Z)
            {
                ScaleSpeed = new Vector2(0.5f),
                Scale = new Vector2(150),
                Lifetime = 60,
                MaxAlpha = 0.3f,
                FadeInSpeed = 0.1f,
                FadeOutSpeed = -0.01f,
                Bloom = 1,
                Color = new Color(255, 150, 100),
                Distortion = 0f
            };
            try
            {
                launchSound.Play();
            }
            catch
            {

            }
            try
            {
                movingSound.Play();
            }
            catch
            {

            }
            //Global.Camera.AddShake(10, 0.3f, 0.01f);
        }
    }
   
}
