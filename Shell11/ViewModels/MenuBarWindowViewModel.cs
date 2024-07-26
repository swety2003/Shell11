using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using iNKORE.UI.WPF.Modern.Controls;
using ManagedShell.Common.Helpers;
using Shell11.Common.Application.Contracts;
using Shell11.Common.Dialogs;
using Shell11.Common.Utils;
using Shell11.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using Brushes = System.Windows.Media.Brushes;

namespace Shell11.ViewModels
{
    public partial class MenuBarWindowViewModel : ObservableObject, IDisposable
    {
        private IApplication application;
        private IMenuBar menuBarWindow;


        [ObservableProperty]
        IEnumerable<IMenuItem> logoMenuItems;

        public MenuBarWindowViewModel(IApplication application, IMenuBar menuBarWindow, FrameworkElement i) 
        {
            this.application = application;
            this.menuBarWindow = menuBarWindow;
            MenuExtras.Add(i);
            SetUpMenu();
            EnableMenuBarExtensions();
        }

        void SetUpMenu()
        {
            var collection = new List<IMenuItem>();
            var powerGroup = new MenuItemData("电源选项");
            powerGroup.Items.Add(new MenuItemData("锁定", LockScreenCommand));
            powerGroup.Items.Add(new MenuItemData("注销", LogOutCommand));
            powerGroup.Items.Add(new MenuItemData("关机", PowerOffCommand));
            powerGroup.Items.Add(new MenuItemData("睡眠", SleepCommand));
            powerGroup.Items.Add(new MenuItemData("重启", RebootCommand));
            collection.Add(powerGroup);
            collection.Add(new SeparatorData());
            collection.Add(new MenuItemData("运行", OpenRunWindowCommand));
            collection.Add(new MenuItemData("任务管理器", OpenTaskMgrCommand));
            collection.Add(new MenuItemData("终端", OpenTermCommand));
            collection.Add(new MenuItemData("设置", OpenSystemSettingsCommand));
            collection.Add(new MenuItemData("控制面板", OpenControlPanelCommand));
            collection.Add(new SeparatorData());
            collection.Add(new MenuItemData("Shell设置", OpenSettingsCommand));
            collection.Add(new MenuItemData("退出Shell", ExitAppCommand));

            LogoMenuItems = collection;
        }

        public ObservableCollection<FrameworkElement> MenuExtras { get; init; } = new ObservableCollection<FrameworkElement>();


        [RelayCommand]
        private async Task ExitApp()
        {

            ContentDialog dialog = new ContentDialog
            {
                Title = "提示",
                Content = "Exit Shell11?",
                PrimaryButtonText = "Exit",
                SecondaryButtonText = "Restart",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Primary,
            };

            dialog.Resources.Add("SystemControlPageBackgroundMediumAltMediumBrush", Brushes.Transparent);
            dialog.Resources.Add("ContentDialogSmokeFill", Brushes.Transparent);

            var win = new ContentDialogW(dialog);

            var result = await win.ShowAsync();
            switch (result)
            {
                case ContentDialogResult.None:
                    break;
                case ContentDialogResult.Primary:
                    application.ExitApp();
                    break;
                case ContentDialogResult.Secondary:
                    application.RestartApp();
                    break;
                default:
                    break;
            }

        }



        [RelayCommand]
        void OpenSystemSettings() =>
            ShellHelper.StartProcess("ms-settings://");

        [RelayCommand]
        void PowerOff() => SystemPower.ShowShutdownConfirmation();
        [RelayCommand]
        void Reboot() => SystemPower.ShowRebootConfirmation();
        [RelayCommand]
        void Sleep() => PowerHelper.Sleep();
        [RelayCommand]
        void LockScreen() => ShellHelper.Lock();
        [RelayCommand]
        void LogOut() => SystemPower.ShowLogOffConfirmation();
        [RelayCommand]
        void OpenTaskMgr() => ShellHelper.StartTaskManager();
        [RelayCommand]
        void OpenControlPanel() => ShellHelper.StartProcess("control.exe");
        [RelayCommand]
        void OpenTerm() => ShellHelper.StartProcess("wt.exe");

        [RelayCommand]
        private void OpenRunWindow()
        {
            ShellHelper.ShowRunDialog("运行", "Windows 将根据你所输入的名称，为你打开相应的程序、文件夹、文档或 Internet 资源。");
        }
        private bool disposedValue;

        [RelayCommand]
        private void OpenSettings()
        {
            if (SettingsUI.Instance == null)
            {
                SettingsUI.Instance = application.GetService<SettingsUI>();
                SettingsUI.Instance.Closed += (s, e) =>
                {
                    SettingsUI.Instance = null;
                };
                SettingsUI.Instance.Show();
            }
            else
            {
                SettingsUI.Instance.Activate();
            }
        }

        [RelayCommand]
        void EnableMenuBarExtensions()
        {
            foreach (var menuBarExtension in application.MenuBarExtensions)
            {
                HandleExtensionStateChange(menuBarExtension);
            }
        }

        Dictionary<IMenuBarExtension,FrameworkElement> extMap { get; } = new Dictionary<IMenuBarExtension, FrameworkElement>();

        internal void HandleExtensionStateChange(IMenuBarExtension ext)
        {
            if (ext.IsEnabled)
            {
                if (!extMap.ContainsKey(ext))
                {
                    var uc = ext.StartControl(menuBarWindow);
                    if (uc != null)
                    {
                        MenuExtras.Add(uc);
                        extMap.Add(ext,uc);
                    }
                }
            }
            else
            {
                if (extMap.ContainsKey(ext))
                {
                    var uc =extMap[ext];
                    MenuExtras.Remove(uc);
                    extMap.Remove(ext);
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                MenuExtras.Clear();
                application = null; 
                menuBarWindow = null;
                logoMenuItems = null;

                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~MenuBarWindowViewModel()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
