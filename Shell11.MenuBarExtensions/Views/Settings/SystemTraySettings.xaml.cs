using Shell11.Common.Interfaces;
using Shell11.MenuBarExtensions.ViewModels;
using System.Windows.Controls;

namespace Shell11.MenuBarExtensions.Views.Settings
{
    /// <summary>
    /// SystemTraySettings.xaml 的交互逻辑
    /// </summary>
    public partial class SystemTraySettings : UserControl, INavigationPage
    {
        public SystemTraySettings(SystemTraySettingsViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
