using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using SecondShiftMobile.UI.PausePanels;
using SecondShiftMobile.UI.Animations;

namespace SecondShiftMobile.UI
{
    public class PauseMenu : Panel
    {
        StackPanel menuPanel, aboutPanel;
        TextBlock title, aboutText;
        TextBlock resume, options, scenes, quit, about;
        Panel optionsPanel;
        OptionsPanel opsPanel;
        ScrollViewer cutsceneScroll, aboutScroll;
        CutscenePicker cutscenePicker;
        Panel p = new Panel();
        public PauseMenu()
        {
            menuPanel = new StackPanel()
            {
                Margin = new Thickness(96, 300, 0, 0),
                Padding = new Thickness(0, 0, 0, 0),
                Width = 800
            };
            resume = new TextBlock() { Text = "resume" };
            options = new TextBlock() { Text = "options" };
            quit = new TextBlock() { Text = "quit" };
            scenes = new TextBlock() { Text = "scenes" };
            about = new TextBlock() { Text = "about" };

            aboutText = new TextBlock() { Wrap = TextWrapping.Wrap };
            string s = Global.Game.Content.Load<String>("Miscellaneous/About");
            aboutText.Text = s;

            menuPanel.AddChild(resume, options, scenes, about, quit);
            title = new TextBlock()
            {
                Font = Fonts.UITitle,
                Text = "paused",
                Margin = new Thickness(84, 0, 0, 0)
            };
            optionsPanel = new Panel()
            {
                Margin = new Thickness(960, 0, 48, 0)
            };
            p.Width = 800;
            p.Opacity = 0.3f;
            p.BackgroundColor = new Color(250, 100, 40);
            AddChild(p, optionsPanel, menuPanel, title);
            BackgroundColor = Color.Black * 0.75f;

            cutsceneScroll = new ScrollViewer();
            aboutScroll = new ScrollViewer();
            opsPanel = new OptionsPanel();
            cutscenePicker = new CutscenePicker();
            aboutPanel = new StackPanel();

            aboutPanel.AddChild(aboutText);
            aboutScroll.AddChild(aboutPanel);
            cutsceneScroll.AddChild(cutscenePicker);

            optionsPanel.AddChild(aboutScroll);
            options.Clicked += options_Clicked;
            scenes.Clicked += scenes_Clicked;
            quit.Clicked += quit_Clicked;
            about.Clicked += about_Clicked;
            resume.Clicked += resume_Clicked;
        }

        void resume_Clicked(object sender, MouseEventArgs e)
        {
            Global.GameState = GameState.Playing;
        }

        void about_Clicked(object sender, MouseEventArgs e)
        {
            SwitchChild(aboutScroll);
        }

        void quit_Clicked(object sender, MouseEventArgs e)
        {
            Global.Game.Exit();
        }

        void scenes_Clicked(object sender, MouseEventArgs e)
        {
            SwitchChild(cutsceneScroll);
            //optionsPanel.ClearChildren();
            //optionsPanel.AddChild(cutsceneScroll);
        }

        void options_Clicked(object sender, MouseEventArgs e)
        {
            SwitchChild(opsPanel);
        }

        async void SwitchChild(UIElement child)
        {
            FloatAnimation ani1 = new FloatAnimation(optionsPanel, "Opacity", 0) { FrameDuration = 10 };
            Vector2Animation ani2 = new Vector2Animation(optionsPanel, "Translation", new Vector2(0, 200)) { StartPower = 3, FrameDuration = 10 };
            AnimationManager.Add(ani1);
            AnimationManager.Add(ani2);
            await ani1.AwaitCompletion();
            await ani2.AwaitCompletion();
            optionsPanel.ClearChildren();
            optionsPanel.AddChild(child);
            optionsPanel.Translation = new Vector2(0, -200);
            ani1 = new FloatAnimation(optionsPanel, "Opacity", 1) { FrameDuration = 15 };
            ani2 = new Vector2Animation(optionsPanel, "Translation", new Vector2(0, 0)) { EndPower = 6, FrameDuration = 60 };
            AnimationManager.Add(ani1);
            AnimationManager.Add(ani2);
            await ani1.AwaitCompletion();
            await ani2.AwaitCompletion();
            
        }
        protected override Vector2 MeasureOverride(Vector2 availableSpace)
        {
            foreach (var c in children)
            {
                float width = availableSpace.X;
                if (c.Width != float.NaN)
                {
                    if (c.Width < width)
                    {
                        width = c.Width;
                    }
                }
                if (width + c.Margin.Left > availableSpace.X)
                {
                    width = availableSpace.X - c.Margin.Left;
                }

                float height = availableSpace.Y;
                if (c.Height != float.NaN)
                {
                    if (c.Height < height)
                    {
                        height = c.Height;
                    }
                }
                if (height + c.Margin.Top > availableSpace.Y)
                {
                    height = availableSpace.Y - c.Margin.Right;
                }
                width -= c.Margin.Right;
                c.Measure(new Vector2(width, height));
            }
            return new Vector2(Width, Height);
        }
        public void Animate()
        {
            title.Translation = optionsPanel.Translation = new Vector2(0, -200);
            optionsPanel.Opacity = 0;
            AnimationManager.Add(new Vector2Animation(title, "Translation", Vector2.Zero) { EndPower = 5, FrameDuration = 30, StartFrame = 0 });
            AnimationManager.Add(new Vector2Animation(optionsPanel, "Translation", Vector2.Zero) { EndPower = 5, FrameDuration = 60, StartFrame = 10 });
            AnimationManager.Add(new FloatAnimation(optionsPanel, "Opacity", 1) { FrameDuration = 30, StartFrame = 10 });
            float start = 0;
            foreach (var c in menuPanel.GetChildren())
            {
                c.Translation = new Vector2(0, -200);
                c.Opacity = 0;
                AnimationManager.Add(new Vector2Animation(c, "Translation", Vector2.Zero) { EndPower = 5, FrameDuration = 40, StartFrame = start });
                AnimationManager.Add(new FloatAnimation(c, "Opacity", 1) { FrameDuration = 30, StartFrame = start });
                start += 3.5f;
            }
        }
        protected override Vector2 ArrangeOverride(Vector2 availableSpace)
        {
            foreach (var c in children)
            {
                float width = availableSpace.X;
                if (c.Width != float.NaN)
                {
                    if (c.Width < width)
                    {
                        width = c.Width;
                    }
                }
                if (width + c.Margin.Left > availableSpace.X)
                {
                    width = availableSpace.X - c.Margin.Left;
                }

                float height = availableSpace.Y;
                if (c.Height != float.NaN)
                {
                    if (c.Height < height)
                    {
                        height = c.Height;
                    }
                }
                if (height + c.Margin.Top > availableSpace.Y)
                {
                    height = availableSpace.Y - c.Margin.Right;
                }
                width -= c.Margin.Right;
                c.Arrange(new Vector2(width, height), new Vector2(c.Margin.Left, c.Margin.Top));
            }
            return availableSpace;
        }
    }
}
