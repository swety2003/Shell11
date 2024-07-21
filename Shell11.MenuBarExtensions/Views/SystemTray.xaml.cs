using ManagedShell.Interop;
using ManagedShell.WindowsTray;
using Microsoft.Extensions.DependencyInjection;
using Shell11.Common.Application.Contracts;
using Shell11.Common.Application.Structs;
using Shell11.Common.Interfaces;
using Shell11.MenuBarExtensions.ViewModels;
using Shell11.MenuBarExtensions.Views.Settings;
using Shell11.Services;
using System.Composition;
using System.Windows.Controls;

namespace Shell11.MenuBarExtensions.Views
{
    /// <summary>
    /// SystemTray.xaml 的交互逻辑
    /// </summary>
    public partial class SystemTray : UserControl
    {
        private readonly NotificationArea _notificationArea;
        internal readonly IMenuBar Host;

        public SystemTray(IMenuBar host)
        {
            InitializeComponent();
            this.Host = host;
            _notificationArea = host.notificationArea;
            _notificationArea.SetPinnedIcons(NotificationArea.DEFAULT_PINNED);
            _notificationArea.Initialize();

            DataContext = _notificationArea;


        }


        private TrayHostSizeData GetTrayHostSizeData()
        {
            MenuBarDimensions dimensions = Host.GetDimensions();

            return new TrayHostSizeData
            {
                edge = (NativeMethods.ABEdge)dimensions.ScreenEdge,
                rc = new NativeMethods.Rect
                {
                    Top = (int)(dimensions.Top * dimensions.DpiScale),
                    Left = (int)(dimensions.Left * dimensions.DpiScale),
                    Bottom = (int)((dimensions.Top + dimensions.Height) * dimensions.DpiScale),
                    Right = (int)((dimensions.Left + dimensions.Width) * dimensions.DpiScale)
                }
            };
        }
        public void SetTrayHostSizeData()
        {
            if (Host != null)
            {
                // set current menu bar to return placement for ABM_GETTASKBARPOS message
                _notificationArea.SetTrayHostSizeData(GetTrayHostSizeData());
            }
        }

    }



    [Export(typeof(IMenuBarExtension))]
    public class SystemTrayExtension : menuBarExtension
    {
        public override string NavKey => "menubar/SystemTraySettings".ToLower();

        public override void RegisterSettingsView(IServiceCollection services)
        {
            services.AddSingleton<SystemTraySettingsViewModel>();
            services.RegistorForNavigate<SystemTraySettings>(NavKey,"系统托盘设置");
        }

        public override UserControl? StartControl(IMenuBar host)
        {
            if (IsEnabled)
            {
                return new SystemTray(host);
            }
            return null;
        }
    }
}
