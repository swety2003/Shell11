﻿using ManagedShell;
using ManagedShell.AppBar;
using ManagedShell.WindowsTray;
using Shell11.Common.Application.Contracts;
using Shell11.Common.Application.Structs;
using Shell11.Common.Utils;
using Shell11.Interfaces;
using Shell11.ViewModels;
using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace Shell11.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MenuBarWindow : AppBarWindow, IMenuBar
    {
        private IApplication application;
        private ShellManager shellManager;
        private IWindowManager windowManager;

        public NotificationArea notificationArea => shellManager.NotificationArea;

        public MenuBarWindow() : base(null, null, null, AppBarScreen.FromPrimaryScreen(), AppBarEdge.Top, AppBarMode.Normal, 27)
        {
            InitializeComponent();
        }

        public MenuBarWindow(IApplication Application, ShellManager shellManager, IWindowManager windowManager,
            AppBarScreen screen, AppBarEdge edge, AppBarMode mode, double height = 28)
            : base(shellManager.AppBarManager, shellManager.ExplorerHelper, shellManager.FullScreenHelper, screen, edge, mode, height)
        {
            InitializeComponent();

            this.application = Application;
            this.shellManager = shellManager;
            this.windowManager = windowManager;

            var internalItem = FindResource("InternalItem") as FrameworkElement;
            DataContext = new MenuBarWindowViewModel(application, this, internalItem);

            Loaded += MenuBarWindow_Loaded;
            Closed += MenuBarWindow_Closed;
        }

        private void MenuBarWindow_Closed(object? sender, EventArgs e)
        {
            if (DataContext is MenuBarWindowViewModel vm)
            {
                vm.Dispose();
                DataContext = null;
            }

            Loaded -= MenuBarWindow_Loaded;
            Closed -= MenuBarWindow_Closed;
            this.application = null;
            this.shellManager = null;
            this.windowManager = null;
            GC.Collect();
        }

        ~MenuBarWindow() { }

        private void MenuBarWindow_Loaded(object sender, RoutedEventArgs e)
        {
            new WindowAccentCompositor(this).Composite(Color.FromArgb(232, 232, 232, 232));
        }

        public MenuBarDimensions GetDimensions()
        {
            return new MenuBarDimensions
            {
                ScreenEdge = AppBarEdge,
                DpiScale = DpiScale,
                Height = Height,
                Width = Width,
                Left = Left,
                Top = Top
            };
        }

        public IntPtr GetHandle()
        {
            return new WindowInteropHelper(this).Handle;
        }

        public bool GetIsPrimaryDisplay()
        {
            return Screen.Primary;
        }

        void IMenuBar.PeekDuringAutoHide(int msToPeek)
        {
            throw new NotImplementedException();
        }
    }
}