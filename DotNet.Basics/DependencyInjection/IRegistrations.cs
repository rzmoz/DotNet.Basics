namespace Microsoft.Extensions.DependencyInjection
{
    public interface IRegistrations
    {
        void RegisterIn(IServiceCollection services);
    }
}
