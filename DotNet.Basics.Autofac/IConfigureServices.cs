using Microsoft.Extensions.DependencyInjection;

namespace DotNet.Basics.Autofac
{
    public interface IConfigureServices
    {
        void Configure(IServiceCollection services);
    }
}
