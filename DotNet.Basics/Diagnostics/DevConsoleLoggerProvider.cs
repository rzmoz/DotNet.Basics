using Microsoft.Extensions.Logging;

namespace DotNet.Basics.Diagnostics
{
    public class DevConsoleLoggerProvider(DevConsoleOptions devConsoleOptions) : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
        {
            return new DevConsoleLogger(devConsoleOptions);
        }
        public void Dispose()
        { }
    }
}
