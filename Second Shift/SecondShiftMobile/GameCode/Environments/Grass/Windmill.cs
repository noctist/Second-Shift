using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondShiftMobile.Environments.Grass
{
    public class Windmill : Obj
    {
        TextureFrame blades;
        Vector3 bladesOrigin;
        float rot = 0;
        SoundEffectInstance sound;
        public Windmill(Game1 Doc, float X, float Y, float Z)
            : base(Doc, Doc.LoadTex("Environment/Grass/Windmill"), X, Y, Z)
        {
            blades = Doc.LoadTex("Environment/Grass/Windmill Blades");
            bladesOrigin = blades.Texture.GetCenter().ToVector3();
            sound = Doc.LoadSoundEffectInstance("windmill");
            sound.Volume = 0;
            sound.IsLooped = true;
            sound.Play();
            //blades.TextureScale.X = 0.5f;
            Origin.Y = 2048;
        }
        public override void Update()
        {
            base.Update();
            sound.Volume = GetSoundVolume(1);
            sound.SetPan(Pos);
            sound.Pitch = GetSoundPitch();
            rot -= 0.75f * PlaySpeed;
        }
        public override void Remove()
        {
            sound.Stop();
            base.Remove();
        }
        public override void Draw()
        {
            //Global.Effects.Technique = Techniques.NormalDepth;
            //Global.Effects.SetObjValues(this.Pos, Textures.Light2.GetCenter().ToVector3(), new Vector3(90, 0, 0), new Vector3(30, 18, 1), Vector2.One);
            /*foreach (var p in Global.Effects.CurrentTechnique.Passes)
            {
                p.Apply();
                doc.DrawSprite(Textures.Light2, Vector3.Zero, Color.Black * 0.9f, SpriteEffects.None);
            }*/
            base.Draw();
            Global.Drawer.Draw(blades, this.GetDrawTechnique(), this.Pos + new Vector3(0, ((-Origin.Y) + 291) * Scale.Y, 0), GetDrawColor() * GetDrawAlpha(), bladesOrigin, new Vector3(0, 0, rot), Scale.ToVector3(), Vector2.One, 1, distortion: 0, spriteEffects: spriteEffect, blend: BlendState, samp: SamplerState, shaderValues: ShaderValues, bevelData:new BevelData(), depthSort:DepthSortingType.ZDepth, posType:PosType.Normal);
            //Global.Effects.SetObjValues(this.Pos + new Vector3(0, (-Origin.Y) + 291, 0), bladesOrigin, new Vector3(0, 0, rot), new Vector3(1), Vector2.One);
            /*foreach (var p in Global.Effects.CurrentTechnique.Passes)
            {
                p.Apply();
                doc.DrawSprite(blades, Vector3.Zero, GetDrawColor() * GetDrawAlpha(), SpriteEffects.FlipHorizontally);
            }*/
        }
    }
}
