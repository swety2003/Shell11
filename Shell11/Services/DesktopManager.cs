using ManagedShell.AppBar;
using ManagedShell.Common.Helpers;
using ManagedShell.Common.SupportingClasses;
using ManagedShell.Interop;
using ManagedShell.ShellFolders;
using ManagedShell;
using Microsoft.Extensions.Logging;
using Shell11.Common.Application.Contracts;
using Shell11.Interfaces;
using Shell11.Models;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System;
using Shell11.Common.Configuration;

namespace Shell11.Services
{

    public class DesktopManager : IDesktopManager, IDisposable
    {
        // TODO: find a better solution for these static properties
        public static bool IsEnabled => Settings.Instance.EnableDesktop && !GroupPolicyHelper.NoDesktop;



        public ShellWindow ShellWindow { get; private set; }

        private IWindowManager _windowManager;
        private readonly ShellManager _shellManager;
        private readonly IApplication _cairoApplication;
        private readonly ILogger<DesktopManager> _logger;
        //private readonly ISettingsUIService _settingsUiService;



        public bool AllowProgmanChild => ShellWindow == null && EnvironmentHelper.IsWindows8OrBetter; // doesn't work in win7 due to layered child window restrictions


        public DesktopManager(ILogger<DesktopManager> logger, IApplication cairoApplication, ShellManagerService shellManagerService)
        {
            // DesktopManager is always created on startup by WindowManager, regardless of desktop preferences
            // this allows for dynamic creation and destruction of the desktop per user preference
            _cairoApplication = cairoApplication;
            _shellManager = shellManagerService.ShellManager;
            _logger = logger;
        }

        public void Initialize(IWindowManager manager)
        {
            _windowManager = manager;
            _windowManager.DwmChanged += DwmChanged;
            _windowManager.ScreensChanged += ScreensChanged;

            InitDesktop();
        }

        private void InitDesktop()
        {
            if (!IsEnabled && !EnvironmentHelper.IsAppRunningAsShell)
                return;

            ToggleDesktopIcons(false);
        }


        private void TeardownDesktop()
        {
            ToggleDesktopIcons(true);
        }

        private void ToggleDesktopIcons(bool enable)
        {
            throw new NotImplementedException();
            //if (IsEnabled)
            //    ShellHelper.ToggleDesktopIcons(enable);
        }

        private void RegisterHotKey()
        {

            throw new NotImplementedException();
        }

        private void UnregisterHotKey()
        {

            throw new NotImplementedException();
        }


        public void ResetPosition(bool displayChanged)
        {
            ShellWindow?.SetSize();


        }

        #region Shell window
        //private void CreateShellWindow()
        //{
        //    if (!EnvironmentHelper.IsAppRunningAsShell || ShellWindow != null)
        //        return;

        //    // create native shell window; we must pass a native window's handle to SetShellWindow
        //    ShellWindow = new ShellWindow();
        //    ShellWindow.WallpaperChanged += WallpaperChanged;
        //    ShellWindow.WorkAreaChanged += WorkAreaChanged;

        //    if (ShellWindow.IsShellWindow)
        //    {
        //        // we did it
        //        _logger.LogDebug("Successfully set as shell window");
        //    }
        //}

        //private void DestroyShellWindow()
        //{
        //    if (ShellWindow == null)
        //        return;

        //    ShellWindow.WallpaperChanged -= WallpaperChanged;
        //    ShellWindow.WorkAreaChanged -= WorkAreaChanged;
        //    ShellWindow?.Dispose();
        //    ShellWindow = null;
        //}
        #endregion


        #region Event handling

        private void WorkAreaChanged(object sender, EventArgs e)
        {
            ResetPosition(false);
        }



        private void ScreensChanged(object sender, WindowManagerEventArgs e)
        {
            if (e.Reason == ScreenSetupReason.DpiChange)
            {
                // treat dpi change as display change
                ResetPosition(true);
            }
            else
            {
                ResetPosition(e.DisplaysChanged);
            }
        }

        private void DwmChanged(object sender, WindowManagerEventArgs e)
        {
            //throw new NotImplementedException();
        }

        #endregion


        public void Dispose()
        {
            if (_windowManager != null)
            {
                _windowManager.DwmChanged -= DwmChanged;
                _windowManager.ScreensChanged -= ScreensChanged;
            }

            TeardownDesktop();
        }
    }
}