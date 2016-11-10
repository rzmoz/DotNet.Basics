using System.Diagnostics;

namespace DotNet.Basics
{
    public static class DebugOut
    {
        public delegate void OutputEventHandler(string message);

        public static event OutputEventHandler Out;

        public static void WriteLine(string msg)
        {
            Out?.Invoke(msg);
            Debug.WriteLine(msg);
        }
    }
}
