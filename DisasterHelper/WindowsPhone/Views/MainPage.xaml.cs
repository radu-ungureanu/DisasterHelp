using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using WindowsPhone.Utils;
using Common;
using Common.Tables.Disaster;
using Windows.Devices.Geolocation;
using Microsoft.Phone.Controls.Maps;
using System.Device.Location;

namespace WindowsPhone.Views
{
    public partial class MainPage : PhoneApplicationPage
    {
        Geolocator geolocator;

        public MainPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            SystemTray.IsVisible = true;
            panorama.Title = "Hi, " + App.User.Fullname;

            while (NavigationService.CanGoBack)
            {
                try
                {
                    NavigationService.RemoveBackEntry();
                }
                catch{}
            }

            try
            {
                var items = await Service.GetTable<Disaster>()
                    .Where(t => t.Active == DisasterStatusType.Active)
                    .ToListAsync();

                if (items.Count == 0)
                {
                    txtNoDisasters.Visibility = Visibility.Visible;
                    txtNoMyDisasters.Visibility = Visibility.Visible;
                    txtNoAttendDisasters.Visibility = Visibility.Visible;
                }
                else
                {
                    txtNoDisasters.Visibility = Visibility.Collapsed;
                    txtNoMyDisasters.Visibility = Visibility.Collapsed;
                    txtNoAttendDisasters.Visibility = Visibility.Collapsed;

                    var attending = await Service.GetTable<DisasterAttend>().Where(t => t.ParticipantId == App.User.Id).ToListAsync();
                    var attendingDisasters = (from item in items
                                              let ids = attending.Select(t => t.DisasterId)
                                              where ids.Contains(item.Id)
                                              select item).ToList();
                    if (attendingDisasters.Count == 0)
                        txtNoAttendDisasters.Visibility = Visibility.Visible;
                    else
                        lstAttendDisasters.ItemsSource = attendingDisasters;

                    var myDisasters = items.Where(t => t.InitiatorId == App.User.Id);
                    if (myDisasters.Count() == 0)
                        txtNoMyDisasters.Visibility = Visibility.Visible;
                    else
                        lstMyDisasters.ItemsSource = myDisasters;


                    var dis = items.Where(t => t.InitiatorId != App.User.Id);
                    lstDisasters.ItemsSource = dis;
                    if (dis.Count() == 0)
                        txtNoDisasters.Visibility = Visibility.Visible;


                    foreach (var item in items)
                    {
                        var length = item.Description.Length > 20 ? 20 : item.Description.Length;
                        var pin = new Pushpin()
                        {
                            Location = new GeoCoordinate(item.Latitude, item.Longitude),
                            Content = item.Description.Substring(0, length),
                            Tag = string.Format("{0}?DisasterId={1}&DisasterName={2}", PageConstants.ViewDisasterDetailsPage, item.Id, item.Country)
                        };
                        pin.Tap += pin_Tap;
                        map.Children.Add(pin);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something went wrong!");
            }

            geolocator = new Geolocator();
            geolocator.DesiredAccuracy = PositionAccuracy.High;
            geolocator.DesiredAccuracyInMeters = 100;
            var location = await geolocator.GetGeopositionAsync();
            map.Center = new GeoCoordinate(location.Coordinate.Latitude, location.Coordinate.Longitude);
        }

        void pin_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var disaster = sender as Pushpin;
            var uri = new Uri((string)disaster.Tag, UriKind.RelativeOrAbsolute);
            NavigationService.Navigate(uri);
        }

        private void LogoutButton_Click(object sender, EventArgs e)
        {
            var settings = new SettingsHelper();
            settings.DeleteAll();

            var uri = new Uri(PageConstants.LoginPage, UriKind.RelativeOrAbsolute);
            NavigationService.Navigate(uri);
        }

        private void UpdateProfileButton_Click(object sender, EventArgs e)
        {
            var uri = new Uri(PageConstants.UpdateAccountPage, UriKind.RelativeOrAbsolute);
            NavigationService.Navigate(uri);
        }

        private void AddDisasterButton_Click(object sender, EventArgs e)
        {
            var uri = new Uri(PageConstants.AddNewDisasterPage, UriKind.RelativeOrAbsolute);
            NavigationService.Navigate(uri);
        }

        private void Disaster_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var disaster = (sender as StackPanel).DataContext as Disaster;
            var path = string.Format("{0}?DisasterId={1}&DisasterName={2}",
                PageConstants.ViewDisasterDetailsPage,
                disaster.Id,
                disaster.Country);
            var uri = new Uri(path, UriKind.RelativeOrAbsolute);
            NavigationService.Navigate(uri);
        }
    }
}