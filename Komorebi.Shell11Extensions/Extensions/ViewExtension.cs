using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Komorebi.Shell11Extensions.Extensions
{
    internal static class ViewExtension
    {
        public static T GetViewModel<T>(this FrameworkElement view) where T : ObservableObject
        {
            return view.DataContext as T ?? throw new InvalidOperationException();
        }
    }
}
