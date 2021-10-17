

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using ShadowEye.Model;
using ShadowEye.Utils;

namespace ShadowEye.ViewModel
{
    public class ImageViewModel : Notifier
    {
        public ImageViewModel(AnalyzingSource source, MainWorkbenchViewModel parent)
        {
            VerifyArgument(source, "source");
            VerifyArgument(parent, "parent");
            this.Source = source;
            ImageContainerVM = parent;
        }

        public void CloseThisTab()
        {
            VerifyArgument(ImageContainerVM, "ImageContainerVM");
            ImageContainerVM.CloseTab(this);
        }

        private static void VerifyArgument(object arg, string argName)
        {
            if (arg == null)
            {
                throw new ArgumentNullException(argName);
            }
        }

        public AnalyzingSource Source { get; set; }
        public string Header { get; set; }
        public MainWorkbenchViewModel ImageContainerVM { get; set; }
    }
}
