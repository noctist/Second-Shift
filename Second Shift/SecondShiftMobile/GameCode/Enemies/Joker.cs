using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace SecondShiftMobile.Enemies
{
    public class Joker :Enemy
    {
        CombatObj player;
        Combo normalCombo;
        public Joker(Game1 Doc, float X, float Y, float Z)
            :base(Doc, Doc.LoadTex("Characters/Enemies/Joker/Stand"), X, Y, Z)
        {
            Faction = SecondShiftMobile.Faction.AntiHero;
            acceleration = 0.7f;
            Scale = new Microsoft.Xna.Framework.Vector2(2);
            maxHealth = health = 500;
            BoundingRectangle = new Rectangle(48, 72, 48, 0);

            StandSprite = Doc.LoadTex("Characters/Enemies/Joker/Stand");
            RunAnimation = Doc.LoadAnimation("Characters/Enemies/Joker/Run", "Run", 0, 8);

            normalCombo = new Combo()
            {
                Animation = Doc.LoadAnimation("Characters/Enemies/Joker/Combo", "Combo", 0, 4),
                Framespeed = 0.2f, LoopAttack = true
            };
            normalCombo.Attacks.Add(new Attack(0, 4, 3)
            {
                Power = 30,
                AttackSound = "meleemiss3",
                HitSound = "strongpunch",
                WaitTime = 60,
                HitBox = new Rectangle(82, 80, 24, 10),
                EffectType = AttackEffectType.Sharp
            }); 
            
            ChangeCurrentCombo(normalCombo);
            DeactivateOffscreen = false;
            AddCombo(normalCombo);
            //Active = false;
        }
        protected override void SetFramespeed()
        {
            if (State == PlatformerState.Running)
            {
                Framespeed = speed2 * 0.025f;
            }
            base.SetFramespeed();
        }
        public override void ParallelUpdate()
        {
            var players = doc.FindObjects<CombatObj>();
            var dist = float.MaxValue;
            foreach (var p in players)
            {
                if (p != this && p.Faction != SecondShiftMobile.Faction.Civilian)
                {
                    var l = (p.Pos - Pos).Length();
                    if (l < dist)
                    {
                        dist = l;
                        player = p;
                    }
                }
            }
            /*if (player == null)
            {
                player = doc.FindObject<Player>();
            }*/
            base.ParallelUpdate();
        }
        public override void Update()
        {
            if (player != null)
            {
                float xDist = Math.Abs(Pos.X - player.Pos.X);
                if (xDist < 500)
                {
                    if (xDist > 75)
                    {
                        if (player.Pos.X < Pos.X)
                        {
                            MoveLeft();
                        }
                        if (player.Pos.X > Pos.X)
                        {
                            MoveRight();
                        }
                    }
                    else
                    {
                        if (!Attacking)
                        {
                            NextAttack();
                        }
                    }
                }
            }
            base.Update();
        }
        public override void Draw()
        {
            base.Draw();
            Global.Effects.Technique = Techniques.NormalDepth;
            foreach (var p in Global.Effects.CurrentTechnique.Passes)
            {
                p.Apply();
                doc.DrawSprite(Textures.WhiteBlock, this.ConvertToScreenRec(new Rectangle((int)Pos.X - 50, (int)Pos.Y - 70, 100, 20)), null, Color.White);
                doc.DrawSprite(Textures.WhiteBlock, this.ConvertToScreenRec(new Rectangle((int)Pos.X - 48, (int)Pos.Y - 68, (int)(96 * (health / maxHealth)), 16)), null, Color.DarkRed);
            }
        }
    }
}
