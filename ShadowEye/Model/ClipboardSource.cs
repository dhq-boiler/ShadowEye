using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ShadowEye.Model
{
    internal class ClipboardSource : AnalyzingSource
    {
        private static int count = 0;
        private BitmapSource bitmap;

        public ClipboardSource(BitmapSource bitmap)
            : base($"clipboard-{++count}")
        {
            this.bitmap = bitmap;
            this.HowToUpdate = new StaticUpdater(this);
            UpdateImage();
            ChannelType = GetChannelType(Bitmap.Format);
        }
        private libimgeng.ChannelType GetChannelType(System.Windows.Media.PixelFormat pixelFormat)
        {
            if (pixelFormat == PixelFormats.Bgr24)
                return libimgeng.ChannelType.BGR24;
            else if (pixelFormat == PixelFormats.Bgr32)
                return libimgeng.ChannelType.BGR32;
            else if (pixelFormat == PixelFormats.Bgra32)
                return libimgeng.ChannelType.BGRA;
            else if (pixelFormat == PixelFormats.Gray8 || (Mat != null && Mat.Type().Channels == 1))
                return libimgeng.ChannelType.Gray;
            else if (pixelFormat == PixelFormats.Rgb24)
                return libimgeng.ChannelType.RGB;
            else
                return libimgeng.ChannelType.Unknown;
        }

        public override bool UpdateOnce => throw new System.NotImplementedException();

        public override void Activate()
        {
        }

        public override void Deactivate()
        {
        }

        public override void UpdateImage()
        {
            Bitmap = new WriteableBitmap(bitmap);
            OnSourceUpdated(this, new System.EventArgs());
        }
    }
}