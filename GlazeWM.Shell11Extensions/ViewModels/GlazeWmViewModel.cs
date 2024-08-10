using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GlazeWm.Messages;
using GlazeWM.Shell11Extensions.Services;
using System.Windows;
using Monitor = GlazeWm.Messages.Monitor;

namespace GlazeWM.Shell11Extensions.ViewModels
{
    public partial class GlazeWmViewModel : ObservableObject
    {
        GlazeWmIpcService IpcClient = new GlazeWmIpcService();
        public GlazeWmViewModel()
        {
            IpcClient.OnRspData += IpcClient_OnRspData;
            IpcClient.OnSubData += IpcClient_OnSubData;
            IpcClient.OnError += IpcClient_OnError;
        }

        private void IpcClient_OnError(object sender, Exception ex)
        {
            Connected = false;
            Workspaces?.Clear();
            IpcClient.Close();
            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageBox.Show(ex.ToString());
            });
#if DEBUG
            throw ex;
#endif
        }

        List<Monitor> monitors = new List<Monitor>();

        Monitor activeMonitor => monitors.FirstOrDefault();
        [ObservableProperty]
        List<ActivatedWorkspace> workspaces = new List<ActivatedWorkspace>();

        [ObservableProperty]
        int activeWorkspaceIndex = 0;

        [ObservableProperty]
        Focused focusedWindow;

        [RelayCommand]
        void OnUnload()
        {
            IpcClient.Close();
            IpcClient.OnSubData -= IpcClient_OnSubData;
            IpcClient.OnRspData -= IpcClient_OnRspData;
            IpcClient.OnError -= IpcClient_OnError;
        }

        private void SetActiveWorkspaceValue(int value)
        {
            activeWorkspaceIndex = value;
            OnPropertyChanged(nameof(ActiveWorkspaceIndex));
        }
        partial void OnActiveWorkspaceIndexChanged(int value)
        {
            IpcClient.FocuseWorkspace(value + 1);
        }

        private void IpcClient_OnSubData(object sender, GlazeWm.Messages.Data data)
        {
            if (data.EventType == "workspace_activated")
            {
                //var index = workspaces.IndexOf(workspaces
                //    .FirstOrDefault(x => x.HasFocus));
                //if (index != -1)
                //{
                //    SetActiveWorkspaceValue(index);
                //}

                IpcClient.QueryMonitors();
            }
            else if (data.EventType == "focus_changed")
            {
                FocusedWindow = data.FocusedContainer;
                IpcClient.QueryFocused();
            }
            else
            {

            }
        }

        private void IpcClient_OnRspData(object sender, GlazeWm.Messages.Data data)
        {
            if (data == null) { return; }
            if (data.Monitors != null)
            {
                monitors = data.Monitors;
                Workspaces = activeMonitor.Children;

                var focusedWorkspace = Workspaces.Where(x => x.HasFocus).FirstOrDefault();
                var index = Workspaces.IndexOf(focusedWorkspace);
                if (index != -1)
                {
                    SetActiveWorkspaceValue(index);
                }
                else
                {
                    SetActiveWorkspaceValue(Workspaces.Count - 1);
                }

            }
            else if (data.Focused != null)
            {
                var focusedWorkspace = Workspaces.Where(x => x.Id == data.Focused.ParentId).FirstOrDefault();
                var index = Workspaces.IndexOf(focusedWorkspace);
                if (index != -1)
                {
                    SetActiveWorkspaceValue(index);
                }
            }
        }

        [ObservableProperty] bool connected = false;

        [RelayCommand]
        void Connect()
        {
            IpcClient.Connect();
            Connected = true;
        }

    }
}