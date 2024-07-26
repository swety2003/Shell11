using Komorebi.Notifications;
using TestLib.ViewModels;

namespace Komorebi.Shell11Extensions.KomorebiHelper.EventHandlers
{
    [KEventHandler(TypeEnum.WorkspaceLayout)]
    internal class WorkspaceLayoutHandler : KeventHandlerBase
    {

        public override void Handle(ViewKHViewModel vm, object? item)
        {
            if (item is Content content)
            {
                if (content.AnythingArray != null)
                {
                    vm.SetActiveWorkspaceLayoutValue(content.AnythingArray[2].String);
                }
            }
        }
    }
}
