using Shell11.Common.Interfaces;
using Shell11.Interfaces;
using Shell11.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
