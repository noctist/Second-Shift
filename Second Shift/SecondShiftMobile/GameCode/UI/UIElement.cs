using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SecondShiftMobile.UI
{
    public abstract class UIElement : INotifyPropertyChanged
    {
        float width = float.NaN;
        float height = float.NaN;
        float opacity = 1;
        Vector2 position = Vector2.Zero;
        Vector2 translation = Vector2.Zero;
        Vector2 desiredSize = Vector2.Zero;
        Vector2 realSize = Vector2.Zero;
        Thickness margin = new Thickness(0);
        Rectangle screenRectangle = Rectangle.Empty;
        public event MouseEventHandler Clicked;
        bool measured = false, arranged = false;
        Color background = Color.Transparent;
        Vector2 lastTouchPos = Vector2.Zero;
        Vector2 startTouchPos = new Vector2(-100);
        bool touchMoved = false;
        bool touchStartedOnMe = false;
        bool touchStarted = false;
        public Color BackgroundColor
        {
            get
            {
                return background;
            }
            set
            {
                if (background != value)
                {
                    background = value;
                    Changed("Background");
                }
            }
        }
        public bool NeedsLayoutUpdate
        {
            get { return !measured || !arranged; }
        }
        public Thickness Margin
        {
            get
            {
                return margin;
            }
            set
            {
                if (margin != value)
                {
                    margin = value;
                    Changed("Margin");
                    InvalidateMeasure();
                }
            }
        }

        public Vector2 DesiredSize
        {
            get
            {
                return desiredSize;
            }
            private set
            {
                if (desiredSize != value)
                {
                    desiredSize = value;
                    Changed("DesiredSize");
                }
            }
        }

        public Vector2 RealSize
        {
            get
            {
                return realSize;
            }
            private set
            {
                if (realSize != value)
                {
                    realSize = value;
                    Changed("RealSize");
                }
            }
        }

        public Vector2 Position
        {
            get
            {
                return position;
            }
            private set
            {
                if (position != value)
                {
                    position = value;
                    Changed("Position");
                }
            }
        }

        public Vector2 Translation
        {
            get
            {
                return translation;
            }
            set
            {
                if (translation != value)
                {
                    translation = value;
                    Changed("Translation");
                }
            }
        }
        
        public float Width
        {
            get
            {
                return width;
            }
            set
            {
                if (width != value)
                {
                    width = value;
                    Changed("Width");
                    InvalidateMeasure();
                }
            }
        }
        public float Height
        {
            get 
            {
                return height;
            }
            set 
            {
                if (height != value)
                {
                    height = value;
                    Changed("Height");
                    InvalidateMeasure();
                }
            }
        }
        public float Opacity
        {
            get
            {
                return opacity;
            }
            set
            {
                if (opacity != value)
                {
                    opacity = value;
                    Changed("Opacity");
                }
            }
        }
        bool firstUpdate = false;
        public bool FirstUpdateRun
        {
            get
            {
                return firstUpdate;
            }
        }
        public UIElement()
        {

        }
        public virtual void Update()
        {
            firstUpdate = true;
            bool checkMouse = true;
            Vector2 mousePos = Controls.MousePos;
            MouseType mouseType = MouseType.Mouse;
            bool touching = Controls.MouseLeft == ControlState.Held || Controls.MouseLeft == ControlState.Pressed || Touch.States[0] == TouchScreen.Pressed || Touch.States[0] == TouchScreen.Moved;
            touching = touching || Controls.MouseLeft == ControlState.Released || Touch.States[0] == TouchScreen.Released;
            if (Touch.States[0] == TouchScreen.None)
            {
                
            }
            else
            {
                mouseType = MouseType.Touch;
                mousePos = Touch.MoveLocation2[0];
            }
            if (screenRectangle.Contains((int)mousePos.X, (int)mousePos.Y))
            {
                if (!touchStartedOnMe || touchMoved)
                {
                    if ((Controls.MouseLeft == ControlState.Pressed || Touch.States[0] == TouchScreen.Pressed))
                    {
                        touchMoved = false;
                        touchStartedOnMe = true;
                        startTouchPos = mousePos;
                        lastTouchPos = mousePos;
                    }
                    else
                    {
                        touchStartedOnMe = false;
                    }
                }
                if (!touchStarted)
                {
                    startTouchPos = mousePos;
                    lastTouchPos = mousePos;
                    touchStarted = true;
                }
                if (!touchMoved)
                {
                    touchMoved = (startTouchPos - mousePos).Length() > 36;
                }
                MouseEventArgs args = new MouseEventArgs()
                {
                    MousePosition = mousePos,
                    RelativePosition = new Vector2(mousePos.X - screenRectangle.X, mousePos.Y - screenRectangle.Y),
                    MouseType = mouseType,
                    IsDown = touching,
                    PositionOffset = mousePos - lastTouchPos
                };
                if (touchMoved && lastTouchPos != mousePos)
                {
                    OnMouseMoved(args);
                }
                if (this is TextPicker)
                {
                    //Global.Output = touchMoved;
                }
                if (Controls.MouseLeft == ControlState.Pressed || Touch.States[0] == TouchScreen.Pressed)
                {
                    OnDown(args);
                }
                else if (Controls.MouseLeft == ControlState.Held || Touch.States[0] == TouchScreen.Moved)
                {

                }
                else if ((Controls.MouseLeft == ControlState.Released || Touch.States[0] == TouchScreen.Released))
                {
                    OnUp(args);
                    if (!touchMoved && touchStartedOnMe)
                    {
                        OnClicked(args);
                        if (Clicked != null)
                        {
                            Clicked(this, args);
                        }
                    }
                }
                lastTouchPos = mousePos;
            }
            else
            {
                touchStartedOnMe = false;
                touchMoved = false;
                startTouchPos = new Vector2(-100);
                if (touchStarted)
                {
                    OnLeave(new MouseEventArgs()
                    {

                    });
                }
                touchStarted = false;
            }
        }
        public virtual void OnDown(MouseEventArgs e)
        {

        }
        public virtual void OnUp(MouseEventArgs e)
        {

        }
        public virtual void OnLeave(MouseEventArgs e)
        {

        }
        public virtual void OnClicked(MouseEventArgs e)
        {
            
        }
        public virtual void OnMouseMoved(MouseEventArgs e)
        {

        }
        public void UpdateLayout()
        {
            InvalidateMeasure();
            if (firstUpdate)
            {
                Measure(DesiredSize);
                Arrange(DesiredSize, position);
            }
        }
        public void InvalidateMeasure()
        {
            measured = false; arranged = false;
        }
        public void Measure(Vector2 availableSpace)
        {
            if (!float.IsNaN(width))
                availableSpace.X = width;
            if (!float.IsNaN(height))
                availableSpace.Y = height;
            DesiredSize = MeasureOverride(availableSpace);
            arranged = false;
            measured = true;
        }
        protected virtual Vector2 MeasureOverride(Vector2 availableSpace)
        {
            return new Vector2(width, height);
        }
        public void Arrange(Vector2 availableSpace, Vector2 position)
        {
            Position = position;
            RealSize = ArrangeOverride(availableSpace);
            arranged = true;
        }
        protected virtual Vector2 ArrangeOverride(Vector2 availableSpace)
        {
            return availableSpace;
        }
        public virtual void Draw(Rectangle DrawRect, GraphicsDevice device, GraphicsDeviceManager graphics, SpriteBatch spriteBatch, float opacity = 1)
        {
            screenRectangle = DrawRect;
            spriteBatch.Draw(Textures.WhiteBlock, DrawRect, BackgroundColor * Opacity * opacity);
        }
        public virtual void Changed(string Property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(Property));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
