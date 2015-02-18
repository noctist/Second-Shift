using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondShiftMobile.UI
{
    public class ScrollViewer : Panel
    {
        int velocitiesIndex = 0;
        const int velocitiesLength = 5;
        Vector2[] velocities = new Vector2[velocitiesLength];
        float scrollBarOpacity = 0;
        float scrollBarTimer = 0;
        const float maxScrollBarTimer = 120;
        float horizontalOffset = 0, verticalOffset = 0;
        bool horizontalScrollEnabled = false, verticalScrollEnabled = true;
        public bool HorizontalScrollEnabled
        {
            get { return horizontalScrollEnabled; }
            set
            {
                if (horizontalScrollEnabled != value)
                {
                    horizontalScrollEnabled = value;
                    Changed("HorizontalScrollEnabled");
                }
            }
        }
        public bool VerticalScrollEnabled
        {
            get { return verticalScrollEnabled; }
            set
            {
                if (verticalScrollEnabled != value)
                {
                    verticalScrollEnabled = value;
                    Changed("VerticalScrollEnabled");
                }
            }
        }
        public float HorizontalOffset
        {
            get
            {
                return horizontalOffset;
            }
            set
            {
                if (horizontalOffset != value)
                {
                    horizontalOffset = value;
                    Changed("HorizontalOffset");
                }
            }
        }
        public float VerticalOffset
        {
            get
            {
                return verticalOffset;
            }
            set
            {
                if (verticalOffset != value)
                {
                    verticalOffset = value;
                    Changed("VerticalOffset");
                }
            }
        }
        public Vector2 offset = Vector2.Zero;
        float verticelVel = 0, horizontalVel = 0;
        bool touching = false;
        Vector2 childrenSize = new Vector2();
        const int scrollBarWidth = 10;
        float scrollBarHeight = 100;
        float scrollableX = 0, scrollableY = 0;

        public ScrollViewer()
        {
            ResetVelocities();
        }
        public override void Update()
        {
            base.Update();
            float renderOffsetY = 0;
            float renderOffsetX = 0;
            if (!touching)
            {
                VerticalOffset += verticelVel;
                verticelVel *= 0.95f;

                HorizontalOffset += horizontalVel;
                horizontalVel *= 0.95f;
            }
            else
            {
                
            }
            scrollableY = RealSize.Y - childrenSize.Y;
            scrollableX = RealSize.X - childrenSize.X;
            if (!touching)
            {
                if (verticalOffset > 0)
                    verticalOffset += -verticalOffset * 0.1f;

                if (horizontalOffset > 0)
                    horizontalOffset += -horizontalOffset * 0.1f;

                if (scrollableY < 0 && verticalOffset < scrollableY)
                {
                    Helper.Write("Scrolling back up, Children Size: " + childrenSize + ", Real Size: " + RealSize + ", Offet: " + verticalOffset + ", Remaining Y: " + scrollableY); 
                    verticalOffset += ((scrollableY) - verticalOffset) * 0.1f;
                }

                if (scrollableX < 0 && horizontalOffset < scrollableX)
                {
                    Helper.Write("Scrolling back left, Children Size: " + childrenSize + ", Real Size: " + RealSize + ", Offet: " + horizontalOffset + ", Remaining X: " + scrollableX);
                    horizontalOffset += ((scrollableX) - horizontalOffset) * 0.1f;
                }
            }
            renderOffsetY = verticalOffset;
            renderOffsetX = horizontalOffset;
            if (renderOffsetY > 0)
            {
                renderOffsetY = (float)Math.Pow(renderOffsetY, 0.9);
                verticelVel *= 0.8f;
            }
            if (renderOffsetY < scrollableY)
            {
                renderOffsetY = scrollableY - (float)Math.Pow(scrollableY - renderOffsetY, 0.9);
                verticelVel *= 0.8f;
            }

            if (renderOffsetX > 0)
            {
                renderOffsetX = (float)Math.Pow(renderOffsetX, 0.9);
                horizontalVel *= 0.8f;
            }
            if (renderOffsetX < scrollableX)
            {
                renderOffsetX = scrollableX - (float)Math.Pow(scrollableX - renderOffsetX, 0.9);
                horizontalVel *= 0.8f;
            }

            offset.X = renderOffsetX;
            offset.Y = renderOffsetY;
            if (touching)
            {
                scrollBarOpacity += 0.1f;
                scrollBarTimer = 0;
            }
            else
            {
                if (Math.Abs(horizontalVel) < 1 && Math.Abs(verticelVel) < 1)
                {
                    if (scrollBarTimer < maxScrollBarTimer)
                        scrollBarTimer++;
                    else
                    {
                        scrollBarOpacity -= 0.05f;
                    }
                }
            }
            scrollBarOpacity = MathHelper.Clamp(scrollBarOpacity, 0, 1);
        }
        public override void OnMouseMoved(MouseEventArgs e)
        {
            if (!e.IsDown)
                return;
            if (!touching)
            {
                ResetVelocities();
                touching = true;
            }
            Vector2 vel = new Vector2();
            if (verticalScrollEnabled)
            {
                VerticalOffset += e.PositionOffset.Y;
                vel.Y = e.PositionOffset.Y;
            }

            if (horizontalScrollEnabled)
            {
                HorizontalOffset += e.PositionOffset.X;
                vel.X = e.PositionOffset.X;
            }
            AddVelocity(vel);
            base.OnMouseMoved(e);
        }
        public override void OnLeave(MouseEventArgs e)
        {
            ApplyVelocities();
            touching = false;
            base.OnLeave(e);
        }
        public override void OnUp(MouseEventArgs e)
        {
            ApplyVelocities();
            touching = false;
            base.OnUp(e);
        }
        void AddVelocity(Vector2 vel)
        {
            velocities[velocitiesIndex & velocitiesLength] = vel;
            velocitiesIndex++;

            if (velocitiesIndex >= velocitiesLength)
                velocitiesIndex = 0;
        }
        void ApplyVelocities()
        {
            Vector2 vec = new Vector2();
            foreach (var v in velocities)
            {
                if (v.LengthSquared() > vec.LengthSquared())
                 vec = v;
            }
            //vec /= velocitiesLength;
            horizontalVel = vec.X;
            verticelVel = vec.Y;
        }
        void ResetVelocities()
        {
            for (int i = 0; i < velocitiesLength; i++)
                velocities[i] = Vector2.Zero;
            velocitiesIndex = 0;
        }
        public override Microsoft.Xna.Framework.Vector2 GetDrawTranslation()
        {
            return base.GetDrawTranslation() + offset;
        }
        protected override Vector2 ArrangeOverride(Vector2 availableSpace)
        {
            childrenSize = base.ArrangeOverride(availableSpace);
            scrollBarHeight = (availableSpace.Y / childrenSize.Y) * availableSpace.Y;
            return availableSpace;
        }
        public override void Draw(Rectangle DrawRect, Microsoft.Xna.Framework.Graphics.GraphicsDevice device, GraphicsDeviceManager graphics, Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, float opacity = 1)
        {
            base.Draw(DrawRect, device, graphics, spriteBatch, opacity);
            spriteBatch.Draw(Textures.WhiteBlock, new Rectangle(DrawRect.X + DrawRect.Width - scrollBarWidth, DrawRect.Y + (int)((verticalOffset / scrollableY) * (DrawRect.Height - scrollBarHeight)), scrollBarWidth, (int)scrollBarHeight), Color.White * 0.3f * scrollBarOpacity);
        }
    }
}
