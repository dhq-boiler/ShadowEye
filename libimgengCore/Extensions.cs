using OpenCvSharp;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace libimgengCore
{
    public static class Extensions
    {
        public static WriteableBitmap ToWriteableBitmap(this Mat mat)
        {
            // WriteableBitmapを作成
            WriteableBitmap bitmap = new WriteableBitmap(mat.Width, mat.Height, 96, 96, PixelFormats.Bgr24, null);

            // MatのデータをWriteableBitmapにコピー
            bitmap.Lock();
            try
            {
                IntPtr sourcePtr = mat.Data;
                int stride = mat.Width * mat.Channels();
                int bufferSize = mat.Height * stride;
                bitmap.WritePixels(new Int32Rect(0, 0, mat.Width, mat.Height), sourcePtr, bufferSize, stride);
            }
            finally
            {
                bitmap.Unlock();
            }

            return bitmap;
        }

        public static Mat ToMat(this Bitmap bitmap)
        {
            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            // BitmapをLockしてピクセルデータにアクセス
            BitmapData bitmapData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            try
            {
                // Matを作成してピクセルデータをコピー
                Mat mat = new Mat(bitmap.Height, bitmap.Width, MatType.CV_8UC3, bitmapData.Scan0);
                return mat.Clone(); // Matのコピーを返す
            }
            finally
            {
                // ロックを解除
                bitmap.UnlockBits(bitmapData);
            }
        }
    }
}
