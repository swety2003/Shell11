﻿using Shell11.ViewModels;
using System;
using System.Windows.Data;

namespace Shell11.Convertors
{
    [ValueConversion(typeof(CollectionViewGroup), typeof(TaskGroup))]
    public class TaskGroupConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is CollectionViewGroup group)
            {
                return new TaskGroup(group.Items);
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
