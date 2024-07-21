using Microsoft.Extensions.DependencyInjection;
using Shell11.Common.Application.Contracts;
using Shell11.MenuBarExtensions.ViewModels;
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
    /// MediaController.xaml 的交互逻辑
    /// </summary>
    public partial class MediaController : UserControl
    {
        public MediaController()
        {
            InitializeComponent();
        }
    }

    [Export(typeof(IMenuBarExtension))]
    public class MediaControllerExtension : menuBarExtension
    {
        public override string NavKey => "menubar/MediaController".ToLower();

        public override void RegisterSettingsView(IServiceCollection services)
        {
            //services.AddKeyedSingleton<SystemTraySettings>(NavKey);
        }
        public override UserControl? StartControl(IMenuBar host)
        {
            if (IsEnabled)
            {
                return new MediaController { DataContext = new MediaViewModel() };
            }
            return null;
        }
    }
}
