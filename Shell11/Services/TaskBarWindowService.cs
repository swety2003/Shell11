using AppGrabber;
using ManagedShell.AppBar;
using Shell11.Common.Application.Contracts;
using Shell11.Common.Configuration;
using Shell11.Interfaces;
using Shell11.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Shell11.Services
{
    public class TaskbarWindowService : AppBarWindowService
    {
        //private readonly IAppGrabber _appGrabber;
        private readonly IDesktopManager _desktopManager;
        private readonly IAppGrabber appGrabber;

        public TaskbarWindowService(IApplication cairoApplication,
            ShellManagerService shellManagerService,
            IWindowManager windowManager,
            IDesktopManager desktopManager
            , IAppGrabber appGrabber
            )
            : base(cairoApplication, shellManagerService, windowManager)
        {
            //_appGrabber = appGrabber;
            _desktopManager = desktopManager;
            this.appGrabber = appGrabber;
            EnableMultiMon = Settings.Instance.EnableTaskbarMultiMon;
            EnableService = Settings.Instance.EnableTaskBar;

            if (EnableService)
            {
                _shellManager.ExplorerHelper.HideExplorerTaskbar = true;
                _shellManager.AppBarManager.AppBarEvent += AppBarEvent;
            }
        }

        private void AppBarEvent(object? sender, AppBarEventArgs e)
        {
            if (Settings.Instance.TaskbarMode == 2)
            {
                if (sender is MenuBarWindow menuBar)
                {
                    var taskbar = (TaskBarWindow)WindowManager.GetScreenWindow(Windows, menuBar.Screen);

                    if (taskbar == null)
                    {
                        return;
                    }

                    if (taskbar.AppBarEdge != menuBar.AppBarEdge)
                    {
                        return;
                    }

                    if (e.Reason == AppBarEventReason.MouseEnter)
                    {
                        taskbar.DisableAutoHide = true;
                    }
                    else if (e.Reason == AppBarEventReason.MouseLeave)
                    {
                        taskbar.DisableAutoHide = false;
                    }
                }
            }
        }


        protected override void OpenWindow(AppBarScreen screen)
        {
            var appbarmode = Settings.Instance.AutoHideTaskBar ? AppBarMode.AutoHide : AppBarMode.Normal;
            TaskBarWindow newTaskbar = new (_cairoApplication, _shellManager, _windowManager, _desktopManager,
                appGrabber,
                screen,
                AppBarEdge.Bottom, appbarmode);
            Windows.Add(newTaskbar);
            newTaskbar.Show();
        }

        public override void Dispose()
        {
            base.Dispose();

            if (EnableService)
            {
                _shellManager.AppBarManager.AppBarEvent -= AppBarEvent;
                _shellManager.ExplorerHelper.HideExplorerTaskbar = false;
            }
        }

        public override void HandleSettingChange(string setting)
        {
            switch (setting)
            {
                case "EnableTaskBar":

                    _shellManager.ExplorerHelper.HideExplorerTaskbar = Settings.Instance.EnableTaskBar;
                    HandleEnableServiceChanged(Settings.Instance.EnableTaskBar);
                    break;
                case "EnableTaskbarMultiMon":
                    HandleEnableMultiMonChanged(Settings.Instance.EnableTaskbarMultiMon);
                    break;

                case "AutoHideTaskBar":

                    HandleAutoHideChanged(Settings.Instance.AutoHideTaskBar);
                    break;
            }
        }

    }
}
