using System;

namespace DotNet.Basics.Tasks
{
    [Flags]
    public enum RunMode
    {
        Transient = 0,
        Singleton = 1,
        Background = 2
    }
}
