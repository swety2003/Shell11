using ManagedShell.AppBar;
using ManagedShell.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shell11.Common.Configuration
{
    public sealed class Settings
    {
        public static Settings Instance { get; private set; } = new Settings();
        public bool EnableDesktop { get; set; } = true;
        public bool EnableMenuBarMultiMon { get; set; }
        public bool EnableTaskbarMultiMon { get; set; }
        public int TaskbarMultiMonMode { get; set; }
        public int AutoHideShowDelayMs { get; set; } = 200;
        public int TaskbarMode { get; set; } = 2;
        public bool EnableTaskbarThumbnails { get; set; } = false;
        public bool ShowTaskbarLabels { get; set; } = true;
        public IconSize TaskbarIconSize { get; set; } = IconSize.Medium;
        public bool ShowTaskbarBadges { get; set; }
        public int TaskbarMiddleClick { get; set; }
        public AppBarEdge TaskbarEdge { get; set; }
        public bool EnableTaskBar { get; set; } = true;
    }
}
