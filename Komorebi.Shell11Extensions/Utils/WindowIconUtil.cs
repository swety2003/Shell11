using Komorebi.Notifications;
using Komorebi.Shell11Extensions.Extensions;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Media;

namespace Komorebi.Shell11Extensions.Utils
{
    internal class WindowIconUtil
    {

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(nint hWnd, out int lpdwProcessId);



        public static ImageSource? GetIcon(PurpleWindow w)
        {
            if (w != null && w.Hwnd != null)
            {
                var h = new Windows.Win32.Foundation.HWND((nint)w.Hwnd);
                return GetIcon(h);
            }
            return null;
        }
        public static ImageSource? GetIcon(Windows.Win32.Foundation.HWND hwnd)
        {

            try
            {
                uint hicon = Windows.Win32.PInvoke.GetClassLong(hwnd, Windows.Win32.UI.WindowsAndMessaging.GET_CLASS_LONG_INDEX.GCL_HICON);
                if (hicon > 0)
                {
                    var bmp = Bitmap.FromHicon(new IntPtr(hicon));
                    return bmp.ToImageSource();
                }
                else
                {

                    GetWindowThreadProcessId(hwnd, out var processId);
                    var process = Process.GetProcessById(processId);
                    // 检查进程是否存在并获取其主模块（通常是可执行文件）的路径
                    if (!process.HasExited)
                    {
                        var file = process.MainModule.FileName;
                        if (File.Exists(file)) return FileIconUtil.GetImgByFile(file);
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

            }
            return null;
        }
    }
}
