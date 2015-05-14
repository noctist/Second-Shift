using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecondShiftMobile
{
    public class Cloud : Obj
    {
        /// <summary>
        /// Create a "box" of clouds in the sky, by setting the number of clouds you want in the box, and the size of the clouds, and the size of the box
        /// </summary>
        /// <param name="numberOfClouds"></param>
        /// <param name="left">The left side of the box</param>
        /// <param name="right">The right side of the box</param>
        /// <param name="top">The top of the box</param>
        /// <param name="bottom">The bottom of the box</param>
        /// <param name="front">The front of the box</param>
        /// <param name="back">The back of the box</param>
        /// <param name="minScale">The minimum scale of clouds in the box</param>
        /// <param name="maxScale">The maximum scale of clouds in the box</param>
        public static List<Cloud> CreateCloudBox(int numberOfClouds, float left, float right, float top, float bottom, float front, float back, float minScale, float maxScale, bool list = false, float alpha = 1, Color color = new Color())
        {
            List<Cloud> clouds = null;
            if (list)
                clouds = new List<Cloud>();
            for (int i = 0; i < numberOfClouds; i++)
            {
                Cloud c = new Cloud(Global.Game, MyMath.RandomRange(left, right), MyMath.RandomRange(top, bottom), MyMath.RandomRange(front, back));
                c.Color = color;
                c.Scale = new Vector2(MyMath.RandomRange(minScale, maxScale));
                c.left = left;
                c.right = right;
                c.Speed.X = MyMath.RandomRange(5, 20);
                c.MaxAlpha = c.Alpha = alpha;
                if (list)
                    clouds.Add(c);
            }
            return clouds;
        }
        public static void CreateCloudClusters(int numberOfClusters, int cloudsPerCluster, float skyWidth, float skyHeight, float skyLength, float clusterWidth, float clusterHeight, float clusterLength, float cloudSize, float alpha)
        {
            for (int i = 0; i < numberOfClusters; i++)
            {
                float xCenter = MyMath.RandomRange(-skyWidth / 2, skyWidth / 2);
                float yCenter = MyMath.RandomRange(-20000, -20000 + skyHeight);
                float zCenter = MyMath.RandomRange(0, skyLength);
                float cloudWidth = MyMath.RandomRange(clusterWidth * 0.5f, clusterWidth * 1.5f);
                float cloudHeight = MyMath.RandomRange(clusterHeight * 0.5f, clusterHeight * 1.5f);
                float cloudLength = MyMath.RandomRange(clusterLength * 0.5f, clusterLength * 1.5f);
                CreateCloudBox(cloudsPerCluster, xCenter - (clusterWidth / 2), xCenter + (clusterWidth / 2), yCenter - (cloudHeight / 2), yCenter + (cloudHeight / 2), zCenter - (cloudLength / 2), zCenter + (cloudLength / 2), cloudSize * 0.5f, cloudSize * 1.5f, false, alpha);
            }
        }
        //bool skyBox = false;
        float left = -1000000000, right = 10000000000;
        public float MaxAlpha = 1;
        public Cloud(Game1 Doc, float X, float Y, float Z)
            :base(Doc, Doc.Content.Load<Texture2D>("Cloud3"), X, Y, Z)
        {
            //Parallel = true;
            SlowDown = false;
            SunBlockAlpha = 0.25f;
            //Active = false;
        }
        public override void Update()
        {
            if (Pos.X >= right)
            {
                AlphaSpeed = -0.01f;
                if (Alpha < 0)
                {
                    Pos.X = left;
                    Alpha = 0;
                }
            }
            else if (Alpha < MaxAlpha && Pos.X < right)
            {
                AlphaSpeed = 0.01f;
            }
            else
            {
                Alpha = MaxAlpha;
                AlphaSpeed = 0;
            }
            base.Update();
        }
        public override StageObjectProperty[] GetStageProperties()
        {
            return new StageObjectProperty[]
            {
                new StageObjectProperty() { Name = "Scale", Info = "A single float representing scale", Value = Scale.X },
                new StageObjectProperty() { Name = "Speed", Info = "The speed of the cloud object", Value = Speed },
                new StageObjectProperty() { Name = "Left", Info = "A single float representing left", Value = left },
                new StageObjectProperty() { Name = "Right", Info = "A single float representing right", Value = right },
                new StageObjectProperty() { Name = "Alpha", Info = "The alpha value of the cloud", Value = MaxAlpha },
                new StageObjectProperty() { Name = "Color", Info = "The color of the cloud", Value = Color }
            };
        }
        public override void SetStageProperties(params StageObjectProperty[] sop)
        {
            foreach (var s in sop)
            {
                if (s.Name == "Scale")
                {
                    Scale = new Vector2(s.GetFloat());
                }
                else if (s.Name == "Speed")
                {
                    Speed = s.GetVector3();
                }
                else if (s.Name == "Right")
                {
                    right = s.GetFloat();
                }
                else if (s.Name == "Left")
                {
                    left = s.GetFloat();
                }
                else if (s.Name == "Alpha")
                {
                    MaxAlpha = s.GetFloat();
                }
                else if (s.Name == "Color")
                {
                    Color = s.GetColor();
                }
            }
        }
    }
}
