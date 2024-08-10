using ManagedShell.Common.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Shell11.Common.Application.Contracts;
using System;
using System.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Shell11.MenuBarExtensions.Views
{
    /// <summary>
    /// Clock.xaml 的交互逻辑
    /// </summary>
    public partial class Clock : UserControl
    {
        private readonly bool _isPrimaryScreen;
        private static bool isClockHotkeyRegistered;

        public Clock(WeakReference<IMenuBar> hostref)
        {
            InitializeComponent();

            hostref.TryGetTarget(out var host);
            _isPrimaryScreen = host.GetIsPrimaryDisplay();
            this.Unloaded += Clock_Unloaded;
            InitializeClock();
        }

        private void Clock_Unloaded(object sender, RoutedEventArgs e)
        {

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

        public override void RegisterServices(IServiceCollection services)
        {
            //services.AddKeyedSingleton<SystemTraySettings>(NavKey);
        }

        public override UserControl? StartControl(WeakReference<IMenuBar> hostref)
        {
            if (IsEnabled)
            {
                return new Clock(hostref);
            }
            return null;
        }
    }
}
