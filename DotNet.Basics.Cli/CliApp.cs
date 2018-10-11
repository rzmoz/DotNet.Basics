using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace DotNet.Basics.Cli
{
    public class CliApp : ICliApp
    {
        public CliApp(IConfigurationRoot configuration, IServiceProvider serviceProvider)
        {
            Configuration = configuration;
            ServiceProvider = serviceProvider;
        }

        public IConfigurationRoot Configuration { get; }
        public IServiceProvider ServiceProvider { get; }
        public async Task<int> RunAsync(Func<ICliApp, Task<int>> runAsync)
        {
            if (runAsync == null) throw new ArgumentNullException(nameof(runAsync));
            return await runAsync.Invoke(this).ConfigureAwait(false);
        }
    }
}
