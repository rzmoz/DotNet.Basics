namespace DotNet.Basics.Serilog.Diagnostics
{
    public interface ILoogDispatcher
    {
        event Loog.MessageLoggedEventHandler MessageLogged;
        event Loog.TimingLoggedEventHandler TimingLogged;
    }
}
