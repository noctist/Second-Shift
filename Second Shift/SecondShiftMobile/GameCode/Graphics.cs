using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondShiftMobile
{
    public static class Graphics
    {
        static GraphicsDevice device;
        static GraphicsDeviceManager graphics;
        static GraphicsAdapter adapter;
        static Vector2 windowSize = Vector2.Zero;
        static Vector2 resolotion = Vector2.Zero;
        static bool fullScreen = false;
        public static Vector2 WindowSize
        {
            get
            {
                return (fullScreen) ? resolotion : windowSize;
            }
        }
        static Vector2 screenRes = Vector2.Zero;
        public static Vector2 ScreenResolution
        {
            get
            {
                return screenRes;
            }
        }
        public static void init(GraphicsDevice device, GraphicsDeviceManager graphics)
        {
            Graphics.device = device;
            Graphics.graphics = graphics;
            windowSize.X = device.PresentationParameters.BackBufferWidth; windowSize.Y = device.PresentationParameters.BackBufferHeight;
            resolotion.X = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width; resolotion.Y = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
#if WINDOWS_PHONE
            
            try
            {
                var size = (System.Windows.Size)Microsoft.Phone.Info.DeviceExtendedProperties.GetValue("PhysicalScreenResolution");
                screenRes = new Vector2((float)size.Height, (float)size.Width);
                Global.Output = "Res: " + size;
            }
            catch (Exception ex)
            {
                Global.Output = "Res error: " + ex;
                screenRes = new Vector2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
            }
            /*switch (res)
            {
                case RykenTube.Resolutions.WVGA:
                    screenRes = new Vector2(800, 480);
                    break;
                case RykenTube.Resolutions.WXGA:
                    screenRes = new Vector2(1280, 768);
                    break;
                case RykenTube.Resolutions.HD720p:
                    screenRes = new Vector2(1280, 720);
                    break;
                case RykenTube.Resolutions.HD1080p:
                    screenRes = new Vector2(1920, 1080);
                    break;
            }*/
#else
            screenRes = new Vector2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
#endif
        }
        
        public static void SetWindowSize(Vector2 size)
        {
            SetWindowSize((int)size.X, (int)size.Y);
        }
        public static void SetWindowSize(int X, int Y)
        {
            if (!fullScreen)
            {
                graphics.PreferredBackBufferWidth = X; graphics.PreferredBackBufferHeight = Y;
                graphics.ApplyChanges();
            }
            windowSize = new Vector2(X, Y);
        }
        public static void SetResolution(int X, int Y)
        {
            resolotion.X = X;
            resolotion.Y = Y;
            if (fullScreen)
            {
                graphics.PreferredBackBufferWidth = (int)resolotion.X;
                graphics.PreferredBackBufferHeight = (int)resolotion.Y;
                graphics.ApplyChanges();
            }
        }
        public static void ToggleFullscreen()
        {
            if (!fullScreen)
            {
                graphics.PreferredBackBufferWidth = (int)resolotion.X;
                graphics.PreferredBackBufferHeight = (int)resolotion.Y;
                graphics.IsFullScreen = true;
                fullScreen = true;
                Global.Output = "Full screen";
            }
            else
            {
                graphics.PreferredBackBufferWidth = (int)windowSize.X;
                graphics.PreferredBackBufferHeight = (int)windowSize.Y;
                graphics.IsFullScreen = false;
                fullScreen = false;
                Global.Output = "Not full screen";
            }
            graphics.ApplyChanges();
        }
    }
}
