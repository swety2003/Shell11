using GongSolutions.Wpf.DragDrop;
using ManagedShell.Interop;
using ManagedShell.WindowsTray;
using Microsoft.Extensions.DependencyInjection;
using Shell11.Common.Application.Contracts;
using Shell11.Common.Application.Structs;
using Shell11.MenuBarExtensions.ViewModels;
using Shell11.MenuBarExtensions.Views.Settings;
using Shell11.Services;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using settings = Shell11.Common.Configuration.Settings;
namespace Shell11.MenuBarExtensions.Views
{
    /// <summary>
    /// SystemTray.xaml 的交互逻辑
    /// </summary>
    public partial class SystemTray : UserControl, IDropTarget
    {
        private readonly NotificationArea _notificationArea;
        private readonly WeakReference<IMenuBar> weakReference;

        public IMenuBar GetMenuBar()
        {

            weakReference.TryGetTarget(out var host);
            return host;
        }


        public NotificationArea NotificationArea { get; }

        public SystemTray(WeakReference<IMenuBar> weakReference)
        {
            InitializeComponent();
            weakReference.TryGetTarget(out var host);

            _notificationArea = host.notificationArea;

            _notificationArea.SetPinnedIcons(settings.Instance.PinnedNotifyIcons);
            _notificationArea.Initialize();

            NotificationArea = _notificationArea;
            this.weakReference = weakReference;


            DataContext = this;
        }

        private TrayHostSizeData GetTrayHostSizeData()
        {
            weakReference.TryGetTarget(out var host);
            MenuBarDimensions dimensions = host.GetDimensions();

            return new TrayHostSizeData
            {
                edge = (NativeMethods.ABEdge)dimensions.ScreenEdge,
                rc = new NativeMethods.Rect
                {
                    Top = (int)(dimensions.Top * dimensions.DpiScale),
                    Left = (int)(dimensions.Left * dimensions.DpiScale),
                    Bottom = (int)((dimensions.Top + dimensions.Height) * dimensions.DpiScale),
                    Right = (int)((dimensions.Left + dimensions.Width) * dimensions.DpiScale)
                }
            };
        }
        public void SetTrayHostSizeData()
        {
            _notificationArea.SetTrayHostSizeData(GetTrayHostSizeData());

        }

        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            var sourceItem = dropInfo.Data as NotifyIcon;
            var targetItem = dropInfo.TargetItem as NotifyIcon;

            if (sourceItem != null && targetItem != null)// && targetItem.CanAcceptChildren)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                dropInfo.Effects = DragDropEffects.Move;
            }
        }

        void IDropTarget.Drop(IDropInfo dropInfo)
        {
            var source = dropInfo.Data as NotifyIcon;
            var target = dropInfo.TargetItem as NotifyIcon;
            if (source != null && target != null)
            {
                if (source.IsPinned != target.IsPinned)
                {
                    if (target.IsPinned)
                    {
                        List<string> list = _notificationArea.PinnedNotifyIcons.ToList();
                        var targetIndex = list.IndexOf(target.Identifier);
                        //var removeIndex = list.IndexOf(source.Identifier);
                        if (targetIndex != -1)
                        {
                            source.Pin(targetIndex + 1);
                        }
                    }
                    else
                    {
                        source.Unpin(); ;
                    }
                }
                else
                {
                    if (target.IsPinned)
                    {

                        List<string> list = _notificationArea.PinnedNotifyIcons.ToList();
                        var removeIndex = list.IndexOf(source.Identifier);
                        var targetIndex = list.IndexOf(target.Identifier);

                        if (targetIndex != -1)
                        {
                            if (removeIndex < targetIndex)
                            {
                                source.Pin(targetIndex + 1);
                            }
                            else
                            {
                                int remIdx = removeIndex + 1;
                                if (list.Count + 1 > remIdx)
                                {
                                    source.Pin(targetIndex);
                                }
                            }
                        }

                    }
                }
                //var targetIndex = Extensions.IndexOf(target);
                //Extensions.Remove(source);
                //Extensions.Insert(targetIndex, source);
            }
        }
    }



    [Export(typeof(IMenuBarExtension))]
    public class SystemTrayExtension : menuBarExtension
    {
        public override string NavKey => "menubar/SystemTraySettings".ToLower();

        public override string Title => "系统托盘";

        public override string Description => "显示系统托盘区域图标";

        public override void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<SystemTraySettingsViewModel>();

            services.RegistorForNavigate<SystemTraySettings>(NavKey, "系统托盘设置");
        }

        public override UserControl? StartControl(WeakReference<IMenuBar> hostref)
        {
            IMenuBar host;
            hostref.TryGetTarget(out host);
            if (IsEnabled)
            {
                return new SystemTray(new WeakReference<IMenuBar>(host));
                //return Extension.ServiceProvider.GetService<SystemTray>();
            }
            return null;
        }
    }
}
