
using Microsoft.Extensions.DependencyInjection;
using Shell11.Common.Application.Contracts;
using System.Composition;

namespace GlazeWM.Shell11Extensions
{
    [Export(typeof(IExtension))]
    public class Extension : IExtension
    {
        public IServiceProvider ServiceProvider { get; private set; }

        public void ConfigureServices(IServiceCollection services)
        {

        }

        public void SetProvider(IServiceProvider sp)
        {
            this.ServiceProvider = sp;
        }
    }

}
