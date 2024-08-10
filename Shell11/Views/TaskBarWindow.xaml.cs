using AppGrabber;
using ManagedShell;
using ManagedShell.AppBar;
using ManagedShell.Common.Helpers;
using ManagedShell.Interop;
using ManagedShell.WindowsTasks;
using Shell11.Common.Application.Contracts;
using Shell11.Common.Configuration;
using Shell11.Interfaces;
using Shell11.ViewModels;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Shell11.Views
{
    /// <summary>
    /// TaskBarWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TaskBarWindow : AppBarWindow
    {
        private IApplication application;
        //private IDesktopManager _desktopManager;
        private ShellManager _shellManager;
        private IWindowManager _windowManager;
        private IAppGrabber appGrabber;

        public IAppGrabber AppGrabber => appGrabber;

        DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1000) };

        public TaskBarWindow() : base(null, null, null, AppBarScreen.FromPrimaryScreen(), AppBarEdge.Top, AppBarMode.Normal, 34)
        {
            InitializeComponent();
        }

        public TaskBarWindow(IApplication Application,
            ShellManager shellManager,
            IWindowManager windowManager,
            //IDesktopManager desktopManager,
            IAppGrabber appGrabber,
            AppBarScreen screen,
            AppBarEdge edge, AppBarMode mode, double height = 52)
            : base(shellManager.AppBarManager, shellManager.ExplorerHelper, shellManager.FullScreenHelper, screen, edge, mode, height)
        {
            InitializeComponent();
            application = Application;
            this.appGrabber = appGrabber;
            _shellManager = shellManager;
            _windowManager = windowManager;


            AutoHideShowDelayMs = Settings.Instance.AutoHideShowDelayMs;

            if (!Screen.Primary
                && !Settings.Instance.EnableMenuBarMultiMon)
            {
                ProcessScreenChanges = true;
            }
            else
            {
                ProcessScreenChanges = false;
            }

            DataContext = new TaskBarWindowViewModel(appGrabber, shellManager, screen, windowManager);

            setupTaskbar();

            SizeChanged += TaskBarWindow_SizeChanged;

            TaskBarWindow_Loaded();

            tasks = _shellManager.Tasks.CreateGroupedWindowsCollection();
            timer.Start();
            timer.Tick += Timer_Tick;

            Closed += TaskBarWindow_Closed;

        }

        internal TaskBarWindowViewModel ViewModel => DataContext as TaskBarWindowViewModel;

        private void TaskBarWindow_Closed(object? sender, EventArgs e)
        {
            timer.Stop();
            timer = null;
            if (DataContext is TaskBarWindowViewModel vm)
            {
                vm.Dispose();
            }

            _shellManager.ExplorerHelper.HideExplorerTaskbar = false;
        }

        #region 窗口重叠隐藏
        bool IsOverlapping(RECT rect1, RECT rect2)
        {
            return !(rect1.Right < rect2.Left || rect1.Left > rect2.Right ||
                rect1.Bottom < rect2.Top || rect1.Top > rect2.Bottom);
        }

        ICollectionView tasks;
        private void Timer_Tick(object? sender, EventArgs e)
        {
            foreach (var item in tasks)
            {
                if (item is ApplicationWindow window)
                {
                    if (window.State == ApplicationWindow.WindowState.Active)
                    {
                        RECT rect = new RECT();
                        RECT rect1 = new RECT();
                        GetWindowRect(window.Handle, ref rect);
                        GetWindowRect(Handle, ref rect1);
                        DisableAutoHide = !IsOverlapping(rect, rect1);

                        return;
                    }
                }
            }
            DisableAutoHide = true;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;   // Left coordinate
            public int Top;    // Top coordinate
            public int Right;  // Right coordinate
            public int Bottom; // Bottom coordinate
        }

        #endregion


        private bool _disableAutoHide = false;
        public bool DisableAutoHide
        {
            get { return _disableAutoHide; }
            set
            {
                _disableAutoHide = value;
                OnPropertyChanged("AllowAutoHide");
            }
        }

        private bool _isPopupOpen;
        public bool IsPopupOpen
        {
            get { return _isPopupOpen; }
            set
            {
                _isPopupOpen = value;
                OnPropertyChanged("AllowAutoHide");
            }
        }
        protected override bool ShouldAllowAutoHide()
        {
            return !DisableAutoHide && !IsPopupOpen && base.ShouldAllowAutoHide();
        }

        public override void AfterAppBarPos(bool isSameCoords, NativeMethods.Rect rect)
        {
            base.AfterAppBarPos(isSameCoords, rect);
            TaskBarWindow_SizeChanged(null, null);
        }
        private async void TaskBarWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            await Task.Delay(200);
            Left = (Screen.WorkingArea.Width / DpiScale - this.Width) / 2;
        }



        private void setupTaskbar()
        {
            AutoHideElement = bdrMain;
        }


        private async Task TaskBarWindow_Loaded()
        {
            TaskBarWindow_SizeChanged(null, null);
        }

        private void TaskView_Click(object sender, RoutedEventArgs e)
        {
            ShellHelper.ShowWindowSwitcher();

        }

        private void quickLaunchList_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }

            e.Handled = true;
        }

        private void quickLaunchList_Drop(object sender, DragEventArgs e)
        {
            string[] fileNames = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (fileNames != null)
            {
                AppGrabber.AddByPath(fileNames, AppCategoryType.QuickLaunch);
            }

            e.Handled = true;
        }
    }
}
