
using Komorebi.Shell11Extensions.KomorebiHelper.EventHandlers;
using Komorebi.Shell11Extensions.KomorebiHelper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shell11.Common.Application.Contracts;
using System.Composition;

namespace Komorebi.Shell11Extensions
{
    [Export(typeof(IExtension))]
    public class Extension : IExtension
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddSingleton<IKEventHandler, OnSubscribeEventHandler>();
            services.AddSingleton<IKEventHandler, FocusChangeHandler>();
            services.AddSingleton<IKEventHandler, FocusNamedWorkspaceHandler>();
            services.AddSingleton<IKEventHandler, TitleUpdateHandler>();
            services.AddSingleton<IKEventHandler, UncloakHandler>();
            services.AddSingleton<IKEventHandler, WorkspaceLayoutHandler>();
        }

        public void SetProvider(IServiceProvider sp)
        {
            ServiceProvider =sp;
        }
    }

}
