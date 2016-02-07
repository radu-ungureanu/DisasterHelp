using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Common.Tables.Disaster;
using Common;
using Common.Utils;
using Microsoft.Phone.Tasks;
using System.Windows.Media.Imaging;
using System.IO;
using Windows.ApplicationModel;

namespace WindowsPhone.Views
{
    public partial class AddNewPatient : PhoneApplicationPage
    {
        private string _centerId;
        private byte[] _image;

        public AddNewPatient()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SystemTray.IsVisible = true;
            _centerId = NavigationContext.QueryString["CenterId"];
            cmbStatus.ItemsSource = PacientStatusTypeExtensions.GetPacientStatusTypes();
            cmbBlood.ItemsSource = BloodTypeExtensions.GetBloodTypes();

            if (e.NavigationMode == NavigationMode.New)
            {
                var defaultImage = File.OpenRead("Assets/Resources/person_icon.png");
                _image = ImageHelper.GetFileBytes(defaultImage);
                var bmp = new BitmapImage();
                bmp.SetSource(ImageHelper.GetFileStream(_image));
                img.Source = bmp;
            }
        }

        private async void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            DisableErrors();
            if (!CheckInput()) return;

            var pacient = new Pacient
            {
               BloodType = (BloodType)cmbBlood.SelectedItem,
               CenterId = _centerId,
               Description = txtDescription.Text,
               Image = _image,
               Name = txtName.Text,
               Status = (PacientStatusType)cmbStatus.SelectedItem,
            };
            var error = false;
            try
            {
                await Service.InsertItemAsync(pacient);
            }
            catch (Exception ex)
            {
                error = true;
            }

            if (error)
            {
                MessageBox.Show("Something went wrong!");
                return;
            }

            NavigationService.GoBack();
        }

        private void DisableErrors()
        {
            errorDescription.Visibility = Visibility.Collapsed;
        }

        private bool CheckInput()
        {
            var everythingAllRight = true;

            if (txtDescription.Text.IsNullOrEmptyOrWhiteSpace())
            {
                errorDescription.Visibility = Visibility.Visible;
                everythingAllRight = false;
            }

            return everythingAllRight;
        }

        private void OpenImage_Click(object sender, RoutedEventArgs e)
        {
            CameraCaptureTask camera = new CameraCaptureTask();
            camera.Completed += camera_Completed;
            camera.Show();
        }

        void camera_Completed(object sender, PhotoResult e)
        {
            _image = ImageHelper.GetFileBytes(e.ChosenPhoto);
            var bmp = new BitmapImage();
            bmp.SetSource(ImageHelper.GetFileStream(_image));
            img.Source = null;
            img.Source = bmp;
        }
    }
}