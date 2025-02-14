namespace DotNet.Basics.Serilog.Diagnostics
{
    public interface ILogDispatcher
    {
        event Log.MessageLoggedEventHandler MessageLogged;
        event Log.TimingLoggedEventHandler TimingLogged;
    }
}
