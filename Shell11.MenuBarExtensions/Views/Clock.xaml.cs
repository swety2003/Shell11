using ManagedShell.Common.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Shell11.Common.Application.Contracts;
using Shell11.MenuBarExtensions.ViewModels;
using Shell11.MenuBarExtensions.Views.Settings;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Shell11.MenuBarExtensions.Views
{
    /// <summary>
    /// Clock.xaml 的交互逻辑
    /// </summary>
    public partial class Clock : UserControl
    {
        public Clock()
        {
            InitializeComponent();
        }
        private readonly bool _isPrimaryScreen;
        private static bool isClockHotkeyRegistered;

        public Clock(IMenuBar host)
        {
            InitializeComponent();

            _isPrimaryScreen = host.GetIsPrimaryDisplay();

            InitializeClock();
        }

        private void InitializeClock()
        {
            UpdateTextAndToolTip();

            // Create our timer for clock
            DispatcherTimer clock = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 500), DispatcherPriority.Background, Clock_Tick, Dispatcher);

            if (_isPrimaryScreen)
            {
                // register time changed handler to receive time zone updates for the clock to update correctly
                Microsoft.Win32.SystemEvents.TimeChanged += new EventHandler(TimeChanged);
                Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;

                //try
                //{
                //    if (!isClockHotkeyRegistered)
                //    {
                //        new HotKey(Key.D, HotKeyModifier.Win | HotKeyModifier.Alt | HotKeyModifier.NoRepeat, OnShowClock);
                //        isClockHotkeyRegistered = true;
                //    }
                //}
                //catch { }
            }
        }

        //private void OnShowClock(HotKey hotKey)
        //{
        //    ToggleClockDisplay();
        //}

        private void Dispatcher_ShutdownStarted(object sender, EventArgs e)
        {
            Microsoft.Win32.SystemEvents.TimeChanged -= new EventHandler(TimeChanged);
        }

        private void Clock_Tick(object sender, EventArgs args)
        {
            UpdateTextAndToolTip();
        }

        private void UpdateTextAndToolTip()
        {
            UpdateText();
            UpdateToolTip();
        }

        private void UpdateToolTip()
        {
            dateText.ToolTip = DateTime.Now.ToString("D");
        }

        private void UpdateText()
        {
            dateText.Text = DateTime.Now.ToString("ddd H:mm");
        }

        private void OpenTimeDateCPL(object sender, RoutedEventArgs e)
        {
            ShellHelper.StartProcess("timedate.cpl");
        }

        private void ClockMenuItem_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            monthCalendar.DisplayDate = DateTime.Now;
        }

        private void TimeChanged(object sender, EventArgs e)
        {
            TimeZoneInfo.ClearCachedData();
        }

        public void ToggleClockDisplay()
        {
            ClockMenuItem.IsSubmenuOpen = !ClockMenuItem.IsSubmenuOpen;
        }
    }



    [Export(typeof(IMenuBarExtension))]
    public class ClockExtension : menuBarExtension
    {

        public override string Title => "时钟";

        public override string Description => "显示系统时间";

        public override string NavKey => "menubar/clockSettings".ToLower();

        public override void RegisterSettingsView(IServiceCollection services)
        {
            //services.AddKeyedSingleton<SystemTraySettings>(NavKey);
        }
        public override UserControl? StartControl(IMenuBar host)
        {
            if (IsEnabled)
            {
                return new Clock(host);
            }
            return null;
        }
    }
}
