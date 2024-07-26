using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shell11.Common.Application.Contracts;
using Shell11.Services;
using System.Collections;
using System.Collections.Generic;

namespace Shell11.ViewModels
{
    public partial class MenuBarSettingsViewModel : ObservableObject
    {
        private readonly IApplication app;
        private readonly IHost host;
        private readonly SettingsUIViewModel navHost;
        private IEnumerable<IWindowService> menubarList => host.Services.GetServices<IWindowService>();

        public MenuBarSettingsViewModel(IApplication app,IHost host,SettingsUIViewModel setting_host)
        {
            this.app = app;
            this.host = host;
            this.navHost = setting_host;
        }
        public IEnumerable<IMenuBarExtension> Extensions => app.MenuBarExtensions;

        [RelayCommand]
        void NavigateTo(object args)
        {
            if (args is IMenuBarExtension ext)
            {
                navHost.DoNavigate(ext.NavKey);
            }
        }

        [RelayCommand]
        void EnableChanged(object args)
        {
            if (args is IMenuBarExtension extension)
            {
                foreach (var item in menubarList)
                {
                    if (item is MenuBarWindowService menubar)
                    {
                        menubar.HandleExtensionStateChange(extension);
                    }
                }
            }
        }


        ~MenuBarSettingsViewModel()
        {

        }

    }
}