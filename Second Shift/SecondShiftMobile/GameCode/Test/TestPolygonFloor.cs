using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondShiftMobile.Test
{
    public class TestPolygonFloor : PolygonObj
    {
        public TestPolygonFloor(Game1 Doc, float X, float Y, float Z)
            : base(Doc, Doc.LoadTex("TestGrass"), X, Y, Z)
        {
            IsFloor = true;
            Collidable = true;
            Origin.Y = 0;
            SlowDown = false;
        }
        public override void Update()
        {
            SortingType = (Global.Camera.View.Y > Pos.Y) ? DepthSortingType.Top : DepthSortingType.ZDepth;
            base.Update();
        }
        protected override Microsoft.Xna.Framework.Graphics.VertexPositionColorTexture[] GetVertices()
        {
            float size = 4000;
            ZWidth = 4000;
            VertexPositionColorTexture[] verts = new VertexPositionColorTexture[6];
            /*verts[0] = new VertexPositionColorTexture(new Vector3(-size, 0, -size), Color.White, new Vector2(1,1));
            verts[1] = new VertexPositionColorTexture(new Vector3(size, 0, -size), Color.White, new Vector2(0, 1));
            verts[2] = new VertexPositionColorTexture(new Vector3(size, 0, size), Color.White, new Vector2(0, 0));
            verts[3] = new VertexPositionColorTexture(new Vector3(size, 0, size), Color.White, new Vector2(0,0));
            verts[4] = new VertexPositionColorTexture(new Vector3(-size, 0, size), Color.White, new Vector2(1,0));
            verts[5] = new VertexPositionColorTexture(new Vector3(-size, 0, -size), Color.White, new Vector2(1, 1));*/
            return PolygonObj.GetRectangle(PrimitiveType = Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, 8000, 8000, 20, 20);
            return verts;
        }
    }
}
