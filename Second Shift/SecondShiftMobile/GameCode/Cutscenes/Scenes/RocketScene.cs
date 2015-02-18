using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace SecondShiftMobile.Cutscenes.Scenes
{
    public class RocketScene : Cutscene
    {
        public RocketScene(XElement x)
            : base(x)
        {

        }
        public override void ActionExecuted(CutsceneAction action)
        {
            if (action.Name == "Launch")
            {
                var rocket = Global.Game.FindObject<Test.LaunchRocket>();
                if (rocket != null)
                {
                    rocket.Launch();
                }
                Global.Camera.Shake.SetShakeSize(20, 1);
                Global.Camera.Shake.SetShakeSpeed(0.05f, 0.005f);
            }
            else if (action.Name == "Explode")
            {
                var rocket = Global.Game.FindObject<Test.LaunchRocket>();
                if (rocket != null)
                {
                    var ex = new Explosion(Global.Game, rocket.Pos.X, rocket.Pos.Y, rocket.Pos.Z, 60, 0.1f, 600, 3, 600, 60);
                    Random r = new Random(48);
                    for (int i = 0; i < 0; i++)
                    {
                        var em = new SmokeEmitter(Global.Game, ex.Pos.X, ex.Pos.Y, ex.Pos.Z);
                        em.Speed = new Microsoft.Xna.Framework.Vector3(MyMath.RandomRange(-1, 1), MyMath.RandomRange(-1, 1), MyMath.RandomRange(-1, 1)) * 40;
                        em.AddBehaviour(new Behaviours.GravityBehaviour(0.4f));
                        em.AddBehaviour(new Behaviours.DisappearBehaviour(300, 0.1f, 0.1f, 1));
                        em.MaxTimer = 4;
                        em.StartScale = 5;
                    }
                    rocket.Remove();
                    Global.Camera.Shake.SetShakeSpeed(0.0f, 0.01f);
                }
            }
            base.ActionExecuted(action);
        }
    }
}
