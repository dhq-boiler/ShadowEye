

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using ShadowEye.Model;
using ShadowEye.Utils;

namespace ShadowEye.ViewModel
{
    public class ImageViewModel : Notifier
    {
        private CompositeDisposable disposables = new CompositeDisposable();
        public ImageViewModel(AnalyzingSource source, MainWorkbenchViewModel parent)
        {
            VerifyArgument(source, "source");
            VerifyArgument(parent, "parent");
            this.Source = source;
            ImageContainerVM = parent;
            SelectionStartCommand.Subscribe(() =>
            {
                var filmsource = (source as FilmSource);
                filmsource.SelectionEnable.Value = true;
                filmsource.SelectionStart.Value = filmsource.CurrentIndex.Value;
                if (filmsource.SelectionEnd.Value == 0)
                {
                    filmsource.SelectionEnd.Value = filmsource.Frames.Count();
                }
            })
            .AddTo(disposables);
            SelectionEndCommand.Subscribe(() =>
            {
                var filmsource = (source as FilmSource);
                filmsource.SelectionEnable.Value = true;
                filmsource.SelectionEnd.Value = filmsource.CurrentIndex.Value;
                if (filmsource.SelectionStart.Value == 0)
                {
                    filmsource.SelectionStart.Value = 0;
                }
            })
            .AddTo(disposables);
            SelectionCancelCommand.Subscribe(() =>
            {
                var filmsource = (source as FilmSource);
                filmsource.SelectionEnable.Value = false;
                filmsource.SelectionStart.Value = 0;
                filmsource.SelectionEnd.Value = 0;
            })
            .AddTo(disposables);
        }

        public void CloseThisTab()
        {
            VerifyArgument(ImageContainerVM, "ImageContainerVM");
            ImageContainerVM.CloseTab(this);
        }

        private static void VerifyArgument(object arg, string argName)
        {
            if (arg == null)
            {
                throw new ArgumentNullException(argName);
            }
        }

        public ReactiveCommand SelectionStartCommand { get; } = new ReactiveCommand();
        public ReactiveCommand SelectionEndCommand { get; } = new ReactiveCommand();
        public ReactiveCommand SelectionCancelCommand { get; } = new ReactiveCommand();
        public AnalyzingSource Source { get; set; }
        public string Header { get; set; }
        public MainWorkbenchViewModel ImageContainerVM { get; set; }
    }
}
