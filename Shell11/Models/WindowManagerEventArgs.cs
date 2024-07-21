using ManagedShell.AppBar;
using System;

namespace Shell11.Models
{
    public class WindowManagerEventArgs : EventArgs
    {
        public bool DisplaysChanged;
        public bool IsFastSetup;
        public ScreenSetupReason Reason;
    }
}