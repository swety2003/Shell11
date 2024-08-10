using iNKORE.UI.WPF.Modern.Controls;
using System.Windows;

namespace Shell11.Common.Dialogs
{
    /// <summary>
    /// ContentDialogW.xaml 的交互逻辑
    /// </summary>
    public partial class ContentDialogW : Window
    {
        private readonly ContentDialog dialog;

        public ContentDialogW()
        {
            InitializeComponent();
        }

        public ContentDialogW(ContentDialog dialog)
        {
            InitializeComponent();
            this.dialog = dialog;
            Title = dialog.Title.ToString();
        }

        public async Task<ContentDialogResult> ShowAsync()
        {
            Show();
            var ret = await dialog.ShowAsync(this);
            Close();
            return ret;
        }
    }
}
