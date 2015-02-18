using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using SecondShiftMobile.Cutscenes;

namespace SecondShiftMobile.UI.PausePanels
{
    public class CutscenePicker : StackPanel
    {
        public CutscenePicker()
        {
            Padding = new Thickness(0, 0, 0, 24);
            try
            {
                var files = Directory.GetFiles("Content\\Cutscenes");
                //for (int i = 0; i < 5; i++)
                foreach (var f in files)
                {
                    string file = f.Split('\\').Last().Replace(".xml", "");
                    TextBlock tb = new TextBlock() { Text = file };
                    AddChild(tb);
                    tb.Clicked += delegate
                    {
                        Global.Output = "Loading cutscene: " + file;
                        var cut = Cutscene.Load(file);
                        Global.Cutscene = cut;
                        Global.GameState = GameState.Playing;
                    };
                }
            }
            catch
            {

            }
        }
    }
}
