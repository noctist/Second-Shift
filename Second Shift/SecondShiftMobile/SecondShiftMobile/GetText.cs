using SecondShiftMobile.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SecondShiftMobile
{
    public static class EnterText
    {
        public static async Task<string> GetText()
        {
            NetworkManager.Log(App.RootFrame.ToString());
            TaskCompletionSource<Task<string>> tcs = new TaskCompletionSource<Task<string>>();
            var op = App.RootFrame.Dispatcher.BeginInvoke(() =>
                {
                    tcs.SetResult(((App.Current.RootVisual as Frame).Content as GamePage).EnterText());
                });
            var task = await tcs.Task;
            return await task;
        }
    }
}
