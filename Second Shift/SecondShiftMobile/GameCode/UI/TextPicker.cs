using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondShiftMobile.UI
{
    public class TextPicker : Panel
    {
        TextBlock title, leftChevron, valueText, rightChevron;
        int index = 0;
        public int Index
        {
            get
            {
                return index;
            }
            set
            {
                if (index != value)
                {
                    index = value;
                    Changed("Index");
                    if (options.Count > 0)
                        valueText.Text = options[index];
                    if (IndexChanged != null)
                        IndexChanged(this, new EventArgs());
                }
            }
        }
        public string Title
        {
            get
            {
                return title.Text;
            }
            set
            {
                title.Text = "| " + value;
                Changed("Title");
            }
        }
        public event EventHandler IndexChanged;
        List<string> options;
        public TextPicker()
        {
            options = new List<string>();
            title = new TextBlock()
            {
                Text = ""
            };
            leftChevron = new TextBlock()
            {
                Text = ">"
            };
            rightChevron = new TextBlock()
            {
                Text = " |"
            };
            valueText = new TextBlock()
            {
                Text = ""
            };
            AddChild(title, leftChevron, rightChevron, valueText);
        }

        public override void OnClicked(MouseEventArgs e)
        {
            ChangeIndex(1);
            base.OnClicked(e);
        }
        public void SetIndexNoEvent(int ind)
        {
            int i = ind;
            index = ((i % options.Count) + options.Count) % options.Count;
            if (options.Count > 0)
                valueText.Text = options[index];
        }
        public void AddItems(params string[] objects)
        {
            foreach (var o in objects)
            {
                options.Add(o);
            }
            if (options.Count > 0)
                valueText.Text = options[index];
        }
        public void ChangeIndex(int byHowMuch)
        {
            int i = index + byHowMuch;
            Index = ((i % options.Count) + options.Count) % options.Count;
            if (options.Count > 0)
                valueText.Text = options[index];
        }
        protected override Microsoft.Xna.Framework.Vector2 MeasureOverride(Vector2 aS)
        {
            Vector2 vec = Vector2.Zero;
            vec.X = aS.X;
            foreach (var c in children)
            {
                c.Measure(aS);
                if (c.DesiredSize.Y > vec.Y)
                    vec.Y = c.DesiredSize.Y;
            }
            return vec;
        }
        protected override Vector2 ArrangeOverride(Vector2 availableSpace)
        {
            float wid = availableSpace.X;
            float hei = availableSpace.Y;
            if (float.IsNaN(wid) || availableSpace.X > 1000)
            {
                wid = 400;
            }
            if (float.IsNaN(hei))
            {
                hei = DesiredSize.Y;
            }
            title.Arrange(title.DesiredSize, new Vector2(0, 0));
            rightChevron.Arrange(rightChevron.DesiredSize, new Vector2(wid - rightChevron.DesiredSize.X, 0));
            valueText.Arrange(valueText.DesiredSize, new Vector2(rightChevron.Position.X - valueText.DesiredSize.X, 0));

            float leftC = ((wid - leftChevron.DesiredSize.X) / 2f);
            if (leftC < title.Position.X + title.RealSize.X + 12)
                leftC = title.Position.X + title.RealSize.X + 12;
            leftChevron.Arrange(leftChevron.DesiredSize, new Vector2(leftC, 0));
            return new Vector2(wid, hei);
        }
    }
}
