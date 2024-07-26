using ManagedShell.WindowsTasks;
using Shell11.Common.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Shell11.Convertors
{
    [ValueConversion(typeof(CollectionViewGroup), typeof(string))]
    public class TaskGroupNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is CollectionViewGroup group && group.ItemCount > 0)
            {
                if (Settings.Instance.TaskbarGroupingStyle == 0)
                {
                    return group.Name;
                }

                if (group.Items[0] is ApplicationWindow window)
                {
                    if (window.IsUWP)
                    {
                        return ManagedShell.UWPInterop.StoreAppHelper.AppList.GetAppByAumid(window.AppUserModelID).DisplayName;
                    }

                    return window.WinFileDescription;
                }
            }

            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
