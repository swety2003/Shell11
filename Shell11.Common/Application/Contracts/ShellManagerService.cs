using ManagedShell.Common.Enums;
using ManagedShell;
using System.ComponentModel;
using Shell11.Common.Configuration;

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
                PinnedNotifyIcons = Settings.Instance.PinnedNotifyIcons
            };

            return new ShellManager(config);
        }

        public void Dispose()
        {
            ShellManager.Dispose();
        }
    }
}