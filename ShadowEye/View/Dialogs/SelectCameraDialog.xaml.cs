

using libcamenmCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ShadowEye.View.Dialogs
{
    /// <summary>
    /// Interaction logic for SelectCameraDialog.xaml
    /// </summary>
    public partial class SelectCameraDialog : Window
    {
        public SelectCameraDialog()
        {
            InitializeComponent();

            try
            {
                _enumDeviceList = DeviceEnumerator.EnumVideoInputDevice();
            }
            catch (COMException)
            {
                throw;
            }

            this.DataContext = _enumDeviceList;
        }

        private void Button_OK_Click(object sender, RoutedEventArgs e)
        {
            if (ListBox_CameraDevices.SelectedIndex == -1)
            {
                MessageBox.Show(Properties.Resource_Localization_Messages.NotSelectedCameraNumber);
                return;
            }
            _selectedCameraNumber = ListBox_CameraDevices.SelectedIndex;
            _selectedCameraDeviceName = _enumDeviceList.Cast<string>().ElementAt(_selectedCameraNumber);
            DialogResult = true;
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void ListBoxItem_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListBoxItem item = sender as ListBoxItem;
            if (item != null)
            {
                _selectedCameraNumber = ListBox_CameraDevices.SelectedIndex;
                _selectedCameraDeviceName = _enumDeviceList.Cast<string>().ElementAt(_selectedCameraNumber);
                if (!item.Content.Equals(_selectedCameraDeviceName))
                {
                    throw new Exception(Properties.Resource_Localization_Messages.CameraNumberDoNotCorrespondToDeviceName);
                }
                DialogResult = true;
            }
        }

        private IEnumerable<string> _enumDeviceList;

        private int _selectedCameraNumber;
        public int SelectedCameraNumber { get { return _selectedCameraNumber; } }

        private string _selectedCameraDeviceName;
        public string SelectedCameraDeviceName { get { return _selectedCameraDeviceName; } }
    }
}
