using Komorebi.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestLib.ViewModels;

namespace Komorebi.Shell11Extensions.KomorebiHelper.EventHandlers
{

    [KEventHandler(TypeEnum.Uncloak), KEventHandler(TypeEnum.Cloak)]
    internal class UncloakHandler : KeventHandlerBase
    {
        public override void Handle(ViewKHViewModel vm, object? item)
        {
            if (item is RingForMonitor monitors)
            {
                //vm.Workspaces.Clear();
                var monitor = monitors.Elements[(int)monitors.Focused];
                //foreach (var workspace in monitor.Workspaces.Elements)
                //{

                //    vm.Workspaces.Add(workspace);
                //}
                vm.SetActiveWorkspaceValue((int)monitor.Workspaces.Focused);
            }
        }
    }
}
