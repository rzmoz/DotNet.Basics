namespace DotNet.Basics.Serilog.Looging
{
    public interface ILoogDispatcher
    {
        event Loog.MessageLoggedEventHandler MessageLogged;
        event Loog.TimingLoggedEventHandler TimingLogged;
    }
}
