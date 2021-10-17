

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShadowEye.Utils
{
    internal class ScreenShotVirtualScreen : ScreenShotArea
    {
        internal ScreenShotVirtualScreen()
        { }

        internal override Bitmap GetScreenShot()
        {
            try
            {
                return GetVirtualScreenBitmap();
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

        private static Bitmap GetVirtualScreenBitmap()
        {
            var virtualScreenRect = SystemInformation.VirtualScreen;

            try
            {
                Bitmap bmpScreenCapture = new Bitmap(virtualScreenRect.Width,
                                                     virtualScreenRect.Height);

                using (var g = Graphics.FromImage(bmpScreenCapture))
                {
                    g.CopyFromScreen(virtualScreenRect.X,
                                     virtualScreenRect.Y,
                                     0, 0,
                                     virtualScreenRect.Size,
                                     CopyPixelOperation.SourceCopy);
                }

                return bmpScreenCapture;
            }
            catch (Win32Exception)
            {
                //スクリーンセーバー切り替えによるWin32Exception
                throw;
            }
            catch (ArgumentException)
            {
                //スクリーンセーバー切り替えによるWin32Exception
                throw;
            }
        }

        internal override bool IsReady
        {
            get { return true; }
        }
    }
}
