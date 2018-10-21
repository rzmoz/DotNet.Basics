using Microsoft.Extensions.DependencyInjection;

namespace DotNet.Basics.DependencyInjection
{
    public interface IConfigureServices
    {
        void Configure(IServiceCollection services);
    }
}
