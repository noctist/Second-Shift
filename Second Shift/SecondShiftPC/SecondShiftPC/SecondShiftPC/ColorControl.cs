using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
//using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SecondShiftMobile
{
    
    public partial class ColorControl : UserControl
    {
        ColorControlValueType type = ColorControlValueType.Color;
        public event EventHandler<ColorChangedEventArgs> ColorChanged;
        public event EventHandler<Vector4ChangedEventArgs> Vector4Changed;
        public ColorControl()
        {
            InitializeComponent();
        }
        float min = 0, max = 1;
        public void SetColor(Color c)
        {
            if (type == ColorControlValueType.Color)
            {
                rBar.Value = c.R; gBar.Value = c.G; bBar.Value = c.B; aBar.Value = c.A;
            }
        }
        public void SwitchToVector4(float min, float max)
        {
            type = ColorControlValueType.Vector4;
            this.min = min; this.max = max;

        }
        public void SetVector4(Vector4 v)
        {
            if (type == ColorControlValueType.Vector4)
            {
                rBar.Value = (int)MyMath.Between(0, 255, MyMath.BetweenValue(min, max, v.X));
                gBar.Value = (int)MyMath.Between(0, 255, MyMath.BetweenValue(min, max, v.Y));
                bBar.Value = (int)MyMath.Between(0, 255, MyMath.BetweenValue(min, max, v.Z));
                aBar.Value = (int)MyMath.Between(0, 255, MyMath.BetweenValue(min, max, v.W));
            }
        }

        public void SetValues(int val)
        {
            rBar.Value = gBar.Value = bBar.Value = aBar.Value = val;
        }

        private void colorChanged(object sender, EventArgs e)
        {
            if (type == ColorControlValueType.Color)
            {
                Color c = new Color(rBar.Value, gBar.Value, bBar.Value, aBar.Value);
                rText.Text = rBar.Value.ToString();
                gText.Text = gBar.Value.ToString();
                bText.Text = bBar.Value.ToString();
                aText.Text = aBar.Value.ToString();
                if (ColorChanged != null)
                {
                    ColorChanged(this, new ColorChangedEventArgs() { Color = c });
                }
                colorLabel.BackColor = System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B);
            }
            else if (type == ColorControlValueType.Vector4)
            {
                Vector4 val = new Vector4(
                    MyMath.Between(min, max, MyMath.BetweenValue(0, 255, rBar.Value)),
                    MyMath.Between(min, max, MyMath.BetweenValue(0, 255, gBar.Value)),
                    MyMath.Between(min, max, MyMath.BetweenValue(0, 255, bBar.Value)),
                    MyMath.Between(min, max, MyMath.BetweenValue(0, 255, aBar.Value))
                    );
                rText.Text = val.X.ToString();
                gText.Text = val.Y.ToString();
                bText.Text = val.Z.ToString();
                aText.Text = val.W.ToString();
                if (Vector4Changed != null)
                {
                    Vector4Changed(this, new Vector4ChangedEventArgs() { Value = val });
                }
            }
        }
    }
    public enum ColorControlValueType
    {
        Color, Vector4, Vector3
    }
    public class ColorChangedEventArgs : EventArgs
    {
        public Color Color;
    }
    public class Vector4ChangedEventArgs : EventArgs
    {
        public Vector4 Value;
    }
}
