
using Microsoft.Extensions.DependencyInjection;
using Shell11.Common.Application.Contracts;
using System;
using System.Composition;

namespace Shell11.MenuBarExtensions
{
    [Export(typeof(IExtension))]
    public class Extension : IExtension
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        public void SetProvider(IServiceProvider sp)
        {
            ServiceProvider = sp;
        }

        public void ConfigureServices(IServiceCollection services)
        {

        }

    }

}
