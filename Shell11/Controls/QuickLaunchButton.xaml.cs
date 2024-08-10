using AppGrabber;
using ManagedShell.Common.Helpers;
using Shell11.Common.Application.Contracts;
using Shell11.Common.Configuration;
using Shell11.Views;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Shell11.Controls;

public partial class QuickLaunchButton : IConfigurationChangeAware
{
    public static DependencyProperty ParentTaskbarProperty = DependencyProperty.Register("ParentTaskbar", typeof(TaskBarWindow), typeof(QuickLaunchButton));
    public TaskBarWindow ParentTaskbar
    {
        get { return (TaskBarWindow)GetValue(ParentTaskbarProperty); }
        set { SetValue(ParentTaskbarProperty, value); }
    }

    public QuickLaunchButton()
    {
        InitializeComponent();

        setIconSize();

        // register for settings changes
        handler = Settings.Subscribe(this);

        Unloaded += QuickLaunchButton_Unloaded;
    }

    private void QuickLaunchButton_Unloaded(object sender, RoutedEventArgs e)
    {
        Settings.UnSubscribe(handler);
    }


    private void setIconSize()
    {
        imgIcon.Width = IconHelper.GetSize(Settings.Instance.TaskbarIconSize);
        imgIcon.Height = IconHelper.GetSize(Settings.Instance.TaskbarIconSize);
    }

    private void LaunchProgram(object sender, RoutedEventArgs e)
    {
        Button item = (Button)sender;
        ApplicationInfo app = (ApplicationInfo)item.DataContext;

        ParentTaskbar.AppGrabber.LaunchProgram(app);
    }

    private void programsMenu_Open(object sender, RoutedEventArgs e)
    {
        MenuItem item = (MenuItem)sender;
        ApplicationInfo app = item.DataContext as ApplicationInfo;

        ParentTaskbar.AppGrabber.LaunchProgram(app);
    }

    private void programsMenu_OpenAsAdmin(object sender, RoutedEventArgs e)
    {
        MenuItem item = (MenuItem)sender;
        ApplicationInfo app = item.DataContext as ApplicationInfo;

        ParentTaskbar.AppGrabber.LaunchProgramAdmin(app);
    }

    private void programsMenu_OpenRunAs(object sender, RoutedEventArgs e)
    {
        MenuItem item = (MenuItem)sender;
        ApplicationInfo app = item.DataContext as ApplicationInfo;

        ParentTaskbar.AppGrabber.LaunchProgramVerb(app, "runasuser");
    }

    private void programsMenu_Remove(object sender, RoutedEventArgs e)
    {
        MenuItem item = (MenuItem)sender;
        ApplicationInfo app = item.DataContext as ApplicationInfo;

        ParentTaskbar.AppGrabber.QuickLaunch.Remove(app);
    }

    private void programsMenu_Rename(object sender, RoutedEventArgs e)
    {
        MenuItem item = (MenuItem)sender;
        ApplicationInfo app = item.DataContext as ApplicationInfo;

        //ParentTaskbar._appGrabber.RenameAppDialog(app, null);
    }

    private void programsMenu_Properties(object sender, RoutedEventArgs e)
    {
        MenuItem item = (MenuItem)sender;
        ApplicationInfo app = item.DataContext as ApplicationInfo;

        //ParentTaskbar._appGrabber.ShowAppProperties(app);
    }

    #region Drag and drop reordering

    private Point? startPoint = null;
    private bool inDrag = false;
    private PropertyChangedEventHandler handler;

    // receive drop functions
    private void btn_DragOver(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            e.Effects = DragDropEffects.Link;
        }
        else if (!e.Data.GetDataPresent(typeof(ApplicationInfo)))
        {
            e.Effects = DragDropEffects.None;
        }

        e.Handled = true;
    }

    private void btn_Drop(object sender, DragEventArgs e)
    {
        Button dropContainer = sender as Button;
        ApplicationInfo replacedApp = dropContainer.DataContext as ApplicationInfo;

        string[] fileNames = e.Data.GetData(DataFormats.FileDrop) as string[];
        if (fileNames != null)
        {
            // todo 
            int dropIndex = ParentTaskbar.AppGrabber.QuickLaunch.IndexOf(replacedApp);
            //ParentTaskbar.AppGrabber.QuickLaunchManager.InsertByPath(fileNames, dropIndex);
        }
        else if (e.Data.GetDataPresent(typeof(ApplicationInfo)))
        {
            ApplicationInfo dropData = e.Data.GetData(typeof(ApplicationInfo)) as ApplicationInfo;

            int initialIndex = ParentTaskbar.AppGrabber.QuickLaunch.IndexOf(dropData);
            int dropIndex = ParentTaskbar.AppGrabber.QuickLaunch.IndexOf(replacedApp);
            ParentTaskbar.AppGrabber.QuickLaunch.Move(initialIndex, dropIndex);
            //ParentTaskbar._appGrabber.Save();
        }

        e.Handled = true;
    }

    // send drag functions
    private void btn_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        // Store the mouse position
        startPoint = e.GetPosition(this);
    }

    private void btn_PreviewMouseMove(object sender, MouseEventArgs e)
    {
        if (!inDrag && startPoint != null)
        {
            inDrag = true;

            Point mousePos = e.GetPosition(this);
            Vector diff = (Point)startPoint - mousePos;

            if (mousePos.Y <= this.ActualHeight && ((Point)startPoint).Y <= this.ActualHeight && e.LeftButton == MouseButtonState.Pressed && (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                Button button = sender as Button;
                ApplicationInfo selectedApp = button.DataContext as ApplicationInfo;

                try
                {
                    DragDrop.DoDragDrop(button, selectedApp, DragDropEffects.Move);
                }
                catch { }

                // reset the stored mouse position
                startPoint = null;
            }
            else if (e.LeftButton != MouseButtonState.Pressed)
            {
                // reset the stored mouse position
                startPoint = null;
            }

            inDrag = false;
        }

        e.Handled = true;
    }

    #endregion

    private void ContextMenu_Opened(object sender, RoutedEventArgs e)
    {
        ContextMenu menu = sender as ContextMenu;

        foreach (Control item in menu.Items)
        {
            ApplicationInfo app = item.DataContext as ApplicationInfo;

            switch (item.Name)
            {
                case "miProgramsItemAdmin":
                    item.Visibility = app.AllowRunAsAdmin ? Visibility.Visible : Visibility.Collapsed;
                    break;
                case "miProgramsItemRunAs":
                    //item.Visibility = KeyboardUtilities.IsKeyDown(System.Windows.Forms.Keys.ShiftKey) && !app.IsStoreApp ? Visibility.Visible : Visibility.Collapsed;
                    break;
                default:
                    break;
            }
        }
    }

    public void HandleSettingChange(string setting)
    {

        if (setting != null)
        {
            switch (setting)
            {
                case "TaskbarIconSize":
                    setIconSize();
                    break;
            }
        }
    }
}