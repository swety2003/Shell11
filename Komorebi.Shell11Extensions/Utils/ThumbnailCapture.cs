using Komorebi.Notifications;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using Windows.Win32;

namespace Komorebi.Shell11Extensions.Utils
{

    public class ThumbnailCapture
    {
        [DllImport("dwmapi.dll")]
        private static extern int DwmRegisterThumbnail(nint dest, nint src, out nint thumbnail);

        [DllImport("dwmapi.dll")]
        private static extern int DwmUnregisterThumbnail(nint thumbnail);

        [DllImport("dwmapi.dll")]
        private static extern int DwmUpdateThumbnailProperties(nint thumbnail, ref DWM_THUMBNAIL_PROPERTIES props);

        [DllImport("dwmapi.dll")]
        private static extern int DwmCopyThumbnail(nint thumbnail, out nint hBitmap);
        [DllImport("Dwmapi.dll")]
        public static extern int DwmQueryThumbnailSourceSize(nint hThumbnailId, out PSIZE pSize);

        [StructLayout(LayoutKind.Sequential)]
        public struct PSIZE
        {
            /// <summary>
            /// 缩略图的宽度
            /// </summary>
            public int x;

            /// <summary>
            /// 缩略图的高度
            /// </summary>
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DWM_THUMBNAIL_PROPERTIES
        {
            public int dwFlags;
            public RECT rcDestination;
            public RECT rcSource;
            public byte opacity;
            public bool fVisible;
            public bool fSourceClientAreaOnly;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        private const int DWM_TNP_RECTDESTINATION = 0x00000001;
        private const int DWM_TNP_VISIBLE = 0x00000008;


        public static void UnDwmRegisterThumbnail(nint thumbnail)
        {
            // 注销缩略图
            DwmUnregisterThumbnail(thumbnail);
        }

        public static nint RegisterThumbnail(nint src, nint hwndDest, RECT rect)
        {
            nint thumbnail;

            // 注册缩略图
            int hr = DwmRegisterThumbnail(hwndDest, src, out thumbnail);
            if (hr != 0)
            {
                //throw new Exception("DwmRegisterThumbnail failed with error code " + hr);
            }


            DwmQueryThumbnailSourceSize(thumbnail, out var size); //获取缩略图原大小

            //RECT rectNew = new RECT(); //保持原大小
            rect.Right = size.x + rect.Left;
            rect.Bottom = size.y + rect.Top;

            // 设置缩略图属性
            DWM_THUMBNAIL_PROPERTIES props = new DWM_THUMBNAIL_PROPERTIES
            {
                dwFlags = DWM_TNP_RECTDESTINATION | DWM_TNP_VISIBLE,
                rcDestination = rect, // 目标区域
                fVisible = true,
                opacity = 255
            };

            hr = DwmUpdateThumbnailProperties(thumbnail, ref props);
            if (hr != 0)
            {
                //throw new Exception("DwmUpdateThumbnailProperties failed with error code " + hr);
            }

            return thumbnail;

        }
    }


    public class ThumbnailUtil
    {
        private unsafe static void SetWindowToForegroundWithAttachThreadInput(System.Windows.Window window)
        {
            var interopHelper = new WindowInteropHelper(window);
            // 以下 Win32 方法可以在 https://github.com/kkwpsv/lsjutil/tree/master/Src/Lsj.Util.Win32 找到
            var thisWindowThreadId = PInvoke.GetWindowThreadProcessId(new Windows.Win32.Foundation.HWND(interopHelper.Handle), (uint*)0);
            var currentForegroundWindow = PInvoke.GetForegroundWindow();
            var currentForegroundWindowThreadId = PInvoke.GetWindowThreadProcessId(currentForegroundWindow, (uint*)0);

            // [c# - Bring a window to the front in WPF - Stack Overflow](https://stackoverflow.com/questions/257587/bring-a-window-to-the-front-in-wpf )
            // [SetForegroundWindow的正确用法 - 子坞 - 博客园](https://www.cnblogs.com/ziwuge/archive/2012/01/06/2315342.html )
            /*
               　　1.得到窗口句柄FindWindow 
            　　　　2.切换键盘输入焦点AttachThreadInput 
            　　　　3.显示窗口ShowWindow(有些窗口被最小化/隐藏了) 
            　　　　4.更改窗口的Zorder，SetWindowPos使之最上，为了不影响后续窗口的Zorder,改完之后，再还原 
            　　　　5.最后SetForegroundWindow 
             */
            PInvoke.AttachThreadInput(currentForegroundWindowThreadId, thisWindowThreadId, true);

            window.Show();
            window.Activate();
            // 去掉和其他线程的输入链接
            PInvoke.AttachThreadInput(currentForegroundWindowThreadId, thisWindowThreadId, false);

            // 用于踢掉其他的在上层的窗口
            window.Topmost = true;
            window.Topmost = false;
        }

        public static System.Windows.Window GetWorkspaceShortcut(Workspace workspace, Notifications.Monitor monitor)
        {
            var win = new System.Windows.Window() { WindowStyle = WindowStyle.None };
            win.ResizeMode = ResizeMode.NoResize;
            win.AllowsTransparency = true;
            //win.ShowInTaskbar = false;
            //win.WindowState = System.Windows.WindowState.Maximized;
            win.Width = monitor.WorkAreaSize.Right - monitor.WorkAreaSize.Left;
            win.Height = monitor.WorkAreaSize.Bottom - monitor.WorkAreaSize.Top;
            win.Left = monitor.WorkAreaSize.Left;
            win.Top = monitor.WorkAreaSize.Top;
            win.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(200, 255, 255, 255));
            //win.Deactivated += (s, e) =>
            //{
            //    win.Close();
            //};
            //win.Width = workspace.;
            var hwnds = workspace.Containers.Elements.Select(x => x.Windows.Elements.FirstOrDefault())
                .Where(x => x != null)
                .Select(x => new IntPtr(x.Hwnd)).ToArray();
            int index = 0;
            List<nint> thumbnails = new List<nint>();
            win.Loaded += (s, e) =>
            {
                var targetH = new WindowInteropHelper(win).Handle;
                foreach (var item in hwnds)
                {
                    var layout = workspace.LatestLayout[index];
                    var t = ThumbnailCapture.RegisterThumbnail(item, targetH,
                        new ThumbnailCapture.RECT
                        {
                            Left = (int)layout.Left,
                            Top = (int)(layout.Top - monitor.WorkAreaSize.Top),
                            Right = (int)layout.Right,
                            Bottom = (int)(layout.Bottom + monitor.WorkAreaSize.Top)
                        }); ;
                    thumbnails.Add(t);
                    index++;
                }
                SetWindowToForegroundWithAttachThreadInput(win);
            };
            win.Closing += (s, e) =>
            {
                foreach (var t in thumbnails)
                {
                    ThumbnailCapture.UnDwmRegisterThumbnail(t);
                }
            };
            //foreach (var layout in workspace.LatestLayout)
            //{
            //    var img = winCaps[index];
            //    graphics.DrawImage(img, layout.Left, layout.Top, img.Width, img.Height);
            //    index++;
            //    img.Dispose();
            //}
            return win;
        }

    }
}

