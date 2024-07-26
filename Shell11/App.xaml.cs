using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shell11.Common.Application.Contracts;
using Shell11.Common.Configuration;
using Shell11.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

namespace Shell11
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application, IApplication
    {
        private readonly ILogger<App> logger;
        private readonly IInitializationService _initializationService;
        internal static string StartupPath => 
            Path.GetDirectoryName((Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly()).Location);
        internal static string CairoApplicationDataFolder => 
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Shell11");

        private bool IsShuttingDown;


        public App(IHost host, ILogger<App> logger,
            IInitializationService initializationService)
        {
            Host = host;
            this.logger = logger;
            this._initializationService = initializationService;


            ShutdownMode = ShutdownMode.OnExplicitShutdown;

            InitializeComponent();
        }


        public IEnumerable<IMenuBarExtension> MenuBarExtensions => Host.Services.GetServices<IMenuBarExtension>();

        public IHost Host { get; }


        public void InvokeInUIThread(Action action)
        {
            // todo
        }


        public void ExitApp()
        {
            IsShuttingDown = true;
            Host.Services.GetService<ShellManagerService>()?.ShellManager.AppBarManager.SignalGracefulShutdown();

            Dispatcher.Invoke(Shutdown, DispatcherPriority.Normal);


            Settings.Save();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);


            Settings.Load();

            var extensions = Host.Services.GetServices<IExtension>();
            foreach (var ext in extensions)
            {
                ext.SetHost(Host);
            }

            _initializationService.SetupWindowServices();
        }

        public void RestartApp()
        {
            throw new NotImplementedException();
        }

        public T? GetService<T>()
        {
            return Host.Services.GetService<T>();
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.ToString(), "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

}
