

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
    internal class ScreenShotScreen : ScreenShotArea
    {
        internal Screen SelectedScreen { get; set; }

        internal ScreenShotScreen()
        { }

        internal ScreenShotScreen(Screen selectedScreen)
        {
            SelectedScreen = selectedScreen;
        }

        internal override Bitmap GetScreenShot()
        {
            try
            {
                return GetScreenBitmap(SelectedScreen);
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

        private static Bitmap GetScreenBitmap(Screen screen)
        {
            try
            {
                Bitmap bmpScreenCapture = new Bitmap(screen.Bounds.Width,
                                                     screen.Bounds.Height);

                using (var g = Graphics.FromImage(bmpScreenCapture))
                {
                    g.CopyFromScreen(screen.Bounds.X,
                                     screen.Bounds.Y,
                                     0, 0,
                                     bmpScreenCapture.Size,
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
            get { return SelectedScreen != null; }
        }
    }
}
