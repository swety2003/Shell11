using iNKORE.UI.WPF.Modern.Controls;
using ManagedShell.Common.Helpers;
using Shell11.Common.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Shell11.Common.Utils
{

    public class SystemPower
    {
        private static async void ShowActionConfirmation(string message, string title, string okButtonText, string cancelButtonText
            , Action systemAction)
        {
            var dialog = new ContentDialogW(new ContentDialog
            {
                Title = title,
                Content = message,
                PrimaryButtonText = okButtonText,
                CloseButtonText = cancelButtonText,
                DefaultButton = ContentDialogButton.Primary

            });

            dialog.Resources.Add("SystemControlPageBackgroundMediumAltMediumBrush", Brushes.Transparent);
            dialog.Resources.Add("ContentDialogSmokeFill", Brushes.Transparent);
            switch (await dialog.ShowAsync())
            {
                case ContentDialogResult.None:
                    break;
                case ContentDialogResult.Primary:
                    systemAction.Invoke();
                    break;
                case ContentDialogResult.Secondary:
                    break;
                default:
                    break;
            }
        }

        public static void ShowShutdownConfirmation()
        {
            ShowActionConfirmation("确定要关机吗？","提示","确定","取消"
                , PowerHelper.Shutdown);
        }

        public static void ShowRebootConfirmation()
        {
            ShowActionConfirmation("确定要重启吗？", "提示", "确定", "取消"
                , PowerHelper.Reboot);
        }

        public static void ShowLogOffConfirmation()
        {
            ShowActionConfirmation("确定要注销吗？", "提示", "确定", "取消"
                , ShellHelper.Logoff);
        }
    }
}
