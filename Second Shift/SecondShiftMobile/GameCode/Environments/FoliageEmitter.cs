using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SecondShiftMobile.Environments
{
    public class FoliageEmitter : Obj
    {
        Random rand;
        public Vector3 StartBox = Vector3.Zero, EndBox = Vector3.Zero;
        public float Step = 1;
        float time = 0;
        public string TexturePath = "Environment/Paper";
        public float MaxLifetime = 600;
        public float Gravity = 0.1f;
        public Vector3 StartSpeed = Vector3.Zero, EndSpeed = Vector3.Zero;
        public Color TextureColor = Color.White;
        public Vector2 TextureScale = Vector2.One;
        int seed = 0;
        public int Seed
        {
            get
            {
                return seed;
            }
            set
            {
                seed = value;
                rand = new Random(seed);
            }
        }
        public FoliageEmitter(Game1 Doc, float X, float Y, float Z)
            :base(Doc, Textures.SmallLight, X, Y, Z)
        {
            Color = Color.Black;
            Scale = new Vector2(10);
            SlowDown = false;
            rand = new Random(seed);
        }
        public override StageObjectProperty[] GetStageProperties()
        {
            return new StageObjectProperty[]
            {
                new StageObjectProperty() { Name = "Step", Value = Step },
                new StageObjectProperty() { Name = "StartBox", Value = StartBox },
                new StageObjectProperty() { Name = "EndBox", Value = EndBox },
                new StageObjectProperty() { Name = "StartSpeed", Value = StartSpeed },
                new StageObjectProperty() { Name = "EndSpeed", Value = EndSpeed }
            };
        }
        public override void SetStageProperties(params StageObjectProperty[] sop)
        {
            foreach (var s in sop)
            {
                if (s.Name == "StartBox")
                    StartBox = s.GetVector3();
                else if (s.Name == "EndBox")
                    EndBox = s.GetVector3();
                else if (s.Name == "Step")
                    Step = s.GetFloat();
                else if (s.Name == "StartSpeed")
                    StartSpeed = s.GetVector3();
                else if (s.Name == "EndSpeed")
                    EndSpeed = s.GetVector3();
            }
        }
        public override void Update()
        {
            time += PlaySpeed;
            while (time > Step)
            {
                time -= Step;
                TextureFrame t;
                try
                {
                    t = doc.LoadTex(TexturePath);
                }
                catch
                {
                    break;
                }
                Vector3 v = MyMath.RandomRange(StartBox, EndBox, rand) + Global.Camera.View;
                var f = new FoliageDisappear(doc, t, v.X, v.Y, v.Z) 
                { 
                    Lifetime = MaxLifetime, MaxAlpha = 1, Gravity = Gravity ,
                    Speed = MyMath.RandomRange(StartSpeed, EndSpeed, rand),
                    Color = TextureColor, Scale = TextureScale
                }; 
            }
            base.Update();
        }
        public override void Draw()
        {
            if (LevelBuilder.Active)
            {
                base.Draw();
            }
        }
        public override void BloomDraw(float alpha)
        {
            if (LevelBuilder.Active)
            {
                base.BloomDraw(alpha);
            }
        }
    }
    public class FoliageDisappear : Disappear
    {
        bool disappearOff = false;
        public float Gravity = 0;
        Vector3 windSpeed = Vector3.Zero;
        public FoliageDisappear(Game1 Doc, TextureFrame Tex, float X, float Y, float Z)
            : base(Doc, Tex, X, Y, Z)
        {

        }
        public override void Update()
        {
            base.Update();
            RotationSpeed.Z = Speed.X * 0.4f;
            Speed.Y += Gravity;
            if (OnScreen)
                disappearOff = true;
            if (!OnScreen && disappearOff)
            {
                Remove();
            }
        }
        public override Vector3 GetMoveSpeedOverride()
        {
            return base.GetMoveSpeedOverride() + windSpeed;
        }
        public override void ParallelUpdate()
        {
            windSpeed += (Vector3.Zero - windSpeed) * 0.05f * PlaySpeed;
            var winds = doc.FindObjects<Elements.Wind>();
            if (winds != null)
            {
                foreach (var wind in winds)
                {
                    if (IsCollidingWith(wind))
                    {
                        Vector3 windVal = (Vector3)wind.GetElementValue(this);
                        windSpeed += (windVal - windSpeed) * 0.5f * PlaySpeed;
                    }
                }
            }
            base.ParallelUpdate();
        }
    }
}
