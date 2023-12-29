

using Reactive.Bindings;
using Reactive.Bindings.Disposables;
using Reactive.Bindings.Extensions;
using ShadowEye.Model;
using ShadowEye.Utils;
using ShadowEye.View.Dialogs;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Collections.ObjectModel;

namespace ShadowEye.ViewModel
{
    public class ScreenShotDialogViewModel : Notifier, IDisposable
    {
        private static ScreenShotTarget _DefaultTarget;
        private static bool _DefaultOnlyClientArea;
        private static ScreenShotDialogViewModel.PictureOrMovie _DefaultCapturingType;
        private static ProcessItem _DefaultSelectedProcess;
        private static ProcessItem.WindowInfo _DefaultSelectedWindowInfo;
        private static Screen _DefaultSelectedScreen;
        private static int s_createCount;

        private ScreenShotArea _ScreenShotSourceArea;
        private CompositeDisposable _disposables = new();

        public ScreenShotDialog Dialog { get; set; }
        public ReactivePropertySlim<MainWorkbenchViewModel> MainWorkbenchVM { get; } = new();
        public ReactivePropertySlim<ScreenShotSource> Source { get; } = new();
        public ReactivePropertySlim<ScreenShotTarget> Target { get; } = new();
        public ReactivePropertySlim<bool> OnlyClientArea { get; } = new();
        public ReactivePropertySlim<PictureOrMovie> CapturingType { get; } = new();
        public ScreenOption[] Monitors => ScreenOption.CreateArray();
        public ReactivePropertySlim<Screen> SelectedScreen { get; } = new();
        public ReactiveCollection<ProcessItem> Processes { get; } = [];
        public ReactivePropertySlim<ProcessItem> SelectedProcess { get; } = new();
        public ReactiveCollection<ProcessItem.WindowInfo> WindowInfos { get; } = [];
        public ReadOnlyReactivePropertySlim<bool> ProcessIsSelected => Target.CombineLatest(SelectedProcess, (t, sp) => t == ScreenShotTarget.Window && sp is not null).ToReadOnlyReactivePropertySlim();
        public ReactivePropertySlim<ProcessItem.WindowInfo> SelectedWindowInfo { get; } = new();
        public ReactiveCommandSlim<SelectionChangedEventArgs> SelectScreenCommand { get; } = new();
        public ReactiveCommandSlim<SelectionChangedEventArgs> SelectProcessCommand { get; } = new();
        public ReactiveCommandSlim<SelectionChangedEventArgs> SelectWindowCommand { get; } = new();
        public ReactiveCommandSlim ShootCommand { get; } = new();
        public ReactiveCommandSlim CancelCommand { get; } = new();
        public ReactiveCommandSlim SaveFileCommand { get; } = new();
        public ReactiveCommandSlim LoadedCommand { get; } = new();
        public ReactiveCommandSlim ClosedCommand { get; } = new();
        public ReactiveCommandSlim UpdateProcessesCommand { get; } = new();

        public ScreenShotDialogViewModel(MainWorkbenchViewModel imageContainerVM)
        {
            LoadedCommand.Subscribe(() =>
            {
                Initialize();
                Source.Value.HowToUpdate.Request();
            }).AddTo(_disposables);
            ClosedCommand.Subscribe(() =>
            {
                Source.Value.HowToUpdate.RequestAccomplished();
            }).AddTo(_disposables);
            ShootCommand.Subscribe(() =>
            {
                AddComputeTab();
                Dialog.DialogResult = true;
            }).AddTo(_disposables);
            CancelCommand.Subscribe(() =>
            {
                Dialog.DialogResult = false;
            }).AddTo(_disposables);
            UpdateProcessesCommand.Subscribe(o =>
            {
                UpdateProcesses();
            }).AddTo(_disposables);
            SelectScreenCommand.Subscribe(e =>
            {
                if (Source.Value.Area is ScreenShotScreen)
                {
                    SelectedScreen.Value = (e.AddedItems.Cast<Screen>().Except(e.RemovedItems.Cast<Screen>()) as ScreenOption)?.Target;
                    (Source.Value.Area as ScreenShotScreen).SelectedScreen = SelectedScreen.Value;
                    if (Source.Value.HowToUpdate is StaticUpdater)
                    {
                        Source.Value.UpdateImage();
                    }
                }
            }).AddTo(_disposables);
            SelectProcessCommand.Subscribe(e =>
            {
                Source.Value.Area = new ScreenShotWindow();
                
                if (Source.Value.Area is ScreenShotWindow area)
                {
                    area.SelectedProcess = SelectedProcess.Value?.Process;
                    area.OnlyClientArea = OnlyClientArea.Value;
                }

                if (Source.Value.HowToUpdate is StaticUpdater)
                {
                    Source.Value.UpdateImage();
                }

                UpdateWindowInfos();

                if (Source.Value.Area is ScreenShotWindow)
                {
                    SelectWindowHandle();
                }
            }).AddTo(_disposables);
            SelectWindowCommand.Subscribe(e =>
            {
                if (Source.Value.Area is ScreenShotWindow window && SelectedWindowInfo.Value is not null)
                {
                    window.SelectedProcess = SelectedProcess.Value?.Process;
                    window.OnlyClientArea = OnlyClientArea.Value;
                    window.SelectedWindowHandle = SelectedWindowInfo.Value.WindowHandle;
                    if (Source.Value.HowToUpdate is StaticUpdater)
                    {
                        Source.Value.UpdateImage();
                    }
                }
            }).AddTo(_disposables);
            Target.Subscribe(target =>
            {
                if (Source.Value is null)
                    return;
                SwitchTarget();
                Source.Value.UpdateImage();
            }).AddTo(_disposables);
            OnlyClientArea.Subscribe(onlyClientArea =>
            {
                if (Source.Value is null || Source.Value.Area is not ScreenShotWindow sss) return;
                sss.OnlyClientArea = OnlyClientArea.Value;
                Source.Value.UpdateImage();
            }).AddTo(_disposables);
            CapturingType.Subscribe(capturingType =>
            {
                if (Source.Value is null) return;
                SwitchCapturingType();
                Source.Value.UpdateImage();
            }).AddTo(_disposables);

            MainWorkbenchVM.Value = imageContainerVM;
            Source.Value = new ScreenShotSource("ScreenShot");
            Initialize(); 
            Source.Value.UpdateImage();
        }

        public void Initialize()
        {
            Target.Value = _DefaultTarget != ScreenShotTarget.Unknown ? _DefaultTarget : ScreenShotTarget.VirtualScreen; //初期状態はVirtualScreen選択
            CapturingType.Value = _DefaultCapturingType != PictureOrMovie.Unknown ? _DefaultCapturingType : PictureOrMovie.Picture; //初期状態はPicture選択
            if (_DefaultSelectedScreen is not null)
            {
                SelectedScreen.Value = _DefaultSelectedScreen;
            }
            if (_DefaultSelectedProcess is not null)
            {
                SelectedProcess.Value = _DefaultSelectedProcess;
            }

            if (_DefaultSelectedWindowInfo is not null)
            {
                SelectedWindowInfo.Value = _DefaultSelectedWindowInfo;
            }

            Source.Value.HowToUpdate.RequestAccomplished();

            if (_ScreenShotSourceArea is not null)
            {
                Source.Value.Area = _ScreenShotSourceArea;
            }
            else
            {
                switch (Target.Value)
                {
                    case ScreenShotTarget.VirtualScreen:
                        Source.Value.Area = new ScreenShotVirtualScreen();
                        break;
                    case ScreenShotTarget.Screen:
                        Source.Value.Area = new ScreenShotScreen();
                        break;
                    case ScreenShotTarget.Desktop:
                        Source.Value.Area = new ScreenShotDesktop();
                        break;
                    case ScreenShotTarget.Window:
                        Source.Value.Area = new ScreenShotWindow();
                        break;
                }
            }

            Source.Value.HowToUpdate.Request();
        }

        private void SwitchCapturingType()
        {
            if (Source.Value is null)
                return;

            Source.Value.HowToUpdate.ForceStop();

            switch (CapturingType.Value)
            {
                case PictureOrMovie.Picture:
                    Source.Value.HowToUpdate = new StaticUpdater(Source.Value);
                    break;
                case PictureOrMovie.Movie:
                    Source.Value.HowToUpdate = new DynamicUpdater(Source.Value);
                    break;
                case PictureOrMovie.Film:
                    Source.Value.HowToUpdate = new ManualUpdater(Source.Value);
                    break;
            }

            Source.Value.HowToUpdate.Request();
        }

        private void SwitchTarget()
        {
            if (Source.Value is null)
                return;

            Source.Value.HowToUpdate.RequestAccomplished();

            switch (Target.Value)
            {
                case ScreenShotTarget.VirtualScreen:
                    Source.Value.Area = new ScreenShotVirtualScreen();
                    break;
                case ScreenShotTarget.Screen:
                    Source.Value.Area = new ScreenShotScreen(SelectedScreen.Value);
                    break;
                case ScreenShotTarget.Desktop:
                    Source.Value.Area = new ScreenShotDesktop();
                    break;
                case ScreenShotTarget.Window:
                    Source.Value.Area = new ScreenShotWindow(SelectedProcess.Value?.Process);
                    break;
            }

            Source.Value.HowToUpdate.Request();
        }

        public void UpdateProcesses()
        {
            Processes.Clear();
            foreach (var p in Process.GetProcesses())
            {
                if (!p.MainWindowTitle.Any()) continue;
                Processes.Add(new ProcessItem(p));
            }
        }

        public void UpdateWindowInfos()
        {
            if (SelectedProcess.Value is null)
                return;
            WindowInfos.Clear();
            var wis = ScreenShotWindow.EnumWindows(SelectedProcess.Value.Process);
            WindowInfos.AddRange(wis);
        }

        public void SelectWindowHandle()
        {
            if (SelectedProcess == null)
                return;
            SelectedWindowInfo.Value = WindowInfos.SingleOrDefault(a => a.WindowHandle.Equals(SelectedProcess.Value.Process.MainWindowHandle));
        }

        public void AddComputeTab()
        {
            var source = (AnalyzingSource)new DummySource();
            source = Source.Value;
            source.Name = $"ScreenShot-{++s_createCount}";
            if (CapturingType.Value == PictureOrMovie.Film)
            {
                source = new FilmSource($"Film-{++s_createCount}", source);
                ((FilmSource)source).StartRecoding();
            }
            MainWorkbenchVM.Value.AddOrActive(source);

            _DefaultTarget = Target.Value;
            _DefaultCapturingType = CapturingType.Value;
            _DefaultSelectedScreen = SelectedScreen.Value;
            _DefaultSelectedProcess = SelectedProcess.Value;
            _DefaultSelectedWindowInfo = SelectedWindowInfo.Value;
            _ScreenShotSourceArea = Source.Value.Area;
        }

        public enum ScreenShotTarget
        {
            Unknown,
            VirtualScreen,
            Screen,
            Desktop,
            Window
        }

        public enum PictureOrMovie
        {
            Unknown,
            Picture,
            Movie,
            Film,
        }

        public void Dispose()
        {
            _disposables?.Dispose();
            WindowInfos?.Dispose();
            MainWorkbenchVM?.Dispose();
            Source?.Dispose();
            Target?.Dispose();
            OnlyClientArea?.Dispose();
            CapturingType?.Dispose();
            SelectedScreen?.Dispose();
            Processes?.Dispose();
            SelectedProcess?.Dispose();
            SelectedWindowInfo?.Dispose();
            SelectScreenCommand?.Dispose();
            SelectProcessCommand?.Dispose();
            SelectWindowCommand?.Dispose();
        }
    }
}
