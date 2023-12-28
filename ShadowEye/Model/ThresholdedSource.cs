

using OpenCvSharp;
using System;

namespace ShadowEye.Model
{
    public class ThresholdedSource : ComputingSource
    {
        private double _Threshold;
        private double _ThresholdingMaxValue;

        public ThresholdedSource(string name, AnalyzingSource target, ComputingMethod thresholdMethod, double threshold, double maxValue)
            : base(name)
        {
            this.HowToUpdate = target.HowToUpdate.SameUpdater(this);
            LeftHand = target;
            Method = thresholdMethod;
            OutputColorType = EColorMode.Grayscale;
            ThresholdValue = threshold;
            ThresholdingMaxValue = maxValue;

            LeftHand.MatChanged += LeftHand_MatChanged_OnlyLeft;
            UpdateImage();
        }

        private void LeftHand_MatChanged_OnlyLeft(object sender, MatChangedEventArgs e)
        {
            if (LeftHand != null)
                UpdateImage();
        }

        public override void Compute()
        {
            using (Mat newMat = new Mat(LeftHand.Mat.Value.Rows, LeftHand.Mat.Value.Cols, MatType.CV_8UC1))
            {
                switch (Method)
                {
                    case ComputingMethod.Threshold_Binary:
                    case ComputingMethod.Threshold_Binary_Inverse:
                    case ComputingMethod.Threshold_ToZero:
                    case ComputingMethod.Threshold_ToZero_Inverse:
                    case ComputingMethod.Threshold_Trunc:
                        Threshold(LeftHand.Mat.Value, newMat, ThresholdValue, ThresholdingMaxValue, ToThresholdType(Method));
                        break;
                    default:
                        throw new InvalidOperationException("Unknown computing method.");
                }
                Mat.Value = newMat.Clone();
            }
        }

        private unsafe void Threshold(Mat src, Mat dest, double threshold, double maxValue, ThresholdTypes type)
        {
            int srcRows = src.Rows;
            int srcCols = src.Cols;
            int destRows = dest.Rows;
            int destCols = dest.Cols;

            switch (src.Type().Depth)
            {
                case MatType.CV_16S:
                    ThresholdShort2Byte(src, dest, threshold, maxValue, type, srcRows, srcCols);
                    break;
                case MatType.CV_16U:
                    ThresholdUShort2Byte(src, dest, threshold, maxValue, type, srcRows, srcCols);
                    break;
                case MatType.CV_32F:
                    ThresholdFloat2Byte(src, dest, threshold, maxValue, type, srcRows, srcCols);
                    break;
                case MatType.CV_32S:
                    ThresholdInt2Byte(src, dest, threshold, maxValue, type, srcRows, srcCols);
                    break;
                case MatType.CV_64F:
                    ThresholdDouble2Byte(src, dest, threshold, maxValue, type, srcRows, srcCols);
                    break;
                case MatType.CV_8S:
                    ThresholdSByte2Byte(src, dest, threshold, maxValue, type, srcRows, srcCols);
                    break;
                case MatType.CV_8U:
                    ThresholdByte2Byte(src, dest, threshold, maxValue, type, srcRows, srcCols);
                    break;
                default:
                    throw new NotSupportedException("Not supported type.");
            }
        }

        unsafe private void ThresholdShort2Byte(Mat src, Mat dest, double threshold, double maxValue, ThresholdTypes type, int srcRows, int srcCols)
        {
            for (int y = 0; y < srcRows; ++y)
            {
                short* p_s = (short*)src.Ptr(y);
                byte* p_d = (byte*)dest.Ptr(y);
                for (int x = 0; x < srcCols; ++x)
                {
                    short value = *(p_s + x);
                    *(p_d + x) = (byte)Thresholding(value, threshold, maxValue, type);
                }
            }
        }

        unsafe private void ThresholdUShort2Byte(Mat src, Mat dest, double threshold, double maxValue, ThresholdTypes type, int srcRows, int srcCols)
        {
            for (int y = 0; y < srcRows; ++y)
            {
                ushort* p_s = (ushort*)src.Ptr(y);
                byte* p_d = (byte*)dest.Ptr(y);
                for (int x = 0; x < srcCols; ++x)
                {
                    ushort value = *(p_s + x);
                    *(p_d + x) = (byte)Thresholding(value, threshold, maxValue, type);
                }
            }
        }

        unsafe private void ThresholdFloat2Byte(Mat src, Mat dest, double threshold, double maxValue, ThresholdTypes type, int srcRows, int srcCols)
        {
            for (int y = 0; y < srcRows; ++y)
            {
                float* p_s = (float*)src.Ptr(y);
                byte* p_d = (byte*)dest.Ptr(y);
                for (int x = 0; x < srcCols; ++x)
                {
                    float value = *(p_s + x);
                    *(p_d + x) = (byte)Thresholding(value, threshold, maxValue, type);
                }
            }
        }

        unsafe private void ThresholdInt2Byte(Mat src, Mat dest, double threshold, double maxValue, ThresholdTypes type, int srcRows, int srcCols)
        {
            for (int y = 0; y < srcRows; ++y)
            {
                int* p_s = (int*)src.Ptr(y);
                byte* p_d = (byte*)dest.Ptr(y);
                for (int x = 0; x < srcCols; ++x)
                {
                    int value = *(p_s + x);
                    *(p_d + x) = (byte)Thresholding(value, threshold, maxValue, type);
                }
            }
        }

        unsafe private void ThresholdDouble2Byte(Mat src, Mat dest, double threshold, double maxValue, ThresholdTypes type, int srcRows, int srcCols)
        {
            for (int y = 0; y < srcRows; ++y)
            {
                double* p_s = (double*)src.Ptr(y);
                byte* p_d = (byte*)dest.Ptr(y);
                for (int x = 0; x < srcCols; ++x)
                {
                    double value = *(p_s + x);
                    *(p_d + x) = (byte)Thresholding(value, threshold, maxValue, type);
                }
            }
        }

        unsafe private void ThresholdSByte2Byte(Mat src, Mat dest, double threshold, double maxValue, ThresholdTypes type, int srcRows, int srcCols)
        {
            for (int y = 0; y < srcRows; ++y)
            {
                sbyte* p_s = (sbyte*)src.Ptr(y);
                byte* p_d = (byte*)dest.Ptr(y);
                for (int x = 0; x < srcCols; ++x)
                {
                    sbyte value = *(p_s + x);
                    *(p_d + x) = (byte)Thresholding(value, threshold, maxValue, type);
                }
            }
        }

        unsafe private void ThresholdByte2Byte(Mat src, Mat dest, double threshold, double maxValue, ThresholdTypes type, int srcRows, int srcCols)
        {
            for (int y = 0; y < srcRows; ++y)
            {
                byte* p_s = (byte*)src.Ptr(y);
                byte* p_d = (byte*)dest.Ptr(y);
                for (int x = 0; x < srcCols; ++x)
                {
                    byte value = *(p_s + x);
                    *(p_d + x) = (byte)Thresholding(value, threshold, maxValue, type);
                }
            }
        }

        private double Thresholding(double value, double threshold, double maxValue, ThresholdTypes type)
        {
            switch (type)
            {
                case ThresholdTypes.Binary:
                    return value > threshold ? maxValue : 0;
                case ThresholdTypes.BinaryInv:
                    return value > threshold ? 0 : maxValue;
                case ThresholdTypes.Tozero:
                    return value > threshold ? value : 0;
                case ThresholdTypes.TozeroInv:
                    return value > threshold ? 0 : value;
                case ThresholdTypes.Trunc:
                    return value > threshold ? threshold : value;
                default:
                    throw new NotSupportedException("ThresholdType.Otsu and ThresholdType.Mask isn't supported.");
            }
        }

        private ThresholdTypes ToThresholdType(ComputingMethod Method)
        {
            switch (Method)
            {
                case ComputingMethod.Threshold_Binary:
                    return ThresholdTypes.Binary;
                case ComputingMethod.Threshold_Binary_Inverse:
                    return ThresholdTypes.BinaryInv;
                case ComputingMethod.Threshold_ToZero:
                    return ThresholdTypes.Tozero;
                case ComputingMethod.Threshold_ToZero_Inverse:
                    return ThresholdTypes.TozeroInv;
                case ComputingMethod.Threshold_Trunc:
                    return ThresholdTypes.Trunc;
                default:
                    throw new NotSupportedException("Method is not ThresholdType.");
            }
        }

        public double ThresholdValue
        {
            get { return _Threshold; }
            set { _Threshold = value; }
        }

        public double ThresholdingMaxValue
        {
            get { return _ThresholdingMaxValue; }
            set { _ThresholdingMaxValue = value; }
        }
    }
}
