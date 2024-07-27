using ManagedShell.Common.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Composition;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Composition.Hosting;
using System.Runtime.Loader;
using Shell11.Common.Application.Contracts;

namespace Shell11.Common.DependencyInjection
{

    public static class DependencyLoader
    {

        private static Assembly LoadPlugin(string pluginLocation)
        {
            if (!File.Exists(pluginLocation))
            {
                pluginLocation = Path.Combine(Path.GetDirectoryName(pluginLocation), "Debug",
                    Path.GetFileName(pluginLocation));
            }
            //_logger.LogDebug($"加载: {Path.GetFileName(pluginLocation)}");
            var loadContext = new PluginLoadContext(pluginLocation);
            return loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(pluginLocation)));
        }

        public static IServiceCollection LoadDependencies(this IServiceCollection serviceCollection, string path, string pattern = "*.dll")
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return serviceCollection;
            }

            if (!Directory.Exists(path))
            {
                return serviceCollection;
            }

            var dirs = Directory.GetDirectories(path);


            foreach (var item in dirs)
            {
                var name = Path.GetFileName(item);
                var dll = Directory.GetFiles(item, pattern)
                    .Where(x => Path.GetFileNameWithoutExtension(x)==name).FirstOrDefault();
                if (dll != null)
                {
                    var assembly = LoadPlugin(dll);
                    var configuration = new ContainerConfiguration()
                        .WithAssembly(assembly);

                    using (var container = configuration.CreateContainer())
                    {
                        var plugin = container.GetExport<IExtension>();

                        plugin.ConfigureServices(serviceCollection);

                        serviceCollection.AddSingleton(typeof(IExtension), plugin);

                        var mbes = container.GetExports<IMenuBarExtension>();


                        foreach (var mbe in mbes)
                        {
                            serviceCollection.AddSingleton(typeof(IMenuBarExtension), mbe);
                            mbe.RegisterServices(serviceCollection);

                        }
                    }
                }
            }

            return serviceCollection;
        }


        internal class PluginLoadContext : AssemblyLoadContext
        {
            private readonly AssemblyDependencyResolver _resolver;

            public PluginLoadContext(string pluginPath)
            {
                if (!File.Exists(pluginPath)) throw new FileNotFoundException(pluginPath);
                _resolver = new AssemblyDependencyResolver(pluginPath);
            }

            protected override Assembly? Load(AssemblyName assemblyName)
            {
                var assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
                if (assemblyPath != null) return LoadFromAssemblyPath(assemblyPath);

                return null;
            }

            protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
            {
                var libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
                if (libraryPath != null) return LoadUnmanagedDllFromPath(libraryPath);

                return IntPtr.Zero;
            }
        }

    }
}
