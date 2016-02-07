using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Windows.Devices.Geolocation;
using System.Device.Location;
using Common.Utils;
using Common;
using Common.Tables.Disaster;

namespace WindowsPhone.Views
{
    public partial class AddNewCenterPage : PhoneApplicationPage
    {
        Geolocator geolocator;
        private string _disasterId;

        public AddNewCenterPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            SystemTray.IsVisible = true;
            _disasterId = NavigationContext.QueryString["DisasterId"];

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

            var disaster = new Center
            {
                Name = txtName.Text,
                City = txtCity.Text,
                DisasterId = _disasterId,
                Latitude = map.Center.Latitude,
                Longitude = map.Center.Longitude,
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
            errorCity.Visibility = Visibility.Collapsed;
            errorName.Visibility = Visibility.Collapsed;
        }

        private bool CheckInput()
        {
            var everythingAllRight = true;

            if (txtCity.Text.IsNullOrEmptyOrWhiteSpace())
            {
                errorCity.Visibility = Visibility.Visible;
                everythingAllRight = false;
            }
            if (txtName.Text.IsNullOrEmptyOrWhiteSpace())
            {
                errorName.Visibility = Visibility.Visible;
                everythingAllRight = false;
            }

            return everythingAllRight;
        }
    }
}