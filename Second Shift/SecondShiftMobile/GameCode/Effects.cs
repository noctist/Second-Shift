using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecondShiftMobile
{
    public enum Quality { VeryLow, Low, Medium, High, VeryHigh };
    public enum Techniques { 
        Normal, NormalDepth, Blur, ScreenBloom, ScreenObjectBloom, LightBloom, Smoke, Selected, Distortion, Mask, DepthMask, GodRays, SlowDown, ScreenFinal, Bevel, Average,
        GetAverageColor, GetAverageColorMedium, GetAverageColorSmall, Polygon, PlainNormal
    };
    public class Effects : Effect
    {
        Quality quality;
        public Quality Quality
        {
            get
            {
                return quality;
            }
            set
            {
                if (quality != value)
                {
                    quality = value;
                    if (QualityChanged != null)
                    {
                        QualityChanged(this, new EventArgs());
                    }
                }
            }
        }
        public event EventHandler QualityChanged;
        public static Dictionary<Quality, string> QualityNameMap = new Dictionary<Quality,string>()
        {
            {Quality.VeryLow, "Very Low"},
            {Quality.VeryHigh, "Very High"}
        };
        public static string GetQualityName(Quality q)
        {
            if (QualityNameMap.ContainsKey(q))
            {
                return QualityNameMap[q];
            }
            else return q.ToString();
        }
        float blur = 0.00f, depth = 50, depthRange = 8000;
        float depthTarget = 50, depthSpeed = 1, depthRangeTarget = 8000, depthRangeSpeed = 1;
        float bloom = 0f, bloomTarget = 0, bloomSpeed = 1;
        float blurTarget = 0, blurSpeed = 1;
        float objBloom = 0;
        float objDistort = 0;
        public float Bloom
        {
            get { return bloom; }
        }
        float backgroundBloom = 0.0f;
        public float BackgroundBloom
        {
            get
            {
                return backgroundBloom;
            }
            set
            {
                backgroundBloom = value;
            }
        }
        public float ObjBloom
        {
            set
            {
                if (objBloom != value)
                {
                    objBloom = value;
                    Parameters["objBloom"].SetValue(value);
                }
            }
        }
        public float ObjDistort
        {
            set
            {
                if (objDistort != value)
                {
                    objDistort = value;
                    Parameters["distortion"].SetValue(value);
                }
            }
            get
            {
                return objDistort;
            }
        }
        float smokeAlpha = 0;
        public float SmokeAlpha
        {
            get
            {
                return smokeAlpha;
            }
            set
            {
                if (smokeAlpha != value)
                {
                    smokeAlpha = value;
                    Parameters["smokeAlpha"].SetValue(value);
                }
            }
        }
        
        public bool Blurring
        {
            get { return blur > 0.001f && Quality >= Quality.Medium && !LevelBuilder.Active; }
        }
        public bool Blooming
        {
            get { return (bloom > 0.01f || backgroundBloom > 0.01f) && Quality > Quality.VeryLow; }
        }
        public float Blur
        {
            get { return blur; }
        }

        public float Depth
        {
            get { return depth; }
        }

        public float DepthRange
        {
            get { return depthRange; }
        }

        public Color FogColor = Color.Black;
        public float FogDistance = 5000;
        public float FogDistanceTarget = 5000;

        public float FogStartDistance = 2000;
        public float FogStartDistanceTarget = 2000;

        public float FogIntensity = 1;
        public float FogIntensityTarget =0;

        public float FogSpeed = 1;
        
        Techniques tech;
        public Techniques Technique
        {
            set
            {
                if (tech != value)
                {
                    tech = value;
                    switch (value)
                    {
                        default:
                            CurrentTechnique = base.Techniques["Normal"];
                            break;
                        case SecondShiftMobile.Techniques.PlainNormal:
                            CurrentTechnique = base.Techniques["PlainNormal"];
                            break;
                        case SecondShiftMobile.Techniques.NormalDepth:
                            CurrentTechnique = base.Techniques["NormalDepth"];
                            break;
                        case SecondShiftMobile.Techniques.Blur:
                            if (Quality <= Quality.Medium)
                            {
                                CurrentTechnique = base.Techniques["BlurMedium"];
                            }
                            else
                            {
                                CurrentTechnique = base.Techniques["BlurHigh"];
                            }
                            break;
                        case SecondShiftMobile.Techniques.ScreenBloom:
                            CurrentTechnique = Techniques["Bloom"];
                            break;
                        case SecondShiftMobile.Techniques.ScreenObjectBloom:
                            if (Quality <= Quality.Medium)
                            {
                                CurrentTechnique = Techniques["ObjBloom"];
                            }
                            else
                            {
                                CurrentTechnique = Techniques["ObjBloomHigh"];
                            }
                            break;
                        case SecondShiftMobile.Techniques.Smoke:
                            CurrentTechnique = Techniques["Smoke"];
                            break;
                        case SecondShiftMobile.Techniques.LightBloom:
                            if (Quality <= Quality.High)
                            {
                                CurrentTechnique = Techniques["LightBloom"];
                            }
                            else
                            {
                                CurrentTechnique = Techniques["LightBloomHigh"];
                            }
                            break;
                        case SecondShiftMobile.Techniques.Selected:
                            CurrentTechnique = Techniques["Selected"];
                            break;
                        case SecondShiftMobile.Techniques.Distortion:
                            if (Quality >= Quality.Medium)
                            {
                                CurrentTechnique = Techniques["Distort"];
                            }
                            else
                            {
                                CurrentTechnique = Techniques["Normal"];
                            }
                            break;
                        case SecondShiftMobile.Techniques.Mask:
                            CurrentTechnique = Techniques["Mask"];
                            break;
                        case SecondShiftMobile.Techniques.GodRays:
                            if (Quality <= Quality.Medium)
                            {
                                CurrentTechnique = Techniques["GodRaysLow"];
                            }
                            else if (Quality == Quality.High)
                            {
                                CurrentTechnique = Techniques["GodRays"];
                            }
                            else if (Quality == SecondShiftMobile.Quality.VeryHigh)
                            {
                                CurrentTechnique = Techniques["GodRaysHigh"];
                            }
                            break;
                        case SecondShiftMobile.Techniques.SlowDown:
                            CurrentTechnique = Techniques["SlowDown"];
                            break;
                        case SecondShiftMobile.Techniques.ScreenFinal:
                            if (Quality >= Quality.Low)
                            {
                                CurrentTechnique = Techniques["ScreenFinal"];
                            }
                            else
                            {
                                CurrentTechnique = Techniques["Normal"];
                            }
                            break;
                        case SecondShiftMobile.Techniques.Bevel:
                            if (Quality >= Quality.High)
                            {
                                CurrentTechnique = Techniques["Bevel"];
                            }
                            else
                            {
                                CurrentTechnique = Techniques["BevelLow"];
                            }
                            break;
                        case SecondShiftMobile.Techniques.Average:
                            CurrentTechnique = Techniques["Average"];
                            break;
                        case SecondShiftMobile.Techniques.GetAverageColor:
                            CurrentTechnique = Techniques["GetAverageColor"];
                            break;
                        case SecondShiftMobile.Techniques.GetAverageColorSmall:
                            CurrentTechnique = Techniques["GetAverageColorSmall"];
                            break;
                        case SecondShiftMobile.Techniques.GetAverageColorMedium:
                            CurrentTechnique = Techniques["GetAverageColorMedium"];
                            break;
                        case SecondShiftMobile.Techniques.Polygon:
                            CurrentTechnique = Techniques["Polygon"];
                            break;
                        case SecondShiftMobile.Techniques.DepthMask:
                            CurrentTechnique = Techniques["DepthMask"];
                            break;
                    }
                }
            }
            get { return tech; }
        }

        Matrix matrix;
        public Matrix MatrixTransform
        {
            set
            {

                if (matrix != value)
                {
                    matrix = value;
                    base.Parameters["MatrixTransform"].SetValue(value);
                }
            }
            get
            {
                return matrix;
            }
        }
        Vector4 cont = new Vector4(1, 1, 1, 0.5f);
        public Vector4 Contrast
        {
            get { return cont; }
            set
            {
                if (cont != value)
                {
                    cont = value;
                    base.Parameters["contrastColor"].SetValue(value);
                }
            }
        }


        float drawDepth = 0;
        public float DrawDepth
        {
            get { return drawDepth; }
            set
            {
                if (drawDepth != value)
                {
                    drawDepth = value;
                    Parameters["depth"].SetValue(value);
                }
            }
        }

        Vector2 bevelDelta = Vector2.One;
        public Vector2 BevelDelta
        {
            get
            {
                return bevelDelta;
            }
            set
            {
                if (bevelDelta != value)
                {
                    bevelDelta = value;
                    Parameters["bevelDelta"].SetValue(value);
                }
            }
        }
        Vector3 bevelColor = Vector3.One;
        public Vector3 BevelColor
        {
            get
            {
                return bevelColor;
            }
            set
            {
                if (bevelColor != value)
                {
                    bevelColor = value;
                    Parameters["bevelColor"].SetValue(value);
                }
            }
        }

        float bevelGlow = 1;
        public float BevelGlow
        {
            get
            {
                return bevelGlow;
            }
            set
            {
                if (bevelGlow != value)
                {
                    bevelGlow = value;
                    Parameters["bevelGlow"].SetValue(bevelGlow);
                }
            }
        }

        public Texture2D DepthMap
        {
            set
            {
                Parameters["depthTexture"].SetValue(value);
            }
        }
        Vector3 objPos = Vector3.Zero;
        public Vector3 ObjPos
        {
            get
            {
                return objPos;
            }
            set
            {
                if (objPos != value)
                {
                    objPos = value;
                    Parameters["objPos"].SetValue(value);
                }
            }
        }
        public int NearZPlane = 1000;
        public int FarZPlane = -150000;
        public Effects(Effect effect)
            :base(effect)
        {
            Technique = SecondShiftMobile.Techniques.Normal;
            //Parameters["closeDepth"].SetValue(CloseDepth);
            //Parameters["farDepth"].SetValue(FarDepth);
        }
        public void Update()
        {
            blur += (blurTarget - blur) * blurSpeed * Global.Speed;
            bloom += (bloomTarget - bloom) * bloomSpeed * Global.Speed;
            depth += (depthTarget - depth) * depthSpeed * Global.Speed;
            depthRange += (depthRangeTarget - depthRange) * depthRangeSpeed * Global.Speed;
            //Parameters["closeBlur"].SetValue(MyMath.BetweenValue(CloseDepth, FarDepth, depth));
            //Parameters["farBlur"].SetValue(MyMath.BetweenValue(CloseDepth, FarDepth, depth + depthRange));
            FogDistance += (FogDistanceTarget - FogDistance) * FogSpeed * Global.Speed;
            FogStartDistance += (FogStartDistanceTarget - FogStartDistance) * FogSpeed * Global.Speed;
            FogIntensity += (FogIntensityTarget - FogIntensity) * FogSpeed * Global.Speed;
        }
        public void SetObjValues(Obj obj)
        {
            SetObjValues(obj.Pos, obj.Origin.ToVector3(), obj.Rotation, obj.GetDrawScale().ToVector3(), obj.TextureScale);
        }
        public void SetObjValues(Vector3 Pos, Vector3 Origin, Vector3 Rotation, Vector3 Scale, Vector2 TextureScale)
        {
            var m = Matrix.CreateTranslation(-Origin)
                * Matrix.CreateScale(Scale)
                * Matrix.CreateRotationX(MathHelper.ToRadians(Rotation.X))
                * Matrix.CreateRotationY(MathHelper.ToRadians(Rotation.Y))
                * Matrix.CreateRotationZ(MathHelper.ToRadians(Rotation.Z))
                * Matrix.CreateTranslation(Pos);
            Parameters["ObjectTransform"].SetValue(m);
            //Parameters["textureScale"].SetValue(TextureScale);
        }
        public float GetDepthValue(float Z)
        {
            return MyMath.BetweenValue(-NearZPlane, -FarZPlane, Z);
        }
        public void SetCameraValues(Camera camera)
        {
            try
            {
                Parameters["cameraPos"].SetValue(camera.View);
                Parameters["cameraDepthSize"].SetValue(camera.DepthSize);
                Parameters["cameraDepthPower"].SetValue(camera.DepthPower);
                Parameters["cameraLookDirection"].SetValue(camera.LookDirection);
                Parameters["nearZPlane"].SetValue((float)NearZPlane);
                Parameters["farZPlane"].SetValue((float)FarZPlane);
            }
            catch (Exception ex)
            {
                Helper.Write(ex);
            }
        }
        public void SetBlur(float target, float speed)
        {
            blurTarget = target;
            blurSpeed = speed;
        }
        public void SetBlur(float target, float speed, float startBlur)
        {
            blur = startBlur;
            blurTarget = target;
            blurSpeed = speed;
        }
        public void SetDepth(float target, float speed, float startDepth = float.NaN)
        {
            depthTarget = target;
            depthSpeed = speed;
            if (!float.IsNaN(startDepth))
                depth = startDepth;
        }
        public void SetDepthRange(float target, float speed, float startRange = float.NaN)
        {
            depthRangeTarget = target;
            depthRangeSpeed = speed;
            if (!float.IsNaN(startRange))
                depthRange = startRange;
        }
        public void SetBloom(float target, float speed)
        {
            bloomTarget = target;
            bloomSpeed = speed;
        }
        public void SetBloom(float target, float speed, float startBloom)
        {
            bloom = startBloom;
            bloomTarget = target;
            bloomSpeed = speed;
        }
        public void SetValue(KeyValuePair<string, object> pair)
        {
            SetValue(pair.Key, pair.Value);
        }
        public void SetValue(string name, object value)
        {
            if (value is float)
            {
                Parameters[name].SetValue((float)value);
            }
            else if (value is Vector2)
            {
                Parameters[name].SetValue((Vector2)value);
            }
            else if (value is Vector3)
            {
                Parameters[name].SetValue((Vector3)value);
            }
            else if (value is Vector4)
            {
                Parameters[name].SetValue((Vector4)value);
            }
            else if (value is bool)
            {
                Parameters[name].SetValue((bool)value);
            }
            else if (value is Color)
            {
                Parameters[name].SetValue(((Color)value).ToVector4());
            }
        }
        public void SetValues()
        {
            try
            {
                Parameters["blur"].SetValue(blur);
                Parameters["bloom"].SetValue(bloom);
                Parameters["blurStart"].SetValue(MyMath.BetweenValue(-NearZPlane, -FarZPlane, Depth));
                Parameters["blurEnd"].SetValue(MyMath.BetweenValue(-NearZPlane, -FarZPlane, Depth + DepthRange));

            }
            catch (Exception ex)
            {
                Helper.Write(ex);
            }
        }
    }
}
