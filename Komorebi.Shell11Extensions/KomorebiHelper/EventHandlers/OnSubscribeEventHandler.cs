using Komorebi.Notifications;
using TestLib.ViewModels;

namespace Komorebi.Shell11Extensions.KomorebiHelper.EventHandlers
{
    [KEventHandler(TypeEnum.AddSubscriberPipe)]
    internal class OnSubscribeEventHandler : KeventHandlerBase
    {

        public override void Handle(ViewKHViewModel vm, object? item)
        {
            vm.Workspaces.Clear();
            if (item is RingForMonitor monitors)
            {
                var monitor = monitors.Elements[(int)monitors.Focused];
                foreach (var workspace in monitor.Workspaces.Elements)
                {

                    vm.Workspaces.Add(workspace);
                }
                vm.SetActiveWorkspaceValue((int)monitor.Workspaces.Focused);

                if (vm.ActiveWorkspace.Layout.Default != null)
                {
                    vm.SetActiveWorkspaceLayoutValue(vm.ActiveWorkspace.Layout.Default.Value.ToString());
                }
                else
                {
                    vm.SetActiveWorkspaceLayoutValue("UnKnownLayout");
                }
            }
        }
    }
}
