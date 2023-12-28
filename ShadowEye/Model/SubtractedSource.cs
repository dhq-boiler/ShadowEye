

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
            Debug.Assert(left.Mat.Value.Width == right.Mat.Value.Width);
            Debug.Assert(left.Mat.Value.Height == right.Mat.Value.Height);
            Debug.Assert(left.Mat.Value.Type().Channels == right.Mat.Value.Type().Channels);
            Debug.Assert(left.Mat.Value.Type().Depth == right.Mat.Value.Type().Depth);

            this.HowToUpdate = HaveAnyDynamicUpdater(left, right);

            LeftHand = left;
            RightHand = right;
            Method = method;
            OutputColorType = colorMode;
            ChannelType = ConvertToChannelType(colorMode);
            Mat.Value = new Mat();

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
                        Cv2.Subtract(LeftHand.Mat.Value, RightHand.Mat.Value, newMat);
                        break;
                    case ComputingMethod.Subtract_Absolute:
                        Cv2.Absdiff(LeftHand.Mat.Value, RightHand.Mat.Value, newMat);
                        break;
                    default:
                        throw new InvalidOperationException("Unknown computing method.");
                }
                Mat.Value = newMat.Clone();
            }
        }
    }
}
