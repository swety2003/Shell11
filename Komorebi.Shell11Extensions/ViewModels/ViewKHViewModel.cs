using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Komorebi.Notifications;
using Komorebi.Shell11Extensions.KomorebiHelper;
using Komorebi.Shell11Extensions.Utils;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

namespace TestLib.ViewModels
{
    public partial class ViewKHViewModel : ObservableObject
    {

        public State KState
        {
            get;
            set;
        }

        public Komorebi.Notifications.Monitor ActiveMonitor =>
            KState.Monitors.Elements[(int)KState.Monitors.Focused];
        public Komorebi.Notifications.Workspace ActiveWorkspace =>
            ActiveMonitor.Workspaces.Elements[(int)ActiveMonitor.Workspaces.Focused];

        //[ObservableProperty]
        //ImageSource workspacePreview;

        [ObservableProperty]
        ObservableCollection<Workspace> workspaces = new ObservableCollection<Workspace>();

        [ObservableProperty]
        int activeWorkspaceIndex = -1;

        [ObservableProperty, NotifyPropertyChangedFor(nameof(WinICO))]
        PurpleWindow? activeWindow = null;

        [ObservableProperty] string activeWorkspaceLayout = "";


        public string[] Layouts
        {
            get
            {
                return Enum.GetValues<DefaultLayout>().Select(x => x.ToString()).ToArray();
            }
        }

        [RelayCommand]
        void UpdateWorkspacePreview(object value)
        {
            //CloseWorkspacePreview();
            if (value is Workspace workspace)
            {
                var index = Workspaces.IndexOf(workspace);
                WorkspacePreview = ThumbnailUtil.GetWorkspaceShortcut(ActiveMonitor.Workspaces.Elements[index],ActiveMonitor);
                WorkspacePreview.Show();
            }
        }
        [RelayCommand]
        void CloseWorkspacePreview()
        {
            if (WorkspacePreview!=null)
            {
                try
                {
                    WorkspacePreview.Close();
                }
                catch
                {
                }

                WorkspacePreview = null;
            }
        }

        [RelayCommand]
        void SendToWorkspace(object? value)
        {
            if (value is string name)
            {
                CommandHelper.SendFocusedToWorkSpace(name);
            }
        }

        [RelayCommand]
        void OnUnload()
        {
            CommandHelper.StopProcess();
        }
        public ImageSource? WinICO => WindowIconUtil.GetIcon(activeWindow);

        public System.Windows.Window WorkspacePreview { get; private set; }

        private readonly ILogger<ViewKHViewModel> _logger;

        public ViewKHViewModel()
        {
            Task.Run(Run);
        }


        async Task Run()
        {
            if (CommandHelper.Running())
            {
                CommandHelper.UnSubscribe();
                //CommandHelper.StopProcess();
            }
            else
            {

                CommandHelper.StartProcess();
                _logger.LogDebug("Wait for 2s...");

                await Task.Delay(2000);

            }


            CancellationTokenSource cts = new CancellationTokenSource();
            PipeServer pipeServer = new PipeServer();
            KEventHelper eventHelper = new KEventHelper(pipeServer.Create());
            eventHelper.Init(this);
            CommandHelper.Subscribe(PipeServer.pipeName);
            await eventHelper.Watch(cts.Token)
                .ContinueWith((task) =>
                {
                    if (task.Exception != null)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            _logger.LogError(new EventId(), task.Exception, task.Exception.Message);
                        });
                    }
                });

        }

        partial void OnActiveWorkspaceIndexChanged(int value)
        {
            CommandHelper.ChangeWorkSpace(value);
        }

        partial void OnActiveWorkspaceLayoutChanged(string value)
        {
            CommandHelper.SetWorkspaceLayout(ActiveWorkspaceIndex, value);

            if (ActiveWorkspace.Layout.Default is DefaultLayout layout)
            {
                SetActiveWorkspaceLayoutValue(layout.ToString());
            }
        }

        public void SetActiveWorkspaceValue(int value)
        {
            activeWorkspaceIndex = value;
            OnPropertyChanged(nameof(ActiveWorkspaceIndex));

        }
        public void SetActiveWorkspaceLayoutValue(string value)
        {
            activeWorkspaceLayout = value;
            OnPropertyChanged(nameof(ActiveWorkspaceLayout));
        }
        //public void SetAcytiveWindowValue(PurpleWindow w)
        //{
        //    activeWindow = w;
        //    OnPropertyChanged(nameof(ActiveWindow));
        //}
    }
}