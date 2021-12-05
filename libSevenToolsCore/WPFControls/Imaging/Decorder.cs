// Copyright © 2015 dhq_boiler.

using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace libSevenToolsCore.WPFControls.Imaging
{
    internal class Decorder
    {
        public static WriteableBitmap LoadBitmap(string path)
        {
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
                {
                    BitmapDecoder decoder = BitmapDecoder.Create(fs, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
                    FormatConvertedBitmap bmpSrc = new FormatConvertedBitmap(decoder.Frames[0], decoder.Frames[0].Format, null, 0);
                    return new WriteableBitmap(bmpSrc);
                }
            }
            catch (IOException)
            {
                return null;
            }
        }

        public static WriteableBitmap ConvertToPbgra32(WriteableBitmap src)
        {
            FormatConvertedBitmap bmpSrc = new FormatConvertedBitmap(src, PixelFormats.Pbgra32, null, 0);
            return new WriteableBitmap(bmpSrc);
        }
    }
}
