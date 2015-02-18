using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SecondShiftMobile
{
    public class LensFlare : Obj
    {
        public float CloseDist = 300;
        public float FarDist = 900;
        LensFlareData[] data;
        public Obj Target;
        public LensFlare(Game1 Doc)
            : this(Doc, 0, 0, 0)
        {
            
        }
        public LensFlare(Game1 Doc, float X, float Y, float Z)
            : base(Doc, Doc.Content.Load<Texture2D>("Light"), X, Y, Z)
        {
            CreateFlareData();
            SortingType = SecondShiftMobile.DepthSortingType.Top;
            Collidable = false;
            SlowDown = false;
            BlendState = BlendState.Additive;
        }
        public override void LateUpdate()
        {
            if (Target != null)
            {
                Pos = Target.Pos;
                Alpha = Target.Alpha;
                if (Target.RemoveCalled)
                    Remove();
            }
            base.LateUpdate();
        }
        protected override void SetOnScreen()
        {
            OnScreen = true;
        }
        public virtual void CreateFlareData()
        {
            data = new LensFlareData[]
            {
                new LensFlareData() { TextureName = "Light", Alpha = 0.4f, Distance = 0.0f, Scale = new Vector2(10, 1f), Color = Color.Coral },
                new LensFlareData() { TextureName = "Flares/Flare", Alpha = 0.5f, Distance = -0.4f, Scale = new Vector2(0.3f), Color = Color.Aqua },
                new LensFlareData() { TextureName = "Flares/Flare", Alpha = 0.75f, Distance = 0.2f, Scale = new Vector2(0.1f), Color = Color.Orange },
                new LensFlareData() { TextureName = "Light", Alpha = 0.4f, Distance = 0.34f, Scale = new Vector2(0.5f), Color = Color.Fuchsia },
                new LensFlareData() { TextureName = "Flares/Flare", Alpha = 0.6f, Distance = 0.45f, Scale = new Vector2(0.5f), Color = Color.LightPink },
                new LensFlareData() { TextureName = "Flares/Flare2", Alpha = 0.3f, Distance = 0.55f, Scale = new Vector2(0.25f), Color = Color.Cornsilk },
                new LensFlareData() { TextureName = "Light", Alpha = 0.3f, Distance = 0.75f, Scale = new Vector2(0.15f), Color = Color.LawnGreen },
                new LensFlareData() { TextureName = "Flares/Flare2", Alpha = 0.4f, Distance = 1.2f, Scale = new Vector2(1.5f), Color = Color.DarkOrchid },
                new LensFlareData() { TextureName = "Flares/Flare", Alpha = 0.6f, Distance = 1.5f, Scale = new Vector2(1.0f), Color = Color.Honeydew }
            };
        }
        public override void Draw()
        {
            var zPos = Global.Camera.GetScreenPosition(Pos);
            var zFactor = Camera.GetZFactor(Camera.GetZDiff(Global.Camera.View.Z, Pos.Z, Global.Camera.DepthSize), Global.Camera.DepthSize, Global.Camera.DepthPower);
            float distance = Vector2.Distance(zPos, new Vector2(Global.Camera.View.X, Global.Camera.View.Y));
            float distFactor = Vector2.Distance(zPos, new Vector2(Global.Camera.View.X, Global.Camera.View.Y)) / 320;
            float scaleFactor = (float)Math.Pow(distFactor, 2);
            float flareAlpha;

            if (distance < (Texture.Width * Scale.X * zFactor) + CloseDist)
            {
                flareAlpha = MyMath.BetweenValue((Texture.Width * Scale.X * zFactor), (Texture.Width * Scale.X * zFactor) + CloseDist, distance);
            }
            else if (distance > FarDist)
            {
                flareAlpha = MyMath.BetweenValue(FarDist + 300, FarDist, distance);
            }
            else flareAlpha = 1;
            if (Target != null && Target is SmokeEmitter)
            {
                //Global.Output = Scale * new Vector2(scaleFactor);
            }

            if (Alpha > 0.005f)
            {
                /*Global.Effects.Technique = Techniques.Normal;
                doc.GraphicsDevice.BlendState = BlendState.Additive;
                Global.Effects.CurrentTechnique.Passes[0].Apply();*/
                foreach (var flare in data)
                {
                    Vector3 pos = new Vector3(0, 0, Global.Camera.View.Z);
                    pos.X = MyMath.Between(zPos.X, Global.Camera.View.X, flare.Distance);
                    pos.Y = MyMath.Between(zPos.Y, Global.Camera.View.Y, flare.Distance);
                    Global.Drawer.Draw(flare.Texture, Techniques.Normal, pos, flare.Color * Alpha * flare.Alpha, flare.Texture.Texture.GetCenter().ToVector3(), Vector3.Zero, (Scale * flare.Scale).ToVector3(1), BlendState.Additive, DepthSortingType.Top);
                }
            }
        }
    }
    public class LensFlareData
    {
        public Vector2 Scale = Vector2.One;
        public float Alpha = 0.5f;
        public float Power = 1;
        public float Distance = 0.5f;
        public Color Color = Color.White;
        public string TextureName
        {
            set { Texture = Global.Game.LoadTex(value); }
        }
        public TextureFrame Texture { get; private set;}
    }
}
