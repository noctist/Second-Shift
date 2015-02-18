using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
namespace SecondShiftMobile.Cutscenes.Scenes
{
    public class TestScene2 : Cutscene
    {
        public TestScene2(XElement x)
            : base(x)
        {

        }
        public override void ActionExecuted(CutsceneAction action)
        {
            if (action.Name == "Explosion")
            {
                Obj o = Global.Game.FindObject<Player>();
                if (o != null)
                {
                    var sm = new SmokeEmitter(Global.Game, o.Pos.X, o.Pos.Y - 100, o.Pos.Z) { Speed = new Vector3(0, 0, 10), MaxTimer = 3, Name = "CutSmokeEmitter" };
                    LensFlare lf = new LensFlare(Global.Game) { Target = sm };
                }
            }
            else if (action.Name == "Kill Explosion")
            {
                var obj = Global.Game.FindObjectByName("CutSmokeEmitter");
                if (obj != null)
                {
                    obj.Remove();
                }
            }
            base.ActionExecuted(action);
        }
    }
}
