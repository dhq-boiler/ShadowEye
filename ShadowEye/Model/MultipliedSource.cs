

using OpenCvSharp;
using System.Diagnostics;

namespace ShadowEye.Model
{
    public class MultipliedSource : ComputingSource
    {
        private double _ScaleFactor;

        public MultipliedSource(string name, AnalyzingSource left, AnalyzingSource right, double scaleFactor, EColorMode colorMode)
            : base(name)
        {
            Debug.Assert(left.Mat.Value.Width == right.Mat.Value.Width);
            Debug.Assert(left.Mat.Value.Height == right.Mat.Value.Height);
            Debug.Assert(left.Mat.Value.Type().Channels == right.Mat.Value.Type().Channels);
            Debug.Assert(left.Mat.Value.Type().Depth == right.Mat.Value.Type().Depth);

            this.HowToUpdate = HaveAnyDynamicUpdater(left, right);

            LeftHand = left;
            RightHand = right;
            OutputColorType = colorMode;
            ChannelType = ConvertToChannelType(colorMode);
            _ScaleFactor = scaleFactor;
            Mat.Value = new Mat();

            LeftHand.MatChanged += LeftHand_MatChanged;
            RightHand.MatChanged += RightHand_MatChanged;
            UpdateImage();
        }

        private Updater HaveAnyDynamicUpdater(AnalyzingSource left, AnalyzingSource right)
        {
            if (left.HowToUpdate is DynamicUpdater || right.HowToUpdate is DynamicUpdater)
                return new DynamicUpdater(this);
            else
                return new StaticUpdater(this);
        }

        public override void Compute()
        {
            using (Mat newMat = new Mat())
            {
                Cv2.Multiply(LeftHand.Mat.Value, RightHand.Mat.Value, newMat, _ScaleFactor);
                Mat.Value = newMat.Clone();
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is MultipliedSource)
            {
                var ms = obj as MultipliedSource;
                return LeftHand.Equals(ms.LeftHand)
                    && RightHand.Equals(ms.RightHand)
                    && _ScaleFactor.Equals(ms._ScaleFactor);
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return LeftHand.GetHashCode()
                ^ RightHand.GetHashCode()
                ^ _ScaleFactor.GetHashCode();
        }
    }
}
