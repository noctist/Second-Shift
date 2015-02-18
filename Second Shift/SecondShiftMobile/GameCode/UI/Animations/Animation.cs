using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SecondShiftMobile.UI.Animations
{
    public enum AnimationSpeedType { GameSpeed, FrameSpeed, None }
    public abstract class Animation<T> : IAnimation
    {
        public string Path { get; set; }
        public object Object { get; set; }
        FieldAndObjectInfo info;
        T startValue, targetValue;
        float progress = 0;
        public float FrameDuration = 0;
        public float StartFrame = 0;
        public float EndPower = 1, StartPower = 1;
        float currentFrame = 0;
        bool readyToRemove = false;
        public AnimationSpeedType SpeedType = AnimationSpeedType.FrameSpeed;
        public bool ReadyToRemove { get { return readyToRemove; } }
        TaskCompletionSource<bool> completionTask = null;
        public Animation(object obj, string valuePath, T targetValue)
        {
            Path = valuePath; Object = obj;
            info = GameExtensions.GetFieldInfo(valuePath, obj);
            startValue = GetStartingValue(obj, valuePath, targetValue);
            this.targetValue = targetValue;
        }
        protected virtual T GetStartingValue(object obj, string valuePath, T fallbackValue)
        {
            
            if (info.FieldInfo.GetValue(info.Object) is T)
            {
                return (T)info.FieldInfo.GetValue(info.Object);
            }
            return fallbackValue;
        }
        public void Begin()
        {
            AnimationManager.Add(this);
        }
        public async Task AwaitCompletion()
        {
            if (ReadyToRemove)
            {
                return;
            }
            else
            {
                if (completionTask == null)
                    completionTask = new TaskCompletionSource<bool>();
                await completionTask.Task;
                return;
            }
        }
        public void Stop()
        {

        }
        public void Update(float frameSpeed, float gameSpeed)
        {
            currentFrame += (SpeedType == AnimationSpeedType.FrameSpeed) ? frameSpeed : (SpeedType == AnimationSpeedType.GameSpeed) ? gameSpeed : 1;
            float progress = (currentFrame - StartFrame) / FrameDuration;
            if (progress < 0)
                progress = 0;
            if (progress > 1)
                progress = 1;
            readyToRemove = progress >= 1;
            if (readyToRemove && completionTask != null)
            {
                completionTask.TrySetResult(true);
            }
            progress = (float)Math.Pow(progress, StartPower);
            progress = 1 - (float)Math.Pow(1 - progress, EndPower);
            SetValue(startValue, targetValue, progress, info);
        }
        protected abstract void SetValue(T startValue, T endValue, float progress, FieldAndObjectInfo info);
    }
}
