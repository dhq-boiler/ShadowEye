

using OpenCvSharp;
using System.Diagnostics;

namespace ShadowEye.Model
{
    public class ColorConvertedSource : ComputingSource
    {
        private ColorConversionCodes _ColorConversion;

        public ColorConvertedSource(string name, AnalyzingSource target, ColorConversionCodes conversion)
            : base(name)
        {
            Debug.Assert(target != null);
            Debug.Assert(target.Mat != null);
            Debug.Assert(target.Mat.Rows != 0 && target.Mat.Cols != 0);

            this.HowToUpdate = target.HowToUpdate.SameUpdater(target);

            LeftHand = target;
            OutputColorType = EColorMode.BGR;
            ChannelType = ConvertToChannelType(EColorMode.BGR);
            _ColorConversion = conversion;

            LeftHand.MatChanged += LeftHand_MatChanged;
            UpdateImage();
        }

        public override void Compute()
        {
            using (Mat newMat = new Mat())
            {
                Cv2.CvtColor(LeftHand.Mat, newMat, _ColorConversion);
                Mat = newMat.Clone();
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is ColorConvertedSource)
            {
                var ccs = obj as ColorConvertedSource;
                return LeftHand.Equals(ccs.LeftHand) && _ColorConversion.Equals(ccs._ColorConversion);
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return LeftHand.GetHashCode() ^ _ColorConversion.GetHashCode();
        }
    }
}
