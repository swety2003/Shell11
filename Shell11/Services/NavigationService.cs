using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shell11.Common.Interfaces;
using System.Windows;
using System.Windows.Controls;

namespace Shell11.Services
{
    class NavigationService : INavigationService
    {
        private readonly IHost host;

        public NavigationService(IHost host)
        {
            this.host = host;
        }

        public FrameworkElement? GetPage(string key)
        {
            var page = host.Services.GetKeyedService<INavigationPage>(key);
            return page as FrameworkElement ?? new TextBlock { Text = $"Requested Page {key} 404 Not Found!" }; ;
        }
    }
}
