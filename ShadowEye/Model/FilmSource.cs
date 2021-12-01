using OpenCvSharp;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Windows.Threading;

namespace ShadowEye.Model
{
    public class FilmSource : AnalyzingSource
    {
        private CompositeDisposable disposables = new CompositeDisposable();
        private bool _disposed;
        private DispatcherTimer _timer;
        private DateTime _previousRecordDateTime;
        public ReactiveCollection<Tuple<Mat, TimeSpan>> Frames { get; } = new ReactiveCollection<Tuple<Mat, TimeSpan>>();
        public ReactivePropertySlim<int> CurrentIndex { get; } = new ReactivePropertySlim<int>(0);

        public ReactivePropertySlim<AnalyzingSource> TargetSource { get; } = new ReactivePropertySlim<AnalyzingSource>();

        public ReactiveCommand StopRecordingCommand { get; } = new ReactiveCommand();

        public ReactiveCommand FrameAdvanceCommand { get; } = new ReactiveCommand();

        public ReactiveCommand FrameBackCommand { get; } = new ReactiveCommand();

        public ReactivePropertySlim<int> SelectionStart { get; } = new ReactivePropertySlim<int>();

        public ReactivePropertySlim<int> SelectionEnd { get; } = new ReactivePropertySlim<int>();

        public ReactivePropertySlim<bool> SelectionEnable { get; } = new ReactivePropertySlim<bool>();

        public FilmSource(string name, AnalyzingSource source) : base(name)
        {
            this.HowToUpdate = new ManualUpdater(source);
            TargetSource.Value = source;
            ChannelType = source.ChannelType;
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromTicks(1);
            _timer.Tick += timer_Tick;

            CurrentIndex.Subscribe(_ =>
            {
                if (Frames.Count() - 1 >= CurrentIndex.Value)
                {
                    Mat = Frames[CurrentIndex.Value].Item1;
                    SetBitmapFromMat(Mat);
                    OnSourceUpdated(this, new EventArgs());
                }
            })
            .AddTo(disposables);

            StopRecordingCommand.Subscribe(() =>
            {
                StopRecording();
            })
            .AddTo(disposables);

            FrameAdvanceCommand.Subscribe(() =>
            {
                if (Frames.Count() >= CurrentIndex.Value + 1)
                {
                    CurrentIndex.Value++;
                }
            })
            .AddTo(disposables);

            FrameBackCommand.Subscribe(() =>
            {
                if (CurrentIndex.Value - 1 >= 0)
                {
                    CurrentIndex.Value--;
                }
            })
            .AddTo(disposables);
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            UpdateImage();
        }

        public override bool UpdateOnce => true;

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

        public void StartRecoding()
        {
            Activate();
        }

        public void StopRecording()
        {
            Deactivate();
        }

        public override void UpdateImage()
        {
            try
            {
                TargetSource.Value.UpdateImage();
                Mat = TargetSource.Value.Mat.Clone();
                OnSourceUpdated(this, new EventArgs());
                Frames.Add(new Tuple<Mat, TimeSpan>(Mat, (_previousRecordDateTime != null ? DateTime.Now - _previousRecordDateTime : TimeSpan.Zero)));
                _previousRecordDateTime = DateTime.Now;
                CurrentIndex.Value++;
                if (HowToUpdate is StaticUpdater
                    || IsShowingCurrentTab()
                    || HowToUpdate.InUse)
                {
                    UpdateDisplay();
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

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                base.Dispose(disposing);

                if (disposing)
                {
                    Frames.ToList().ForEach(x => x.Item1.Dispose());
                    Frames.Dispose();
                    CurrentIndex.Dispose();
                    TargetSource.Dispose();
                    StopRecordingCommand.Dispose();
                    FrameAdvanceCommand.Dispose();
                    FrameBackCommand.Dispose();
                }

                _disposed = true;
            }
        }
    }
}
