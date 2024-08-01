﻿using ManagedShell.Common.Helpers;
using Shell11.Common.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shell11.Common.Utils
{
    // TODO: Convert this to a service that uses DI to get IDesktopManager and require consumers to get it from ServiceProvider or DI
    public class FolderHelper
    {
        public static bool OpenLocation(string path)
        {
            //if (Settings.Instance.EnableDynamicDesktop && Settings.Instance.FoldersOpenDesktopOverlay && DesktopManager.IsEnabled)
            //{
            //    try
            //    {
            //        var desktopManager = CairoApplication.Current.Host.Services.GetService<IDesktopManager>();

            //        desktopManager.NavigationManager.NavigateTo(path);
            //        desktopManager.IsOverlayOpen = true;

            //        return true;
            //    }
            //    catch
            //    {
            //        return false;
            //    }
            //}

            return OpenWithShell(path);
        }

        public static bool OpenWithShell(string path)
        {
            //var desktopManager = CairoApplication.Current.Host.Services.GetService<IDesktopManager>();

            //desktopManager.IsOverlayOpen = false;

            var args = Environment.ExpandEnvironmentVariables(path);
            var filename = Environment.ExpandEnvironmentVariables(Settings.Instance.FileManager);

            return ShellHelper.StartProcess(filename, $@"""{args}""");
        }
    }
}
