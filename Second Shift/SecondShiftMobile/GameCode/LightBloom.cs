using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SecondShiftMobile
{
    public enum LightBloomCategory { Default, Sun }
    public class LightBloom : Obj
    {
        //public bool FullZoom = true;
        public LightBloomCategory Category = LightBloomCategory.Default;
        public BlendState ZoomBlendState = BlendState.Additive;
        public Color OverlayColor = Color.White;
        Color? zoomColor = null;
        public Color ZoomColor
        {
            get
            {
                if (!zoomColor.HasValue)
                {
                    return Color;
                }
                else return (Color)zoomColor;
            }
            set
            {
                zoomColor = value;
            }
        }
        public int Resolution
        {
            get { return resolution; }
        }
        public float BloomWidth = 10;
        public float BloomIntensity = 0;
        public float OverlayScale = 0.3f;
        public int ZoomTimes = 15;
        public float ZoomScale = 4;
        public float ZoomIntensity = 1;
        protected float blockAlpha = 1;
        RenderTarget2D renTarg, renTarg2, blockTarg, finalTarg;
        int resolution;
        float scaleFactor = 1;
        Vector2 lightOrigin;
        Color[] colorData;
        Color[] singlePixelData;
        LensFlare flare;
        public float FlareAlpha = 1;
        public float FlareScale = 1;
        public float OverlayAlpha = 1;
        public Texture2D FlareTex;
        float overlayScale = 0, overlayAlpha = 0;
        float overlayScaleTarget = 0, overlayTargetAlpha = 0;
        public float BlackCircleAlpha = 1, BlackCircleScale = 2, BlockTargetAlpha = 0.5f;
        public bool Rays3D = false;
        public float AverageAlpha = 254.9f;
        float objScale = 1;
        float blackCircleAl = 0;
        RenderTarget2D singlePixelTarget = null;
        void CreateSinglePixelTarget()
        {
            if (singlePixelTarget == null)
            {
                singlePixelTarget = new RenderTarget2D(Global.Game.GraphicsDevice, 3, 1);
                singlePixelData = new Color[singlePixelTarget.Width * singlePixelTarget.Height];
            }
        }
        public bool Drawable
        {
            get { return Active && !RemoveCalled && !renTarg.IsDisposed && !renTarg2.IsDisposed && renTarg != null && renTarg2 != null; }
        }
        public LightBloom(Game1 Doc, float X, float Y, float Z, int resolution = 100, string textureName = "Light2")
            : base(Doc,Doc.Content.Load<Texture2D>(textureName), X, Y, Z)
        {
            IsLight = true;
            FlareTex = Doc.LoadTex("Sun");
            CreateRenderTargets(resolution);
            
            flare = new LensFlare(doc);
            Collidable = false;
            BlendState = BlendState.Additive;
            SunBlockAlpha = 0;
            //Texture = renTarg;
        }
        public virtual void CreateRenderTargets(int resolution)
        {
            if (this.resolution != resolution)
            {
                this.resolution = resolution;
                int wid = (int)((float)resolution * ((float)Texture.Height / (float)Texture.Width));
                bool mipMap = resolution.IsPowerOf2() && wid.IsPowerOf2();

                GameExtensions.TryDispose(renTarg, renTarg2, blockTarg, finalTarg, singlePixelTarget);
                renTarg = new RenderTarget2D(doc.GraphicsDevice, resolution, wid, mipMap, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
                renTarg2 = new RenderTarget2D(doc.GraphicsDevice, resolution, wid, mipMap, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
                blockTarg = new RenderTarget2D(doc.GraphicsDevice, resolution, wid, mipMap, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
                finalTarg = new RenderTarget2D(doc.GraphicsDevice, resolution, wid, mipMap, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
                lightOrigin = new Vector2(renTarg.Width / 2, renTarg.Height / 2);
                scaleFactor = (float)Texture.Width / (float)resolution;
                colorData = new Color[renTarg.Width * renTarg.Height];
                CreateSinglePixelTarget();
            }
        }
        public override void EarlyUpdate()
        {
            base.EarlyUpdate();
            //if (FullZoom)
            {
                objScale = MathHelper.Clamp(ZoomScale, 1, 1000);
            }
            flare.Pos = Pos;
            flare.Scale = new Vector2(FlareScale);
            flare.Alpha = FlareAlpha * overlayAlpha * Alpha;
            //flare.Alpha = 1;
            if (Global.Effects.Quality <= Quality.Low)
            {
                Bloom = 1;
            }
            else
            {
                Bloom = 0;
            }
        }
        public override void Remove()
        {
            GameExtensions.TryDispose(renTarg, renTarg2, blockTarg, finalTarg, singlePixelTarget);
            flare.Remove();
            base.Remove();
        }
        public void DrawObjects()
        {
            var ss = doc.GraphicsDevice.SamplerStates[0];
            doc.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
            doc.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            doc.GraphicsDevice.SetRenderTarget(renTarg);
            doc.GraphicsDevice.Clear(Color.White);
            Rectangle sr = ScreenRect;
            sr.Inflate((int)(sr.Width * (objScale - 1) / 2), (int)(sr.Height * (objScale - 1) / 2));
            DrawBase(sr);
            
            Global.Effects.MatrixTransform = Matrix.CreateOrthographicOffCenter(sr.Left, sr.Right, sr.Bottom, sr.Top, 0, 1);
            Global.Effects.Technique = Techniques.Normal;
            for (int i = 0; i < doc.NumberOfObjects; i++)
            {
                var o = doc.objArray[i];
                if (o.Visible && o.Active && o.SunBlockAlpha > 0 && o != this && !o.IsLight && o.Pos.Z < Pos.Z && o.OnScreen)
                {
                    //o.BloomDraw(MathHelper.Clamp((Pos.Z - o.Pos.Z) / (Scale.X * 10), 0, 1));
                }
            }
            doc.GraphicsDevice.SamplerStates[0] = ss;
        }
        public virtual void DrawBase(Rectangle sr)
        {
            //if (!FullZoom)
            {
                Global.Effects.Technique = Techniques.DepthMask;
                Vector2 texStart = new Vector2(sr.Left, sr.Top) - Global.Camera.View.ToVector2() + (Global.Camera.CameraSize / 2);
                Vector2 texEnd = new Vector2(sr.Right, sr.Bottom) - Global.Camera.View.ToVector2() + (Global.Camera.CameraSize / 2);
                Global.Effects.Parameters["depthMaskTexStart"].SetValue(texStart / Global.Camera.CameraSize);
                Global.Effects.Parameters["depthMaskTexEnd"].SetValue(texEnd / Global.Camera.CameraSize);
                switch (Category)
                {
                    default:
                        Global.Effects.Parameters["depthMaskDepthStart"].SetValue(Global.Effects.GetDepthValue(Pos.Z - (Texture.Width * Scale.X * 0.5f)));
                        Global.Effects.Parameters["depthMaskDepthEnd"].SetValue(Global.Effects.GetDepthValue(Pos.Z + (Texture.Width * Scale.X * 0.5f)));
                
                        break;
                    case LightBloomCategory.Sun:
                        Global.Effects.Parameters["depthMaskDepthStart"].SetValue(Global.Effects.GetDepthValue(Pos.Z - (Texture.Width * Scale.X)));
                        Global.Effects.Parameters["depthMaskDepthEnd"].SetValue(Global.Effects.GetDepthValue(Pos.Z));
                        //Global.Effects.Parameters["depthMaskDepthStart"].SetValue(0.0f);
                        //Global.Effects.Parameters["depthMaskDepthEnd"].SetValue(0.1f);
                        break;
                }
                //Global.Effects.Parameters["depthMaskDepthStart"].SetValue(Global.Effects.GetDepthValue(Pos.Z - (Texture.Width * Scale.X * 0.5f)));
                //Global.Effects.Parameters["depthMaskDepthEnd"].SetValue(Global.Effects.GetDepthValue(Pos.Z + (Texture.Width * Scale.X * 0.5f)));
                //Global.Effects.Parameters["depthMaskTexStart"].SetValue(new Vector2(0.5f, 0));
                //Global.Effects.Parameters["depthMaskTexEnd"].SetValue(new Vector2(1, 0.5f));
                Global.Effects.MatrixTransform = Matrix.CreateOrthographicOffCenter(0, Texture.Width, Texture.Height, 0, 0, 1);
                Global.Effects.CurrentTechnique.Passes[0].Apply();
                doc.DrawSprite(Texture, Vector2.Zero, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
            }
        }
        public override void ParallelUpdate()
        {
            updateCount++;
            if (updateCount >= MaxUpdateCount)
            {
                GetBrightnessValues();
                updateCount = 0;
            }
            if (float.IsNaN(overlayScaleTarget))
                overlayScaleTarget = overlayScale;
            overlayScale += ((float)Math.Pow(overlayScaleTarget, 0.1) - overlayScale) * 0.05f;
            if (float.IsNaN(overlayScale))
                overlayScale = 1;
            overlayScale = MathHelper.Clamp(overlayScale, 0, OverlayScale);

            overlayTargetAlpha = MathHelper.Clamp((float)Math.Pow(blackCircleAl - 100, 1), 0, 1);
            if (float.IsNaN(overlayTargetAlpha))
                overlayTargetAlpha = 0;
            overlayAlpha += (overlayScaleTarget - overlayAlpha) * 0.3f;
            //overlayAlpha = overlayScaleTarget;
            blackCircleAl += (overlayAlpha - blackCircleAl) * 0.01f * Global.Speed;
            //blackCircleAl = 1;

            //overlayAlpha = 1;
            base.ParallelUpdate();
        }
        const int MaxUpdateCount = 5;
        int updateCount = MaxUpdateCount;
        public override void NonCriticalUpdate()
        {
            base.NonCriticalUpdate();
            
        }
        
        protected void GetBrightnessValues()
        {
            if (singlePixelTarget != null)
            {
                try
                {
                    singlePixelTarget.GetData<Color>(singlePixelData, 0, 3);
                    float normal = (singlePixelData[1].R + singlePixelData[1].G + singlePixelData[1].B) / (255f * 3f);
                    float medium = (singlePixelData[1].R + singlePixelData[1].G + singlePixelData[1].B) / (255f * 5.5f);
                    // Default division is (150f * 3f)
                    float small = MathHelper.Clamp((singlePixelData[2].R + singlePixelData[2].G + singlePixelData[2].B) / (150f * 3f), 0 ,1);
                    blockAlpha = medium;
                    overlayScaleTarget = MathHelper.Clamp(small, 0, 1);
                    //Global.Output = singlePixelData[0] + ", " + singlePixelData[1] + ", " + singlePixelData[2];
                }
                catch
                {

                }
            }
            /*float ba = 0;
            float oWidth = resolution / objScale;
            float oStart = (resolution - oWidth) / 2;
            float oEnd = oWidth + oStart;
            if (renTarg != null && !renTarg.IsDisposed)
            {
                try
                {
                    renTarg.GetData<Color>(colorData);
                    float alphaLevel = 0, alphaLevel2 = 0, alphaLevel3 = 0, num = 0, num2 = 0, num3 = 0;
                    for (int i = 0; i < colorData.Length; i += 5)
                    {
                        var c = colorData[i];
                        Vector2 v = new Vector2(i % renTarg.Width, i / renTarg.Height);
                        alphaLevel += (c.R + c.B + c.G) / 3f;
                        num++;
                        if (v.X > oStart && v.Y > oStart && v.X < oEnd && v.Y < oEnd)
                        {
                            alphaLevel3 += (c.R + c.B + c.G) / 3f;
                            num3++;
                        }

                        if ((lightOrigin - v).Length() < (renTarg.Width / (16f * objScale)))
                        {
                            alphaLevel2 += (c.R + c.B + c.G);
                            num2++;
                        }
                    }
                    alphaLevel /= 3;
                    blockAlpha = ((alphaLevel / num) / AverageAlpha);
                    //blackCircleAl = ((alphaLevel3 / num3) / AverageAlpha);
                    ba = ((alphaLevel2 / num2) / AverageAlpha);
                }
                catch
                {

                }
            }
            overlayScaleTarget = MathHelper.Clamp((float)Math.Pow(ba - 0.5f, 0.2), 0, 1);
            overlayScaleTarget = MathHelper.Clamp(ba, 0, 1);*/
        }
        public void DrawLight(Matrix resetMatrix)
        {
            float zFactor = Camera.GetZFactor(Global.Camera.View.Z, Pos.Z, Global.Camera.DepthSize, Global.Camera.DepthPower);
            //Vector2 drawPos = zPos - (new Vector2(Texture.Width / (float)renTarg.Width, Texture.Height / (float)renTarg.Height) * Scale * zFactor * ZoomScale * 2);
#if PC
            Vector2 resPos = -new Vector2(1);
#else 
            Vector2 resPos = Vector2.Zero;
#endif
            //resPos = Vector2.One;
            
            doc.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            #region Get Average Values
            var ss = doc.GraphicsDevice.SamplerStates[0];
            doc.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            CreateSinglePixelTarget();
            doc.GraphicsDevice.SetRenderTarget(singlePixelTarget);
            Global.Effects.MatrixTransform = Matrix.CreateOrthographicOffCenter(0, 3, 1, 0, 0, 1);
            Global.Effects.Technique = Techniques.GetAverageColor;
            foreach (var p in Global.Effects.CurrentTechnique.Passes)
            {
                p.Apply();
                doc.DrawSprite(renTarg, new Rectangle(0, 0, 1, 1), null, Color.White);
            }
            Global.Effects.Technique = Techniques.GetAverageColorMedium;
            foreach (var p in Global.Effects.CurrentTechnique.Passes)
            {
                p.Apply();
                doc.DrawSprite(renTarg, new Rectangle(1, 0, 1, 1), null, Color.White);
            }
            Global.Effects.Technique = Techniques.GetAverageColorSmall;
            foreach (var p in Global.Effects.CurrentTechnique.Passes)
            {
                p.Apply();
                doc.DrawSprite(renTarg, new Rectangle(2, 0, 1, 1), null, Color.White);
            }
            doc.GraphicsDevice.SamplerStates[0] = ss;
            #endregion
            Global.Effects.Technique = Techniques.Mask;
            Global.Effects.MatrixTransform = Matrix.CreateOrthographicOffCenter(0, renTarg.Width, renTarg.Height, 0, 0, 1);
            doc.GraphicsDevice.SetRenderTarget(blockTarg);

            doc.GraphicsDevice.Clear(Color.Transparent);
            Global.Effects.Parameters["blendTexture"].SetValue(Texture.Texture);
            Global.Effects.Parameters["blendScale"].SetValue(1 / objScale);
            Global.Effects.CurrentTechnique.Passes[0].Apply();
            doc.DrawSprite(renTarg, blockTarg.GetCenter(), Color.White, Rotation.Z, renTarg.GetCenter(), Vector2.One, spriteEffect, Depth);
            //doc.DrawSprite(FlareTex, blockTarg.GetCenter(


            

            var bsSubtract = new BlendState
            {
                AlphaBlendFunction = BlendFunction.Add,
                ColorBlendFunction = BlendFunction.ReverseSubtract,
                ColorDestinationBlend = Blend.DestinationAlpha,
                AlphaDestinationBlend = Blend.DestinationAlpha
            };

            
            doc.GraphicsDevice.BlendState = BlendState.Additive; ;
            //doc.DrawSprite(blockTarg, zPos + resPos, Color * Alpha * blockAlpha * 2, Rotation, lightOrigin, Scale * zFactor * scaleFactor, spriteEffect, Depth);
            float bloom = BloomIntensity * (1.0f - blackCircleAl);
            if (BloomIntensity > 0 && bloom > 0 && false)
            {
                doc.GraphicsDevice.BlendState = BlendState.AlphaBlend; ;
                Global.Effects.Parameters["bloomWidth"].SetValue((BloomWidth) / (Scale.X * zFactor * resolution));
                Global.Effects.Parameters["bloomIntensity"].SetValue(MathHelper.Clamp(bloom, 0, BloomIntensity));
                Global.Effects.Parameters["blendTexture"].SetValue(renTarg);
                Global.Effects.Parameters["bloomScale"].SetValue(objScale * 2);
                doc.GraphicsDevice.SetRenderTarget(renTarg2);
                doc.GraphicsDevice.Clear(Color.Transparent);
                Global.Effects.Technique = Techniques.LightBloom;
                Global.Effects.MatrixTransform = Matrix.CreateOrthographicOffCenter(0, renTarg.Width, renTarg.Height, 0, 0, 1);
                Global.Effects.CurrentTechnique.Passes[0].Apply();
                
                doc.DrawSprite(renTarg, Vector2.Zero, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
                
            }
            
            doc.GraphicsDevice.SetRenderTarget(finalTarg);
            doc.GraphicsDevice.Clear(Color.Transparent);
            Global.Effects.Technique = Techniques.Normal;
            //Global.Effects.MatrixTransform = resetMatrix;
            Global.Effects.CurrentTechnique.Passes[0].Apply();
            
            doc.DrawSprite(blockTarg, finalTarg.GetCenter() + resPos, Color * Alpha * blockAlpha * BlockTargetAlpha, Rotation.Z, blockTarg.GetCenter(), new Vector2(1) / objScale, spriteEffect, Depth);
            
            doc.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            if (BloomIntensity > 0 && bloom > 0 && false)
            {
                Global.Effects.Technique = Techniques.LightBloom;
                Global.Effects.CurrentTechnique.Passes[1].Apply();
            
                doc.DrawSprite(renTarg2, finalTarg.GetCenter() + resPos, Color * Alpha, Rotation.Z, lightOrigin, Vector2.One, spriteEffect, Depth);
            }


            doc.GraphicsDevice.SetRenderTarget(finalTarg);
            if (ZoomIntensity > 0 && ZoomTimes > 0)
            {
                doc.GraphicsDevice.BlendState = BlendState.AlphaBlend;
                Global.Effects.Technique = Techniques.GodRays;
                foreach (var pass in Global.Effects.CurrentTechnique.Passes)
                {
                    if (Rays3D)
                    {
                        Vector2 pos = GetRay3DPos();
                        Global.Effects.Parameters["rayDelta"].SetValue(pos);
                    }
                    else
                    {
                        Global.Effects.Parameters["rayDelta"].SetValue(Vector2.Zero);
                    }
                    //if (FullZoom)
                    {
                        Global.Effects.Parameters["godRayScale"].SetValue(ZoomScale / (objScale));
                    }
                    //else Global.Effects.Parameters["godRayScale"].SetValue(ZoomScale);

                    pass.Apply();
                    //Global.Effects.Parameters["godRayScale"].SetValue(1);
                    //doc.GraphicsDevice.SetRenderTarget(renTarg2);
                    doc.DrawSprite(renTarg, finalTarg.GetCenter() + resPos, ZoomColor * Alpha * ScreenAlpha * ZoomIntensity, Rotation.Z, renTarg.GetCenter(), Vector2.One, spriteEffect, Depth);
                }
            }
            Global.Effects.Technique = Techniques.Mask;
            Global.Effects.Parameters["blendTexture"].SetValue(FlareTex);
            float fScale = ((float)FlareTex.Width / 100f) * OverlayScale;
            Global.Effects.Parameters["blendScale"].SetValue(fScale / objScale);
            Global.Effects.CurrentTechnique.Passes[0].Apply();
#if MONO
            Vector2 sub = new Vector2(0);
#else
            Vector2 sub = new Vector2(1, 1);
#endif
            doc.DrawSprite(renTarg, finalTarg.GetCenter() - sub, OverlayColor * Alpha * 1.5f * (float)Math.Pow(1 - overlayAlpha, 0.5) * OverlayAlpha, 0, renTarg.GetCenter(), new Vector2(fScale / objScale), SpriteEffects.None, 0);
            
            
        }
        public virtual void DrawFinal()
        {
            //Vector3 offset = new Vector3(Global.Camera.LookDirection, 0) * ((1f / Camera.GetZFactor(Global.Camera.View.Z, this.Pos.Z, Global.Camera.DepthSize, Global.Camera.DepthPower)) - Camera.LookDirectionW);
            Global.Effects.Technique = Techniques.Normal;
            Global.Effects.SetObjValues(this.Pos, Origin.ToVector3(), Rotation, new Vector3(BlackCircleScale) * Scale.ToVector3(), Vector2.One );
            Global.Effects.CurrentTechnique.Passes[0].Apply();

            float blackAl = MathHelper.Clamp((float)Math.Pow(blackCircleAl, 0.5) * 2, 0, 2.0f);
            //Global.Output = blackAl;
            // Draw black circle
            doc.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            //doc.DrawSprite(renTarg, new Vector3(40, 40, 0), Color.White, 0, Vector2.Zero, new Vector2(0.1f), spriteEffect);
            //return;
            doc.DrawSprite(Texture.Texture, Vector3.Zero, Color.Black * Alpha * BlackCircleAlpha * (blackAl),  spriteEffect);

            doc.GraphicsDevice.BlendState = BlendState; ;

            //Global.Effects.Technique = Techniques.Flare;
            //Global.Effects.Parameters["flareAlpha"].SetValue(Alpha * OverlayAlpha * overlayAlpha);
            //Global.Effects.CurrentTechnique.Passes[0].Apply();
            Global.Effects.SetObjValues(this.Pos, FlareTex.GetCenter().ToVector3(), Rotation, Scale.ToVector3() * new Vector3(OverlayScale), Vector2.One);
            Global.Effects.CurrentTechnique.Passes[0].Apply();
            doc.DrawSprite(FlareTex, Vector3.Zero, OverlayColor * Alpha * OverlayAlpha * MathHelper.Clamp((float)Math.Pow(overlayAlpha, 1), 0, overlayAlpha), spriteEffect);

            //doc.GraphicsDevice.BlendState = BlendState.Additive;
            //Global.Effects.Technique = Techniques.Normal;
            //Global.Effects.CurrentTechnique.Passes[0].Apply();
            //doc.DrawSprite(blockTarg, GetScreenPosition(Global.Camera.View), Color.White, Rotation, new Vector2(250, 250), new Vector2(2), spriteEffect, Depth);
            Global.Effects.SetObjValues(this.Pos, finalTarg.GetCenter().ToVector3(), Rotation, Scale.ToVector3() * new Vector3(scaleFactor * objScale) / 1.0f, Vector2.One);
            //Global.Effects.SetObjValues(this.Pos, finalTarg.GetCenter().ToVector3(), Rotation, new Vector3(scaleFactor * objScale));
            Global.Effects.CurrentTechnique.Passes[0].Apply();
            doc.DrawSprite(finalTarg, Vector3.Zero, Color.White,  spriteEffect);
            //doc.DrawSprite(renTarg, Vector3.Zero, Color.White, SpriteEffects.None);
            //doc.DrawSprite(renTarg, zPos, Color.White, Rotation, finalTarg.GetCenter(), Scale * zFactor * scaleFactor, spriteEffect, 0);
            doc.GraphicsDevice.BlendState = BlendState.AlphaBlend;
        }
        protected virtual Vector2 GetRay3DPos()
        {
            var pos = GetScreenPosition(Pos);
            pos -= Global.Camera.View.ToVector2();
            pos /= Global.ScreenSize.X * 2;
            return -pos;
        }
    }
    
}
