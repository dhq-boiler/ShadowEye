// Copyright © 2015 dhq_boiler.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace libSevenTools.WPFControls.Imaging
{
    internal class Decorder
    {
        public static WriteableBitmap LoadBitmap(string path)
        {
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
                {
                    BitmapDecoder decoder = BitmapDecoder.Create(fs, BitmapCreateOptions.IgnoreImageCache, BitmapCacheOption.Default);

                    FormatConvertedBitmap bmpSrc = new FormatConvertedBitmap(decoder.Frames[0], PixelFormats.Pbgra32, null, 0);

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
