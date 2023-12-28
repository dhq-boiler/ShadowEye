

using OpenCvSharp;
using System;

namespace ShadowEye.Model
{
    public abstract class ComputingSource : AnalyzingSource, IToggle
    {
        private AnalyzingSource _LeftHand;
        private AnalyzingSource _RightHand;
        private ComputingMethod _Method;
        private EColorMode _OutputColorType;

        public AnalyzingSource LeftHand
        {
            get { return _LeftHand; }
            set { _LeftHand = value; }
        }

        public AnalyzingSource RightHand
        {
            get { return _RightHand; }
            set { _RightHand = value; }
        }

        public ComputingMethod Method
        {
            get { return _Method; }
            set { _Method = value; }
        }

        public EColorMode OutputColorType
        {
            get { return _OutputColorType; }
            set { _OutputColorType = value; }
        }

        private int _leftChangeCount;
        private int _rightChangeCount;

        protected ComputingSource(string name) : base(name)
        { }

        protected void RightHand_MatChanged(object sender, AnalyzingSource.MatChangedEventArgs e)
        {
            if (IsEnable)
            {
                ++_rightChangeCount;
                WhenBothLeftAndRightIsChanged();
            }
        }

        private void WhenBothLeftAndRightIsChanged()
        {
            if (_leftChangeCount > 0 && _rightChangeCount > 0)
            {
                UpdateImage();
                _leftChangeCount = _rightChangeCount = 0;
            }
        }

        protected void LeftHand_MatChanged(object sender, AnalyzingSource.MatChangedEventArgs e)
        {
            if (IsEnable)
            {
                ++_leftChangeCount;
                WhenBothLeftAndRightIsChanged();
            }
        }

        protected libimgengCore.ChannelType ConvertToChannelType(EColorMode OutColorType)
        {
            switch (OutColorType)
            {
                case EColorMode.BGR:
                    return libimgengCore.ChannelType.BGR24;
                case EColorMode.Grayscale:
                    return libimgengCore.ChannelType.Gray;
                case EColorMode.RGB:
                    return libimgengCore.ChannelType.RGB;
                default:
                    throw new NotSupportedException(OutColorType.ToString());
            }
        }

        public abstract void Compute();

        public override void UpdateImage()
        {
            try
            {
                try
                {
                    Compute();

                    if (Mat.Value.Type().Channels == 3)
                    {
                        switch (OutputColorType)
                        {
                            case EColorMode.Unknown:
                                throw new InvalidOperationException("OutputColorType didn't set.");
                            case EColorMode.Original:
                                this.ChannelType = LeftHand.ChannelType;
                                break;
                            case EColorMode.BGR:
                                this.ChannelType = libimgengCore.ChannelType.BGR24;
                                break;
                            case EColorMode.RGB:
                                Cv2.CvtColor(Mat.Value, Mat.Value, ColorConversionCodes.BGR2RGB);
                                this.ChannelType = libimgengCore.ChannelType.RGB;
                                break;
                            case EColorMode.Grayscale:
                                Cv2.CvtColor(Mat.Value, Mat.Value, ColorConversionCodes.BGR2GRAY);
                                this.ChannelType = libimgengCore.ChannelType.Gray;
                                break;
                            default:
                                throw new InvalidOperationException("Unknown computing OutputColorType.");
                        }
                    }
                    else
                    {
                        this.ChannelType = libimgengCore.ChannelType.Gray;
                    }
                }
                finally
                {
                    if (Mat.Value != null && (!AnyDynamicSource() || IsShowingCurrentTab()))
                    {
                        SetBitmapFromMat(Mat.Value);
                        OnSourceUpdated(this, new EventArgs());
                    }
                }
            }
            catch (ArgumentNullException)
            {
                SafetyStop();
            }
            catch (OpenCVException)
            {
                SafetyStop();
                throw;
            }
        }

        private bool AnyDynamicSource()
        {
            return (LeftHand != null && LeftHand.HowToUpdate != null && LeftHand.HowToUpdate is DynamicUpdater)
                || (RightHand != null && RightHand.HowToUpdate != null && RightHand.HowToUpdate is DynamicUpdater);
        }

        protected void SafetyStop()
        {
            IsEnable = false;
            if (LeftHand != null)
            {
                LeftHand.TurnDownDiscadedMat(this);
                LeftHand.MatChanged -= LeftHand_MatChanged;
            }
            if (RightHand != null)
                RightHand.MatChanged -= RightHand_MatChanged;
            LeftHand = null;
            RightHand = null;
        }

        public override bool UpdateOnce
        {
            get { return false; }
        }

        public override void Activate()
        {
            if (LeftHand != null)
            {
                LeftHand.HowToUpdate.Request();
            }

            if (RightHand != null)
            {
                RightHand.HowToUpdate.Request();
            }
        }

        public override void Deactivate()
        {
            if (LeftHand != null)
            {
                LeftHand.HowToUpdate.RequestAccomplished();
            }

            if (RightHand != null)
            {
                RightHand.HowToUpdate.RequestAccomplished();
            }
        }

        private bool _disposed = false;
        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    //Manage objects release
                    if (LeftHand is IToggle)
                    {
                        (LeftHand as IToggle).Deactivate();
                    }

                    if (RightHand is IToggle)
                    {
                        (RightHand as IToggle).Deactivate();
                    }

                    SafetyStop();
                }

                _disposed = true;
                base.Dispose(disposing);
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ComputingSource)) return false;
            var o = obj as ComputingSource;
            return this.LeftHand.Equals(o.LeftHand)
                && (this.RightHand != null ? this.RightHand.Equals(o.RightHand) : true)
                && this.Method.Equals(o.Method)
                && this.OutputColorType.Equals(o.OutputColorType);
        }

        public override int GetHashCode()
        {
            return (this.LeftHand != null ? this.LeftHand.GetHashCode() : 0x7FFFFFFF)
                ^ (this.RightHand != null ? this.RightHand.GetHashCode() : 0x7FFFFFFF)
                ^ this.Method.GetHashCode()
                ^ this.OutputColorType.GetHashCode();
        }
    }

    public enum ComputingMethod
    {
        Unknown,
        Subtract,
        Subtract_Absolute,
        IntegrateChannel,
        IntegrateChannel_Normalize,
        Threshold_Binary,
        Threshold_Binary_Inverse,
        Threshold_Trunc,
        Threshold_ToZero,
        Threshold_ToZero_Inverse,
        EdgeExtraction_Sobel,
        EdgeExtraction_Canny,
        EdgeExtraction_Laplacian
    }
}
