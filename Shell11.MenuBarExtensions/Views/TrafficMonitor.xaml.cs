using Microsoft.Extensions.DependencyInjection;
using Shell11.Common.Application.Contracts;
using Shell11.MenuBarExtensions.ViewModels;
using Shell11.MenuBarExtensions.Views.Settings;
using System;
using System.Collections.Generic;
using System.Composition;
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

namespace Shell11.MenuBarExtensions.Views
{
    /// <summary>
    /// TrafficMonitor.xaml 的交互逻辑
    /// </summary>
    public partial class TrafficMonitor : UserControl
    {
        public TrafficMonitor()
        {
            InitializeComponent();
        }
    }



    [Export(typeof(IMenuBarExtension))]
    public class TrafficMonitorExtension : menuBarExtension
    {
        public override string NavKey => "menubar/TrafficMonitorSettings".ToLower();

        public override string Title => "TrafficMonitor";

        public override string Description => "资源占用显示";

        public override void RegisterServices(IServiceCollection services)
        {
            //services.AddSingleton<SystemTraySettingsViewModel>();
            //services.RegistorForNavigate<SystemTraySettings>(NavKey, "系统托盘设置");
        }

        public override UserControl? StartControl(WeakReference<IMenuBar> hostref)
        {

            if (IsEnabled)
            {
                return new TrafficMonitor { DataContext = new TrafficMonitorViewModel() };
            }
            return null;
        }
    }
}
