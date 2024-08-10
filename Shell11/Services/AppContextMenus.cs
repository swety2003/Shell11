using CommunityToolkit.Mvvm.Input;
using iNKORE.UI.WPF.Modern.Controls;
using ManagedShell.Common.Helpers;
using Shell11.Common.Application.Contracts;
using Shell11.Common.Dialogs;
using Shell11.Common.Utils;
using Shell11.Views;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Shell11.Services
{
    public static class AppContextMenus
    {

        public static IEnumerable<IMenuItem> LogoMenuItems;
        static AppContextMenus()
        {

            var collection = new List<IMenuItem>();
            var powerGroup = new MenuItemData("电源选项");
            powerGroup.Items.Add(new MenuItemData("锁定", new RelayCommand(LockScreen)));
            powerGroup.Items.Add(new MenuItemData("注销", new RelayCommand(LogOut)));
            powerGroup.Items.Add(new MenuItemData("关机", new RelayCommand(PowerOff)));
            powerGroup.Items.Add(new MenuItemData("睡眠", new RelayCommand(Sleep)));
            powerGroup.Items.Add(new MenuItemData("重启", new RelayCommand(Reboot)));
            collection.Add(powerGroup);
            collection.Add(new SeparatorData());
            collection.Add(new MenuItemData("运行", new RelayCommand(OpenRunWindow)));
            collection.Add(new MenuItemData("任务管理器", new RelayCommand(OpenTaskMgr)));
            collection.Add(new MenuItemData("终端", new RelayCommand(OpenTerm)));
            collection.Add(new MenuItemData("设置", new RelayCommand(OpenSystemSettings)));
            collection.Add(new MenuItemData("控制面板", new RelayCommand(OpenControlPanel)));
            collection.Add(new SeparatorData());
            collection.Add(new MenuItemData("Shell设置", new RelayCommand(OpenSettings)));
            collection.Add(new MenuItemData("退出Shell", new AsyncRelayCommand(ExitApp)));

            LogoMenuItems = collection;
        }
        static async Task ExitApp()
        {
            IApplication app = Application.Current as IApplication;

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
                    app.ExitApp();
                    break;
                case ContentDialogResult.Secondary:
                    app.RestartApp();
                    break;
                default:
                    break;
            }

        }

        static void OpenSystemSettings() =>
            ShellHelper.StartProcess("ms-settings://");

        static void PowerOff() => SystemPower.ShowShutdownConfirmation();
        static void Reboot() => SystemPower.ShowRebootConfirmation();
        static void Sleep() => PowerHelper.Sleep();
        static void LockScreen() => ShellHelper.Lock();
        static void LogOut() => SystemPower.ShowLogOffConfirmation();
        static void OpenTaskMgr() => ShellHelper.StartTaskManager();
        static void OpenControlPanel() => ShellHelper.StartProcess("control.exe");
        static void OpenTerm() => ShellHelper.StartProcess("wt.exe");

        static private void OpenRunWindow()
        {
            ShellHelper.ShowRunDialog("运行", "Windows 将根据你所输入的名称，为你打开相应的程序、文件夹、文档或 Internet 资源。");
        }

        static private void OpenSettings()
        {
            IApplication app = Application.Current as IApplication;
            if (SettingsUI.Instance == null)
            {
                SettingsUI.Instance = app.GetService<SettingsUI>();
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




    }
}
