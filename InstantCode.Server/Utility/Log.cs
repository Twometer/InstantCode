using System;

namespace InstantCode.Server.Utility
{
    public class Log
    {
        public static void I(string tag, string message)
        {
            Write("INFO", tag, message);
        }

        public static void E(string tag, string message)
        {
            Write("ERROR", tag, message);
        }

        private static void Write(string prefix, string tag,  string msg)
        {
            Console.WriteLine($"[{DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}] [{prefix}] {tag}: {msg}");
        }
    }
}
