using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace SecondShiftMobile.Cutscenes.Scenes
{
    public class FlatFloorScene : Cutscene
    {
        public FlatFloorScene(XElement x)
            : base(x)
        {

        }
        public override void ActionExecuted(CutsceneAction action)
        {
            if (action.Name == "TestEvent")
            {
                //new Explosion(Global.Game, 0, -2000, 0, 20, 0.1f, 180, 1, 60, 60);
            }
            base.ActionExecuted(action);
        }
    }
}
