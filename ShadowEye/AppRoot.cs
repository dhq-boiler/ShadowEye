

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShadowEye.ViewModel;

namespace ShadowEye
{
    public sealed class AppRoot
    {
        public static readonly AppRoot Instance = new AppRoot();

        private ObservableCollection<ImageViewModel> _tabs;

        public AppRoot()
        {
            _tabs = new ObservableCollection<ImageViewModel>();
        }

        public int SelectedTabIndex { get; set; }
        public MainWindowViewModel MainWindowVM { get; set; }
    }
}
