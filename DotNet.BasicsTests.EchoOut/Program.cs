using System;

namespace NetCore.Basics.Tests.EchoOut
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
            catch (Exception)
            {
                return -100000;
            }
        }
    }
}
