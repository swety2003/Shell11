using ManagedShell.WindowsTasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;
using ManagedShell.Common.Logging;

namespace Shell11.Convertors
{
    [ValueConversion(typeof(bool), typeof(Style))]
    public class TaskButtonStyleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // todo
            //if (!(values[0] is FrameworkElement fxElement))
            //{
            //    return null;
            //}

            //// Default style is Inactive...
            //var fxStyle = fxElement.FindResource("CairoTaskbarButtonInactiveStyle");
            //if (values[1] == DependencyProperty.UnsetValue || values[1] == null)
            //{
            //    // Default - couldn't get window state.
            //    return fxStyle;
            //}

            //EnumUtility.TryCast(values[1], out ApplicationWindow.WindowState winState, ApplicationWindow.WindowState.Inactive);

            //if (winState == ApplicationWindow.WindowState.Active)
            //{
            //    fxStyle = fxElement.FindResource("CairoTaskbarButtonActiveStyle");
            //}

            //return fxStyle;
            return Binding.DoNothing;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public static class EnumUtility
    {
        public static bool TryCast<T>(object value, out T result, T defaultValue) where T : struct
        {
            result = defaultValue;
            try
            {
                // TODO: look at doing cast without throwing exception.
                // typeof(T).IsAssignableFrom(value.GetType());
                result = (T)value;
                return true;
            }
            catch (Exception ex)
            {
                ShellLogger.Warning($"Unable to perform cast: {ex.Message}");
            }

            return false;
        }
    }
}
