
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace SecondShiftMobile
{
    public class PolygonRect
    {
        public Vector2 pos;
        public VertexPositionTexture[] vertices;

        float x = 0, y = 0, z = 0, width = 0, height = 0;
        Texture2D tex;

        public Vector3 Origin = Vector3.Zero;
        public float Width
        {
            get
            { return width; }
        }
        public float Height
        {
            get
            { return height; }
        }
        public Vector2 TexPos
        {
            set
            {
                vertices[0].TextureCoordinate = Vector2.One + value;
                vertices[1].TextureCoordinate = new Vector2(0, 1) + value;
                vertices[2].TextureCoordinate = new Vector2(0, 0) + value;
                vertices[3].TextureCoordinate = new Vector2(0, 0) + value;
                vertices[4].TextureCoordinate = new Vector2(1, 0) + value;
                vertices[5].TextureCoordinate = new Vector2(1, 1) + value;
            }
        }
        public PolygonRect(float Width, float Height, Texture2D Tex)
        {
            width = Width; height = Height;
            vertices = new VertexPositionTexture[6];
            vertices[0] = new VertexPositionTexture(new Vector3(0, 0, 0),  Vector2.One);
            vertices[1] = new VertexPositionTexture(new Vector3(Width, 0, 0),  new Vector2(0,1));
            vertices[2] = new VertexPositionTexture(new Vector3(Width, Height, 0),  new Vector2(0,0));
            vertices[3] = new VertexPositionTexture(new Vector3(Width, Height, 0),  new Vector2(0, 0));
            vertices[4] = new VertexPositionTexture(new Vector3(0, Height, 0),  new Vector2(1, 0));
            vertices[5] = new VertexPositionTexture(new Vector3(0, 0, 0), Vector2.One);
            tex = Tex;
            Origin = Vector3.Zero;
        }
        /// <summary>
        /// Places the origin at the center of the polygons
        /// </summary>
        public void CenterOrigin()
        {
            Origin = new Vector3(width / 2, height / 2, 0);
        }
        //reset Rectangle to a default state
        public static PolygonRect Default(Texture2D tex)
        {
            return new PolygonRect(100, 100, tex);
        }
        // return positions of where the vertices would be if they were rotated
        public Vector3[] PositionsWithRotation(Vector3 rot, float scale)
        {
            var pos = new Vector3[4];
            pos[0] = Vector3.Zero;
            float l = vertices[1].Position.X;
            pos[1] = new Vector3(MyMath.LengthDirX(l, rot.Z), MyMath.LengthDirY(l, rot.Z), MyMath.LengthDirY(l, rot.Y)) * scale;
            l = vertices[4].Position.Y;
            pos[3] = new Vector3(-MyMath.LengthDirY(l, rot.Z), MyMath.LengthDirX(l, rot.Z), MyMath.LengthDirY(l, rot.X)) * scale;
            pos[2] = pos[3] + (pos[1] - pos[0]);
            return pos;
        }
    }
}
