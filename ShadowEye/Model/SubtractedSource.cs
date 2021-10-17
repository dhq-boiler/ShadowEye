

using OpenCvSharp;
using System;
using System.Diagnostics;

namespace ShadowEye.Model
{
    public class SubtractedSource : ComputingSource
    {
        public SubtractedSource(string name, AnalyzingSource left, AnalyzingSource right, ComputingMethod method, EColorMode colorMode)
            : base(name)
        {
            Debug.Assert(left.Mat.Width == right.Mat.Width);
            Debug.Assert(left.Mat.Height == right.Mat.Height);
            Debug.Assert(left.Mat.Type().Channels == right.Mat.Type().Channels);
            Debug.Assert(left.Mat.Type().Depth == right.Mat.Type().Depth);

            this.HowToUpdate = HaveAnyDynamicUpdater(left, right);

            LeftHand = left;
            RightHand = right;
            Method = method;
            OutputColorType = colorMode;
            ChannelType = ConvertToChannelType(colorMode);
            Mat = new Mat();

            LeftHand.MatChanged += LeftHand_MatChanged;
            RightHand.MatChanged += RightHand_MatChanged;
            UpdateImage();
        }

        private Updater HaveAnyDynamicUpdater(AnalyzingSource left, AnalyzingSource right)
        {
            if ((left.HowToUpdate is DynamicUpdater || right.HowToUpdate is DynamicUpdater))
                return new DynamicUpdater(this);
            else
                return new StaticUpdater(this);
        }

        public override void Compute()
        {
            using (Mat newMat = new Mat())
            {
                switch (Method)
                {
                    case ComputingMethod.Subtract:
                        Cv2.Subtract(LeftHand.Mat, RightHand.Mat, newMat);
                        break;
                    case ComputingMethod.Subtract_Absolute:
                        Cv2.Absdiff(LeftHand.Mat, RightHand.Mat, newMat);
                        break;
                    default:
                        throw new InvalidOperationException("Unknown computing method.");
                }
                Mat = newMat.Clone();
            }
        }
    }
}
