

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using ShadowEye.Model;
using ShadowEye.Utils;

namespace ShadowEye.ViewModel
{
    public class ScreenShotDialogViewModel : Notifier
    {
        private ScreenShotTarget _Target;
        private bool _OnlyClientArea;
        private ScreenShotDialogViewModel.PictureOrMovie _CapturingType;
        private ScreenShotSource _ScreenShotSource;
        private List<ProcessItem> _Processes;
        private ProcessItem _SelectedProcess;
        private ProcessItem.WindowInfo[] _WindowInfos;
        private ProcessItem.WindowInfo _SelectedWindowInfo;
        private Screen _SelectedScreen;
        private MainWorkbenchViewModel _MainWorkbenchVM;
        private static int s_createCount;

        public ScreenShotDialogViewModel(MainWorkbenchViewModel imageContainerVM)
        {
            MainWorkbenchVM = imageContainerVM;
            _ScreenShotSource = new ScreenShotSource("ScreenShot");
            PropertyChanged += ScreenShotDialogViewModel_PropertyChanged;
            Target = ScreenShotTarget.VirtualScreen; //初期状態はVirtualScreen選択
            CapturingType = PictureOrMovie.Picture; //初期状態はPicture選択
            _ScreenShotSource.UpdateImage();
            UpdateProcesses();
        }

        private void ScreenShotDialogViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Target":
                    SwitchTarget();
                    _ScreenShotSource.UpdateImage();
                    break;
                case "OnlyClientArea":
                    if (_ScreenShotSource.Area is ScreenShotWindow)
                    {
                        var sss = _ScreenShotSource.Area as ScreenShotWindow;
                        sss.OnlyClientArea = OnlyClientArea;
                        _ScreenShotSource.UpdateImage();
                    }
                    break;
                case "CapturingType":
                    SwitchCapturingType();
                    _ScreenShotSource.UpdateImage();
                    break;
            }
        }

        private void SwitchCapturingType()
        {
            _ScreenShotSource.HowToUpdate.ForceStop();

            switch (CapturingType)
            {
                case PictureOrMovie.Picture:
                    _ScreenShotSource.HowToUpdate = new StaticUpdater(_ScreenShotSource);
                    break;
                case PictureOrMovie.Movie:
                    _ScreenShotSource.HowToUpdate = new DynamicUpdater(_ScreenShotSource);
                    break;
                case PictureOrMovie.Film:
                    _ScreenShotSource.HowToUpdate = new ManualUpdater(_ScreenShotSource);
                    break;
            }

            _ScreenShotSource.HowToUpdate.Request();
        }

        private void SwitchTarget()
        {
            _ScreenShotSource.HowToUpdate.RequestAccomplished();

            switch (Target)
            {
                case ScreenShotTarget.VirtualScreen:
                    _ScreenShotSource.Area = new ScreenShotVirtualScreen();
                    break;
                case ScreenShotTarget.Screen:
                    _ScreenShotSource.Area = new ScreenShotScreen(SelectedScreen);
                    break;
                case ScreenShotTarget.Desktop:
                    _ScreenShotSource.Area = new ScreenShotDesktop();
                    break;
                case ScreenShotTarget.Window:
                    _ScreenShotSource.Area = new ScreenShotWindow(SelectedProcess != null ? SelectedProcess.Process : null);
                    break;
            }

            _ScreenShotSource.HowToUpdate.Request();
        }

        public MainWorkbenchViewModel MainWorkbenchVM
        {
            get { return _MainWorkbenchVM; }
            set { SetProperty<MainWorkbenchViewModel>(ref _MainWorkbenchVM, value, "MainWorkbenchVM"); }
        }

        public ScreenShotSource Source
        {
            get { return _ScreenShotSource; }
            set { SetProperty<ScreenShotSource>(ref _ScreenShotSource, value, "Source"); }
        }

        public ScreenShotTarget Target
        {
            get { return _Target; }
            set
            {
                if (value == ScreenShotTarget.Unknown) return;
                SetProperty<ScreenShotTarget>(ref _Target, value, "Target");
            }
        }

        public bool OnlyClientArea
        {
            get { return _OnlyClientArea; }
            set { SetProperty<bool>(ref _OnlyClientArea, value, "OnlyClientArea"); }
        }

        public PictureOrMovie CapturingType
        {
            get { return _CapturingType; }
            set { SetProperty<PictureOrMovie>(ref _CapturingType, value, "CapturingType"); }
        }

        public ScreenOption[] Monitors
        {
            get { return ScreenOption.CreateArray(); }
        }

        public Screen SelectedScreen
        {
            get { return _SelectedScreen; }
            set { SetProperty<Screen>(ref _SelectedScreen, value, "SelectedScreen"); }
        }

        public ProcessItem[] Processes
        {
            get
            {
                return _Processes.ToArray();
            }
        }

        public ProcessItem SelectedProcess
        {
            get { return _SelectedProcess; }
            set { SetProperty<ProcessItem>(ref _SelectedProcess, value, "SelectedProcess", "WindowInfos", "IsSelectedProcess"); }
        }

        public ProcessItem.WindowInfo[] WindowInfos
        {
            get
            {
                return _WindowInfos?.ToArray();
            }
        }

        public ProcessItem.WindowInfo SelectedWindowInfo
        {
            get { return _SelectedWindowInfo; }
            set { SetProperty(ref _SelectedWindowInfo, value, "SelectedWindowInfo"); }
        }

        public bool IsSelectedProcess
        {
            get { return Target == ScreenShotTarget.Window && SelectedProcess != null; }
        }

        public void UpdateProcesses()
        {
            _Processes = new List<ProcessItem>();
            foreach (var p in Process.GetProcesses())
            {
                if (p.MainWindowTitle.Count() == 0) continue;
                _Processes.Add(new ProcessItem(p));
            }
            OnPropertyChanged("Processes");
        }

        public void UpdateWindowInfos()
        {
            if (SelectedProcess == null)
                return;
            var wis = ScreenShotWindow.EnumWindows(SelectedProcess.Process);
            _WindowInfos = wis.ToArray();
            OnPropertyChanged("WindowInfos");
        }

        public void SelectMainWindowHandle()
        {
            if (SelectedProcess == null)
                return;
            SelectedWindowInfo = WindowInfos.Where(a => a.WindowHandle.Equals(SelectedProcess.Process.MainWindowHandle)).Single();
        }

        public void AddComputeTab()
        {
            var source = (AnalyzingSource)new DummySource();
            source = _ScreenShotSource;
            source.Name = string.Format("ScreenShot-{0}", ++s_createCount);
            if (CapturingType == PictureOrMovie.Film)
            {
                source = new FilmSource($"Film-{++s_createCount}", source);
                (source as FilmSource).StartRecoding();
            }
            MainWorkbenchVM.AddOrActive(source);
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
    }
}
