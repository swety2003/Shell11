using Shell11.Common.Interfaces;
using Shell11.ViewModels;
using System.Windows.Controls;

namespace Shell11.Views.SettingPages
{
    /// <summary>
    /// MenuBarSettings.xaml 的交互逻辑
    /// </summary>
    public partial class MenuBarSettings : UserControl, INavigationPage
    {
        public MenuBarSettings(MenuBarSettingsViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }

    }
}
