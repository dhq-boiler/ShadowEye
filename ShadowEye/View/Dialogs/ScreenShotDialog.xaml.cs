

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
        //private ScreenShotDialogViewModel VM => DataContext as ScreenShotDialogViewModel;

        public ScreenShotDialog()
        {
            InitializeComponent();
        }

        //private void ShootButton_Click(object sender, RoutedEventArgs e)
        //{
        //    VM.AddComputeTab();
        //    DialogResult = true;
        //}

        //private void CancelButton_Click(object sender, RoutedEventArgs e)
        //{
        //    DialogResult = false;
        //}

        //private void Window_Loaded(object sender, RoutedEventArgs e)
        //{
        //    VM.Initialize();
        //    VM.Source.Value.HowToUpdate.Request();
        //}

        //private void Window_Closed(object sender, EventArgs e)
        //{
        //    VM.Source.Value.HowToUpdate.RequestAccomplished();
        //}

        //private void ComboBox_SelectScreen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (VM.Source.Value.Area is ScreenShotScreen)
        //    {
        //        VM.SelectedScreen.Value = (ComboBox_SelectScreen.SelectedItem as ScreenOption).Target;
        //        (VM.Source.Value.Area as ScreenShotScreen).SelectedScreen = VM.SelectedScreen.Value;
        //        if (VM.Source.Value.HowToUpdate is StaticUpdater)
        //        {
        //            VM.Source.Value.UpdateImage();
        //        }
        //    }
        //}

        //private void ComboBox_SelectProcess_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    VM.Source.Value.Area = new ScreenShotWindow();
        //    VM.SelectedProcess.Value = ComboBox_SelectProcess.SelectedItem as ProcessItem;

        //    if (VM.Source.Value.Area is ScreenShotWindow)
        //    {
        //        (VM.Source.Value.Area as ScreenShotWindow).SelectedProcess = VM.SelectedProcess.Value?.Process;
        //        (VM.Source.Value.Area as ScreenShotWindow).OnlyClientArea = VM.OnlyClientArea.Value;
        //    }

        //    if (VM.Source.Value.HowToUpdate is StaticUpdater)
        //    {
        //        VM.Source.Value.UpdateImage();
        //    }

        //    VM.UpdateWindowInfos();

        //    if (VM.Source.Value.Area is ScreenShotWindow)
        //    {
        //        VM.SelectWindowHandle();
        //    }
        //}

        //private void ComboBox_SelectProcess_DropDownOpened(object sender, EventArgs e)
        //{
        //    VM.UpdateProcesses();
        //}

        //private void SaveFileButton_Click(object sender, RoutedEventArgs e)
        //{
        //    VM.MainWorkbenchVM.Value.SaveAsDialogOpen(VM.Source.Value);
        //}

        //private void ComboBox_SelectWindow_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (VM.Source.Value.Area is ScreenShotWindow && VM.SelectedWindowInfo.Value is not null)
        //    {
        //        (VM.Source.Value.Area as ScreenShotWindow).SelectedProcess = VM.SelectedProcess.Value?.Process;
        //        (VM.Source.Value.Area as ScreenShotWindow).OnlyClientArea = VM.OnlyClientArea.Value;
        //        (VM.Source.Value.Area as ScreenShotWindow).SelectedWindowHandle = VM.SelectedWindowInfo.Value.WindowHandle;
        //        if (VM.Source.Value.HowToUpdate is StaticUpdater)
        //        {
        //            VM.Source.Value.UpdateImage();
        //        }
        //    }
        //}

        //private void ComboBox_SelectWindow_DropDownOpened(object sender, EventArgs e)
        //{
        //}

        //private void ComboBox_SelectProcess_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        //{
        //    bool enable = (bool)e.NewValue;
        //    if (!enable)
        //    {
        //        VM.SelectedProcess.Value = null;
        //    }
        //}

    }
}
