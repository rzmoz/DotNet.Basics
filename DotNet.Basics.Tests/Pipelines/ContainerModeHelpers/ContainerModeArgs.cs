using System;
using DotNet.Basics.Ioc;

namespace DotNet.Basics.Tests.Pipelines.ContainerModeHelpers
{
    public class ContainerModeArgs : EventArgs
    {
        public IocMode Mode { get; set; }
    }
}
