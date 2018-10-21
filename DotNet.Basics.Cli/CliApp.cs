using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotNet.Basics.Cli
{
    public class CliApp : ICliApp
    {
        public CliApp(IConfigurationRoot configuration, IServiceProvider serviceProvider)
        {
            Configuration = configuration ?? new ConfigurationBuilder().Build();
            ServiceProvider = serviceProvider ?? new ServiceCollection().BuildServiceProvider();
        }

        public IConfigurationRoot Configuration { get; }
        public IServiceProvider ServiceProvider { get; }

        public Task<int> RunAsync(Func<IConfigurationRoot, IServiceProvider, Task<int>> runAsync)
        {
            if (runAsync == null) throw new ArgumentNullException(nameof(runAsync));
            return runAsync.Invoke(Configuration, ServiceProvider);
        }
    }
}
