using System;
using System.Collections.Generic;
using System.Text;

namespace SecondShiftMobile.Players
{
    public class Batman : Player
    {
        Combo normalCombo;
        public Batman(Game1 Doc, float X, float Y, float Z)
            : base(Doc, Doc.LoadTex("Characters/Batman/Stand"), X, Y, Z)
        {
            BevelDeltaMultiplier = new Microsoft.Xna.Framework.Vector2(0.75f);
            Weight = 15;
            health = maxHealth = 750;
            BoundingRectangle = new Microsoft.Xna.Framework.Rectangle(48, 60, 50, 0);
            Scale = new Microsoft.Xna.Framework.Vector2(2f);
            StandSprite = Doc.LoadTex("Characters/Batman/Stand");
            JumpSprite = Doc.LoadTex("Characters/Batman/Jump");
            FallAnimation = Doc.LoadAnimation("Characters/Batman/Fall", "Fall", 0, 2);
            RunAnimation = Doc.LoadAnimation("Characters/Batman/Run", "Run", 0, 9);
            RunFramespeedMultiplier = 0.025f;
            WallStickCenter = 64;

            normalCombo = new Combo()
            {
                Framespeed = 0.3f,
                Animation = Doc.LoadAnimation("Characters/Batman/Combo", "Combo", 0, 18)
            };
            normalCombo.Attacks.Add(new Attack(0, 5, 3) { Power = 10, AttackSound = "meleemiss2", HitSound = "mediumpunch", HitBox = new Microsoft.Xna.Framework.Rectangle(74, 80, 24, 10) });
            normalCombo.Attacks.Add(new Attack(6, 10, 7) { Power = 15, AttackSound = "meleemiss1", HitSound = "weakpunch", HitBox = new Microsoft.Xna.Framework.Rectangle(72, 81, 26, 9) });
            normalCombo.Attacks.Add(new Attack(11, 18, 16) { Power = 100, AttackSound = "meleemiss3", HitSound = "strongkick", KnockBack = true, HitBox = new Microsoft.Xna.Framework.Rectangle(82, 78, 22, 9) });
            Combos.Add(normalCombo);
            
        }
        public override void AttackButtonHit(ControlDirection direction)
        {
            ChangeCurrentCombo(normalCombo, false);
            base.AttackButtonHit(direction);
        }
        protected override void SetFramespeed()
        {
            if (State == PlatformerState.Falling)
            {
                Framespeed = 0.25f;
            }
            else
            {
                base.SetFramespeed();
            }
        }
    }
}
