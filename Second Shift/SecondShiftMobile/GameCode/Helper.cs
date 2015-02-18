using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecondShiftMobile
{
    public static class Helper
    {
        public static void Write(Object o)
        {
#if DEBUG && !PC
            System.Diagnostics.Debug.WriteLine(o);
#endif

#if PC
            Console.WriteLine(o);
#endif

        }
    }
}
