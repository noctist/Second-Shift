using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
namespace SecondShiftMobile
{
    public static class EnterText
    {
        public static Task<string> GetText()
        {
            TextForm tf = new TextForm();
            tf.Show();
            return tf.Task;
        }
    }
}
