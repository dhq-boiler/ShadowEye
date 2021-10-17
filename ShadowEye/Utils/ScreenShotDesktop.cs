

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShadowEye.Utils
{
    internal class ScreenShotDesktop : ScreenShotArea
    {
        internal ScreenShotDesktop()
        { }

        internal override Bitmap GetScreenShot()
        {
            try
            {
                return GetDesktopBitmap();
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

        private static Bitmap GetDesktopBitmap()
        {
            int screenX;
            int screenY;
            IntPtr hWnd = IntPtr.Zero;
            IntPtr hdcScreen = IntPtr.Zero;
            IntPtr hdcCompatible = IntPtr.Zero;
            IntPtr hBmp = IntPtr.Zero;
            IntPtr hOldBmp = IntPtr.Zero;

            hWnd = NativeMethods.GetDesktopWindow();

            try
            {
                hdcScreen = NativeMethods.GetDC(hWnd);
                if (hdcScreen == IntPtr.Zero)
                {
                    throw new Win32Exception(string.Format("Failed GetDC({0})", hWnd));
                }

                try
                {
                    hdcCompatible = NativeMethods.CreateCompatibleDC(hdcScreen);

                    NativeMethods.RECT rect = new NativeMethods.RECT();
                    NativeMethods.GetWindowRect(hWnd, ref rect);
                    screenX = rect.right - rect.left;
                    screenY = rect.bottom - rect.top;

                    try
                    {
                        hBmp = NativeMethods.CreateCompatibleBitmap(hdcScreen, screenX, screenY);
                        if (hBmp == IntPtr.Zero)
                        {
                            throw new Win32Exception(string.Format("Failed CreateCompatibleBitmap({0}, {1}, {2})", hdcScreen, screenX, screenY));
                        }

                        try
                        {
                            hOldBmp = (IntPtr)NativeMethods.SelectObject(hdcCompatible, hBmp);
                            bool succeeded = NativeMethods.BitBlt(hdcCompatible, 0, 0, screenX, screenY, hdcScreen, 0, 0, 13369376);
                            if (!succeeded)
                            {
                                throw new Win32Exception(string.Format("Failed BitBlt({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8})",
                                                                       hdcCompatible, 0, 0, screenX, screenY, hdcScreen, 0, 0, 13369376));
                            }
                        }
                        finally
                        {
                            if (hOldBmp != IntPtr.Zero)
                                NativeMethods.SelectObject(hdcCompatible, hOldBmp);
                        }

                        return System.Drawing.Image.FromHbitmap(hBmp);
                    }
                    finally
                    {
                        if (hBmp != IntPtr.Zero)
                            NativeMethods.DeleteObject(hBmp);
                    }
                }
                finally
                {
                    if (hdcCompatible != IntPtr.Zero)
                        NativeMethods.DeleteDC(hdcCompatible);
                }
            }
            finally
            {
                if (hdcScreen != IntPtr.Zero)
                    NativeMethods.ReleaseDC(hWnd, hdcScreen);
            }
        }

        internal override bool IsReady
        {
            get { return true; }
        }
    }
}
