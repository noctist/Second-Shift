using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using SecondShiftMobile.Test;
using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace SecondShiftMobile
{
    public static class LevelBuilder
    {
        public static bool ShowBoundingBoxes = false;
        static bool moving = false;
        static bool active;
        static int scroll;
        static float z = 0;
        static int index = 0;
        static Stopwatch watch;
        public static float ObjZ = 0;
        static Obj selObj = null;
        public static Obj SelectedObj
        {
            get
            {
                return selObj;
            }
            private set
            {
                if (selObj != value)
                {
                    if (EditMode)
                    {
                        foreach (var sd in level.StageObjectData)
                        {
                            if (value != null && sd.Obj == value)
                            {
#if PC
                                BuildForm.SetSelectedObjProperties(sd);
#endif
                                break;
                            }
                        }
                        selObj = value;
                    }
                }
            }
        }
        static bool editMode = false;
        static Level level;
        public static String LastName = "";
        static bool changed = false;
        public static bool Changed
        {
            get
            {
                return changed;
            }
            set
            {
                if (value != changed)
                {
                    changed = value;
#if PC
                    if (BuildForm != null && !BuildForm.IsDisposed)
                    {
                        BuildForm.ChangedChanged(value);
                    }
#endif
                }
            }
        }
        public static bool EditMode
        {
            get
            {
                return editMode;
            }
            set
            {
                if (editMode != value)
                {
                    if (value)
                    {
                        if (obj != null)
                        {
                            Global.Game.RemoveObj(obj);
                            obj = null;
                        }
                    }
                    else
                    {

                    }

                    editMode = value;
#if PC
                    if (BuildForm != null && !BuildForm.IsDisposed)
                        BuildForm.EditModeChanged();
#endif
                }
            }
        }
        public static bool Active
        {
            get { return active; }
            set 
            {
#if PC
                if (active != value)
                {
                    if (value)
                    {
                        scroll = Controls.ScrollWheel;
                        pos = Global.Camera.View;
                        z = pos.Z;
                        if (obj != null && Global.Game.objArray.Contains(obj))
                        {
                            Global.Game.AddObj(obj);
                        }
                        if (BuildForm.IsDisposed)
                            BuildForm = new BuildForm();
                        BuildForm.Show();
                        if (level != null)
                        {
                            BuildForm.SetSkyColor(level.SkyColor);
                            BuildForm.SetContrast(level.Contrast);
                        }
                        else
                        {
                            BuildForm.SetSkyColor(Global.Game.SkyColor);
                        }
                    }
                    else
                    {
                        if (obj != null)
                            Global.Game.RemoveObj(obj);
                        obj = null;
                        if (!BuildForm.IsDisposed)
                            BuildForm.Hide();
                    }
                }
#endif
                active = value;
            }
        }
#if PC
        public static BuildForm BuildForm;
        static System.Windows.Forms.Form window;
#endif
        public static List<Type> Types = new List<Type>();
        static Obj obj;
        static Vector3 pos = Vector3.Zero;
        static Vector3 speed = Vector3.Zero, speedT = Vector3.Zero;
        //static List<Obj> objects = new List<Obj>();
        public static Vector3 Pos
        {
            get { return pos; }
        }
        public static void init()
        {
            watch = new Stopwatch();
            watch.Start();
            level = new Level();
            Types.Add(typeof(Players.Carrot));
            Types.Add(typeof(Environments.City.Building1));
            Types.Add(typeof(Environments.City.Building2));
            Types.Add(typeof(Environments.City.Building3));
            Types.Add(typeof(Environments.City.Building4));
            Types.Add(typeof(Test.InvisibleFloor));
            Types.Add(typeof(Environments.City.Rail));
            Types.Add(typeof(Environments.City.Train));
            Types.Add(typeof(TestFloor));
            Types.Add(typeof(Grey));
            Types.Add(typeof(MovingFloor));
            Types.Add(typeof(SmokeEmitter));
            Types.Add(typeof(LargeGrass));
            Types.Add(typeof(Mountain));
            Types.Add(typeof(Sun));
            Types.Add(typeof(TestPropellor));
            Types.Add(typeof(TestVortex));
            Types.Add(typeof(Enemies.Guard));
            Types.Add(typeof(Moon));
            Types.Add(typeof(LightPole));
            Types.Add(typeof(Test.LogoBloom));
            Types.Add(typeof(Players.Braveheart));
            Types.Add(typeof(Environments.FoliageEmitter));
            Types.Add(typeof(Test.TestPropellors));
            Types.Add(typeof(Test.LaunchRocket));
            Types.Add(typeof(Test.Thing));
            Types.Add(typeof(Players.Batman));
            Types.Add(typeof(Enemies.Joker));
            Types.Add(typeof(Environments.Snow.SnowMountain));
            Types.Add(typeof(Environments.Grass.Tree));
            Types.Add(typeof(Environments.Grass.TreeFlatBottom));
            Types.Add(typeof(Environments.Grass.Windmill));
            Types.Add(typeof(Enemies.HomingMissle));
            Types.Add(typeof(Test.Ball));
            Types.Add(typeof(Test.TestPolygonFloor));
            Types.Add(typeof(Test.FlatFloor));
            Types.Add(typeof(Test.TestGrass));
#if PC
            window = (System.Windows.Forms.Form)System.Windows.Forms.Form.FromHandle(Global.Game.Window.Handle);
            window.GotFocus += new EventHandler(window_GotFocus);
            Types = Types.OrderBy((t) => t.Name).ToList();
            BuildForm = new BuildForm();
#endif
        }
        static void window_GotFocus(object sender, EventArgs e)
        {
            watch.Restart();
        }
        public static void ChangeIndex(int i)
        {
            if (i != index)
            {
                index = i;
                if (obj != null)
                {
                    Global.Game.RemoveObj(obj);
                }
                if (!EditMode)
                    obj = CreateObj(Pos);
            }
        }
        public static void Update()
        {
            float sp = 10;
            
            #region Movement
#if PC
            if (Controls.GetKey(Keys.LeftShift) == ControlState.Held)
                sp = 40;
            if (Controls.GetKey(Keys.W) == ControlState.Held && window.Focused)
            {
                speedT.Y = -sp;
            }
            else if (Controls.GetKey(Keys.S) == ControlState.Held && window.Focused)
            {
                speedT.Y = sp;
            }
            else
            {
                speedT.Y = 0;
            }

            if (Controls.GetKey(Keys.A) == ControlState.Held && window.Focused)
            {
                speedT.X = -sp;
            }
            else if (Controls.GetKey(Keys.D) == ControlState.Held && window.Focused)
            {
                speedT.X = sp;
            }
            else
            {
                speedT.X = 0;
            }
            /*if (Controls.GetKey(Keys.F) == ControlState.Held)
            {
                speedT.Z = -sp;
            }
            else if (Controls.GetKey(Keys.R) == ControlState.Held)
            {
                speedT.Z = sp;
            }
            else
            {
                speedT.Z = 0;
            }*/
#endif
            speed += (speedT - speed) * 0.1f * Global.FrameSpeed;
            pos += speed;
#if PC
            pos.Z += ((z + ((Controls.ScrollWheel - scroll) * 1)) - pos.Z) * 0.1f * Global.FrameSpeed;
#endif
            #endregion
            #region Position Object
#if PC
            if (window.Focused)
            {
                if (Controls.GetKey(Keys.B) == ControlState.Pressed)
                    ShowBoundingBoxes = !ShowBoundingBoxes;
                if (Controls.GetKey(Keys.Z) == ControlState.Pressed)
                    ObjZ += (Controls.GetKey(Keys.LeftShift) == ControlState.Held) ? 2000 : 250;
                if (Controls.GetKey(Keys.X) == ControlState.Pressed)
                    ObjZ -= (Controls.GetKey(Keys.LeftShift) == ControlState.Held) ? 2000 : 250;
            }
            Obj moveObj = null;
            if (EditMode)
            {
                if (selObj != null && moving)
                    moveObj = selObj;
            }
            else
            {
                if (obj != null)
                    moveObj = obj;
            }
            if (moveObj != null)
            {
                if (window.Focused && watch.Elapsed.TotalSeconds > 0.5)
                {
                    moveObj.SetScreenPosition(Controls.MousePos);
                    if (Controls.GetKey(Keys.LeftControl) == ControlState.None)
                    {
                        float xx = moveObj.Pos.X % moveObj.Snap.X, yy = moveObj.Pos.Y % moveObj.Snap.Y;
                        if (xx < moveObj.Snap.X / 2)
                        {
                            moveObj.Pos.X -= xx;
                        }
                        else
                        {
                            moveObj.Pos.X += moveObj.Snap.X - xx;
                        }
                        if (yy < moveObj.Snap.Y / 2)
                        {
                            moveObj.Pos.Y -= yy;
                        }
                        else
                        {
                            moveObj.Pos.Y += moveObj.Snap.Y - yy;
                        }
                    }
                    moveObj.Pos.Z = ObjZ;
                    if (Controls.MouseLeft == ControlState.Pressed && !EditMode && !moving && moveObj == obj)
                    {
                        level.Add(moveObj);
                        obj = CreateObj(moveObj.Pos);
                        Changed = true;
                    }
                }
                else if (obj != null)
                {
                    obj.SetScreenPosition(Global.ScreenSize / 2);
                }
            }
#endif
            #endregion
#if PC
            if (BuildForm.IsDisposed)
            {
                Active = false;
                return;
            }
            
            if (Controls.GetKey(Keys.E) == ControlState.Pressed)
            {
                if (window.Focused)
                {
                    EditMode = !editMode;
                }
            }
            if (Controls.GetKey(Keys.Delete) == ControlState.Pressed)
            {
                if (EditMode && SelectedObj != null)
                {
                    level.Remove(selObj);
                    Global.Game.RemoveObj(selObj);
                    Changed = true;
                }
            }
            if (Controls.MouseRight == ControlState.Held && selObj != null && EditMode && window.Focused)
            {
                moving = true;
                ObjZ = selObj.Pos.Z;
            }
            else
            {
                if (moving)
                {
                    moving = false;
                    if (selObj != null)
                    {
                        var d = level.GetData(selObj);
                        d.Pos = selObj.Pos;
                        Changed = true;
                    }
                }
            }
            if (/*Controls.MouseRight == ControlState.Pressed ||*/ Controls.MouseLeft == ControlState.Released && window.Focused)
            {
                var p = (Controls.MousePos * (Global.Camera.CameraSize / Controls.ScreenSize)) + Global.Camera.View.ToVector2() - (Global.Camera.CameraSize / 2);
                float zz = 10000000000;
                Obj sel = null;
                var objs = level.StageObjectData;
                for (int i = 0; i < objs.Count; i++)
                {
                    Obj o = objs[i].Obj;
                    if (o != null && level.Contains(o) && o.IsInsideScreenRect(p) && o != selObj)
                    {
                        if (o is Cloud)
                            continue;
                        if (sel == null)
                        {
                            sel = o;
                        }
                        else if ((sel.ScreenRect.Contains(o.ScreenRect) || o.ScreenRect.Intersects(sel.ScreenRect)) && (o.ScreenRect.Center.ToVector2() - p).Length() * 6 < (sel.ScreenRect.Center.ToVector2() - p).Length())
                        {
                            sel = o;
                        }
                        /*else if (o.ScreenRect.Intersects(sel.ScreenRect) && (o.ScreenRect.Center.ToVector2() - p).Length() < (sel.ScreenRect.Center.ToVector2() - p).Length())
                        {
                            sel = o;
                        }*/
                        else if (o.Pos.Z < sel.Pos.Z)
                        {
                            sel = o;
                        }
                        else if (o.Depth < sel.Depth)
                        {
                            sel = o;
                        }
                    }
                }
                if (sel != null && !moving)
                {
                    if (Controls.MouseLeft == ControlState.Released && EditMode)
                    {
                        SelectedObj = sel;
                        //Changed = true;
                    }
                    else if (Controls.MouseRight == ControlState.Pressed)
                    {
                        level.Remove(sel);
                        sel.Remove();
                        Changed = true;
                    }
                }
            }
#endif
            if (obj != null)
            {

            }
            else if (!EditMode)
            {
                obj = CreateObj(new Vector3(Pos.X, Pos.Y, ObjZ));
            }
        }
        public static string ReturnDebugString()
        {
            string s = "";
            if (Active)
            {
                if (EditMode)
                {
                    if (selObj != null)
                    {
                        s += selObj.GetType().ToString().Split('.').Last() + ": " + selObj.Pos.X + ", " + selObj.Pos.Y + ", " + selObj.Pos.Z;
#if PC
                        s += "\nMousePos = " + Controls.MousePos;
#endif
                    }
                    else
                    {
                        s = "No object is selected";
                    }
                }
                else
                {
                    if (obj != null)
                    {
                        s += obj.GetType().ToString().Split('.').Last() + ": " + obj.Pos.X + ", " + obj.Pos.Y + ", " + obj.Pos.Z;
                    }
                    else
                    {
                        s = "No object is selected";
                    }
                }
            }
            else
            {
                s = "The level builder is inactive";
            }
            return s;
        }
        public static void Save(string name)
        {
            /*foreach (var o in objects)
            {
                var d = new StageObjectData();
                d.SetObject(o);
                level.AddObject(d);
            }*/
            level.Save("Content/Levels/", name + ".xml");
            changed = false;
        }
        public static Level Load(string name)
        {
            string folder = "Content/Levels/";
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            if (File.Exists(folder + name + ".xml"))
            {
                var sr = new StreamReader(folder + name + ".xml");
                string s = sr.ReadToEnd();
                sr.Dispose();
                XElement x = XElement.Parse(s);

                return new Level(x) { FileName = name };
            }
            else 
            {
                return new Level() { FileName = name };
            }
            
        }
        public static void CreateLevel(Level l)
        {
            LastName = l.FileName;
            l.StageObjectData.Clear();
            level = l;
            var od = l.ObjectData;
            for (int i = 0; i < od.Length; i++)
            {
                var sd = od[i];
                var obj = sd.Create();
                obj.SetStageProperties(sd.Properties);
                sd.SetObject(obj);
                if (sd.CameraTarget)
                    Global.Camera.Target = sd.Obj;
                l.Add(sd);
            }
            Global.Game.SkyColor = level.SkyColor;
            Global.Effects.Contrast = level.Contrast;
            changed = false;
        }
        static Obj CreateObj(Vector3 pos)
        {
            return (Obj)Activator.CreateInstance(Types[index], Global.Game, pos.X, pos.Y, pos.Z);
        }
        public static void SetCameraTarget(Obj o, bool value)
        {
            foreach (var sd in level.StageObjectData)
            {
                if (sd.Obj == o && value && !sd.CameraTarget)
                {
                    sd.CameraTarget = true;
                    Changed = true;
                }
                else if (sd.CameraTarget)
                {
                    sd.CameraTarget = false;

                    Changed = true;
                }
            }
        }
        public static void SetName(Obj o, string name)
        {
            foreach (var sd in level.StageObjectData)
            {
                if (sd.Obj == o && sd.Name != name)
                {
                    sd.Name = name;
                    o.Name = name;
                    Changed = true;
                }
            }
        }
        public static void SetSkyColor(Color color)
        {
            Global.Game.SkyColor = color;
            level.SkyColor = color;
            Changed = true;
        }
        public static void SetContrast(Vector4 color)
        {
            Global.Effects.Contrast = color;
            level.Contrast = color;
            Changed = true;
        }
        public static void CreateClouds(int numberOfClouds, float left, float right, float top, float bottom, float front, float back, float minScale, float maxScale, float alpha, Color col)
        {
            foreach (var s in level.StageObjectData)
            {
                if (s.Type == typeof(Cloud))
                {
                    if (s.Obj != null)
                        s.Obj.Remove();
                }
            }
            for (int i = 0; i < level.StageObjectData.Count; i++)
            {
                var s = level.StageObjectData[i];
                if (s.Type == typeof(Cloud))
                {
                    level.StageObjectData.RemoveAt(i);
                    i--;
                }
            }
            var clouds = Cloud.CreateCloudBox(numberOfClouds, left, right, top, bottom, front, back, minScale, maxScale, true, alpha, col);
            foreach (var cloud in clouds)
            {
                level.Add(cloud);
            }
            Changed = true;
        }
        public static void SetProperties(StageObjectData dat, Obj o)
        {
            dat.SetProperties(o);
            Changed = true;
        }
        
    }
    public class StageObjectData
    {
        XElement xml;
        public XElement XML
        {
            get { return xml; }
        }
        public StageObjectData(XElement xml)
        {
            this.xml = xml;
        }
        public string Name
        {
            get
            {
                return xml.GetAttribute("Name").Value;
            }
            set
            {
                xml.GetAttribute("Name").Value = value;
            }
        }
        public float X
        {
            get
            {
                return xml.GetAttributeInt("X");
            }
            set
            {
                xml.GetAttribute("X").Value = ((int)value).ToString();
            }
        }
        public float Y
        {
            get
            {
                return xml.GetAttributeInt("Y");
            }
            set
            {
                xml.GetAttribute("Y").Value = ((int)value).ToString();
            }
        }
        public float Z
        {
            get
            {
                return xml.GetAttributeInt("Z");
            }
            set
            {
                xml.GetAttribute("Z").Value = ((int)value).ToString();
            }
        }
        public Vector3 Pos
        {
            get
            {
                return new Vector3(X, Y, Z);
            }
            set
            {
                X = value.X;
                Y = value.Y;
                Z = value.Z;
            }
        }
        public Type Type
        {
            get
            {
                string t = xml.GetAttribute("Type").Value;
                Type type;
                try
                {
                    type = Type.GetType(t);
                    return type;
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                xml.GetAttribute("Type").Value = value.ToString();
            }
        }
        public bool CameraTarget
        {
            get
            {
                return xml.GetBool("CameraTarget");
            }
            set
            {
                xml.SetBool("CameraTarget", value);
            }
        }
        public static void Create(Type type)
        {

        }
        public StageObjectData()
        {
            xml = new XElement("Object");
        }
        public StageObjectProperty[] Properties
        {
            get
            {
                XElement[] xe = xml.Elements("Property").ToArray();
                StageObjectProperty[] sod = new StageObjectProperty[xe.Length];
                for (int i = 0; i < xe.Length; i++)
                {
                    sod[i] = new StageObjectProperty(xe[i]);
                }
                return sod;
            }
        }
        Obj obj = null;
        public Obj Obj
        {
            get { return obj; }
        }
        public StageObjectData(Obj o)
        {
            this.xml = new XElement("Object");
            SetObject(o);
        }
        public void SetObject(Obj o)
        {
            obj = o;
            Pos = o.Pos;
            Name = o.Name;
            Type = o.GetType();
            SetProperties(o);
            var s = "";
        }
        public void SetProperties(Obj o)
        {
            var sop = o.GetStageProperties();
            if (sop != null)
            {
                foreach (var s in Properties)
                    s.XML.Remove();
                foreach (var s in sop)
                {
                    xml.Add(s.XML);
                }
            }
        }
        public Obj Create()
        {
            if (Type == typeof(Cloud))
            {
                var s = "";
            }
            var obj = (Obj)Activator.CreateInstance(Type, Global.Game, X, Y, Z);
            obj.Name = Name;
            return obj;
        }

    }
    public class StageObjectProperty
    {
        XElement xml;
        public XElement XML
        {
            get { return xml; }
        }
        public string Name
        {
            get
            {
                return xml.GetAttribute("Name").Value;
            }
            set
            {
                xml.GetAttribute("Name").Value = value;
            }
        }
        
        public string Info
        {
            get
            {
                return xml.GetAttribute("Info").Value;
            }
            set
            {
                xml.GetAttribute("Info").Value = value;
            }
        }
        public object Value
        {
            get
            {
                return XML.Value;
            }
            set
            {
                SetValue(value);
            }
        }
        public StageObjectProperty()
        {
            xml = new XElement("Property");
        }
        public StageObjectProperty(XElement Xml)
        {
            xml = Xml;
        }
        public void SetValue(object value)
        {
            xml.Value = StageObjectPropertyConverter.SetValue(value);
        }
        
        public float GetFloat()
        {
            return StageObjectPropertyConverter.GetFloat(xml.Value);
        }
        public Vector2 GetVector2()
        {
            return StageObjectPropertyConverter.GetVector2(xml.Value);
        }
        public Vector3 GetVector3()
        {
            return StageObjectPropertyConverter.GetVector3(xml.Value);
        }
        public Color GetColor()
        {
            return StageObjectPropertyConverter.GetColor(xml.Value);
        }
    }
    public static class StageObjectPropertyConverter
    {
        public static string SetValue(object value)
        {
            string s;
            if (value is Vector3)
            {
                var v = (Vector3)value;
                s = v.X + ", " + v.Y + ", " + v.Z;
            }
            else if (value is Vector4)
            {
                var v = (Vector4)value;
                s = v.X + ", " + v.Y + ", " + v.Z + ", " + v.W;
            }
            else if (value is Vector2)
            {
                var v = (Vector2)value;
                s = v.X + ", " + v.Y;
            }
            else if (value is float)
            {
                s = value.ToString();
            }
            else if (value is int)
            {
                s = value.ToString();
            }
            else if (value is Color)
            {
                Color c = (Color)value;
                s = c.R + ", " + c.G + ", " + c.B + ", " + c.A;
            }
            else if (value is Texture2D)
            {
                s = ((Texture2D)value).Name;
            }
            else if (value is bool)
            {
                s = (bool)value ? "true" : "false";
            }

            else
            {
                s = value.ToString();
            }
            return s;
        }
        public static Object GetValue(Type type, string val)
        {
            if (type == typeof(Vector3))
            {
                return GetVector3(val);
            }
            else if (type == typeof(Vector2))
            {
                return GetVector2(val);
            }
            else if (type == typeof(float))
            {
                return GetFloat(val);
            }
            else if (type == typeof(Color))
            {
                return GetColor(val);
            }
            else if (type == typeof(Vector4))
            {
                return GetVector4(val);
            }
            else if (type == typeof(Texture2D))
            {
                return GetTexture2D(val);
            }
            else if (type == typeof(bool))
            {
                return GetBool(val);
            }
            else if (type == typeof(int))
            {
                return GetInt(val);
            }
            else if (type == typeof(string))
            {
                return val;
            }
            else
            {
                return null;
            }
        }
        public static float GetFloat(string val)
        {
            float f;
            if (float.TryParse(val, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out f))
            {
                return f;
            }
            else return 0;
        }
        public static int GetInt(string val)
        {
            int i;
            if (int.TryParse(val, out i))
            {
                return i;
            }
            else return 0;
        }
        public static Vector2 GetVector2(string val)
        {
            string[] vals = val.Split(',');
            Vector2 v = Vector2.Zero;
            for (int i = 0; i < vals.Length; i++)
            {
                float f;
                if (float.TryParse(vals[i], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out f))
                {
                    if (i == 0)
                        v.X = f;
                    else if (i == 1)
                        v.Y = f;
                }
            }
            return v;
        }
        public static Texture2D GetTexture2D(string val)
        {
            try 
            {
                return Global.Game.LoadTex(val);
            }
            catch 
            {
                return null;
            }
        }
        public static Vector3 GetVector3(string val)
        {
            string[] vals = val.Split(',');
            Vector3 v = Vector3.Zero;
            for (int i = 0; i < vals.Length; i++)
            {
                float f;
                if (float.TryParse(vals[i], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out f))
                {
                    if (i == 0)
                        v.X = f;
                    else if (i == 1)
                        v.Y = f;
                    else if (i == 2)
                        v.Z = f;
                }
            }
            return v;
        }
        public static Vector4 GetVector4(string val)
        {
            string[] vals = val.Split(',');
            Vector4 v = Vector4.Zero;
            for (int i = 0; i < vals.Length; i++)
            {
                float f;
                if (float.TryParse(vals[i], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out f))
                {
                    if (i == 0)
                        v.X = f;
                    else if (i == 1)
                        v.Y = f;
                    else if (i == 2)
                        v.Z = f;
                    else if (i == 3)
                        v.W = f;
                }
            }
            return v;
        }
        public static Color GetColor(string val)
        {
            string[] vals = val.Split(',');
            Color c = new Color(0, 0, 0, 255);
            for (int i = 0; i < vals.Length; i++)
            {
                byte f;
                if (byte.TryParse(vals[i], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out f))
                {
                    if (i == 0)
                        c.R = f;
                    else if (i == 1)
                        c.G = f;
                    else if (i == 2)
                        c.B = f;
                    else if (i == 3)
                        c.A = f;
                }
            }
            return c;
        }
        public static bool GetBool(string val)
        {
            return (val.Contains("true") || val.Contains("True"));
        }
        
    }
    public class Level
    {
        XElement xml;
        public string FileName = "Level String Test";
        public XElement XML
        {
            get { return xml; }
        }
        public string Name
        {
            get
            {
                return xml.GetAttribute("Name").Value;
            }
            set
            {
                xml.GetAttribute("Name").Value = value;
            }
        }
        public Color SkyColor
        {
            get
            {
                string s = xml.GetAttribute("SkyColor").Value;
                if (s.Length > 0)
                {
                    return StageObjectPropertyConverter.GetColor(s);
                }
                else
                {
                    return Color.SkyBlue;
                }
            }
            set
            {
                xml.GetAttribute("SkyColor").Value = StageObjectPropertyConverter.SetValue(value);
            }
        }
        public Vector4 Contrast
        {
            get
            {
                string s = xml.GetAttribute("Contrast").Value;
                if (s.Length > 0)
                {
                    return StageObjectPropertyConverter.GetVector4(s);
                }
                else
                {
                    return new Vector4(1, 1, 1, 0.5f);
                }
            }
            set
            {
                xml.GetAttribute("Contrast").Value = StageObjectPropertyConverter.SetValue(value);
            }
        }
        public StageObjectData[] ObjectData
        {
            get
            {
                XElement[] xe = xml.Elements("Object").ToArray();
                StageObjectData[] sod = new StageObjectData[xe.Length];
                for (int i = 0; i < xe.Length; i++)
                {
                    sod[i] = new StageObjectData(xe[i]);
                }
                return sod;
            }
        }
        public List<StageObjectData> StageObjectData = new List<StageObjectData>();
        public Level()
        {
            xml = new XElement("Level");
        }
        public Level(XElement Xml)
        {
            xml = Xml;
        }
        public void Add(StageObjectData sod)
        {
            //xml.Add(sod.XML);
            StageObjectData.Add(sod);
        }
        public void Add(Obj o)
        {
            var sod = new StageObjectData(o);
            Add(sod);
        }
        public void Remove(Obj o)
        {
            for (int i = 0; i < StageObjectData.Count; i++)
            {
                var s = StageObjectData[i];
                if (s.Obj == o)
                {
                    StageObjectData.Remove(s);
                    return;
                }
            }
        }
        public bool Contains(Obj o)
        {
            foreach (var s in StageObjectData)
            {
                if (s.Obj == o)
                {
                    return true;
                }
            }
            return false;
        }
        public StageObjectData GetData(Obj o)
        {
            foreach (var s in StageObjectData)
            {
                if (s.Obj == o)
                {
                    return s;
                }
            }
            return null;
        }
        public void Save(string folder, string name)
        {
            foreach (var s in ObjectData)
                s.XML.Remove();
            foreach (var s in StageObjectData)
                xml.Add(s.XML);
            if (folder.Last() != '/' && folder.Last() != '\\')
                folder += '/';
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            StreamWriter sw = new StreamWriter(folder + name);
            sw.Write(xml.ToString());
            sw.Dispose();
        }
    }

}
