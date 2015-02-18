using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace SecondShiftMobile
{
    public enum RoundingMode { Up, Down }
    public static class MyMath
    {
        static Random random = new Random();
        public static int ToPowerOf2(this int num, RoundingMode round)
        {
            int test = 1;
            for (int i = 1; i < 16; i++)
            {
                if (test >= num)
                {
                    if (round == RoundingMode.Up)
                        return test;
                    else return test / 2;
                }
                test *= 2;
            }
            return num;
        }
        public static bool IsPowerOf2(this int num)
        {
            int test = 1;
            for (int i = 1; i < 16; i++)
            {
                test *= 2;
                if (num == test)
                    return true;
            }
            return false;
        }
        public static Vector2 FromLengthDir(float length, float dir)
        {
            return new Vector2(LengthDirX(length, dir), LengthDirY(length, dir));
        }
        public static float Angle(this Vector2 vec)
        {
            return MathHelper.ToDegrees((float)Math.Atan2(vec.Y, vec.X));
        }
        public static Vector2 Rotate(this Vector2 vec, float angle)
        {
            float ang = vec.Angle();
            float len = vec.Length();
            return  new Vector2(LengthDirX(len, ang + angle), LengthDirY(len, ang + angle));
        }
        public static Color Between(Color col1, Color col2, float val)
        {
            Vector4 v1 = col1.ToVector4(), v2 = col2.ToVector4();
            return new Color(v1 + ((v2 - v1) * val));
        }
        public static Vector3 Between(Vector3 vec1, Vector3 vec2, float val)
        {
            return vec1 + ((vec2 - vec1) * val);
        }
        public static Vector2 Between(Vector2 vec1, Vector2 vec2, float val)
        {
            return vec1 + ((vec2 - vec1) * val);
        }
        public static float Between(float num1, float num2, float val)
        {
            return num1 + ((num2 - num1) * val);
        }
        public static float BetweenValue(float val1, float val2, float between)
        {
            return (between - val1) / (val2 - val1);
        }
        public static float RandomRange(float num1, float num2)
        {
            return num1 + ((num2 - num1) * (float)random.NextDouble());
        }
        public static float RandomRange(float num1, float num2, Random rand)
        {
            return num1 + ((num2 - num1) * (float)rand.NextDouble());
        }
        public static Vector2 RandomRange(Vector2 vec1, Vector3 vec2)
        {
            return new Vector2(RandomRange(vec1.X, vec2.X), RandomRange(vec1.Y, vec2.Y));
        }
        public static Vector3 RandomRange(Vector3 vec1, Vector3 vec2)
        {
            return new Vector3(RandomRange(vec1.X, vec2.X), RandomRange(vec1.Y, vec2.Y), RandomRange(vec1.Z, vec2.Z));
        }
        public static Vector2 RandomRange(Vector2 vec1, Vector2 vec2, Random rand)
        {
            return new Vector2(RandomRange(vec1.X, vec2.X, rand), RandomRange(vec1.Y, vec2.Y, rand));
        }
        public static Vector3 RandomRange(Vector3 vec1, Vector3 vec2, Random rand)
        {
            return new Vector3(RandomRange(vec1.X, vec2.X, rand), RandomRange(vec1.Y, vec2.Y, rand), RandomRange(vec1.Z, vec2.Z, rand));
        }
        public static float Random()
        {
            return (float)random.NextDouble();
        }
        public static float Direction(float x1, float y1, float x2, float y2)
        {
            return (float)Math.Atan2(y2 - y1, x2 - x1);
        }
        public static float Direction(Vector2 vec1, Vector2 vec2)
        {
            return (float)Direction(vec1.X, vec1.Y, vec2.X, vec2.Y);
        }
        public static float AngleDistance(Vector2 vec1, Vector2 vec2, float target)
        {
            float dir = MathHelper.ToDegrees(Direction(vec1, vec2));
            float d1 = Math.Abs(target - dir);
            float d2 = Math.Abs(target - (dir + 360));
            float d3 = Math.Abs(target - (dir - 360));
            return Math.Min(Math.Min(d1, d2), d3);
        }
        public static float ClosestAngleDifference(float dir,  float target)
        {
            float d1 = (target - dir);
            float d2 = (target - (dir + 360));
            float d3 = (target - (dir - 360));
            float abs1 = Math.Abs(d1);
            float abs2 = Math.Abs(d2);
            float abs3 = Math.Abs(d3);
            if (abs1 < abs2 && abs1 < abs3)
                return d1;
            if (abs2 < abs1 && abs2 < abs3)
                return d2;
            return d3;
        }
        public static Vector2 Direction(Vector3 vec1, Vector3 vec2)
        {
            return new Vector2(Direction(vec1.Z, vec1.X, vec2.Z, vec2.X), Direction(vec1.Z, vec1.Y, vec2.Z, vec2.Y));
        }
        public static float Distance(Vector3 pos1, Vector3 pos2)
        {
            Vector3 pos3 = pos1 - pos2;
            return (float)Math.Sqrt((pos3.X * pos3.X) + (pos3.Y * pos3.Y) + (pos3.Z * pos3.Z));
        }
        public static float Distance(Vector3 pos)
        {
            return (float)Math.Sqrt((pos.X * pos.X) + (pos.Y * pos.Y) + (pos.Z * pos.Z));
        }
        public static float LengthDirX(float Length, float Direction)
        {
            return (float)Math.Cos(MathHelper.ToRadians(Direction)) * Length;
        }
        public static float LengthDirY(float Length, float Direction)
        {
            return (float)Math.Sin(MathHelper.ToRadians(Direction)) * Length;
        }
        public static Vector2 LengthDir(float Length, float Direction)
        {
            return new Vector2(LengthDirX(Length, Direction), LengthDirY(Length, Direction));
        }

    }
}
