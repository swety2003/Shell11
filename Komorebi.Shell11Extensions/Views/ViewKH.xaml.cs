using Microsoft.Extensions.DependencyInjection;
using Shell11.Common.Application.Contracts;
using System.Composition;
using System.Windows.Controls;
using TestLib.ViewModels;


namespace Komorebi.Shell11Extensions.Views
{
    /// <summary>
    /// ViewKH.xaml 的交互逻辑
    /// </summary>
    public partial class ViewKH : UserControl
    {
        public ViewKH()
        {
            InitializeComponent();
        }
    }



    [Export(typeof(IMenuBarExtension))]
    public class ViewKHExtension : menuBarExtension
    {
        public override string NavKey => "menubar/ViewKHSettings".ToLower();

        public override string Title => "Komorebi";

        public override string Description => "Komorebi";

        public ViewKHExtension()
        {
            IsEnabled = false;
        }


        public override void RegisterServices(IServiceCollection services)
        {
            //services.AddSingleton<SystemTraySettingsViewModel>();
            //services.RegistorForNavigate<SystemTraySettings>(NavKey, "系统托盘设置");
        }

        public override UserControl? StartControl(WeakReference<IMenuBar> hostref)
        {
            //IMenuBar host;
            //hostref.TryGetTarget(out host);
            if (IsEnabled)
            {
                return new ViewKH { DataContext = new ViewKHViewModel() };
            }
            return null;
        }
    }
}
