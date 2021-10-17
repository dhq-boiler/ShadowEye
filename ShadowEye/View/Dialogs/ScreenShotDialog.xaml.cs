

using System;
using System.Windows;
using System.Windows.Controls;
using ShadowEye.Model;
using ShadowEye.Utils;
using ShadowEye.ViewModel;

namespace ShadowEye.View.Dialogs
{
    /// <summary>
    /// Interaction logic for ScreenShotDialog.xaml
    /// </summary>
    public partial class ScreenShotDialog : Window
    {
        private ScreenShotDialogViewModel VM { get { return DataContext as ScreenShotDialogViewModel; } }

        public ScreenShotDialog()
        {
            InitializeComponent();
        }

        private void ShootButton_Click(object sender, RoutedEventArgs e)
        {
            VM.AddComputeTab();
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            VM.Source.HowToUpdate.Request();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            VM.Source.HowToUpdate.RequestAccomplished();
        }

        private void ComboBox_SelectScreen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VM.Source.Area is ScreenShotScreen)
            {
                VM.SelectedScreen = (ComboBox_SelectScreen.SelectedItem as ScreenOption).Target;
                (VM.Source.Area as ScreenShotScreen).SelectedScreen = VM.SelectedScreen;
                if (VM.Source.HowToUpdate is StaticUpdater)
                {
                    VM.Source.UpdateImage();
                }
            }
        }

        private void ComboBox_SelectProcess_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VM.Source.Area is ScreenShotWindow)
            {
                VM.SelectedProcess = ComboBox_SelectProcess.SelectedItem as ProcessItem;
                (VM.Source.Area as ScreenShotWindow).SelectedProcess = VM.SelectedProcess != null ? VM.SelectedProcess.Process : null;
                (VM.Source.Area as ScreenShotWindow).OnlyClientArea = VM.OnlyClientArea;
                if (VM.Source.HowToUpdate is StaticUpdater)
                {
                    VM.Source.UpdateImage();
                }

                VM.UpdateWindowInfos();
                VM.SelectMainWindowHandle();
            }
        }

        private void ComboBox_SelectProcess_DropDownOpened(object sender, EventArgs e)
        {
            VM.UpdateProcesses();
        }

        private void SaveFileButton_Click(object sender, RoutedEventArgs e)
        {
            VM.MainWorkbenchVM.SaveAsDialogOpen(VM.Source);
        }

        private void ComboBox_SelectWindow_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VM.Source.Area is ScreenShotWindow && VM.SelectedWindowInfo != null)
            {
                (VM.Source.Area as ScreenShotWindow).SelectedProcess = VM.SelectedProcess != null ? VM.SelectedProcess.Process : null;
                (VM.Source.Area as ScreenShotWindow).OnlyClientArea = VM.OnlyClientArea;
                (VM.Source.Area as ScreenShotWindow).SelectedWindowHandle = VM.SelectedWindowInfo.WindowHandle;
                if (VM.Source.HowToUpdate is StaticUpdater)
                {
                    VM.Source.UpdateImage();
                }
            }
        }

        private void ComboBox_SelectWindow_DropDownOpened(object sender, EventArgs e)
        {
        }

        private void ComboBox_SelectProcess_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            bool enable = (bool)e.NewValue;
            if (!enable)
            {
                VM.SelectedProcess = null;
            }
        }
    }
}
