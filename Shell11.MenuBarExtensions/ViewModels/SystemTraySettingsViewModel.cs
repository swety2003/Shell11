using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ManagedShell.WindowsTray;
using Shell11.Common.Application.Contracts;
using Shell11.Common.Configuration;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Shell11.MenuBarExtensions.ViewModels
{
    public partial class SystemTraySettingsViewModel : ObservableObject
    {
        private readonly NotificationArea notificationArea;

        public SystemTraySettingsViewModel(ShellManagerService sms)
        {
            this.notificationArea = sms.ShellManager.NotificationArea;
        }

        public ICollectionView Pinned => notificationArea.PinnedIcons;
        public ICollectionView UnPinned => notificationArea.UnpinnedIcons;


        [RelayCommand]
        void Save()
        {
            Settings.Instance.PinnedNotifyIcons = notificationArea.PinnedNotifyIcons;
        }



    }
}