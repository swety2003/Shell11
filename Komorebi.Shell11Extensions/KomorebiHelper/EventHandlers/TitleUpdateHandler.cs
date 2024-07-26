using Komorebi.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestLib.ViewModels;

namespace Komorebi.Shell11Extensions.KomorebiHelper.EventHandlers
{
    [KEventHandler(TypeEnum.TitleUpdate)]
    internal class TitleUpdateHandler : KeventHandlerBase
    {
        public override void Handle(ViewKHViewModel vm, object? item)
        {
            if (item is Content content)
            {
                if (content.AnythingArray != null)
                {
                    PurpleWindow activeInfo = content.AnythingArray[1].PurpleWindow;
                    vm.ActiveWindow = activeInfo;
                }
            }
        }
    }
}
