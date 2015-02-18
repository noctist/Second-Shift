using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SecondShiftMobile
{
    public class Sun : LightBloom
    {
        public Sun(Game1 Doc, float X, float Y, float Z)
            :base(Doc, X, Y, Z, (Global.Effects.Quality > Quality.Medium) ? 512 : 512)
        {
            //Category = LightBloomCategory.Sun;
            Color = new Microsoft.Xna.Framework.Color(200, 120, 80, 255);
            OverlayColor = Color;
            ZoomColor = new Microsoft.Xna.Framework.Color(255, 210, 180, 255);
            this.BloomWidth = 40;
            this.BloomIntensity = 0.75f;
            this.OverlayScale = 0.2f;
            OverlayAlpha = 0.9f;
            this.ZoomScale = 3.0f;
            ZoomTimes = 15;
            ZoomIntensity = 0.110f;
            FlareAlpha = 1f;
            Scale = new Vector2(1000);
            BlackCircleAlpha = 0.4f;
            BlackCircleScale = 3;
            ZoomBlendState = BlendState.Additive;
            //Active = false;
            //BlendState = BlendState.AlphaBlend;
            //Active = false;
        }
        public override void QualityChanged(Quality quality)
        {
            CreateRenderTargets((quality > Quality.Medium) ? 512 : 512);
            base.QualityChanged(quality);
        }
        public override StageObjectProperty[] GetStageProperties()
        {
            return new StageObjectProperty[]
            {
                new StageObjectProperty() { Name = "Scale", Info = "A single float representing scale", Value = Scale.X },
                new StageObjectProperty() { Name = "BloomWidth", Info = "The width of the bloom effect", Value = BloomWidth },
                new StageObjectProperty() { Name = "BloomIntensity", Info = "The intensity of the bloom effect", Value = BloomIntensity },
                new StageObjectProperty() { Name = "ZoomScale", Info = "The scale of the zoom effect", Value = ZoomScale },
                new StageObjectProperty() { Name = "Speed", Info = "The speed of the sun", Value = Speed }
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
                else if (s.Name == "BloomWidth")
                {
                    BloomWidth = s.GetFloat();
                }
                else if (s.Name == "BloomIntensity")
                {
                    BloomIntensity = s.GetFloat();
                }
                else if (s.Name == "ZoomScale")
                {
                    ZoomScale = s.GetFloat();
                }
                else if (s.Name == "Speed")
                {
                    Speed = s.GetVector3();
                }
            }
        }
    }
}
