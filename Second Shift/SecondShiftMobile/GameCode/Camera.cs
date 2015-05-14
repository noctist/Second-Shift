using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
namespace SecondShiftMobile
{
    public class Camera
    {
        Vector3 truePosTarget;
        /// <summary>
        /// The camera's flat position, with out the shake effect factored in
        /// </summary>
        private Vector3 Position;
        Vector2 shakePos;
        /// <summary>
        /// The camera's view position
        /// </summary>
        public Vector3 View
        {
            set { Position = truePosTarget = value; }
            get { return Position + new Vector3(shakePos, 0); }
        }

        public const float LookDirectionW = 1.5f;
        Vector2 lookDirection = Vector2.Zero;
        Vector2 lookDirectionTarget = Vector2.Zero;
        float lookDirectionSpeed = 0;
        public Vector2 BaseLookDirection
        {
            get { return lookDirection; }
        }
        public Vector2 LookDirection
        {
            get { return lookDirection; }
        }
        Vector3 prevView = Vector3.Zero;
        public Vector3 PrevView
        {
            get { return prevView; }
        }
        /// <summary>
        /// The object that the camera will follow
        /// </summary>
        public Obj Target;
        /// <summary>
        /// The speed the camera will follow objects at
        /// </summary>
        public float FollowSpeed = 0.1f;
        /// <summary>
        /// The size of the camera's depth, this is usually 500
        /// </summary>
        public float DepthSize = 500;

        /// <summary>
        /// The power that the object's zFactor is multiplied by
        /// </summary>
        public float DepthPower = 1;
        float depthSizeTarget = 500, depthSizeTarget2 = 500, depthSizeTargetSpeed = 0;
        /// <summary>
        /// The camera's current rotation
        /// </summary>
        public float Rotation = 0;

        /// <summary>
        /// This value is added to the camera target's position
        /// </summary>
        public Vector3 DeltaPos = Vector3.Zero;
        public Shake Shake;
        private Shake speedShake;
        List<Shake> shakes = new List<Shake>();
        public Vector2 CameraSize;
        public readonly Vector3 HelperPos = new Vector3(0, 0, 90);
        
        public Camera()
        {
            CameraSize = new Vector2();
            CameraSize.X = 1164;
            CameraSize.Y = CameraSize.X * (9f / 16f);
            truePosTarget = Vector3.Zero;
            Position = Vector3.Zero;
            shakePos = Vector2.Zero;
            Shake = new Shake();
            speedShake = new Shake();
            speedShake.SetShakeSize(20, 1);
            speedShake.SetShakeSpeed(0, 1);
        }
        public void Update()
        {
            prevView = View;
            Vector3 prevPos = Position;
            lookDirection += (lookDirectionTarget - lookDirection) * lookDirectionSpeed;
            if (!LevelBuilder.Active)
            {
                if (Shake != null)
                {
                    Shake.Update();
                }
                if (speedShake != null)
                {
                    //speedShake.Update();
                }
                shakePos = Shake.ShakePos + speedShake.ShakePos;
                for (int i = 0; i < shakes.Count; i++)
                {
                    var s = shakes[i];
                    s.Update();
                    shakePos += s.ShakePos;
                    if (s.Removed)
                    {
                        i--;
                    }
                }
            }
            else
            {
                shakePos = Vector2.Zero;
            }
            depthSizeTarget2 += (depthSizeTarget - depthSizeTarget2) * depthSizeTargetSpeed * Global.FrameSpeed;
            DepthSize += (depthSizeTarget2 - DepthSize) * depthSizeTargetSpeed * Global.FrameSpeed;
            if (LevelBuilder.Active)
            {
                truePosTarget += (LevelBuilder.Pos - Position) * (FollowSpeed * Global.FrameSpeed);
                Position += (truePosTarget - Position) * (FollowSpeed * Global.FrameSpeed);
            }
            else if (Target != null)
            {
                //Helper.Write((Target.pos - Target.previousPos).Length() / 60);
                truePosTarget += ((Target.Pos + DeltaPos) - Position) * (FollowSpeed * Global.Speed * (1 + ((Target.Pos - Target.PreviousPos).Length() / 30)));
                Position += (truePosTarget - Position) * (FollowSpeed * Global.Speed);
            }
            var l = MathHelper.Clamp((prevPos - Position).Length(), 0, 100);
           // Global.Output = l;
            if (l > 30 && !float.IsNaN(l))
            {
                speedShake.SetShakeSpeed(l / 360, 0.01f);
            }
            else
            {
                speedShake.SetShakeSpeed(0, 0.02f);
            }
        }
        public void Reset()
        {
            lookDirection = lookDirectionTarget;
            if (Target != null)
            {
                Position = truePosTarget = Target.Pos + DeltaPos;
            }
            Shake.SetShakeSize(0, 1);
            Shake.SetShakeSpeed(0, 1);
        }
        public static float GetZDiff(float cameraZ, float objZ, float cameraDepthSize)
        {
            return (objZ - cameraZ) + cameraDepthSize;
        }
        public static float GetZFactor(float cameraZ, float objZ, float cameraDepthSize, float cameraDepthPower)
        {
            return GetZFactor(GetZDiff(cameraZ, objZ, cameraDepthSize), cameraDepthSize, cameraDepthPower);
        }
        public Vector2 GetScreenPosition(Vector3 objPos)
        {
            return GetScreenPosition(objPos, View, DepthSize, DepthPower, LookDirection, CameraSize);
        }
        public static Vector2 GetScreenPosition(Vector3 objPos, Vector3 camPos, float cameraDepthSize, float cameraDepthPower, Vector2 lookDirection, Vector2 cameraSize)
        {
            float zFactor = GetZFactor(camPos.Z, objPos.Z, cameraDepthSize, cameraDepthPower);
            float inverseZfactor = ((1f / (zFactor)) - LookDirectionW) * zFactor;
            Vector2 os = new Vector2();
            os.X = camPos.X + ((objPos.X - camPos.X) * zFactor);
            os.Y = camPos.Y + ((objPos.Y - camPos.Y) * zFactor);
            Vector2 offset = lookDirection * (inverseZfactor);
            Vector2 multipliedOffset = offset * (new Vector2(cameraSize.X, -cameraSize.Y) * 0.5f);
            os += multipliedOffset;
            return os;
        }
        public static float GetZFactor(float zDiff, float cameraDepthSize, float cameraDepthPower)
        {
            if (zDiff != 0)
            {
                return (float)Math.Pow(cameraDepthSize / zDiff, cameraDepthPower);
            }
            else return 1f;
        }
        public void SetLookDirection(Vector2 target, float speed)
        {
            if (speed == 1)
                lookDirection = target;
            lookDirectionTarget = target;
            lookDirectionSpeed = speed;
        }
        public void AddShake(float size, float speed, float dissipationSpeed)
        {
            Shake shake = new Shake(this, size, speed, dissipationSpeed);
            shakes.Add(shake);
        }
        public void RemoveShake(Shake shake)
        {
            if (shakes.Contains(shake))
            {
                shakes.Remove(shake);
            }
        }
        public void CenterOnTarget()
        {
            if (Target != null)
            {
                Position = truePosTarget = Target.Pos + DeltaPos;
            }
        }
        public void SetDepthSize(float target, float speed, float startSize)
        {
            DepthSize = startSize;
            depthSizeTarget = target;
            depthSizeTargetSpeed = speed;
        }

    }

    public class Shake
    {
        Random rand = new Random();
        Camera camera;
        float shakespeed = 0f, shakespeedtarget = 3f, shakespeedspeed = 0;
        float shakesize = 0, shakesizetarget = 0, shakesizespeed = 0;
        float shakeaccx = 1, shakeaccy = 1;
        float shakespeedx = 0, shakespeedy = 0;
        float shakedirx = 1, shakediry = 1;
        Vector2 shakePos = Vector2.Zero;
        float dissSpeed = 0;
        public bool Removed = false;
        public Vector2 ShakePos
        {
            get { return shakePos * shakesize; }
        }
        public Shake()
        {

        }
        public Shake(Camera cam, float size, float speed, float dissapationSpeed)
        {
            int xs = rand.Next(0, 100);
            if (xs < 50)
                xs = -1;
            else xs = 1;
            int ys = rand.Next(0, 100);
            if (ys < 50)
                ys = -1;
            else ys = 1;
            shakeaccx = MyMath.RandomRange(0f, 1f) * xs;
            shakeaccy = MyMath.RandomRange(0f, 1f) * ys;
            dissSpeed = dissapationSpeed;
            shakespeed = speed;
            shakesize = size;
            camera = cam;
        }
        public void Update()
        {
            /*if ((shakespeedx < 1 && shakedirx == 1) || (shakespeedx > -1 && shakedirx == -1))
                shakespeedx += shakeaccx * shakedirx * Global.Speed;
            if ((shakespeedy < 1 && shakediry == 1) || (shakespeedy > -1 && shakediry == -1))
                shakespeedy += shakeaccy * shakediry * Global.Speed;
            shakePos.X += shakespeedx * shakespeed * Global.Speed;
            shakePos.Y += shakespeedy * shakespeed * Global.Speed;
            if (shakePos.X > 1 && shakedirx == 1)
            {
                shakeaccx = MyMath.RandomRange(0.05f, 0.2f);
                shakedirx = -1;
            }
            else if (shakePos.X < -1 && shakedirx == -1)
            {
                shakeaccx = MyMath.RandomRange(0.05f, 0.2f);
                shakedirx = 1;
            }
            if (shakePos.Y > 1 && shakediry == 1)
            {
                shakeaccy = MyMath.RandomRange(0.05f, 0.2f);
                shakediry = -1;
            }
            else if (shakePos.Y < -1 && shakediry == -1)
            {
                shakeaccy = MyMath.RandomRange(0.05f, 0.2f);
                shakediry = 1;
            }
            shakePos += (Vector2.Zero - shakePos) * 0.05f;*/
            //shakespeed += (0 - shakespeed) * shakespeedspeed * doc.speed;
            if (shakespeedx > 1)
                shakeaccx = -1 - ((float)rand.NextDouble() * 2);
            if (shakespeedy > 1)
                shakeaccy = -1 - ((float)rand.NextDouble() * 2);
            if (shakespeedx < -1)
                shakeaccx = 1 + ((float)rand.NextDouble() * 2);
            if (shakespeedy < -1)
                shakeaccy = 1 + ((float)rand.NextDouble() * 2);
            shakespeedx += shakespeed * shakeaccx * Global.Speed;
            shakespeedy += shakespeed * shakeaccy * Global.Speed;
            shakePos.X += (shakespeed * shakespeedx);
            shakePos.Y += (shakespeed * shakespeedy);
            /*if (shakeposx < -1 && movex == -1)
                movex = 1;
            if (shakeposx > 1 && movex == 1)
                movex = 1;
            if (shakeposy < -1 && movey == -1)
                movey = 1;
            if (shakeposy > 1 && movey == 1)
                movey = 1;*/
            shakePos += (Vector2.Zero - shakePos) * 0.05f * Global.Speed;
            if (camera != null)
            {
                shakespeed += -shakespeed * dissSpeed;
                if (shakespeed < 0)
                    shakespeed = 0;
                if (shakespeed < 0.05f && Math.Abs(shakePos.X * shakesize) < 1 && Math.Abs(shakePos.Y * shakesize) < 1)
                {
                    Removed = true;
                    camera.RemoveShake(this);
                }
            }
            else
            {
                shakespeed += (shakespeedtarget - shakespeed) * shakespeedspeed * Global.Speed;
            }
            shakesize += (shakesizetarget - shakesize) * shakesizespeed * Global.Speed;
        }
        public void SetShakeSize(float size, float sizeSpeed)
        {
            shakesizetarget = size;
            shakesizespeed = sizeSpeed;
        }
        public void SetShakeSpeed(float speed, float speedSpeed)
        {
            shakespeedtarget = speed;
            shakespeedspeed = speedSpeed;
        }
    }
}
