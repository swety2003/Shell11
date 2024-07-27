using Shell11.Common.Interfaces;
using Shell11.Interfaces;
using Shell11.ViewModels;
using System.Windows.Controls;

namespace Shell11.Views.SettingPages
{
    /// <summary>
    /// GeneralSettings.xaml 的交互逻辑
    /// </summary>
    public partial class GeneralSettings : UserControl,INavigationPage
    {
        public GeneralSettings(GeneralSettingsViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
