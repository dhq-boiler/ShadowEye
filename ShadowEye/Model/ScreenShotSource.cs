using OpenCvSharp.Extensions;
using OpenCvSharp.WpfExtensions;
using Reactive.Bindings;
using Reactive.Bindings.Disposables;
using Reactive.Bindings.Extensions;
using ShadowEye.Utils;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reactive.Linq;

namespace ShadowEye.Model
{
    public class ScreenShotSource : AnalyzingSource
    {
        private CompositeDisposable _disposables = new();
        private ScreenShotArea _Area;

        public ReactivePropertySlim<bool> IsRunning { get; } = new(false);

        public ScreenShotSource(string name)
            : base(name)
        {
            HowToUpdate = new StaticUpdater(this);
            ChannelType = libimgengCore.ChannelType.BGR24;
        }

        public ScreenShotArea Area
        {
            get { return _Area; }
            set { SetProperty<ScreenShotArea>(ref _Area, value, "Area"); }
        }

        public override void UpdateImage()
        {
            if (!Area.IsReady) return;
            try
            {
                using (var b = Area.GetScreenShot())
                {
                    Mat.Value = b.ToMat();
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
            IsRunning.Value = true;
            Observable.Interval(TimeSpan.FromSeconds(1.0 / 60.0))
                .Subscribe(_ =>
                {
                    if (IsRunning.Value) UpdateImage();
                })
                .AddTo(_disposables);
        }

        public override void Deactivate()
        {
            IsRunning.Value = false;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            _disposables.Dispose();
        }
    }
}
