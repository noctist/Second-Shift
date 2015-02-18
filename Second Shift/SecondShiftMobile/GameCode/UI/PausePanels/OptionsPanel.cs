using System;
using System.Collections.Generic;
using System.Text;

namespace SecondShiftMobile.UI.PausePanels
{
    public class OptionsPanel :StackPanel
    {
        public TextPicker resolution, quality, showFPS, optimize;
        Resolutions[] resVals;
        Quality[] qVals;
        public OptionsPanel()
        {
            resolution = new TextPicker()
            {
                Title = "resolution"
            };
            resVals = (Resolutions[])Enum.GetValues(typeof(Resolutions));
            foreach (var v in resVals)
            {
                if ((int)v <= Graphics.ScreenResolution.Y)
                    resolution.AddItems(((int)v).ToString() + "p");
            }

            quality = new TextPicker()
            {
                Title = "quality"
            };
            qVals = (Quality[])Enum.GetValues(typeof(Quality));
            foreach (var q in qVals)
            {
                quality.AddItems(Effects.GetQualityName(q).ToLower());
            }

            showFPS = new TextPicker()
            {
                Title = "FPS"
            };
            showFPS.AddItems("show", "hide");
            optimize = new TextPicker() { Title = "optimize" };
            optimize.AddItems("yes", "no");
            if (!Global.Drawer.Optimize)
                optimize.SetIndexNoEvent(1);
            optimize.IndexChanged += optimize_IndexChanged;
            AddChild(resolution, quality, showFPS, optimize);

            resolution.IndexChanged += resolution_IndexChanged;

            quality.IndexChanged += quality_IndexChanged;

            showFPS.IndexChanged += showFPS_IndexChanged;

            quality.SetIndexNoEvent(qVals.IndexOf<Quality>(Global.Effects.Quality));
            resolution.SetIndexNoEvent(resVals.IndexOf(Global.Game.Resolution));
            showFPS.SetIndexNoEvent((Global.ShowFramerate) ? 0 : 1);
            Global.Effects.QualityChanged += Effects_QualityChanged;

            Global.Game.ResolutionChanged += Game_ResolutionChanged;
        }

        void optimize_IndexChanged(object sender, EventArgs e)
        {
            if (optimize.Index == 0)
            {
                Global.Drawer.Optimize = true;
            }
            else Global.Drawer.Optimize = false;
        }

        void showFPS_IndexChanged(object sender, EventArgs e)
        {
            Global.ShowFramerate = showFPS.Index == 0;
        }

        void Game_ResolutionChanged(object sender, EventArgs e)
        {
            resolution.SetIndexNoEvent(resVals.IndexOf(Global.Game.Resolution));
        }

        void Effects_QualityChanged(object sender, EventArgs e)
        {
            quality.SetIndexNoEvent(qVals.IndexOf<Quality>(Global.Effects.Quality));
        }
        protected override Microsoft.Xna.Framework.Vector2 MeasureOverride(Microsoft.Xna.Framework.Vector2 aS)
        {
            
            return base.MeasureOverride(aS);
        }
        void quality_IndexChanged(object sender, EventArgs e)
        {
            Global.Effects.Quality = qVals[quality.Index];
        }

        void resolution_IndexChanged(object sender, EventArgs e)
        {
            Global.Game.SetResolution(resVals[resolution.Index]);
        }
        protected override Microsoft.Xna.Framework.Vector2 ArrangeOverride(Microsoft.Xna.Framework.Vector2 availableSpace)
        {
            return base.ArrangeOverride(availableSpace);
        }
    }
}
