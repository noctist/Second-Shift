using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondShiftMobile.UI
{
    public class StackPanel : Panel
    {
        Thickness padding = new Thickness(0, 0, 0, 0);
        public Thickness Padding
        {
            get
            {
                return padding;
            }
            set
            {
                if (padding != value)
                {
                    padding = value;
                    Changed("Padding");
                    InvalidateMeasure();
                }
            }
        }
        public StackPanel()
        {

        }
        protected override Microsoft.Xna.Framework.Vector2 MeasureOverride(Vector2 aS)
        {
            Vector2 pos = Vector2.Zero;
            Vector2 size = aS;
            if (!float.IsNaN(Width))
            {
                size.X = Width;
            }
            foreach (var c in children)
            {
                c.Measure(size);
                pos.Y += c.DesiredSize.Y + padding.Top + padding.Bottom + c.Margin.Top;
                float wid = padding.Left + c.Margin.Left + c.DesiredSize.X;
                if (wid > pos.X)
                {
                    pos.X = wid;
                }
            }
            return pos;
        }
        protected override Microsoft.Xna.Framework.Vector2 ArrangeOverride(Microsoft.Xna.Framework.Vector2 availableSpace)
        {
            Vector2 returnSize = new Vector2();
            Vector2 pos = Vector2.Zero;
            foreach (var c in children)
            {
                pos.Y += padding.Top;
                c.Arrange(c.DesiredSize + new Vector2(c.Margin.Left + c.Margin.Right, c.Margin.Top + c.Margin.Bottom), new Vector2(padding.Left + c.Margin.Left, pos.Y + c.Margin.Top));
                pos.Y += padding.Bottom + c.RealSize.Y;
                if (c.RealSize.X > pos.X)
                    pos.X = c.RealSize.X;
            }
            return pos;
        }
    }
}
