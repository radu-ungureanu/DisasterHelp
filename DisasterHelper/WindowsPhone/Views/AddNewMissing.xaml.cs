using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Common.Utils;
using System.Windows.Media.Imaging;
using System.IO;
using Common.Tables.Disaster;
using Common;

namespace WindowsPhone.Views
{
    public partial class AddNewMissing : PhoneApplicationPage
    {
        private string _disasterId;
        private byte[] _image;

        public AddNewMissing()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SystemTray.IsVisible = true;
            _disasterId = NavigationContext.QueryString["DisasterId"];

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

            var missing = new Missing
            {
                DisasterId = _disasterId,
                Description = txtDescription.Text,
                Image = _image,
                Name = txtName.Text,
                Feedback = "",
                Found = false,
                UserPostingId = App.User.Id
            };
            var error = false;
            try
            {
                await Service.InsertItemAsync(missing);
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
            PhotoChooserTask chooser = new PhotoChooserTask();
            chooser.Completed += camera_Completed;
            chooser.Show();
        }

        void camera_Completed(object sender, PhotoResult e)
        {
            if (e.ChosenPhoto == null)
                return;
            _image = ImageHelper.GetFileBytes(e.ChosenPhoto);
            var bmp = new BitmapImage();
            bmp.SetSource(ImageHelper.GetFileStream(_image));
            img.Source = null;
            img.Source = bmp;
        }
    }
}