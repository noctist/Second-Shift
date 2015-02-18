using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondShiftMobile.Test
{
    public class TestPolygonGrass : PolygonObj
    {
        Vector3 windSpeed = Vector3.Zero;
        Vector3 windSpeedTarget = Vector3.Zero;
        public TestPolygonGrass(Game1 Doc, float X, float Y, float Z)
            : base(Doc, TextureFrame.FromTexture(Textures.WhiteBlock), X, Y, Z)
        {
            PrimitiveType = Microsoft.Xna.Framework.Graphics.PrimitiveType.LineStrip;
        }
        protected override Microsoft.Xna.Framework.Graphics.VertexPositionColorTexture[] GetVertices()
        {
            var verts = new VertexPositionColorTexture[10];
            for (int i = 0; i < verts.Length; i++)
            {
                verts[i] = new VertexPositionColorTexture(new Vector3(0, ((float)i / verts.Length) * -50, 0), Color.Green, new Vector2(0, (float)i / verts.Length));
            }
            return verts;
        }
        public override void ParallelUpdate()
        {
            var winds = doc.FindObjects<Elements.Wind>();
            foreach (var w in winds)
            {
                if (IsCollidingWith(w))
                {
                    Vector3 v = (Vector3)w.GetElementValue(this);
                    windSpeed += v;
                    windSpeedTarget = windSpeed * 0.1f;
                }
            }
            windSpeedTarget += (-windSpeed) * 0.2f;
            windSpeed += (windSpeedTarget - windSpeed) * 0.05f;
            for (int i = 0; i < vertices.Length; i++)
            {
                float val = ((float)i / vertices.Length);
                float origL = val * 50;
                Vector3 v = origVerts[i].Position + (windSpeed * (float)Math.Pow(val, 2));
                var l = v.Length();
                if (l > origL)
                {
                    v *= origL / l;
                }
                vertices[i].Position = v;
            }
            base.ParallelUpdate();
        }
    }
}
