using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SecondShiftMobile.Cutscenes;
using SecondShiftMobile.Networking;
namespace SecondShiftMobile
{
    public enum GameOrient { Left, Right };
    public enum GameState { None, Playing, Paused }
    public enum Resolutions { R240p = 240, R360p = 360, R480p = 480, R720p = 720, R1080p = 1080, R1440p = 1440, R2160p = 2160 }
    public static class Global
    {
        public static GameState GameState = GameState.Playing;
        public static Cutscene Cutscene = null;
        public static object Output = "";
        public static Game1 Game;
        public static Effects Effects;
        public static EffectsDrawer Drawer;
        public static Matrix Projection;
        public static GameOrient Orient = GameOrient.Left;
        public static Camera Camera;
        static float speed = 1, framespeed = 1, gamespeed = 1, speedtarget = 1f, speedspeed = 0.1f;
        public static Obj HUDObj;
        public static float FrameSpeed
        {
            get { return framespeed; }
        }
        public static Touch Touch;
        public static Controls Controls;
        public static Vector2 ScreenSize = new Vector2(1920, 1080);
        public static bool ShowFramerate = false;
        public static Vector3 TestVec = new Vector3(0, -1000, 0);
        public static Rectangle ScreenRect = new Rectangle();
        public static Rectangle NetworkScreenRect = new Rectangle();
        static Rectangle prevScreenRect = new Rectangle();
        static long frameCount = 0;
        public static long FrameCount { get { return frameCount; } }
        public static VirtualJoystick VirtualJoystick;
        public static float Speed
        {
            get 
            {
                if (LevelBuilder.Active)
                    return 0;
                return speed; 
            }
        }
        public static void init()
        {
#if DEBUG
            ShowFramerate = true;
#endif
            Textures.init();
            Fonts.init();
            LevelBuilder.init();
            SoundManager.init();
            VirtualJoystick = new SecondShiftMobile.VirtualJoystick() { Radius = 150, SnapY = false };
            Camera = new Camera();
            Touch = new SecondShiftMobile.Touch();
            Controls = new Controls();
        }
        public static void Update(GameTime time)
        {
            frameCount++;
            if (Global.GameState != SecondShiftMobile.GameState.Paused)
            {
                framespeed = (float)time.ElapsedGameTime.Milliseconds / (float)TimeSpan.FromSeconds(1d / 60d).Milliseconds;
                //framespeed = 1;
                gamespeed += (speedtarget - gamespeed) * speedspeed * framespeed;
                speed = gamespeed * framespeed;
                Camera.Update();
                ScreenRect = new Rectangle((int)(Camera.View.X - (Camera.CameraSize.X / 2)), (int)(Camera.View.Y - (Camera.CameraSize.Y / 2)), (int)Camera.CameraSize.X, (int)Camera.CameraSize.Y);
                if (ScreenRect != prevScreenRect && frameCount % 3 == 0)
                {
                    if (NetworkManager.SocketRole != SocketRole.None)
                    {
                        SocketMessage sm = new SocketMessage();
                        sm.Info.BaseAddress = "ScreenRect";
                        sm.Info["rect"] = ScreenRect.ToStageString();
                        sm.Send();
                        prevScreenRect = ScreenRect;
                    }
                }
                //ScreenRect.Inflate(20000, 20000);
                Touch.ScreenSize = Controls.ScreenSize = ScreenSize;
                Touch.WindowSize = 
                Controls.WindowSize = Graphics.WindowSize;
                
                Effects.Update();
            }
            Controls.update();
            Touch.update();
            VirtualJoystick.Update();
#if DEBUG
            if (LevelBuilder.Active)
                LevelBuilder.Update();
#endif
            
        }
        public static void SetSpeed(float target, float speed, float initialSpeed = float.NaN)
        {
            if (!float.IsNaN(initialSpeed))
            {
                gamespeed = initialSpeed;
            }
            speedtarget = target;
            speedspeed = speed;
        }
        public static void SetPropertiesFromString(object obj, string prop)
        {
            prop = prop.Replace(" ", "");
            string[] props = prop.Split(new char[] {';'}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var p in props)
            {
                string[] ps = p.Split('=');
                if (ps.Length > 1)
                {
                    string name = ps[0];
                    string[] names = ps[0].Split('.');
                    var info = GameExtensions.GetFieldInfo(name, obj);
                    string value = ps[1];
                    try
                    {
                        object val = GetPropertyValueFromString(value);
                        if (val == null)
                        {
                            val = StageObjectPropertyConverter.GetValue(info.FieldInfo.Type(), value);
                        }
                        if (val != null)
                        {
                            if (val.GetType() == info.FieldInfo.Type())
                            {
                                info.FieldInfo.SetValue(info.Object, val);
                                info.BubbleBackValues();
                                info = info;
                            }
                        }
                    }
                    catch
                    {

                    }
                }
            }
        }
        public static void SetStaticPropertiesFromString(string typeName, string prop)
        {
            prop = prop.Replace(" ", "");
            string[] props = prop.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var p in props)
            {
                string[] ps = p.Split('=');
                if (ps.Length > 1)
                {
                    //string name = ps[0];
                    //string[] names = ps[0].Split('.');
                    var info = GameExtensions.GetStaticFieldInfo(typeName + "." + ps[0]);
                    string value = ps[1];
                    try
                    {
                        object val = GetPropertyValueFromString(value);
                        if (val == null)
                        {
                            val = StageObjectPropertyConverter.GetValue(info.FieldInfo.Type(), value);
                        }
                        if (val != null)
                        {
                            if (val.GetType() == info.FieldInfo.Type())
                            {
                                info.FieldInfo.SetValue(info.Object, val);
                                info.BubbleBackValues();
                            }
                        }
                    }
                    catch
                    {

                    }
                }
            }
        }
        public static object GetPropertyValueFromString(string prop)
        {
            prop = prop.Replace(" ", "");
            if (prop[0] == '(' && prop.Contains(')'))
            {
                string type = "";
                for (int i = 1; i < prop.Length; i++)
                {
                    if (prop[i] != ')')
                        type += prop[i];
                    else break;
                }
                type = type.ToLowerInvariant();
                int l = prop.IndexOf(')') + 1;
                prop = prop.Substring(l, prop.Length - l);
                if (type == "static")
                {
                    if (prop.Contains('.'))
                    {
                        /*string className = prop.Substring(0, prop.IndexOf('.'));
                        l = prop.IndexOf('.') + 1;
                        prop = prop.Substring(l, prop.Length - l);*/
                        var info = GameExtensions.GetStaticFieldInfo(prop);
                        if (info.FieldInfo != null && info.Object != null)
                        {
                            return info.FieldInfo.GetValue(info.Object);
                        }
                    }
                }
                else if (type == "obj" || type == "object")
                {
                    if (prop.Contains('.'))
                    {
                        string objName = prop.Substring(0, prop.IndexOf('.'));
                        l = prop.IndexOf('.') + 1;
                        prop = prop.Substring(l, prop.Length - l);
                        var obj = Global.Game.FindObjectByName(objName);
                        if (obj != null)
                        {
                            var info = GameExtensions.GetFieldInfo(prop, obj);
                            if (info.FieldInfo != null)
                            {
                                return info.FieldInfo.GetValue(info.Object);
                            }
                        }
                    }
                    else
                    {
                        return Game.FindObjectByName(prop);
                    }
                }
            }
            return null;
        }
        
    }
    public static class Textures
    {
        public static TextureFrame HealthIcon;
        public static TextureFrame TimeIcon;
        public static TextureFrame SmallLight;
        public static TextureFrame TextLine;
        public static TextureFrame WhiteBlock;
        public static TextureFrame Light, Light2;
        public static void init()
        {
            load();
        }
        private static void load()
        {
            HealthIcon = Global.Game.LoadTex("healthIcon");
            TimeIcon = Global.Game.LoadTex("timeIcon");
            SmallLight = l("SmallLight");
            TextLine = l("TextLine");
            WhiteBlock = l("WhiteBlock");
            Light = l("Light");
            Light2 = l("Light2");
        }
        public static void Reload()
        {
            load();
        }
        static TextureFrame l(string name)
        {
            return Global.Game.LoadTex(name);
        }
        public static Color[,] GetTextureData(Texture2D tex)
        {
            var dat = new Color[tex.Width, tex.Height];
            var c = new Color[tex.Width * tex.Height];
            tex.GetData(c);
            for (int i = 0; i < c.Length; i++)
            {
                dat[i % tex.Width, i / tex.Height] = c[i];
            }
            return dat;
        }
    }
    public static class Fonts
    {
        public static SpriteFont Default;
        public static SpriteFont Cutscene;
        public static SpriteFont UIDefault;
        public static SpriteFont UITitle;
        public static void init()
        {
            Default = Global.Game.Content.Load<SpriteFont>("SpriteFont1");
            Cutscene = Global.Game.Content.Load<SpriteFont>("SpriteFont2");
            UIDefault = lf("UIDefault");
            UITitle = lf("UITitle");
        }
        static SpriteFont lf(string font)
        {
            return Global.Game.Content.Load<SpriteFont>("Fonts/" + font);
        }
    }
}
