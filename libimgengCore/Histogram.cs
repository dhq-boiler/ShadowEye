

using OpenCvSharp;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace libimgengCore
{
    public class Histogram : IDisposable, INotifyPropertyChanged
    {
        public Histogram()
        {
            _IsCreatedBitmap = false;
        }

        public void Initialize(Mat mat, ChannelType channelType)
        {
            Debug.Assert(mat != null, "mat != null");
            try
            {
                MatType type = mat.Type();
                if (type.Channels <= 0)
                    throw new ArgumentOutOfRangeException("type.Channels <= 0");
                this.Type = channelType;
                Width = mat.Width;
                Height = mat.Height;
                if (!_IsCreatedBitmap) InitWBitmap();
                InitHueStandard();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Calculate(Mat mat)
        {
            Debug.Assert(mat != null, "mat != null");

            int[] hueHistogram, saturationHistogram, valueHistogram, firstHistogram, secondHistogram, thirdHistogram;

            using (Mat hsv = mat.Clone())
            {
                switch (Type)
                {
                    case ChannelType.BGR24:
                        Cv2.CvtColor(mat, hsv, OpenCvSharp.ColorConversionCodes.BGR2HSV);
                        break;
                    case ChannelType.BGR32:
                        using (Mat itm = new Mat(mat.Height, mat.Width, MatType.CV_8UC3))
                        {
                            Cv2.CvtColor(mat, itm, OpenCvSharp.ColorConversionCodes.BGRA2BGR);
                            Cv2.CvtColor(itm, hsv, OpenCvSharp.ColorConversionCodes.BGR2HSV);
                        }
                        break;
                    case ChannelType.BGRA:
                        using (Mat itm = mat.Clone())
                        {
                            Cv2.CvtColor(mat, itm, OpenCvSharp.ColorConversionCodes.BGRA2BGR);
                            Cv2.CvtColor(itm, hsv, OpenCvSharp.ColorConversionCodes.BGR2HSV);
                        }
                        break;
                    case ChannelType.Gray:
                        using (Mat itm = mat.Clone())
                        {
                            Cv2.CvtColor(mat, itm, OpenCvSharp.ColorConversionCodes.GRAY2BGR);
                            Cv2.CvtColor(itm, hsv, OpenCvSharp.ColorConversionCodes.BGR2HSV);
                        }
                        break;
                    case ChannelType.RGB:
                        Cv2.CvtColor(mat, hsv, OpenCvSharp.ColorConversionCodes.RGB2HSV);
                        break;
                    case ChannelType.RGBA:
                        using (Mat itm = mat.Clone())
                        {
                            Cv2.CvtColor(mat, itm, OpenCvSharp.ColorConversionCodes.RGBA2BGR);
                            Cv2.CvtColor(itm, hsv, OpenCvSharp.ColorConversionCodes.BGR2HSV);
                        }
                        break;
                    //case ChannelType.BlackWhite:
                    //    break;
                    //case ChannelType.CMYK:
                    //    break;
                    //case ChannelType.PBGRA:
                    //    break;
                    //case ChannelType.PRGBA:
                    //    break;
                    default:
                        throw new NotSupportedException(Type.ToString());
                }

                hueHistogram = CreateHistogram(hsv, 0, 181);   //Hue
                saturationHistogram = CreateHistogram(hsv, 1, 256);   //Saturation
                valueHistogram = CreateHistogram(hsv, 2, 256);   //Value

                ComputeStatistic(hsv);
            }

            firstHistogram = CreateHistogram(mat, 0, 256);   //First
            secondHistogram = CreateHistogram(mat, 1, 256);   //Second
            thirdHistogram = CreateHistogram(mat, 2, 256);   //Third

            _HueMaxFrequency = hueHistogram.Max(a => a);
            _SaturationMaxFrequency = saturationHistogram.Max(a => a);
            _ValueMaxFrequency = valueHistogram.Max(a => a);
            _FirstMaxFrequency = firstHistogram.Max(a => a);
            _SecondMaxFrequency = secondHistogram.Max(a => a);
            _ThirdMaxFrequency = thirdHistogram.Max(a => a);

            DrawHistogramLog10(HHistogram, hueHistogram, _HueMaxFrequency, (x) => _HueStandard[x, 0], (x) => _HueStandard[x, 1], (x) => _HueStandard[x, 2]);
            DrawHistogramLog10(SHistogram, saturationHistogram, _SaturationMaxFrequency, (x) => (byte)(255 - x), (x) => (byte)(255 - x), (x) => 255);
            DrawHistogramLog10(VHistogram, valueHistogram, _ValueMaxFrequency, 255, 255, 255);

            switch (Type)
            {
                case ChannelType.BGR24:
                case ChannelType.BGR32:
                case ChannelType.BGRA:
                    DrawHistogramLog10(FirstHistogram, firstHistogram, _FirstMaxFrequency, 255, 0, 0);
                    DrawHistogramLog10(SecondHistogram, secondHistogram, _SecondMaxFrequency, 0, 255, 0);
                    DrawHistogramLog10(ThirdHistogram, thirdHistogram, _ThirdMaxFrequency, 0, 0, 255);
                    break;
                case ChannelType.RGB:
                case ChannelType.RGBA:
                    DrawHistogramLog10(FirstHistogram, firstHistogram, _FirstMaxFrequency, 255, 0, 0);
                    DrawHistogramLog10(SecondHistogram, secondHistogram, _SecondMaxFrequency, 0, 255, 0);
                    DrawHistogramLog10(ThirdHistogram, thirdHistogram, _ThirdMaxFrequency, 0, 0, 255);
                    break;
                case ChannelType.Gray:
                default:
                    break;
            }

            OnPropertyChanged();
        }

        public int Channels { get; private set; }
        public double Average { get; private set; }
        public double StandardDeviation { get; private set; }
        public double Median { get; private set; }
        public int Pixels { get { return Width * Height; } }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public ChannelType Type { get; private set; }
        public double MaxFrequency { get; private set; }
        public WriteableBitmap HHistogram { get; private set; }
        public WriteableBitmap SHistogram { get; private set; }
        public WriteableBitmap VHistogram { get; private set; }
        public WriteableBitmap FirstHistogram { get; private set; }
        public WriteableBitmap SecondHistogram { get; private set; }
        public WriteableBitmap ThirdHistogram { get; private set; }

        private unsafe void ComputeStatistic(Mat hsv)
        {
            Debug.Assert(hsv != null, "hsv != null");

            double average_value = 0.0;
            double standardDeviation_value = 0.0;
            double median_value = 0.0;
            byte[] values = CopyOneChannelToArray(hsv, 2);
            int valuesLength = values.Length;
            int valuesLengthHalf = valuesLength / 2;

            average_value = values.Average(a => a);

            for (int i = 0; i < valuesLength; ++i)
            {
                double d = values[i] - average_value;
                standardDeviation_value += d * d;
            }
            standardDeviation_value /= valuesLength;
            standardDeviation_value = Math.Sqrt(standardDeviation_value);

            var sorted = values.OrderBy(a => a);
            if (Modulo((uint)valuesLength, 2) == 0)
            {
                median_value = (sorted.ElementAt(valuesLengthHalf - 1) + sorted.ElementAt(valuesLengthHalf)) / 2.0;
            }
            else
            {
                median_value = values[valuesLengthHalf];
            }

            Average = average_value;
            StandardDeviation = standardDeviation_value;
            Median = median_value;
        }

        private uint Modulo(uint p1, uint p2)
        {
            uint a = p1;
            uint b = p2;
            b <<= 31;
            for (int i = 0; i < 32; ++i)
            {
                if (a >= b)
                {
                    a -= b;
                }
                b >>= 1;
            }
            return a;
        }

        private unsafe static byte[] CopyOneChannelToArray(Mat hsv, int channel)
        {
            Debug.Assert(hsv != null, "hsv != null");
            byte* p = (byte*)hsv.Data.ToPointer();
            long step = hsv.Step();
            int channels = hsv.Channels();
            int width = hsv.Width;
            int height = hsv.Height;

            byte[] values = new byte[hsv.Width * hsv.Height];
            Parallel.For(0, height, y =>
            {
                for (int x = 0; x < width; ++x)
                {
                    values[y * width + x] = (*(p + y * step + x * channels + channel));
                }
            });

            return values;
        }

        private unsafe void DrawHistogram(WriteableBitmap bitmap, int[] histogram, double maxFrequency, byte b, byte g, byte r)
        {
            DrawHistogram(bitmap, histogram, maxFrequency, (x) => b, (x) => g, (x) => r);
        }

        private unsafe void DrawHistogramLog10(WriteableBitmap bitmap, int[] histogram, double maxFrequency, byte b, byte g, byte r)
        {
            DrawHistogramLog10(bitmap, histogram, maxFrequency, (x) => b, (x) => g, (x) => r);
        }

        private unsafe void DrawHistogram(WriteableBitmap bitmap, int[] histogram, double maxFrequency, Func<int, byte> b, Func<int, byte> g, Func<int, byte> r)
        {
            try
            {
                bitmap.Lock();

                byte* p = (byte*)bitmap.BackBuffer.ToPointer();
                int height = bitmap.PixelHeight;
                int width = bitmap.PixelWidth;
                int backbufferStride = bitmap.BackBufferStride;
                Parallel.For(0, width, x =>
                {
                    int count = histogram[x];
                    for (int y = 0; y < height; ++y)
                    {
                        if (y <= (int)(count / maxFrequency * height))
                        {
                            *(p + x * 3 + (height - y) * backbufferStride + 0) = b(x);
                            *(p + x * 3 + (height - y) * backbufferStride + 1) = g(x);
                            *(p + x * 3 + (height - y) * backbufferStride + 2) = r(x);
                        }
                        else
                        {
                            *(p + x * 3 + (height - y) * backbufferStride + 0) = 0;
                            *(p + x * 3 + (height - y) * backbufferStride + 1) = 0;
                            *(p + x * 3 + (height - y) * backbufferStride + 2) = 0;
                        }
                    }
                });

                bitmap.AddDirtyRect(new Int32Rect(0, 0, width, height));
            }
            finally
            {
                bitmap.Unlock();
            }
        }

        private unsafe void DrawHistogramLog10(WriteableBitmap bitmap, int[] histogram, double maxFrequency, Func<int, byte> b, Func<int, byte> g, Func<int, byte> r)
        {
            try
            {
                bitmap.Lock();

                byte* p = (byte*)bitmap.BackBuffer.ToPointer();
                int height = bitmap.PixelHeight;
                int width = bitmap.PixelWidth;
                int backbufferStride = bitmap.BackBufferStride;
                Parallel.For(0, width, x =>
                {
                    int count = histogram[x];
                    double log10_count = Math.Log10(count);
                    double log10_maxFrequency = Math.Log10(maxFrequency);
                    for (int y = 0; y < height; ++y)
                    {
                        if (y <= (int)(log10_count / log10_maxFrequency * (double)height))
                        {
                            *(p + x * 3 + (height - y) * backbufferStride + 0) = b(x);
                            *(p + x * 3 + (height - y) * backbufferStride + 1) = g(x);
                            *(p + x * 3 + (height - y) * backbufferStride + 2) = r(x);
                        }
                        else
                        {
                            *(p + x * 3 + (height - y) * backbufferStride + 0) = 0;
                            *(p + x * 3 + (height - y) * backbufferStride + 1) = 0;
                            *(p + x * 3 + (height - y) * backbufferStride + 2) = 0;
                        }
                    }
                });

                bitmap.AddDirtyRect(new Int32Rect(0, 0, width, height));
            }
            finally
            {
                bitmap.Unlock();
            }
        }

        private unsafe int[] CreateHistogram(Mat mat, int channel, int bin)
        {
            // ローカルヒストグラムの配列を初期化
            int[][] localHistograms = new int[Environment.ProcessorCount][];
            for (int i = 0; i < localHistograms.Length; i++)
            {
                localHistograms[i] = new int[bin];
            }

            byte* p = (byte*)mat.Data.ToPointer();
            int rows = mat.Rows;
            int cols = mat.Cols;
            long step = mat.Step();
            int channels = mat.Channels();

            // 並列処理で各行を処理
            Parallel.For(0, rows, (y, state) =>
            {
                int threadIndex = Thread.CurrentThread.ManagedThreadId % Environment.ProcessorCount;
                int[] threadHistogram = localHistograms[threadIndex];

                long mat_y = y * step;
                for (int x = 0; x < cols; ++x)
                {
                    byte value = *(p + mat_y + x * channels + channel);
                    threadHistogram[value]++;
                }
            });

            // 全てのローカルヒストグラムを合算
            int[] histogram = new int[bin];
            foreach (var localHistogram in localHistograms)
            {
                for (int i = 0; i < bin; i++)
                {
                    histogram[i] += localHistogram[i];
                }
            }

            return histogram;
        }

        private void InitWBitmap()
        {
            HHistogram = new WriteableBitmap(181, 100, 96, 96, PixelFormats.Bgr24, null);
            SHistogram = new WriteableBitmap(256, 100, 96, 96, PixelFormats.Bgr24, null);
            VHistogram = new WriteableBitmap(256, 100, 96, 96, PixelFormats.Bgr24, null);
            FirstHistogram = new WriteableBitmap(256, 100, 96, 96, PixelFormats.Bgr24, null);
            SecondHistogram = new WriteableBitmap(256, 100, 96, 96, PixelFormats.Bgr24, null);
            ThirdHistogram = new WriteableBitmap(256, 100, 96, 96, PixelFormats.Bgr24, null);
            _IsCreatedBitmap = true;
        }

        private unsafe void InitHueStandard()
        {
            try
            {
                _HueStandard = new byte[181, 3];

                using (Mat BGR = new Mat(new OpenCvSharp.Size(181, 1), MatType.CV_8UC3))
                using (Mat HSV = BGR.Clone())
                {
                    byte* p = (byte*)BGR.Data.ToPointer();
                    for (int x = 0; x < BGR.Cols; ++x)
                    {
                        *(p + x * 3) = 255;         //B
                        *(p + x * 3 + 1) = 255;     //G
                        *(p + x * 3 + 2) = 255;     //R
                    }
                    Cv2.CvtColor(BGR, HSV, OpenCvSharp.ColorConversionCodes.BGR2HSV);
                    p = (byte*)HSV.Data.ToPointer();
                    for (int x = 0; x < HSV.Cols; ++x)
                    {
                        *(p + x * 3) = (byte)x;     //Hue
                        *(p + x * 3 + 1) = 255;     //Saturation
                        *(p + x * 3 + 2) = 255;     //Value
                    }
                    Cv2.CvtColor(HSV, BGR, OpenCvSharp.ColorConversionCodes.HSV2BGR);
                    p = (byte*)BGR.Data.ToPointer();
                    for (int x = 0; x < BGR.Cols; ++x)
                    {
                        _HueStandard[x, 0] = *(p + x * 3);       //B
                        _HueStandard[x, 1] = *(p + x * 3 + 1);   //G
                        _HueStandard[x, 2] = *(p + x * 3 + 2);   //R
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        private bool _IsCreatedBitmap;
        private byte[,] _HueStandard;
        private int _HueMaxFrequency;
        private int _SaturationMaxFrequency;
        private int _ValueMaxFrequency;
        private int _FirstMaxFrequency;
        private int _SecondMaxFrequency;
        private int _ThirdMaxFrequency;

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        private bool _disposed;
        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    //マネージリソースの解放
                    HHistogram = null;
                    SHistogram = null;
                    VHistogram = null;
                    FirstHistogram = null;
                    SecondHistogram = null;
                    ThirdHistogram = null;
                    _IsCreatedBitmap = false;
                }

                //アンマネージリソースの解放

                _disposed = true;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName = null)
        {
            PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
