using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SecondShiftMobile.Elements;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondShiftMobile.Environments.Grass
{
    public class Tree : Obj
    {
        public bool Emit = true;
        public float MaxLeafTime = 15;
        float leafTimer = 0;
        SamplerState clamp = SamplerState.LinearClamp;
        SamplerState wrap = SamplerState.LinearWrap;
        public Tree(Game1 Doc, float X, float Y, float Z)
            : this(Doc, Doc.LoadTex("Environment/Grass/Tree"), X, Y, Z)
        {

        }
        protected Tree(Game1 Doc, TextureFrame Tex, float X, float Y, float Z)
            : base(Doc, Tex, X, Y, Z)
        {
            DeactivateOffscreen = true;
            Origin.Y = 450;
        }
        public override StageObjectProperty[] GetStageProperties()
        {
            return new StageObjectProperty[] {
                new StageObjectProperty() { Name = "Scale", Info = "This will also control the TextureScale", Value = Scale }
            };
        }
        public override void SetStageProperties(params StageObjectProperty[] sop)
        {
            base.SetStageProperties(sop);
            foreach (var s in sop)
            {
                if (s.Name == "Scale")
                {
                    Scale = s.GetVector2();
                }
            }
        }
        public override void ParallelUpdate()
        {
            TextureScale = Scale;
            if (TextureScale.X > 1)
            {
                SamplerState = wrap;
            }
            else
            {
                SamplerState = clamp;
            }
            base.ParallelUpdate();
            if (Emit)
            {
                leafTimer += PlaySpeed;
                while (leafTimer > MaxLeafTime)
                {
                    leafTimer -= MaxLeafTime;
                    var leaf = new TreeLeaf(doc, MyMath.RandomRange(BoundingBox.Left + (72f * Scale.X), BoundingBox.Left + (442f * Scale.X)), MyMath.RandomRange(BoundingBox.Top + (150f * Scale.Y), BoundingBox.Top + (215f * Scale.Y)), Pos.Z)
                    {
                        Lifetime = 600,
                        MaxAlpha = 1,
                        FadeInSpeed = 0.01f,
                        FadeOutSpeed = -0.01f
                    };

                }
            }
        }
    }
    public class TreeFlatBottom : Tree
    {
        public float MaxLeafTime = 15;
        float leafTimer = 0;
        public TreeFlatBottom(Game1 Doc, float X, float Y, float Z)
            : base(Doc,  Doc.LoadTex("Environment/Grass/Tree2"), X, Y, Z)
        {
            Origin.Y = 512;
            Emit = false;
            SlowDown = false;
        }
        public override void Draw()
        {
            /*Global.Effects.Technique = Techniques.NormalDepth;
            Global.Effects.SetObjValues(this.Pos, Textures.Light.GetCenter().ToVector3(), new Vector3(90, 0, 0), new Vector3(10, 6, 1), Vector2.One);
            foreach (var p in Global.Effects.CurrentTechnique.Passes)
            {
                p.Apply();
                doc.DrawSprite(Textures.Light2, Vector3.Zero, Color.Black * 0.9f, SpriteEffects.None);
            }*/
            Global.Drawer.DrawRelativeToObj(this, Textures.Light2, Vector3.Zero, Color.Black * 0.9f, Textures.Light2.Texture.GetCenter().ToVector3(), new Vector3(0, 0, 0), new Vector3(10, 6, 1), Vector2.One, spriteEffect, DepthSortingType.Bottom, PosType.Floor);
            base.Draw();
        }
    }
    public class TreeLeaf : Disappear
    {
        Behaviours.GravityBehaviour grav;
        float windMult;
        public TreeLeaf(Game1 Doc, float X, float Y, float Z)
            :base(Doc, Doc.LoadTex("Shard"), X, Y, Z)
        {
            AddBehaviour(grav = new Behaviours.GravityBehaviour(0.03f));
            //AddBehaviour(new Behaviours.SpeedDissipation(5, 0.99f));
            Scale = new Microsoft.Xna.Framework.Vector2(0.1f);
            Color = new Microsoft.Xna.Framework.Color(150, 255, 50);
            Parallel = true;
            windMult = MyMath.RandomRange(0.035f, 0.065f);
            Rotation.Z = MyMath.RandomRange(0, 360);
        }
        public override void Update()
        {
            Wind[] winds = doc.FindObjects<Wind>();
            Pos.Y -= Math.Abs(Speed.X * 0.3f) * PlaySpeed;
            //Global.Output = "Wind length: " + winds.Length;
            RotationSpeed.Z = (Speed.X * 0.5f) + (Speed.Length() * 0.1f);
            Speed.X *= 0.95f * PlaySpeed;
            if (OnScreen)
            {
                foreach (var w in winds)
                {
                    if (IsCollidingWith(w))
                    {
                        Vector3 wind = (Vector3)w.GetElementValue(this);
                        Speed += (wind) * windMult * PlaySpeed;
                    }
                }
                if (Speed.Y > 0)
                {
                    for (int i = 0; i < doc.NumberOfObjects; i++)
                    {
                        var o = doc.objArray[i];
                        if (o == null)
                            continue;
                        if (o.Active && o.IsFloor && BoundingBox.Right >= o.BoundingBox.Left && BoundingBox.Left <= o.BoundingBox.Right)
                        {
                            if (BoundingBox.Top <= o.BoundingBox.Bottom && BoundingBox.Bottom >= o.BoundingBox.Top)
                            {
                                Pos.Y = o.BoundingBox.Top - ((Texture.Height - Origin.Y) * Scale.Y);
                                Speed.Y = 0;
                                break;
                            }
                        }
                    }
                }
            }
            base.Update();
        }
    }
}
