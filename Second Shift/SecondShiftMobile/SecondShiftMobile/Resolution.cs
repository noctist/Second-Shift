using Microsoft.Phone.Info;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SecondShiftMobile;

namespace RykenTube
{
    public enum Resolutions { WVGA, WXGA, HD720p, HD1080p };

    public static class ResolutionHelper
    {
        static private Size _size;

        private static bool IsWvga
        {
            get
            {
                return App.Current.Host.Content.ScaleFactor == 100;
            }
        }

        private static bool IsWxga
        {
            get
            {
                return App.Current.Host.Content.ScaleFactor == 160;
            }
        }

        private static bool Is720p
        {
            get
            {
                return (App.Current.Host.Content.ScaleFactor == 150 && !Is1080p);
            }
        }

        private static bool Is1080p
        {
            get
            {
                if (_size.Width == 0)
                {
                    try
                    {
                        _size = (Size)DeviceExtendedProperties.GetValue("PhysicalScreenResolution");
                    }
                    catch (Exception)
                    {
                        _size.Width = 0;
                    }
                }
                return _size.Width == 1080;
            }
        }

        public static Resolutions CurrentResolution
        {
            get
            {
                if (IsWvga) return Resolutions.WVGA;
                else if (IsWxga) return Resolutions.WXGA;
                else if (Is720p) return Resolutions.HD720p;
                else if (Is1080p) return Resolutions.HD1080p;
                else throw new InvalidOperationException("Unknown resolution");
            }
        }
    }
    class ScreenSizeHelper
    {
        static private double _screenSize = -1.0f;
        static private double _screenDpiX = 0.0f;
        static private double _screenDpiY = 0.0f;
        static private Size _resolution;

        static public bool IsBigScreen
        {
            get
            {
                // Use 720p emulator to simulate big screen.
                if (Microsoft.Devices.Environment.DeviceType == Microsoft.Devices.DeviceType.Emulator)
                {
                    _screenSize = (App.Current.Host.Content.ScaleFactor == 150) ? 6.0f : 0.0f;
                }

                if (_screenSize == -1.0f)
                {
                    try
                    {
                        _screenDpiX = (double)DeviceExtendedProperties.GetValue("RawDpiX");
                        _screenDpiY = (double)DeviceExtendedProperties.GetValue("RawDpiY");
                        _resolution = (Size)DeviceExtendedProperties.GetValue("PhysicalScreenResolution");

                        // Calculate screen diagonal in inches.
                        _screenSize =
                            Math.Sqrt(Math.Pow(_resolution.Width / _screenDpiX, 2) +
                                      Math.Pow(_resolution.Height / _screenDpiY, 2));
                    }
                    catch (Exception e)
                    {
                        // We're on older software with lower screen size, carry on.
                        //Debug.WriteLine("IsBigScreen error: " + e.Message);
                        _screenSize = 0;
                    }
                }

                // Returns true if screen size is bigger than 5 inches - you may edit the value based on your app's needs.
                return (_screenSize > 5.0f);
            }
        }
    }
}
