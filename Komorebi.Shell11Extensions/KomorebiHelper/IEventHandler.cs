using Komorebi.Notifications;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestLib.ViewModels;

namespace Komorebi.Shell11Extensions.KomorebiHelper
{
    public interface IKEventHandler
    {
        void Handle(ViewKHViewModel vm, object? item);
    }

    public abstract class KeventHandlerBase : IKEventHandler
    {

        public abstract void Handle(ViewKHViewModel vm, object? item);
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class KEventHandlerAttribute : Attribute
    {
        public static Dictionary<TypeEnum, IKEventHandler> EventHandlers = new Dictionary<TypeEnum, IKEventHandler>();
        static KEventHandlerAttribute()
        {
        }

        public static void InitEventHandlers()
        {
            if (EventHandlers.Count>0)
            {
                return;
            }
            var h = Extension.Host.Services.GetServices<IKEventHandler>();
            foreach (var item in h)
            {
                var alist = item.GetType().GetCustomAttributes<KEventHandlerAttribute>();
                if (alist != null)
                {
                    foreach (var a in alist)
                    {
                        EventHandlers.TryAdd(a.EventName, item);
                    }
                }
            }
        }

        public TypeEnum EventName { get; set; }
        public KEventHandlerAttribute(TypeEnum eventName)
        {

            EventName = eventName;

        }
    }
}
