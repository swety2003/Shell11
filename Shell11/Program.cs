using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shell11.Common.Application.Contracts;
using Shell11.Common.DependencyInjection;
using Shell11.Common.Interfaces;
using Shell11.Interfaces;
using Shell11.Services;
using Shell11.ViewModels;
using Shell11.Views;
using Shell11.Views.SettingPages;
using System;
using System.IO;
using System.Threading;

namespace Shell11
{
    static class Program
    {

        private const string MutexName = "com.shell11.app";
        private const int MutexAttempts = 10;
        private const int MutexWaitMs = 1000;

        private static IHost _host;
        private static Mutex _cairoMutex;


        [STAThread]
        public static int Main(string[] args)
        {
            if (!SingleInstanceCheck())
            {
                return 1;
            }

            _host = new HostBuilder()
                .ConfigureServices((context, services) =>
                {

                    services.AddSingleton<IInitializationService, ApplicationInitializationService>();

                    services.AddSingleton<ShellManagerService>();
                    //services.AddView<MenuBarWindow,MenuBarWindowViewModel>();

                    services.AddSingleton<IApplication, App>();
                    services.AddSingleton<IWindowManager, WindowManager>();
                    services.AddSingleton<IDesktopManager, DesktopManager>();

                    services.AddSingleton<IWindowService, MenuBarWindowService>();
                    services.AddSingleton<IWindowService, TaskbarWindowService>();

                    services.AddSingleton<INavigationService,NavigationService>();

                    services.AddSingleton<SettingsUIViewModel>();
                    services.AddSingleton<MenuBarSettingsViewModel>();


                    services.RegistorForNavigate<GeneralSettings>("general","通用");
                    services.RegistorForNavigate<MenuBarSettings>("menubar","菜单栏");
                    services.RegistorForNavigate<AdvancedSettings>("advanced","高级");
                    services.RegistorForNavigate<TaskBarSettings>("taskbar","任务栏");
                    services.RegistorForNavigate<About>("about","关于");

                    services.AddTransient(sp=>new SettingsUI { DataContext = sp.GetRequiredService<SettingsUIViewModel>() });

                    //services.AddSingletonView<SettingsUI,SettingsUIViewModel>();

                    // TODO: this should not be a property of CairoApplication... Possible solution, use Configuration?
                    var extensionPaths = new[]
                    {
                        Path.Combine(App.StartupPath, "Extensions"),
                        Path.Combine(App.CairoApplicationDataFolder, "Extensions"),
#if DEBUG
                        //Path.Combine(@"D:\Repo\CS\Shell11\Shell11.MenuBarExtensions\bin\Debug")
#endif
                    };

                    services.AddDependencyLoadingServices(context.Configuration, extensionPaths);
                })
                .ConfigureLogging((context, logging) =>
                {
                    logging.AddDebug();
                    logging.AddSimpleConsole();
#if DEBUG
                    logging.SetMinimumLevel(LogLevel.Debug);
#endif
                })
                .Build();

            var app = _host.Services.GetRequiredService<IApplication>();


            return app.Run();


        }

        private static bool GetMutex()
        {
            _cairoMutex = new Mutex(true, MutexName, out bool ok);

            return ok;
        }


        private static bool SingleInstanceCheck()
        {
            for (int i = 0; i < MutexAttempts; i++)
            {
                if (!GetMutex())
                {
                    // Dispose the mutex, otherwise it will never create new
                    _cairoMutex.Dispose();
                    System.Threading.Thread.Sleep(MutexWaitMs);
                }
                else
                {
                    return true;
                }
            }

            return false;
        }
    }
}
