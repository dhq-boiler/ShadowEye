

using libimgengCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Threading;

namespace ShadowEye.Model
{
    public class CameraSource : AnalyzingSource, IToggle
    {
        private Camera _cam;
        private DispatcherTimer _timer;
        private static Dictionary<int, CameraSource> s_cams = new Dictionary<int, CameraSource>();

        public static CameraSource CreateInstance(int cameraNumber, string cameraName, int width, int height)
        {
            if (!s_cams.Keys.Contains(cameraNumber))
            {
                s_cams.Add(cameraNumber, new CameraSource(cameraNumber, cameraName, width, height));
            }
            return s_cams[cameraNumber];
        }

        private CameraSource(int cameraNumber, string cameraName, int width, int height) : base(cameraName)
        {
            this.HowToUpdate = new DynamicUpdater(this);
            this.CameraNumber = cameraNumber;
            this.ChannelType = libimgengCore.ChannelType.BGR24;
            using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
            {
                _cam = new Camera(cameraNumber, (int)g.DpiX, (int)g.DpiY, width, height);
            }
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromTicks(1);
            _timer.Tick += timer_Tick;
        }

        public override void Activate()
        {
            if (_timer != null)
            {
                if (!_cam.IsOpen)
                    _cam.Reopen();
                if (!_timer.IsEnabled)
                    _timer.Start();
            }
        }

        public override void Deactivate()
        {
            if (_timer != null)
            {
                if (_timer.IsEnabled)
                    _timer.Stop();
                _cam.Close(true);
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            UpdateImage();
        }

        public override void UpdateImage()
        {
            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                Mat.Value = _cam.NextFrame();
                if (IsShowingCurrentTab() || HowToUpdate.InUse)
                {
                    UpdateDisplay();
                }
                sw.Stop();
                Fps = 1.0 / (sw.ElapsedMilliseconds / 1000.0);
            }
            catch (COMException e)
            {
                Trace.WriteLine(e);
            }
            catch (InvalidOperationException e)
            {
                MessageBox.Show(e.ToString(), e.Message, MessageBoxButton.OK);
                IsEnable = false;
                _timer.Stop();
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is CameraSource)) return false;
            return CameraNumber == (obj as CameraSource).CameraNumber;
        }

        public override int GetHashCode()
        {
            return CameraNumber.GetHashCode();
        }

        private bool _disposed = false;
        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                }
                s_cams.Remove(_cam.OpenedCameraNumber);
                _cam.Dispose();
                if (_timer != null && _timer.IsEnabled)
                {
                    _timer.Stop();
                    _timer = null;
                }
                _disposed = true;
            }
            base.Dispose(disposing);
        }

        public int CameraNumber { get; private set; }

        public override bool UpdateOnce
        {
            get { return false; }
        }

        private double _Fps;
        public double Fps
        {
            get { return _Fps; }
            private set { SetProperty<double>(ref _Fps, value, "Fps"); }
        }

        //private int _InUseCount;
        //private int InUseCount
        //{
        //    get { return _InUseCount; }
        //    set { if (value >= 0) _InUseCount = value; }
        //}
    }
}
