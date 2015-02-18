using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondShiftMobile
{
    public struct SSEVertex : IVertexType
    {
        public Vector3 Position;
        public Color Color;
        public Vector2 TexCoord;
        //public short SampleIndex;
        public static VertexDeclaration VertexDeclaration
        {
            get
            {
                return new VertexDeclaration(
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(12, VertexElementFormat.Color, VertexElementUsage.Color, 0),
                new VertexElement(16, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)//,
                //new VertexElement(24, VertexElementFormat.Short2, VertexElementUsage.PointSize, 0)
                );
            }
        }
        public SSEVertex(Vector3 pos, Color c, Vector2 texCoord)
        {
            Position = pos;
            Color = c;
            TexCoord = texCoord;
            //SampleIndex = 0;
        }
        public SSEVertex(Vector3 pos, Color c, Vector2 texCoord, short sampleIndex)
        {
            Position = pos;
            Color = c;
            TexCoord = texCoord;
            //SampleIndex = sampleIndex;
        }
        VertexDeclaration IVertexType.VertexDeclaration
        {
            get { return VertexDeclaration; }
        }
    }
}
