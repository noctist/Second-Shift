using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace SecondShiftMobile.Cutscenes.Scenes
{
    public class BatmanScene : Cutscene
    {
        Enemies.Joker joker;
        Player batman;
        bool close = false;
        bool lookForObj = false;
        public BatmanScene()
            :base()
        {

        }
        public BatmanScene(XElement x)
            : base(x)
        {
            joker = null;
            batman = null;
        }
        public override void Update()
        {
            base.Update();
            if (lookForObj)
            {
                if (joker == null)
                {
                    joker = Global.Game.FindObject<Enemies.Joker>();
                }
                if (batman == null)
                {
                    batman = Global.Game.FindObject<Player>();
                }
            }
            if (batman != null && joker != null)
            {
                if (!close && (batman.Pos - joker.Pos).Length() < 400)
                {
                    base.ChangeTimerState(false);
                    close = true;
                }
            }
        }
        public override void ActionExecuted(CutsceneAction action)
        {
            if (action.Name.Contains("LoadCity"))
            {
                lookForObj = true;
            }
            base.ActionExecuted(action);
        }
    }
}
