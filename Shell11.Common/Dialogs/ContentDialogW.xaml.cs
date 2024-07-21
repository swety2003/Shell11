using iNKORE.UI.WPF.Modern.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Shell;
using static ManagedShell.Interop.NativeMethods;

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
