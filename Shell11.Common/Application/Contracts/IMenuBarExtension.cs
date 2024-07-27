using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Windows.Controls;

namespace Shell11.Common.Application.Contracts
{
    public interface IMenuBarExtension:INotifyPropertyChanged
    {
        string NavKey { get; }
        string Title { get; }
        string Description { get; }
        bool IsEnabled { get; set; }

        void RegisterServices(IServiceCollection services);
        UserControl? StartControl(WeakReference<IMenuBar> hostref);
    }

    public abstract partial class menuBarExtension :ObservableObject, IMenuBarExtension
    {
        [ObservableProperty]
        bool isEnabled = true;

        public abstract string NavKey { get; }
        public abstract string Title { get; }
        public abstract string Description { get; }

        public abstract void RegisterServices(IServiceCollection services);
        public abstract UserControl? StartControl(WeakReference<IMenuBar> hostref);
    }


}
