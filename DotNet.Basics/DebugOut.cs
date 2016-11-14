using System;

namespace DotNet.Basics
{
    public static class DebugOut
    {
        public delegate void OutputEventHandler(string message);

        public static event OutputEventHandler Out;

        public static void WriteLine(string msg)
        {
            Out?.Invoke(msg);
#if DEBUG
            //TODO:doesn't work in xunit
            Console.WriteLine(msg);
#endif
        }
        public static void WriteLine(object msg)
        {
            if (msg == null)
                return;
            WriteLine(msg.ToString());
        }
    }
}
