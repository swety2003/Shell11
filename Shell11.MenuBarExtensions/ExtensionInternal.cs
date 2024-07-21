
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shell11.Common.Application.Contracts;
using Shell11.Common.DependencyInjection;
using Shell11.MenuBarExtensions.ViewModels;
using System.Composition;

namespace Shell11.MenuBarExtensions
{
    [Export(typeof(IExtension))]
    public class ExtensionInternal : IExtension
    {


    }

}
