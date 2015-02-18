using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace SecondShiftMobile.Enemies
{
    public class Guard : Enemy
    {
        CombatObj player;
        protected float RunFramespeedMultiplier = 0.03f;
        Combo normalCombo;
        public Guard(Game1 Doc, float x, float y, float z)
            : base(Doc, Doc.LoadTex("Characters/Enemies/Guard/Stand"), x, y, z)
        {
            health = maxHealth = 80;
            StandSprite = Texture;
            RunAnimation = Doc.LoadAnimation("Characters/Enemies/Guard/Run", "run", 0, 3);
            topSpeed = 14;
            BoundingRectangle = new Rectangle(30, 0, 30, 0);
            normalCombo = new Combo() { Animation = doc.LoadAnimation("Characters/Enemies/Guard/Attack", "hit", 0, 3), Framespeed = 0.2f, LoopAttack = false };
            normalCombo.Attacks.Add(new Attack(0, 3, 2)
            {
                Power = 5,
                AttackSound = "meleemiss1", HitSound = "weakkick",
                WaitTime = 3000,
                TimeOut = 60,
                HitBox = new Rectangle(80, 52, 47, 70),
                KnockBack = false
            });
            ChangeCurrentCombo(normalCombo);
            Active = true;
            //Parallel = true;
            DeactivateOffscreen = true;
            OnScreenExpansion = 400;
        }
        Rectangle recIntersection = Rectangle.Empty;
        public override void Attacked(Attack attack, Obj obj, Rectangle AttackBox, Rectangle intersection)
        {
            recIntersection = intersection;
            base.Attacked(attack, obj, AttackBox, intersection);
        }
        protected override void Die(Attack attack, Obj obj, Rectangle intersection)
        {
            var dat = Textures.GetTextureData(Texture);
            Vector3 cent = intersection.Center.ToVector2().ToVector3();
            Vector3 mycent = BoundingBox.Center.ToVector2().ToVector3();
            cent.Z = mycent.Z = Pos.Z;
            Vector3 baseSpeed = mycent - cent;
            for (int i = 0; i < dat.Length; i+= 60)
            {
                float ix = i % Texture.Width;
                float iy = i / Texture.Height;
                var c = dat[i % Texture.Width, i / Texture.Height];
                if (c.A > 100)
                {
                    var d = new Disappear(doc, Textures.SmallLight, Pos.X, Pos.Y, Pos.Z) {  Color = c };
                    Vector3 pos = Pos + ((new Vector3(ix, iy, 0) - Origin.ToVector3()) * Scale.ToVector3());
                    d.Pos = Pos + ((new Vector3(ix, iy, 0) - Origin.ToVector3()) * Scale.ToVector3());
                    d.Speed = ((baseSpeed * 0.5f) + (new Vector3(MyMath.RandomRange(-1, 1), MyMath.RandomRange(-1, 1), MyMath.RandomRange(-1, 1)) * 10));
                    //d.Speed = baseSpeed;
                    d.UpscaledSecondShiftDraw = true;
                }
            }
            base.Die(attack, obj, intersection);
        }
        public override void Update()
        {
            if (player == null)
            {
                player = doc.FindObject<Player>();
            }
            else
            {
                if (Math.Abs(player.Pos.X - Pos.X) < 1500 && Math.Abs(player.Pos.Y - Pos.Y) < 400 && OnTheGround)
                {
                    if (Math.Abs(player.Pos.X - Pos.X) > 100)
                    {
                        if (player.Pos.X > Pos.X)
                            MoveRight();
                        else if (player.Pos.X < Pos.X)
                            MoveLeft();
                    }
                    if (Math.Abs(player.Pos.X - Pos.X) < 50)
                    {
                        if (player.Pos.X > Pos.X)
                            MoveLeft();
                        else if (player.Pos.X < Pos.X)
                            MoveRight();
                    }
                    if (player.Pos.Y < Pos.Y - 100 && !Attacking)
                        Jump(10);
                    if (Math.Abs(player.Pos.X - Pos.X) < 65 && OnTheGround && player.CanAttack)
                    {
                        if (Speed.X < 1)
                        {
                            if (player.Pos.X > Pos.X)
                                speedMul = 1;
                            else if (player.Pos.X < Pos.X)
                                speedMul = -1;
                        }
                        NextAttack();
                    }
                }
            }
            base.Update();
        }
        protected override void SetFramespeed()
        {
            if (!Attacking)
            {
                switch (State)
                {
                    case PlatformerState.Running:
                        Framespeed = speed2 * RunFramespeedMultiplier;
                        break;
                    default:
                        Framespeed = 1;
                        break;
                }
                base.SetFramespeed();
            }
        }
        public override void Draw()
        {
            base.Draw();
            //var rec = ConvertToScreenRec(BoundingBox);
            //doc.DrawSprite(doc.LoadTex("TestWall"), new Vector2(rec.Center.X, rec.Top - 20), Color.Black * 0.4f, 0, new Vector2(64, 64), new Vector2(1, 0.3f), Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0);
            //doc.DrawSprite(doc.LoadTex("TestWall"), new Vector2(rec.Center.X, rec.Top - 20), Color.White, 0, new Vector2(64, 64), new Vector2(health / maxHealth, 0.3f), Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0);
        }
    }
}
