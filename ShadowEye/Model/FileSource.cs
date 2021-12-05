

using OpenCvSharp;
using System;
using System.IO;
using System.Windows.Media;

namespace ShadowEye.Model
{
    public class FileSource : AnalyzingSource
    {
        public FileSource(string path) : base(Path.GetFileName(path))
        {
            try
            {
                this.HowToUpdate = new StaticUpdater(this);
                Location = new Uri(path);
                UpdateImage();
                ChannelType = GetChannelType(Bitmap.Format);
            }
            catch (ArgumentException)
            {
                throw;
            }
        }

        private libimgengCore.ChannelType GetChannelType(System.Windows.Media.PixelFormat pixelFormat)
        {
            if (pixelFormat == PixelFormats.Bgr24)
                return libimgengCore.ChannelType.BGR24;
            else if (pixelFormat == PixelFormats.Bgr32)
                return libimgengCore.ChannelType.BGR32;
            else if (pixelFormat == PixelFormats.Bgra32)
                return libimgengCore.ChannelType.BGRA;
            else if (pixelFormat == PixelFormats.Gray8 || (Mat != null && Mat.Type().Channels == 1))
                return libimgengCore.ChannelType.Gray;
            else if (pixelFormat == PixelFormats.Rgb24)
                return libimgengCore.ChannelType.RGB;
            else
                return libimgengCore.ChannelType.Unknown;
        }

        public override void UpdateImage()
        {
            try
            {
                Mat = new Mat(Location.LocalPath, ImreadModes.Unchanged);
                SetBitmapFromMat(Mat);
                OnSourceUpdated(this, new EventArgs());
            }
            catch (ArgumentException)
            {
                throw;
            }
        }

        public override bool UpdateOnce
        {
            get { return true; }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FileSource)) return false;
            return this.Location.Equals((obj as FileSource).Location);
        }

        public override int GetHashCode()
        {
            return this.Location.GetHashCode();
        }

        public override string ToString()
        {
            return Location.LocalPath.ToString();
        }

        public override void Activate()
        {
        }

        public override void Deactivate()
        {
        }
    }
}
