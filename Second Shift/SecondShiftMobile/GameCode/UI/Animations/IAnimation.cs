using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SecondShiftMobile.UI.Animations
{
    public interface IAnimation
    {
        string Path { get; set; }
        object Object { get; set; }
        bool ReadyToRemove { get; }
        void Update(float frameSpeed, float gameSpeed);
        Task AwaitCompletion();
    }
}
