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

namespace Shell11.MenuBarExtensions
{
    /// <summary>
    /// ActionCenter.xaml 的交互逻辑
    /// </summary>
    public partial class ActionCenter : UserControl
    {
        public ActionCenter()
        {
            InitializeComponent();
        }
    }


    [Export(typeof(IMenuBarExtension))]
    public class ActionCenterExtension : menuBarExtension
    {
        public override string NavKey => "menubar/actionCenterSettings".ToLower();

        public override void RegisterSettingsView(IServiceCollection services)
        {
            //services.AddKeyedSingleton<SystemTraySettings>(NavKey);
        }
        public override UserControl? StartControl(IMenuBar host)
        {
            if (IsEnabled && host.GetIsPrimaryDisplay())
            {
                return new ActionCenter { DataContext = new ActionCenterViewModel(host) };
            }
            return null;
        }
    }
}
