using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Shell11.Common.Application.Contracts
{
    public interface IExtension
    {
        void ConfigureServices(IServiceCollection services);

        //IServiceCollection Services { get; }
        abstract void SetHost(IHost host);
    }
}
