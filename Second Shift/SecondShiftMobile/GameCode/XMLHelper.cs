using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Globalization;

namespace SecondShiftMobile
{
    public static class XMLHelper
    {
        public static float GetElementFloat(this XElement x, string path, float def = float.NaN)
        {
            string s = x.GetElement(path).Value;
            float f;
            if (float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out f))
                return f;
            else return def;
        }
        public static XElement GetElement(this XElement x, string path)
        {
            string[] strings = path.Split('.');
            foreach (var st in strings)
            {
                if (x != null)
                {
                    var xx = x.Element(st);
                    if (xx != null)
                    {
                        x = xx;
                        continue;
                    }
                    else
                    {
                        XElement xxx = new XElement(st);
                        x.Add(xxx);
                        x = xxx;
                        continue;
                    }
                }
                else return null;
            }
            if (x != null)
            {
                return x;
            }
            else return null;
        }
        public static float GetAttributeFloat(this XElement x, string path)
        {
            string s = x.GetAttribute(path).Value;
            float f;
            if (float.TryParse(s, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out f))
                return f;
            else return float.NaN;
        }
        public static int GetAttributeInt(this XElement x, string path)
        {
            string s = x.GetAttribute(path).Value;
            int f;
            if (int.TryParse(s, out f))
                return f;
            else return 0;
        }
        public static XAttribute GetAttribute(this XElement x, string name)
        {
            var a = x.Attribute(name);
            if (a != null)
            {
                return a;
            }
            else
            {
                a = new XAttribute(name, "");
                x.Add(a);
                return a;
            }
        }
        public static bool GetBool(this XElement x, string name, bool defaultValue = false)
        {
            var a = x.Attribute(name);
            if (a != null)
            {
                string v = a.Value;
                return a.Value == "true";
            }
            else return defaultValue;
        }
        public static void SetBool(this XElement x, string name, bool value)
        {
            var a = x.Attribute(name);
            string s = "false";
            if (value) s = "true";
            if (a != null)
            {
                a.Value = s;
            }
            else
            {
                x.Add(new XAttribute(name, s));
            }
        }
    }
}
