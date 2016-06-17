﻿using System;

namespace DotNet.Basics.Diagnostics
{
    public class ConsoleLogger : DotNetBasicsLogger
    {
        protected override void Log(LogEntry entry)
        {
            Console.WriteLine(FullFormat(entry));
        }
    }
}
