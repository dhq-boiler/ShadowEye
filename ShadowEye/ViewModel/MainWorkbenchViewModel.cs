

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using ShadowEye.Model;
using ShadowEye.Utils;
using ShadowEye.View.Controls;

namespace ShadowEye.ViewModel
{
    public class MainWorkbenchViewModel : CommandSink
    {
        private int _selectedIndex;
        private Dictionary<string, AnalyzingSource> _tab_collection;
        private ImageViewModel _selectedImageVM;
        public MainWindowViewModel MainWindowVM { get; private set; }

        public MainWorkbenchViewModel(MainWindowViewModel mainwindowVM)
        {
            _tab_collection = new Dictionary<string, AnalyzingSource>();
            Tabs = new ObservableCollection<ImageViewModel>();
            this.MainWindowVM = mainwindowVM;
            RegisterCommands();
        }

        private void RegisterCommands()
        {
            base.RegisterCommand(RoutedCommands.SaveAsCommand, (object p) => true, (object p) =>
            {
                this.SaveAsDialogOpen(p as ImageViewModel);
            });
            base.RegisterCommand(RoutedCommands.CloseThisTabCommand, (object p) => true, (object p) =>
            {
                this.CloseTab(p as ImageViewModel);
            });
            base.RegisterCommand(RoutedCommands.StoreDiscadedImageCommand, (object p) => true, (object p) =>
            {
                ImageViewModel ivm = p as ImageViewModel;
                AnalyzingSource asrc = ivm.Source as AnalyzingSource;
                this.MainWindowVM.ImageContainerVM.AddOrActive(new DiscadedSource(asrc));
            });
        }

        internal void SaveAsDialogOpen(AnalyzingSource source)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.FileName = source.Name;
            dialog.DefaultExt = "bmp";
            dialog.Filter = "Microsoft Windows Bitmap Image|*.bmp|All Files|*.*";
            if (dialog.ShowDialog() == true)
            {
                source.Mat.SaveImage(dialog.FileName);
            }
        }

        private void SaveAsDialogOpen(ImageViewModel ivm)
        {
            if (ivm == null)
            {
                Trace.WriteLine("ivm is null");
                return;
            }

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.FileName = ivm.Source.Name;
            dialog.DefaultExt = "bmp";
            dialog.Filter = "Microsoft Windows Bitmap Image|*.bmp|All Files|*.*";
            if (dialog.ShowDialog() == true)
            {
                ivm.Source.Mat.SaveImage(dialog.FileName);
            }
        }

        public void AddOrActive(AnalyzingSource source)
        {
            //Active
            for (int i = 0; i < Tabs.Count; ++i)
            {
                ImageViewModel ivm = Tabs[i];

                if (ivm.Source.Equals(source))
                {
                    SelectedTabIndex = i;
                    return;
                }
            }

            //Add
            ImageViewModel item = new ImageViewModel(source, this)
            {
                Header = source.Name
            };
            source.SourceUpdated += MainWindowVM.SubWorkbenchVM.ChangeImageSource;
            Tabs.Add(item);
            SelectedTabIndex = Tabs.Count - 1;
            OnPropertyChanged("Tabs");
        }

        public void CloseTab(ImageViewModel remove)
        {
            if (Tabs.Contains(remove))
            {
                Tabs.Remove(remove);
                remove.Source.Dispose();
            }
            OnPropertyChanged("Tabs");
        }

        public int SelectedTabIndex
        {
            get { return _selectedIndex; }
            set
            {
                SetProperty<int>(ref _selectedIndex, value, "SelectedTabIndex");
                MainWindowVM.MainWindow.MainWorkbench.TabControl_Workbench.Focus();
            }
        }

        public ImageViewModel SelectedImageVM
        {
            get { return _selectedImageVM; }
            set
            {
                SetProperty<ImageViewModel>(ref _selectedImageVM, value, "SelectedImageVM");
                OnTabChanged(this, new EventArgs());
            }
        }

        public ObservableCollection<ImageViewModel> Tabs { get; set; }

        public event EventHandler TabChanged;
        protected virtual void OnTabChanged(object sender, EventArgs e)
        {
            if (TabChanged != null)
                TabChanged(sender, e);
        }
    }
}
