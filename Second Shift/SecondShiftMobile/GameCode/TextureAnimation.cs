using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondShiftMobile
{
    public enum TextureDirection { Horizontal, Vertical }
    public class TextureAnimation
    {

    }
    public class TextureFrame : IDisposable
    {
        public Vector2 TexturePos = Vector2.Zero;
        public Vector2 TextureScale = Vector2.One;
        public Texture2D Texture;
        int w, h;
        public int Width
        {
            get { return w; }
            set { w = value; }
        }
        public int Height { get { return h; } set { h = value; } }
        public bool IsDisposed { get { return Texture.IsDisposed; } }
        public static TextureFrame FromTexture(Texture2D tex)
        {
            return new TextureFrame() { Texture = tex, w = tex.Width, h = tex.Height };
        }
        public static TextureFrame FromAtlas(Texture2D tex, Vector2 frameOrigin, Vector2 frameSize, int frame, TextureDirection dir)
        {
            return FromAtlas(tex, frameOrigin, frameSize, new Vector2(tex.Width, tex.Height), frame, dir);
        }
        public static TextureFrame FromAtlas(Texture2D tex, Vector2 frameOrigin, Vector2 frameSize, Vector2 animationSize, int frame, TextureDirection dir)
        {
            var t = new TextureFrame() { Texture = tex, w = (int)frameSize.X, h = (int)frameSize.Y };
            switch (dir)
            {
                case TextureDirection.Horizontal:
                    int maxXFrames = (int)(animationSize.X / frameSize.X);
                    t.TextureScale = new Vector2(frameSize.X / (float)tex.Width, frameSize.Y / (float)tex.Height);
                    int xframe = (int)(((frameSize.X * frame) % animationSize.X) / frameSize.X);
                    int yframe = (int)(frameSize.X * frame / animationSize.X);
                    t.TexturePos = (frameOrigin + new Vector2(frameSize.X * xframe, frameSize.Y * yframe)) / new Vector2(tex.Width, tex.Height);
                    break;
            }
            return t;
        }
        public static implicit operator Texture2D(TextureFrame t)
        {
            return t.Texture;
        }

        public void Dispose()
        {
            Texture.Dispose();
        }
    }
}
