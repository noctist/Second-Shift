using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;

namespace SecondShiftMobile.Environments.City
{
    public class Building : Obj
    {
        static TextureFrame[] textures = null;
        public static TextureFrame[] Textures
        {
            get
            {
                if (textures == null)
                {
                    var tex = Global.Game.Content.Load<Texture2D>("Environment/City/Buildings");
                    textures = Global.Game.LoadAtlasAnimation("Environment/City/Buildings", Vector2.Zero, new Vector2(tex.Width / 4, tex.Height), 4, TextureDirection.Horizontal);
                }
                return textures;
            }
        }
        Vector2 dScale = Vector2.Zero;
        public Building(Game1 Doc, TextureFrame Tex, float X, float Y, float Z, int realWidth, int realHeight)
            : base(Doc, Tex, X, Y, Z)
        {
            Scale = new Microsoft.Xna.Framework.Vector2(5);
            dScale = new Vector2(realWidth, realHeight) / new Vector2(Tex.Width, Tex.Height);
            DeactivateOffscreen = true;
        }
        public override StageObjectProperty[] GetStageProperties()
        {
            return new StageObjectProperty[]
            {
                new StageObjectProperty() { Name = "Scale", Value = Scale.X }
            };
        }
        public override void SetStageProperties(params StageObjectProperty[] sop)
        {
            foreach (var s in sop)
            {
                if (s.Name == "Scale")
                {
                    Scale = new Microsoft.Xna.Framework.Vector2(s.GetFloat());
                }
            }
        }
        public override Vector2 GetDrawScale()
        {
            return base.GetDrawScale() * dScale;
        }
    }
    public class Building1 : Building
    {
        public Building1(Game1 Doc, float X, float Y, float Z)
            : base(Doc, /*Doc.LoadTex("Environment/City/Building1")*/Building.Textures[0], X, Y, Z, 720, 1280)
        {

        }
    }
    public class Building2 : Building
    {
        public Building2(Game1 Doc, float X, float Y, float Z)
            : base(Doc, /*Doc.LoadTex("Environment/City/Building2")*/Building.Textures[1], X, Y, Z, 720, 1280)
        {

        }
    }
    public class Building3 : Building
    {
        public Building3(Game1 Doc, float X, float Y, float Z)
            : base(Doc, /*Doc.LoadTex("Environment/City/Building3")*/Building.Textures[2], X, Y, Z, 720, 1280)
        {

        }
    }
    public class Building4 : Building
    {
        public Building4(Game1 Doc, float X, float Y, float Z)
            : base(Doc, /*Doc.LoadTex("Environment/City/Building4")*/Building.Textures[3], X, Y, Z, 720, 1280)
        {

        }
    }
    public class Rail : Obj
    {
        TextureFrame railGlass;
        public Rail(Game1 Doc, float X, float Y, float Z)
            : base(Doc, Doc.LoadTex("Environment/City/Rail"), X, Y, Z)
        {
            railGlass = Doc.LoadTex("Environment/City/RailGlass");
            Snap.X = 128;
            DeactivateOffscreen = true;
            SlowDown = false;
        }
        public override void Draw()
        {
            SetDrawTechniques();
            Global.Drawer.DrawRelativeToObj(this, railGlass, Vector3.Zero, GetDrawColor() * GetDrawAlpha(), Origin.ToVector3());
            //doc.DrawSprite(railGlass, Pos, GetDrawColor() * GetDrawAlpha(), Rotation.Z, Origin, Scale, spriteEffect);
            base.Draw();
        }
    }
    public class Train : Obj
    {
        SoundEffectInstance sound;
        public float StartX = 0;
        public float EndX = -10000;
        public Train(Game1 Doc, float X, float Y, float Z)
            : base(Doc, Doc.LoadTex("Environment/City/Train"), X, Y, Z)
        {
            Depth = 1;
            sound = Doc.LoadSoundEffectInstance("Train");
            sound.IsLooped = true;
            sound.Play();
            Speed.X = -20;
        }
        public override StageObjectProperty[] GetStageProperties()
        {
            return new StageObjectProperty[]
            {
                new StageObjectProperty() { Name = "StartX", Value = StartX },
                new StageObjectProperty() { Name = "EndX", Value = EndX },
            };
        }
        public override void Remove()
        {
            sound.Stop();
            base.Remove();
        }
        public override void SetStageProperties(params StageObjectProperty[] sop)
        {
            foreach (var s in sop)
            {
                if (s.Name == "StartX")
                    StartX = s.GetFloat();
                else if (s.Name == "EndX")
                    EndX = s.GetFloat();
            }
        }
        public override void Update()
        {
            sound.Pan = GetSoundPan();
            sound.Volume = GetSoundVolume(2 * Alpha);
            sound.Pitch = GetSoundPitchDoppler();
            base.Update();
            if (Speed.X < 0)
            {
                if (Pos.X < EndX)
                {
                    AlphaSpeed = -0.1f;
                    if (Alpha < 0)
                    {
                        AlphaSpeed = 0.1f;
                        Pos.X = StartX;
                    }
                }
                else
                {
                    if (Alpha > 1)
                    {
                        Alpha = 1;
                        AlphaSpeed = 0;
                    }
                }
            }
        }
    }
}
