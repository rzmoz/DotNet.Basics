using Microsoft.Extensions.Configuration;

namespace DotNet.Basics.Tests.Cli
{
    public class TestCliHost
    {
        public TestCliHost(IConfigurationRoot config)
        {
            MySetting = config[nameof(MySetting)];
        }

        public string MySetting { get; }
    }
}
