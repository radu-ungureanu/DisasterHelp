using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Device.Location;
using Windows.Devices.Geolocation;
using Common.Utils;
using Common.Tables.Disaster;
using Common;

namespace WindowsPhone.Views
{
    public partial class AddNewDisasterPage : PhoneApplicationPage
    {
        Geolocator geolocator;

        public AddNewDisasterPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            SystemTray.IsVisible = true;

            geolocator = new Geolocator();
            geolocator.DesiredAccuracy = PositionAccuracy.High;
            geolocator.DesiredAccuracyInMeters = 100;
            var location = await geolocator.GetGeopositionAsync();
            map.Center = new GeoCoordinate(location.Coordinate.Latitude, location.Coordinate.Longitude);
        }

        private async void SaveButton_Click(object sender, EventArgs e)
        {
            DisableErrors();
            if (!CheckInput()) return;

            var disaster = new Disaster
            {
                Active = DisasterStatusType.Active,
                Country = txtCountry.Text,
                Description = txtDescription.Text,
                InitiatorId = App.User.Id,
                InitiatorName = App.User.Fullname,
                Latitude = map.Center.Latitude,
                Longitude = map.Center.Longitude,
                RatingMinus = 0,
                RatingPlus = 0
            };
            var error = false;
            try
            {
                await Service.InsertItemAsync(disaster);
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

        void DisableErrors()
        {
            errorDescription.Visibility = Visibility.Collapsed;
            errorCountry.Visibility = Visibility.Collapsed;
        }

        private bool CheckInput()
        {
            var everythingAllRight = true;

            if (txtDescription.Text.IsNullOrEmptyOrWhiteSpace())
            {
                errorDescription.Visibility = Visibility.Visible;
                everythingAllRight = false;
            }
            if (txtCountry.Text.IsNullOrEmptyOrWhiteSpace())
            {
                errorCountry.Visibility = Visibility.Visible;
                everythingAllRight = false;
            }

            return everythingAllRight;
        }
    }
}