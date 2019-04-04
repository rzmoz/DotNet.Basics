using System;
using System.Collections.Generic;
using System.Text;

namespace DotNet.Basics.Diagnostics
{
    public interface ILogger
    {
        event LogDispatcher.MessageLoggedEventHandler MessageLogged;
    }
}
