﻿using Microsoft.Extensions.DependencyInjection;
using Shell11.Common.Application.Contracts;
using Shell11.MenuBarExtensions.ViewModels;
using System;
using System.Composition;
using System.Windows;
using System.Windows.Controls;

namespace Shell11.MenuBarExtensions.Views
{
    /// <summary>
    /// MediaController.xaml 的交互逻辑
    /// </summary>
    public partial class MediaController : UserControl
    {
        public MediaController()
        {
            InitializeComponent();
            Unloaded += MediaController_Unloaded;
        }

        private void MediaController_Unloaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is MediaViewModel mediaViewModel)
            {
                mediaViewModel.Dispose();
            }
        }
    }

    [Export(typeof(IMenuBarExtension))]
    public class MediaControllerExtension : menuBarExtension
    {
        public override string NavKey => "menubar/MediaController".ToLower();

        public override string Title => "媒体控制";

        public override string Description => "控制当前播放的媒体，需要SMTC支持。";

        public override void RegisterServices(IServiceCollection services)
        {
            //services.AddKeyedSingleton<SystemTraySettings>(NavKey);
        }
        public override UserControl? StartControl(WeakReference<IMenuBar> hostref)
        {
            if (IsEnabled)
            {
                return new MediaController { DataContext = new MediaViewModel() };
            }
            return null;
        }
    }
}
