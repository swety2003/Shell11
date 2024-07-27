using System.Collections.Generic;
using System.ComponentModel;
using ManagedShell;
using ManagedShell.AppBar;
using Shell11.Common.Application.Contracts;
using Shell11.Common.Configuration;
using Shell11.Interfaces;
using Shell11.Models;

namespace Shell11.Services
{
    public abstract class AppBarWindowService : IWindowService, IConfigurationChangeAware
    {
        public bool EnableMultiMon { get; protected set; }
        public bool EnableService { get; protected set; }

        public List<AppBarWindow> Windows { get; } = new List<AppBarWindow>();

        protected readonly IApplication _cairoApplication;
        protected readonly ShellManager _shellManager;
        protected readonly IWindowManager _windowManager;
        private readonly PropertyChangedEventHandler handler;

        protected AppBarWindowService(IApplication cairoApplication, ShellManagerService shellManagerService, IWindowManager windowManager)
        {
            _cairoApplication = cairoApplication;
            _shellManager = shellManagerService.ShellManager;
            _windowManager = windowManager;

            handler = Settings.Subscribe(this);
        }

        public void Register()
        {
            _windowManager?.RegisterWindowService(this);
        }

        public virtual void Dispose()
        {
            Settings.UnSubscribe(handler);
        }

        public void HandleScreenAdded(AppBarScreen screen)
        {
            if (EnableService && (EnableMultiMon || screen.Primary))
            {
                OpenWindow(screen);
            }
        }

        public void HandleScreenRemoved(string screenDeviceName)
        {
            if (EnableService)
            {
                CloseScreenWindow(screenDeviceName);
            }
        }

        public void RefreshWindows(WindowManagerEventArgs args)
        {
            // update screens of stale windows
            if (!EnableService)
            {
                return;
            }

            if (EnableMultiMon && !args.IsFastSetup)
            {
                foreach (AppBarScreen screen in _windowManager.ScreenState)
                {
                    AppBarWindow window = WindowManager.GetScreenWindow(Windows, screen);

                    if (window != null)
                    {
                        window.Screen = screen;
                        window.SetScreenPosition();
                    }
                }
            }
            else if (Windows.Count > 0)
            {
                Windows[0].Screen = AppBarScreen.FromPrimaryScreen();
                Windows[0].SetScreenPosition();
            }
        }

        protected void HandleEnableMultiMonChanged(bool newValue)
        {
            EnableMultiMon = newValue;

            if (!EnableService)
            {
                return;
            }

            if (EnableMultiMon)
            {
                foreach (var screen in _windowManager.ScreenState)
                {
                    bool exists = false;

                    foreach (var window in Windows)
                    {
                        if (window.Screen != null && window.Screen.DeviceName == screen.DeviceName)
                        {
                            exists = true;
                            break;
                        }
                    }

                    if (exists)
                    {
                        continue;
                    }

                    HandleScreenAdded(screen);
                }
            }
            else
            {
                foreach (var screen in _windowManager.ScreenState)
                {
                    if (screen.Primary)
                    {
                        continue;
                    }

                    CloseScreenWindow(screen.DeviceName);
                }
            }
        }

        protected void HandleEnableServiceChanged(bool newValue)
        {
            EnableService = newValue;

            if (EnableService)
            {
                foreach (var screen in _windowManager.ScreenState)
                {
                    HandleScreenAdded(screen);
                }
            }
            else
            {
                foreach (var window in Windows)
                {
                    CloseWindow(window);
                }

                Windows.Clear();
            }
        }

        protected void HandleAutoHideChanged(bool autoHideTaskBar)
        {
            var appbarmode = autoHideTaskBar ? AppBarMode.AutoHide : AppBarMode.Normal;
            foreach (var window in Windows)
            {
                window.AppBarMode = appbarmode;
            }
        }

        protected void CloseScreenWindow(string screenDeviceName)
        {
            AppBarWindow windowToClose = null;

            foreach (var window in Windows)
            {
                if (window.Screen != null && window.Screen.DeviceName == screenDeviceName)
                {
                    windowToClose = window;
                    break;
                }
            }

            if (windowToClose != null)
            {
                CloseWindow(windowToClose);

                Windows.Remove(windowToClose);
            }
        }

        protected void CloseWindow(AppBarWindow window)
        {
            if (!window.IsClosing)
            {
                window.AllowClose = true;
                window.Close();
            }
        }

        protected abstract void OpenWindow(AppBarScreen screen);

        public abstract void HandleSettingChange(string setting);

    }
}
