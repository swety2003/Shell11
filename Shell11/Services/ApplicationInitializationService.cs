using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shell11.Interfaces;
using System;

namespace Shell11.Services
{
    class ApplicationInitializationService : IInitializationService
    {
        private readonly IHost _host;
        private readonly ILogger<ApplicationInitializationService> logger;

        public ApplicationInitializationService(IHost host,
            ILogger<ApplicationInitializationService> logger)
        {
            this._host = host;
            this.logger = logger;
        }
        //public void LoadExtensions()
        //{
        //    throw new NotImplementedException();
        //}

        public void SetupWindowServices()
        {
            foreach (var service in _host.Services.GetServices<IWindowService>())
            {
                service.Register();
            }

            _host.Services.GetService<IWindowManager>()?.InitialSetup();
        }
    }
}
