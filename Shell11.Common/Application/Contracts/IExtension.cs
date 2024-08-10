using Microsoft.Extensions.DependencyInjection;

namespace Shell11.Common.Application.Contracts
{
    public interface IExtension
    {
        void ConfigureServices(IServiceCollection services);

        //IServiceCollection Services { get; }
        abstract void SetProvider(IServiceProvider sp);
    }
}
