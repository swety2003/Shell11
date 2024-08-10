using Shell11.Common.Interfaces;
using Shell11.ViewModels;
using System.Windows.Controls;

namespace Shell11.Views.SettingPages
{
    /// <summary>
    /// AppGrabberSettings.xaml 的交互逻辑
    /// </summary>
    public partial class AppGrabberSettings : UserControl, INavigationPage
    {
        public AppGrabberSettings(AppGrabberSettingsViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
