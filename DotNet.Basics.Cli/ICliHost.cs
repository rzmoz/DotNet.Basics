namespace DotNet.Basics.Cli
{
    public interface ICliHost<out T> : ICliConfiguration
    {
        T Args { get; }
    }
}
