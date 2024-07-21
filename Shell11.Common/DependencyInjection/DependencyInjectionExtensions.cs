using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Shell11.Common.DependencyInjection
{
    public static class DependencyInjectionExtensions
    {

        public static IServiceCollection AddDependencyLoadingServices(this IServiceCollection services, IConfiguration configuration, string path, string pattern = "*.dll")
        {
            services.LoadDependencies(path, pattern);


            return services;
        }

        public static IServiceCollection AddDependencyLoadingServices(this IServiceCollection services, IConfiguration configuration, string[] paths, string pattern = "*.dll")
        {
            foreach (string path in paths)
            {
                services.AddDependencyLoadingServices(configuration, path, pattern);
            }

            return services;
        }

        public static IServiceCollection AddSingletonView<TView, TVM>(this IServiceCollection services) where TView : FrameworkElement,new ()
            where TVM : class, INotifyPropertyChanged
        {
            services.AddSingleton<TVM>();
            services.AddSingleton(sp => new TView { DataContext = sp.GetService<TVM>() });
            return services;
        }
    }
}
