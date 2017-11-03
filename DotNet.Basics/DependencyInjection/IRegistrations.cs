using Microsoft.Extensions.DependencyInjection;

namespace DotNet.Basics.DependencyInjection
{
    public interface IRegistrations
    {
        void RegisterIn(IServiceCollection services);
    }
}
