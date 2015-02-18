using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
namespace SecondShiftMobile.Test
{
    public class FlatFloor : Floor
    {
        public FlatFloor(Game1 Doc, float X, float Y, float Z)
            : base(Doc, Doc.LoadTex("Environment/Grass/Flat Grass"), X, Y, Z)
        {
            Rotation.X = 90;
            BoundingRectangle = new Microsoft.Xna.Framework.Rectangle(0, Texture.Height / 2, 0, 0);
            SamplerState = SamplerState.LinearWrap;
        }
        public override void Update()
        {
            SortingType = (Global.Camera.View.Y > Pos.Y) ? DepthSortingType.Top : DepthSortingType.Bottom;
        }
        public override void SetStageProperties(params StageObjectProperty[] sop)
        {
            base.SetStageProperties(sop);
            foreach (var s in sop)
            {
                if (s.Name == "Scale")
                {
                    Scale = s.GetVector2();
                }
                else if (s.Name == "Texture Scale")
                {
                    TextureScale = s.GetVector2();
                }
            }
        }
        protected override void SetScreenAlpha()
        {
            ScreenAlpha = 1;
        }
        public override StageObjectProperty[] GetStageProperties()
        {
            return new StageObjectProperty[] 
            {
                new StageObjectProperty() { Name = "Scale", Value = Scale },
                new StageObjectProperty() { Name = "Texture Scale", Value = TextureScale }
            };
        }
    }
}
