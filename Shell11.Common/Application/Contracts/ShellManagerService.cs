using ManagedShell.Common.Enums;
using ManagedShell;
using System.ComponentModel;

namespace Shell11.Common.Application.Contracts
{

    public class ShellManagerService : IDisposable
    {
        public ShellManager ShellManager { get; }

        public ShellManagerService()
        {
            ShellManager = ConfigureShellManager();

        }

        private ShellManager ConfigureShellManager()
        {
            ShellConfig config = new ShellConfig()
            {
                EnableTasksService = true,
                EnableTrayService = true,
                AutoStartTasksService = false,
                AutoStartTrayService = false,
            };

            return new ShellManager(config);
        }

        public void Dispose()
        {
            ShellManager.Dispose();
        }
    }
}