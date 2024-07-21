using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using iNKORE.UI.WPF.Modern.Controls;
using Shell11.Models;
using Shell11.Services;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Shell11.ViewModels
{
    public partial class SettingsUIViewModel : ObservableObject
    {
        private readonly INavigationService navigationService;

        [ObservableProperty] FrameworkElement content;

        [ObservableProperty] bool needRestart = false;

        [ObservableProperty] object navIndex = 0;

        partial void OnNavIndexChanged(object value)
        {
            if (value is NavigationViewItem item)
            {

                DoNavigate(item.Tag);
            }
        }

        public SettingsUIViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;
        }

        [ObservableProperty]
        IEnumerable<NavItem> navPaths;

        static List<NavItem> ParseNavItems(string input)
        {
            List<NavItem> navItems = new List<NavItem>();
            string[] parts = input.Split('/');
            string fullPath = "";

            foreach (string part in parts)
            {
                fullPath = string.IsNullOrEmpty(fullPath) ? part : $"{fullPath}/{part}";
                NavigationHelper.TitleMap.TryGetValue(fullPath,out var name);
                navItems.Add(new NavItem(name??part, fullPath));
            }

            return navItems;
        }


        string currentPath = "";
        [RelayCommand]
        internal void DoNavigate(object parameter)
        {
            if (parameter is string key)
            {
                key = key.ToLower();
                if (currentPath==key)
                {
                    return;
                }
                currentPath = key;
                NavPaths = ParseNavItems(key);

                var page = navigationService.GetPage(key.ToLower());
                if (page!=null)
                {
                    Content = page;
                }
            }
        }
    }
}