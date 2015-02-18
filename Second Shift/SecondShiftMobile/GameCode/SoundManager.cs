using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondShiftMobile
{
    public static class SoundManager
    {
        static float soundEffectVolume, mediaPlayerVolume;
        public static float SoundEffectVolume
        {
            get
            {
                return soundEffectVolume;
            }
            set
            {
                soundEffectVolume = value;
#if MONO
                SoundEffect.MasterVolume = value * 0.2f;
#else
                SoundEffect.MasterVolume = value;
#endif
            }
        }
        public static float MediaPlayerVolume
        {
            get
            {
                return mediaPlayerVolume;
            }
            set
            {
#if WINDOWS_PHONE
                mediaPlayerVolume = value;
                MediaPlayer.Volume = (float)Math.Pow(value* 0.6, 0.1);
#else
                mediaPlayerVolume = MediaPlayer.Volume = value;
#endif
            }
        }
        public static bool IsRepeating
        {
            get
            {
                return MediaPlayer.IsRepeating;
            }
            set
            {
                MediaPlayer.IsRepeating = value;
            }
        }
        public static void init()
        {
            SoundEffectVolume = MediaPlayerVolume = 1;
        }

        public static void SetPan(this SoundEffectInstance sound, Vector3 pos)
        {
            Vector2 zPos = Global.Camera.GetScreenPosition(pos);
#if MONO
            sound.Pan = MathHelper.Clamp((zPos.X - Global.Camera.View.X) / (Global.Camera.CameraSize.X / 2), -1, 1) * 2;
#else
            sound.Pan = MathHelper.Clamp((zPos.X - Global.Camera.View.X) / (Global.Camera.CameraSize.X / 2), -1, 1);
#endif
        }
    }
}
