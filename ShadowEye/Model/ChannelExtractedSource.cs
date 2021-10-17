

using OpenCvSharp;
using System.Diagnostics;

namespace ShadowEye.Model
{
    public class ChannelExtractedSource : ComputingSource
    {
        private int _Channel;

        public ChannelExtractedSource(string name, AnalyzingSource target, int Channel)
            : base(name)
        {
            Debug.Assert(Channel >= 0 && Channel < target.Mat.Type().Channels);

            this.HowToUpdate = target.HowToUpdate.SameUpdater(this);

            LeftHand = target;
            OutputColorType = EColorMode.Grayscale;
            ChannelType = ConvertToChannelType(EColorMode.Grayscale);
            _Channel = Channel;

            LeftHand.MatChanged += LeftHand_MatChanged;
            UpdateImage();
        }

        public override void Compute()
        {
            var split = Cv2.Split(LeftHand.Mat);
            Mat = split[_Channel].Clone();
        }

        public override bool Equals(object obj)
        {
            if (obj is ChannelExtractedSource)
            {
                var ces = obj as ChannelExtractedSource;
                return LeftHand.Equals(ces.LeftHand)
                    && _Channel.Equals(ces._Channel);
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return LeftHand.GetHashCode() ^ _Channel.GetHashCode();
        }
    }
}
