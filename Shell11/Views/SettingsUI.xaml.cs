using System.Windows;

namespace Shell11.Views
{
    /// <summary>
    /// SettingsUI.xaml 的交互逻辑
    /// </summary>
    public partial class SettingsUI : Window
    {
        internal static SettingsUI? Instance { get; set; }

        public SettingsUI()
        {
            InitializeComponent();
        }

    }
}
