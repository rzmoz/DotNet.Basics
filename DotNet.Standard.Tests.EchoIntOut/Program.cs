using System;

namespace DotNet.Standard.Tests.EchoIntOut
{
    class Program
    {
        static int Main(string[] args)
        {
            foreach (var arg in args)
                Console.WriteLine(arg);

            try
            {
                return int.Parse(args[0]);
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine(e.ToString());
                return -100000;
            }
        }
    }
}
