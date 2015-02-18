using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Threading;
using System.Diagnostics;
using SecondShiftMobile.Cutscenes;
using SecondShiftMobile.UI;
using Microsoft.Xna.Framework.Input.Touch;
using SecondShiftMobile.Test;
using SecondShiftMobile.UI.Animations;
using SecondShiftMobile.Networking;

namespace SecondShiftMobile
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        bool batchSprites = true;

        public bool Distort
        {
            get { return distort; }
        }
        Resolutions resolution = Resolutions.R360p;
        public Resolutions Resolution
        {
            get
            {
                return resolution;
            }
        }
        Texture2D joystickTex;
        GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        Texture2D skySprite;
        public RenderTarget2D finalTarget, screenTarget, depthTarget, depthTarget2, effectMap, blurTargetX, blurTargetY, bloomTargetX, bloomTargetY, screenTargetSmall;
        public SpriteFont font1;
        float aspectRatio = 1;
        public Obj[] objArray;
        int numObj = 0;
        List<Obj> removeObjects = new List<Obj>();
        List<LightBloom> blooms = new List<LightBloom>();

        

        public int NumberOfObjects
        {
            get { return numObj; }
        }

        bool _firstUpdate = true;
        float[] frameRates, updateRates;
        int frameIndex = 0, updateIndex;
        public bool objBlooming = false, drawSecondShift = false;
        TimeSpan targetDrawTime, drawTime;
        int draws = 0;
        public bool LimitFramerate = true;
        int cpuCount, threadCount;
        ParameterizedThreadStart threadStart;
        ThreadStart nonCriticalThreadStart;
        Thread nonCriticalThread;
        bool nonCriticalThreadReady = true;
        Thread[] threads;
        Dictionary<string, Obj> objectNames;
        Dictionary<Type, Obj> objTypes;
        Dictionary<Type, Obj[]> objTypesArray;
        public Color SkyColor = Color.SkyBlue;
        public event EventHandler ResolutionChanged;
#if PC
        System.Windows.Forms.Form window;
#endif
        bool spriteBegan = false;
        BlendState spriteBlend = BlendState.AlphaBlend;
        int spriteEnds = 0;

        public BlendState SpriteBlend
        {
            get
            {
                return spriteBlend;
            }
        }
        Panel menuPanel;
        Color[] hdrColor;
        float hdrPowerTarget = 1;
        bool multiThread = false;
        List<string> networkLogs = new List<string>();
        public Game1()
        {
            Helper.Write("Game1 constructor");
            hdrColor = new Color[] { new Color(120, 120, 120, 255) };
            cpuCount = System.Environment.ProcessorCount;
            threadCount = cpuCount - 2;
            if (threadCount > 0)
            {
                multiThread = true;
                threadStart = new ParameterizedThreadStart(parallelUpdate);
                threads = new Thread[threadCount];
            }
            multiThread = false;
            frameRates = new float[30];
            for (int i = 0; i < frameRates.Length; i++)
            {
                frameRates[i] = 0;
            }
            updateRates = new float[30];
            for (int i = 0; i < updateRates.Length; i++)
            {
                updateRates[i] = 0;
            }
            graphics = new GraphicsDeviceManager(this);
            Content = new CustomContent(Services);
            Content.RootDirectory = "Content";
            Global.Game = this;
            objArray = new Obj[10000];
            objectNames = new Dictionary<string, Obj>();
            objTypes = new Dictionary<Type, Obj>();
            objTypesArray = new Dictionary<Type, Obj[]>();
            
#if MONO
            TargetElapsedTime = TimeSpan.FromSeconds(1d / 60d);
            targetDrawTime = TimeSpan.FromSeconds(1d / 30d);
#else
            TargetElapsedTime = TimeSpan.FromSeconds(1d / 60d);
            targetDrawTime = TimeSpan.FromSeconds(1d / 45);
#endif
            //graphics.SynchronizeWithVerticalRetrace = false;
            //IsFixedTimeStep = false;
            drawTime = TimeSpan.Zero;
#if PC
            window = (System.Windows.Forms.Form)System.Windows.Forms.Form.FromHandle(Global.Game.Window.Handle);
#endif
            NetworkManager.Logged += NetworkManager_Logged;
        }
        protected override void OnExiting(object sender, EventArgs args)
        {
            NetworkManager.Dispose();
            base.OnExiting(sender, args);
        }
        void NetworkManager_Logged(object sender, NetworkLoggedEventArgs e)
        {
            networkLogs.Add(e.Log);
            if (networkLogs.Count > 5)
                networkLogs.RemoveAt(0);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            base.Initialize();
            //GraphicsDevice.PresentationParameters.BackBufferFormat = SurfaceFormat.Rgba64;
#if MONO
            TouchPanel.EnabledGestures = GestureType.None;
            TouchPanel.EnableMouseGestures = false; 
#endif

        }
        #region Object Code
        /// <summary>
        /// Finds and returns the first active object with the given name. Returns null if the object doesn't exist.
        /// </summary>
        /// <param name="Name">The name of the object to find</param>
        public Obj FindObjectByName(string Name)
        {
            if (objectNames.ContainsKey(Name))
                return objectNames[Name];
            else
            {
                for (int i = 0; i < numObj; i++)
                {
                    var o = objArray[i];
                    if (o != null && o.Name == Name)
                    {
                        try
                        {
                            objectNames.Add(Name, o);
                        }
                        catch
                        {

                        }
                        return o;
                    }
                }
            }
            return null;
        }
        public T FindObject<T>() where T : Obj
        {
            Type t;
            if (objTypes.ContainsKey(t = typeof(T)))
            {
                return objTypes[t] as T;
            }
            for (int i = 0; i < numObj; i++)
            {
                var o = objArray[i];
                T ot;
                if (o != null && ((ot = o as T) != null))
                {
                    try
                    {
                        objTypes.Add(typeof(T), ot);
                    }
                    catch
                    {

                    }
                    return ot;
                }
            }
            return null;
        }

        public T[] FindObjects<T>() where T : Obj
        {
            Type t;
            if (objTypesArray.ContainsKey(t = typeof(T)))
            {
                return objTypesArray[t] as T[];
            }
            List<T> list = new List<T>();
            for (int i = 0; i < numObj; i++)
            {
                var o = objArray[i];
                T ot;
                if (o != null && ((ot = o as T) != null))
                {
                    list.Add(ot);
                }
            }
            var arr = list.ToArray();
            try
            {
                objTypesArray.Add(typeof(T), arr);
            }
            catch
            {

            }
            return arr;
        }
        public void AddObj(Obj obj)
        {
            objArray[numObj] = obj;
            numObj++;
        }
        public void RemoveObj(Obj obj)
        {
            removeObjects.Add(obj);
        }
        void clearRemoveObjects()
        {
            foreach (var o in removeObjects)
            {
                removeObject(o);
            }
            removeObjects.Clear();
        }
        void removeObject(Obj obj)
        {
            int ind = -1;
            for (int i = 0; i < numObj; i++)
            {
                if (objArray[i] == obj)
                {
                    ind = i;
                    break;
                }
            }
            if (ind != -1)
            {
                objArray[ind] = null;
                for (int i = ind; i < numObj - 1; i++)
                {
                    if (i < numObj)
                    {
                        var temp = objArray[i + 1];
                        objArray[i + 1] = null;
                        objArray[i] = temp;
                    }
                }
                numObj--;
            }
            else
                return;
        }
        public void ClearObjects()
        {
            for (int i = 0; i < numObj; i++)
            {
                objArray[i].Remove();
            }
        }
        #endregion
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            Graphics.init(GraphicsDevice, graphics);
            nonCriticalThreadStart = new ThreadStart(nonCriticalUpdate);
            Global.init();
            LimitFramerate = false;
            spriteBatch = new SpriteBatch(GraphicsDevice);
            joystickTex = Content.Load<Texture2D>("Joystick");
            Effect effect = Content.Load<Effect>("Shaders");
            skySprite = Content.Load<Texture2D>("Sky");
            font1 = Content.Load<SpriteFont>("SpriteFont1");
            var l = Content.Load<object>("Light");
            Helper.Write(effect + " " + effect.Parameters.Count + " effects in the shader");
            Global.Effects = new Effects(effect);
            Global.Effects.Parameters["cameraLookDirectionW"].SetValue(Camera.LookDirectionW);
            Global.Drawer = new EffectsDrawer(Global.Effects);
#if PC
            Global.Effects.Quality = Quality.Medium;
            IsMouseVisible = true;
            Vector2 defaultScreen = new Vector2(1280, 0);
            if (defaultScreen.X > Graphics.ScreenResolution.X * 0.75f)
            {
                defaultScreen.X = Graphics.ScreenResolution.X * 0.75f;
            }
            defaultScreen.Y = defaultScreen.X * (9f / 16f);
            Graphics.SetWindowSize(defaultScreen);
#if !DEBUG
            Graphics.ToggleFullscreen();
#endif
            graphics.ApplyChanges();
            SetResolution(Resolutions.R480p, 16, 9, 1, 4, false);
#if DEBUG
            var cb = new CutsceneBuilder();
            cb.Show();
#endif
            
#else
            Graphics.SetWindowSize(GraphicsDevice.PresentationParameters.BackBufferHeight, GraphicsDevice.PresentationParameters.BackBufferWidth);
            Global.Effects.Quality = Quality.Medium;
            SetResolution(Resolutions.R360p, 16, 9, 2f, 4, false);
#endif
#if !PC
            NetworkManager.BeginListening("192.168.1.101");      
#endif
            var cut = Cutscene.Load("Flat Floor Scene");
            Global.Cutscene = cut;
            /*Global.Camera.SetLookDirection(new Vector2(1, -0f), 1);
            Obj obj = new Obj(this, LoadTex("Thing"), 100, 0, 0);
            var ani = new FloatAnimation(obj, "Rotation.Y", -3600) { FrameDuration = 600, EndPower = 4 };
            ani.Begin();
            var ani2 = new CameraLookDirectionAnimation(new Vector2(0, 0)) { FrameDuration = 600, EndPower = 4 };
            ani2.Begin();*/
            //LevelBuilder.CreateLevel(LevelBuilder.Load("Flat Floor Stage"));
            /*for (int i = -1000; i < 1000; i+= 50)
            {
                for (int o = -100; o < 5000; o+= 50)
                {
                    new Test.TestPolygonGrass(this, i, 64, o);
                }
            }*/
            
            //Global.Effects.SetBloom(0.25f, 0.1f);
#if MONO
            
#endif
            SoundManager.IsRepeating = true;
            menuPanel = new PauseMenu()
            {
                Opacity = 0,
                Width = Global.ScreenSize.X,
                Height = Global.ScreenSize.Y
            };
            menuPanel.UpdateLayout();
            Global.Effects.QualityChanged += Effects_QualityChanged;
            Camera.GetScreenPosition(new Vector3(0, 0, 50000), new Vector3(0, 0, 0), 500, 1, new Vector2(1, 0), new Vector2(960, 540));
        }

        void Effects_QualityChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < numObj; i++)
            {
                objArray[i].QualityChanged(Global.Effects.Quality);
            }
        }

        float horizontalRatio = 16, verticalRatio = 9, blurTargetDivision = 2, bloomTargetDivision = 4;
        bool forcePowerOfTwo = false;
        public void SetResolution(Resolutions res, float horizontalRatio, float verticalRatio, float blurTargetDivision = 2, float bloomTargetDivision = 4, bool forcePowerOfTwo = false)
        {
            this.horizontalRatio = horizontalRatio; this.verticalRatio = verticalRatio; this.blurTargetDivision = blurTargetDivision; this.bloomTargetDivision = bloomTargetDivision;
            this.forcePowerOfTwo = forcePowerOfTwo;
            float ratio = aspectRatio = horizontalRatio / verticalRatio;
            int height = (int)res;
            int width = (int)(height * ratio);
            int blurSizeX = (forcePowerOfTwo) ? MyMath.ToPowerOf2((int)(width / blurTargetDivision), RoundingMode.Down) : (int)(width / blurTargetDivision);
            int blurSizeY = (forcePowerOfTwo) ? MyMath.ToPowerOf2((int)(height / blurTargetDivision), RoundingMode.Down) : (int)(height / blurTargetDivision);
            int bloomSizeX = (forcePowerOfTwo) ? MyMath.ToPowerOf2((int)(width / bloomTargetDivision), RoundingMode.Down) : (int)(width / bloomTargetDivision);
            int bloomSizeY = (forcePowerOfTwo) ? MyMath.ToPowerOf2((int)(height / bloomTargetDivision), RoundingMode.Down) : (int)(height / bloomTargetDivision);
            //bloomSizeX = bloomSizeY = 180;
            int screenSmallSize = blurSizeX;

            GameExtensions.TryDispose(finalTarget, screenTarget, depthTarget, depthTarget2, blurTargetX, blurTargetY, bloomTargetX, bloomTargetY, screenTargetSmall);

#if MONO
            SurfaceFormat surfaceFormat = SurfaceFormat.Color;
#else
            SurfaceFormat surfaceFormat = SurfaceFormat.Rgba1010102;
#endif
            SurfaceFormat surfaceFormat2 = SurfaceFormat.Color;

            finalTarget = new RenderTarget2D(GraphicsDevice, width, height, false, GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            screenTarget = new RenderTarget2D(GraphicsDevice, width, height, false, surfaceFormat, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            depthTarget = new RenderTarget2D(GraphicsDevice, width, height, false, surfaceFormat, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            depthTarget2 = new RenderTarget2D(GraphicsDevice, width, height, false, surfaceFormat, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            screenTargetSmall = new RenderTarget2D(GraphicsDevice, screenSmallSize, screenSmallSize, true, GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            blurTargetX = new RenderTarget2D(GraphicsDevice, blurSizeX, blurSizeX, forcePowerOfTwo, surfaceFormat2, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            blurTargetY = new RenderTarget2D(GraphicsDevice, blurSizeX, blurSizeX, forcePowerOfTwo, surfaceFormat2, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            bloomTargetX = new RenderTarget2D(GraphicsDevice, bloomSizeX, bloomSizeX, forcePowerOfTwo, surfaceFormat2, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            bloomTargetY = new RenderTarget2D(GraphicsDevice, bloomSizeX, bloomSizeX, forcePowerOfTwo, surfaceFormat2, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            Global.Effects.DepthMap = depthTarget;
            GraphicsDevice.SamplerStates[1] = SamplerState.LinearClamp;
            GraphicsDevice.SamplerStates[2] = SamplerState.LinearClamp;
            resolution = res;
            if (ResolutionChanged != null)
            {
                ResolutionChanged(this, new EventArgs());
            }
        }

        public void SetResolution(Resolutions res)
        {
            SetResolution(res, horizontalRatio, verticalRatio, blurTargetDivision, bloomTargetDivision, forcePowerOfTwo);
        }

        #region Content Loading
        /// <summary>
        /// This will load a song out of the music folder
        /// </summary>
        /// <param name="Name">The name of the song in the music folder</param>
        public Song LoadSong(string Name)
        {
            return Content.Load<Song>("Sound/Music/" + Name);
        }

        /// <summary>
        /// Thos will load a texture from the given path
        /// </summary>
        /// <param name="Path">The path to the texture file</param>
        /// <returns></returns>
        public TextureFrame LoadTex(string Path)
        {
            return TextureFrame.FromTexture(Content.Load<Texture2D>(Path));
        }

        /// <summary>
        /// This will load a sound effect out of the sound effects folder
        /// </summary>
        /// <param name="Name">The name of the sound in the folder</param>
        /// <returns>The sound effect you want to load</returns>
        public SoundEffect LoadSoundEffect(string Name)
        {
            return Content.Load<SoundEffect>("Sound/Effects/" + Name);
        }

        /// <summary>
        /// This will load a sound effect out of the sound effects folder and creates an instance
        /// </summary>
        /// <param name="Name">The name of the sound in the folder</param>
        /// <returns>An instance of the sound effect you wish to load</returns>
        public SoundEffectInstance LoadSoundEffectInstance(string Name)
        {
            return Content.Load<SoundEffect>("Sound/Effects/" + Name).CreateInstance();
        }
        #endregion
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            
            //Global.Camera.SetLookDirection(-Controls.RightStick, 0.1f);
            if (menuPanel.Opacity >= 0.1f)
            {
                menuPanel.Update();
                if (menuPanel.NeedsLayoutUpdate)
                    menuPanel.UpdateLayout();
            }
            objectNames.Clear();
            objTypes.Clear();
            objTypesArray.Clear();
            updateRates[updateIndex % updateRates.Length] = (float)(1d / gameTime.ElapsedGameTime.TotalSeconds);
            updateIndex++;
            menuPanel.Height = Global.ScreenSize.Y;
            menuPanel.Width = Global.ScreenSize.X;
            // TODO: Add your update logic here
#if PC
            if (Controls.GetButton(Buttons.Start) == ControlState.Pressed || Controls.GetKey(Keys.Escape) == ControlState.Pressed)
#else
            if (GamePad.GetState(0).IsButtonDown(Buttons.Back))
#endif
            {
                if (Global.GameState == GameState.Playing)
                {
                    (menuPanel as PauseMenu).Animate();
                    Global.GameState = GameState.Paused;
                }
                else if (Global.GameState == GameState.Paused)
                {
                    Global.GameState = GameState.Playing;
                }
            }
#if PC
            if (window.Focused)
            {
                if (Controls.GetKey(Keys.N) == ControlState.Released)
                {
                    NetworkManager.BeginListening("192.168.1.101");
                }
                if (Controls.GetKey(Keys.B) == ControlState.Released)
                {
                    NetworkManager.ConnectTo("129.21.75.77");
                }
                if (Controls.GetKey(Keys.M) == ControlState.Released)
                {
                    NetworkManager.Send(new SocketMessage() { Message = "hello!", Command = "woo!", ObjectId = "id" });
                }
#if DEBUG
                if (Controls.GetKey(Keys.Tab) == ControlState.Pressed)
                {
                    LevelBuilder.Active = !LevelBuilder.Active;
                }
#endif
                if (Controls.GetKey(Keys.Enter) == ControlState.Pressed && (Controls.GetKey(Keys.RightAlt) == ControlState.Held || Controls.GetKey(Keys.LeftAlt) == ControlState.Held))
                {
                    Graphics.ToggleFullscreen();
                }
                if (Controls.GetKey(Keys.D1) == ControlState.Pressed)
                {
                    Global.Effects.Quality = Quality.VeryLow;
                }
                if (Controls.GetKey(Keys.D2) == ControlState.Pressed)
                {
                    Global.Effects.Quality = Quality.Low;
                }
                if (Controls.GetKey(Keys.D3) == ControlState.Pressed)
                {
                    Global.Effects.Quality = Quality.Medium;
                }
                if (Controls.GetKey(Keys.D4) == ControlState.Pressed)
                {
                    Global.Effects.Quality = Quality.High;
                }
                if (Controls.GetKey(Keys.D5) == ControlState.Pressed)
                {
                    Global.Effects.Quality = Quality.VeryHigh;
                }
                
            }
#endif
            if (Global.GameState != GameState.Paused)
            {
                
                if ((nonCriticalThread != null && nonCriticalThread.ThreadState == System.Threading.ThreadState.Stopped) || nonCriticalThreadReady)
                {
                    nonCriticalThread = new Thread(nonCriticalThreadStart) { IsBackground = true };
                    nonCriticalThreadReady = false;
                    nonCriticalThread.Start();
                }
            }
            Global.Update(gameTime);
            AnimationManager.Update(Global.FrameSpeed, Global.Speed);
#if DEBUG
            if (LevelBuilder.Active)
            {
                for (int i = 0; i < numObj; i++)
                {
                    objArray[i].SetPosition();
                    if (objArray[i].DeactivateOffscreen && objArray[i].OnScreen)
                        objArray[i].Active = true;
                }
            }
            else
#endif
            if (Global.GameState != GameState.Paused)
            {
                if (multiThread)
                {
                    for (int i = 0; i < threadCount; i++)
                    {
                        threads[i] = new Thread(threadStart);
                        threads[i].Start(i);
                    }
                    serialUpdate();
                }
                else
                    normalUpdate();
            }
            if (Global.Cutscene != null && !LevelBuilder.Active && Global.GameState != GameState.Paused)
            {
                Global.Cutscene.Update();
            }

            if (multiThread)
            {
                for (int i = 0; i < threadCount; i++)
                {
                    threads[i].Join();
                }
            }

            clearRemoveObjects();
            sortArray();
            base.Update(gameTime);
#if MONO
            if (_firstUpdate)
            {
                // Temp hack to fix gestures
                /*typeof(Microsoft.Xna.Framework.Input.Touch.TouchPanel)
                    .GetField("_touchScale", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
                    .SetValue(null, Vector2.One);*/

                _firstUpdate = false;
            }

#endif

            menuPanel.Opacity = MathHelper.Clamp(menuPanel.Opacity + (((Global.GameState == GameState.Paused) ? 0.2f : -0.1f) * Global.FrameSpeed), 0, 1);

        }
        void sortArray()
        {
            Obj obj, obj1;
            for (int i = 0; i < numObj; i++)
            {
                if (i > 0)
                {
                    obj = objArray[i];
                    obj1 = objArray[i - 1];
                    if (obj1 != null && obj != null)
                    {
                        
                        if (
                            (!obj.AlwaysOnTop && obj1.AlwaysOnTop) || 
                            (obj.AlwaysOnBottom && !obj1.AlwaysOnBottom) || 
                            (obj.SortingType == DepthSortingType.ZDepth && obj1.SortingType == DepthSortingType.ZDepth && ( (obj1.Pos.Z < obj.Pos.Z) )) || 
                            (obj.SortingType == DepthSortingType.ZDepth && obj1.SortingType == DepthSortingType.ZDepth && obj1.Pos.Z == obj.Pos.Z && obj1.Depth < obj.Depth) 
                            )
                        {
                            //if ((obj is FlatFloor || obj1 is FlatFloor))
                                //System.Diagnostics.Debugger.Break();
                            objArray[i] = obj1;
                            objArray[i - 1] = obj;
                            i -= 2;
                        }
                    }
                }
            }
        }
        #region Updates
        void nonCriticalUpdate()
        {
            try
            {
                for (int i = 0; i < numObj; i++)
                {
                    if (objArray[i].Active)
                    {
                        objArray[i].NonCriticalUpdate();
                    }
                }
            }
            catch
            {

            }
            //nonCriticalThreadReady = true;
        }
        void normalUpdate()
        {
            distort = false;
            objBlooming = false;
            drawSecondShift = false;
            blooms.Clear();
            for (int i = 0; i < numObj; i++)
            {
                if (objArray[i].Active)
                {
                    if (objArray[i].IsPaused)
                    {
                        objArray[i].IncrementPause();
                        continue;
                    }
                    objArray[i].EarlyUpdate();
                }
            }
            for (int i = 0; i < numObj; i++)
            {
                if (objArray[i].Active)
                {
                    if (objArray[i].IsPaused)
                    {
                        objArray[i].IncrementPause();
                        continue;
                    }
                    objArray[i].Update();
                }
            }
            for (int i = 0; i < numObj; i++)
            {
                if (objArray[i].Active)
                {
                    if (objArray[i].IsPaused)
                    {
                        objArray[i].IncrementPause();
                        continue;
                    }
                    if (objArray[i].OnScreen && objArray[i].Alpha > 0.01)
                    {
                        if (objArray[i].Bloom > 0.01f || (objArray[i].IsBeveling && objArray[i].BevelGlow > 0.01f))
                        {
                            objBlooming = true;
                        }
                        if (objArray[i].Distortion > 0)
                        {
                            distort = true;
                        }
                        if (objArray[i].ShouldDrawSecondShift())
                        {
                            drawSecondShift = true;
                        }
                    }
                    objArray[i].LateUpdate();
                }
            }
            for (int i = 0; i < numObj; i++)
            {
                if (objArray[i].Active)
                {
                    if (objArray[i].IsPaused)
                    {
                        objArray[i].IncrementPause();
                        continue;
                    }
                    objArray[i].ParallelUpdate();
                    if (objArray[i].IsLight)
                    {
                        blooms.Add(objArray[i] as LightBloom);
                    }
                }
                else if (objArray[i].DeactivateOffscreen)
                {
                    objArray[i].SetPosition();
                }
            }
        }
        bool distort = false;
        void serialUpdate()
        {
            distort = false;
            objBlooming = false;
            blooms.Clear();
            drawSecondShift = false;
            int n = numObj;
            for (int i = 0; i < n; i++)
            {
                if (objArray[i].Active && !objArray[i].Parallel)
                {
                    if (objArray[i].IsPaused)
                    {
                        objArray[i].IncrementPause();
                        continue;
                    }
                    objArray[i].EarlyUpdate();
                }
            }
            for (int i = 0; i < n; i++)
            {
                if (objArray[i].Active && !objArray[i].Parallel)
                {
                    if (objArray[i].IsPaused)
                    {
                        objArray[i].IncrementPause();
                        continue;
                    }
                    objArray[i].Update();
                }
            }
            for (int i = 0; i < n; i++)
            {
                if (objArray[i].Active)
                {
                    if (objArray[i].IsPaused)
                    {
                        objArray[i].IncrementPause();
                        continue;
                    }
                    if (objArray[i].OnScreen && objArray[i].Alpha > 0.01)
                    {
                        if (objArray[i].Bloom > 0.01f || (objArray[i].IsBeveling && objArray[i].BevelGlow > 0.01f))
                        {
                            objBlooming = true;
                        }
                        if (objArray[i].Distortion > 0)
                        {
                            distort = true;
                        }
                        if (objArray[i].ShouldDrawSecondShift())
                        {
                            drawSecondShift = true;
                        }
                    }
                    if (objArray[i].IsLight)
                    {
                        blooms.Add((LightBloom)objArray[i]);
                    }
                    if (!objArray[i].Parallel)
                        objArray[i].LateUpdate();

                }
            }
        }
        void parallelUpdate(object o)
        {
            int num = (int)o;
            int n = numObj;
            for (int i = num; i < n; i+=threadCount)
            {
                if (objArray[i].Active)
                {
                    if (objArray[i].IsPaused)
                    {
                        objArray[i].IncrementPause();
                        continue;
                    }
                    if (objArray[i].Parallel)
                    {
                        objArray[i].EarlyUpdate();
                        objArray[i].Update();
                        objArray[i].LateUpdate();
                    }
                    objArray[i].ParallelUpdate();
                }
                else if (objArray[i].DeactivateOffscreen)
                {
                    objArray[i].SetPosition();
                }
            }
        }
        #endregion

        public TextureFrame[] LoadAnimation(string folder, string name, int startIndex, int endIndex)
        {
            TextureFrame[] textures = new TextureFrame[(endIndex - startIndex) + 1];
            int index = 0;
            string path;
            for (int i = startIndex; i <= endIndex; i++)
            {
                if (i >= 10)
                    path = folder + "/" + name + i;
                else path = folder + "/" + name + "0" + i;
                textures[index] = TextureFrame.FromTexture(Content.Load<Texture2D>(path));
                index++;
            }
            return textures;
        }
        public TextureFrame[] LoadAtlasAnimation(string path, Vector2 originFrame, Vector2 frameSize, int frames, TextureDirection dir)
        {
            Texture2D tex = LoadTex(path);
            TextureFrame[] textures = new TextureFrame[frames];
            int index = 0;
            for (int i = 0; i < frames; i++)
            {
                textures[i] = TextureFrame.FromAtlas(tex, originFrame, frameSize, i, dir);
            }
            return textures;
        }
        public TextureFrame[] LoadAtlasAnimation(string path, Vector2 originFrame, Vector2 frameSize, int frames, TextureDirection dir, int loop)
        {
            Texture2D tex = LoadTex(path);
            TextureFrame[] textures = new TextureFrame[frames * (loop + 1)];
            int index = 0;
            for (int i = 0; i < frames * (loop + 1); i++)
            {
                textures[i] = TextureFrame.FromAtlas(tex, originFrame, frameSize, i % frames, dir);
            }
            return textures;
        }
        public void SpriteBegin(BlendState blendState)
        {
            SpriteBegin(SpriteSortMode.Deferred, blendState, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone);
        }
        public void SpriteBegin(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState)
        {
            SpriteBegin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, Global.Effects);
        }
        public void SpriteBegin(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState, Effect effect)
        {
            if (!spriteBegan)
            {
                spriteBatch.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect);
                spriteBegan = true;
                spriteBlend = blendState;
            }
        }
        public void SpriteEnd()
        {
            if (spriteBegan)
            {
                spriteBatch.End();
                spriteBegan = false;
                spriteEnds++;
            }
        }
        public void DrawSprite(Obj o, Texture2D tex, Vector2 pos, Color color, float rot, Vector2 origin, Vector2 scale, SpriteEffects effect, float depth)
        {
            if (spriteBegan)
            spriteBatch.Draw(tex, pos, null, color, MathHelper.ToRadians(rot), origin, scale, effect, depth);
        }
        public void DrawSprite(Texture2D tex, Vector2 pos, Color color, float rot, Vector2 origin, Vector2 scale, SpriteEffects effect, float depth)
        {
            if (spriteBegan)
            spriteBatch.Draw(tex, pos, null, color, MathHelper.ToRadians(rot), origin, scale, effect, depth);
        }
        public void DrawSprite(Texture2D tex, Vector3 pos, Color color, SpriteEffects effect)
        {
            DrawSprite(tex, pos, color, 0, Vector2.Zero, Vector2.One, effect);
        }
        public void DrawSprite(Texture2D tex, Vector3 pos, Color color, float rot, Vector2 origin, Vector2 scale, SpriteEffects effect)
        {
            if (spriteBegan)
                spriteBatch.Draw(tex, pos.ToVector2(), null, color, MathHelper.ToRadians(rot), origin, scale, effect, (pos.Z));
        }
        public void DrawSprite(Texture2D tex, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color)
        {
            if (spriteBegan)
                spriteBatch.Draw(tex, destinationRectangle, sourceRectangle, color);
        }
        public void DrawString(SpriteFont font, string text, Vector2 pos, Color color, float rotation, Vector2 origin, float scale, SpriteEffects spriteEffect)
        {
            if (spriteBegan)
                spriteBatch.DrawString(font, text, pos, color, rotation, origin, scale, spriteEffect, 0);
        }
        float rZ = -45;
        TimeSpan drawCounter = TimeSpan.Zero;
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            /*GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.CornflowerBlue);
            Global.Effects.MatrixTransform = Matrix.CreateOrthographicOffCenter(0, Global.ScreenSize.X, Global.ScreenSize.Y, 0, 0, 1);
            //Global.Effects.MatrixTransform = Matrix.CreateTranslation(0, 0, 0);
            Global.Effects.Technique = Techniques.Smoke;
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, Global.Effects, Matrix.CreateTranslation(0, 0, 0));
            
            foreach (var p in Global.Effects.CurrentTechnique.Passes)
            {
                p.Apply();
                spriteBatch.Draw(Textures.TimeIcon, Vector2.Zero, Color.Red);
            }
            
            spriteBatch.End();
            return;*/
            bool batch = batchSprites && !LevelBuilder.Active && !drawSecondShift;
            
            //batch = true;
            batch = false;
            objBlooming = objBlooming || Global.Effects.Blooming;
            spriteEnds = 0;
            drawTime += gameTime.ElapsedGameTime;
            if (LimitFramerate)
                drawCounter += gameTime.ElapsedGameTime;
            else drawCounter = gameTime.ElapsedGameTime;
            TimeSpan ts = targetDrawTime;
            Global.Effects.SetCameraValues(Global.Camera);

            int[] drawArray = null;
            if (!LimitFramerate || drawTime >= ts)
            {
                rZ += (-rZ) * 0.001f;
                GraphicsDevice.BlendState = BlendState.AlphaBlend;
                frameRates[frameIndex] = (float)(1d / drawCounter.TotalSeconds);
                frameIndex++;
                if (frameIndex >= frameRates.Length)
                    frameIndex = 0;

                while (drawTime >= ts && LimitFramerate)
                    drawTime -= ts;
                drawCounter = TimeSpan.Zero;
                //drawTime = TimeSpan.Zero;
                draws = 0;
                Global.Effects.SetValues();
                //GraphicsDevice.SetRenderTarget(null);
                if (Global.GameState != GameState.Paused)
                {
                    //Matrix m = Matrix.CreateOrthographicOffCenter(Global.Camera.View.X - (Global.Camera.CameraSize.X / 2), Global.Camera.View.X + (Global.Camera.CameraSize.X / 2), Global.Camera.View.Y + (Global.Camera.CameraSize.Y / 2), Global.Camera.View.Y - (Global.Camera.CameraSize.Y / 2), Global.Effects.NearZPlane, Global.Effects.FarZPlane);
                    Matrix m = Matrix.CreateTranslation(-Global.Camera.View.X, -Global.Camera.View.Y, 0) * Matrix.CreateOrthographic(Global.Camera.CameraSize.X, -Global.Camera.CameraSize.Y, Global.Effects.NearZPlane, Global.Effects.FarZPlane);
                    Global.Effects.MatrixTransform = m;

                    var rast = new RasterizerState()
                    {
                        FillMode = Microsoft.Xna.Framework.Graphics.FillMode.Solid,
                        CullMode = Microsoft.Xna.Framework.Graphics.CullMode.None,
                        MultiSampleAntiAlias = false
                    };
                    DepthStencilState depth = new DepthStencilState()
                    {
                        DepthBufferEnable = false,
                        DepthBufferWriteEnable = false,
                        StencilEnable = false
                    };

                    LightBloom bloom;
                    //Setting render targets
                    Global.Effects.Technique = Techniques.PlainNormal;
                    Global.Effects.CurrentTechnique.Passes[0].Apply();
                    Global.Effects.MatrixTransform = Matrix.CreateOrthographicOffCenter(0, screenTarget.Width, screenTarget.Height, 0, 0, 1);
                    //if (Global.Effects.Blurring || (objBlooming && Global.Effects.Quality > Quality.VeryLow) || (distort && Global.Effects.Quality > Quality.VeryLow))
                    {
                        GraphicsDevice.SetRenderTarget(depthTarget);
                        GraphicsDevice.Clear(new Color(1f, Global.Effects.BackgroundBloom, 0, 1f));
                        GraphicsDevice.SetRenderTarget(depthTarget2);
                        GraphicsDevice.Clear(new Color(1f, 0, 0, 1f));
                    }

                    GraphicsDevice.SetRenderTarget(screenTarget);
                    GraphicsDevice.Clear(SkyColor);
                    SpriteBegin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null);
                    Global.Effects.MatrixTransform = Matrix.CreateOrthographicOffCenter(0, screenTarget.Width, screenTarget.Height, 0, 0, 1);
                    Global.Effects.CurrentTechnique.Passes[0].Apply();
                    spriteBatch.Draw(skySprite, new Rectangle(0, 0, screenTarget.Width, screenTarget.Height), null, Color.White * 0.85f);
                    SpriteEnd();

                    Global.Effects.MatrixTransform = m;
                    //Global.Effects.MatrixTransform = Matrix.CreateOrthographicOffCenter(0 - (Global.Camera.CameraSize.X / 2), 0 + (Global.Camera.CameraSize.X / 2), 0 + (Global.Camera.CameraSize.Y / 2), 0 - (Global.Camera.CameraSize.Y / 2), 1000, int.MinValue);
                    //if (Global.Effects.Blurring || (objBlooming && Global.Effects.Quality > Quality.VeryLow) || (distort && Global.Effects.Quality > Quality.VeryLow))
                    {
                        GraphicsDevice.SetRenderTargets(screenTarget, depthTarget, depthTarget2);
                    }
                    Global.Drawer.Begin();
                    if (batch)
                    {
                        SpriteBegin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, depth, null);
                    }
                    else
                    {
                        SpriteBegin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, depth, rast);
                    }
                    Obj obj;
                    Obj lastObj = null;
                    //GraphicsDevice.DepthStencilState = DepthStencilState.None;
                    for (int i = 0; i < numObj; i++)
                    {
                        obj = objArray[i];
                        if (obj.Active && obj.Visible && obj.Texture != null && !obj.Texture.IsDisposed && obj.ScreenAlpha > 0 && obj.OnScreen)
                        {
                            draws++;
                            /*if (batch)
                            {
                                if (spriteBegan && lastObj != null && objArray[i].CheckIfSpriteBatchChangeNeeded(lastObj))
                                {
                                    SpriteEnd();
                                    SpriteBegin(objArray[i].BlendState);
                                }
                                else if (!spriteBegan)
                                {
                                    SpriteBegin(objArray[i].BlendState);
                                }
                            }
                            else if (lastObj != null && lastObj.AlwaysChangeSpriteBatchAfter)
                            {
                                SpriteEnd();
                                SpriteBegin(SpriteSortMode.Immediate, objArray[i].BlendState, SamplerState.LinearWrap, depth, rast);
                            }*/
                            objArray[i].DrawSecondShift();
                            //if (!(objArray[i] is LensFlare))
                            objArray[i].Draw();

                            if (LevelBuilder.Active)
                            {
                                if (LevelBuilder.ShowBoundingBoxes)
                                    objArray[i].DrawScreenRect();
                                if (objArray[i] == LevelBuilder.SelectedObj && LevelBuilder.EditMode)
                                {
                                    objArray[i].SelectedDraw();
                                }
                            }
                            lastObj = objArray[i];
                        }
                    }
                    drawArray = Global.Drawer.End();
                    SpriteEnd();
                    SpriteBegin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, rast, null);
                    //LightBloom bloom;
                    if (blooms.Count > 0 && Global.Effects.Quality >= Quality.Medium)
                    {
                        for (int i = 0; i < blooms.Count; i++)
                        {
                            bloom = blooms[i];
                            if (bloom.Drawable && (bloom != LevelBuilder.SelectedObj || !LevelBuilder.Active || !LevelBuilder.EditMode))
                            {
                                bloom.DrawObjects();
                                bloom.DrawLight(m);
                                //bloom.DrawFinal();
                            }
                        }
                    }
                    Global.Effects.MatrixTransform = m;
                    GraphicsDevice.SetRenderTargets(screenTarget, depthTarget, depthTarget2);
                    if (blooms.Count > 0 && Global.Effects.Quality >= Quality.Medium)
                    {
                        for (int i = 0; i < blooms.Count; i++)
                        {
                            bloom = blooms[i];
                            if (bloom.Drawable && (bloom != LevelBuilder.SelectedObj || !LevelBuilder.Active || !LevelBuilder.EditMode))
                            {
                                //bloom.DrawObjects();
                                //bloom.DrawLight(m);
                                bloom.DrawFinal();
                            }
                        }
                    }
                    GraphicsDevice.BlendState = BlendState.AlphaBlend;
                    /*GraphicsDevice.SetRenderTarget(screenTargetSmall);
                    Global.Effects.Technique = Techniques.PlainNormal;
                    Global.Effects.MatrixTransform = Matrix.CreateOrthographicOffCenter(0, Global.ScreenSize.X, Global.ScreenSize.Y, 0, 0, 1);
                    Global.Effects.CurrentTechnique.Passes[0].Apply();
                    spriteBatch.Draw(screenTarget, new Rectangle(0, 0, (int)Global.ScreenSize.X, (int)Global.ScreenSize.Y), null, Color.White);*/

                    if (Global.Effects.Blurring)
                    {
                        //Global.Effects.Parameters["blur"].SetValue(Global.Effects.Blur);
                        //GraphicsDevice.SamplerStates[0] = new SamplerState() { Filter = TextureFilter.Point, MaxMipLevel = 2, AddressU = TextureAddressMode.Clamp, AddressW = TextureAddressMode.Clamp, AddressV = TextureAddressMode.Clamp };
                        Global.Effects.MatrixTransform = Matrix.CreateOrthographicOffCenter(0, Global.ScreenSize.X, Global.ScreenSize.Y, 0, 0, 1);
                        Global.Effects.Technique = Techniques.Blur;
                        GraphicsDevice.SetRenderTarget(blurTargetX);
                        GraphicsDevice.Clear(Color.Green);

                        Global.Effects.DepthMap = depthTarget2;
                        //GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp
                        Global.Effects.CurrentTechnique.Passes[0].Apply();
                        spriteBatch.Draw(screenTarget, new Rectangle(0, 0, (int)Global.ScreenSize.X, (int)Global.ScreenSize.Y), null, Color.White);

                        GraphicsDevice.SetRenderTarget(blurTargetY);
                        GraphicsDevice.Clear(Color.Transparent);
                        Global.Effects.CurrentTechnique.Passes[1].Apply();
                        spriteBatch.Draw(blurTargetX, new Rectangle(0, 0, (int)Global.ScreenSize.X, (int)Global.ScreenSize.Y), null, Color.White);
                    }
                    if ((Global.Effects.Blooming || objBlooming) && Global.Effects.Quality > Quality.VeryLow)
                    {
                        //GraphicsDevice.SamplerStates[0] = new SamplerState() { Filter = TextureFilter.Point, MaxMipLevel = 2, AddressU = TextureAddressMode.Clamp, AddressV = TextureAddressMode.Clamp };
                        Global.Effects.MatrixTransform = Matrix.CreateOrthographicOffCenter(0, Global.ScreenSize.X, Global.ScreenSize.Y, 0, 0, 1);
                        //Global.Effects.Technique = (objBlooming) ? Techniques.ScreenObjectBloom : Techniques.ScreenBloom;
                        Global.Effects.Technique = Techniques.ScreenObjectBloom;

                        GraphicsDevice.SetRenderTarget(bloomTargetX);
                        GraphicsDevice.Clear(Color.Transparent);
                        Global.Effects.DepthMap = depthTarget;
                        Global.Effects.CurrentTechnique.Passes[0].Apply();
                        spriteBatch.Draw(screenTarget, new Rectangle(0, 0, (int)Global.ScreenSize.X, (int)Global.ScreenSize.Y), null, Color.White);
                        GraphicsDevice.SetRenderTarget(bloomTargetY);
                        GraphicsDevice.Clear(Color.Transparent);
                        //Global.Effects.Technique = Techniques.Normal;
                        Global.Effects.CurrentTechnique.Passes[1].Apply();
                        spriteBatch.Draw(bloomTargetX, new Rectangle(0, 0, (int)Global.ScreenSize.X, (int)Global.ScreenSize.Y), null, Color.White);

                    }

                    GraphicsDevice.SetRenderTarget(finalTarget);
                    Global.Effects.MatrixTransform = Matrix.CreateOrthographicOffCenter(0, Global.ScreenSize.X, Global.ScreenSize.Y, 0, 0, 1);


                    GraphicsDevice.Clear(Color.Black);
                    Global.Effects.Technique = Techniques.PlainNormal;
                    SpriteEnd();

                    SpriteBegin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null);

                    foreach (var pass in Global.Effects.CurrentTechnique.Passes)
                    {
                        //GraphicsDevice.RasterizerState = RasterizerState.
                        //GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
                        pass.Apply();
                        GraphicsDevice.BlendState = BlendState.AlphaBlend;
                        spriteBatch.Draw(screenTarget, new Rectangle(0, 0, (int)Global.ScreenSize.X, (int)Global.ScreenSize.Y), null, Color.White);
                        if (Global.Effects.Blurring)
                        {
                            spriteBatch.Draw(blurTargetY, new Rectangle(0, 0, (int)Global.ScreenSize.X, (int)Global.ScreenSize.Y), null, Color.White);
                        }
                        if (Global.Effects.Blooming || objBlooming)
                        {
                            //GraphicsDevice.BlendState = BlendState.Additive;
                            spriteBatch.Draw(bloomTargetY, new Rectangle(0, 0, (int)Global.ScreenSize.X, (int)Global.ScreenSize.Y), null, new Color(255, 255, 255, 255));
                        }
                    }
                    SpriteEnd();




                    //base.Draw(gameTime);
                }
            }
            SpriteBegin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone);
            
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);
#if MONO
            float hei = ((float)GraphicsDevice.Viewport.AspectRatio) * Global.ScreenSize.X;
            int y = (int)(hei - Global.ScreenSize.Y) / 2;
#else
            float hei = Global.ScreenSize.X / (float)GraphicsDevice.Viewport.AspectRatio;
            int y = (int)(hei - Global.ScreenSize.Y) / 2;
#endif
#if MONO
            if (Global.Orient == GameOrient.Left)
            {
                Global.Effects.MatrixTransform = Matrix.CreateOrthographicOffCenter(0, Global.ScreenSize.X, hei - y, -y, 0, 1) * Matrix.CreateRotationZ(MathHelper.ToRadians(270));
            }
            else
            {
                Global.Effects.MatrixTransform = Matrix.CreateOrthographicOffCenter(0, Global.ScreenSize.X, hei, 0, 0, 1) * Matrix.CreateRotationZ(MathHelper.ToRadians(0));
            }
#else
            Global.Effects.MatrixTransform = Matrix.CreateOrthographicOffCenter(0, Global.ScreenSize.X, hei - y, -y, 0, 1);

#endif

            Global.Effects.DepthMap = depthTarget;
            if (distort && Global.Effects.Quality > Quality.VeryLow)
                Global.Effects.Technique = Techniques.Distortion;
            else Global.Effects.Technique = Techniques.ScreenFinal;
            //Global.Effects.Technique = Techniques.Normal;
            foreach (var pass in Global.Effects.CurrentTechnique.Passes)
            {
                pass.Apply();
                spriteBatch.Draw(finalTarget, new Rectangle(0, 0, (int)Global.ScreenSize.X, (int)Global.ScreenSize.Y), null, Color.White);
                
            }
            Global.Effects.Technique = Techniques.PlainNormal;
            foreach (var pass in Global.Effects.CurrentTechnique.Passes)
            {
                pass.Apply();
                if (Global.HUDObj != null)
                    Global.HUDObj.DrawHUD(new Vector2(0, 0), 1);


#if MONO
                if (Global.GameState == GameState.Playing)
                spriteBatch.Draw(joystickTex, VirtualJoystick.Center, null, Color.White * 0.3f * VirtualJoystick.Alpha, 0, new Vector2(joystickTex.Width / 2, joystickTex.Height / 2), (Global.VirtualJoystick.Radius * 2) / joystickTex.Width, SpriteEffects.None, 0);
#endif
                if (Global.ShowFramerate)
                {

                    string s = "";
                    s += ats(s, (int)frameRates.Average() + " FPS, " + (int)updateRates.Average() + " UPS");
#if DEBUG
                    s = ats(s, numObj + " objects");
                    s = ats(s, "Game state: " + Global.GameState + ", Mouse Position: " + Controls.MousePos);
                    if (drawArray != null)
                        s = ats(s, "\nDraws: {" + drawArray[0] + ", " + drawArray[1] + " batched, " + (int)(((float)(drawArray[0] - drawArray[1]) / ((float)drawArray[0])) * 100f) + "% optimized"  + " }" + "\nOutput: " + Global.Output);
                    else s = ats(s, "\nDraws: {" + draws + (batch ? ", " + spriteEnds + " batched" : "") + " }" + "\nOutput: " + Global.Output);
                    //s = ats(s, "\nCPUs: " + cpuCount);
                    if (LevelBuilder.Active)
                        s = ats(s, LevelBuilder.ReturnDebugString());
                    s = ats(s, "Memory: " + GC.GetTotalMemory(false) / 1000000);

                    s = ats(s, "Screen Resolution: " + Graphics.ScreenResolution);
                    if (objBlooming)
                        s = ats(s, "Bloom enabled");
                    s = ats(s, "\nNetwork Logs: ");
                    {
                        foreach (var log in networkLogs)
                        {
                            s = ats(s, log);
                        }
                    }

#else
                    if (drawArray != null)
                        s = ats(s, (int)(((float)(drawArray[0] - drawArray[1]) / ((float)drawArray[0])) * 100f) + "% batching optimization");
#endif
                    if (s.Length > 0)
                        spriteBatch.DrawString(font1, s, new Vector2(12, 0), Color.Black);
                }


            }
            if (Global.Cutscene != null)
            {
                Global.Cutscene.Draw(spriteBatch, Global.Effects.MatrixTransform);
            }
            if (menuPanel.Opacity >= 0.01f)
            {
                menuPanel.Draw(new Rectangle(0, 0, (int)Global.ScreenSize.X, (int)Global.ScreenSize.Y), GraphicsDevice, graphics, spriteBatch, 1);
            }
            SpriteEnd();
            base.Draw(gameTime);
        }
        string ats(string s, string val)
        {
            if (s.Length > 0)
                s += "\n";
            s += val;
            return s;
        }
    }
}
