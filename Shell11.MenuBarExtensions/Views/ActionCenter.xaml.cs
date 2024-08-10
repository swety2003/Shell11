using Microsoft.Extensions.DependencyInjection;
using Shell11.Common.Application.Contracts;
using Shell11.MenuBarExtensions.ViewModels;
using System;
using System.Composition;
using System.Windows.Controls;

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
        public override string Title => "操作中心";

        public override string Description => "点击即可打开操作中心s";

        public override string NavKey => "menubar/actionCenterSettings".ToLower();

        public override void RegisterServices(IServiceCollection services)
        {
            //services.AddKeyedSingleton<SystemTraySettings>(NavKey);
        }
        public override UserControl? StartControl(WeakReference<IMenuBar> hostref)
        {

            hostref.TryGetTarget(out var host);
            if (IsEnabled && host.GetIsPrimaryDisplay())
            {
                return new ActionCenter { DataContext = new ActionCenterViewModel(hostref) };
            }
            return null;
        }
    }
}
