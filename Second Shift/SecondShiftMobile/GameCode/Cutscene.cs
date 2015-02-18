using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using System.IO;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System.Threading.Tasks;
using SecondShiftMobile.UI.Animations;

namespace SecondShiftMobile.Cutscenes
{
    public class Cutscene
    {

        bool paused = false;
        public static string LastLoadedName = "";
        public float Timer { get; protected set; }
        string dialogText = "";
        int index = 0;
        Texture2D face;
        string character = "";
        public float BlockAlpha = 0;
        public int Index 
        { 
            get
            {
                return index;
            } 
            protected set
            {
                value = (int)MathHelper.Clamp(value, 0, untimedActions.Count);
                index = value;
            } 
        }
        public CutsceneAction CurrentAction
        {
            get
            {
                if (untimedActions.Count > 0)
                {
                    return untimedActions[(int)MathHelper.Clamp(index, 0, untimedActions.Count - 1)];
                }
                else return null;
            }
        }
        XElement xml;
        public XElement XML
        {
            get
            {
                return xml;
            }
        }
        public Type Class
        {
            get
            {
                try
                {
                    var t = Type.GetType(xml.GetAttribute("Class").Value);
                    if (t == null)
                        return typeof(Cutscene);
                    else return t;
                }
                catch
                {
                    return typeof(Cutscene);
                }
            }
            set
            {
                xml.GetAttribute("Class").Value = value.ToString();
            }
        }
        float dialogAlpha = 0;
        float dialogAlphaTarget = 0;
        public bool DialogVisible
        {
            get
            {
                return dialogAlphaTarget > 0.5f;
            }
        }
#if PC && DEBUG
        BindingList<CutsceneAction> actions;
        public BindingList<CutsceneAction> Actions
#else
        List<CutsceneAction> actions;
        public List<CutsceneAction> Actions
#endif
        {
            get
            {
                return actions;
            }
        }
        List<CutsceneAction> timedActions;
        List<CutsceneAction> untimedActions;

        public Cutscene(XElement data)
        {
            Timer = 0;
            this.xml = data;
            loadData();
        }
        public Cutscene()
        {
            Timer = 0;
#if PC && DEBUG
            actions = new BindingList<CutsceneAction>();
#else
            actions = new List<CutsceneAction>();
#endif
            xml = new XElement("Cutscene");
            actions.Add(new SetCameraDelta());
            actions.Add(new SetMusicVolume() { VolumeType = Test.VolumeType.MediaPlayer });
            actions.Add(new SetMusicVolume() { VolumeType = Test.VolumeType.SoundEffect });
        }
        void loadData()
        {
            if (actions == null)
#if PC && DEBUG
                actions = new BindingList<CutsceneAction>();
#else
                actions = new List<CutsceneAction>();
#endif
            if (untimedActions == null)
                untimedActions = new List<CutsceneAction>();
            if (timedActions == null)
                timedActions = new List<CutsceneAction>();
            untimedActions.Clear();
            timedActions.Clear();
            actions.Clear();
            int maxtime = 0;
            foreach (var el in xml.Elements("Action"))
            {
                CutsceneAction ca = new CutsceneAction(el);
                if (CutsceneAction.ActionTypeConverter.ContainsKey(ca.Type))
                {
                    if (ca.Time > maxtime)
                        maxtime = ca.Time;
                    var typeAndName = CutsceneAction.ActionTypeConverter[ca.Type];
                    CutsceneAction ca2 = (CutsceneAction)Activator.CreateInstance(typeAndName.Type, el);
                    if (ca2.Time <= 0)
                    {
                        untimedActions.Add(ca2);
                        ca2.WaitTime = maxtime;
                    }
                    else
                    {
                        timedActions.Add(ca2);
                    }
                    actions.Add(ca2);
                }
            }
        }
        public static Cutscene Load(string name)
        {
            string folder = "Content/Cutscenes/";
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
                LastLoadedName = name;
                Type t;
                try
                {
                    t = Type.GetType(x.GetAttribute("Class").Value);
                }
                catch
                {
                    t = typeof(Cutscene);
                }
                if (t == null)
                    t = typeof(Cutscene);
                var cut = Activator.CreateInstance(t, x);
                return (Cutscene)cut;
            }
            else
            {
                return new Cutscene() {  };
            }
        }
        public static Cutscene CreateCutscene(XElement x)
        {
            Type t;
            try
            {
                t = Type.GetType(x.GetAttribute("Class").Value);
            }
            catch
            {
                t = typeof(Cutscene);
            }
            if (t == null)
                t = typeof(Cutscene);
            var cut = Activator.CreateInstance(t, x);
            return (Cutscene)cut;
        }
        public static void Save(Cutscene cut, string name)
        {
            string folder = "Content/Cutscenes/";
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            if (File.Exists(folder + name + ".xml"))
            {
                File.Delete(folder + name + ".xml");
            }
            StreamWriter sw = new StreamWriter(folder + name + ".xml");
            sw.Write(cut.GetXML().ToString());
            sw.Dispose();
        }
        public XElement GetXML()
        {
            XElement x = new XElement("Cutscene");
            x.GetAttribute("Class").Value = Class.ToString();
            foreach (var act in actions)
            {
                x.Add(act.XML);
            }
            return x;
        }
        public void SetDialog(Dialog dialog)
        {
            string[] words = dialog.Text.Split(' ');
            string sizeText = "";
            for (int i = 0; i < words.Length; i++)
            {
                string testText = sizeText + words[i];
                var size = Fonts.Cutscene.MeasureString(testText);
                if (size.X < Global.ScreenSize.X - 200)
                {
                    sizeText = testText + " ";
                }
                else
                {
                    sizeText = sizeText + "\n" + words[i] + " ";
                }
            }
            
            dialogText = sizeText;
            character = dialog.Character;
            Texture2D tex;
            if (File.Exists("Content/Cutscenes/Faces/" + dialog.Face + ".xnb"))
            {
                face = Global.Game.LoadTex("Cutscenes/Faces/" + dialog.Face);
            }
            else if (File.Exists("Content/Cutscenes/Faces/" + dialog.Character + ".xnb"))
            {
                face = Global.Game.LoadTex("Cutscenes/Faces/" + dialog.Character);

            }
            else
            {
                face = Global.Game.LoadTex("Cutscenes/Faces/QuestionMark");
            }
        }
        public virtual void Update()
        {
            dialogAlpha += (dialogAlphaTarget - dialogAlpha) * 0.1f;
            if (!paused)
                Timer+= Global.FrameSpeed;
            if (CurrentAction != null && Timer >= CurrentAction.WaitTime)
            {
                if (!CurrentAction.Selected)
                {
                    CurrentAction.Select(this);
                }
                if (CurrentAction.AutoExecute)
                {
                    if (!CurrentAction.Executed && CurrentAction.Execute(this))
                    {
                        ActionExecuted(CurrentAction);
                        Index++;
                    }
                }
                else if (!CurrentAction.Executed)
                {
#if PC
                    if (Controls.GetKey(Microsoft.Xna.Framework.Input.Keys.Z) == ControlState.Pressed || Controls.GetButton(Microsoft.Xna.Framework.Input.Buttons.A) == ControlState.Pressed)
                    {
                        if (CurrentAction.Execute(this))
                        {
                            ActionExecuted(CurrentAction);
                            index++;
                        }
                    }
#elif MONO
                    for (int i = 0; i < 4; i++)
                    {
                        if (Touch.States[i] == TouchScreen.Released && Touch.TouchTimers[i] < 15)
                        {
                            if (CurrentAction.Execute(this))
                            {
                                ActionExecuted(CurrentAction);
                                index++;
                            }
                            break;
                        }
                    }
#endif

                }
            }
            foreach (var act in timedActions)
            {
                if (!act.Executed && Timer >= act.Time)
                {
                    if (!act.Selected)
                    {
                        act.Select(this);
                    }
                    if (act.Execute(this))
                    {
                        ActionExecuted(act);
                    }
                }
            }
        }
        public virtual void ChangeTimerState(bool pause)
        {
            paused = pause;
        }
        public void SetDialogVisibility(bool visible)
        {
            if (visible)
                dialogAlphaTarget = 1;
            else dialogAlphaTarget = 0;
        }
        public virtual void ActionExecuted(CutsceneAction action)
        {

        }

        public virtual void Draw(SpriteBatch spriteBatch, Matrix defaultMatrix)
        {

            Global.Effects.Technique = Techniques.PlainNormal;
            foreach (var pass in Global.Effects.CurrentTechnique.Passes)
            {
                pass.Apply();
                #if DEBUG
                string time = "Cutscene timer: " + Timer;
                var vec = Fonts.Default.MeasureString(time);
                spriteBatch.DrawString(Fonts.Default, time, new Vector2(Global.ScreenSize.X - vec.X - 12, 0), Color.Black);
                #endif
                spriteBatch.Draw(Textures.WhiteBlock, new Rectangle(0, 0, (int)Global.ScreenSize.X, (int)Global.ScreenSize.Y), Color.Black * BlockAlpha);
                if (dialogAlpha > 0.01f)
                {
                    float textLineScale = Global.ScreenSize.X / Textures.TextLine.Width;
                    spriteBatch.Draw(Textures.TextLine, new Vector2(0, Global.ScreenSize.Y - (Textures.TextLine.Height * textLineScale)), null, Color.Black * dialogAlpha * 0.5f, 0, Vector2.Zero, textLineScale, SpriteEffects.None, 0);
                    
                    //spriteBatch.DrawString(Fonts.Cutscene, dialogText, new Vector2(200, Global.ScreenSize.Y - 144), Color.Black * dialogAlpha);
                    spriteBatch.DrawString(Fonts.Cutscene, dialogText, new Vector2(200, Global.ScreenSize.Y - 96), Color.White * dialogAlpha, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                    if (face != null)
                    {
                        //spriteBatch.Draw(face, new Vector2(0, Global.ScreenSize.Y - face.Height), Color.White * dialogAlpha);
                        int h = (int)(128f * ((float)face.Height / (float)face.Height));
                        spriteBatch.Draw(face, new Rectangle(0, (int)Global.ScreenSize.Y - h, 128, h), Color.White * dialogAlpha);
                    }
                    //spriteBatch.DrawString(Fonts.Cutscene, character, new Vector2(24, Global.ScreenSize.Y - 48), Color.White * dialogAlpha);
                    //spriteBatch.DrawString(Fonts.Cutscene, character, new Vector2(24, Global.ScreenSize.Y - 48), Color.Black * dialogAlpha);
                }
            }
            for (int i = 0; i < timedActions.Count; i++)
            {
                if (timedActions[i].Time <= Timer)
                {
                    timedActions[i].Execute(this);
                }
                if (timedActions[i].Executed)
                {
                    timedActions.RemoveAt(i);
                    i--;
                }
            }

        }
        
    }

    #region Cutscene Actions
    public class CutsceneAction : INotifyPropertyChanged
    {
        public struct TypeAndName
        {
            public string Name;
            public Type Type;
            public static implicit operator Type(TypeAndName t)
            {
                return t.Type;
            }
            public override string ToString()
            {
                return Name ?? ((Type == null) ? "TypeAndName" : Type.Name);
            }
        }
        static TypeAndName ts(Type type, string name = null)
        {
            return new TypeAndName() { Type = type, Name = name ?? type.Name };
        }
        public static Dictionary<ActionType, TypeAndName> ActionTypeConverter = new Dictionary<ActionType, TypeAndName>
        {
            { ActionType.Dialog, ts(typeof(Dialog)) },
            { ActionType.LoadLevel, ts(typeof(LoadLevel)) },
            { ActionType.ResetCamera, ts(typeof(ResetCamera)) },
            { ActionType.SetMusicState, ts(typeof(SetMusicState)) },
            { ActionType.SetDialogVisibility, ts(typeof(SetDialogVisibility)) },
            { ActionType.ChangeTimerState, ts(typeof(ChangeTimerState)) },
            { ActionType.SetMusicVolume, ts(typeof(SetMusicVolume)) },
            { ActionType.SetGameSpeed, ts(typeof(SetGameSpeed)) },
            { ActionType.SetObjectProperty, ts(typeof(SetObjectProperty)) },
            { ActionType.Event, ts(typeof(CutsceneEvent)) },
            { ActionType.SetBlur, ts(typeof(SetBlur)) },
            { ActionType.SetBlurDepth, ts(typeof(SetBlurDepth)) },
            { ActionType.SetCameraDelta, ts(typeof(SetCameraDelta)) },
            { ActionType.SetScreenFade, ts(typeof(SetScreenFade), "Fade screen in or out") },
            { ActionType.PreloadContent, ts(typeof(PreloadContent), "Preload content") },
            { ActionType.SetLookDirection, ts(typeof(SetLookDirection), "Set look direction") },
            {ActionType.SetCameraShake, ts(typeof(SetCameraShake), "Set camera shake") }
        };
        public int WaitTime = 0;
        bool executed = false;
        bool selected = false;
        public bool Executed
        {
            get { return executed; }
        }
        public bool Selected
        {
            get { return selected; }
        }
        protected XElement xml;
        public XElement XML
        {
            get
            {
                return xml;
            }
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
                OnPropertyChanged("Name");
            }
        }
        public ActionType Type
        {
            set
            {
                xml.GetAttribute("Type").Value = value.ToString();
                
            }
            get
            {
                try
                {
                    return (ActionType)Enum.Parse(typeof(ActionType), xml.GetAttribute("Type").Value);
                }
                catch
                {
                    return ActionType.None;
                }
            }
        }
        public string DisplayText
        {
            get 
            {
                return ToString();
            }
        }
        public int Time
        {
            set
            {
                xml.GetElement("Time").Value = value.ToString();
                OnPropertyChanged("Time");
            }
            get
            {
                int i = -1;
                int.TryParse(xml.GetElement("Time").Value, out i);
                return i;
            }
        }
        public bool AutoExecute
        {
            set
            {
                xml.SetBool("AutoExecute", value);
            }
            get
            {
                return xml.GetBool("AutoExecute");
            }
        }
        public CutsceneAction(XElement action)
        {
            xml = action;
        }
        public CutsceneAction()
        {
            xml = new XElement("Action");
            AutoExecute = true;
        }
        public virtual List<ActionContract> GetContracts()
        {
            return new List<ActionContract>
            {
                new ActionContract("Name", typeof(string)),
                new ActionContract("Time", typeof(int))
            };
        }
        public virtual bool Execute(Cutscene cutscene)
        {
            executed = true;
            return true;
        }
        public override string ToString()
        {
            return "[" + Time + "]: ";
            return "[" + Time + "] " + Type+ ": ";
        }
        protected void OnPropertyChanged(string Property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(Property));
        }
        protected void OnPropertyChanged()
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs("DisplayText"));
        }
        protected void opc(string Property)
        {
            OnPropertyChanged(Property);
        }
        public virtual void Select(Cutscene cutscene)
        {
            selected = true;
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
    }
    public enum ActionType
    {
        None, ClearObjects, Event, LoadLevel, ChangeTimerState, Dialog, SetDialogClickability, SetDialogVisibility, SetMusicState, SetMusicVolume, CreateObject, RemoveObjects, SwapObject, SetObjectProperty, SetGameSpeed, SetCamera, SetBlur,
        SetBlurDepth, SetCameraDelta, SetScreenFade, PreloadContent, SetLookDirection, ResetCamera, SetCameraShake
    }
    public struct ActionContract
    {
        public string Property;
        public Type Type;
        public ActionContract(string property, Type type)
        {
            Property = property;
            Type = type;
        }
        public object GetValue(Object o)
        {
            return o.GetType().GetProperty(Property).GetValue(o, null);
        }
        public void SetValue(Object o, object value)
        {
            o.GetType().GetProperty(Property).SetValue(o, value, null);
        }
    }
    public class Dialog : CutsceneAction
    {
        public String Character
        {
            get
            {
                return xml.GetAttribute("Character").Value;
            }
            set
            {
                xml.GetAttribute("Character").Value = value;
                OnPropertyChanged("Character");
            }
        }
        public String Text
        {
            get
            {
                return xml.GetElement("Text").Value;
            }
            set
            {
                xml.GetElement("Text").Value = value;
                OnPropertyChanged("Text");
            }
        }
        public String Face
        {
            get
            {
                return xml.GetAttribute("Face").Value;
            }
            set
            {
                xml.GetAttribute("Face").Value = value;
            }
        }
        public Dialog(XElement Xml)
            :base(Xml)
        {

        }
        public Dialog()
        {
            Type = ActionType.Dialog;
            AutoExecute = false;
        }
        public override List<ActionContract> GetContracts()
        {
            var c = base.GetContracts();
            c.Add(new ActionContract("Text", typeof(string)));
            c.Add(new ActionContract("Character", typeof(string)));
            c.Add(new ActionContract("Face", typeof(string)));
            return c;
        }
        public override string ToString()
        {
            return base.ToString() + "" + ((Character.Length > 0) ? Character : "Someone") + " says \"" + Text + "\"";
        }
        public override void Select(Cutscene cutscene)
        {
            cutscene.SetDialog(this);
            base.Select(cutscene);
        }
        public override bool Execute(Cutscene cutscene)
        {
            if (cutscene.DialogVisible)
            {
                return base.Execute(cutscene);
            }
            else
            {
                return false;
            }
        }
    }
    public class LoadLevel : CutsceneAction
    {
        public string LevelName
        {
            get
            {
                return xml.GetElement("LevelName").Value;
            }
            set
            {
                xml.GetElement("LevelName").Value = value;
                OnPropertyChanged("LevelName");
            }
        }
        public bool ClearObjects
        {
            get
            {
                return xml.GetBool("ClearObjects");
            }
            set
            {
                xml.SetBool("ClearObjects", value);
                OnPropertyChanged("ClearObjects");
            }
        }
        public LoadLevel(XElement Xml)
            : base(Xml)
        {

        }
        public LoadLevel()
        {
            Type = ActionType.LoadLevel;
            ClearObjects = true;
        }
        public override List<ActionContract> GetContracts()
        {
            var cons = base.GetContracts();
            cons.Add(new ActionContract("LevelName", typeof(string)));
            cons.Add(new ActionContract("ClearObjects", typeof(bool)));
            return cons;
        }
        public override bool Execute(Cutscene cutscene)
        {
            if (LevelName.Length > 0)
            {
                var l = LevelBuilder.Load(LevelName);
                if (ClearObjects)
                    Global.Game.ClearObjects();
                LevelBuilder.CreateLevel(l);
            }

            return base.Execute(cutscene);
        }
        public override string ToString()
        {
            var s = base.ToString();
            s += (LevelName.Length > 0) ? "Load " + LevelName + ". " : "";
            s += (ClearObjects) ? "Clear objects." : "";
            return s;
        }
    }
    public enum CutsceneMusicState { DontChange, Play, Pause, Stop };
    public enum CutsceneMusicLoopState { DontChange, Loop, DontLoop };
    public class SetMusicState : CutsceneAction
    {
        public CutsceneMusicState State
        {
            get
            {
                try
                {
                    return (CutsceneMusicState)Enum.Parse(typeof(CutsceneMusicState), xml.GetElement("State").Value);
                }
                catch
                {
                    return CutsceneMusicState.DontChange;
                }
            }
            set
            {
                xml.GetElement("State").Value = value.ToString();
                OnPropertyChanged("State");
            }
        }
        public CutsceneMusicLoopState Loop
        {
            get
            {
                try
                {
                    return (CutsceneMusicLoopState)Enum.Parse(typeof(CutsceneMusicLoopState), xml.GetElement("Loop").Value);
                }
                catch
                {
                    return CutsceneMusicLoopState.DontChange;
                }
            }
            set
            {
                xml.GetElement("Loop").Value = value.ToString();
                OnPropertyChanged("Loop");
            }
        }
        public string SongName
        {
            get
            {
                return xml.GetAttribute("SongName").Value;
            }
            set
            {
                xml.GetAttribute("SongName").Value = value;
                OnPropertyChanged("SongName");
            }
        }
        public SetMusicState(XElement Xml)
            : base(Xml)
        {
            
        }
        public SetMusicState()
        {
            Type = ActionType.SetMusicState;
        }
        public override List<ActionContract> GetContracts()
        {
            var cons = base.GetContracts();
            cons.Add(new ActionContract("SongName", typeof(string)));
            cons.Add(new ActionContract("State", typeof(CutsceneMusicState)));
            cons.Add(new ActionContract("Loop", typeof(CutsceneMusicLoopState)));
            return cons;
        }
        public override string ToString()
        {
            string s = base.ToString();
            s += ((SongName.Length > 0) ? "Load " + SongName + ", " : "") + State + ", " + Loop;
            return s;
        }
        public override bool Execute(Cutscene cutscene)
        {
            Song s = null;
            if (SongName.Length > 0)
            {
                try
                {
                    s = Global.Game.LoadSong(SongName);
                    //MediaPlayer.Play(s);
                }
                catch
                {
                    
                }
            }
            switch (State)
            {
                case CutsceneMusicState.Play:
                    if (s != null)
                    {
                        MediaPlayer.Play(s);
                    }
                    else
                    {
                        MediaPlayer.Resume();
                    }
                    break;
                case CutsceneMusicState.Pause:
                    MediaPlayer.Pause();
                    break;
                case CutsceneMusicState.Stop:
                    MediaPlayer.Stop();
                    break;
            }
            switch (Loop)
            {
                case CutsceneMusicLoopState.DontLoop:
                    MediaPlayer.IsRepeating = false;
                    break;
                case CutsceneMusicLoopState.Loop:
                    MediaPlayer.IsRepeating = true;
                    break;
            }
            return base.Execute(cutscene);
        }
    }
    public class SetDialogVisibility : CutsceneAction
    {
        public bool Visible
        {
            get
            {
                return xml.GetBool("Visible");
            }
            set
            {
                xml.SetBool("Visible", value);
                OnPropertyChanged("Visible");
            }
        }
        public SetDialogVisibility(XElement Xml)
            : base(Xml)
        {

        }
        public SetDialogVisibility()
        {
            Visible = true;
            Type = ActionType.SetDialogVisibility;
        }
        public override List<ActionContract> GetContracts()
        {
            var cons = base.GetContracts();
            cons.Add(new ActionContract("Visible", typeof(bool)));
            return cons;
        }
        public override string ToString()
        {
            return base.ToString() + "Dialog is made " + ((Visible) ? "visible" : "invisible");
        }
        public override bool Execute(Cutscene cutscene)
        {
            cutscene.SetDialogVisibility(Visible);
            return base.Execute(cutscene);
        }
    }
    public class ChangeTimerState : CutsceneAction
    {
        public bool Paused
        {
            get
            {
                return xml.GetBool("Paused");
            }
            set
            {
                xml.SetBool("Paused", value);
                OnPropertyChanged("Paused");
            }
        }
        public ChangeTimerState(XElement Xml)
            : base(Xml)
        {

        }
        public ChangeTimerState()
        {
            Type = ActionType.ChangeTimerState;
            
        }
        public override bool Execute(Cutscene cutscene)
        {
            cutscene.ChangeTimerState(Paused);
            return base.Execute(cutscene);
        }
        public override List<ActionContract> GetContracts()
        {
            var cons = base.GetContracts();
            cons.Add(new ActionContract("Paused", typeof(bool)));
            return cons;
        }
        public override string ToString()
        {
            return base.ToString() + ((Paused) ? "Pause" : "Resume");
        }
    }
    public class SetMusicVolume : CutsceneAction
    {
        public Test.VolumeType VolumeType
        {
            get
            {
                try
                {
                    return (Test.VolumeType)Enum.Parse(typeof(Test.VolumeType), xml.GetElement("VolumeType").Value);
                }
                catch
                {
                    return Test.VolumeType.MediaPlayer;
                }
            }
            set
            {
                xml.GetElement("VolumeType").Value = value.ToString();
                opc("VolumeType");
            }
        }
        public float Volume
        {
            get
            {
                float f = xml.GetElementFloat("Volume");
                if (float.IsNaN(f))
                    return 1;
                else return f;
            }
            set
            {
                xml.GetElement("Volume").Value = value.ToString();
                OnPropertyChanged("Volume");
            }
        }
        public int Frames
        {
            get
            {
                int i = 60;
                int.TryParse(xml.GetElement("Frames").Value, out i);
                return i;
            }
            set
            {
                xml.GetElement("Frames").Value = value.ToString();
                OnPropertyChanged("Frames");
            }
        }
        public SetMusicVolume(XElement Xml)
            : base(Xml)
        {
            
        }
        public SetMusicVolume()
        {
            Type = ActionType.SetMusicVolume;
        }
        public override List<ActionContract> GetContracts()
        {
            var cons = base.GetContracts();
            cons.Add(new ActionContract("Volume", typeof(float)));
            cons.Add(new ActionContract("Frames", typeof(int)));
            cons.Add(new ActionContract("VolumeType", typeof(Test.VolumeType)));
            return cons;
        }
        public override string ToString()
        {
            return base.ToString() + "Set " + VolumeType + " volume to " + Volume + " in " + Frames + " frames";
        }
        public override bool Execute(Cutscene cutscene)
        {
            new Test.VolumeChanger(Global.Game, Volume, Frames, VolumeType);
            return base.Execute(cutscene);
        }
        
    }
    public class SetGameSpeed : CutsceneAction
    {
        public float Target
        {
            get
            {
                float f = xml.GetElementFloat("Target", 1);
                return f;
            }
            set
            {
                xml.GetElement("Target").Value = value.ToString();
                OnPropertyChanged("Target");
            }
        }
        public float InitialSpeed
        {
            get
            {
                float f = xml.GetElementFloat("InitialSpeed");
                return f;
            }
            set
            {
                xml.GetElement("InitialSpeed").Value = value.ToString();

                OnPropertyChanged("InitialSpeed");
            }
        }
        public float TransitionSpeed
        {
            get
            {
                float f = xml.GetElementFloat("TransitionSpeed", 0.5f);
                return f;
            }
            set
            {
                xml.GetElement("TransitionSpeed").Value = value.ToString();
                OnPropertyChanged("TransitionSpeed");
            }
        }
        public SetGameSpeed(XElement Xml)
            : base(Xml)
        {

        }
        public SetGameSpeed()
        {
            Type = ActionType.SetGameSpeed;
        }
        public override bool Execute(Cutscene cutscene)
        {
            Global.SetSpeed(Target, TransitionSpeed, InitialSpeed);
            return base.Execute(cutscene);
        }
        public override List<ActionContract> GetContracts()
        {
           var c = base.GetContracts();
            c.Add(new ActionContract("Target", typeof(float)));
            c.Add(new ActionContract("TransitionSpeed", typeof(float)));
            c.Add(new ActionContract("InitialSpeed", typeof(float)));
            return c;
        }
        public override string ToString()
        {
            return base.ToString() + "Set game speed to " + Target + " at " + TransitionSpeed + ((!float.IsNaN(InitialSpeed)) ? " starting at " + InitialSpeed: "");
        }
    }
    public class SetObjectProperty : CutsceneAction
    {
        public string ObjectName
        {
            get
            {
                return xml.GetElement("ObjectName").Value;
            }
            set
            {
                xml.GetElement("ObjectName").Value = value;
                opc("ObjectName");
            }
        }
        public string PropertyStrings
        {
            get
            {
                return xml.GetElement("PropertyStrings").Value;
            }
            set
            {
                xml.GetElement("PropertyStrings").Value = value;
                opc("PropertyStrings");
            }
        }
        public bool Static
        {
            get
            {
                return xml.GetBool("Static");
            }
            set
            {
                xml.SetBool("Static", value);
                OnPropertyChanged("Static");
            }
        }
        public SetObjectProperty(XElement Xml)
            :base(Xml)
        {

        }
        public SetObjectProperty()
        {
            Type = ActionType.SetObjectProperty;
        }
        public override List<ActionContract> GetContracts()
        {
            var c = base.GetContracts();
            c.Add(new ActionContract("Static", typeof(bool)));
            c.Add(new ActionContract("ObjectName", typeof(string)));
            c.Add(new ActionContract("PropertyStrings", typeof(string)));
            return c;
        }
        public override string ToString()
        {
            return base.ToString() + "Change properties of " + ObjectName;
        }
        public override bool Execute(Cutscene cutscene)
        {
            if (ObjectName.Length > 0)
            {
                if (!Static)
                {
                    Obj o = Global.Game.FindObjectByName(ObjectName);
                    if (o != null)
                    {
                        Global.SetPropertiesFromString(o, PropertyStrings);
                    }
                }
                else
                {
                    Global.SetStaticPropertiesFromString(ObjectName, PropertyStrings);
                }
            }
            return base.Execute(cutscene);
        }
    }
    public class CutsceneEvent : CutsceneAction
    {
        public CutsceneEvent(XElement x)
            :base(x)
        {

        }
        public CutsceneEvent()
        {
            Type = ActionType.Event;
        }
        public override string ToString()
        {
            return base.ToString() + "Event - " + Name; ;
        }
    }
    public class SetBlur : CutsceneAction
    {
        public float Blur
        {
            get
            {
                return xml.GetElementFloat("Blur", 0);
            }
            set
            {
                xml.GetElement("Blur").Value = value.ToString();
                opc("Blur");
            }
        }
        public float Speed
        {
            get
            {
                return xml.GetElementFloat("Speed", 0);
            }
            set
            {
                xml.GetElement("Speed").Value = value.ToString();
                opc("Speed");
            }
        }
        public float StartBlur
        {
            get
            {
                return xml.GetElementFloat("StartBlur");
            }
            set
            {
                xml.GetElement("StartBlur").Value = value.ToString();
                opc("StartBlur");
            }
        }
        public SetBlur(XElement x)
            : base(x)
        {

        }
        public SetBlur()
        {
            Type = ActionType.SetBlur;
        }
        public override bool Execute(Cutscene cutscene)
        {
            if (float.IsNaN(StartBlur))
            {
                Global.Effects.SetBlur(Blur, Speed);
            }
            else
            {
                Global.Effects.SetBlur(Blur, Speed, StartBlur);
            }
            return base.Execute(cutscene);
        }
        public override List<ActionContract> GetContracts()
        {
            var c = base.GetContracts();
            c.Add(new ActionContract("Blur", typeof(float)));
            c.Add(new ActionContract("Speed", typeof(float)));
            c.Add(new ActionContract("StartBlur", typeof(float)));
            return c;
        }
        public override string ToString()
        {
            return base.ToString() + "Blur to " + Blur + " with a speed of " + Speed + ((float.IsNaN(StartBlur) ? "" : "starting at " + StartBlur));
        }
    }
    public class SetBlurDepth : CutsceneAction
    {
        public float Depth
        {
            get
            {
                return XML.GetElementFloat("Depth", 0);
            }
            set
            {
                xml.GetElement("Depth").Value = value.ToString();
                opc("Depth");
            }
        }
        public float DepthSpeed
        {
            get
            {
                return XML.GetElementFloat("DepthSpeed", 1);
            }
            set
            {
                xml.GetElement("DepthSpeed").Value = value.ToString();
                opc("DepthSpeed");
            }
        }
        public float StartDepth
        {
            get
            {
                return XML.GetElementFloat("StartDepth", float.NaN);
            }
            set
            {
                xml.GetElement("StartDepth").Value = value.ToString();
                opc("StartDepth");
            }
        }

        public float DepthRange
        {
            get
            {
                return XML.GetElementFloat("DepthRange");
            }
            set
            {
                xml.GetElement("DepthRange").Value = value.ToString();
                opc("DepthRange");
            }
        }
        public float DepthRangeSpeed
        {
            get
            {
                return XML.GetElementFloat("DepthRangeSpeed");
            }
            set
            {
                xml.GetElement("DepthRangeSpeed").Value = value.ToString();
                opc("DepthRangeSpeed");
            }
        }
        public float StartDepthRange
        {
            get
            {
                return XML.GetElementFloat("StartDepthRange");
            }
            set
            {
                xml.GetElement("StartDepthRange").Value = value.ToString();
                opc("StartDepthRange");
            }
        }
        public override string ToString()
        {
            return base.ToString() + " Set blur depth to " + Depth;
        }
        public SetBlurDepth()
        {
            Type = ActionType.SetBlurDepth;
        }
        public SetBlurDepth(XElement x)
            : base(x)
        {

        }
        public override List<ActionContract> GetContracts()
        {
            var c = base.GetContracts();
            c.Add(new ActionContract("Depth", typeof(float)));
            c.Add(new ActionContract("DepthSpeed", typeof(float)));
            c.Add(new ActionContract("StartDepth", typeof(float)));

            c.Add(new ActionContract("DepthRange", typeof(float)));
            c.Add(new ActionContract("DepthRangeSpeed", typeof(float)));
            c.Add(new ActionContract("StartDepthRange", typeof(float)));
            return c;
        }

        public override bool Execute(Cutscene cutscene)
        {
            base.Execute(cutscene);
            Global.Effects.SetDepth(Depth, DepthSpeed, StartDepth);
            if (!float.IsNaN(DepthRange) && !float.IsNaN(DepthRangeSpeed))
            Global.Effects.SetDepthRange(DepthRange, DepthRangeSpeed, StartDepthRange);
            return true;
        }
    }
    public class SetCameraDelta : CutsceneAction
    {
        public bool CenterCameraFirst
        {
            get
            {
                return xml.GetBool("CenterCameraFirst");
            }
            set
            {
                xml.SetBool("CenterCameraFirst", value);
                opc("CenterCameraFirst");
            }
        }
        public Vector3 TargetDelta
        {
            get
            {
                return StageObjectPropertyConverter.GetVector3(xml.GetElement("TargetDelta").Value);
            }
            set
            {
                xml.GetElement("TargetDelta").Value = StageObjectPropertyConverter.SetValue(value);
                opc("TargetDelta");
            }
        }
        public bool SetStartingDelta
        {
            get
            {
                return xml.GetBool("SetStartingDelta");
            }
            set
            {
                xml.SetBool("SetStartingDelta", true);
            }
        }
        public Vector3 StartingDelta
        {
            get
            {
                return StageObjectPropertyConverter.GetVector3(xml.GetElement("StartingDelta").Value);
            }
            set
            {
                xml.GetElement("StartingDelta").Value = StageObjectPropertyConverter.SetValue(value);
                opc("StartingDelta");
            }
        }
        public float Frames
        {
            get
            {
                return xml.GetElementFloat("Frames", 1);
            }
            set
            {
                xml.GetElement("Frames").Value = value.ToString();
                opc("Frames");
            }
        }
        public float StartPower
        {
            get
            {
                return xml.GetElementFloat("StartPower", 1);
            }
            set
            {
                xml.GetElement("StartPower").Value = value.ToString();
                opc("StartPower");
            }
        }
        public float EndPower
        {
            get
            {
                return xml.GetElementFloat("EndPower", 1);
            }
            set
            {
                xml.GetElement("EndPower").Value = value.ToString();
                opc("EndPower");
            }
        }

        public SetCameraDelta()
        {
            Type = ActionType.SetCameraDelta;
        }
        public SetCameraDelta(XElement x)
            :base(x)
        {

        }
        public override List<ActionContract> GetContracts()
        {
            var c = base.GetContracts();
            c.Add(new ActionContract("CenterCameraFirst", typeof(bool)));
            c.Add(new ActionContract("TargetDelta", typeof(Vector3)));
            c.Add(new ActionContract("Frames", typeof(float)));
            c.Add(new ActionContract("SetStartingDelta", typeof(bool)));
            c.Add(new ActionContract("StartingDelta", typeof(Vector3)));
            c.Add(new ActionContract("StartPower", typeof(float)));
            c.Add(new ActionContract("EndPower", typeof(float)));
            return c;
        }
        public override bool Execute(Cutscene cutscene)
        {
            if (SetStartingDelta)
                Global.Camera.DeltaPos = StartingDelta;

            if (CenterCameraFirst)
                Global.Camera.CenterOnTarget();
            new Test.CameraDeltaChanger(TargetDelta, Frames, StartPower, EndPower);
            return base.Execute(cutscene);
        }
        public override string ToString()
        {
            return base.ToString() + " Set camera delta to " + TargetDelta + " in " + Frames + " frames";
        }
    }
    public class SetScreenFade : CutsceneAction
    {
        public Test.FadeType FadeType
        {
            set
            {
                xml.GetElement("FadeType").Value = value.ToString();
                opc("FadeType");
            }
            get
            {
                try
                {
                    return (Test.FadeType)Enum.Parse(typeof(Test.FadeType), xml.GetElement("FadeType").Value);
                }
                catch
                {
                    return Test.FadeType.FadeOut;
                }
            }
        }
        public float Frames
        {
            set
            {
                xml.GetElement("Frames").Value = value.ToString();
                opc("Frames");
            }
            get
            {
                return xml.GetElementFloat("Frames", 0);
            }
        }
        public SetScreenFade()
        {
            Type = ActionType.SetScreenFade;
        }
        public SetScreenFade(XElement x)
            : base(x)
        {

        }
        public override string ToString()
        {
            return base.ToString() + " " + ((FadeType == Test.FadeType.FadeIn) ? "Fade in screen in " : "Fade out screen in ") + Frames + " frames";
        }
        public override List<ActionContract> GetContracts()
        {
            var c = base.GetContracts();
            c.Add(new ActionContract("FadeType", typeof(Test.FadeType)));
            c.Add(new ActionContract("Frames", typeof(float)));
            return c;
        }
        public override bool Execute(Cutscene cutscene)
        {
            new Test.ScreenFadeChanger(cutscene, FadeType, Frames);
            return base.Execute(cutscene);
        }
    }
    public class PreloadContent : CutsceneAction
    {
        public String Content
        {
            get
            {
                return xml.GetElement("Content").Value;
            }
            set
            {
                opc("Content");
                xml.GetElement("Content").Value = value;
            }
        }
        public bool Async
        {
            get
            {
                return xml.GetBool("Async");
            }
            set
            {
                opc("Async");
                xml.SetBool("Async", value);
            }
        }
        public PreloadContent(XElement xml)
            :base(xml)
        {
            
        }
        public PreloadContent()
        {
            Type = ActionType.PreloadContent;
            Async = true;
        }
        public override List<ActionContract> GetContracts()
        {
            var c = base.GetContracts();
            c.Add(new ActionContract("Content", typeof(string)));
            c.Add(new ActionContract("Async", typeof(bool)));
            return c;
        }
        public override bool Execute(Cutscene cutscene)
        {
            if (Async)
            {
                loadAsync();
            }
            else
            {
                load();
            }
            return base.Execute(cutscene);
        }
        private async void loadAsync()
        {
            await load();
        }
        private async Task load()
        {
            string[] files = Content.Split(',');
            foreach (var f in files)
            {
                string file = f.RemoveSpaces();
                try
                {
                    var con = Global.Game.Content.Load<object>(file);
                    con = con;
                }
                catch
                {

                }
            }
        }
        public override string ToString()
        {
            return base.ToString() + "Preload content" + (Async ? " asynchronously" : "");
        }
    }
    public class SetLookDirection : CutsceneAction
    {
        public Vector2 LookDirection
        {
            get
            {
                var s = xml.GetElement("LookDirection").Value;
                if (!string.IsNullOrWhiteSpace(s))
                    return StageObjectPropertyConverter.GetVector2(s);
                else return Vector2.Zero;
            }
            set
            {
                xml.GetElement("LookDirection").Value = StageObjectPropertyConverter.SetValue(value);
                opc("LookDirection");
            }
        }
        public float FrameDuration
        {
            get
            {
                var f = xml.GetElementFloat("FrameDuration", 60);
                return f;
            }
            set
            {
                xml.GetElement("FrameDuration").Value = value.ToString();
                opc("LookDirection");
            }
        }
        public float StartPower
        {
            get { return xml.GetElementFloat("StartPower", 1); }
            set { xml.GetElement("StartPower").Value = value.ToString(); }
        }
        public float EndPower
        {
            get { return xml.GetElementFloat("EndPower", 1); }
            set { xml.GetElement("EndPower").Value = value.ToString(); }
        }
        public bool Animated
        {
            get
            {
                return xml.GetBool("Animated", true);
            }
            set
            {
                xml.SetBool("Animated", value);
            }
        }
        public SetLookDirection(XElement xml)
            : base(xml)
        {

        }
        public SetLookDirection()
        {
            Type = ActionType.SetLookDirection;
        }
        public override List<ActionContract> GetContracts()
        {
            var c = base.GetContracts();
            c.Add(new ActionContract("Animated", typeof(bool)));
            c.Add(new ActionContract("LookDirection", typeof(Vector2)));
            c.Add(new ActionContract("FrameDuration", typeof(float)));
            c.Add(new ActionContract("EndPower", typeof(float)));
            c.Add(new ActionContract("StartPower", typeof(float)));
            return c;
        }
        public override bool Execute(Cutscene cutscene)
        {
            if (Animated)
            {
                var ani = new CameraLookDirectionAnimation(LookDirection);
                ani.FrameDuration = FrameDuration;
                ani.EndPower = EndPower;
                ani.StartPower = StartPower;
                ani.Begin();
            }
            else
            {
                AnimationManager.ClearAnimation(Global.Camera, "LookDirection");
                Global.Camera.SetLookDirection(LookDirection, 1);
            }
            return base.Execute(cutscene);
        }
        public override string ToString()
        {
            return base.ToString() + "Set look direction to " + StageObjectPropertyConverter.SetValue(LookDirection) + " in " + (int)FrameDuration + " frames";
        }
    }
    public class ResetCamera : CutsceneAction
    {
        public ResetCamera(XElement xml)
            : base(xml)
        {

        }
        public ResetCamera()
        {
            Type = ActionType.ResetCamera;
        }
        public override bool Execute(Cutscene cutscene)
        {
            Global.Camera.Reset();
            return base.Execute(cutscene);
        }
        public override string ToString()
        {
            return base.ToString() + "Reset camera";
        }
    }
    public class SetCameraShake : CutsceneAction
    {
        public float ShakeSpeed
        {
            get
            {
                return xml.GetElementFloat("Speed", 0);
            }
            set
            {
                xml.GetElement("Speed").Value = value.ToString();
                opc("ShakeSpeed");
            }
        }
        public float TargetOrDissipationSpeed
        {
            get
            {
                return xml.GetElementFloat("TargetSpeed", 0);
            }
            set
            {
                xml.GetElement("TargetSpeed").Value = value.ToString();
                opc("TargetOrDissipationSpeed");
            }
        }
        public float ShakeSize
        {
            get
            {
                return xml.GetElementFloat("Size", 0);
            }
            set
            {
                xml.GetElement("Size").Value = value.ToString();
                opc("ShakeSize");
            }
        }
        public bool IsOverallShake
        {
            get
            {
                return xml.GetBool("Overall", true);
            }
            set
            {
                xml.SetBool("Overall", value);
                opc("IsOverallShake");
            }

        }
        public SetCameraShake(XElement Xml)
            : base(Xml)
        {

        }
        public SetCameraShake()
        {
            Type = ActionType.SetCameraShake;
        }
        public override List<ActionContract> GetContracts()
        {
            var c = base.GetContracts();
            c.Add(new ActionContract("IsOverallShake", typeof(bool)));
            c.Add(new ActionContract("ShakeSpeed", typeof(float)));
            c.Add(new ActionContract("ShakeSize", typeof(float)));
            c.Add(new ActionContract("TargetOrDissipationSpeed", typeof(float)));
            return c;
        }
        public override bool Execute(Cutscene cutscene)
        {
            if (IsOverallShake)
            {
                Global.Camera.Shake.SetShakeSize(ShakeSize, TargetOrDissipationSpeed);
                Global.Camera.Shake.SetShakeSpeed(ShakeSpeed, TargetOrDissipationSpeed);
            }
            else
            {
                Global.Camera.AddShake(ShakeSize, ShakeSpeed, TargetOrDissipationSpeed);
            }
            return base.Execute(cutscene);
        }
    }
    #endregion
}
