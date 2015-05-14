using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondShiftMobile.Players
{
    public class Match : Player
    {
        Combo normalCombo;
        public Match(Game1 Doc, float X, float Y, float Z)
            : base(Doc, Doc.LoadTex("Characters/Match/Stand"), X, Y, Z)
        {
            StandSprite = Doc.LoadTex("Characters/Match/Stand");
            Scale = new Vector2((200) / 506f);
            topSpeed = 13;
            BoundingRectangle = new Rectangle(200, 220, 200, 0);
            normalCombo = new Combo()
            {
                Animation = Doc.LoadStickAnimation("Characters/Match/NormalCombo", 31),
                Framespeed = 0.65f,
                AllowSwitchOut = false
            };
            normalCombo.Attacks.Add(new Attack(0, 10, 5)
            {
                Power = 10,
                HitBox = new Rectangle(280, 280, 80, 15),
                AttackSound = "meleemiss2",
                HitSound = "weakpunch",
                MoveSpeed = new Vector3(8, 0, 0)
            });
            normalCombo.Attacks.Add(new Attack(11, 20, 16)
            {
                Power = 10,
                HitBox = new Rectangle(310, 280, 70, 100),
                Direction = 270,
                KnockBackAmount = 11,
                AttackSound = "meleemiss3",
                HitSound = "mediumkick",
                HitPauseLength = 15,
                MoveSpeed = new Vector3(6, -5, 0)
            });
            normalCombo.Attacks.Add(new Attack(21, 31, 24)
            {
                Power = 31,
                HitBox = new Rectangle(310, 250, 80, 15),
                MoveSpeed = new Vector3(14, 0 ,0),
                HitPauseLength = 45,
                KnockBackAmount = 25,
                EffectType = AttackEffectType.Sharp,
                AttackSound = "meleemiss1",
                HitSound = "mediumpunch"
            });
            AddCombo(normalCombo);
        }

        public override void AttackButtonHit(ControlDirection direction)
        {
            if (!Attacking)
            {
                if (direction == ControlDirection.Back)
                    speedMul *= -1;
            }
            ChangeCurrentCombo(normalCombo, false);
            NextAttack();
            //base.AttackButtonHit(direction);
        }
    }
}
