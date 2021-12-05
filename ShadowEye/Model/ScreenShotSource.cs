

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using OpenCvSharp.Extensions;
using ShadowEye.Utils;

namespace ShadowEye.Model
{
    public class ScreenShotSource : AnalyzingSource
    {
        private DispatcherTimer _timer;
        private ScreenShotArea _Area;

        public ScreenShotSource(string name)
            : base(name)
        {
            HowToUpdate = new StaticUpdater(this);
            ChannelType = libimgengCore.ChannelType.BGR24;
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1.0 / 30.0); //30fps
            _timer.Tick += timer_Tick;
        }

        public ScreenShotArea Area
        {
            get { return _Area; }
            set { SetProperty<ScreenShotArea>(ref _Area, value, "Area"); }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            UpdateImage();
        }

        public override void UpdateImage()
        {
            if (!Area.IsReady) return;
            try
            {
                using (var b = Area.GetScreenShot())
                {
                    Mat = b.ToMat();
                    if (HowToUpdate is StaticUpdater
                        || IsShowingCurrentTab()
                        || HowToUpdate.InUse)
                    {
                        UpdateDisplay();
                    }
                }
            }
            catch (Win32Exception e)
            {
                Trace.WriteLine(e);
            }
            catch (ArgumentException e)
            {
                Trace.WriteLine(e);
            }
        }

        public override bool UpdateOnce
        {
            get { return HowToUpdate is StaticUpdater; }
        }

        public override void Activate()
        {
            if (_timer != null)
            {
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
            }
        }
    }
}
