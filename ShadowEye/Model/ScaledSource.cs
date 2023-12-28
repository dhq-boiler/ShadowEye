

using OpenCvSharp;

namespace ShadowEye.Model
{
    internal class ScaledSource : ComputingSource
    {
        public ScaledSource(string name, AnalyzingSource target, int width, int height)
            : base(name)
        {
            this.HowToUpdate = target.HowToUpdate.SameUpdater(this);
            LeftHand = target;
            Width = width;
            Height = height;
            OutputColorType = EColorMode.Original;

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
            using (Mat newMat = new Mat(Height, Width, LeftHand.Mat.Value.Type()))
            {
                Cv2.Resize(LeftHand.Mat.Value, newMat, new Size(Width, Height));
                Mat.Value = newMat.Clone();
            }
        }

        public int Height { get; set; }

        public int Width { get; set; }
    }
}
