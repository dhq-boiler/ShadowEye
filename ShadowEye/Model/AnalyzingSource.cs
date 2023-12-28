

using libimgengCore;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using ShadowEye.Utils;
using ShadowEye.View;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media.Imaging;

namespace ShadowEye.Model
{
    public abstract class AnalyzingSource : Notifier, IDisposable, IToggle
    {
        private Uri _location;
        private Mat _mat;
        private WriteableBitmap _bitmap;
        private bool _disposed;
        private string _Name;
        private bool _IsEnable = true;
        private HashSet<AnalyzingSource> _RequestDiscadedMat;

        private AnalyzingSource()
        {
            _RequestDiscadedMat = new HashSet<AnalyzingSource>();
        }

        protected AnalyzingSource(string name) : this()
        {
            Name = name;
        }

        public string Name
        {
            get { return _Name == null ? "" : _Name; }
            set { SetProperty<string>(ref _Name, value, "Name"); }
        }

        public Uri Location
        {
            get { return _location; }
            set
            {
                SetProperty<Uri>(ref _location, value, "Location");
            }
        }

        public Mat Mat
        {
            get
            {
                if (_mat == null || _mat.IsDisposed)
                    return null;
                else
                    return _mat;
            }
            set
            {
                Mat discaded = null;

                if (_mat != null && !_mat.IsDisposed)
                {
                    if (IsRequestedDiscadedMat)
                    {
                        discaded = _mat;
                    }
                    else if (this is FilmSource)
                    {
                        //do not dispose
                    }
                    else
                    {
                        _mat.Dispose();
                        Trace.WriteLine(string.Format("{0}", Name), "MatDisposed");
                    }
                }

                SetProperty<Mat>(ref _mat, value, "Mat");

                if (value != null)
                    OnMatChanged(this, new MatChangedEventArgs(discaded));
            }
        }

        public Updater HowToUpdate { get; set; }

        public void RequestDiscadedMat(AnalyzingSource source)
        {
            _RequestDiscadedMat.Add(source);
        }

        public void TurnDownDiscadedMat(AnalyzingSource source)
        {
            _RequestDiscadedMat.Remove(source);
        }

        private bool IsRequestedDiscadedMat { get { return _RequestDiscadedMat.Count() > 0; } }

        protected void SetBitmapFromMat(Mat mat)
        {
            try
            {
                WriteableBitmapConverter.ToWriteableBitmap(mat, Bitmap);
            }
            catch (ArgumentException)
            {
                Bitmap = WriteableBitmapConverter.ToWriteableBitmap(mat);
            }
        }

        public WriteableBitmap Bitmap
        {
            get { return _bitmap; }
            set
            {
                SetProperty<WriteableBitmap>(ref _bitmap, value, "Bitmap");
                if (value != null)
                {
                    OnBitmapChanged(this, new EventArgs());
                    OnPropertyChanged("BitsPerPixel");
                    OnPropertyChanged("Bits");
                    OnPropertyChanged("Channels");
                }
            }
        }

        public ChannelType ChannelType { get; set; }

        public int BitsPerPixel
        {
            get
            {
                if (Mat != null)
                    return Bits * Channels;
                else
                    return 0;
            }
        }

        public int Bits
        {
            get
            {
                if (Mat != null)
                    return Mat.Depth();
                else
                    return 0;
            }
        }

        public int Channels
        {
            get
            {
                if (Mat != null)
                    return Mat.Channels();
                else
                    return 0;
            }
        }

        public bool IsEnable
        {
            get { return _IsEnable; }
            set { SetProperty<bool>(ref _IsEnable, value, "IsEnable"); }
        }

        public void UpdateDisplay()
        {
            if (Mat.Cols == 0 || Mat.Rows == 0)
                return;
            SetBitmapFromMat(Mat);
            OnSourceUpdated(this, new EventArgs());
        }

        protected bool IsShowingCurrentTab()
        {
            return (App.Current.MainWindow as MainWindow).MainWindowVM.ImageContainerVM.SelectedImageVM != null
                && (App.Current.MainWindow as MainWindow).MainWindowVM.ImageContainerVM.SelectedImageVM.Source.Equals(this);
        }

        public abstract void UpdateImage();

        public abstract bool UpdateOnce { get; }

        public abstract void Activate();
        public abstract void Deactivate();

        public class MatChangedEventArgs : EventArgs, IDisposable
        {
            public Mat DiscadedMat { get; private set; }

            public MatChangedEventArgs(Mat discaded)
                : base()
            {
                DiscadedMat = discaded;
            }

            public void Dispose()
            {
                Dispose(true);

                GC.SuppressFinalize(this);
            }

            private bool _disposed = false;
            private void Dispose(bool disposing)
            {
                if (!_disposed)
                {
                    if (disposing)
                    {
                    }
                    if (DiscadedMat != null)
                    {
                        DiscadedMat.Dispose();
                    }
                    _disposed = true;
                }
            }
        }

        public delegate void MatChangedEventHandler(object sender, MatChangedEventArgs e);
        public event MatChangedEventHandler MatChanged;
        protected virtual void OnMatChanged(object sender, MatChangedEventArgs e)
        {
            if (MatChanged != null)
                MatChanged(this, e);
        }

        public event EventHandler BitmapChanged;
        protected virtual void OnBitmapChanged(object sender, EventArgs e)
        {
            if (BitmapChanged != null)
                BitmapChanged(this, e);
        }

        public event EventHandler SourceUpdated;
        protected virtual void OnSourceUpdated(object sender, EventArgs e)
        {
            if (SourceUpdated != null)
                SourceUpdated(this, e);
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Bitmap = null;
                }

                if (_mat != null)
                {
                    _mat.Dispose();
                    _mat = null;
                }

                _disposed = true;
            }
        }

        ~AnalyzingSource()
        {
            Dispose(false);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
