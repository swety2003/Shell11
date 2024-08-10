using GlazeWM.Shell11Extensions.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Shell11.Common.Application.Contracts;
using System.Composition;
using System.Windows.Controls;

namespace GlazeWM.Shell11Extensions.Views
{
    /// <summary>
    /// GlazeWm.xaml 的交互逻辑
    /// </summary>
    public partial class GlazeWm : UserControl
    {
        public GlazeWm()
        {
            InitializeComponent();
        }
    }

    [Export(typeof(IMenuBarExtension))]
    public class GlazeWmMenuBarExtension : menuBarExtension
    {
        public override string NavKey => "";

        public override string Title => "GlazeWm";

        public override string Description => "GlazeWm Ext";

        public override void RegisterServices(IServiceCollection services)
        {

        }

        public override UserControl? StartControl(WeakReference<IMenuBar> hostref)
        {
            if (IsEnabled)
            {
                return new GlazeWm { DataContext = new GlazeWmViewModel() };
            }
            else
            {
                return null;
            }
        }
    }
}
