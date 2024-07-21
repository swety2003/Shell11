using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Windows.Controls;

namespace Shell11.Common.Application.Contracts
{
    public interface IMenuBarExtension:INotifyPropertyChanged
    {
        string NavKey { get; }
        bool IsEnabled { get; set; }

        void RegisterSettingsView(IServiceCollection services);
        UserControl? StartControl(IMenuBar host);
    }

    public abstract partial class menuBarExtension :ObservableObject, IMenuBarExtension
    {
        [ObservableProperty]
        bool isEnabled = true;

        public abstract string NavKey { get; }

        public abstract void RegisterSettingsView(IServiceCollection services);
        public abstract UserControl? StartControl(IMenuBar host);
    }


}
