using Microsoft.Xna.Framework;
using SecondShiftMobile.UI.Animations;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondShiftMobile.UI
{
    public class Panel : UIElement
    {
        protected List<UIElement> children;
        public Panel()
        {
            children = new List<UIElement>();
        }
        public UIElement[] GetChildren()
        {
            return children.ToArray();
        }
        public void AddChild(params UIElement[] child)
        {
            foreach (var c in child)
            {
                children.Add(c);
            }
            if (FirstUpdateRun)
            {
                InvalidateMeasure();
            }
        }

        public void ClearChildren()
        {
            children.Clear();
        }
        
        public override void Update()
        {
            base.Update();
            bool needsUpdate = false;
            for (int i = 0; i < children.Count; i++)
            {
                children[i].Update();
                if (children[i].NeedsLayoutUpdate)
                {
                    needsUpdate = true;
                    break;
                }
            }
            if (needsUpdate)
                InvalidateMeasure();
        }
        protected override Vector2 MeasureOverride(Vector2 aS)
        {
            foreach (var c in children)
            {
                c.Measure(aS);
            }
            return base.MeasureOverride(aS);
        }
        protected override Vector2 ArrangeOverride(Vector2 availableSpace)
        {
            Vector2 returnSize = new Vector2(availableSpace.X, availableSpace.Y);
            foreach (var c in children)
            {
                c.Arrange(availableSpace, new Vector2(0));
                if (c.RealSize.X + c.Position.X > returnSize.X)
                    returnSize.X = c.RealSize.X + c.Position.X;
                if (c.RealSize.Y + c.Position.Y > returnSize.Y)
                    returnSize.Y = c.RealSize.Y + c.Position.Y;
            }
            return returnSize;
            return base.ArrangeOverride(returnSize);
        }
        public virtual Vector2 GetDrawTranslation()
        {
            return Translation;
        }
        public override void Draw(Microsoft.Xna.Framework.Rectangle DrawRect, Microsoft.Xna.Framework.Graphics.GraphicsDevice device, Microsoft.Xna.Framework.GraphicsDeviceManager graphics, Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, float opacity = 1)
        {
            base.Draw(DrawRect, device, graphics, spriteBatch, opacity);
            for (int i = 0; i < children.Count; i++)
            {
                var c = children[i];
                Vector2 pos = c.Position + new Vector2(DrawRect.X, DrawRect.Y) + GetDrawTranslation() + c.Translation;
                //pos += new Vector2(200, 200);
                Rectangle r = new Rectangle((int)pos.X, (int)pos.Y, (int)c.RealSize.X, (int)c.RealSize.Y);
                c.Draw(r, device, graphics, spriteBatch, Opacity * opacity);
            }
        }
    }
}
