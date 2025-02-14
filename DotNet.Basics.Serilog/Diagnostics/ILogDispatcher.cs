namespace DotNet.Basics.Serilog.Diagnostics
{
    public interface ILogDispatcher
    {
        event Logger.MessageLoggedEventHandler MessageLogged;
        event Logger.TimingLoggedEventHandler TimingLogged;
    }
}
