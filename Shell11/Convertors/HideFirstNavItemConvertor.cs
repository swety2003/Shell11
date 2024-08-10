using Shell11.Models;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Shell11.Convertors
{
    public class HideFirstNavItemConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is NavItem ni)
            {
                if (ni.fullPath.IndexOf("/") == -1)
                {
                    return Visibility.Collapsed;
                }
                else
                {

                    return Visibility.Visible;
                }
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
