using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace DotNet.Basics.Cli
{
    public interface ICliApp
    {
        IConfigurationRoot CliArgsConfiguration { get; }
        IConfigurationRoot AppConfiguration { get; }
        IServiceProvider ServiceProvider { get; }
        Task<int> RunAsync(Func<ICliApp, Task<int>> runAsync);
    }
}
