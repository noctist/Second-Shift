using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SecondShiftMobile.UI
{
    public enum TextWrapping { None, Wrap }
    public class TextBlock : UIElement
    {
        SpriteFont font = Fonts.UIDefault;
        string text = null;
        TextWrapping wrap = TextWrapping.None;
        string renderText = null;
        Thickness padding = new Thickness();
        public Thickness Padding
        {
            get
            {
                return padding;
            }
            set
            {
                padding = value;
                Changed("Padding");
                InvalidateMeasure();
            }
        }
        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                if (text != value)
                {
                    text = value;
                    Changed("Text");
                    InvalidateMeasure();
                }
            }
        }
        public SpriteFont Font
        {
            get
            {
                return font;
            }
            set
            {
                if (font != value)
                {
                    font = value;
                    Changed("Font");
                    InvalidateMeasure();
                }
            }
        }
        Vector2 fontSize = Vector2.Zero;
        Color foreground = Color.White;
        public Color Foreground
        {
            get
            {
                return foreground;
            }
            set
            {
                if (foreground != value)
                {
                    foreground = value;
                    Changed("Foreground");
                }
            }
        }
        public TextWrapping Wrap
        {
            get
            {
                return wrap;
            }
            set
            {
                if (wrap != value)
                {
                    wrap = value;
                    Changed("Wrap");
                }
            }
        }
        public TextBlock()
        {

        }
        protected override Vector2 MeasureOverride(Vector2 aS)
        {
            if (font != null && text != null)
            {
                if (wrap == TextWrapping.None)
                {
                    fontSize = font.MeasureString(text);
                    renderText = text;
                }
                else if (wrap == TextWrapping.Wrap)
                {
                    string s = "", lastValidString = "";
                    int lastValidIndex = 0;
                    Vector2 size;
                    for (int i = 0; i < text.Length; i++)
                    {
                        char c = text[i];
                        if (c == ' ')
                        {
                            lastValidIndex = i;
                            lastValidString = s;
                        }
                        s += text[i];
                        size = font.MeasureString(s);
                        if (size.X > aS.X)
                        {
                            s = lastValidString + "\n";
                            i = lastValidIndex;
                            continue;
                        }
                    }
                    fontSize = font.MeasureString(s);
                    renderText = s;
                }
                fontSize.X += Padding.Left + padding.Right;
                fontSize.Y += Padding.Bottom + padding.Top;
                return fontSize;
            }
            else return base.MeasureOverride(aS);
        }
        public override void Draw(Rectangle DrawRect, GraphicsDevice device, GraphicsDeviceManager graphics, SpriteBatch spriteBatch, float opacity = 1)
        {
            base.Draw(DrawRect, device, graphics, spriteBatch, opacity);
            if (font != null && text != null && renderText != null)
            {
                spriteBatch.DrawString(font, renderText, new Vector2(DrawRect.X + Padding.Left, DrawRect.Y + Padding.Top), Foreground * opacity * Opacity);
                //spriteBatch.Draw(Textures.WhiteBlock, DrawRect, Color.Blue);
            }
        }
    }
}
