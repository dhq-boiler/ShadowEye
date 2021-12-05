

using ShadowEye.Model;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Windows.Graphics.Capture;
using WinRT;
using WinRT.Interop;
using static ShadowEye.Utils.NativeMethods;

namespace ShadowEye.Utils
{
    internal class ScreenShotWindow : ScreenShotArea
    {
        internal Process SelectedProcess { get; set; }
        internal bool OnlyClientArea { get; set; }

        internal IntPtr SelectedWindowHandle { get; set; }

        internal ScreenShotWindow()
        { }

        internal ScreenShotWindow(Process selectedProcess)
        {
            SelectedProcess = selectedProcess;
        }

        internal static ProcessItem.WindowInfo[] EnumWindows(Process selectedProcess)
        {
            ProcessItem pi = new ProcessItem(selectedProcess);
            NativeMethods.EnumWindows(pi.EnumerateWindows, 0);
            return pi.FoundWindows;
        }

        internal override Bitmap GetScreenShot()
        {
            try
            {
                if (SelectedWindowHandle == IntPtr.Zero)
                    throw new Win32Exception("SelectedWindowHandle != IntPtr.Zero");

                if (OnlyClientArea)
                    return GetWindowClientBitmap(SelectedWindowHandle);
                else
                    return GetWindowBitmap(SelectedWindowHandle);
            }
            catch (Win32Exception)
            {
                throw;
            }
            catch (ArgumentException)
            {
                throw;
            }
        }

        private static Bitmap GetWindowBitmap(IntPtr hWnd)
        {
            IntPtr hdcScreen = IntPtr.Zero;
            IntPtr hdcCompatible = IntPtr.Zero;
            IntPtr hBmp = IntPtr.Zero;
            IntPtr hOldBmp = IntPtr.Zero;

            try
            {
                IntPtr hDC = IntPtr.Zero;
                IntPtr winDC = IntPtr.Zero;
                Graphics g = null;
                try
                {
                    winDC = GetWindowDC(hWnd);
                    RECT winRect = new RECT();
                    GetWindowRect(hWnd, out winRect);

                    //Bitmapの作成
                    Bitmap bmp = new Bitmap(winRect.right - winRect.left,
                        winRect.bottom - winRect.top);
                    //Graphicsの作成
                    g = Graphics.FromImage(bmp);
                    //Graphicsのデバイスコンテキストを取得
                    hDC = g.GetHdc();

                    PrintWindow(hWnd, hDC, 0);
                    //Bitmapに画像をコピーする
                    BitBlt(hDC, 0, 0, bmp.Width, bmp.Height,
                        winDC, 0, 0, SRCCOPY);

                    return bmp;
                }
                finally
                {
                    if (g != null)
                    {
                        //解放
                        g.ReleaseHdc(hDC);
                        g.Dispose();
                    }
                    ReleaseDC(hWnd, winDC);
                }
            }
            finally
            {
                if (hdcScreen != IntPtr.Zero)
                    NativeMethods.ReleaseDC(hWnd, hdcScreen);
            }
        }

        private const int SRCCOPY = 13369376;

        private static Bitmap GetWindowClientBitmap(IntPtr hWnd)
        {
            IntPtr hdcScreen = IntPtr.Zero;
            IntPtr hdcCompatible = IntPtr.Zero;
            IntPtr hBmp = IntPtr.Zero;
            IntPtr hOldBmp = IntPtr.Zero;

            try
            {
                IntPtr hDC = IntPtr.Zero;
                IntPtr winDC = IntPtr.Zero;
                Graphics g = null;
                try
                {
                    winDC = GetDC(hWnd);
                    ShowIfError();
                    GetClientRect(hWnd, out var rect);
                    ShowIfError();
                    //Bitmapの作成
                    Bitmap bmp = new Bitmap(rect.right - rect.left,
                        rect.bottom - rect.top);
                    //Graphicsの作成
                    g = Graphics.FromImage(bmp);
                    //Graphicsのデバイスコンテキストを取得
                    hDC = g.GetHdc();
                    //Bitmapに画像をコピーする
                    BitBlt(hDC, 0, 0, bmp.Width, bmp.Height,
                        winDC, 0, 0, SRCCOPY);

                    return bmp;
                }
                finally
                {
                    if (g != null)
                    {
                        //解放
                        g.ReleaseHdc(hDC);
                        g.Dispose();
                    }
                    ReleaseDC(hWnd, winDC);
                }
            }
            finally
            {
                if (hdcScreen != IntPtr.Zero)
                    NativeMethods.ReleaseDC(hWnd, hdcScreen);
            }
        }

        private static void ShowIfError()
        {
            var errorCode = Marshal.GetLastWin32Error();
            if (errorCode != 0)
            {
                StringBuilder message = new StringBuilder(255);
                FormatMessage(
                  FORMAT_MESSAGE_FROM_SYSTEM,
                  IntPtr.Zero,
                  (uint)errorCode,
                  0,
                  message,
                  message.Capacity,
                  IntPtr.Zero);
                System.Windows.MessageBox.Show(message.ToString());
            }
        }

        internal override bool IsReady
        {
            get { return SelectedProcess != null; }
        }
    }
}
