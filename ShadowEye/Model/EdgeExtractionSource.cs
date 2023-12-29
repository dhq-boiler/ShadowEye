

using OpenCvSharp;
using System;
using System.Windows;

namespace ShadowEye.Model
{
    public class EdgeExtractionSource : ComputingSource
    {
        private MatType _ddepth;
        private int _xorder;
        private int _yorder;
        private int _ksize;
        private double _scale;
        private double _delta;
        private BorderTypes _borderType;

        public EdgeExtractionSource(string name, AnalyzingSource target, ComputingMethod edgeExtractMethod, MatType ddepth, int xorder, int yorder, int ksize, double scale, double delta, BorderTypes borderType)
            : base(name)
        {
            HowToUpdate = target.HowToUpdate.SameUpdater(this);
            LeftHand = target;
            Method = edgeExtractMethod;
            OutputColorType = EColorMode.BGR;
            ChannelType = ConvertToChannelType(EColorMode.BGR);

            _ddepth = ddepth;
            _xorder = xorder;
            _yorder = yorder;
            _ksize = ksize;
            _scale = scale;
            _delta = delta;
            _borderType = borderType;

            LeftHand.MatChanged += LeftHand_MatChanged;
            try
            {
                UpdateImage();
            }
            catch (OpenCVException)
            {
                throw;
            }
        }

        public override void Compute()
        {
            try
            {
                using (Mat newMat = new Mat(LeftHand.Mat.Value.Rows, LeftHand.Mat.Value.Cols, _ddepth))
                {
                    switch (Method)
                    {
                        case ComputingMethod.EdgeExtraction_Sobel:
                            Cv2.Sobel(LeftHand.Mat.Value, newMat, _ddepth, _xorder, _yorder, _ksize, _scale, _delta, _borderType);
                            break;
                        case ComputingMethod.EdgeExtraction_Canny:
                            break;
                        case ComputingMethod.EdgeExtraction_Laplacian:
                            break;
                        default:
                            throw new InvalidOperationException("Unknown computing method.");
                    }
                    Mat.Value = newMat.Clone();
                }
            }
            catch (OpenCVException e)
            {
                MessageBox.Show(e.Message);
                throw;
            }
        }

        public static EdgeExtractionSource CreateInstanceSobel(string name, AnalyzingSource source, MatType ddepth, int xorder, int yorder, int ksize, double scale, double delta, BorderTypes borderType)
        {
            try
            {
                return new EdgeExtractionSource(name, source, ComputingMethod.EdgeExtraction_Sobel, ddepth, xorder, yorder, ksize, scale, delta, borderType);
            }
            catch (OpenCVException)
            {
                throw;
            }
        }
    }
}
