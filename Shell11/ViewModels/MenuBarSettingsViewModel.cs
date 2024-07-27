using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GongSolutions.Wpf.DragDrop;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shell11.Common.Application.Contracts;
using Shell11.Services;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Security;
using System.Windows;

namespace Shell11.ViewModels
{
    public partial class MenuBarSettingsViewModel : ObservableObject, IDropTarget
    {
        private readonly IApplication app;
        private readonly IHost host;
        private readonly SettingsUIViewModel navHost;
        private IEnumerable<IWindowService> menubarList => host.Services.GetServices<IWindowService>();

        public MenuBarSettingsViewModel(IApplication app,IHost host,SettingsUIViewModel setting_host)
        {
            this.app = app;
            this.host = host;
            this.navHost = setting_host;

            Extensions =new ObservableCollection < IMenuBarExtension>( app.MenuBarExtensions);
        }

        public ObservableCollection<IMenuBarExtension> Extensions { get; private set; }

        [RelayCommand]
        void NavigateTo(object args)
        {
            if (args is IMenuBarExtension ext)
            {
                navHost.DoNavigate(ext.NavKey);
            }
        }

        [RelayCommand]
        void EnableChanged(object args)
        {
            if (args is IMenuBarExtension extension)
            {
                foreach (var item in menubarList)
                {
                    if (item is MenuBarWindowService menubar)
                    {
                        menubar.HandleExtensionStateChange(extension);
                    }
                }
            }
        }

        public void DragOver(IDropInfo dropInfo)
        {
            var sourceItem = dropInfo.Data as IMenuBarExtension;
            var targetItem = dropInfo.TargetItem as IMenuBarExtension;

            if (sourceItem != null && targetItem != null)// && targetItem.CanAcceptChildren)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = DragDropEffects.Move;
            }
        }



        public void Drop(IDropInfo dropInfo)
        {
            var source = dropInfo.Data as IMenuBarExtension;
            var target = dropInfo.TargetItem as IMenuBarExtension;
            if (source!=null && target!=null)
            {

                var targetIndex = Extensions.IndexOf(target);
                Extensions.Remove(source);
                Extensions.Insert(targetIndex, source);
            }


        }

    }
}