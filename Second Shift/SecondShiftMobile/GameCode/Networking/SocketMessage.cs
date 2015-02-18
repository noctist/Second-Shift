using System;
using System.Collections.Generic;
using System.Text;

namespace SecondShiftMobile.Networking
{
    public class SocketMessage
    {
        public const String SeparatorString = "|:";
        public const String EndingString = ":;";
        public string ObjectId;
        public string Command;
        public string Message;
        string toString()
        {
            return ObjectId + SeparatorString + Command + SeparatorString + Message + EndingString;
        }
        public byte[] GetBytes()
        {
            return Encoding.UTF8.GetBytes(toString());
        }
    }
}
