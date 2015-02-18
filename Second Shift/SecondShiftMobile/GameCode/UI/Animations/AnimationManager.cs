using System;
using System.Collections.Generic;
using System.Text;

namespace SecondShiftMobile.UI.Animations
{
    public static class AnimationManager
    {
        static List<IAnimation> animations = new List<IAnimation>();
        public static void Update(float frameSpeed, float gameSpeed)
        {
            for (int i = 0; i < animations.Count; i++)
            {
                animations[i].Update(frameSpeed, gameSpeed);
                if (animations[i].ReadyToRemove)
                {
                    animations.RemoveAt(i);
                    i--;
                }
            }
        }
        public static void Add(IAnimation ani)
        {
            if (!animations.Contains(ani))
            {
                animations.Add(ani);
            }
            for (int i = 0; i < animations.Count; i++)
            {
                if (animations[i] != ani && animations[i].Object == ani.Object && animations[i].Path == ani.Path)
                {
                    animations.RemoveAt(i);
                    break;
                }
            }
        }
        public static void ClearAnimation(object obj, string path)
        {
            for (int i = 0; i < animations.Count; i++)
            {
                if (animations[i].Object == obj && animations[i].Path == path)
                {
                    animations.RemoveAt(i);
                }
            }
        }
    }
}
