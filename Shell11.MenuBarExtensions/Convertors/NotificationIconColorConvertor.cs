using ManagedShell.Common.Helpers;
using ManagedShell.WindowsTray;
using Shell11.Common.Configuration;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Shell11.MenuBarExtensions.Convertors
{
    internal class NotificationIconColorConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            bool invertByTheme = Settings.Instance.ColorTheme == 0;
            if (value is NotifyIcon _notifyIcon)
            {

                if (!EnvironmentHelper.IsWindows10OrBetter || _notifyIcon == null)
                {
                    return Binding.DoNothing;
                }

                string iconGuid = _notifyIcon.GUID.ToString();

                if (!(iconGuid == NotificationArea.HARDWARE_GUID ||
                    iconGuid == NotificationArea.UPDATE_GUID ||
                    iconGuid == NotificationArea.MICROPHONE_GUID ||
                    iconGuid == NotificationArea.LOCATION_GUID ||
                    iconGuid == NotificationArea.MEETNOW_GUID ||
                    iconGuid == NotificationArea.NETWORK_GUID ||
                    iconGuid == NotificationArea.POWER_GUID ||
                    iconGuid == NotificationArea.VOLUME_GUID))
                {
                    return Binding.DoNothing;
                }

                if (invertByTheme)
                {
                    return parameter;
                }
            }

            return null;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
