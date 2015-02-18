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
    public class Explosion : LightBloom
    {
        SoundEffectInstance sound;
        float smokeTimer = 0, timer = 0, smokeStep = 5, maxSmokeTime = 0, lifetime = 0, smokeLifetime = 0;
        float explosionMapPos = 0;
        float sparkSpeed, sparkNum;
        int sparks = 0;
        float sparkTimer = 0 ;
        const float maxSparkTimer = 1;
        public Explosion(Game1 Doc, float X, float Y, float Z, float scale, float scalespeed, float lifetime, float smokeStep, float smokeLifetime, float howLongToSmoke)
            : base(Doc, X, Y, Z, 150, "Light")
        {
            this.smokeLifetime = smokeLifetime;
            this.lifetime = lifetime;
            Scale = new Microsoft.Xna.Framework.Vector2(scale);
            ScaleSpeed = new Vector2(scalespeed);
            maxSmokeTime = howLongToSmoke;
            this.smokeStep = smokeStep;
            Color = new Color(255, 120, 50);
            var diss = new Disappear(doc, doc.LoadTex("Flares/Flare2"), X, Y, Z) { 
                Scale = Scale, ScaleSpeed = Scale / 150f, 
                Lifetime = lifetime / 2, MaxAlpha = 0.7f,
                Bloom = 0.5f,
                Color = Color
            };
            diss = new Disappear(doc, doc.LoadTex("Flares/Flare2"), X, Y, Z) 
            { 
                Scale = Scale, ScaleSpeed = new Vector2(3.5f), Color = Color, BlendState = BlendState.Additive, MaxAlpha = 0.3f,
                Bloom = 5, Distortion = 5
            };
            diss = new Disappear(doc, doc.LoadTex("Flares/Flare"), X, Y, Z)
            {
                Scale = Scale * 5,
                ScaleSpeed = new Vector2(0),
                Color = Color,
                //BlendState = BlendState.Additive,
                MaxAlpha = 1f,
                Bloom = 1,
                Distortion = 0,
                Lifetime = 10,
                FadeInSpeed = 0.1f,
                FadeOutSpeed = -0.05f
            };
            Elements.Winds.SphereWind wind = new Elements.Winds.SphereWind(Doc, X, Y, Z)
            {
                Power = scale * 2,
                Size = scale * 600,
                PowerSpeed = ((scale * 2) / (lifetime / 2)) * -1.5f
            };
            wind.AddBehaviour(new SecondShiftMobile.Behaviours.DisappearBehaviour(lifetime / 2, 1, 1, 1));
            Global.Camera.AddShake(MathHelper.Clamp(640 * scale * 7 / (Vector3.Distance(Pos, Global.Camera.View) + 640), 0, 50), 0.5f, 0.01f);
            Alpha = 0;
            BloomWidth = 10;
            BloomIntensity = 1;
            BlackCircleAlpha = 0;
            OverlayAlpha = 0;
            BloomIntensity = 1;
            ZoomIntensity = 0.2f;
            ZoomScale = 1f;
            sound = doc.LoadSoundEffectInstance("Explosion");
            sparkSpeed = scale * 7f;
            sparkNum = scale * 5;
            /*for (int i = 0; i < sparkNum; i++)
            {
                var es = new ExplostionSpark(doc, X, Y, Z) { Color = Color, BlendState = BlendState.Additive };
                es.Speed = new Vector3(MyMath.RandomRange(-sparkSpeed, sparkSpeed), MyMath.RandomRange(-sparkSpeed, sparkSpeed), MyMath.RandomRange(-sparkSpeed, sparkSpeed));
                es.Scale = Scale / 60;
                es.AddBehaviour(new SecondShiftMobile.Behaviours.DirectionToRotation(5) { UseCamera = false });
            }*/
            sound.Volume = GetSoundVolume(Scale.X, 30000);
            sound.Play();
        }
        public override void EarlyUpdate()
        {
            explosionMapPos += PlaySpeed * 0.25f;
            sparkTimer += PlaySpeed;
            while (sparkTimer > maxSparkTimer && sparks < sparkNum)
            {
                sparks++;
                sparkTimer -= maxSparkTimer;
                var es = new ExplostionSpark(doc, Pos.X, Pos.Y, Pos.Z) { Color = Color, BlendState = BlendState.Additive };
                es.Speed = new Vector3(MyMath.RandomRange(-sparkSpeed, sparkSpeed), MyMath.RandomRange(-sparkSpeed, sparkSpeed), MyMath.RandomRange(-sparkSpeed, sparkSpeed));
                es.Scale = Scale / 60;
                es.AddBehaviour(new SecondShiftMobile.Behaviours.DirectionToRotation(20) { UseCamera = false });
            }
            sound.Pan = GetSoundPan();
            sound.Pitch = GetSoundPitchDoppler();
            sound.Volume = GetSoundVolume(Scale.X, 30000);
            if (timer < lifetime)
            {
                if (Alpha < 1)
                {
                    Alpha += 0.1f * PlaySpeed;
                }
            }
            else
            {
                if (Alpha > 0)
                {
                    Alpha -= 0.01f * PlaySpeed;
                }
                else
                {
                    Remove();
                }
            }
            smokeTimer += PlaySpeed;
            timer += PlaySpeed;
            if (timer <= maxSmokeTime)
            {
                while (smokeTimer >= smokeStep)
                {
                    smokeTimer -= smokeStep;
                    var sm = new ExplosionSmoke(doc, this, Pos.X, Pos.Y, Pos.Z) { Lifetime = smokeLifetime * MyMath.RandomRange(0.75f, 1.25f) };
                    sm.SmokeSpeed = MyMath.RandomRange(-Vector3.One, Vector3.One) * 2;
                    sm.Rotation.Z = MyMath.RandomRange(0, 360);
                    sm.SunBlockAlpha = 0.5f;
                    sm.Scale = Scale * 0.3f;
                }
            }
            base.EarlyUpdate();
        }
        
    }
    public class ExplosionSmoke : Smoke
    {
        Explosion explo;
        float exploSize;
        public Vector3 SmokeSpeed = Vector3.Zero;
        float speedscale = 1;
        float speedtarget = 1;
        public ExplosionSmoke(Game1 Doc, Explosion explo, float X, float Y, float Z)
            : base(Doc, X, Y, Z)
        {
            this.explo = explo;
            exploSize = explo.Scale.X * 20;
            Color = new Color(52, 50, 40);
            SunBlockAlpha = 1f;
            AffectedByWind = false;
            MaxAlpha = 1;
        }
        public override void Update()
        {
            Speed = SmokeSpeed * explo.Scale.X * speedscale;

            if (Vector3.Distance(Pos, explo.Pos) > exploSize)
            {
                speedtarget = 0.0075f;
            }
            else
            {
                speedtarget = 1;
            }
            speedscale += (speedtarget - speedscale) * 0.2f * Global.Speed;
            base.Update();
        }
    }
    public class ExplostionSpark : Disappear
    {
        public ExplostionSpark(Game1 Doc, float X, float Y, float Z)
            :base(Doc, Doc.LoadTex("Light"), X, Y, Z)
        {
            this.MaxAlpha = 1;
            this.Lifetime = 15;
            this.FadeInSpeed = 0.1f;
            AddBehaviour(new Behaviours.DirectionToRotation(2) { UseCamera = false });
            
        }
    }
    public class ExplosionFire : FireSmoke
    {
        public ExplosionFire(Game1 Doc, Explosion Explo)
            : base(Doc, Explo.Pos.X, Explo.Pos.Y, Explo.Pos.Z)
        {
            Lifetime = 100;
            Pos.Y = Explo.Pos.Y + (Explo.Texture.Height * Explo.Scale.Y);
        }
    }
}
