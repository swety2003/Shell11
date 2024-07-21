using ManagedShell.AppBar;
using Shell11.Common.Application.Contracts;
using Shell11.Interfaces;
using Shell11.ViewModels;
using Shell11.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace Shell11.Services
{
    public class MenuBarWindowService : AppBarWindowService
    {
        private readonly IApplication application;

        public MenuBarWindowService(IApplication application, ShellManagerService shellManagerService, IWindowManager windowManager) 
            : base(application, shellManagerService, windowManager)
        {
            this.application = application;
            EnableMultiMon = true;
        }

        protected override void HandleSettingChange(string setting)
        {
            throw new NotImplementedException();
        }

        public void HandleExtensionStateChange(IMenuBarExtension ext)
        {

            foreach (var window in Windows)
            {
                if (window is MenuBarWindow mbWindow)
                {
                    MenuBarWindowViewModel vm = mbWindow.DataContext as MenuBarWindowViewModel;
                    if (vm != null)
                    {
                        vm.HandleExtensionStateChange(ext);
                    }
                }
            }
        }

        protected override void OpenWindow(AppBarScreen screen)
        {
            MenuBarWindow newMenuBar = new MenuBarWindow(application, _shellManager, _windowManager, screen, AppBarEdge.Top , AppBarMode.Normal);
            Windows.Add(newMenuBar);
            newMenuBar.Show();
        }
    }
}
