﻿namespace DotNet.Basics.Tests.ConsoleApp
{
    public class TestCmdArgs: TestCmdAncestorArgs
    {
        //public getter and setter
        public string Prop1 { get; set; }

        //public getter 
        public string Prop2 { get; protected set; }
    }
}
