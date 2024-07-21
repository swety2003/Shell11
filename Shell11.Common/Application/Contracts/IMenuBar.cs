using ManagedShell.WindowsTray;
using Shell11.Common.Application.Structs;

namespace Shell11.Common.Application.Contracts
{
    public interface IMenuBar
    {
        NotificationArea notificationArea { get; }
        IntPtr GetHandle();

        bool GetIsPrimaryDisplay();

        MenuBarDimensions GetDimensions();

        void PeekDuringAutoHide(int msToPeek = 1000);
    }
}