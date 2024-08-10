using ManagedShell.Common.Enums;
using ManagedShell.Interop;
using Microsoft.Extensions.DependencyInjection;
using Shell11.Common.Application.Contracts;
using Shell11.Common.Utils;
using System;
using System.Composition;
using System.Windows;
using System.Windows.Controls;

namespace Shell11.MenuBarExtensions.Views
{
    /// <summary>
    /// QuickFolder.xaml 的交互逻辑
    /// </summary>
    public partial class QuickFolder : UserControl
    {
        public QuickFolder()
        {
            InitializeComponent();

            UserName.Header = Environment.UserName;
        }


        #region Places menu items
        private void OpenMyDocs(object sender, RoutedEventArgs e)
        {
            FolderHelper.OpenLocation(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments, Environment.SpecialFolderOption.DoNotVerify));
        }

        private void OpenMyPics(object sender, RoutedEventArgs e)
        {
            FolderHelper.OpenLocation(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures, Environment.SpecialFolderOption.DoNotVerify));
        }

        private void OpenMyMusic(object sender, RoutedEventArgs e)
        {
            FolderHelper.OpenLocation(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic, Environment.SpecialFolderOption.DoNotVerify));
        }

        private void OpenMyVideos(object sender, RoutedEventArgs e)
        {
            FolderHelper.OpenLocation(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos, Environment.SpecialFolderOption.DoNotVerify));
        }

        private void OpenDownloads(object sender, RoutedEventArgs e)
        {
            FolderHelper.OpenLocation(KnownFolders.GetPath(KnownFolder.Downloads));
        }

        private void OpenMyComputer(object sender, RoutedEventArgs e)
        {
            FolderHelper.OpenLocation(ShellFolderPath.ComputerFolder.Value);
        }

        private void OpenUserFolder(object sender, RoutedEventArgs e)
        {
            FolderHelper.OpenLocation(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile, Environment.SpecialFolderOption.DoNotVerify));
        }

        private void OpenProgramFiles(object sender, RoutedEventArgs e)
        {
            FolderHelper.OpenLocation(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles, Environment.SpecialFolderOption.DoNotVerify));
        }

        private void OpenRecycleBin(object sender, RoutedEventArgs e)
        {
            FolderHelper.OpenWithShell(ShellFolderPath.RecycleBinFolder.Value);
        }
        #endregion

    }


    [Export(typeof(IMenuBarExtension))]
    public class QuickFolderExtension : menuBarExtension
    {
        public override string NavKey => "menubar/QuickFolderSettings".ToLower();

        public override string Title => "QuickFolder";

        public override string Description => "文件夹";

        public override void RegisterServices(IServiceCollection services)
        {
            //services.AddSingleton<SystemTraySettingsViewModel>();
            //services.RegistorForNavigate<SystemTraySettings>(NavKey, "系统托盘设置");
        }

        public override UserControl? StartControl(WeakReference<IMenuBar> hostref)
        {

            if (IsEnabled)
            {
                return new QuickFolder { };
            }
            return null;
        }
    }
}
