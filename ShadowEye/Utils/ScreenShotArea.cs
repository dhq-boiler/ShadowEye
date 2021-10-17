

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace ShadowEye.Utils
{
    public abstract class ScreenShotArea
    {
        abstract internal Bitmap GetScreenShot();
        abstract internal bool IsReady { get; }

        public static BitmapSource ToBitmapSource(Bitmap bitmap)
        {
            IntPtr ptr = IntPtr.Zero;
            try
            {
                ptr = bitmap.GetHbitmap();

                return Imaging.CreateBitmapSourceFromHBitmap(
                    ptr, IntPtr.Zero, System.Windows.Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions()
                );
            }
            finally
            {
                if (ptr != IntPtr.Zero)
                    NativeMethods.DeleteObject(ptr);
            }
        }
    }
}
