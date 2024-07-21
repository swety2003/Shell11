using Microsoft.Extensions.DependencyInjection;
using Shell11.Common.Interfaces;
using System.Collections.Generic;
using System.Windows;

namespace Shell11.Services
{
    public static class NavigationHelper
    {
        public static Dictionary<string, string> TitleMap = new Dictionary<string, string>();

        public static void RegistorForNavigate<T>(this IServiceCollection services,string key,string title = "Title") 
            where T:FrameworkElement, INavigationPage
        {
            key = key.ToLower();
            services.AddKeyedTransient<INavigationPage, T>(key);
            TitleMap.Add(key, title);
        }
    }
}
