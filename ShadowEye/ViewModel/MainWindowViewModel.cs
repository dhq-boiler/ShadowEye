

using Microsoft.Win32;
using OpenCvSharp;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using ShadowEye.Model;
using ShadowEye.View;
using ShadowEye.View.Dialogs;
using System;
using System.IO;
using System.Reactive.Disposables;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;

namespace ShadowEye.ViewModel
{
    public class MainWindowViewModel : CommandSink, IDisposable
    {
        private CompositeDisposable disposables = new CompositeDisposable();
        private MainWorkbenchViewModel _imageContainerVM;
        private SubWorkbenchViewModel _subWorkbenchVM;
        private MainWindow _mainWindow;
        private bool disposedValue;

        public ReactiveCommand PasteCommand { get; } = new ReactiveCommand();

        public MainWindowViewModel(MainWindow mainwindow)
        {
            MainWindow = mainwindow;
            SetMainWindowTitle();
            SubWorkbenchVM = new SubWorkbenchViewModel(this);
            ImageContainerVM = new MainWorkbenchViewModel(this);
            ImageContainerVM.TabChanged += SubWorkbenchVM.ChangeSourceBySwitchTab;
            RegisterCommands();
        }

        private void SetMainWindowTitle()
        {
#pragma warning disable CS0162, IDE0035
            if (ThisAssembly.Git.Branch == "master" || ThisAssembly.Git.Branch == "main")
            {
                MainWindow.Title = $"ShadowEye {ThisAssembly.Git.BaseTag}";
            }
            if (ThisAssembly.Git.Branch == "develop")
            {
                MainWindow.Title = $"ShadowEye {ThisAssembly.Git.Tag}";
            }
#pragma warning restore CS0162, IDE0035
        }

        private void RegisterCommands()
        {
            base.RegisterCommand(RoutedCommands.FileOpenCommand, (p) => true, (p) =>
            {
                this.OpenFileDialogOpen();
            });
            base.RegisterCommand(RoutedCommands.CameraOpenCommand, (p) => true, (p) =>
            {
                this.CameraDialogOpen();
            });
            base.RegisterCommand(RoutedCommands.ScreenShotCommand, (p) => true, (p) =>
            {
                this.SelectScreenShotModeDialogOpen();
            });
            base.RegisterCommand(RoutedCommands.ImageProcessing_SubtractionCommand, (p) => true, (p) =>
            {
                this.SubtractionDialogOpen();
            });
            base.RegisterCommand(RoutedCommands.ImageProcessing_IntegrateChannelCommand, (p) => true, (p) =>
            {
                this.ChannelIntegrationDialogOpen();
            });
            base.RegisterCommand(RoutedCommands.ImageProcessing_ThresholdingCommand, (p) => true, (p) =>
            {
                this.ThresholdingDialogOpen();
            });
            base.RegisterCommand(RoutedCommands.ImageProcessing_ScalingCommand, (p) => true, (p) =>
            {
                this.ScalingDialogOpen();
            });
            base.RegisterCommand(RoutedCommands.ImageProcessing_MultiplicationCommand, (p) => true, (p) =>
            {
                this.MultiplicationDialogOpen();
            });
            base.RegisterCommand(RoutedCommands.ImageProcessing_ColorConversionCommand, (p) => true, (p) =>
            {
                this.ColorConversionDialogOpen();
            });
            base.RegisterCommand(RoutedCommands.ImageProcessing_ExtractChannelCommand, (p) => true, (p) =>
            {
                this.ChannelExtractionDialogOpen();
            });
            base.RegisterCommand(RoutedCommands.Filter_EdgeExtractionCommand, (p) => true, (p) =>
            {
                this.EdgeExtractionDialogOpen();
            });
            PasteCommand.Subscribe(() =>
            {
                var bitmap = Clipboard.GetImage();
                var source = new ClipboardSource(bitmap);
                ImageContainerVM.AddOrActive(source);
            })
            .AddTo(disposables);
        }

        private void EdgeExtractionDialogOpen()
        {
            EdgeExtractionDialog dialog = new EdgeExtractionDialog();
            dialog.DataContext = new EdgeExtractionDialogViewModel(_imageContainerVM);
            dialog.ShowDialog();
        }

        private void ChannelExtractionDialogOpen()
        {
            ChannelExtractionDialog dialog = new ChannelExtractionDialog();
            dialog.DataContext = new ChannelExtractionDialogViewModel(_imageContainerVM);
            dialog.ShowDialog();
        }

        private void ColorConversionDialogOpen()
        {
            ColorConversionDialog dialog = new ColorConversionDialog();
            dialog.DataContext = new ColorConversionDialogViewModel(_imageContainerVM);
            dialog.ShowDialog();
        }

        private void MultiplicationDialogOpen()
        {
            MultiplicationDialog dialog = new MultiplicationDialog();
            dialog.DataContext = new MultiplicationDialogViewModel(_imageContainerVM);
            dialog.ShowDialog();
        }

        private void ScalingDialogOpen()
        {
            ScalingDialog dialog = new ScalingDialog();
            dialog.DataContext = new ScalingDialogViewModel(_imageContainerVM);
            dialog.ShowDialog();
        }

        private void SelectScreenShotModeDialogOpen()
        {
            var vm = new ScreenShotDialogViewModel(_imageContainerVM);
            var dialog = new ScreenShotDialog
            {
                DataContext = vm
            };
            vm.Dialog = dialog;
            dialog.ShowDialog();
        }

        private void ThresholdingDialogOpen()
        {
            ThresholdingDialog dialog = new ThresholdingDialog();
            dialog.DataContext = new ThresholdingDialogViewModel(_imageContainerVM);
            dialog.ShowDialog();
        }

        private void ChannelIntegrationDialogOpen()
        {
            ChannelIntegrationDialog dialog = new ChannelIntegrationDialog();
            dialog.DataContext = new ChannelIntegrationDialogViewModel(_imageContainerVM);
            dialog.ShowDialog();
        }

        private void SubtractionDialogOpen()
        {
            SubtractionDialog dialog = new SubtractionDialog();
            dialog.DataContext = new SubtractionDialogViewModel(ImageContainerVM);
            dialog.ShowDialog();
        }

        private void CameraDialogOpen()
        {
            try
            {
                SelectCameraDialog dialog = new SelectCameraDialog();
                if (dialog.ShowDialog() == true)
                {
                    CameraSource cam = CameraSource.CreateInstance(dialog.SelectedCameraNumber, dialog.SelectedCameraDeviceName, dialog.SelectedResolution.Width, dialog.SelectedResolution.Height);
                    ImageContainerVM.AddOrActive(cam);
                }
            }
            catch (COMException e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void OpenFileDialogOpen()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;
            var b = dialog.ShowDialog();
            if (b.HasValue && b.Value)
            {
                foreach (var filename in dialog.FileNames)
                {
                    if (Path.GetExtension(filename) == ".gif")
                    {
                        var source = new FilmSource(filename);
                        using (VideoCapture capture = new VideoCapture(filename))
                        {
                            while (capture.IsOpened())
                            {
                                Mat mat = new Mat();

                                if (capture.Read(mat))
                                {
                                    source.Frames.Add(new Tuple<Mat, TimeSpan>(mat, TimeSpan.FromSeconds(1d / capture.Fps)));
                                }
                                else
                                {
                                    break;
                                }
                            }
                            source.ChannelType = libimgengCore.ChannelType.BGR24;
                        }
                        ImageContainerVM.AddOrActive(source);
                    }
                    else if (Path.GetExtension(filename) == ".mp4")
                    {
                        var source = new FilmSource(filename);
                        using (VideoCapture capture = new VideoCapture(filename))
                        {
                            while (capture.IsOpened())
                            {
                                Mat mat = new Mat();

                                if (capture.Read(mat))
                                {
                                    source.Frames.Add(new Tuple<Mat, TimeSpan>(mat, TimeSpan.FromSeconds(1d / capture.Fps)));
                                }
                                else
                                {
                                    break;
                                }
                            }
                            source.ChannelType = libimgengCore.ChannelType.BGR24;
                        }
                        ImageContainerVM.AddOrActive(source);
                    }
                    else
                    {
                        ImageContainerVM.AddOrActive(new FileSource(filename));
                    }
                }
            }
        }

        public MainWorkbenchViewModel ImageContainerVM
        {
            get { return _imageContainerVM; }
            set { SetProperty<MainWorkbenchViewModel>(ref _imageContainerVM, value, "ImageContainerVM"); }
        }

        public SubWorkbenchViewModel SubWorkbenchVM
        {
            get { return _subWorkbenchVM; }
            set { SetProperty<SubWorkbenchViewModel>(ref _subWorkbenchVM, value, "SubWorkbenchVM"); }
        }

        public MainWindow MainWindow
        {
            get { return _mainWindow; }
            set { SetProperty<MainWindow>(ref _mainWindow, value, "MainWindow"); }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    disposables.Dispose();
                }

                disposables = null;
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
