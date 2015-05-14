using System;
using System.Collections.Generic;
using System.Text;

namespace SecondShiftMobile.Networking
{
    public class SocketMessage
    {
        public const String SeparatorString = "|:";
        public const String EndingString = ":;";
        public string NetworkId = "";
        public URLConstructor Info = new URLConstructor();
        public SocketMessage()
        {
            Info = new URLConstructor();
        }
        public SocketMessage(string message)
        {
            var split = message.Replace(EndingString, "").Split(new string[] { "|:" }, StringSplitOptions.None);
            NetworkId = indexString(split, 0);
            Info = new URLConstructor(indexString(split, 1));
        }
        public override string ToString()
        {
            return toString();
        }
        string toString()
        {
            return NetworkId + SeparatorString + Info.ToString() +EndingString;
        }
        public byte[] GetBytes()
        {
            return Encoding.UTF8.GetBytes(toString());
        }
        string indexString(string[] split, int index)
        {
            if (split.Length > index)
            {
                return split[index];
            }
            else return "";
        }
    }
}
