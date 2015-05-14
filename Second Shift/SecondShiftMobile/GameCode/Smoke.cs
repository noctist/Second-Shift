using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecondShiftMobile
{
    public class Smoke : Disappear
    {
        public bool DrawNormal = false;
        public bool AffectedByWind = true;
        Vector3 windSpeed = Vector3.Zero;
        public Smoke(Game1 Doc, float X, float Y, float Z)
            : base(Doc, Doc.LoadTex("Smoke"), X, Y, Z)
        {
            SunBlockAlpha = 0.5f;
            Collidable = false;
            //ShaderValues.Add("smokeAlpha", 1f);
            //Parallel = true;
        }
        public override Vector3 GetMoveSpeedOverride()
        {
            return base.GetMoveSpeedOverride() + windSpeed;
        }
        public override void ParallelUpdate()
        {
            windSpeed += (-windSpeed) * 0.05f * PlaySpeed;
            if (AffectedByWind)
            {
                var winds = doc.FindObjects<Elements.Wind>();
                if (winds != null)
                {
                    foreach (var wind in winds)
                    {
                        if (IsCollidingWith(wind))
                        {
                            windSpeed += (Vector3)wind.GetElementValue(this) * PlaySpeed * 0.01f;
                        }
                    }
                }
            }
            base.ParallelUpdate();
        }
        public override Techniques GetDrawTechnique()
        {
            if (!DrawNormal)
            {
                //ShaderValues["smokeAlpha"] = Alpha * ScreenAlpha;
                return Techniques.Smoke;
            }
            else
            {
                return base.GetDrawTechnique();
            }
        }
        protected override void SetDrawTechniques()
        {
            
            Global.Effects.SmokeAlpha = (Alpha * ScreenAlpha);
            base.SetDrawTechniques();
        }
        public override void SetBloomTechnique()
        {
            Global.Effects.Technique = GetDrawTechnique();
            Global.Effects.SmokeAlpha = (Alpha * ScreenAlpha);
        }
        public override void Draw()
        {
            if (Alpha > 0.4f || DrawNormal)
            {
                base.Draw();
            }
        }
        public override bool CheckIfSpriteBatchChangeNeeded(Obj o)
        {
            return (base.CheckIfSpriteBatchChangeNeeded(o))
                || (Alpha * ScreenAlpha != Global.Effects.SmokeAlpha && !DrawNormal);
        }
        /*public override float GetDrawAlpha()
        {
            return (DrawNormal) ? base.GetDrawAlpha() : 1;
        }*/
        public override void DrawSecondShift()
        {
            
        }
        public override bool ShouldDrawSecondShift()
        {
            return false;
        }
    }
}
