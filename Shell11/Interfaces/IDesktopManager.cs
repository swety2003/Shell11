using ManagedShell.Common.SupportingClasses;

namespace Shell11.Interfaces
{
    public interface IDesktopManager
    {
        ShellWindow ShellWindow { get; }

        bool AllowProgmanChild { get; }

        //DesktopNavigationToolbar DesktopToolbar { get; }

        //bool IsOverlayOpen { get; set; }

        //ShellFolder DesktopLocation { get; }

        //NavigationManager NavigationManager { get; }

        //DesktopOverlay DesktopOverlayWindow { get; }

        //DesktopIcons DesktopIconsControl { get; }

        //Desktop DesktopWindow { get; }

        //void ConfigureDesktop();

        void Initialize(IWindowManager manager);

        void ResetPosition(bool displayChanged);

        //void ToggleOverlay();
    }
}