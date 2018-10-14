namespace DotNet.Basics.Diagnostics
{
    public interface IHasLogging
    {
        event LogEntry.TaskLogEventHandler EntryLogged;
    }
}
