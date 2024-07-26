using Komorebi.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestLib.ViewModels;

namespace Komorebi.Shell11Extensions.KomorebiHelper.EventHandlers
{
    [KEventHandler(TypeEnum.FocusWorkspaceNumber)]
    internal class FocusNamedWorkspaceHandler : KeventHandlerBase
    {
        public override void Handle(ViewKHViewModel vm, object? item)
        {
            if (item is Content content)
            {
                if (content.Integer != null)
                {
                    vm.SetActiveWorkspaceValue((int)content.Integer.Value);

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
}

