using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondShiftMobile
{
    public enum PosType { Normal, Floor }
    public class EffectsDrawer
    {
        private struct DrawInfoIndex
        {
            public DrawInfo DrawInfo;
            public int Index;
        }
        Effects effects;
        bool began = false;
        List<DrawInfo> drawInfo;
        List<DrawInfoIndex> drawInfoIndex;
        SSEVertex[] vertices;
        int bottomIndex = 0, topIndex = 0;
        public bool Optimize = true;
        public EffectsDrawer(Effects effects)
        {
            this.effects = effects;
            drawInfo = new List<DrawInfo>(2000);
            vertices = new SSEVertex[10000];
            drawInfoIndex = new List<DrawInfoIndex>(2000);
        }
        public void Begin()
        {
            if (began)
                throw new ArgumentException("You must call End() before calling begin again");
            bottomIndex = 0;
            began = true;
        }
        Dictionary<string, object> emptyShaderVals = new Dictionary<string, object>();
        public DrawInfo Draw(TextureFrame tex, Techniques technique, Vector3 pos, Color color, Vector3 origin, Vector3 rotation, Vector3 scale, BlendState blendState, DepthSortingType depthSortType)
        {
            return Draw(tex, technique, pos, color, origin, rotation, scale, Vector2.One, 0, 0, SpriteEffects.None, blendState, SamplerState.LinearClamp, new BevelData(), emptyShaderVals, depthSortType, PosType.Normal);
        }
        public DrawInfo Draw(TextureFrame tex, Techniques technique, Vector3 pos, Color color, Vector3 origin, Vector3 rotation, Vector3 scale, Vector2 texScale, float bloom, float distortion, SpriteEffects spriteEffects, BlendState blend, SamplerState samp, BevelData bevelData, Dictionary<string, object> shaderValues, DepthSortingType depthSort, PosType posType)
        {
            if (!began)
                throw new ArgumentException("You must call Begin() first before you start drawing");

            DrawInfo inf = new DrawInfo()
            {
                Texture = tex,
                Rotation = rotation,
                Technique = technique,
                Pos = pos,
                Origin = origin,
                Scale = scale,
                BlendState = blend,
                SamplerState = samp,
                TextureScale = texScale,
                Color = color,
                SpriteEffects = spriteEffects,
                Distortion = distortion,
                Bloom = bloom,
                PosType = posType
            };
            if (shaderValues != null && shaderValues.Count > 0)
                inf.ShaderValues = shaderValues;
            else inf.ShaderValues = null;
            inf.BevelData = bevelData;
            switch (depthSort)
            {
                case DepthSortingType.Bottom:
                    drawInfo.Insert(bottomIndex, inf);
                    bottomIndex++;
                    break;
                default:
                    drawInfo.Add(inf);
                    break;
            }
            return inf;
        }

        public DrawInfo DrawObj(Obj obj)
        {
            return Draw(obj.Texture, obj.GetDrawTechnique(), obj.Pos, obj.GetDrawColor() * obj.GetDrawAlpha(), obj.Origin.ToVector3(), obj.Rotation, obj.GetDrawScale().ToVector3(), obj.TextureScale, obj.Bloom, obj.Distortion, obj.spriteEffect, obj.BlendState, obj.SamplerState, obj.GetBevel(), obj.ShaderValues, obj.SortingType, obj.PosType);
        }
        public DrawInfo DrawRelativeToObj(Obj obj, TextureFrame tex, Vector3 pos, Color color, Vector3 origin)
        {
            return DrawRelativeToObj(obj, obj.GetDrawTechnique(), tex, pos, color, origin, obj.Rotation, obj.GetDrawScale().ToVector3(), obj.TextureScale, obj.spriteEffect, obj.BlendState, obj.SamplerState, obj.SortingType, obj.PosType);
        }
        public DrawInfo DrawRelativeToObj(Obj obj, TextureFrame tex, Vector3 pos, Color color, Vector3 origin, Vector3 rot, Vector3 scale, Vector2 textureScale, SpriteEffects spriteEffect, DepthSortingType sortingType)
        {
            return DrawRelativeToObj(obj, obj.GetDrawTechnique(), tex, pos, color, origin, rot, scale, textureScale, spriteEffect, obj.BlendState, obj.SamplerState, sortingType, obj.PosType);
        }
        public DrawInfo DrawRelativeToObj(Obj obj, TextureFrame tex, Vector3 pos, Color color, Vector3 origin, Vector3 rot, Vector3 scale, Vector2 textureScale, SpriteEffects spriteEffect, DepthSortingType sortingType, PosType posType)
        {
            return DrawRelativeToObj(obj, obj.GetDrawTechnique(), tex, pos, color, origin, rot, scale, textureScale, spriteEffect, obj.BlendState, obj.SamplerState, sortingType, posType);
        }
        public DrawInfo DrawRelativeToObj(Obj obj, Techniques technique, TextureFrame tex, Vector3 pos, Color color, Vector3 origin, Vector3 rot, Vector3 scale, Vector2 textureScale, SpriteEffects spriteEffect, BlendState blendState, SamplerState samplerState, DepthSortingType sortingType, PosType posType)
        {
            return Draw(tex, technique, obj.Pos + (pos * obj.Scale.ToVector3(1)), color, origin, rot + obj.Rotation, scale * obj.Scale.ToVector3(1), textureScale, obj.Bloom, obj.Distortion, spriteEffect, blendState, samplerState, obj.GetBevel(), obj.ShaderValues, sortingType, posType);

        }

        /// <summary>
        /// Performs all of the collected draw operations
        /// </summary>
        /// <returns>An array of integers, with the 1st value representing the total number of draws, and the 2nd value representing the number of draws after batching</returns>
        public int[] End()
        {
            int[] draws = new int[] { drawInfo.Count, 0 };
            if (!began)
                throw new ArgumentException("You must call Begin() first before calling End()");
            began = false;
            if (drawInfo.Count == 0)
                return draws;
            var gd = effects.GraphicsDevice;
            //VertexBuffer buff = new VertexBuffer(gd, typeof(VertexPositionColorTexture), 60000, BufferUsage.WriteOnly);
            int startIndex = 0;
            int endIndex = 0;
            int verticesIndex = 0;
            Vector3 offset = Vector3.Zero;
            for (int i = 1; i < drawInfo.Count; i++)
            {
                DrawInfo prevInf = drawInfo[startIndex], inf = drawInfo[i];
                if (
                    //true ||
                    !Optimize ||
                    prevInf.Texture.Texture != inf.Texture.Texture ||
                    prevInf.Technique != inf.Technique || 
                    prevInf.SamplerState != inf.SamplerState ||
                    prevInf.BlendState != inf.BlendState ||
                    prevInf.Bloom != inf.Bloom ||
                    prevInf.Distortion != inf.Distortion ||
                    inf.BevelData != prevInf.BevelData ||
                    //prevInf.Scale != inf.Scale ||
                    //prevInf.Pos != inf.Pos ||
                    prevInf.Rotation != inf.Rotation ||
                    prevInf.PosType != inf.PosType ||
                    inf.ShaderValues != null
                    )
                {
                    offset = drawInfo[startIndex].Pos;
                    //offset = new Vector3(0, 0, 100000);
                    verticesIndex = addVertices(startIndex, i - 1, verticesIndex, offset, drawInfo[startIndex].PosType);
                    drawInfoIndex.Add(new DrawInfoIndex() { DrawInfo = drawInfo[startIndex], Index = verticesIndex - 1 });
                    //offset = inf.Pos;
                    startIndex = i;
                }
            }
            offset = drawInfo[startIndex].Pos;
            verticesIndex = addVertices(startIndex, drawInfo.Count - 1, verticesIndex, offset, drawInfo[startIndex].PosType);
            drawInfoIndex.Add(new DrawInfoIndex() { DrawInfo = drawInfo[startIndex], Index = verticesIndex - 1 });
            //buff.SetData<VertexPositionColorTexture>(vertices, 0, verticesIndex);
            //gd.SetVertexBuffer(buff);
            int lastVertexIndex = 0;
            for (int i = 0; i < drawInfoIndex.Count; i++)
            {
                var inf = drawInfoIndex[i].DrawInfo;
                int ind = drawInfoIndex[i].Index;
                setInfoData(inf, gd);
                draws[1] += drawVertices(gd, lastVertexIndex, ind, inf.Technique);
                lastVertexIndex = ind + 1;
            }
            
            drawInfo.Clear();
            drawInfoIndex.Clear();
            return draws;
        }
        int drawVertices(GraphicsDevice gd, int startIndex, int endIndex, Techniques technique)
        {
            effects.Technique = technique;
            gd.RasterizerState = RasterizerState.CullNone;

            //var vertices = new SSEVertex[6];
            int verticesStartIndex = 0;
            float w = 1000, h = 1000;
            /*vertices[verticesStartIndex++] = new SSEVertex(new Vector3(0, 0, 0), Color.White, new Vector2(0, 0));
            vertices[verticesStartIndex++] = new SSEVertex(new Vector3(w, 0, 0), Color.White, new Vector2(1, 0));
            vertices[verticesStartIndex++] = new SSEVertex(new Vector3(w, h, 0), Color.White, new Vector2(1, 1));
            vertices[verticesStartIndex++] = new SSEVertex(new Vector3(w, h, 0), Color.White, new Vector2(1, 1));
            vertices[verticesStartIndex++] = new SSEVertex(new Vector3(0, h, 0), Color.White, new Vector2(0, 1));
            vertices[verticesStartIndex++] = new SSEVertex(new Vector3(0, 0, 0), Color.White, new Vector2(0, 0));*/
            int numVertices = ((endIndex - startIndex) / 3) + 1;
/*#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached && Global.Effects.ObjDistort > 0)
                System.Diagnostics.Debugger.Break();
#endif*/
            foreach (var p in effects.CurrentTechnique.Passes)
            {
                p.Apply();
                //gd.DrawUserPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleList, vertices, 0, 2);
                //int size = System.Runtime.InteropServices.Marshal.SizeOf(typeof(SSEVertex));
                gd.DrawUserPrimitives<SSEVertex>(PrimitiveType.TriangleList, this.vertices, startIndex, numVertices);
                //gd.DrawPrimitives(PrimitiveType.TriangleList, startIndex, ((endIndex - startIndex) / 3)  + 1);
                //gd.DrawIndexedPrimitives(PrimitiveType.
            }
            return effects.CurrentTechnique.Passes.Count;
        }
        void setInfoData(DrawInfo inf, GraphicsDevice gd)
        {
            gd.Textures[0] = inf.Texture;
            var m = //Matrix.CreateTranslation(-inf.Origin) * 
                //Matrix.CreateScale(inf.Scale) * 
                Matrix.CreateRotationX(MathHelper.ToRadians(inf.Rotation.X))
                * Matrix.CreateRotationY(MathHelper.ToRadians(inf.Rotation.Y))
                * Matrix.CreateRotationZ(MathHelper.ToRadians(inf.Rotation.Z))
                * Matrix.CreateTranslation(inf.Pos)
                ;
            if (inf.ShaderValues != null)
            {
                foreach (var sv in inf.ShaderValues)
                {
                    effects.SetValue(sv);
                }
            }
            effects.Parameters["ObjectTransform"].SetValue(m);
            effects.Parameters["textureScale"].SetValue(Vector2.One);
            effects.ObjBloom = inf.Bloom;
            effects.ObjDistort = inf.Distortion;
            if (inf.BevelData.IsBeveling)
            {
                effects.BevelColor = inf.BevelData.BevelColor;
                effects.BevelDelta = inf.BevelData.BevelDelta;
                effects.BevelGlow = inf.BevelData.BevelGlow;
            }
/*#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached && inf.Distortion > 0)
                System.Diagnostics.Debugger.Break();
#endif*/
            gd.BlendState = inf.BlendState;
            gd.SamplerStates[0] = inf.SamplerState;
        }
        int addVertices(int startIndex, int endIndex, int verticesStartIndex, Vector3 offset, PosType posType)
        {
            int xt0;
            int xt1;

            if (posType == PosType.Normal)
            {
                for (int i = startIndex; i <= endIndex; i++)
                {
                    DrawInfo inf = drawInfo[i];

                    xt0 = (inf.SpriteEffects == SpriteEffects.FlipHorizontally) ? 1 : 0;
                    xt1 = (inf.SpriteEffects == SpriteEffects.FlipHorizontally) ? 0 : 1;
                    float w = inf.Texture.Width * inf.Scale.X, h = inf.Texture.Height * inf.Scale.Y;
                    //w = h = 1000;
                    Vector3 p = inf.Pos;
                    p = (inf.GetRealPosition()) - offset;
                    //p = offset;
                    Vector2 texScale = inf.TextureScale * inf.Texture.TextureScale;

                    vertices[verticesStartIndex++] = new SSEVertex(new Vector3(0, 0, 0) + p, inf.Color, inf.Texture.TexturePos + (new Vector2(xt0, 0) * texScale));
                    vertices[verticesStartIndex++] = new SSEVertex(new Vector3(w, 0, 0) + p, inf.Color, inf.Texture.TexturePos + (new Vector2(xt1, 0) * texScale));
                    vertices[verticesStartIndex++] = new SSEVertex(new Vector3(w, h, 0) + p, inf.Color, inf.Texture.TexturePos + (new Vector2(xt1, 1) * texScale));
                    vertices[verticesStartIndex++] = new SSEVertex(new Vector3(w, h, 0) + p, inf.Color, inf.Texture.TexturePos + (new Vector2(xt1, 1) * texScale));
                    vertices[verticesStartIndex++] = new SSEVertex(new Vector3(0, h, 0) + p, inf.Color, inf.Texture.TexturePos + (new Vector2(xt0, 1) * texScale));
                    vertices[verticesStartIndex++] = new SSEVertex(new Vector3(0, 0, 0) + p, inf.Color, inf.Texture.TexturePos + (new Vector2(xt0, 0) * texScale));
                }
            }
            else if (posType == PosType.Floor)
            {
                for (int i = startIndex; i <= endIndex; i++)
                {
                    DrawInfo inf = drawInfo[i];

                    xt0 = (inf.SpriteEffects == SpriteEffects.FlipHorizontally) ? 1 : 0;
                    xt1 = (inf.SpriteEffects == SpriteEffects.FlipHorizontally) ? 0 : 1;
                    float w = inf.Texture.Width * inf.Scale.X, h = inf.Texture.Height * inf.Scale.Y;
                    //w = h = 1000;
                    Vector3 p = inf.Pos;
                    p = (inf.GetRealPosition()) - offset;

                    //p = offset;
                    Vector2 texScale = inf.TextureScale;

                    vertices[verticesStartIndex++] = new SSEVertex(new Vector3(0, 0, 0) + p, inf.Color, new Vector2(xt0, 0) * texScale);
                    vertices[verticesStartIndex++] = new SSEVertex(new Vector3(w, 0, 0) + p, inf.Color, new Vector2(xt1, 0) * texScale);
                    vertices[verticesStartIndex++] = new SSEVertex(new Vector3(w, 0, -h) + p, inf.Color, new Vector2(xt1, 1) * texScale);
                    vertices[verticesStartIndex++] = new SSEVertex(new Vector3(w, 0, -h) + p, inf.Color, new Vector2(xt1, 1) * texScale);
                    vertices[verticesStartIndex++] = new SSEVertex(new Vector3(0, 0, -h) + p, inf.Color, new Vector2(xt0, 1) * texScale);
                    vertices[verticesStartIndex++] = new SSEVertex(new Vector3(0, 0, 0) + p, inf.Color, new Vector2(xt0, 0) * texScale);
                }
            }
            return verticesStartIndex;
        }
    }
    public struct DrawInfo
    {
        public bool UpdateMatrix;
        public Vector3 Rotation, Pos, Origin, Scale;
        public Techniques Technique;
        public TextureFrame Texture;
        public SamplerState SamplerState;
        public Color Color;
        public BlendState BlendState;
        public Vector2 TextureScale;
        public SpriteEffects SpriteEffects;
        public float Bloom, Distortion;
        public Dictionary<string, object> ShaderValues;
        public PosType PosType;
        public Vector3 GetRealPosition()
        {
            switch (PosType)
            {
                default:
                    return Pos - (Origin * Scale);
                case SecondShiftMobile.PosType.Floor:
                    return Pos - (new Vector3(Origin.X, Origin.Z, -Origin.Y) * new Vector3(Scale.X, Scale.Z, Scale.Y));
            }
        }
        public BevelData BevelData;
    }
    public struct BevelData
    {
        public Vector2 BevelDelta;
        public Vector3 BevelColor;
        public float BevelGlow;
        public bool IsBeveling;
        public override bool Equals(object obj)
        {
            if (obj != null)
            {
                if (obj is BevelData)
                {
                    var bd = (BevelData)obj;
                    {
                        return bd.BevelDelta == BevelDelta && bd.BevelGlow == BevelGlow && bd.BevelColor == BevelColor;
                    }
                }
            }
            return false;
        }
        public static bool operator ==(BevelData bd1, BevelData bd2)
        {
            return bd1.BevelDelta == bd2.BevelDelta && bd1.BevelGlow == bd2.BevelGlow && bd1.BevelColor == bd2.BevelColor;
        }
        public static bool operator !=(BevelData bd1, BevelData bd2)
        {
            return !(bd1.BevelDelta == bd2.BevelDelta && bd1.BevelGlow == bd2.BevelGlow && bd1.BevelColor == bd2.BevelColor);
        }
    }
}
