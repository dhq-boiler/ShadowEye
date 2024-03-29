

using OpenCvSharp;
using System;
using System.Windows.Media.Imaging;

namespace libimgengCore
{
    public class Camera : IDisposable
    {
        private bool _disposed;
        private VideoCapture _vc;
        public int OpenedCameraNumber { get; private set; }
        public int DpiX { get; set; }
        public int DpiY { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public double FPS { get { return _vc != null ? _vc.Get(VideoCaptureProperties.Fps) : 0.0; } }
        public bool IsOpen { get { return _vc != null && _vc.IsOpened(); } }

        public Camera()
        {
            Initialize(false);
        }

        public Camera(int cameraNumber, int width, int height) : this()
        {
            this.Width = width;
            this.Height = height;
            Open(cameraNumber);
        }

        public Camera(int cameraNumber, int dpiX, int dpiY, int width, int height) : this(cameraNumber, width, height)
        {
            this.DpiX = dpiX;
            this.DpiY = dpiY;
        }

        public void Open(int cameraNumber)
        {
            _vc.Open(cameraNumber);
            _vc.Set(OpenCvSharp.VideoCaptureProperties.FrameWidth, this.Width);
            _vc.Set(OpenCvSharp.VideoCaptureProperties.FrameHeight, this.Height);
            this.OpenedCameraNumber = cameraNumber;
        }

        public void Reopen()
        {
            Initialize(saveCameraNumber: true);
            _vc.Open(OpenedCameraNumber);
            _vc.Set(OpenCvSharp.VideoCaptureProperties.FrameWidth, this.Width);
            _vc.Set(OpenCvSharp.VideoCaptureProperties.FrameHeight, this.Height);
        }

        public void Close(bool saveCameraNumber)
        {
            if (_vc != null && !_vc.IsDisposed)
            {
                _vc.Dispose();
                _vc = null;
            }
            if (!saveCameraNumber)
                OpenedCameraNumber = -1;
        }

        public void Initialize(bool saveCameraNumber)
        {
            Close(saveCameraNumber);
            _vc = new VideoCapture();
        }

        public Mat NextFrame()
        {
            if (!IsOpen) throw new InvalidOperationException("Camera isn't Open.");
            Mat mat = new Mat();
            //_vc.Retrieve(mat, 0);
            _vc.Read(mat);
            return mat;
        }

        public WriteableBitmap NextFrameAsWriteableBitmap()
        {
            using (Mat mat = NextFrame())
            {
                return mat.ToWriteableBitmap();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Close(false);
                }

                _disposed = true;
            }
        }
    }
}
