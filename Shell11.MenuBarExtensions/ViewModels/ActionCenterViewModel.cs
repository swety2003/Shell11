using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ManagedShell.Common.Helpers;
using Shell11.Common.Application.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shell11.MenuBarExtensions.ViewModels
{
    internal partial class ActionCenterViewModel : ObservableObject
    {
        private IMenuBar host;

        public ActionCenterViewModel(IMenuBar host)
        {
            this.host = host;
        }

        [RelayCommand]
        void ShowActionCenter()
        {

            if (EnvironmentHelper.IsWindows11OrBetter)
            {
                ShellHelper.ShowNotificationCenter();
            }
            else
            {
                ShellHelper.ShowActionCenter();
            }
        }
    }
}
