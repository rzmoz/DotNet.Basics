using System;

namespace DotNet.Basics.Tests.NetCore.EchoOut
{
    public class EchoOutProgram
    {
        static int Main(string[] args)
        {
            try
            {
                return int.Parse(args[0]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return -100000;
            }
        }
    }
}
