namespace DotNet.Basics.Diagnostics
{
    public interface ILogDispatcher
    {
        event Logger.MessageLoggedEventHandler MessageLogged;
        event Logger.TimingLoggedEventHandler TimingLogged;
        bool HasListeners { get; }
    }
}
