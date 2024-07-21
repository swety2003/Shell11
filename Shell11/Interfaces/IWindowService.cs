using System;
using System.Collections.Generic;
using ManagedShell.AppBar;
using Shell11.Models;

// TODO: Window services should move to the Infrastructure project

namespace Shell11.Services
{
    public interface IWindowService : IDisposable
    {
        List<AppBarWindow> Windows { get; }

        void Register();

        void HandleScreenAdded(AppBarScreen screen);

        void RefreshWindows(WindowManagerEventArgs args);

        void HandleScreenRemoved(string screenDeviceName);
    }
}
