namespace DotNet.Basics.Diagnostics
{
    public interface ILogger
    {
        event LogDispatcher.MessageLoggedEventHandler MessageLogged;
    }
}
