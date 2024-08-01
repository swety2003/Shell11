using AppGrabber;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ManagedShell;
using ManagedShell.AppBar;
using ManagedShell.Common.Helpers;
using ManagedShell.WindowsTasks;
using Shell11.Common.Application.Contracts;
using Shell11.Common.Configuration;
using Shell11.Common.Utils;
using Shell11.Interfaces;
using System;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Shell11.ViewModels
{
    internal partial class TaskBarWindowViewModel : ObservableObject,IDisposable, IConfigurationChangeAware
    {
        private IDesktopManager desktopManager;
        private readonly IAppGrabber appGrabber;
        private ShellManager _shellManager;
        private AppBarScreen screen;
        private IWindowManager _windowManager;
        private bool disposedValue;

        public ICollectionView TaskBarItems { get; private set; }

        public ICollectionView GroupedWindows { get; private set; }

        //[ObservableProperty]
        public Category PinnedPrograms => appGrabber.QuickLaunch;

        public TaskBarWindowViewModel(IDesktopManager desktopManager, IAppGrabber appGrabber, ShellManager shellManager, AppBarScreen screen, IWindowManager windowManager)
        {
            this.desktopManager = desktopManager;
            this.appGrabber = appGrabber;
            this._shellManager = shellManager;
            this.screen = screen;
            this._windowManager = windowManager;

            setUp();

            windowManager.ScreensChanged += WindowManager_ScreensChanged;
        }

        private void setUp()
        {
            _shellManager.Tasks.Initialize(getTaskCategoryProvider(), true);

            TaskBarItems = _shellManager.Tasks.CreateGroupedWindowsCollection();
            TaskBarItems.Filter = Tasks_Filter;

            //TaskBarItems = _taskbarItems;
            GroupedWindows = _shellManager.Tasks.GroupedWindows;

            if (TaskBarItems != null) TaskBarItems.CollectionChanged += GroupedWindows_Changed;
        }

        [RelayCommand]
        void ShowStartMenu()
        {
            ShellHelper.ShowStartMenu();
        }

        [RelayCommand]
        void ShowDesktop()
        {
            ShellHelper.ShowStartContextMenu();
        }

        private bool Tasks_Filter(object obj)
        {
            if (obj is ApplicationWindow window)
            {
                if (!window.ShowInTaskbar)
                {
                    return false;
                }

                if (!Settings.Instance.EnableTaskbarMultiMon || Settings.Instance.TaskbarMultiMonMode == 0)
                {
                    return true;
                }

                if (Settings.Instance.TaskbarMultiMonMode == 2 && screen.Primary)
                {
                    return true;
                }

                if (screen.Primary && !IsValidHMonitor(window.HMonitor))
                {
                    return true;
                }

                if (window.HMonitor != screen.HMonitor)
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsValidHMonitor(IntPtr hMonitor)
        {
            foreach (var screen in _windowManager.ScreenState)
            {
                if (screen.HMonitor == hMonitor)
                {
                    return true;
                }
            }

            return false;
        }
        private void GroupedWindows_Changed(object? sender, NotifyCollectionChangedEventArgs e)
        {
            //setTaskButtonSize();
        }

        private ITaskCategoryProvider getTaskCategoryProvider()
        {
            return new ApplicationTaskCategoryProvider();
        }
        //~TaskBarWindowViewModel()
        //{
        //    this.Dispose();
        //}

        //public void Dispose()
        //{
        //    _windowManager.ScreensChanged -= WindowManager_ScreensChanged;

        //}

        private void WindowManager_ScreensChanged(object? sender, Models.WindowManagerEventArgs e)
        {
            if (!e.DisplaysChanged || !Settings.Instance.EnableTaskbarMultiMon || Settings.Instance.TaskbarMultiMonMode == 0) return;
            // Re-filter taskbar items to pick up cases where a task's screen no longer exists
            TaskBarItems?.Refresh();

        }

        public void HandleSettingChange(string setting)
        {
            throw new NotImplementedException();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)

                    _windowManager.ScreensChanged -= WindowManager_ScreensChanged;
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~TaskBarWindowViewModel()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

    }
}