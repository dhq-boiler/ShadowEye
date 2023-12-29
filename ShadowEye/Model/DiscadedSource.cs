

using OpenCvSharp;
using System;
using System.Diagnostics;

namespace ShadowEye.Model
{
    public class DiscadedSource : AnalyzingSource, IToggle
    {
        public DiscadedSource(AnalyzingSource monitoring) : base(">" + monitoring.Name)
        {
            this.HowToUpdate = monitoring.HowToUpdate.SameUpdater(this);
            Parent = monitoring;
            ChannelType = monitoring.ChannelType;
            Generation = monitoring is DiscadedSource ? (monitoring as DiscadedSource).Generation + 1 : 2;
            monitoring.RequestDiscadedMat(this);
            monitoring.MatChanged += MatChangedAction;
        }

        private void MatChangedAction(object sender, MatChangedEventArgs e)
        {
            if (e.DiscadedMat == null)
            {
                Trace.WriteLine(string.Format("{0} can't receive Mat from {1}", this.Name, Parent.Name));
                return;
            }
            Store(e.DiscadedMat);
        }

        public void Store(Mat storing)
        {
            Debug.Assert(storing != null && !storing.IsDisposed);
            this.Mat.Value = storing;
            UpdateImage();
        }

        public override void UpdateImage()
        {
            try
            {
                if (IsShowingCurrentTab() || HowToUpdate.InUse)
                {
                    SetBitmapFromMat(this.Mat.Value);
                    OnSourceUpdated(this, new EventArgs());
                }
            }
            catch (InvalidOperationException)
            { }
        }

        public AnalyzingSource Parent { get; private set; }
        public int Generation { get; private set; }

        public override bool UpdateOnce
        {
            get { return false; }
        }

        private bool _disposed;
        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (Parent != null)
                    {
                        Parent.TurnDownDiscadedMat(this);
                        Parent.MatChanged -= MatChangedAction;
                        Parent = null;
                    }
                }

                _disposed = true;
            }
            base.Dispose(disposing);
        }

        public override void Activate()
        {
            if (Parent != null)
            {
                Parent.HowToUpdate.Request();
            }
        }

        public override void Deactivate()
        {
            if (Parent != null)
            {
                Parent.HowToUpdate.RequestAccomplished();
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is DiscadedSource))
                return false;
            var ds = (obj as DiscadedSource);
            return Name.Equals(ds.Name);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
