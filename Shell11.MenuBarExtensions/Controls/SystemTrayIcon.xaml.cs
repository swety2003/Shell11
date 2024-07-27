using ManagedShell.Common.Helpers;
using ManagedShell.Interop;
using ManagedShell.WindowsTray;
using Microsoft.Extensions.Hosting;
using Shell11.MenuBarExtensions.Shaders;
using Shell11.MenuBarExtensions.Views;
using System;
using System.Collections.Generic;
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

namespace Shell11.MenuBarExtensions.Controls
{
    /// <summary>
    /// SystemTrayIcon.xaml 的交互逻辑
    /// </summary>
    public partial class SystemTrayIcon : UserControl
    {
        public SystemTrayIcon()
        {
            InitializeComponent();
        }


        public static DependencyProperty BalloonProperty = DependencyProperty.Register("Balloon", typeof(NotificationBalloon), typeof(SystemTrayIcon));
        public static DependencyProperty HostProperty = DependencyProperty.Register("Host", typeof(SystemTray), typeof(SystemTrayIcon));

        public NotificationBalloon Balloon
        {
            get { return (NotificationBalloon)GetValue(BalloonProperty); }
            set { SetValue(BalloonProperty, value); }
        }

        public SystemTray Host
        {
            get { return (SystemTray)GetValue(HostProperty); }
            set { SetValue(HostProperty, value); }
        }

        private DispatcherTimer _balloonTimer;
        private bool _isLoaded;
        private NotifyIcon _notifyIcon => DataContext as NotifyIcon;


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_isLoaded)
            {
                return;
            }


            if (_notifyIcon == null)
            {
                return;
            }

            applyEffects();

            _notifyIcon.NotificationBalloonShown += TrayIcon_NotificationBalloonShown;

            // If a notification was received before we started listening, it will be here. Show the first one that is not expired.
            NotificationBalloon firstUnexpiredNotification = _notifyIcon.MissedNotifications.FirstOrDefault(balloon => balloon.Received.AddMilliseconds(GetAdjustedBalloonTimeout(balloon)) > DateTime.Now);

            if (firstUnexpiredNotification != null)
            {
                showBalloon(firstUnexpiredNotification);
                _notifyIcon.MissedNotifications.Remove(firstUnexpiredNotification);
            }

            _isLoaded = true;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_notifyIcon != null)
            {
                _notifyIcon.NotificationBalloonShown -= TrayIcon_NotificationBalloonShown;
            }

            _isLoaded = false;
        }


        private void applyEffects()
        {
            return;
            if (!EnvironmentHelper.IsWindows10OrBetter || _notifyIcon == null)
            {
                return;
            }

            string iconGuid = _notifyIcon.GUID.ToString();

            if (!(iconGuid == NotificationArea.HARDWARE_GUID ||
                iconGuid == NotificationArea.UPDATE_GUID ||
                iconGuid == NotificationArea.MICROPHONE_GUID ||
                iconGuid == NotificationArea.LOCATION_GUID ||
                iconGuid == NotificationArea.MEETNOW_GUID ||
                iconGuid == NotificationArea.NETWORK_GUID ||
                iconGuid == NotificationArea.POWER_GUID ||
                iconGuid == NotificationArea.VOLUME_GUID))
            {
                return;
            }

            bool invertByTheme = true;

            if (NotifyIconImage.Effect == null != invertByTheme)
            {
                return;
            }

            if (invertByTheme)
            {
                NotifyIconImage.Effect = new InvertEffect();
            }
            else
            {
                NotifyIconImage.Effect = null;
            }
        }

        #region Notify icon image mouse events
        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            var trayIcon = (sender as FrameworkElement).DataContext as NotifyIcon;

            // set current menu bar to return placement for ABM_GETTASKBARPOS message
            Host?.SetTrayHostSizeData();

            trayIcon?.IconMouseDown(e.ChangedButton, MouseHelper.GetCursorPositionParam(), System.Windows.Forms.SystemInformation.DoubleClickTime);
        }

        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            var trayIcon = (sender as FrameworkElement).DataContext as NotifyIcon;

            trayIcon?.IconMouseUp(e.ChangedButton, MouseHelper.GetCursorPositionParam(), System.Windows.Forms.SystemInformation.DoubleClickTime);
        }

        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            FrameworkElement sendingDecorator = sender as FrameworkElement;
            var trayIcon = sendingDecorator.DataContext as NotifyIcon;

            if (trayIcon == null)
            {
                return;
            }

            // update icon position for Shell_NotifyIconGetRect
            Point location = sendingDecorator.PointToScreen(new Point(0, 0));
            double dpiScale = PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice.M11;

            trayIcon.Placement = new NativeMethods.Rect { Top = (int)location.Y, Left = (int)location.X, Bottom = (int)(sendingDecorator.ActualHeight * dpiScale), Right = (int)(sendingDecorator.ActualWidth * dpiScale) };
            trayIcon.IconMouseEnter(MouseHelper.GetCursorPositionParam());
        }

        private void Image_MouseLeave(object sender, MouseEventArgs e)
        {
            var trayIcon = (sender as FrameworkElement).DataContext as NotifyIcon;

            trayIcon?.IconMouseLeave(MouseHelper.GetCursorPositionParam());
        }

        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            var trayIcon = (sender as FrameworkElement).DataContext as NotifyIcon;

            trayIcon?.IconMouseMove(MouseHelper.GetCursorPositionParam());
        }
        #endregion

        #region Notify icon balloon notifications
        private void TrayIcon_NotificationBalloonShown(object sender, NotificationBalloonEventArgs e)
        {
            if (Host != null && Host.GetMenuBar() != null && !Host.GetMenuBar().GetIsPrimaryDisplay())
            {
                return;
            }

            showBalloon(e.Balloon);

            if (_notifyIcon.IsPinned || (Host != null && Host.UnpinnedItems.Visibility == Visibility.Visible))
            {
                e.Handled = true;
            }
        }

        private void showBalloon(NotificationBalloon balloon)
        {
            Balloon = balloon;
            playSound(Balloon);

            _balloonTimer?.Stop();

            _balloonTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(GetAdjustedBalloonTimeout(Balloon))
            };

            _balloonTimer.Tick += BalloonTimer_Tick;
            _balloonTimer.Start();
        }

        private void closeBalloon()
        {
            _balloonTimer?.Stop();
            Balloon = null;
        }

        private void playSound(NotificationBalloon balloonInfo)
        {
            if ((balloonInfo.Flags & NativeMethods.NIIF.NOSOUND) != 0)
            {
                return;
            }

            SoundHelper.PlayNotificationSound();
        }

        private void BalloonTimer_Tick(object sender, EventArgs e)
        {
            closeBalloon();
            Balloon?.SetVisibility(BalloonVisibility.TimedOut);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            closeBalloon();
            Balloon?.SetVisibility(BalloonVisibility.Hidden);
            e.Handled = true;
        }

        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Balloon?.Click();
            e.Handled = true;
        }

        internal static int GetAdjustedBalloonTimeout(NotificationBalloon balloon)
        {
            // valid timeout is 12-30 seconds
            int timeout = balloon.Timeout;
            if (timeout < 12000)
            {
                timeout = 12000;
            }
            else if (timeout > 30000)
            {
                timeout = 30000;
            }

            return timeout;
        }
        #endregion
    }
}
