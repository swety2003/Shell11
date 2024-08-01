using Komorebi.Shell11Extensions.Extensions;
using ManagedShell.Common.Enums;
using ManagedShell.Common.Helpers;
using ManagedShell.Interop;
using ManagedShell.UWPInterop;
using ManagedShell.WindowsTasks;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Komorebi.Shell11Extensions.Utils
{
    internal static class FileIconUtil
    {
        internal static ImageSource GetImgByFile(string file)
        {
            ImageSource? ico;
            var guid = new Guid("46EB5926-582E-4017-9FDF-E8998DAA0950");
            extracticon.IImageList? iImageList = null;
            extracticon.SHGetImageList(4, ref guid, ref iImageList);
            var icon = extracticon.Icon(iImageList, extracticon.IconIndex(file, true));

            var ico_bmp = icon.ToBitmap();
            CheckSize(ref ico_bmp);
            // ico = Imaging.CreateBitmapSourceFromHBitmap(ico_bmp.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            ico = ico_bmp.ToImageSource();
            return ico;
        }

        internal static ImageSource? GetImgByHandle(IntPtr Handle)
        {
            ImageSource? Icon = null;
            IntPtr retval = default;
            uint messageId = 127u;
            uint messageId2 = 55u;
            int longClass = -14;
            int longClass2 = -34;
            IconSize taskIconSize = IconSize.Medium;
            if (taskIconSize == IconSize.Small)
            {
                NativeMethods.SendMessageTimeout(Handle, messageId, 2u, 0u, 2u, 1000u, ref retval);
                if (retval == IntPtr.Zero)
                {
                    NativeMethods.SendMessageTimeout(Handle, messageId, 0u, 0u, 2u, 1000u, ref retval);
                }
            }
            else
            {
                NativeMethods.SendMessageTimeout(Handle, messageId, 1u, 0u, 2u, 1000u, ref retval);
            }

            if (retval == IntPtr.Zero && taskIconSize == IconSize.Small)
            {
                retval = (Environment.Is64BitProcess ? NativeMethods.GetClassLongPtr(Handle, longClass2) : NativeMethods.GetClassLong(Handle, longClass2));
            }

            if (retval == IntPtr.Zero)
            {
                retval = (Environment.Is64BitProcess ? NativeMethods.GetClassLongPtr(Handle, longClass) : NativeMethods.GetClassLong(Handle, longClass));
            }

            if (retval == IntPtr.Zero)
            {
                NativeMethods.SendMessageTimeout(Handle, messageId2, 0u, 0u, 0u, 1000u, ref retval);
            }


            if (retval != IntPtr.Zero)
            {
                if(true)
                {
                    //_hIcon = retval;
                    //bool returnDefault = _icon == null;
                    ImageSource imageFromHIcon = IconImageConverter.GetImageFromHIcon(retval, false);
                    if (imageFromHIcon != null)
                    {
                        imageFromHIcon.Freeze();
                        Icon = imageFromHIcon;
                    }
                }
                else
                {
                    NativeMethods.DestroyIcon(retval);
                }
            }

            return Icon;


        }

        #region Decompiled from extracticon.exe by dnspy.

        private class extracticon
        {
            // Token: 0x02000004 RID: 4
            [Flags]
            public enum ImageListDrawItemConstants
            {
                // Token: 0x04000004 RID: 4
                ILD_NORMAL = 0,

                // Token: 0x04000005 RID: 5
                ILD_TRANSPARENT = 1,

                // Token: 0x04000006 RID: 6
                ILD_BLEND25 = 2,

                // Token: 0x04000007 RID: 7
                ILD_SELECTED = 4,

                // Token: 0x04000008 RID: 8
                ILD_MASK = 16,

                // Token: 0x04000009 RID: 9
                ILD_IMAGE = 32,

                // Token: 0x0400000A RID: 10
                ILD_ROP = 64,

                // Token: 0x0400000B RID: 11
                ILD_PRESERVEALPHA = 4096,

                // Token: 0x0400000C RID: 12
                ILD_SCALE = 8192,

                // Token: 0x0400000D RID: 13
                ILD_DPISCALE = 16384
            }

            // Token: 0x04000001 RID: 1
            private const int JUMBO_SIZE = 4;

            // Token: 0x04000002 RID: 2
            private const int MAX_PATH = 260;

            // Token: 0x06000002 RID: 2 RVA: 0x00002334 File Offset: 0x00000534
            internal static bool CheckPixelRangeConsistency(ref Bitmap bmp, int startX, int startY, int endX, int endY)
            {
                var pixel = bmp.GetPixel(endX, endY);
                for (var i = 0; i <= endX; i++)
                    for (var j = 0; j <= endY; j++)
                        if ((i >= startX || j >= endY) && bmp.GetPixel(i, j) != pixel)
                            return false;
                return true;
            }

            // Token: 0x06000003 RID: 3 RVA: 0x00002384 File Offset: 0x00000584
            internal static void DrawImage(IImageList iImageList, nint hdc, int index, int x, int y,
                ImageListDrawItemConstants flags)
            {
                var imagelistdrawparams = default(IMAGELISTDRAWPARAMS);
                imagelistdrawparams.hdcDst = hdc;
                imagelistdrawparams.cbSize = Marshal.SizeOf(imagelistdrawparams.GetType());
                imagelistdrawparams.i = index;
                imagelistdrawparams.x = x;
                imagelistdrawparams.y = y;
                imagelistdrawparams.rgbFg = -1;
                imagelistdrawparams.fStyle = (int)flags;
                iImageList.Draw(ref imagelistdrawparams);
            }

            internal static Icon? Icon(IImageList iImageList, int index)
            {
                Icon icon = null;

                var hIcon = IntPtr.Zero;
                if (iImageList == null)
                    //hIcon = ImageList_GetIcon(
                    //    hIml,
                    //    index,
                    //    (int)ImageListDrawItemConstants.ILD_TRANSPARENT);
                    return null;
                iImageList.GetIcon(
                    index,
                    (int)ImageListDrawItemConstants.ILD_TRANSPARENT,
                    ref hIcon);

                if (hIcon != IntPtr.Zero) icon = System.Drawing.Icon.FromHandle(hIcon);
                return icon;
            }

            // Token: 0x06000004 RID: 4 RVA: 0x000023EC File Offset: 0x000005EC
            internal static int IconIndex(string fileName, bool forceLoadFromDisk)
            {
                var uFlags = SHGetFileInfoConstants.SHGFI_SYSICONINDEX;
                var dwFileAttributes = 0;
                var shfileinfo = default(SHFILEINFO);
                var cbFileInfo = (uint)Marshal.SizeOf(shfileinfo.GetType());
                if (SHGetFileInfo(fileName, dwFileAttributes, ref shfileinfo, cbFileInfo, (uint)uFlags)
                    .Equals(IntPtr.Zero)) return 0;
                return shfileinfo.iIcon;
            }

            // Token: 0x06000005 RID: 5
            [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
            public static extern int GetShortPathName([MarshalAs(UnmanagedType.LPTStr)] string path,
                [MarshalAs(UnmanagedType.LPTStr)] StringBuilder shortPath, int shortPathLength);

            // Token: 0x06000006 RID: 6
            [DllImport("gdi32.dll")]
            internal static extern nint CreateDC(string strDriver, string strDevice, string strOutput, nint pData);

            // Token: 0x06000007 RID: 7
            [DllImport("gdi32.dll")]
            internal static extern nint DeleteDC(nint hDc);

            // Token: 0x06000008 RID: 8
            [DllImport("gdi32.dll")]
            internal static extern nint DeleteObject(nint hDc);

            // Token: 0x06000009 RID: 9
            [DllImport("gdi32.dll")]
            internal static extern nint CreateCompatibleDC(nint hdc);

            // Token: 0x0600000A RID: 10
            [DllImport("comctl32")]
            private static extern int ImageList_Draw(nint hIml, int i, nint hdcDst, int x, int y, int fStyle);

            // Token: 0x0600000B RID: 11
            [DllImport("comctl32")]
            private static extern nint ImageList_GetIcon(nint himl, int i, int flags);

            // Token: 0x0600000C RID: 12
            [DllImport("gdi32.dll")]
            internal static extern nint SelectObject(nint hdc, nint bmp);

            // Token: 0x0600000D RID: 13
            [DllImport("shell32")]
            private static extern nint SHGetFileInfo(string pszPath, int dwFileAttributes, ref SHFILEINFO psfi,
                uint cbFileInfo, uint uFlags);

            // Token: 0x0600000E RID: 14
            [DllImport("shell32.dll", EntryPoint = "#727")]
            internal static extern int SHGetImageList(int iImageList, ref Guid riid, ref IImageList ppv);

            // Token: 0x0600000F RID: 15
            [DllImport("shell32.dll", EntryPoint = "#727")]
            internal static extern int SHGetImageListHandle(int iImageList, ref Guid riid, ref nint handle);

            // Token: 0x02000003 RID: 3
            [Guid("46EB5926-582E-4017-9FDF-E8998DAA0950")]
            [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
            [ComImport]
            internal interface IImageList
            {
                // Token: 0x06000011 RID: 17
                [PreserveSig]
                int Add(nint hbmImage, nint hbmMask, ref int pi);

                // Token: 0x06000012 RID: 18
                [PreserveSig]
                int ReplaceIcon(int i, nint hicon, ref int pi);

                // Token: 0x06000013 RID: 19
                [PreserveSig]
                int SetOverlayImage(int iImage, int iOverlay);

                // Token: 0x06000014 RID: 20
                [PreserveSig]
                int Replace(int i, nint hbmImage, nint hbmMask);

                // Token: 0x06000015 RID: 21
                [PreserveSig]
                int AddMasked(nint hbmImage, int crMask, ref int pi);

                // Token: 0x06000016 RID: 22
                [PreserveSig]
                int Draw(ref IMAGELISTDRAWPARAMS pimldp);

                // Token: 0x06000017 RID: 23
                [PreserveSig]
                int Remove(int i);

                // Token: 0x06000018 RID: 24
                [PreserveSig]
                int GetIcon(int i, int flags, ref nint picon);

                // Token: 0x06000019 RID: 25
                [PreserveSig]
                int GetImageInfo(int i, ref IMAGEINFO pImageInfo);

                // Token: 0x0600001A RID: 26
                [PreserveSig]
                int Copy(int iDst, IImageList punkSrc, int iSrc, int uFlags);

                // Token: 0x0600001B RID: 27
                [PreserveSig]
                int Merge(int i1, IImageList punk2, int i2, int dx, int dy, ref Guid riid, ref nint ppv);

                // Token: 0x0600001C RID: 28
                [PreserveSig]
                int Clone(ref Guid riid, ref nint ppv);

                // Token: 0x0600001D RID: 29
                [PreserveSig]
                int GetImageRect(int i, ref RECT prc);

                // Token: 0x0600001E RID: 30
                [PreserveSig]
                int GetIconSize(ref int cx, ref int cy);

                // Token: 0x0600001F RID: 31
                [PreserveSig]
                int SetIconSize(int cx, int cy);

                // Token: 0x06000020 RID: 32
                [PreserveSig]
                int GetImageCount(ref int pi);

                // Token: 0x06000021 RID: 33
                [PreserveSig]
                int SetImageCount(int uNewCount);

                // Token: 0x06000022 RID: 34
                [PreserveSig]
                int SetBkColor(int clrBk, ref int pclr);

                // Token: 0x06000023 RID: 35
                [PreserveSig]
                int GetBkColor(ref int pclr);

                // Token: 0x06000024 RID: 36
                [PreserveSig]
                int BeginDrag(int iTrack, int dxHotspot, int dyHotspot);

                // Token: 0x06000025 RID: 37
                [PreserveSig]
                int EndDrag();

                // Token: 0x06000026 RID: 38
                [PreserveSig]
                int DragEnter(nint hwndLock, int x, int y);

                // Token: 0x06000027 RID: 39
                [PreserveSig]
                int DragLeave(nint hwndLock);

                // Token: 0x06000028 RID: 40
                [PreserveSig]
                int DragMove(int x, int y);

                // Token: 0x06000029 RID: 41
                [PreserveSig]
                int SetDragCursorImage(ref IImageList punk, int iDrag, int dxHotspot, int dyHotspot);

                // Token: 0x0600002A RID: 42
                [PreserveSig]
                int DragShowNolock(int fShow);

                // Token: 0x0600002B RID: 43
                [PreserveSig]
                int GetDragImage(ref POINT ppt, ref POINT pptHotspot, ref Guid riid, ref nint ppv);

                // Token: 0x0600002C RID: 44
                [PreserveSig]
                int GetItemFlags(int i, ref int dwFlags);

                // Token: 0x0600002D RID: 45
                [PreserveSig]
                int GetOverlayImage(int iOverlay, ref int piIndex);
            }

            // Token: 0x02000005 RID: 5
            [Flags]
            private enum SHGetFileInfoConstants
            {
                // Token: 0x0400000F RID: 15
                SHGFI_ICON = 256,

                // Token: 0x04000010 RID: 16
                SHGFI_DISPLAYNAME = 512,

                // Token: 0x04000011 RID: 17
                SHGFI_TYPENAME = 1024,

                // Token: 0x04000012 RID: 18
                SHGFI_ATTRIBUTES = 2048,

                // Token: 0x04000013 RID: 19
                SHGFI_ICONLOCATION = 4096,

                // Token: 0x04000014 RID: 20
                SHGFI_EXETYPE = 8192,

                // Token: 0x04000015 RID: 21
                SHGFI_SYSICONINDEX = 16384,

                // Token: 0x04000016 RID: 22
                SHGFI_LINKOVERLAY = 32768,

                // Token: 0x04000017 RID: 23
                SHGFI_SELECTED = 65536,

                // Token: 0x04000018 RID: 24
                SHGFI_ATTR_SPECIFIED = 131072,

                // Token: 0x04000019 RID: 25
                SHGFI_LARGEICON = 0,

                // Token: 0x0400001A RID: 26
                SHGFI_SMALLICON = 1,

                // Token: 0x0400001B RID: 27
                SHGFI_OPENICON = 2,

                // Token: 0x0400001C RID: 28
                SHGFI_SHELLICONSIZE = 4,

                // Token: 0x0400001D RID: 29
                SHGFI_USEFILEATTRIBUTES = 16,

                // Token: 0x0400001E RID: 30
                SHGFI_ADDOVERLAYS = 32,

                // Token: 0x0400001F RID: 31
                SHGFI_OVERLAYINDEX = 64
            }

            // Token: 0x02000006 RID: 6
            internal struct IMAGEINFO
            {
                // Token: 0x04000020 RID: 32
                public nint hbmImage;

                // Token: 0x04000021 RID: 33
                public nint hbmMask;

                // Token: 0x04000022 RID: 34
                public int Unused1;

                // Token: 0x04000023 RID: 35
                public int Unused2;

                // Token: 0x04000024 RID: 36
                public RECT rcImage;
            }

            // Token: 0x02000007 RID: 7
            internal struct IMAGELISTDRAWPARAMS
            {
                // Token: 0x04000025 RID: 37
                public int cbSize;

                // Token: 0x04000026 RID: 38
                public nint himl;

                // Token: 0x04000027 RID: 39
                public int i;

                // Token: 0x04000028 RID: 40
                public nint hdcDst;

                // Token: 0x04000029 RID: 41
                public int x;

                // Token: 0x0400002A RID: 42
                public int y;

                // Token: 0x0400002B RID: 43
                public int cx;

                // Token: 0x0400002C RID: 44
                public int cy;

                // Token: 0x0400002D RID: 45
                public int xBitmap;

                // Token: 0x0400002E RID: 46
                public int yBitmap;

                // Token: 0x0400002F RID: 47
                public int rgbBk;

                // Token: 0x04000030 RID: 48
                public int rgbFg;

                // Token: 0x04000031 RID: 49
                public int fStyle;

                // Token: 0x04000032 RID: 50
                public int dwRop;

                // Token: 0x04000033 RID: 51
                public int fState;

                // Token: 0x04000034 RID: 52
                public int Frame;

                // Token: 0x04000035 RID: 53
                public int crEffect;
            }

            // Token: 0x02000008 RID: 8
            internal struct POINT
            {
                // Token: 0x04000036 RID: 54
                private int x;

                // Token: 0x04000037 RID: 55
                private int y;
            }

            // Token: 0x02000009 RID: 9
            internal struct RECT
            {
                // Token: 0x04000038 RID: 56
                private int left;

                // Token: 0x04000039 RID: 57
                private int top;

                // Token: 0x0400003A RID: 58
                private int right;

                // Token: 0x0400003B RID: 59
                private int bottom;
            }

            // Token: 0x0200000A RID: 10
            private struct SHFILEINFO
            {
                // Token: 0x0400003C RID: 60
                public nint hIcon;

                // Token: 0x0400003D RID: 61
                public int iIcon;

                // Token: 0x0400003E RID: 62
                public int dwAttributes;

                // Token: 0x0400003F RID: 63
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
                public string szDisplayName;

                // Token: 0x04000040 RID: 64
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
                public string szTypeName;
            }
        }

        private static void CheckSize(ref Bitmap bitmap)
        {
            var num2 = 256;

            if (extracticon.CheckPixelRangeConsistency(ref bitmap, 128, 128, 255, 255))
            {
                num2 = 128;
                if (extracticon.CheckPixelRangeConsistency(ref bitmap, 64, 64, 127, 127))
                {
                    num2 = 64;
                    if (extracticon.CheckPixelRangeConsistency(ref bitmap, 48, 48, 63, 63))
                    {
                        num2 = 48;
                        if (extracticon.CheckPixelRangeConsistency(ref bitmap, 32, 32, 47, 47))
                        {
                            num2 = 32;
                            if (extracticon.CheckPixelRangeConsistency(ref bitmap, 16, 16, 31, 31)) num2 = 16;
                        }
                    }
                }
            }

            if (num2 != 256) bitmap = bitmap.Clone(new Rectangle(0, 0, num2, num2), bitmap.PixelFormat);
        }

        #endregion
    }
}
