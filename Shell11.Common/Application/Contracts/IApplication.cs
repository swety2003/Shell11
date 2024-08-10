using System.Windows;

namespace Shell11.Common.Application.Contracts
{
    public interface IApplication
    {

        IEnumerable<IMenuBarExtension> MenuBarExtensions { get; }

        int Run();

        void InvokeInUIThread(Action action);

        T? GetService<T>();

        void ExitApp();
        void RestartApp();

        Window MainWindow { get; set; }

    }
}
