using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using Reactive.Bindings;
using Reactive.Bindings.Disposables;
using Reactive.Bindings.Extensions;
using ShadowEye.Utils;
using ShadowEye.View;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Media.Imaging;

namespace ShadowEye.Model
{
    public abstract class AnalyzingSource : Notifier, IDisposable, IToggle
    {
        private CompositeDisposable _disposables = new();
        private Uri _location;
        private WriteableBitmap _bitmap;
        private bool _disposed;
        private string _Name;
        private bool _IsEnable = true;
        private List<AnalyzingSource> _RequestDiscadedMat;

        private AnalyzingSource()
        {
            _RequestDiscadedMat = new List<AnalyzingSource>();
            Mat.Zip(Mat.Skip(1), (Old, New) => new { OldItem = Old, NewItem = New })
                .Subscribe(pair =>
                {

                    if (_RequestDiscadedMat.Any() && _RequestDiscadedMat.Last().Equals(this))
                    {
                        var discaded = pair.OldItem;

                        lock (discaded)
                        {
                            discaded.Dispose();
                        }
                    }

                    OnMatChanged(this, new MatChangedEventArgs(pair.OldItem));
                }).AddTo(_disposables);
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

        public ReactivePropertySlim<Mat> Mat { get; } = new();

        public Updater HowToUpdate { get; set; }

        public void RequestDiscadedMat(AnalyzingSource source)
        {
            _RequestDiscadedMat.Add(source);
        }

        public void TurnDownDiscadedMat(AnalyzingSource source)
        {
            _RequestDiscadedMat.Remove(source);
        }

        private bool IsRequestedDiscadedMat => _RequestDiscadedMat.Any();

        protected void SetBitmapFromMat(Mat mat)
        {
            if (App.Current is null)
                return;

            App.Current.Dispatcher.Invoke(() =>
            {
                lock (mat)
                {
                    if (mat.IsDisposed)
                        return;

                    try
                    {
                        Bitmap.Value = mat.ToWriteableBitmap();
                    }
                    catch (ArgumentException)
                    {
                        Bitmap.Value = WriteableBitmapConverter.ToWriteableBitmap(mat);
                    }
                }
            });
        }

        public ReactivePropertySlim<WriteableBitmap> Bitmap { get; } = new();

        public libimgengCore.ChannelType ChannelType { get; set; }

        public int BitsPerPixel
        {
            get
            {
                if (Mat.Value != null)
                    return Bits * Channels;
                else
                    return 0;
            }
        }

        public int Bits
        {
            get
            {
                if (Mat.Value != null)
                    return Mat.Value.Depth();
                else
                    return 0;
            }
        }

        public int Channels
        {
            get
            {
                if (Mat.Value != null)
                    return Mat.Value.Channels();
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
            if (Mat.Value.IsDisposed || Mat.Value.Cols == 0 || Mat.Value.Rows == 0)
                return;
            SetBitmapFromMat(Mat.Value);
        }

        protected bool IsShowingCurrentTab()
        {
            return App.Current is not null &&
                   App.Current.Dispatcher.Invoke(() =>
                MainWindow.MainWindowVM.ImageContainerVM.SelectedImageVM != null
                && MainWindow.MainWindowVM.ImageContainerVM.SelectedImageVM.Source
                .Equals(this));
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

            OnSourceUpdated(sender, e);
        }

        public event EventHandler BitmapChanged;
        protected virtual void OnBitmapChanged(object sender, EventArgs e)
        {
            if (BitmapChanged != null)
                BitmapChanged(this, e);

            OnSourceUpdated(sender, e);
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
                }

                _disposables.Dispose();

                Bitmap?.Dispose();

                var mat = Mat.Value;

                if (mat is not null)
                {
                    lock (mat)
                    {
                        mat?.Dispose();
                    }
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
