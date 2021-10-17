

using OpenCvSharp;
using System;

namespace ShadowEye.Model
{
    public class ChannelIntegratedSource : ComputingSource
    {
        public ChannelIntegratedSource(string name, AnalyzingSource target, ComputingMethod method, MatType CalcuratingMatType)
            : base(name)
        {
            this.HowToUpdate = target.HowToUpdate.SameUpdater(this);
            LeftHand = target;
            Method = method;
            OutputColorType = EColorMode.BGR;
            this.CalcuratingMatType = CalcuratingMatType;

            LeftHand.MatChanged += LeftHand_MatChanged_OnlyLeft;
            UpdateImage();
        }

        public MatType CalcuratingMatType { get; private set; }

        private void LeftHand_MatChanged_OnlyLeft(object sender, MatChangedEventArgs e)
        {
            if (LeftHand != null)
                UpdateImage();
        }

        public override void Compute()
        {
            switch (Method)
            {
                case ComputingMethod.IntegrateChannel:
                    Mat = IntegrateChannels(LeftHand.Mat, false);
                    return;
                case ComputingMethod.IntegrateChannel_Normalize:
                    Mat = IntegrateChannels(LeftHand.Mat, true);
                    return;
                default:
                    throw new InvalidOperationException("Unknown computing method.");
            }
        }

        private unsafe Mat IntegrateChannels(Mat src, bool IsNormalize)
        {
            MatType type = CalcuratingMatType;

            Mat ret = new Mat(src.Rows, src.Cols, type);
            int srcChannels = src.Channels();
            int retChannels = ret.Channels();

            if (IsNormalize)
            {
                using (Mat tmp = new Mat(src.Rows, src.Cols, MatType.CV_64FC1))
                {
                    int tmpChannels = tmp.Channels();
                    double max = double.MinValue;

                    for (int y = 0; y < src.Rows; ++y)
                    {
                        byte* p_src = (byte*)src.Ptr(y).ToPointer();
                        double* p_tmp = (double*)tmp.Ptr(y).ToPointer();
                        for (int x = 0; x < src.Cols; ++x)
                        {
                            double value = 0.0;
                            for (int c = 0; c < srcChannels; ++c)
                            {
                                value += *(p_src + x * srcChannels + c);
                            }
                            *(p_tmp + x * tmpChannels) = value;
                            if (max < value) max = value;
                        }
                    }

                    switch (type.Depth)
                    {
                        case MatType.CV_16S:
                            IntegrateDouble2Short(tmp, ret, tmpChannels, retChannels, max);
                            break;
                        case MatType.CV_16U:
                            IntegrateDouble2UShort(tmp, ret, tmpChannels, retChannels, max);
                            break;
                        case MatType.CV_32F:
                            IntegrateDouble2Float(tmp, ret, tmpChannels, retChannels, max);
                            break;
                        case MatType.CV_32S:
                            IntegrateDouble2Int(tmp, ret, tmpChannels, retChannels, max);
                            break;
                        case MatType.CV_64F:
                            IntegrateDouble2Double(tmp, ret, tmpChannels, retChannels, max);
                            break;
                        case MatType.CV_8S:
                            IntegrateDouble2SByte(tmp, ret, tmpChannels, retChannels, max);
                            break;
                        case MatType.CV_8U:
                            IntegrateDouble2Byte(tmp, ret, tmpChannels, retChannels, max);
                            break;
                        default:
                            throw new NotSupportedException("Not supported type.");
                    }
                }
            }
            else
            {
                switch (type.Depth)
                {
                    case MatType.CV_16S:
                        IntegrateByte2Short(src, ret, srcChannels, retChannels);
                        break;
                    case MatType.CV_16U:
                        IntegrateByte2UShort(src, ret, srcChannels, retChannels);
                        break;
                    case MatType.CV_32F:
                        IntegrateByte2Float(src, ret, srcChannels, retChannels);
                        break;
                    case MatType.CV_32S:
                        IntegrateByte2Int(src, ret, srcChannels, retChannels);
                        break;
                    case MatType.CV_64F:
                        IntegrateByte2Double(src, ret, srcChannels, retChannels);
                        break;
                    case MatType.CV_8S:
                        IntegrateByte2SByte(src, ret, srcChannels, retChannels);
                        break;
                    case MatType.CV_8U:
                        IntegrateByte2Byte(src, ret, srcChannels, retChannels);
                        break;
                    default:
                        throw new NotSupportedException("Not supported type.");
                }
            }

            return ret;
        }

        unsafe private static void IntegrateDouble2Short(Mat src, Mat ret, int srcChannels, int retChannels, double max)
        {
            for (int y = 0; y < src.Rows; ++y)
            {
                double* p_src = (double*)src.Ptr(y).ToPointer();
                short* p_ret = (short*)ret.Ptr(y).ToPointer();
                for (int x = 0; x < src.Cols; ++x)
                {
                    *(p_ret + x * retChannels) = (short)(*(p_src + +x * srcChannels) / max * short.MaxValue);
                }
            }
        }

        unsafe private static void IntegrateDouble2UShort(Mat src, Mat ret, int srcChannels, int retChannels, double max)
        {
            for (int y = 0; y < src.Rows; ++y)
            {
                double* p_src = (double*)src.Ptr(y).ToPointer();
                ushort* p_ret = (ushort*)ret.Ptr(y).ToPointer();
                for (int x = 0; x < src.Cols; ++x)
                {
                    *(p_ret + x * retChannels) = (ushort)(*(p_src + +x * srcChannels) / max * ushort.MaxValue);
                }
            }
        }

        unsafe private static void IntegrateDouble2Float(Mat src, Mat ret, int srcChannels, int retChannels, double max)
        {
            for (int y = 0; y < src.Rows; ++y)
            {
                double* p_src = (double*)src.Ptr(y).ToPointer();
                float* p_ret = (float*)ret.Ptr(y).ToPointer();
                for (int x = 0; x < src.Cols; ++x)
                {
                    *(p_ret + x * retChannels) = (float)(*(p_src + +x * srcChannels) / max * float.MaxValue);
                }
            }
        }

        unsafe private static void IntegrateDouble2Int(Mat src, Mat ret, int srcChannels, int retChannels, double max)
        {
            for (int y = 0; y < src.Rows; ++y)
            {
                double* p_src = (double*)src.Ptr(y).ToPointer();
                int* p_ret = (int*)ret.Ptr(y).ToPointer();
                for (int x = 0; x < src.Cols; ++x)
                {
                    *(p_ret + x * retChannels) = (int)(*(p_src + +x * srcChannels) / max * int.MaxValue);
                }
            }
        }

        unsafe private static void IntegrateDouble2Double(Mat src, Mat ret, int srcChannels, int retChannels, double max)
        {
            for (int y = 0; y < src.Rows; ++y)
            {
                double* p_src = (double*)src.Ptr(y).ToPointer();
                double* p_ret = (double*)ret.Ptr(y).ToPointer();
                for (int x = 0; x < src.Cols; ++x)
                {
                    *(p_ret + x * retChannels) = (double)(*(p_src + +x * srcChannels) / max * double.MaxValue);
                }
            }
        }

        unsafe private static void IntegrateDouble2SByte(Mat src, Mat ret, int srcChannels, int retChannels, double max)
        {
            for (int y = 0; y < src.Rows; ++y)
            {
                double* p_src = (double*)src.Ptr(y).ToPointer();
                sbyte* p_ret = (sbyte*)ret.Ptr(y).ToPointer();
                for (int x = 0; x < src.Cols; ++x)
                {
                    *(p_ret + x * retChannels) = (sbyte)(*(p_src + +x * srcChannels) / max * sbyte.MaxValue);
                }
            }
        }

        unsafe private static void IntegrateDouble2Byte(Mat src, Mat ret, int srcChannels, int retChannels, double max)
        {
            for (int y = 0; y < src.Rows; ++y)
            {
                double* p_src = (double*)src.Ptr(y).ToPointer();
                byte* p_ret = (byte*)ret.Ptr(y).ToPointer();
                for (int x = 0; x < src.Cols; ++x)
                {
                    *(p_ret + x * retChannels) = (byte)(*(p_src + +x * srcChannels) / max * byte.MaxValue);
                }
            }
        }

        unsafe private static void IntegrateByte2Short(Mat src, Mat ret, int srcChannels, int retChannels)
        {
            for (int y = 0; y < src.Rows; ++y)
            {
                byte* p_src = (byte*)src.Ptr(y).ToPointer();
                short* p_ret = (short*)ret.Ptr(y).ToPointer();
                for (int x = 0; x < src.Cols; ++x)
                {
                    short value = 0;
                    for (int c = 0; c < srcChannels; ++c)
                    {
                        value += *(p_src + x * srcChannels + c);
                    }
                    *(p_ret + x * retChannels) = value;
                }
            }
        }

        unsafe private static void IntegrateByte2UShort(Mat src, Mat ret, int srcChannels, int retChannels)
        {
            for (int y = 0; y < src.Rows; ++y)
            {
                byte* p_src = (byte*)src.Ptr(y).ToPointer();
                ushort* p_ret = (ushort*)ret.Ptr(y).ToPointer();
                for (int x = 0; x < src.Cols; ++x)
                {
                    ushort value = 0;
                    for (int c = 0; c < srcChannels; ++c)
                    {
                        value += *(p_src + x * srcChannels + c);
                    }
                    *(p_ret + x * retChannels) = value;
                }
            }
        }

        unsafe private static void IntegrateByte2Float(Mat src, Mat ret, int srcChannels, int retChannels)
        {
            for (int y = 0; y < src.Rows; ++y)
            {
                byte* p_src = (byte*)src.Ptr(y).ToPointer();
                float* p_ret = (float*)ret.Ptr(y).ToPointer();
                for (int x = 0; x < src.Cols; ++x)
                {
                    float value = 0;
                    for (int c = 0; c < srcChannels; ++c)
                    {
                        value += *(p_src + x * srcChannels + c);
                    }
                    *(p_ret + x * retChannels) = value;
                }
            }
        }

        unsafe private static void IntegrateByte2Int(Mat src, Mat ret, int srcChannels, int retChannels)
        {
            for (int y = 0; y < src.Rows; ++y)
            {
                byte* p_src = (byte*)src.Ptr(y).ToPointer();
                int* p_ret = (int*)ret.Ptr(y).ToPointer();
                for (int x = 0; x < src.Cols; ++x)
                {
                    int value = 0;
                    for (int c = 0; c < srcChannels; ++c)
                    {
                        value += *(p_src + x * srcChannels + c);
                    }
                    *(p_ret + x * retChannels) = value;
                }
            }
        }

        unsafe private static void IntegrateByte2Double(Mat src, Mat ret, int srcChannels, int retChannels)
        {
            for (int y = 0; y < src.Rows; ++y)
            {
                byte* p_src = (byte*)src.Ptr(y).ToPointer();
                double* p_ret = (double*)ret.Ptr(y).ToPointer();
                for (int x = 0; x < src.Cols; ++x)
                {
                    double value = 0;
                    for (int c = 0; c < srcChannels; ++c)
                    {
                        value += *(p_src + x * srcChannels + c);
                    }
                    *(p_ret + x * retChannels) = value;
                }
            }
        }

        unsafe private static void IntegrateByte2SByte(Mat src, Mat ret, int srcChannels, int retChannels)
        {
            for (int y = 0; y < src.Rows; ++y)
            {
                byte* p_src = (byte*)src.Ptr(y).ToPointer();
                sbyte* p_ret = (sbyte*)ret.Ptr(y).ToPointer();
                for (int x = 0; x < src.Cols; ++x)
                {
                    sbyte value = 0;
                    for (int c = 0; c < srcChannels; ++c)
                    {
                        value += (sbyte)*(p_src + x * srcChannels + c);
                    }
                    *(p_ret + x * retChannels) = value;
                }
            }
        }

        unsafe private static void IntegrateByte2Byte(Mat src, Mat ret, int srcChannels, int retChannels)
        {
            for (int y = 0; y < src.Rows; ++y)
            {
                byte* p_src = (byte*)src.Ptr(y).ToPointer();
                byte* p_ret = (Byte*)ret.Ptr(y).ToPointer();
                for (int x = 0; x < src.Cols; ++x)
                {
                    byte value = 0;
                    for (int c = 0; c < srcChannels; ++c)
                    {
                        value += *(p_src + x * srcChannels + c);
                    }
                    *(p_ret + x * retChannels) = value;
                }
            }
        }

        //private static MatType SelectMatType(Mat src)
        //{
        //    MatType type = src.Type();
        //    switch (type.Depth)
        //    {
        //        case MatType.CV_16S:
        //            type = MatType.CV_16SC(1);
        //            break;
        //        case MatType.CV_16U:
        //            type = MatType.CV_16UC(1);
        //            break;
        //        case MatType.CV_32F:
        //            type = MatType.CV_32FC(1);
        //            break;
        //        case MatType.CV_32S:
        //            type = MatType.CV_32SC(1);
        //            break;
        //        case MatType.CV_64F:
        //            type = MatType.CV_64FC(1);
        //            break;
        //        case MatType.CV_8S:
        //            type = MatType.CV_8SC(1);
        //            break;
        //        case MatType.CV_8U:
        //            type = MatType.CV_8UC(1);
        //            break;
        //        default:
        //            throw new NotSupportedException("Not supported type.");
        //    }
        //    return type;
        //}
    }
}
