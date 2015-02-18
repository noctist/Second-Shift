using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondShiftMobile
{
    public class PolygonObj : Obj
    {
        protected VertexPositionColorTexture[] origVerts;
        protected VertexPositionColorTexture[] vertices;
        public PrimitiveType PrimitiveType = PrimitiveType.TriangleList;
        public PolygonObj(Game1 Doc, TextureFrame Tex, float X, float Y, float Z)
            :base(Doc, Tex, X, Y, Z)
        {
            
            AlwaysChangeSpriteBatchAfter = true;
            origVerts = GetVertices();
            if (origVerts != null)
            {
                vertices = new VertexPositionColorTexture[origVerts.Length];
                for (int i = 0; i < vertices.Length; i++)
                {
                    vertices[i] = new VertexPositionColorTexture(origVerts[i].Position, origVerts[i].Color, origVerts[i].TextureCoordinate);
                }
            }
            else
            {
                vertices = new VertexPositionColorTexture[0];
            }
        }
        protected override void SetOnScreen()
        {
            OnScreen = true;
        }
        public int GetPrimitiveCount()
        {
            if (PrimitiveType == PrimitiveType.TriangleList)
            {
                return vertices.Length / 3;
            }
            else if (PrimitiveType == Microsoft.Xna.Framework.Graphics.PrimitiveType.LineStrip)
            {
                return vertices.Length - 1;
            }
            else return 0;
        }
        
        public override void Update()
        {
            base.Update();
        }

        public override void SetPosition()
        {
            base.SetPosition();
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].Color = origVerts[i].Color;
            }
        }

        protected virtual VertexPositionColorTexture[] GetVertices()
        {
            return new VertexPositionColorTexture[0];
        }
        public static VertexPositionColorTexture[] GetRectangle(PrimitiveType type, float width, float height, int rows, int columns)
        {
            float x, z, x1, z1, xpos, zpos, xpos1, zpos1;
            var verts = new VertexPositionColorTexture[(int)(rows * columns) * 6];
            for (int i = 0; i < rows; i++)
            {
                x = ((float)(i) / rows);
                x1 = ((float)(i + 1) / rows);
                xpos = (x * width) - (width / 2);
                xpos1 = (x1 * width) - (width / 2);
                for (int o =0; o < columns; o++)
                {

                    int ind = ((i * columns) + (o)) * 6;
                    z = ((float)(o) / columns);
                    z1 = ((float)(o + 1) / columns);
                    zpos = (z * height) - (height / 2);
                    zpos1 = (z1 * height) - (height / 2);
                    /*verts[0] = new VertexPositionColorTexture(new Vector3(xpos, 0, zpos), Color.White, new Vector2(x1, z1));
                    verts[1] = new VertexPositionColorTexture(new Vector3(xpos1, 0, zpos), Color.White, new Vector2(x, z1));
                    verts[2] = new VertexPositionColorTexture(new Vector3(xpos1, 0, zpos1), Color.White, new Vector2(x, z));
                    verts[3] = new VertexPositionColorTexture(new Vector3(xpos1, 0, zpos1), Color.White, new Vector2(x, z));
                    verts[4] = new VertexPositionColorTexture(new Vector3(xpos, 0, zpos1), Color.White, new Vector2(x1, z));
                    verts[5] = new VertexPositionColorTexture(new Vector3(xpos, 0, zpos), Color.White, new Vector2(x1, z1));*/
                    verts[ind + 0] = new VertexPositionColorTexture(new Vector3(xpos, 0, zpos), Color.White, new Vector2(1, 1));
                    verts[ind + 1] = new VertexPositionColorTexture(new Vector3(xpos1, 0, zpos), Color.White, new Vector2(0, 1));
                    verts[ind + 2] = new VertexPositionColorTexture(new Vector3(xpos1, 0, zpos1), Color.White, new Vector2(0, 0));
                    verts[ind + 3] = new VertexPositionColorTexture(new Vector3(xpos1, 0, zpos1), Color.White, new Vector2(0, 0));
                    verts[ind + 4] = new VertexPositionColorTexture(new Vector3(xpos, 0, zpos1), Color.White, new Vector2(1, 0));
                    verts[ind + 5] = new VertexPositionColorTexture(new Vector3(xpos, 0, zpos), Color.White, new Vector2(1, 1));
                }
            }
            return verts;
        }
        public override bool CheckIfSpriteBatchChangeNeeded(Obj o)
        {
            return false;
            return base.CheckIfSpriteBatchChangeNeeded(o);
        }
        public override Techniques GetDrawTechnique()
        {
            return Techniques.Normal;
        }
        public override void Draw()
        {
            Global.Effects.ObjPos = Pos;
            base.Draw();
        }
        protected override void DrawObj()
        {
            //base.DrawObj();
            doc.GraphicsDevice.Textures[0] = Texture;
            doc.GraphicsDevice.DrawUserPrimitives<VertexPositionColorTexture>(PrimitiveType, vertices, 0, GetPrimitiveCount());
        }
    }
}
