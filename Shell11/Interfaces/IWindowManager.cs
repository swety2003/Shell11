

using ManagedShell.AppBar;
using Shell11.Models;
using Shell11.Services;
using System;
using System.Collections.Generic;

namespace Shell11.Interfaces
{
    public interface IWindowManager
    {
        event EventHandler<WindowManagerEventArgs> DwmChanged;

        event EventHandler<WindowManagerEventArgs> ScreensChanged;

        bool IsSettingDisplays { get; set; }

        List<AppBarScreen> ScreenState { get; set; }

        void InitialSetup();

        void NotifyDisplayChange(ScreenSetupReason reason);

        void RegisterWindowService(IWindowService service);
    }
}