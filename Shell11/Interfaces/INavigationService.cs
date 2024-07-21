using System.Windows;

namespace Shell11.Services
{
    public interface INavigationService
    {
        FrameworkElement? GetPage(string key);
    }
}
